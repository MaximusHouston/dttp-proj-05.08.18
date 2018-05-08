using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using DPO.Services.Light;
using DPO.Domain;
using DPO.Common;
using System.Net.Mail;
using DPO.Model.Light;
using System.Web.Mvc;
using System.Net;

namespace DPO.Web.Controllers
{
    public class FileUploadController : BaseController
    {
        public CommonServiceLight commonservice = new CommonServiceLight();
        //public UploadFileService uploadfileservice = new UploadFileService();

        OrderServiceLight orderServiceLight = new OrderServiceLight();
        OrderServices orderService = new OrderServices();

        [HttpPost]
        public ActionResult UploadPOAttachment(long quoteId, HttpPostedFileBase PurchaseOrderAttachment)
        {
            this.ServiceResponse = new ServiceResponse();

            if (PurchaseOrderAttachment != null)
            {
                var message = Utilities.SavePostedFile(PurchaseOrderAttachment, Utilities.GetPOAttachmentDirectory(quoteId), 25000);

                if (message != null)
                {
                    //create the session to store error message.
                    message += "Please select difference file type";
                    Session["SavePoAttachment"] = message;
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, message);
                }
                else
                {
                    return new HttpStatusCodeResult(HttpStatusCode.OK);
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "No File to upload.");
        }

        //Delete SavePOAttachment after 9/30/2016
        //[HttpPost]
        //public ActionResult SavePOAttachment(long quoteId, IEnumerable<HttpPostedFileBase> PurchaseOrderAttachment)
        //{
        //    this.ServiceResponse = new ServiceResponse();

        //    if (PurchaseOrderAttachment != null && PurchaseOrderAttachment.Count() > 0)
        //    {
        //        foreach (HttpPostedFileBase file in PurchaseOrderAttachment)
        //        {
        //            var filebase = file;
        //            var message = Utilities.SavePostedFile(filebase, Utilities.GetPOAttachmentDirectory(quoteId), 25000);

        //            if (message != null)
        //            {
        //                this.ServiceResponse.AddError(message);
        //                //create the session to store error message.
        //                Session["SavePoAttachment"] = message;
        //            }
        //            else
        //            {
        //                this.ServiceResponse.AddSuccess("File attached");
        //            }
        //        }
        //    }

        //    if(!this.ServiceResponse.IsOK)
        //    {
        //        this.ServiceResponse.Messages.HasErrors = true;
        //        this.ServiceResponse.Messages.AddError("Can not attach Po File."); 
        //        return RedirectToAction("SendEmailToTeamWhenFailToSendEmailOnOrder", "ProjectDashboard", quoteId);
        //    }
        //    else
        //    {
        //        return View("OrderForm", null);
        //    }

        //}

        //[HttpPost]
        //public ServiceResponse RemovePOAttachment(string file)
        //{
        //    uploadfileservice.RemoveFile(file);
        //    return null;
        //}
    }
}