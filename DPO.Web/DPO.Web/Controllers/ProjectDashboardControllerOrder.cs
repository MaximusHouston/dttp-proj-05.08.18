using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DPO.Common;
using DPO.Domain;
using DPO.Model.Light;
using Newtonsoft.Json;
using System.Net.Mail;
using System.IO;
using log4net;

namespace DPO.Web.Controllers
{
    public partial class ProjectDashboardController
    {
        #region OrderForm
        [HttpGet]
        public ActionResult OrderForm(long projectId, long quoteId)
        {
            if (this.ServiceResponse == null)
                this.ServiceResponse = new ServiceResponse();

            this.ServiceResponse.Model = CheckProductsWithNoClassCode(projectId, quoteId);

            if (this.ServiceResponse.IsOK)
            {
                var apiOrderCtrl = new OrderController();
                this.ServiceResponse = apiOrderCtrl.GetNewOrder(quoteId);

                var model = this.ServiceResponse.Model as OrderViewModelLight;
                InsertProjectInfoToMapics(model);  //Web api call to Mapics to insert/update Projects info

                ProcessServiceResponse(this.ServiceResponse);

                //this.RouteData.Values["action"] = "OrderForm";
                return View("OrderForm", model);
            }
            else
            {
                this.ServiceResponse.Model = quoteService.GetQuoteItemsModel(this.CurrentUser, new QuoteItemsModel { ProjectId = projectId, QuoteId = quoteId, LoadQuoteItems = true }).Model;
                return RedirectToAction("QuoteItemsWithProductsHasNoClassCode", this.ServiceResponse.Model);
            }
        }

        private void InsertProjectInfoToMapics(OrderViewModelLight model)
        {
            var project = new ProjectInfo
            {
                BusinessId = model.BusinessId.GetValueOrDefault(),
                BusinessName = model.BusinessName,
                ERPAccountId = !string.IsNullOrEmpty(model.ERPAccountId) ?
                                    Convert.ToInt32(model.ERPAccountId) : 0,
                ProjectId = model.ProjectId,
                ProjectName = model.ProjectName,
                AccountId = model.AccountID,
                QuoteId = model.QuoteId
            };

            using (var erpClient = new ERPClient())
            {
                erpClient.PostProjectsInfoToMapicsAsync(project);
            }
        }

        private ServiceResponse CheckProductsWithNoClassCode(long projectId, long quoteId)
        {
            this.ServiceResponse = quoteService.CheckProductWithNoClassCode(this.CurrentUser, quoteId);
            return this.ServiceResponse;
        }
        #endregion

        #region SubmittedOrderForm
        [HttpGet]
        public ActionResult SubmittedOrderForm(long projectId, long quoteId)
        {
            //this.ServiceResponse = orderServiceLight.GetOrderModel(this.CurrentUser, quoteId);
            var apiOrderCtrl = new OrderController();
            this.ServiceResponse = apiOrderCtrl.GetSubmittedOrder(quoteId);

            var model = this.ServiceResponse.Model as OrderViewModelLight;
            ProcessServiceResponse(this.ServiceResponse);

            //this.RouteData.Values["action"] = "OrderForm";
            return View("OrderForm", model);
        }
        #endregion

        #region OrderFAQ
        [HttpGet]
        public ActionResult OrderFAQ()
        {
            return View("OrderFAQ");
        }
        #endregion

        #region OrderInQuote
        [HttpGet]
        public ActionResult OrderInQuote(QuoteItemsModel model)
        {
            model.LoadQuoteItems = true;
            this.ServiceResponse = quoteService.GetQuoteItemsModel(this.CurrentUser, model);
            model = this.ServiceResponse.Model as QuoteItemsModel;

            if (ProcessServiceResponse(this.ServiceResponse))
            {
                this.Session["ImportProjectId"] = model.ProjectId;
                this.Session["ImportQuoteId"] = model.QuoteId;
            }

            this.RouteData.Values["action"] = "OrderInQuote";

            return ((this.IsPostRequest) ? (ViewResultBase)PartialView("OrderInQuote", model) : View("OrderInQuote", model));
        }
        #endregion

        #region Orders
        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ActionResult Orders()
        {
            ServiceResponse response = new ServiceResponse();
            OrderViewModelLight model = new OrderViewModelLight();
            //this.ServiceResponse.Model = new OrderViewModelLight();
            this.RouteData.Values["action"] = "Orders";

            return View("Orders", model);
        }
        #endregion

        #region GetEstDeliveryDate
        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ActionResult ConfirmEstDeliveryDate(long projectId, long quoteId)
        {
            //this.ServiceResponse = projectService.GetProjectModel(this.CurrentUser, projectId);
            //ProcessServiceResponse(this.ServiceResponse);
            //ViewBag.QuoteId = quoteId;
            //return ((this.IsPostRequest) ? (ViewResultBase)PartialView("Project", this.ServiceResponse.Model) : View("Project", this.ServiceResponse.Model));
            //return View("ConfirmEstDeliveryDate", this.ServiceResponse.Model);

            ViewBag.quoteId = quoteId.ToString();
            ViewBag.projectId = projectId.ToString();
            return View("ConfirmEstDeliveryDate");
        }
        #endregion

        #region ProjectEditEstimatedDelivery
        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ActionResult EditEstimatedDelivery_Old(long projectId, long quoteId, DateTime estimatedDelivery)
        {
            this.ServiceResponse = projectService.GetProjectModel(this.CurrentUser, projectId);
            ProcessServiceResponse(this.ServiceResponse);

            var projectModel = this.ServiceResponse.Model as ProjectModel;
            projectModel.EstimatedDelivery = estimatedDelivery;

            this.ServiceResponse = projectService.PostModel(this.CurrentUser, projectModel);

            return Json(this.ServiceResponse);
        }

        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ActionResult EditEstimatedDelivery(long projectId, long quoteId, DateTime estimatedDelivery)
        {
            this.ServiceResponse = new ServiceResponse();

            double result = (estimatedDelivery - DateTime.Now).TotalDays;

            if (result < -1)
            {
                this.ServiceResponse.Messages.AddError("OrderReleaseDateInvalid", "Order Release Date can not less than Order Submit Date");
            }

            Session["orderReleaseDate"] = estimatedDelivery;

            return Json(this.ServiceResponse);
        }
        #endregion

        public void CreateOrderFormPdfForSendMail(OrderViewModelLight orderVMLight)
        {
            log.InfoFormat("Enter CreateOrderFormPdfForSendMail");

            long quoteId = orderVMLight.QuoteId;
            long projectId = orderVMLight.ProjectId;
            var urlAuth = Utilities.DocumentServerURL();
            var controller = string.Format("{0}/{1}", urlAuth, "ProjectDashboard");

            var urlOrderFormBody = string.Format("{0}/{1}?orderId={2}&projectId={3}&quoteId={4}", controller, "OrderPrint", orderVMLight.OrderId, projectId, quoteId);
            var urlOrderFormHeader = string.Format("{0}/{1}", controller, "OrderPrintHeader", projectId, quoteId);
            var urlOrderFormFooter = string.Format("{0}/{1}", controller, "OrderPrintFooter");

            log.DebugFormat("quoteId: {0}, projectId: {1}, url:{2}, controller:{3}",
                           quoteId, projectId, urlAuth, controller);

            log.DebugFormat("urlOrderFormBody: {0}", urlOrderFormBody);

            var pdf = new PdfConvertor();
            var web = new WebClientLocal(System.Web.HttpContext.Current);

            pdf.Options.NoLink = false;
            pdf.Options.HeaderHtmlFormat = web.DownloadString(urlOrderFormHeader);
            pdf.Options.FooterHtmlFormat = web.DownloadString(urlOrderFormFooter);
            pdf.Options.FooterHtmlPosition = pdf.Options.OutputArea.Bottom - 1.25f;

            pdf.Options.OutputArea = new System.Drawing.RectangleF(0f, 1.25f, pdf.Options.OutputArea.Width, pdf.Options.OutputArea.Height - 2.5f);
            pdf.AppendHtml(web.DownloadString(urlOrderFormBody));

            string root = System.Web.HttpContext.Current.Server.MapPath("~");
            string parent = System.IO.Path.GetDirectoryName(root);
            string grandParent = System.IO.Path.GetDirectoryName(parent);

            log.DebugFormat("root directory: {0}", root);

            string _last5DigitsOfProjectId = orderVMLight.ProjectIdStr.Substring(orderVMLight.ProjectIdStr.Length - 5);
            string nameFile = "Daikin City Order " +
                               DateTime.Now.ToString("MM-dd-yyyy") +
                               "-" +
                               _last5DigitsOfProjectId + ".pdf";

            string subPath = grandParent + "/CustomerDataFiles/OrderSubmitFiles/" + orderVMLight.QuoteId;

            log.DebugFormat("subPath: {0}", subPath);

            bool exists = System.IO.Directory.Exists(subPath);

            if (!exists)
                System.IO.Directory.CreateDirectory(subPath);

            string filePath = grandParent + "/CustomerDataFiles/OrderSubmitFiles/" + orderVMLight.QuoteId + "/" + nameFile;

            log.DebugFormat("filePath: {0}", filePath);

            try
            {
                pdf.Document.Save(filePath);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("Create pdf error for file name:{0}", filePath);
                log.ErrorFormat("Error Message: {0}", ex.Message);
                log.ErrorFormat("Error Details Message: {0}", ex.InnerException.Message);
                log.ErrorFormat("Error Stack Trace: {0}", ex.StackTrace);
            }

            log.InfoFormat("Finished CreateOrderFormPdfForSendMail");
        }

        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ActionResult SendEmailOrderSubmit(OrderViewModelLight orderVMLight)
        {
            log.InfoFormat("Involke SendEmailOrderSubmit ControllerAction");

            CreateOrderFormPdfForSendMail(orderVMLight);

            var emailModel = this.orderService.getOrderSendEmailModel(orderVMLight);
            emailModel.HelpLink = "mailto:project.desk@daikincomfort.com";

            var fromEmails = Utilities.Config("dpo.sys.email.ordersubmit").Split(',').ToList();
            if (orderVMLight.HasConfiguredModel)
            {
                emailModel.Subject = string.Format("BTO - An Order has been submitted by " + orderVMLight.BusinessName + " - Project: " + orderVMLight.ProjectName);
            }
            else {
                emailModel.Subject = string.Format("An Order has been submitted by " + orderVMLight.BusinessName + " - Project: " + orderVMLight.ProjectName);
            }
            

            emailModel.From = new MailAddress(fromEmails.First(), "DPO Order");

            foreach (string email in fromEmails)
                emailModel.To.Add(new MailAddress(email, "Daikin Project Desk"));

            //Todo: propery need to check for EmailAccount or EmailAccountManager is null
            //if model return null. maybe need to write code to reget it from db
            //currently it based on value of ViewModel.

            AccountServices accountService = new AccountServices();
            UserSessionModel admin = accountService.GetUserSessionModel(orderVMLight.CreatedByUserId).Model as UserSessionModel;
            BusinessServices businessService = new BusinessServices();
            BusinessModel businessModel = businessService.GetBusinessModel(admin, admin.BusinessId).Model as BusinessModel;

            if (!string.IsNullOrEmpty(emailModel.AccountManagerEmail))
            {
                emailModel.To.Add(new MailAddress(emailModel.AccountManagerEmail));
            }
            else
            {
                var emailAccountManager = businessModel.AccountManagerEmail;
                if (emailAccountManager != null)
                    emailModel.AccountManagerEmail = emailAccountManager;
            }

            log.DebugFormat("Account Manager Email: {0}", emailModel.AccountManagerEmail);

            if (emailModel.AccountOwnerEmail != null)
            {
                emailModel.To.Add(new MailAddress(emailModel.AccountOwnerEmail));
            }
            else
            {
                var emailAccountOwner = businessModel.AccountOwnerEmail;
                if (emailAccountOwner != null)
                    emailModel.AccountOwnerEmail = emailAccountOwner;
            }

            if (CurrentUser.Email != null)
            {
                emailModel.To.Add(new MailAddress(CurrentUser.Email));
            }

            if (!string.IsNullOrEmpty(orderVMLight.ProjectOwnerId.ToString()))
            {
                UserServices userService = new UserServices();
                var projectOwner = userService.GetUserModel(CurrentUser, orderVMLight.ProjectOwnerId, true, true).Model as UserModel;
                if (projectOwner != null && !string.IsNullOrEmpty(projectOwner.Email))
                {
                    if (CurrentUser.Email != null && CurrentUser.Email != projectOwner.Email)
                    {
                        emailModel.To.Add(new MailAddress(projectOwner.Email));
                    }
                }
            }

            log.DebugFormat("AccountOwnerEmail: {0}", emailModel.AccountOwnerEmail);

            string root = Server.MapPath("~");
            string parent = System.IO.Path.GetDirectoryName(root);
            string grandParent = System.IO.Path.GetDirectoryName(parent);
            string _last5DigitsOfProjectId = orderVMLight.ProjectIdStr.Substring(orderVMLight.ProjectIdStr.Length - 5);

            string OrderPdfFile = "Daikin City Order " +
                                  DateTime.Now.ToString("MM-dd-yyyy") +
                                  "-" +
                                  _last5DigitsOfProjectId + ".pdf";

            string OrderPdfFilePath = grandParent + "/CustomerDataFiles/OrderSubmitFiles/" + orderVMLight.QuoteId + "/" + OrderPdfFile;

            log.DebugFormat("OrderPdfFilePath: {0}", OrderPdfFilePath);

            string DARPdfFile = "Daikin City Discount Request " +
                                    DateTime.Now.ToString("MM-dd-yyyy") +
                                    "-" +
                                   _last5DigitsOfProjectId + ".pdf";

            string DARPdfFilePath = grandParent + Utilities.Config("dpo.setup.customerdata.location") + "DiscountRequestFiles/" + orderVMLight.QuoteId + "/" + DARPdfFile;

            log.DebugFormat("DarPdfFilePath: {0}", DARPdfFilePath);

            string COMPdfFile = orderVMLight.CommissionRequestId.ToString() + ".pdf";
            string COMPdfFilePath = grandParent + "/CustomerDataFiles/DiscountRequestFiles/" + orderVMLight.QuoteId + "/" + COMPdfFile;

            log.DebugFormat("COMPdfFilePath: {0}", COMPdfFilePath);

            if (orderVMLight.DiscountRequestId != null)
            {
                if (System.IO.File.Exists(DARPdfFilePath))
                {
                    orderVMLight.DARAttachmentFileName = DARPdfFilePath;
                }
                else
                {
                    orderVMLight.DARAttachmentFileName = System.IO.Directory.GetFiles(grandParent + Utilities.Config("dpo.setup.customerdata.location") + "DiscountRequestFiles/").FirstOrDefault();
                }

            }
            else
            {
                var discountRequest = discountRequestService.GetDiscountRequestModel(this.CurrentUser, orderVMLight.ProjectId, orderVMLight.QuoteId).Model as DiscountRequestModel;

                if (discountRequest.DiscountRequestId != null)
                    orderVMLight.DiscountRequestId = discountRequest.DiscountRequestId;

                if (orderVMLight.DiscountRequestId != null && orderVMLight.DiscountRequestId > 0)
                {
                    if (System.IO.File.Exists(DARPdfFilePath))
                    {
                        orderVMLight.DARAttachmentFileName = DARPdfFilePath;
                    }
                    else
                    {
                        string currentDARDirectory = grandParent + Utilities.Config("dpo.setup.customerdata.location") + @"DiscountRequestFiles\" + orderVMLight.QuoteId + @"\";
                        if (System.IO.Directory.Exists(currentDARDirectory))
                        {
                            orderVMLight.DARAttachmentFileName = System.IO.Directory.GetFiles(currentDARDirectory, "*.pdf").FirstOrDefault();
                        }
                        else
                        {
                            CreateDarPdfForSendMail(discountRequest);
                            orderVMLight.DARAttachmentFileName = System.IO.Directory.GetFiles(currentDARDirectory, "*.pdf").FirstOrDefault();
                        }
                    }

                    emailModel.order.RequestedDiscountPercentage = (discountRequest != null & discountRequest.RequestedDiscount > 0) ? discountRequest.RequestedDiscount : 0;
                    emailModel.order.ApprovedDiscountPercentage = (discountRequest != null && discountRequest.ApprovedDiscount > 0) ? discountRequest.ApprovedDiscount : 0;
                }
            }

            log.DebugFormat("RequesteDiscountPercentage: {0}", emailModel.order.RequestedDiscountPercentage);
            log.DebugFormat("ApprovedDiscountPercentage: {0}", emailModel.order.ApprovedDiscountPercentage);

            if (emailModel.order.ProjectDate == DateTime.MinValue)
            {
                emailModel.order.ProjectDate = orderVMLight.ProjectDate;
                if (emailModel.order.ProjectDate == DateTime.MinValue)
                {
                    var project = projectService.GetProjectModel(this.CurrentUser, orderVMLight.ProjectId).Model as ProjectModel;
                    emailModel.order.ProjectDate = project.ProjectDate.Value;
                }
            }

            log.DebugFormat("Project Date: {0}", emailModel.order.ProjectDate);

            if (orderVMLight.CommissionRequestId != null)
            {
                orderVMLight.COMAttachmentFileName = COMPdfFilePath;
            }

            if (orderVMLight.OrderId != 0 || orderVMLight.QuoteId != 0)
            {
                orderVMLight.OrderAttachmentFileName = OrderPdfFilePath;
            }

            emailModel.OrderAttachmentFile = orderVMLight.OrderAttachmentFileName;
            emailModel.DARAttachmentFile = orderVMLight.DARAttachmentFileName;
            emailModel.COMAttachmentFile = orderVMLight.COMAttachmentFileName;

            if (orderVMLight.DiscountRequestId != null && orderVMLight.DiscountRequestId > 0)
            {
                log.DebugFormat("DARAttachmentFile: {0}", emailModel.DARAttachmentFile);
            }
            if (orderVMLight.CommissionRequestId != null && orderVMLight.CommissionRequestId > 0)
            {
                log.DebugFormat("COMAttachmentFile: {0}", emailModel.COMAttachmentFile);
            }

            log.DebugFormat("OrderAttachmentFile: {0}", emailModel.OrderAttachmentFile);

            //string[] filePaths = Directory.GetFiles(grandParent + "/CustomerDataFiles/POAttachment/" + orderVMLight.QuoteId + "/", "*.*",
            //                         SearchOption.AllDirectories);

            if (emailModel.OtherAttachmentFiles == null)
            {
                emailModel.OtherAttachmentFiles = new List<string>();
            }

            var uploadDirectory = new System.IO.DirectoryInfo(Utilities.GetPOAttachmentDirectory(orderVMLight.QuoteId));
            var LatestUploadFile = (from f in uploadDirectory.GetFiles()
                                    orderby f.LastWriteTime descending
                                    select f).First();

            if (LatestUploadFile != null)
            {
                emailModel.OtherAttachmentFiles.Add(LatestUploadFile.FullName);
            }

            SendEmailModel sendEmailModel = emailModel;

            sendEmailModel.DARAttachmentFile = (emailModel.DARAttachmentFile != null) ? emailModel.DARAttachmentFile : null;
            sendEmailModel.COMAttachmentFile = (emailModel.COMAttachmentFile != null) ? emailModel.COMAttachmentFile : null;
            sendEmailModel.DARAttachmentFileName = DARPdfFile;
            sendEmailModel.COMAttachmentFileName = COMPdfFile;

            if (orderVMLight.DiscountRequestId != null && orderVMLight.DiscountRequestId > 0)
            {
                log.DebugFormat("sendEmailModel.DARAttachmentFile: {0}", sendEmailModel.DARAttachmentFile);
                log.DebugFormat("sendEmailModel.DARAttachmentFileName: {0}", sendEmailModel.DARAttachmentFileName);
            }
            if (orderVMLight.CommissionRequestId != null && orderVMLight.CommissionRequestId > 0)
            {
                log.DebugFormat("sendEmailModel.COMAttachmentFile: {0}", sendEmailModel.COMAttachmentFile);
                log.DebugFormat("sendEmailModel.COMAttachmentFileName: {0}", sendEmailModel.COMAttachmentFileName);
            }

            sendEmailModel.OrderAttachmentFile = (emailModel.OrderAttachmentFile != null) ? emailModel.OrderAttachmentFile : null;
            sendEmailModel.OrderAttachmentFileName = OrderPdfFile;

            log.DebugFormat("sendEmailModel.OrderAttachmentFile: {0}", sendEmailModel.OrderAttachmentFile);
            log.DebugFormat("sendEmailModel.OrderAttachmentFileName: {0}", sendEmailModel.OrderAttachmentFileName);

            sendEmailModel.OtherAttachmentFiles = emailModel.OtherAttachmentFiles;

            if (sendEmailModel.OtherAttachmentFiles.Count > 0)
                log.DebugFormat("sendEmailModel.OtherAttachmentFiles: {0}", sendEmailModel.OtherAttachmentFiles);

            emailModel.RenderTextVersion = true;
            emailModel.BodyTextVersion = RenderView(this, "SendEmailOrderSubmit", emailModel);

            emailModel.RenderTextVersion = false;
            emailModel.BodyHtmlVersion = RenderView(this, "SendEmailOrderSubmit", emailModel);

            sendEmailModel.ProjectId = orderVMLight.ProjectId;
            sendEmailModel.QuoteId = orderVMLight.QuoteId;

            try
            {
                log.DebugFormat("Start sending Order submit email....");
                new EmailServices().SendEmail(sendEmailModel);
            }
            catch (Exception ex)
            {
                log.DebugFormat("Start sending Order Submit error email.... ");
                SendEmailToTeamWhenFailToSendEmailOnOrder(orderVMLight.QuoteId, ex.Message);
            }

            return null;
        }

        public ActionResult SendEmailToTeamWhenFailToSendEmailOnOrder(long quoteId, string exception)
        {
            //var emailModel = this.orderService.getOrderSendEmailModel(orderVMLight);

            long projectId = projectService.GetProjectId(this.CurrentUser, quoteId);
            ProjectModel projectModel = projectService.GetProjectModel(this.CurrentUser, projectId).Model as ProjectModel;

            var emailModel = new DPO.Common.SendEmailModel();

            var sendEmailModel = emailModel;
            var fromEmails = Utilities.Config("dpo.sys.email.orderSendEmailError").Split(',').ToList();
            emailModel.Subject = string.Format("An Order has been submitted");
            emailModel.From = new MailAddress(fromEmails.First(), "Send Email Order Submit Error");

            foreach (string email in fromEmails)
                emailModel.To.Add(new MailAddress(email, "Daikin Project Desk"));

            string emailMessage = string.Format(@"An error happned when try to send email for Order Submit.Below is the Order Details:" + Environment.NewLine +
                                                   "Project Reference: {0} " + Environment.NewLine +
                                                   "Project Date: {1} " + Environment.NewLine +
                                                   "Project Owner: {2} " + Environment.NewLine +
                                                   "Quote Reference: {3} " + Environment.NewLine +
                                                   "Exception Details: {4}",
                                                   projectModel.ProjectId.ToString(),
                                                   projectModel.ProjectDate,
                                                   projectModel.OwnerName,
                                                   quoteId.ToString(),
                                                   (exception != string.Empty) ? exception : Session["SavePoAttachment"].ToString());

            sendEmailModel.Subject = "Order Submit Send Email has error";
            sendEmailModel.RenderTextVersion = true;
            sendEmailModel.BodyTextVersion = emailMessage;

            try
            {
                new EmailServices().SendEmail(sendEmailModel);
            }
            catch (Exception ex)
            {
                log.ErrorFormat("sending Order submit error email failed.", ex.InnerException.Message);
            }

            return null;
        }

        [Authorise(Accesses = new[] { SystemAccessEnum.RequestDiscounts, SystemAccessEnum.ApproveDiscounts })]
        public ActionResult OrderPrint(long? projectId, long? quoteId, long? orderId, bool showCostPricing = false)
        {
            log.InfoFormat("Enter OrderPrint Controller Action for ProjectId: {0}, QuoteId: {1}, OrderId: {2}",
                           projectId, quoteId, orderId);

            if (quoteId == null)
            {
                log.Debug("QuoteId is null. Start getting QuoteId");
                quoteId = orderService.GetQuoteIdByOrder(orderId.Value);
                log.DebugFormat("QuoteId: {0}", quoteId);
            }

            log.Debug("Start getting Order in Quote");
            this.ServiceResponse = orderService.GetOrderInQuote(this.CurrentUser, projectId.Value, quoteId.Value);

            if (ProcessServiceResponse(this.ServiceResponse))
            {
                log.Debug("Get Order in Quote completed successfully");

                this.RouteData.Values["action"] = "OrderPrint";

                //earliest opportunity to tell view to show or hide pricing
                ViewData["ShowCostPricing"] = showCostPricing;

                log.DebugFormat("Render OrdePrint View for OrderId: {0}", orderId);
                log.Debug("Finished OrderPrint");

                return View("OrderPrint", this.ServiceResponse.Model);
            }

            log.DebugFormat("Get Order in Quote failed. Return emty result View");
            log.Debug("finished OrderPrint");

            return new EmptyResult();
        }

        public ActionResult OrderPrintFooter()
        {
            log.Info("Render OrderPrintFooter");
            return View("OrderPrintFooter", "_PrintPartialLayout");
        }

        public ActionResult OrderPrintHeader()
        {
            log.Info("Render OrderPrintHeader");
            return View("OrderPrintHeader", "_PrintPartialLayout");
        }

        [Authorise(Accesses = new[] { SystemAccessEnum.RequestDiscounts, SystemAccessEnum.ApproveDiscounts })]
        public ActionResult OrderPrintWithCostPricing(long? orderId, long? projectId, long? quoteId)
        {
            log.Info("Enter OrderPrintWithCostPricing");
            log.DebugFormat("Orderid: {0}, Projectid: {1}, QuoteId: {2}", orderId, projectId, quoteId);
            return RedirectToAction("OrderPrint", new { OrderId = orderId, projectId = projectId, quoteId = quoteId, showCostPricing = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.RequestDiscounts })]
        public ActionResult OrderDelete(OrderViewModelLight model)
        {
            this.ServiceResponse = orderService.Delete(this.CurrentUser, model);
            this.ShowKeyMessagesOnPage = true;

            if (ProcessServiceResponse(this.ServiceResponse))
            {
                //force email to be sent to user
                var OrderForEmail = this.ServiceResponse.Model as OrderViewModelLight;
                SendApprovalRejectionEmail(OrderForEmail);

                //return RedirectToAction("Quote", new QuoteModel { ProjectId = model.ProjectId, QuoteId = model.QuoteId });
                string url = "/v2/#/quote/" + model.QuoteId + "/existingRecord";
                return Redirect(url);
            }

            return View("OrderForm", this.ServiceResponse.Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.ApproveDiscounts })]
        public ActionResult OrderReject(OrderViewModelLight model)
        {
            this.ServiceResponse = orderService.Reject(this.CurrentUser, model);
            this.ShowKeyMessagesOnPage = true;

            if (ProcessServiceResponse(this.ServiceResponse))
            {

                SendApprovalRejectionEmail(this.ServiceResponse.Model as OrderViewModelLight);
                return RedirectToAction("Orders", "UserDashboard");
            }

            return View("OrderForm", this.ServiceResponse.Model);
        }

        private void SendApprovalRejectionEmail(OrderViewModelLight orderVMLight)
        {
            UserServices userSerivce = new UserServices();

            var user = new UserSessionModel();

            #pragma warning disable CS0472 // The result of the expression is always 'true' since a value of type 'long' is never equal to 'null' of type 'long?'
            if (orderVMLight.ProjectOwnerId != null)
            #pragma warning restore CS0472 // The result of the expression is always 'true' since a value of type 'long' is never equal to 'null' of type 'long?'
            {
                user = new AccountServices().GetUserSessionModel(orderVMLight.ProjectOwnerId).Model as UserSessionModel;
            }

            var orderEmailModel = new SendEmailApprovalModel();
            orderEmailModel.HelpLink = "mailto:project.desk@daikincomfort.com";

            orderEmailModel.Subject = string.Format("The status of an Order request has changed");

            orderEmailModel.Reason = orderVMLight.Comments;
            orderEmailModel.ProjectId = orderVMLight.ProjectId;
            orderEmailModel.ProjectName = orderVMLight.ProjectName;
            orderEmailModel.QuoteTitle = orderVMLight.QuoteTitle;
            orderEmailModel.ERPOrderNumber = (orderVMLight.ERPOrderNumber != null) ? orderVMLight.ERPOrderNumber : string.Empty;
            orderEmailModel.ERPInvoiceNumber = (orderVMLight.ERPInvoiceNumber != null) ? orderVMLight.ERPInvoiceNumber : string.Empty;
            orderEmailModel.ERPPOKey = (orderVMLight.ERPPOKey != null) ? orderVMLight.ERPPOKey.Value : 0;
            orderEmailModel.ERPInvoiceDate = (orderVMLight.ERPInvoiceDate != null) ? orderVMLight.ERPInvoiceDate.Value : DateTime.Now;
            orderEmailModel.ERPOrderDate = (orderVMLight.ERPOrderDate != null) ? orderVMLight.ERPOrderDate.Value : DateTime.Now;
            #pragma warning disable CS0472 // The result of the expression is always 'true' since a value of type 'decimal' is never equal to 'null' of type 'decimal?'
            orderEmailModel.TotalNet = (orderVMLight.TotalNetPrice != null) ? orderVMLight.TotalNetPrice : 0;
            #pragma warning restore CS0472 // The result of the expression is always 'true' since a value of type 'decimal' is never equal to 'null' of type 'decimal?'
            orderEmailModel.Approved = (orderVMLight.OrderStatusTypeId == OrderStatusTypeEnum.InProcess);
            orderEmailModel.ModifierName = (user != null) ? user.FirstName + " " + user.LastName : orderVMLight.UpdatedByUserIdStr;

            orderEmailModel.ProjectOwnerName = (orderVMLight.ProjectOwner != null) ? orderVMLight.ProjectOwner : user.FirstName + " " + user.LastName;
            orderEmailModel.ProjectDate = (orderVMLight.ProjectDate != null) ? orderVMLight.ProjectDate : DateTime.Now;

            var business = new BusinessServices().GetBusinessModel(user, user.BusinessId, false).Model as BusinessModel;
            orderEmailModel.BusinessName = (business != null && business.BusinessName != null) ? business.BusinessName : "No Business Name";

            orderEmailModel.From = new MailAddress(Utilities.Config("dpo.sys.email.discountrequest"), "DPO Project Desk");

            orderEmailModel.To.Add(orderEmailModel.From);

            if (!string.IsNullOrEmpty(business.AccountManagerEmail))
            {
                orderEmailModel.To.Add(new MailAddress(business.AccountManagerEmail));
            }

            if (!string.IsNullOrEmpty(business.AccountOwnerEmail))
            {
                orderEmailModel.To.Add(new MailAddress(business.AccountOwnerEmail));
            }

            orderEmailModel.RenderTextVersion = true;
            orderEmailModel.BodyTextVersion = RenderView(this, "SendEmailOrderApproval", orderEmailModel);

            orderEmailModel.RenderTextVersion = false;
            orderEmailModel.BodyHtmlVersion = RenderView(this, "SendEmailOrderApproval", orderEmailModel);

            new EmailServices().SendEmail(orderEmailModel);
        }

        // [HttpGet]
        //[Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        //public string CheckPONumber(string PONumber, string ERPAccountId)
        //{
        //   int _poCount = orderService.CheckPONumber(PONumber, ERPAccountId);
        //    string message = "";
        //    if(_poCount < 0)
        //    {
        //        message = "Valid PO Number";
        //    }
        //    else
        //    {
        //        message = "Invalid PO Number";
        //    }

        //    return message;
        //}

        [Authorise(Accesses = new[] { SystemAccessEnum.ViewOrder })]
        public ActionResult GetPOAttachment(long quoteId, string poAttachmentFileName)
        {
            string nameFile = poAttachmentFileName;
            string root = Server.MapPath("~");
            string parent = System.IO.Path.GetDirectoryName(root);
            string grandParent = System.IO.Path.GetDirectoryName(parent);
            string filePath = grandParent + "/CustomerDataFiles/POAttachment/" + quoteId + "/" + nameFile;

            Response.ClearContent();
            Response.ClearHeaders();
            Response.AppendHeader("Content-Disposition", "inline; filename=" + nameFile + ";");

            if (nameFile.Contains("pdf"))
            {
                Response.ContentType = "application/pdf";
            }

            string[] excelTypes = new string[] { "xls", "xlsx", "csv" };
            foreach (string type in excelTypes)
            {
                if (nameFile.Contains(type))
                {
                    Response.ContentType = "application/xls";
                    //System.Diagnostics.Process.Start(filePath);
                    //return null;
                }
            }

            if (nameFile.Contains("txt"))
            {
                Response.ContentType = "text/plain";
            }

            string[] imageTypes = new string[] { "jpg", "bmp", "jpeg", "tiff", "gif", "png" };

            foreach (string type in imageTypes)
            {
                if (nameFile.Contains(type))
                {
                    Response.ContentType = "image/" + type;
                }
            }

            string[] wordTypes = new string[] { "doc", "docs", "docx" };
            foreach (string type in wordTypes)
            {
                if (nameFile.Contains(type))
                {
                    Response.ContentType = "application/vnd.ms-word";
                    //System.Diagnostics.Process.Start(filePath);
                    //return null;
                }
            }

            bool fileExist = System.IO.File.Exists(filePath);

            if (fileExist)
            {
                Response.WriteFile(filePath);
                Response.Flush();
                Response.Clear();
            }
            else
            {
                this.ServiceResponse = new ServiceResponse();
                ServiceResponse.Messages.AddError("Can not found the PO Attachment file.Please contact Daikin Technical support.");
            }

            return null;
        }
    }
}