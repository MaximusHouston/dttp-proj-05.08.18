using AutoMapper;
using DPO.Common;
using DPO.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using DPO.Services.Light;
using DPO.Model.Light;
using System.Collections.Specialized;
using System.Net.Http.Formatting;
using log4net;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Net.Mail;

namespace DPO.Web.Controllers
{
    [Authorize]
    public class DiscountRequestController : BaseApiController
    {
        public DiscountRequestServices DiscountRequestService = new DiscountRequestServices();
        public ProjectServices ProjectService = new ProjectServices();

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.RequestDiscounts })]
        public ServiceResponse GetDiscountRequest(long? discountRequestId, long? projectId, long? quoteId) {
            return DiscountRequestService.GetDiscountRequestModel(this.CurrentUser, new DiscountRequestModel { DiscountRequestId = discountRequestId, ProjectId = projectId, QuoteId = quoteId });
        }

        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.RequestDiscounts })]
        public HttpResponseMessage UploadCompetitorQuoteFile()
        {
            var response = new HttpResponseMessage();
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                var file = httpRequest.Files[0];

                var competitorQuoteFile = new HttpPostedFileWrapper(file);

                if (competitorQuoteFile != null && competitorQuoteFile.ContentLength > 0)
                {
                    long quoteId = Convert.ToInt64(httpRequest.Form["QuoteId"]);
                    var message = Utilities.SavePostedFile(competitorQuoteFile, Utilities.GetDARDirectory(quoteId), 512000);
                    if (message != null)
                    {
                        message += "Please select difference file type";
                        response = Request.CreateResponse(HttpStatusCode.NotAcceptable);
                        response.ReasonPhrase = message;
                    }
                }
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.NotAcceptable);
                response.ReasonPhrase = "Import file is missing!";
            }

            return response;
        }

        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.RequestDiscounts })]
        public HttpResponseMessage UploadCompetitorLineComparsionFile()
        {
            var response = new HttpResponseMessage();
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                var file = httpRequest.Files[0];

                var competitorLineComparsionFile = new HttpPostedFileWrapper(file);

                if (competitorLineComparsionFile != null && competitorLineComparsionFile.ContentLength > 0)
                {
                    long quoteId = Convert.ToInt64(httpRequest.Form["QuoteId"]);
                    var message = Utilities.SavePostedFile(competitorLineComparsionFile, Utilities.GetDARDirectory(quoteId), 512000);
                    if (message != null)
                    {
                        message += "Please select difference file type";
                        response = Request.CreateResponse(HttpStatusCode.NotAcceptable);
                        response.ReasonPhrase = message;
                    }
                }
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.NotAcceptable);
                response.ReasonPhrase = "Import file is missing!";
            }

            return response;
        }


        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.RequestDiscounts })]
        public ServiceResponse PostDiscountRequest(DiscountRequestModel model)
        {
            model.OrderDeliveryDate = model.Project.EstimatedDelivery;

            //Handle Emails
            List<string> emailsList = new List<string>();

            if (model.EmailsList != null && model.EmailsList.Length > 0)
            {
                emailsList = model.EmailsList.ToString().Split(',', ';').ToList();
            }

            EmailServices emailService = new EmailServices();

            List<string> InvalidEmails = DiscountRequestService.GetInvalidEmails(emailsList);

            if (InvalidEmails.Count > 0)
            {
                foreach (string email in InvalidEmails)
                {
                    model.InvalidEmails.Add(email);
                }
                model.IsValidEmails = false;
            }

            // set the RequestedDiscount equals to Approved Discount for Daikin Super User Testing Only // TODO: not sure what this is for ...
            //if (model.RequestedDiscount == 0)
            //{
            //    model.RequestedDiscount = model.ApprovedDiscount;
            //}

            this.ServiceResponse = DiscountRequestService.PostModel(this.CurrentUser, model);

            if (this.ServiceResponse.IsOK) {
                var response = ProjectService.PostModel(this.CurrentUser, model.Project);


                //=============Email============
                var emailModel = DiscountRequestService.GetDiscountRequestSendEmailModel(model);

                //if (emailModel.discountRequest.RequestedDiscount == 0)
                //{
                //    var quoteModel = quoteService.GetQuoteModel(this.CurrentUser, model.ProjectId, model.QuoteId).Model as QuoteModel;
                //    if (quoteModel != null)
                //    {
                //        emailModel.discountRequest.RequestedDiscount = quoteModel.DiscountPercentage * 100;
                //    }
                //}

                emailModel.HelpLink = "mailto:project.desk@daikincomfort.com";

                emailModel.Subject = string.Format("A DPO Discount request has been submitted");

                emailModel.From = new MailAddress(Utilities.Config("dpo.sys.email.discountrequest"), "DPO Discount Request");
                emailModel.To.Add(new MailAddress(Utilities.Config("dpo.sys.email.discountrequest"), "Daikin Project Desk"));


                //Add Account Manager - AccountOwner
                if (!string.IsNullOrEmpty(emailModel.AccountManagerEmail))
                {
                    emailModel.To.Add(new MailAddress(emailModel.AccountManagerEmail));
                }

                if (!string.IsNullOrEmpty(emailModel.AccountOwnerEmail) && emailModel.AccountOwnerEmail != emailModel.AccountManagerEmail)
                {
                    emailModel.To.Add(new MailAddress(emailModel.AccountOwnerEmail));
                }


                //Add Project Owner
                if (model.ProjectOwnerId != null)
                {
                    UserServices userService = new UserServices();
                    UserModel projectOwner = userService.GetUserModel(CurrentUser, model.ProjectOwnerId, true, true).Model as UserModel;
                    if (projectOwner != null)
                    {
                        emailModel.To.Add(new MailAddress(projectOwner.Email));
                    }
                }

                //Add Email List
                foreach (string email in emailsList)
                {
                    if (String.IsNullOrWhiteSpace(email))
                    {
                        continue;
                    }

                    emailModel.To.Add(new MailAddress(email.Trim()));
                }

                //Render Email Body
                emailModel.RenderTextVersion = true;
                //emailModel.BodyTextVersion = RenderView(this, "SendEmailDiscountRequest", emailModel);

                emailModel.RenderTextVersion = false;
                //emailModel.BodyHtmlVersion = RenderView(this, "SendEmailDiscountRequest", emailModel);

                new EmailServices().SendEmail(emailModel);

                //Redirect to quote
                //string url = "/v2/#/quote/" + model.QuoteId + "/existingRecord";
                //return Redirect(url);

            }
            

            return this.ServiceResponse;
        }

    }
}