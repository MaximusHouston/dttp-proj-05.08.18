using DPO.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Domain
{
    public class ConversionServices : BaseServices
    {
        public ConversionServices() : base() { }

        public ConversionServices(DPOContext context) : base(context) { }

        public ConversionServices(BaseServices injectService, string propertyReference)
        {
            this.Response = injectService.Response;
            this.Context = injectService.Context;
            this.Db = injectService.Db;
            this.Response.PropertyReference = propertyReference;
        }

        /// <summary>
        /// Converts a date from a normal date to a fiscal date
        /// </summary>
        /// <param name="date"></param>
        public DateTime ToFiscal(DateTime date)
        {
            return date.AddMonths(-3);
        }

        public DateTime? ToFiscal(DateTime? date)
        {
            if (date == null)
            {
                return null;
            }

            return date.Value.AddMonths(-3);
        }

        public DateTime FromFiscal(DateTime date)
        {
            return date.AddMonths(3);
        }

        public DateTime? FromFiscal(DateTime? date)
        {
            if (date == null)
            {
                return null;
            }

            return date.Value.AddMonths(3);
        }
    }
}
