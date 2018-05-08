using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPO.Common;
using DPO.Common.Interfaces;
using System.Configuration;
using DPO.Data;
using DPO.Domain.Properties;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Collections;
using System.Web.Mvc;
using EntityFramework.Extensions;
using EntityFramework.Caching;

namespace DPO.Domain.Services
{
    public class LinkServices : BaseServices
    {
        public LinkServices() : base() { }
        public LinkServices(DPOContext context) : base(context) { }

    }
}

