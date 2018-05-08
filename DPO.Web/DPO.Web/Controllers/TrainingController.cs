using DPO.Common;
using DPO.Domain;
using DPO.Domain.DaikinUniversity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DPO.Web.Controllers
{
    [Authorise(NoSecurityRequired = true)]
    public class TrainingController : BaseController
    {
        private DaikinUniversityLoginServices daikinUniversityLoginServices = new DaikinUniversityLoginServices();
        private AccountServices accountServices = new AccountServices();

        // GET: Training
        public ActionResult Index()
        {
            if (this.CurrentUser == null
                || !this.CurrentUser.Enabled)
            {
                return RedirectToLogin(this.HttpContext.Request.Url);
            }

            return View(daikinUniversityLoginServices.GetAESLoginModel(this.CurrentUser.Email));
        }
    }
}