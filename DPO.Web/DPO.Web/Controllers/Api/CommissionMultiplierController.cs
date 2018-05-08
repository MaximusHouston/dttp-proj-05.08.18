using DPO.Common;
using DPO.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace DPO.Web.Controllers
{
    public class CommissionMultiplierController : BaseApiController
    {
        private CommissionRequestServices commissionRequestService;

        public CommissionMultiplierController()
            : base()
        {
            this.commissionRequestService = new CommissionRequestServices();
        }

        //TODO: Check if this is being used? need to be cleaned up.
        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.RequestCommission, SystemAccessEnum.ApprovedRequestCommission })]
        public ServiceResponse GetCommissionMultipliers(CommissionMultipliersModel model)
        {
            return commissionRequestService.GetCommissionMultipliers(this.CurrentUser, model);
        }

        //This function is used in MVC page
        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.RequestCommission, SystemAccessEnum.ApprovedRequestCommission })]
        public ServiceResponse GetCommissionMultiplier(CommissionMultipliersModel model)
        {
            ServiceResponse resp;
            resp = commissionRequestService.GetCommissionMultiplier(this.CurrentUser, model);
           
            return resp;
        }
                
        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.RequestCommission, SystemAccessEnum.ApprovedRequestCommission })]
        public ServiceResponse GetUnitaryMultiplier(CommissionRequestModel model)
        {
            ServiceResponse resp;
            resp = commissionRequestService.GetUnitaryMultiplier(this.CurrentUser, model);

            return resp;
        }
    }
}