using DPO.Common;
using DPO.Domain;
using System.Web;
using System.Web.Http;
using DPO.Services.Light;

namespace DPO.Web.Controllers
{
    public class UserController : BaseApiController
    {
        UserServiceLight userServiceLight = new UserServiceLight();
        UserServices userService = new UserServices();
        BasketServices basketService = new BasketServices();
        UserGroupsServices groupsService = new UserGroupsServices();

        [HttpGet]
        public ServiceResponse IsAuthenticated()
        {
            ServiceResponse response = new ServiceResponse();
            response.Model = User.Identity.IsAuthenticated;
            return response;
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ServiceResponse GetUsers()
        {
            return userServiceLight.GetUsers(this.CurrentUser);
        }
 
        [HttpGet]
        public ServiceResponse GetCurrentUser()
        {
            if (User.Identity.IsAuthenticated) {
                return userServiceLight.GetCurrentUser(this.CurrentUser);
            }
            return this.ServiceResponse;
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ServiceResponse GetUsersViewable()
        {
            return userService.GetUsersViewable(this.CurrentUser);
        }

        [Authorize]
        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ServiceResponse GetBasket()
        {
            return basketService.GetUserBasketModel(this.CurrentUser);
        }

        //[HttpGet]
        //[Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        //public ServiceResponse GetProjectOwners()
        //{
        //    return userServiceLight.GetProjectOwners(this.CurrentUser); 
        //}

        //[HttpGet]
        //public ServiceResponse ResetBasketQuoteId()
        //{
        //    var session = HttpContext.Current.Session;
        //    session["BasketQuoteId"] = 0;
        //    CurrentUser.BasketQuoteId = 0;
        //    return this.ServiceResponse;
        //}


        //Test
        //[HttpPost]
        //[Authorise(Accesses = new[] { SystemAccessEnum.ManageGroups })]
        //public ServiceResponse UserGroupsList(string filter = "")
        //{
        //    return groupsService.GroupsListModel(this.CurrentUser, filter);
        //}

    }
}