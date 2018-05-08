using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DPO.Common;
using DPO.Domain;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using System.Net.Mail;
using Newtonsoft.Json;
using DPO.Common.Models.Project;
using System.Text.RegularExpressions;
using log4net;

namespace DPO.Web.Controllers
{
    public partial class ProjectDashboardController
    {
        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.RequestDiscounts, SystemAccessEnum.ApproveDiscounts })]
        public ActionResult DiscountRequest(long? discountRequestId, long? projectId, long? quoteId)
        {
            this.ServiceResponse = discountRequestService.GetDiscountRequestModel(this.CurrentUser, new DiscountRequestModel { DiscountRequestId = discountRequestId, ProjectId = projectId, QuoteId = quoteId });

            if (ProcessServiceResponse(this.ServiceResponse))
            {
                this.RouteData.Values["action"] = "DiscountRequest";

                return View("DiscountRequest", this.ServiceResponse.Model);
            }

            return new EmptyResult();
        }

        [Authorise(Accesses = new[] { SystemAccessEnum.RequestDiscounts, SystemAccessEnum.ApproveDiscounts })]
        public ActionResult DiscountRequestExport(long? discountRequestId, long? projectId, long? quoteId, bool showCostPricing = false)
        {

            this.ServiceResponse = discountRequestService.GetDiscountRequestModel(this.CurrentUser, new DiscountRequestModel { DiscountRequestId = discountRequestId, ProjectId = projectId, QuoteId = quoteId });

            if (this.ServiceResponse.IsOK)
            {
                if (this.CurrentUser.ShowPrices == false)
                {
                    showCostPricing = false;
                }

                var model = this.ServiceResponse.Model as DiscountRequestModel;
                var stream = discountRequestService.GetDiscountRequestExportExcelFile(model, showCostPricing);

                this.Response.AddHeader("Content-Disposition", "inline; filename=Discount Request Export.xls");
                this.Response.AddHeader("Cache-Control", "no-cache");
                this.Response.AddHeader("Content-Type", MimeMapping.GetMimeMapping("Discount Request Export.xls"));

                stream.WriteTo(this.Response.OutputStream);

                return new EmptyResult();
            }

            return new EmptyResult();

        }

        [Authorise(Accesses = new[] { SystemAccessEnum.RequestDiscounts, SystemAccessEnum.ApproveDiscounts })]
        public ActionResult DiscountRequestExportWithCostPricing(long? discountRequestId, long? projectId, long? quoteId, bool? createDARpdf)
        {
            return RedirectToAction("DiscountRequestExport", new { discountRequestId = discountRequestId, projectId = projectId, quoteId = quoteId, showCostPricing = true });
        }
        [Authorise(Accesses = new[] { SystemAccessEnum.RequestDiscounts, SystemAccessEnum.ApproveDiscounts })]
        public ActionResult DiscountRequestPrint(long? discountRequestId, long? projectId, long? quoteId, bool? createDARpdf, bool showCostPricing = false)
        {
            this.ServiceResponse = discountRequestService.GetDiscountRequestModel(this.CurrentUser, new DiscountRequestModel { DiscountRequestId = discountRequestId, ProjectId = projectId, QuoteId = quoteId });

            if (ProcessServiceResponse(this.ServiceResponse))
            {
                this.RouteData.Values["action"] = "DiscountRequestPrint";

                //earliest opportunity to tell view to show or hide pricing
                ViewData["ShowCostPricing"] = showCostPricing;
                ViewData["createDARpdf"] = createDARpdf;

                return View("DiscountRequestPrint", this.ServiceResponse.Model);
            }

            return new EmptyResult();
        }

        public ActionResult DiscountRequestPrintFooter()
        {
            return View("DiscountRequestPrintFooter", "_PrintPartialLayout");
        }

        public ActionResult DiscountRequestPrintHeader()
        {
            return View("DiscountRequestPrintHeader", "_PrintPartialLayout");
        }

        [Authorise(Accesses = new[] { SystemAccessEnum.RequestDiscounts, SystemAccessEnum.ApproveDiscounts })]
        public ActionResult DiscountRequestPrintWithCostPricing(long? discountRequestId, long? projectId, long? quoteId)
        {
            return RedirectToAction("DiscountRequestPrint", new { discountRequestId = discountRequestId, projectId = projectId, quoteId = quoteId, showCostPricing = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.RequestDiscounts, SystemAccessEnum.ApproveDiscounts })]
        public ActionResult DiscountRequest(DiscountRequestModel model)
        {
            model.CompetitorQuoteFile = Request.Files["CompetitorQuoteFile"];

            model.CompetitorLineComparsionFile = Request.Files["CompetitorLineComparsionFile"];

            model.CompetitorQuoteFileName = (model.CompetitorQuoteFile == null || model.CompetitorQuoteFile.FileName == "") ? model.CompetitorQuoteFileName : model.CompetitorQuoteFile.FileName;

            model.CompetitorLineComparsionFileName = (model.CompetitorLineComparsionFile == null || model.CompetitorLineComparsionFile.FileName == "") ? model.CompetitorLineComparsionFileName : model.CompetitorLineComparsionFile.FileName;
            model.OrderDeliveryDate = model.Project.EstimatedDelivery;

            List<string> emailsList = new List<string>();

            if (model.EmailsList != null && model.EmailsList.Length > 0)
            {
                emailsList = model.EmailsList.ToString().Split(',', ';').ToList();
            }



            EmailServices emailService = new EmailServices();
          
            List<string> InvalidEmails = this.discountRequestService.GetInvalidEmails(emailsList);
            
            if(InvalidEmails.Count > 0)
            {
                foreach (string email in InvalidEmails)
                {
                    model.InvalidEmails.Add(email);
                }
                model.IsValidEmails = false;
            }
            
            // set the RequestedDiscount equals to Approved Discount for Daikin Super User Testing Only
            if( model.RequestedDiscount == 0)
            {
                model.RequestedDiscount = model.ApprovedDiscount;
            }

            this.ServiceResponse = discountRequestService.PostModel(this.CurrentUser, model);
            
            if (ProcessServiceResponse(this.ServiceResponse))
            {
                model.Project.EstimatedDelivery = model.OrderDeliveryDate;

                model.IsValidEmails = true;

                this.ServiceResponse = projectService.PostModel(this.CurrentUser, model.Project);

                var emailModel = this.discountRequestService.GetDiscountRequestSendEmailModel(model);

                if(emailModel.discountRequest.RequestedDiscount == 0)
                {
                   var quoteModel = quoteService.GetQuoteModel(this.CurrentUser, model.ProjectId, model.QuoteId).Model as QuoteModel;
                   if(quoteModel != null)
                   {
                        emailModel.discountRequest.RequestedDiscount = quoteModel.DiscountPercentage * 100;
                   }
                }

                emailModel.HelpLink = "mailto:project.desk@daikincomfort.com";

                emailModel.Subject = string.Format("A DPO Discount request has been submitted");

                emailModel.From = new MailAddress(Utilities.Config("dpo.sys.email.discountrequest"), "DPO Discount Request");
                emailModel.To.Add(new MailAddress(Utilities.Config("dpo.sys.email.discountrequest"), "Daikin Project Desk"));

                if (!string.IsNullOrEmpty(emailModel.AccountManagerEmail))
                {
                    emailModel.To.Add(new MailAddress(emailModel.AccountManagerEmail));
                }

                if (!string.IsNullOrEmpty(emailModel.AccountOwnerEmail) && emailModel.AccountOwnerEmail != emailModel.AccountManagerEmail)
                {
                    emailModel.To.Add(new MailAddress(emailModel.AccountOwnerEmail));
                }
                
                if(model.ProjectOwnerId != null)
                {
                    UserServices userService = new UserServices();
                    UserModel projectOwner = userService.GetUserModel(CurrentUser, model.ProjectOwnerId, true, true).Model as UserModel;
                    if(projectOwner != null)
                    {
                        emailModel.To.Add(new MailAddress(projectOwner.Email));
                    }
                }               

                foreach (string email in emailsList)
                {
                    if (String.IsNullOrWhiteSpace(email))
                    {
                        continue;
                    }

                    emailModel.To.Add(new MailAddress(email.Trim()));
                }

                emailModel.RenderTextVersion = true;
                emailModel.BodyTextVersion = RenderView(this, "SendEmailDiscountRequest", emailModel);

                emailModel.RenderTextVersion = false;
                emailModel.BodyHtmlVersion = RenderView(this, "SendEmailDiscountRequest", emailModel);

                new EmailServices().SendEmail(emailModel);

                //return RedirectToAction("Quote", new QuoteModel { ProjectId = model.ProjectId, QuoteId = model.QuoteId });
                string url = "/v2/#/quote/" + model.QuoteId + "/existingRecord";
                return Redirect(url);
            }

            ModelState.Clear();
           
            return View("DiscountRequest", this.ServiceResponse.Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.ApproveDiscounts })]
        public ActionResult DiscountRequestApprove(DiscountRequestModel model)
        {
            this.ServiceResponse = discountRequestService.Approve(this.CurrentUser, model);

            this.ShowKeyMessagesOnPage = true;

            if (ProcessServiceResponse(this.ServiceResponse))
            {
                //force email to be sent to user
                DiscountRequestModel DARForEmail = this.ServiceResponse.Model as DiscountRequestModel;
                DARForEmail.ShouldSendEmail = true;

                CreateDarPdfForSendMail(DARForEmail);

                SendApprovalRejectionEmail(DARForEmail);

                return RedirectToAction("DiscountRequests", "UserDashboard");
            }

            return View("DiscountRequest", this.ServiceResponse.Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.ApproveDiscounts })]
        public ActionResult DiscountRequestApproveModification(DiscountRequestModel model, decimal discountRequestPercent)
        {
            //this.ServiceResponse = discountRequestService.Approve(this.CurrentUser, model);

            this.ShowKeyMessagesOnPage = true;

            //if (ProcessServiceResponse(this.ServiceResponse))
            //{
            //    //force email to be sent to user
            //    DiscountRequestModel DARForEmail = this.ServiceResponse.Model as DiscountRequestModel;
            //    DARForEmail.ShouldSendEmail = true;

            //    SendApprovalRejectionEmail(DARForEmail);

            //    return RedirectToAction("DiscountRequests", "UserDashboard");
            //}

            return View("DiscountRequest", this.ServiceResponse.Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.RequestDiscounts })]
        public ActionResult DiscountRequestDelete(DiscountRequestModel model)
        {
            this.ServiceResponse = discountRequestService.Delete(this.CurrentUser, model);

            this.ShowKeyMessagesOnPage = true;

            if (ProcessServiceResponse(this.ServiceResponse))
            {
                //force email to be sent to user
                DiscountRequestModel DARForEmail = this.ServiceResponse.Model as DiscountRequestModel;
                DARForEmail.ShouldSendEmail = true;

                SendApprovalRejectionEmail(DARForEmail);

                //return RedirectToAction("Quote", new QuoteModel { ProjectId = model.ProjectId, QuoteId = model.QuoteId });

                string url = "/v2/#/quote/" + model.QuoteId + "/existingRecord";
                return Redirect(url);
            }

            return View("DiscountRequest", this.ServiceResponse.Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.ApproveDiscounts })]
        public ActionResult DiscountRequestReject(DiscountRequestModel model)
        {
            this.ServiceResponse = discountRequestService.Reject(this.CurrentUser, model);

            this.ShowKeyMessagesOnPage = true;

            if (ProcessServiceResponse(this.ServiceResponse))
            {
                //email is sent to user if checkbox in DAR form is selected
                SendApprovalRejectionEmail(this.ServiceResponse.Model as DiscountRequestModel);

                return RedirectToAction("DiscountRequests", "UserDashboard");
            }

            return View("DiscountRequest", this.ServiceResponse.Model);
        }

        private void SendApprovalRejectionEmail(DiscountRequestModel model)
        {

            string root = Server.MapPath("~");
            string parent = System.IO.Path.GetDirectoryName(root);
            string grandParent = System.IO.Path.GetDirectoryName(parent);

            string _last5DigitsOfProjectId = model.ProjectId.ToString().Substring(model.ProjectId.ToString().Length - 5);

            string DARPdfFile = "Daikin City Discount Request " +
                                     DateTime.Now.ToString("MM-dd-yyyy") +
                                     "-" +
                                    _last5DigitsOfProjectId + ".pdf";

            string DARPdfFilePath = grandParent + "/CustomerDataFiles/DiscountRequestFiles/" + model.QuoteId + "/" + DARPdfFile;

            var emailModel = new SendEmailApprovalModel();
            emailModel.HelpLink = "mailto:project.desk@daikincomfort.com";

            emailModel.Subject = string.Format("The status of a DPO Discount request has changed");

            emailModel.Reason = model.ResponseNotes;
            emailModel.ProjectId = model.ProjectId;
            emailModel.ProjectName = model.Project.Name;
            emailModel.QuoteTitle = model.Quote.Title;
            emailModel.TotalNet = model.Quote.TotalNet;
            emailModel.Approved = (model.DiscountRequestStatusTypeId == (byte)DiscountRequestStatusTypeEnum.Approved);
            emailModel.ModifierName = model.DiscountRequestStatusModifiedBy;

            //only sent Dar attachment when approved Dar
            if (model.DiscountRequestStatusTypeId != (byte)DiscountRequestStatusTypeEnum.Rejected &&
                model.DiscountRequestStatusTypeId != (byte)DiscountRequestStatusTypeEnum.Deleted)
            {
                emailModel.DARAttachmentFile = DARPdfFilePath;
                emailModel.DARAttachmentFileName = DARPdfFile;
            }

            UserSessionModel user = new UserSessionModel();

            if (model.ProjectOwnerId != null)
            {
                user = new AccountServices().GetUserSessionModel(model.ProjectOwnerId.Value).Model as UserSessionModel;
            }
            else
            {
                user = new AccountServices().GetUserSessionModel(model.Project.OwnerId.Value).Model as UserSessionModel;
            }

            emailModel.ProjectOwnerName = user.FirstName + " " + user.LastName;
            emailModel.ProjectDate = model.Project.ProjectDate;

            var business = new BusinessServices().GetBusinessModel(user, user.BusinessId, false).Model as BusinessModel;

            emailModel.BusinessName = business.BusinessName;
            emailModel.RequestedDiscount = model.RequestedDiscount;
            emailModel.ApprovedDiscount = model.ApprovedDiscount;

            emailModel.From = new MailAddress(Utilities.Config("dpo.sys.email.discountrequest"), "DPO Project Desk");

            if (model.ShouldSendEmail == true)
            {
                emailModel.To.Add(new MailAddress(user.Email, user.DisplayName));
            }

            emailModel.To.Add(emailModel.From);

            if (!string.IsNullOrEmpty(business.AccountManagerEmail))
            {
                emailModel.To.Add(new MailAddress(business.AccountManagerEmail));
            }

            if (!string.IsNullOrEmpty(business.AccountOwnerEmail))
            {
                emailModel.To.Add(new MailAddress(business.AccountOwnerEmail));
            }

            List<string> emailsList = new List<string>();

            if (model.EmailsList != null && model.EmailsList.Length > 0)
            {
                emailsList = model.EmailsList.ToString().Split(',', ';').ToList();
            }

            foreach (string email in emailsList)
            {
                if (String.IsNullOrWhiteSpace(email))
                {
                    continue;
                }
                emailModel.To.Add(new MailAddress(email.Trim()));
            }

            emailModel.RenderTextVersion = true;
            emailModel.BodyTextVersion = RenderView(this, "SendEmailDiscountRequestApproval", emailModel);

            emailModel.RenderTextVersion = false;
            emailModel.BodyHtmlVersion = RenderView(this, "SendEmailDiscountRequestApproval", emailModel);

            new EmailServices().SendEmail(emailModel);
        }

        public void CreateDarPdfForSendMail(DiscountRequestModel model)
        {
            long quoteId = model.QuoteId.Value;
            long projectId = model.ProjectId.Value;

            var urlAuth = Utilities.DocumentServerURL();
            var controller = string.Format("{0}/{1}", urlAuth, "ProjectDashboard");

            bool showCostPricing = true;

            var urlDiscountRequestFormBody = string.Format("{0}/{1}?discountRequestId={2}&projectId={3}&quoteId={4}&showCostPricing={5}",
                                             controller, "DiscountRequestPrint", 
                                             model.DiscountRequestId, projectId, quoteId, showCostPricing);

            var urlDiscountRequestFormHeader = string.Format("{0}/{1}", 
                                             controller, "DiscountRequestPrintHeader", projectId, quoteId);

            var urlDiscountRequestFormFooter = string.Format("{0}/{1}", controller, "DiscountRequestPrintFooter");

            var pdf = new PdfConvertor();

            var web = new WebClientLocal(System.Web.HttpContext.Current);

            pdf.Options.NoLink = false;
            pdf.Options.HeaderHtmlFormat = web.DownloadString(urlDiscountRequestFormHeader);
            pdf.Options.FooterHtmlFormat = web.DownloadString(urlDiscountRequestFormFooter);
            pdf.Options.FooterHtmlPosition = pdf.Options.OutputArea.Bottom - 1.25f;

            pdf.Options.OutputArea = new System.Drawing.RectangleF(0f, 1.25f, pdf.Options.OutputArea.Width, pdf.Options.OutputArea.Height - 2.5f);
            pdf.AppendHtml(web.DownloadString(urlDiscountRequestFormBody));

            string root = System.Web.HttpContext.Current.Server.MapPath("~");
            string parent = System.IO.Path.GetDirectoryName(root);
            string grandParent = System.IO.Path.GetDirectoryName(parent);

            string _last5DigitsOfProjectId = model.ProjectId.ToString()
                                            .Substring(model.ProjectId.ToString().Length - 5);

            string nameFile = "Daikin City Discount Request " +
                               DateTime.Now.ToString("MM-dd-yyyy") +
                               "-" +
                               _last5DigitsOfProjectId + ".pdf";

            string subPath = grandParent + "/CustomerDataFiles/DiscountRequestFiles/" + model.QuoteId;

            bool exists = System.IO.Directory.Exists(subPath);

            if (!exists)
                System.IO.Directory.CreateDirectory(subPath);

            string filePath = grandParent + "/CustomerDataFiles/DiscountRequestFiles/" + model.QuoteId + "/" + nameFile;

            pdf.Document.Save(filePath);

        }
    }
}