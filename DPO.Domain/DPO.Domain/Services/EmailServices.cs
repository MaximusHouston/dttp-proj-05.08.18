using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPO.Common;
using DPO.Common.Interfaces;
using DPO.Data;
using DPO.Domain.Properties;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Collections;
using System.Web.Mvc;
using System.Net.Mail;
using System.Configuration;
using System.Runtime.Remoting.Messaging;
using System.ComponentModel;
using log4net;

namespace DPO.Domain
{
    public partial class EmailServices : BaseServices
    {
        private static ILog log;
        public EmailServices() : base() {
            log = Log;
        }
        public EmailServices(DPOContext context) : base(context) { }

       
        public void SendEmail(SendEmailModel model)
        {
            AsyncMethodCaller caller = new AsyncMethodCaller(SendMailInSeperateThread);
            AsyncCallback callbackHandler = new AsyncCallback(AsyncCallback);
            caller.BeginInvoke(model, callbackHandler, null);
        }

        private delegate void AsyncMethodCaller(SendEmailModel model);

        private void AsyncCallback(IAsyncResult ar)
        {
            try
            {
                AsyncResult result = (AsyncResult)ar;
                AsyncMethodCaller caller = (AsyncMethodCaller)result.AsyncDelegate;
                caller.EndInvoke(ar);
            }
            catch (Exception e)
            {
                Utilities.ErrorLog(e);
            }
        }

        private static void SendEmailToTeamWhenFailToSendEmailOnOrder(object sender, AsyncCompletedEventArgs e)
        {
            var emailModel = new DPO.Common.SendEmailModel();

            SendEmailModel sendEmailModel = emailModel;

            List<string> fromEmails = Utilities.Config("dpo.sys.email.orderSendEmailError").Split(',').ToList();

            emailModel.Subject = string.Format("send email Order Submit error");

            emailModel.From = new MailAddress(fromEmails.First(), "Send Email Error");

            foreach (string email in fromEmails)
                emailModel.To.Add(new MailAddress(email, "Daikin Project Desk"));

            string emailMessage = string.Format(@"An error occured when try to send email for Order Submit.Below are the Order Details: <br />" +
                                                 "The below are the error details: <br /> {0}",
                                                 (e.Error.InnerException != null) ? e.Error.InnerException.Message : e.Error.Message); 

            sendEmailModel.Subject = "Order Submit Email Failure Notification.";
            sendEmailModel.RenderTextVersion = true;
            sendEmailModel.BodyTextVersion = emailMessage;

            new EmailServices().SendEmail(sendEmailModel);
        }
        private static void SendCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {
            // Get the unique identifier for this asynchronous operation.
            String token = (string)e.UserState;

            if (e.Cancelled)
            {
                System.Diagnostics.Debug.WriteLine("[{0}] Send canceled.", token);
            }
            if (e.Error != null)
            {
                System.Diagnostics.Debug.WriteLine("[{0}] {1}", token, e.Error.ToString());

                log.ErrorFormat("Send Order submit email failed. {0}", e.Error.Message);
                log.ErrorFormat("Send Order submit email failed. {0}", (e.Error.InnerException != null) ? e.Error.InnerException.Message : e.Error.Message);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Message sent.");
                log.Debug("Send Order submit email completed successfully");
                ((SmtpClient)sender).Dispose();
            }
        }

        private void SendMailInSeperateThread(SendEmailModel model)
        {
            this.Response = new ServiceResponse();

            MailMessage mail = new MailMessage();

            mail.From = model.From;

            foreach (var ccEmail in model.To)
            {
                mail.CC.Add(ccEmail);
            }

            mail.Subject = model.Subject;
            mail.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");

            if (model.OtherAttachmentFiles != null)
            {
                foreach (string file in model.OtherAttachmentFiles)
                {
                    Attachment fileAttachment = new Attachment(file);
                    string[] fileName = file.Select(name => System.IO.Path.GetFileName(file)).ToArray();
                    fileAttachment.Name = fileName[0];
                    mail.Attachments.Add(fileAttachment);
                }
            }

            if (model.DARAttachmentFile != null &&
                model.DARAttachmentFileName != null &&
                model.DARAttachmentFileName.Length > 4)
            {

                if (System.IO.File.Exists(model.DARAttachmentFile))
                {
                    System.Net.Mail.Attachment DARAttachment = new System.Net.Mail.Attachment(model.DARAttachmentFile);
                    DARAttachment.Name = model.DARAttachmentFileName;
                    mail.Attachments.Add(DARAttachment);
                }
               

                if (!mail.Attachments.Any(att => att.Name.Contains(model.DARAttachmentFileName)))
                {
                    this.Response.AddError("Can not attach Discount Request file.");
                }
            }

            if (model.COMAttachmentFile != null &&
                model.COMAttachmentFile != null &&
                model.COMAttachmentFileName.Length > 4)
            {
                if (System.IO.File.Exists(model.COMAttachmentFile) ||
                   System.IO.File.Exists(System.Web.Hosting.HostingEnvironment.MapPath(model.COMAttachmentFile))
                  )
                {
                    System.Net.Mail.Attachment COMAttachment = new System.Net.Mail.Attachment(model.COMAttachmentFile);
                    COMAttachment.Name = model.COMAttachmentFileName;
                    mail.Attachments.Add(COMAttachment);
                }

                if (!mail.Attachments.Any(att => att.Name.Contains(model.COMAttachmentFileName)))
                {
                    this.Response.AddError("Can not attach Commission file.");
                }
            }

            if (model.OrderAttachmentFileName != null && model.OrderAttachmentFileName.Length > 4)
            {
                System.Net.Mail.Attachment OrderAttachment = new System.Net.Mail.Attachment(model.OrderAttachmentFile);
                OrderAttachment.Name = model.OrderAttachmentFileName;
                mail.Attachments.Add(OrderAttachment);
            }

            mail.IsBodyHtml = true;

            if (model.BodyTextVersion != null)
            {
                var plainView = AlternateView.CreateAlternateViewFromString(model.BodyTextVersion, null, "text/plain");
                mail.AlternateViews.Add(plainView);
            }

            if (model.BodyHtmlVersion != null)
            {
                var htmlView = AlternateView.CreateAlternateViewFromString(model.BodyHtmlVersion, null, "text/html");
                mail.AlternateViews.Add(htmlView);
            }

            SmtpClient smtp = new SmtpClient();
            smtp.EnableSsl = Convert.ToBoolean(Utilities.Config("dpo.sys.email.ssl"));
            smtp.SendCompleted += SendCompletedCallback;
            smtp.Host = Utilities.Config("dpo.sys.email.host");
            smtp.Port = int.Parse(Utilities.Config("dpo.sys.email.port"));

            if (!this.Response.HasError)
            {
                try
                {
                    smtp.SendAsync(mail, null);
                    this.Response.AddSuccess(Resources.SystemMessages.SM005);
                }
                catch (Exception ex)
                {
                    this.Response.AddError("Send Mail Error: " + ex.Message);
                }
            }
            else
            {
                try
                {
                    smtp.SendAsync(mail, null);
                    log.Debug("Order Email has been send but has missing attachment files");

                    SmtpClient newSmtp = new SmtpClient();
                    newSmtp.EnableSsl = Convert.ToBoolean(Utilities.Config("dpo.sys.email.ssl"));
                    newSmtp.SendCompleted += SendCompletedCallback;
                    newSmtp.Host = Utilities.Config("dpo.sys.email.host");
                    newSmtp.Port = int.Parse(Utilities.Config("dpo.sys.email.port"));

                    //send out the email to DaikinTeam for error
                    mail = new MailMessage();
                    mail.To.Clear();
                    mail.To.Add(Utilities.Config("dpo.sys.email.orderSendEmailError"));
                    mail.Subject = "Error on Sending Order Email";
                    mail.From = new System.Net.Mail.MailAddress(Utilities.Config("dpo.sys.email.from"));

                    StringBuilder body = new StringBuilder();
                    body.Append("ProjectId: " + model.ProjectId);
                    body.AppendLine();
                    body.Append("QuoteId: " + model.QuoteId);
                    body.AppendLine();

                    foreach (var value in this.Response.Messages.Items)
                    {
                        body.Append(value.Text);
                        body.AppendLine();
                    }

                    mail.Body = body.ToString();
                    newSmtp.SendAsync(mail, null);
                }
                catch (Exception ex)
                {
                    this.Response.AddError("Send Mail Error: " + ex.Message);
                    log.ErrorFormat("Send Order Email with missing Attachment file has Error: {0}", ex.Message);
                }
            }

            if (string.IsNullOrWhiteSpace(Utilities.Config("dpo.sys.email.username")) == false)
            {
                smtp.Credentials = new System.Net.NetworkCredential(Utilities.Config("dpo.sys.email.username"), Utilities.Config("dpo.sys.email.password"));
            }
        }

    }

}
