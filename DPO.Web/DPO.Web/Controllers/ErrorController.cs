using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DPO.Domain;
using System.Net.Mail;
using DPO.Common;
using DPO.Resources;
using DPO.Web;
using Elmah.Mvc;

namespace DPO.Web.Controllers
{
    [Authorise(UserTypeAllowed = UserTypeEnum.Systems)]
    public class ErrorController : ElmahController
    {
        public ActionResult ViewDetail(string resource)
        {
            return base.Detail(resource);
        }

        public new ActionResult View(string resource)
        {
            return base.Index(resource);
        }
    }

    
}
