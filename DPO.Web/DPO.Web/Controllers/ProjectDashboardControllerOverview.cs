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

namespace DPO.Web.Controllers
{
    public partial class ProjectDashboardController
    {
        public ActionResult Daikinheader()
        {
            return View();
        }

        public ActionResult Index()
        {
            return RedirectToAction("Overview");
        }

        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ActionResult Overview(WidgetContainerModel model)
        {
            model.AvailableWidgetTypes = overviewService.GetAvailableWidgetTypes();

            this.ServiceResponse = overviewService.GetOverviewSearchModel(this.CurrentUser, model);

            ProcessServiceResponse(this.ServiceResponse);

            return View("Overview", this.ServiceResponse.Model);
        }
    }
}