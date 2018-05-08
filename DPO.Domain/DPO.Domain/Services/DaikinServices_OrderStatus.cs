using System;
using System.Collections.Generic;
using System.Linq;
using DPO.Common;
using DPO.Data;

namespace DPO.Domain
{
    public partial class DaikinServices : BaseServices
    {
        #region Order Status Import and Update
        private void ProcessOrderStatusImport()
        {
            using (var erpClient = new ERPClient())
            {
                //Pull all orders from DC with Order status = 2 (Submitted)
                //Loop through each order in EDI850HDR and lookup in EDI850 using PO Key
                // If exist, update orders to 3(Awaiting CSR) and update DC Order Timestamp
                ProcessOrdersInSubmittedStatus(erpClient);

                //Pull all orders from DC with Order Status = 3 (Awaiting CSR)
                //Loop through each order in OECPLGP and update DC Order Timestamp,
                // DC order status to 6 (Picked) 
                ProcessOrdersInAwaitingCSRStatus(erpClient);

                //Pull all order with Order Status = 5(In Process)
                //Loop through each order and lookup in Mapics(??? Mahesh / Ashok) table
                //  Update DC Order ERPInvoiceNumber, ERPInvoiceDate, ERPShipDate, Timestamp
                ProcessOrdersInProcessStatus(erpClient);

                //Pull all order with Order Status = 6(Picked)
                //Loop through each order and lookup in MBDHREP table
                //If order number exists in MBDHREP and DHINST = 50 then Order status to 8(Invoiced)
                // Else If order number exists in MBDHREP and DHINST = 20 then Order status to 7(Shipped)
                // Update DC Order ERPInvoiceNumber, ERPInvoiceDate, ERPShipDate, Timestamp
                // Update DC Project as already done in code
                ProcessOrdersInPickedStatus(erpClient);
            }
        }

        private void ProcessOrdersInSubmittedStatus(ERPClient erpClient)
        {
            Console.WriteLine("Attempting to update orders in Submitted Status");

            var orderList = erpClient.CheckStatusForSubmittedOrdersAsync();

            if (orderList != null && orderList.Count() > 0)
            {
                Console.WriteLine($"Numbers of orders imported from EDI850HDR are {orderList?.Count()}");

                //Get all orders currently in statustypeId = 2
                var orderToUpdateList = Db.Orders?.Where(x => x.OrderStatusTypeId == 2);

                //If any record is found in DC that is in status 2
                if (orderToUpdateList != null && orderToUpdateList.Count() > 0)
                {
                    foreach (var order in orderList.ToList())
                    {
                        var orderToUpdate = orderToUpdateList?.FirstOrDefault(x => x.QuoteId == order.QuoteID);

                        if (orderToUpdate != null)
                        {
                            Console.WriteLine($"Order match found for QuoteId {order.QuoteID} in DC");

                            orderToUpdate.OrderStatusTypeId = 3; //update order status to awaiting csr
                            orderToUpdate.WebServiceImportStatus = "SubmittedOrdersUpdate";

                            UpdateAndLogOrdersInDC(orderToUpdate);
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("No orders came through from EDI850HDR.");
            }
        }

        private void ProcessOrdersInAwaitingCSRStatus(ERPClient erpClient)
        {
            Console.WriteLine("Attempting to update orders in Awaiting CSR Status");

            var orderList = erpClient.CheckStatusForAwaitingCSROrdersAsync();

            if (orderList != null && orderList.Count() > 0)
            {
                //Group the list as it might contain duplicates with different comments
                var groupedByOrderList = GetGroupedByOrderList(orderList);

                Console.WriteLine($"Numbers of orders imported from Orders table are {groupedByOrderList?.Count()}");

                //Get all orders currently in statustypeId = 3
                var orderToUpdateList = Db.Orders?.Where(x => x.OrderStatusTypeId == 3);

                //If any record is found in DC that is in status 3
                if (orderToUpdateList != null && orderToUpdateList.Count() > 0)
                {
                    foreach (var order in groupedByOrderList.ToList().Where(x => orderToUpdateList.Any(y => y.QuoteId == x.QuoteID)))
                    {
                        var orderToUpdate = orderToUpdateList?.FirstOrDefault(x => x.QuoteId == order.QuoteID);

                        if (orderToUpdate != null)
                        {
                            Console.WriteLine($"Order match found for  {orderToUpdate.OrderId} in DC");

                            orderToUpdate.ERPOrderNumber = order.OrderNumber;
                            orderToUpdate.ERPOrderDate = order.OrderDate;
                            orderToUpdate.ERPStatus = order.OrderStatus;
                            orderToUpdate.ERPComment = order.OrderComment;
                            orderToUpdate.OrderStatusTypeId = 5;
                            orderToUpdate.WebServiceImportStatus = "AwaitingCSROrdersUpdate";

                            UpdateAndLogOrdersInDC(orderToUpdate);

                            //Also update associated Project
                            var quote = Db.Quotes?.FirstOrDefault(x => x.QuoteId == orderToUpdate.QuoteId);
                            if (quote != null)
                            {
                                Console.WriteLine($"Project match found for  {quote.ProjectId} in DC");

                                UpdateProjectsToOpenOrderStatus(quote.ProjectId, order);
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("No orders came through from MBC6REP.");
            }
        }

        private void ProcessOrdersInProcessStatus(ERPClient erpClient)
        {
            Console.WriteLine("Attempting to update orders in In-Process status");

            var orderToUpdateList = Db.Orders?.Where(x => x.OrderStatusTypeId == 5);
            Console.WriteLine($"Number of orders from DC in status 5 are {orderToUpdateList?.Count()}");

            if (orderToUpdateList != null && orderToUpdateList.Count() > 0)
            {
                foreach (var item in orderToUpdateList)
                {
                    var result = erpClient.CheckStatusForInProcessOrdersAsync(item.ERPOrderNumber);
                    Console.WriteLine($"Sent request to ERP for Order Number - { item.ERPOrderNumber}");

                    if (result != null && result.ProjectID != 0)
                    {
                        var orderToUpdate = orderToUpdateList?.FirstOrDefault(x => x.QuoteId == result.QuoteID);

                        if (orderToUpdate != null)
                        {
                            Console.WriteLine($"Order match for - { orderToUpdate.OrderId }");
                            orderToUpdate.OrderStatusTypeId = 6;
                            orderToUpdate.WebServiceImportStatus = "InProcessOrdersUpdate";

                            UpdateAndLogOrdersInDC(orderToUpdate);
                        }
                    }
                }
            }
        }

        private void ProcessOrdersInPickedStatus(ERPClient erpClient)
        {
            Console.WriteLine("Attempting to update orders in Picked status");
            var orderToUpdateList = Db.Orders?.Where(x => x.OrderStatusTypeId == 6);

            Console.WriteLine($"Number of orders from DC in status 6 are {orderToUpdateList?.Count()}");

            if (orderToUpdateList != null && orderToUpdateList.Count() > 0)
            {
                foreach (var item in orderToUpdateList)
                {
                    var result = erpClient.CheckStatusForPickedOrdersAsync(item.ERPOrderNumber);
                    Console.WriteLine($"Sent request to ERP for Order Number - { item.ERPOrderNumber}");

                    if (result != null && result.QuoteID != 0)
                    {
                        var orderToUpdate = orderToUpdateList?.FirstOrDefault(x => x.QuoteId == result.QuoteID);

                        if (orderToUpdate != null)
                        {
                            Console.WriteLine($"Order match for - { orderToUpdate.OrderId }");
                            orderToUpdate.ERPInvoiceNumber = result.InvoiceNumber;
                            orderToUpdate.ERPInvoiceDate = result.InvoiceDate;
                            orderToUpdate.ERPShipDate = result.ShipmentDate;
                            orderToUpdate.OrderStatusTypeId = 7;
                            orderToUpdate.WebServiceImportStatus = "PickedOrdersUpdate";

                            UpdateAndLogOrdersInDC(orderToUpdate);

                            var quote = Db.Quotes?.FirstOrDefault(x => x.QuoteId == orderToUpdate.QuoteId);
                            if (quote != null)
                            {
                                UpdateProjectsToShippedStatus(quote.ProjectId, result);
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"No match found in Mapics for - { item.ERPOrderNumber} to confirm if the order is picked");
                    }
                }
            }
        }
        #endregion

        #region Bulk Orders Import by datetime
        private void ProcessOrdersImportByDateTime(string datetime)
        {
            using (var erpClient = new ERPClient())
            {
                var orderList = erpClient.GetOrdersToUpdateInDCAsync(string.IsNullOrEmpty(datetime) ? null : datetime);

                if (orderList != null && orderList.Count() > 0)
                {
                    var groupedByOrderList = GetGroupedByOrderList(orderList);

                    if (groupedByOrderList != null && groupedByOrderList.Count() > 0)
                    {
                        foreach (var order in groupedByOrderList.ToList())
                        {
                            UpdateProjectsToOpenOrderStatus(order.ProjectID, order);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No orders were imported from Mapics.");
                }
            }
        }
        #endregion

        #region Bulk Invoices Import by datetime
        private void ProcessInvoicesImportByDateTime(string datetime)
        {
            using (var erpClient = new ERPClient())
            {
                var invoiceList = erpClient.GetInvoicesToUpdateInDCAsync(string.IsNullOrEmpty(datetime) ? null : datetime);

                if (invoiceList != null && invoiceList.Count() > 0)
                {
                    foreach (var invoice in invoiceList)
                    {
                        UpdateProjectsToShippedStatus(invoice.ProjectID, invoice);
                    }
                }
                else
                {
                    Console.WriteLine("No orders were imported from Mapics.");
                }
            }
        }
        #endregion

        #region shared 
        private IEnumerable<OrderResponse> GetGroupedByOrderList(List<OrderResponse> orderList)
        {
            var groupedByOrderList = from ord in orderList
                                     group ord by ord.OrderNumber into grp
                                     select new OrderResponse
                                     {
                                         OrderNumber = grp.Select(x => x.OrderNumber).FirstOrDefault(),
                                         OrderDate = grp.Select(x => x.OrderDate).FirstOrDefault(),
                                         DiscountPercent = grp.Select(x => x.DiscountPercent).FirstOrDefault(),
                                         OrderRefNum = grp.Select(x => x.OrderRefNum).FirstOrDefault(),
                                         CustomerNumber = grp.Select(x => x.CustomerNumber).FirstOrDefault(),
                                         ProjectID = grp.Select(x => x.ProjectID).FirstOrDefault(),
                                         OrderComment = string.Join("; ", grp.Select(x => (x.HeaderCode + ":- " + x.OrderComment))),
                                         OrderStatus = grp.Select(x => x.OrderStatus).FirstOrDefault(),
                                         QuoteID = grp.Select(x => x.QuoteID).FirstOrDefault()
                                     };

            return groupedByOrderList;
        }

        private void UpdateProjectsToOpenOrderStatus(long projectId, OrderResponse orderData)
        {
            var projectToUpdate = Db.Projects?.FirstOrDefault(x => x.ProjectId == projectId
                            && (x.ProjectLeadStatusTypeId == ProjectLeadStatusTypeEnum.Lead ||
                                x.ProjectLeadStatusTypeId == ProjectLeadStatusTypeEnum.Opportunity));

            if (projectToUpdate != null)
            {
                projectToUpdate.ERPFirstOrderComment = orderData.OrderComment;
                projectToUpdate.ERPFirstOrderNumber = orderData.OrderNumber;
                projectToUpdate.ERPFirstOrderDate = orderData.OrderDate;
                projectToUpdate.ProjectLeadStatusTypeId = ProjectLeadStatusTypeEnum.OpenOrder;
                projectToUpdate.ProjectOpenStatusTypeId = 6; //Daikin has PO
                projectToUpdate.Deleted = false;
                projectToUpdate.WebServiceImportStatus = "AwaitingCSRUpdate";

                UpdateAndLogProjectsInDC(projectToUpdate);
            }
        }

        private void UpdateProjectsToShippedStatus(long projectId, ERPInvoiceInfo invoiceData)
        {
            var projectToUpdate = Db.Projects?.FirstOrDefault(x => x.ProjectId == projectId
                                    && (x.ProjectLeadStatusTypeId == ProjectLeadStatusTypeEnum.OpenOrder));

            if (projectToUpdate != null)
            {
                projectToUpdate.ERPFirstInvoiceDate = invoiceData.InvoiceDate;
                projectToUpdate.ERPFirstShipDate = invoiceData.ShipmentDate;
                projectToUpdate.ERPFirstInvoiceNumber = invoiceData.InvoiceNumber;
                projectToUpdate.ProjectLeadStatusTypeId = ProjectLeadStatusTypeEnum.Shipped;
                projectToUpdate.ProjectOpenStatusTypeId = 6;
                projectToUpdate.ProjectStatusTypeId = ProjectStatusTypeEnum.ClosedWon;
                projectToUpdate.Deleted = false;
                projectToUpdate.WebServiceImportStatus = "PickedOrdersUpdate";

                UpdateAndLogProjectsInDC(projectToUpdate);
            }
        }

        private void UpdateAndLogOrdersInDC(Order orderToUpdate)
        {
            try
            {
                Db.SaveChanges();
                Console.WriteLine($"Updated Order - {orderToUpdate.OrderId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed while updating - {orderToUpdate.OrderId} with error {ex.Message}");
            }
        }

        private void UpdateAndLogProjectsInDC(Project projectToUpdate)
        {
            try
            {
                Db.SaveChanges();

                Console.WriteLine($"Updated Projects table for project - {projectToUpdate.ProjectId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed while updating project - {projectToUpdate.ProjectId} with error {ex.Message}");
            }
        }
        #endregion 

    }
}



