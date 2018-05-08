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
using DPO.Common.Models;

namespace DPO.Web.Controllers
{
    public partial class ProjectDashboardController
    {
        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.RequestCommission, SystemAccessEnum.ApprovedRequestCommission })]
        public ActionResult CommissionRequest(long? commissionRequestId, long? projectId, long? quoteId)
        {
            this.ServiceResponse = commissionRequestService.GetCommissionRequestModel(this.CurrentUser, new CommissionRequestModel { CommissionRequestId = commissionRequestId, ProjectId = projectId, QuoteId = quoteId });
            
            if (ProcessServiceResponse(this.ServiceResponse))
            {
                this.RouteData.Values["action"] = "CommissionRequest";

                return View("CommissionRequest", this.ServiceResponse.Model);
            }

            return new EmptyResult();
        }

        [Authorise(Accesses = new[] { SystemAccessEnum.RequestCommission, SystemAccessEnum.ApprovedRequestCommission })]
        public ActionResult GetCommissionRequestMultiplier(decimal? vrvCommissionPercentage, decimal? splitCommissionPercentage, long? projectId, long? quoteId)
        {
            List<decimal> commissionMultipliers = commissionRequestService.GetCommissionRequestMultiplier(vrvCommissionPercentage, splitCommissionPercentage);

            if (commissionMultipliers != null && commissionMultipliers.Count > 0)
            {
                this.ServiceResponse = commissionRequestService.GetCommissionRequestModel(this.CurrentUser, new CommissionRequestModel { ProjectId = projectId, QuoteId = quoteId });

                if (ProcessServiceResponse(this.ServiceResponse))
                {
                    CommissionRequestModel model = this.ServiceResponse.Model as CommissionRequestModel;

                    model.RequestedMultiplierVRV = commissionMultipliers[0];
                    model.RequestedMultiplierSplit = commissionMultipliers[1];

                    this.RouteData.Values["action"] = "CommissionCalculation";

                    return Json(model, JsonRequestBehavior.AllowGet);
                }
            }

            return PartialView("CommissionCalculation");
        }

        [Authorise(Accesses = new[] { SystemAccessEnum.RequestCommission, SystemAccessEnum.ApprovedRequestCommission })]
        public ActionResult GetCommissionRequestPercentage(decimal? vrvCommissionMultiplier, decimal? splitCommissionMultiplier, long? projectId, long? quoteId)
        {
            List<decimal> commissionPercentages = commissionRequestService.GetCommissionRequestPercentage(vrvCommissionMultiplier, splitCommissionMultiplier);

            if (commissionPercentages != null && commissionPercentages.Count > 0)
            {
                this.ServiceResponse = commissionRequestService.GetCommissionRequestModel(this.CurrentUser, new CommissionRequestModel { ProjectId = projectId, QuoteId = quoteId });

                if (ProcessServiceResponse(this.ServiceResponse))
                {
                    CommissionRequestModel model = this.ServiceResponse.Model as CommissionRequestModel;

                    model.RequestedCommissionPercentageVRV = commissionPercentages[0];
                    model.RequestedCommissionPercentageSplit = commissionPercentages[1];

                    this.RouteData.Values["action"] = "CommissionCalculation";

                    return Json(model, JsonRequestBehavior.AllowGet);
                }
            }

            return PartialView("CommissionCalculation");
        }

        [Authorise(Accesses = new[] { SystemAccessEnum.RequestCommission, SystemAccessEnum.ApprovedRequestCommission })]
        public ActionResult CommissionRequestExport(long? commissionRequestId, long? projectId, long? quoteId, bool showCostPricing = false)
        {
            this.ServiceResponse = commissionRequestService.GetCommissionRequestModel(this.CurrentUser, new CommissionRequestModel { CommissionRequestId = commissionRequestId, ProjectId = projectId, QuoteId = quoteId });

            if (this.ServiceResponse.IsOK)
            {
                if (this.CurrentUser.ShowPrices == false)
                {
                    showCostPricing = false;
                }

                var model = this.ServiceResponse.Model as CommissionRequestModel;
                
                var stream = commissionRequestService.GetCommissionRequestExportExcelFile(model, showCostPricing);
               
                this.Response.AddHeader("Content-Disposition", "inline; filename=Commission Request Export.xls");
                this.Response.AddHeader("Cache-Control", "no-cache");
                this.Response.AddHeader("Content-Type", MimeMapping.GetMimeMapping("Commission Request Export.xls"));

                stream.WriteTo(this.Response.OutputStream);

                return new EmptyResult();
            }

            return new EmptyResult();

        }

        [Authorise(Accesses = new[] { SystemAccessEnum.RequestCommission, SystemAccessEnum.ApprovedRequestCommission })]
        public ActionResult CommissionRequestExportWithCostPricing(long? commissionRequestId, long? projectId, long? quoteId)
        {
            return RedirectToAction("CommissionRequestExport", new { commissionRequestId = commissionRequestId, projectId = projectId, quoteId = quoteId, showCostPricing = true });
        }

        [Authorise(Accesses = new[] { SystemAccessEnum.RequestCommission, SystemAccessEnum.ApprovedRequestCommission })]
        public ActionResult CommissionRequestPrint(long? commissionRequestId, long? projectId, long? quoteId, bool showCostPricing = false)
        {
            this.ServiceResponse = commissionRequestService.GetCommissionRequestModel(this.CurrentUser, new CommissionRequestModel { CommissionRequestId = commissionRequestId, ProjectId = projectId, QuoteId = quoteId });

            if (ProcessServiceResponse(this.ServiceResponse))
            {
                this.RouteData.Values["action"] = "CommissionRequestPrint";

                //earliest opportunity to tell view to show or hide pricing
                ViewData["ShowCostPricing"] = showCostPricing;

                return View("CommissionRequestPrint", this.ServiceResponse.Model);
            }

            return new EmptyResult();
        }

        public ActionResult CommissionRequestPrintFooter()
        {
            return View("CommissionRequestPrintFooter", "_PrintPartialLayout");
        }

        public ActionResult CommissionRequestPrintHeader()
        {
            return View("CommissionRequestPrintHeader", "_PrintPartialLayout");
        }

        [Authorise(Accesses = new[] { SystemAccessEnum.RequestCommission, SystemAccessEnum.ApprovedRequestCommission })]
        public ActionResult CommissionRequestPrintWithCostPricing(long? commissionRequestId, long? projectId, long? quoteId)
        {
            return RedirectToAction("CommissionRequestPrint", new { commissionRequestId = commissionRequestId, projectId = projectId, quoteId = quoteId, showCostPricing = true });
        }

        #region Commission Calculation

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.RequestCommission, SystemAccessEnum.ApprovedRequestCommission })]
        public ActionResult CommissionCalculation(long? projectId, long? quoteId, long? commissionRequestId, byte? commissionRequestStatusTypeId)
        {
            byte commissionRequestStatus = 0;

            if (commissionRequestStatusTypeId != null)
            {
                commissionRequestStatus = commissionRequestStatusTypeId.Value;
            }

            if (commissionRequestStatus == (byte)CommissionRequestStatusTypeEnum.Approved ||
                commissionRequestStatus == (byte)CommissionRequestStatusTypeEnum.Pending ||
                commissionRequestStatus == (byte)CommissionRequestStatusTypeEnum.NewRecord)
            {
                this.ServiceResponse = commissionRequestService.GetCommissionRequestModel(this.CurrentUser, new CommissionRequestModel { ProjectId = projectId, QuoteId = quoteId, CommissionRequestId = commissionRequestId }, new CommissionCalculationModel());
            }
            else
            {
                this.ServiceResponse = commissionRequestService.GetCommissionRequestModel(this.CurrentUser, new CommissionRequestModel { ProjectId = projectId, QuoteId = quoteId }, new CommissionCalculationModel());
            }

            if (ProcessServiceResponse(this.ServiceResponse))
            {
                this.RouteData.Values["action"] = "CommissionCalculation";
                return View("CommissionCalculation", this.ServiceResponse.Model);
            }

            return PartialView("CommissionCalculation");
        }

        [Authorise(Accesses = new[] { SystemAccessEnum.RequestCommission, SystemAccessEnum.ApprovedRequestCommission })]
        public ActionResult CommissionCalculation(CommissionRequestModel model, FormCollection form, byte? commissionRequestStatusTypeId)
        {
            this.ServiceResponse = commissionRequestService.CalculateCommission(this.CurrentUser, model);
          
            if (ProcessServiceResponse(this.ServiceResponse))
            {
                return RedirectToAction("QuoteItems", new QuoteModel { ProjectId = model.ProjectId, QuoteId = model.QuoteId });
            }

            // TODO:  This may need to change to show the pop-up with the right errors
            return RedirectToAction("QuoteItems", new QuoteModel { ProjectId = model.ProjectId, QuoteId = model.QuoteId });
        }

        #endregion Commission Calculation

        [Authorise(Accesses = new[] { SystemAccessEnum.RequestCommission, SystemAccessEnum.ApprovedRequestCommission })]
        public ActionResult RedirectToQuoteItems(long? ProjectId, long? QuoteId)
        {
           
          return Json(new { result = "Redirect", url = Url.Action("QuoteItems", "ProjectDashboard", new { ProjectId = ProjectId, QuoteId = QuoteId }) });
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ActionResult BackToActiveQuote(long? projectId, long? quoteId, bool? showImportProductPopup)
        {
            this.ServiceResponse = quoteService.GetQuoteModel(this.CurrentUser, projectId, quoteId);

            var model = this.ServiceResponse.Model as QuoteModel;

            if (ProcessServiceResponse(this.ServiceResponse))
            {
                this.RouteData.Values["action"] = "QuoteItems";
            }

            model.ShowImportProductPopup = showImportProductPopup ?? false;

            return ((this.IsPostRequest) ? (ViewResultBase)PartialView("QuoteItems", model) : View("QuoteItems", model));

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.RequestCommission, SystemAccessEnum.ApprovedRequestCommission })]
        public ActionResult CommissionRequest(CommissionRequestModel model)
        {
            
            model.CompetitorQuoteFile = Request.Files["CompetitorQuoteFile"];

            model.CompetitorLineComparsionFile = Request.Files["CompetitorLineComparsionFile"];

            model.CompetitorQuoteFileName = (model.CompetitorQuoteFile == null || model.CompetitorQuoteFile.FileName == "") ? model.CompetitorQuoteFileName : model.CompetitorQuoteFile.FileName;

            model.CompetitorLineComparsionFileName = (model.CompetitorLineComparsionFile == null || model.CompetitorLineComparsionFile.FileName == "") ? model.CompetitorLineComparsionFileName : model.CompetitorLineComparsionFile.FileName;
            
            model.OrderDeliveryDate = (model.Project == null) ? DateTime.Now : model.Project.EstimatedDelivery;

            List<string> emailsList = new List<string>();

            if (model.EmailsList != null && model.EmailsList.Length > 0)
            {
                emailsList = model.EmailsList.ToString().Split(',', ';').ToList();
            }

            EmailServices emailService = new EmailServices();
            List<string> InvalidEmails = this.commissionRequestService.GetInvalidEmails(emailsList);

            if (InvalidEmails.Count > 0)
            {
                foreach (string email in InvalidEmails)
                {
                    model.InvalidEmails.Add(email);
                }
                model.IsValidEmails = false;
            }

            // set the RequestedCommission equals to Approved Commission for Daikin Super User Testing Only
            if (model.RequestedCommissionTotal == 0)
            {
                model.RequestedCommissionTotal = model.ApprovedCommissionTotal;
            }

            this.ServiceResponse = commissionRequestService.PostModel(this.CurrentUser, model);
            
            if (ProcessServiceResponse(this.ServiceResponse))
            {
                model.Project.EstimatedDelivery = model.OrderDeliveryDate;

                model.IsValidEmails = true;

                this.ServiceResponse = projectService.PostModel(this.CurrentUser, model.Project);

                var emailModel = this.commissionRequestService.GetCommissionRequestSendEmailModel(model);

                emailModel.HelpLink = "mailto:project.desk@daikincomfort.com";

                emailModel.Subject = string.Format("A DPO Commission request has been submitted");

                emailModel.From = new MailAddress(Utilities.Config("dpo.sys.email.commissionrequest"), "DPO Commission Request");
                emailModel.To.Add(new MailAddress(Utilities.Config("dpo.sys.email.commissionrequest"), "Daikin Project Desk"));

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
                else if(model.Project != null && model.Project.OwnerId != null)
                {
                    UserServices userService = new UserServices();
                    UserModel projectOwner = userService.GetUserModel(CurrentUser, model.Project.OwnerId, true, true).Model as UserModel;
                    if (projectOwner != null)
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
                emailModel.BodyTextVersion = RenderView(this, "SendEmailCommissionRequest", emailModel);

                emailModel.RenderTextVersion = false;
                emailModel.BodyHtmlVersion = RenderView(this, "SendEmailCommissionRequest", emailModel);

                new EmailServices().SendEmail(emailModel);

                //return RedirectToAction("QuoteItems", new QuoteModel { ProjectId = model.ProjectId, QuoteId = model.QuoteId });
                string url = "/v2/#/quote/" + model.QuoteId + "/existingRecord";
                return Redirect(url);
            }

            ModelState.Clear();

            return View("CommissionRequest", this.ServiceResponse.Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.ApprovedRequestCommission })]
        public ActionResult CommissionRequestApprove(CommissionRequestModel model) 
        {
            this.ServiceResponse = commissionRequestService.Approve(this.CurrentUser, model);

            this.ShowKeyMessagesOnPage = true;

            if (ProcessServiceResponse(this.ServiceResponse))
            {
                //force email to be sent to user
                CommissionRequestModel CommissionRequestForEmail = this.ServiceResponse.Model as CommissionRequestModel;
                CommissionRequestForEmail.ShouldSendEmail = true;

                SendApprovalRejectionEmailForCommissionRequest(CommissionRequestForEmail);  

                return RedirectToAction("CommissionRequests", "UserDashboard");
            }

            return View("CommissionRequest", this.ServiceResponse.Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.ApprovedRequestCommission })]
        public ActionResult CommissionRequestApproveModification(CommissionRequestModel model, decimal commissionRequestPercent)
        {
            this.ServiceResponse = commissionRequestService.Approve(this.CurrentUser, model);

            this.ShowKeyMessagesOnPage = true;
           
            return View("CommissionRequest", this.ServiceResponse.Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.RequestCommission })]
        public ActionResult CommissionRequestDelete(CommissionRequestModel model)
        {
            //this.ServiceResponse = commissionRequestService.GetCommissionRequestModel(this.CurrentUser, new CommissionRequestModel { CommissionRequestId = model.CommissionRequestId }, null);

            //CommissionRequestModel model = new CommissionRequestModel();

            //if (this.ServiceResponse.IsOK)
            //{
            //    model = this.ServiceResponse.Model as CommissionRequestModel;
            //}

            this.ServiceResponse = commissionRequestService.Delete(this.CurrentUser, model);

            this.ShowKeyMessagesOnPage = true;

            if (ProcessServiceResponse(this.ServiceResponse))
            {
                //force email to be sent to user
                CommissionRequestModel CommissionRequestForEmail = this.ServiceResponse.Model as CommissionRequestModel;
                CommissionRequestForEmail.ShouldSendEmail = true;

               SendApprovalRejectionEmailForCommissionRequest(CommissionRequestForEmail);

                //return RedirectToAction("Quote", new QuoteModel { ProjectId = model.ProjectId, QuoteId = model.QuoteId });

                string url = "/v2/#/quote/" + model.QuoteId + "/existingRecord";
                return Redirect(url);

            }
             
            return View("CommissionRequest", this.ServiceResponse.Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.ApprovedRequestCommission })]
        public ActionResult CommissionRequestReject(CommissionRequestModel model)
        {
            this.ServiceResponse = commissionRequestService.Reject(this.CurrentUser, model);

            this.ShowKeyMessagesOnPage = true;

            if (ProcessServiceResponse(this.ServiceResponse))
            {
                //email is sent to user if checkbox in DAR form is selected
                SendApprovalRejectionEmailForCommissionRequest(this.ServiceResponse.Model as CommissionRequestModel); 

                return RedirectToAction("CommissionRequests", "UserDashboard");
            }

            return View("CommissionRequest", this.ServiceResponse.Model);
        }

        private void SendApprovalRejectionEmailForCommissionRequest(CommissionRequestModel model)
        {
            var emailModel = new SendEmailApprovalModel();
            emailModel.HelpLink = "mailto:project.desk@daikincomfort.com";

            emailModel.Subject = string.Format("The status of a DPO Commission request has changed");

            emailModel.Reason = model.ResponseNotes;
            emailModel.ProjectId = model.ProjectId;
            emailModel.ProjectName = model.Project.Name;
            emailModel.QuoteTitle = model.Quote.Title;
            emailModel.TotalNet = model.Quote.TotalNet;
            emailModel.Approved = (model.CommissionRequestStatusTypeId == (byte)CommissionRequestStatusTypeEnum.Approved);
            emailModel.ModifierName = model.CommissionRequestStatusModifiedBy;
          

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
            emailModel.RequestedCommission = model.RequestedCommissionTotal;
            emailModel.ApprovedCommission = model.ApprovedCommissionTotal;
            emailModel.ApprovedTotalCommission = model.ApprovedCommissionPercentage;
            emailModel.RequestedCommissionPercentage = model.RequestedCommissionPercentage;
            emailModel.ApprovedCommissionPercentage = model.ApprovedCommissionPercentage;
            emailModel.RequestedMultiplier = model.RequestedMultiplier;
            emailModel.ApprovedMultiplier = model.ApprovedMultiplier;
            emailModel.ApprovedTotalNet = model.TotalRevised;
            emailModel.TotalNet = model.Quote.TotalNet;
            emailModel.TotalList = model.Quote.TotalList;

            emailModel.From = new MailAddress(Utilities.Config("dpo.sys.email.commissionrequest"), "DPO Project Desk");

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
            emailModel.BodyTextVersion = RenderView(this, "SendEmailCommissionRequestApproval", emailModel);

            emailModel.RenderTextVersion = false;
            emailModel.BodyHtmlVersion = RenderView(this, "SendEmailCommissionRequestApproval", emailModel);

            new EmailServices().SendEmail(emailModel);
        }
    }
}