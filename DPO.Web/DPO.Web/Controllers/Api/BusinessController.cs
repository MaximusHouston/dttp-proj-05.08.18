using DPO.Common;
using DPO.Domain;
using System.Web.Http;
using DPO.Services.Light;

namespace DPO.Web.Controllers
{
    [Authorize]
    public class BusinessController : BaseApiController
    {
        BusinessServices businessService = new BusinessServices();

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ServiceResponse GetBusinessList()
        {
            return businessService.GetBusinessListModel(this.CurrentUser, new SearchBusiness());
        }
    }
}