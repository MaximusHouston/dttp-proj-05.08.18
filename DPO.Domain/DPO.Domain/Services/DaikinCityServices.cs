using DPO.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Domain.Services
{
    public class DaikinCityServices : BaseServices
    {
        public DaikinCityServices() : base() { }
        public DaikinCityServices(DPOContext context) : base(context) { }

        public string GetPrivacyPolicy()
        {
            return Db.QueryHomeScreen().PrivacyPolicy;
        }
    }
}
