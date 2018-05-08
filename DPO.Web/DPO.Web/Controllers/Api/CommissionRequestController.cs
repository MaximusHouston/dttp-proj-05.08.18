using AutoMapper;
using DPO.Common;
using DPO.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using DPO.Services.Light;
using DPO.Model.Light;
using System.Collections.Specialized;
using System.Net.Http.Formatting;
using log4net;
using System.IO;
using System.Net.Http;
using System.Net;


namespace DPO.Web.Controllers
{
    [Authorize]
    public class CommissionRequestController : BaseApiController
    {
        public CommissionRequestServices commissionRequestService = new CommissionRequestServices();

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewRequestedCommission, SystemAccessEnum.RequestCommission })]
        public ServiceResponse GetCommissionRequestModel(long? projectId, long? quoteId, long? commissionRequestId, int? commissionRequestStatusTypeId)
        {
            ServiceResponse response = new ServiceResponse();

            int commissionRequestStatus = 0;
            if (commissionRequestStatusTypeId != null)
            {
                commissionRequestStatus = commissionRequestStatusTypeId.Value;
            }

            if (commissionRequestStatus == (int)CommissionRequestStatusTypeEnum.Approved ||
               commissionRequestStatus == (int)CommissionRequestStatusTypeEnum.Pending ||
               commissionRequestStatus == (int)CommissionRequestStatusTypeEnum.NewRecord)
            {
                response = commissionRequestService.GetCommissionRequestModel(this.CurrentUser, new CommissionRequestModel { ProjectId = projectId, QuoteId = quoteId, CommissionRequestId = commissionRequestId }, new CommissionCalculationModel());
            }
            else
            {
                response = commissionRequestService.GetCommissionRequestModel(this.CurrentUser, new CommissionRequestModel { ProjectId = projectId, QuoteId = quoteId }, new CommissionCalculationModel());
            }

            return response;
        }

        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.RequestCommission, SystemAccessEnum.ApprovedRequestCommission })]
        public ServiceResponse PostCommissionCalculation(CommissionRequestModel model) {
            ServiceResponse response = new ServiceResponse();
            response = commissionRequestService.CalculateCommission(this.CurrentUser, model);
            if (response.IsOK == true) {
                response.AddSuccess("Calculation saved successfully!");
            }
            return response;
        }

        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.RequestCommission, SystemAccessEnum.ApprovedRequestCommission })]
        public ServiceResponse GetCommissionPercentage(CommissionMultiplierModelV2 model)
        {
            return commissionRequestService.GetCommissionPercentage(this.CurrentUser, model);
        }


    }
}