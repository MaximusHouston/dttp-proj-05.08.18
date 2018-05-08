using DPO.Common; 
using System;
using System.Collections.Generic; 
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace DPO.Domain
{
    public class ERPClient : BaseServices
    {
        public const string http_failure_get_order = "GetOrderInfoCallFailed";
        public const string http_failure_post_order = "PostOrderFailed";

        #region GET Orders Info
        public ServiceResponse GetOrderInfoFromMapicsAsync(string customerNum, string poNum)
        {
            if (string.IsNullOrWhiteSpace(customerNum) || string.IsNullOrWhiteSpace(poNum)) return Response;

            var httpClient = SetUpHeadersAndAuthentication();
            using (httpClient)
            {
                try
                {
                    var url = Utilities.Config("GetByPOandCustomerUrl");
                    var httpResponseMsg = httpClient.GetAsync(url + customerNum + "/pono/" + poNum).Result;

                    using (var responseHelper = new HttpResponseHelper(httpResponseMsg))
                    {
                        Response = httpResponseMsg.IsSuccessStatusCode ? responseHelper.GetOrderInfoApiResponse()
                                        : responseHelper.GetAppropriateResponseMessage(http_failure_get_order);
                    }
                }
                catch (Exception ex)
                {
                    Response.Messages.AddError(ex.Message);
                    return Response;
                }
            }

            return Response;
        }

        public List<OrderResponse> GetOrdersToUpdateInDCAsync(string datetime)
        {
            var orderResponseList = new List<OrderResponse>();

            if (string.IsNullOrEmpty(datetime))
                datetime = DateTime.Now.AddDays(-1).ToString("yyyyMMddHHmmss");

            var httpClient = SetUpHeadersAndAuthentication();
            using (httpClient)
            {
                try
                {
                    var url = Utilities.Config("GetOrdersToUpdateInDC");
                    var httpResponseMsg = httpClient.GetAsync(url + datetime).Result;

                    using (var responseHelper = new HttpResponseHelper(httpResponseMsg))
                    {
                        orderResponseList = httpResponseMsg.IsSuccessStatusCode ? responseHelper.GetOrdersToUpdateApiResponse().ToList()
                                                        : null;
                    }
                }
                catch (Exception ex)
                {
                    orderResponseList.Count.Equals(0);
                }
            }

            return orderResponseList;
        }
        #endregion

        #region GET Order Statuses
        public List<ERPOrderInfo> CheckStatusForSubmittedOrdersAsync()
        {
            var orderResponseList = new List<ERPOrderInfo>();

            var httpClient = SetUpHeadersAndAuthentication();
            using (httpClient)
            {
                try
                {
                    var url = Utilities.Config("CheckSubmittedOrdersStatus");
                    var httpResponseMsg = httpClient.GetAsync(url).Result;

                    using (var responseHelper = new HttpResponseHelper(httpResponseMsg))
                    {
                        orderResponseList = httpResponseMsg.IsSuccessStatusCode 
                                            ? responseHelper.GetSubmittedOrdersStatus().ToList()
                                            : null;
                    }
                }
                catch (Exception ex)
                {
                    orderResponseList.Count.Equals(0);
                }
            }

            return orderResponseList;
        }
         
        public List<OrderResponse> CheckStatusForAwaitingCSROrdersAsync()
        {
            var orderResponseList = new List<OrderResponse>();

            var httpClient = SetUpHeadersAndAuthentication();
            using (httpClient)
            {
                try
                {
                    var url = Utilities.Config("CheckAwaitingCsrOrdersStatus");
                    var httpResponseMsg = httpClient.GetAsync(url).Result;

                    using (var responseHelper = new HttpResponseHelper(httpResponseMsg))
                    {
                        orderResponseList = httpResponseMsg.IsSuccessStatusCode
                                            ? responseHelper.GetAwaitingCSROrderStatus().ToList()
                                            : null;
                    }
                }
                catch (Exception ex)
                {
                    orderResponseList.Count.Equals(0);
                }
            }

            return orderResponseList;
        }

        public ERPOrderInfo CheckStatusForInProcessOrdersAsync(string orderNumber)
        {
            var orderInfo = new ERPOrderInfo();

            var httpClient = SetUpHeadersAndAuthentication();
            using (httpClient)
            {
                try
                {
                    var url = Utilities.Config("CheckInProcessOrdersStatus");
                    var httpResponseMsg = httpClient.GetAsync(url).Result;

                    using (var responseHelper = new HttpResponseHelper(httpResponseMsg))
                    {
                        orderInfo = httpResponseMsg.IsSuccessStatusCode
                                    ? responseHelper.GetInProcessOrderStatus()
                                    : null;
                    }
                }
                catch (Exception ex)
                {
                    orderInfo = null;
                }
            }

            return orderInfo;
        }

        public ERPInvoiceInfo CheckStatusForPickedOrdersAsync(string orderNumber)
        {
            var invoice = new ERPInvoiceInfo();

            var httpClient = SetUpHeadersAndAuthentication();
            using (httpClient)
            {
                try
                {
                    var url = Utilities.Config("CheckPickedOrdersStatus");
                    var httpResponseMsg = httpClient.GetAsync(url).Result;

                    using (var responseHelper = new HttpResponseHelper(httpResponseMsg))
                    {
                        invoice = httpResponseMsg.IsSuccessStatusCode
                                    ? responseHelper.GetPickedOrderStatus() 
                                    : null;
                    }
                }
                catch (Exception ex)
                {
                    invoice = null;
                }
            }

            return invoice;
        }

        public List<ERPInvoiceInfo> GetInvoicesToUpdateInDCAsync(string datetime)
        {
            var invoiceList = new List<ERPInvoiceInfo>();

            if (string.IsNullOrEmpty(datetime))
                datetime = DateTime.Now.AddDays(-1).ToString("yyyyMMddHHmmss");
          
            var httpClient = SetUpHeadersAndAuthentication();
            using (httpClient)
            {
                try
                {
                    var url = Utilities.Config("GetInvoicesToUpdateInDC");
                    var httpResponseMsg = httpClient.GetAsync(url + datetime).Result;

                    using (var responseHelper = new HttpResponseHelper(httpResponseMsg))
                    {
                        invoiceList = httpResponseMsg.IsSuccessStatusCode ? responseHelper.GetInvoicesToUpdateApiResponse().ToList()
                                                : null;
                    }
                }
                catch (Exception ex)
                {
                    invoiceList.Count.Equals(0);
                }
            }

            return invoiceList;
        }
        #endregion

        #region POST Orders   
        public ServiceResponse PostOrderToMapicsAsync(ERPOrderInfo orderArray)
        {
            if (orderArray == null) return Response;

            var httpClient = SetUpHeadersAndAuthentication();
            using (httpClient)
            {
                try
                {
                    var url = Utilities.Config("PostOrdersUrl");
                    var httpResponseMsg = httpClient.PostAsJsonAsync(url, orderArray).Result;

                    using (var responseHelper = new HttpResponseHelper(httpResponseMsg))
                    {
                        Response = httpResponseMsg.IsSuccessStatusCode ? responseHelper.GetPostOrdersApiResponse()
                                            : responseHelper.GetAppropriateResponseMessage(http_failure_post_order);
                    }
                }
                catch (Exception ex)
                {
                    Response.Messages.AddError(ex.Message);
                    return Response;
                }
            }

            return Response;
        }
        #endregion

        #region POST Projects Info 
        public void PostProjectsInfoToMapicsAsync(ProjectInfo project)
        {
            if (project != null)
            {
                var httpClient = SetUpHeadersAndAuthentication();
                using (httpClient)
                {
                    try
                    {
                        var url = Utilities.Config("PostProjectsInfoUrl");
                        var httpResponseMsg = httpClient.PostAsJsonAsync(url, project).Result;

                        if (httpResponseMsg.IsSuccessStatusCode)
                            httpResponseMsg.Content.ReadAsAsync<string>();
                    }
                    catch (Exception ex)
                    {
                        Log.Warn($"Unable to import post project into mapics due to {ex.Message}");
                    }
                }
            }
        }
        #endregion  

        #region common
        public HttpClient SetUpHeadersAndAuthentication()
        {
            var client = new HttpClient();
            var userPwd = Utilities.Config("ERPusername") + ":" + Utilities.Config("ERPpassword");
            using (var auth = new AuthenticationHelper(userPwd))
            {
                var token = auth.Token;
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                return client;
            }         //Encoding.UTF8.GetBytes(userPwd).ToArray();
        }
        #endregion
    }
}