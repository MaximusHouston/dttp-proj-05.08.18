using Newtonsoft.Json;
using System;
using DPO.Common;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Runtime.Serialization.Formatters;

namespace DPO.Web.Controllers
{
    [Authorise(NoSecurityRequired = true)]
    public class ViewRenderController : BaseController
    {
        // GET: ViewRender
        //public ActionResult Index()
        //{
        //    return View();
        //}

        //Not finished
        public String Render(string ViewName, string SerializedModel)
        {
            string HtmlResult = "";

            if (ViewName == "UserRegistrationEmailTemplate")
            {
                try
                {
                    //SendEmailModel model = JsonConvert.DeserializeObject<SendEmailModel>(SerializedModel);
                    SendEmailViewModel sendEmailViewModel = JsonConvert.DeserializeObject<SendEmailViewModel>(SerializedModel);
                    this.ViewData.Model = sendEmailViewModel;

                    //Need to pass in full path to view "~/Views/Account/ ... "
                    //Create another SendEmailUserRegistration mvc view that takes SendEmailViewModel
                    HtmlResult = this.ToHtml("~/Views/Account/" + ViewName + ".cshtml", this.ViewData);
                }
                catch (Exception e)
                {
                    Utilities.ErrorLog(e);
                }

            }

            return HtmlResult;
        }

        [HttpPost]
        public String Render(ViewRenderModel model)
        {
            string HtmlResult = "";
            
            try
            {
                this.ViewData.Model = model.ViewModel;
                HtmlResult = this.ToHtml(model.ViewName, this.ViewData);
            }
            catch (Exception e)
            {
                Utilities.ErrorLog(e);
            }
            
            return HtmlResult;
        }

        //public String RenderSendEmailUserRegistration(string ModelString)
        //{
        //    string HtmlResult = "";

        //    var model = JsonConvert.DeserializeObject(ModelString);
        //    this.ViewData.Model = model;
        //    HtmlResult = this.ToHtml("SendEmailUserRegistration", this.ViewData);

        //    return HtmlResult;
        //}
    }
}