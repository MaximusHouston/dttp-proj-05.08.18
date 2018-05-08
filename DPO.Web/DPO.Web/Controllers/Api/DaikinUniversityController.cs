using DPO.Domain;
using DPO.Domain.DaikinUniversity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DPO.Web.Controllers.Api
{
    public class DaikinUniversityController : BaseApiController
    {
        public static int HttpGet { get; private set; }

        public DaikinUniversityApiServices daikinUniversityServices = new DaikinUniversityApiServices(); 

       
    }
}