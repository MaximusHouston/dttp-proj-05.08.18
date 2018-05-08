using System;
using System.Collections.Generic;
using System.Linq;
using DPO.Common;
using DPO.Common.Interfaces;
using DPO.Data;
using DPO.Domain.Properties;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Domain
{
    public class BusinessLinkServices: BaseServices
    {
        public BusinessLinkServices()
            : base(){
        }

        public BusinessLinkServices(DPOContext context)
            : base(context)
        {
          
        }

        public BusinessLink ModelToEntity(BusinessLinkModel model) {
            var entity = GetEntity(model);

            if (this.Response.HasError) return null;

            return entity;
        }

        private BusinessLink GetEntity(BusinessLinkModel model)
        {
            var newBusinessLink = !model.BusinessLinkId.HasValue;
            BusinessLink entity = (newBusinessLink) ? Db.BusinessLinkCreate(model.BusinessId, model.ParentBusinessId) : this.Db.BusinessLinkQueryByBusinessLinkId(model.BusinessLinkId).FirstOrDefault();

            if (entity == null)
            {
                this.Response.Messages.AddError(Resources.DataMessages.DM006);
            }

            return entity;
        }

        public BusinessLink GetBusinessLinkByBusinessId(long businessId)
        {
            BusinessLink entity = this.Db.BusinessLinkQueryByBusinessId(businessId).FirstOrDefault();

            if (entity == null)
            {
                this.Response.Messages.AddError(Resources.DataMessages.DM006);
            }

            return entity;
        }



    }
}
