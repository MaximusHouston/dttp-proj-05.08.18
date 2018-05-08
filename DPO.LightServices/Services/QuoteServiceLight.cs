using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
//using EntityFramework;
//using EntityFramework.Extensions;
//using System.Web.Mvc;
using System.Web;
using System.IO;
//using NPOI.HSSF.UserModel;
//using NPOI.HPSF;
//using NPOI.SS.UserModel;
using DPO.Resources;
using DPO.Domain;
using DPO.Model.Light;
using DPO.Common;

namespace DPO.Services.Light
{
    public class QuoteServiceLight : BaseServices
    {
        public QuoteServiceLight() : base() { }

        public ServiceResponse HasOrder(UserSessionModel user, long? quoteId)
        {
            ServiceResponse responese = new ServiceResponse();

            var query = from order in this.Context.Orders
                        where order.QuoteId == quoteId
                        select order;
            var existedOrder = query.FirstOrDefault();

            bool HasOrder = false;
            if (existedOrder != null)
            {
                HasOrder = true;
            }
            responese.Model = HasOrder;
            return responese;
        }

        public ServiceResponse GetQuoteOptions(UserSessionModel user, long? projectId, long? currentQuoteId)
        {
            ServiceResponse responese = new ServiceResponse();
            QuoteOptions quoteOptions = new QuoteOptions();


            responese.Model = quoteOptions;
            return responese;
        }

    }
}
