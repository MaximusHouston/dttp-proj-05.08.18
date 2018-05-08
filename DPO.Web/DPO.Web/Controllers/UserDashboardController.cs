using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DPO.Domain;
using System.Net.Mail;
using DPO.Common;
using DPO.Web.Helpers;
using System.Diagnostics;
using System.IO;
using DPO.Common.Models.User;

namespace DPO.Web.Controllers
{

    public class UserDashboardController : BaseController
    {
        public AddressServices addressService = new AddressServices();
        public BasketServices basketService = new BasketServices();
        public BusinessServices businessService = new BusinessServices();
        public DiscountRequestServices discountRequestService = new DiscountRequestServices();
        public UserGroupsServices groupsService = new UserGroupsServices();
        public HtmlServices htmlService = new HtmlServices();
        public ProductServices productService = new ProductServices();
        public UserServices userService = new UserServices();
        public PermissionServices permissionService = new PermissionServices();
        public CommissionRequestServices commissionRequestService = new CommissionRequestServices();

        
        #region Users

        [HttpGet]
        [Authorise(Access = SystemAccessEnum.ApproveUsers)]
        public ActionResult ApprovalRequests(UsersModel model, bool usePartialView = true)
        {
            model.Approved = false;

            if (model.Rejected.HasValue
                    && model.Rejected.Value)
            {
                model.Rejected = true;
            }
            else
            {
                model.Rejected = false;
            }

            this.ServiceResponse = userService.GetApprovalRequestListModel(this.CurrentUser, model);

            if (this.ServiceResponse.IsOK)
            {
                var items = this.ServiceResponse.Model as List<UserListModel>;

                model.Items = new PagedList<UserListModel>(items, model);
            }

            return (IsPostRequest && usePartialView) ? (ViewResultBase)PartialView("ApprovalRequests", model) : (ViewResultBase)View("ApprovalRequests", model);

        }

        //[HttpGet]
        //[Authorise(Accesses = new[] { SystemAccessEnum.ApproveUsers, SystemAccessEnum.EditUser })]
        //public ActionResult ApprovalUserEdit(long? userId, int? userTypeId)
        //{
        //    return UserEdit(userId, userTypeId);
        //}

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ApproveUsers, SystemAccessEnum.EditUser })]
        public ActionResult ApprovalUserEdit(long? userId)
        {
            this.ServiceResponse = userService.GetUserModel(this.CurrentUser, userId.Value, false, false);
            UserModel userModel = this.ServiceResponse.Model as UserModel;

            if (!this.ProcessServiceResponse(this.ServiceResponse))
            {
                return ApprovalRequests(null);
            }

            this.ServiceResponse = businessService.GetBusinessModel(this.CurrentUser, userModel.Business.BusinessId, false);
            userModel.Business = this.ServiceResponse.Model as BusinessModel;

            if (!this.ProcessServiceResponse(this.ServiceResponse))
            {
                return ApprovalRequests(null);
            }

            return WizardUserEdit(userModel, true);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.ApproveUsers, SystemAccessEnum.EditUser })]
        public ActionResult ApprovalUserEdit(UserModel model)
        {
            return WizardUserEdit(model);
        }

        [HttpPost]
        [Authorise(Access = SystemAccessEnum.ApproveUsers)]
        public ActionResult Approve(UserModel model)
        {
            model.Approved = true;
            model.Rejected = false;

            this.ServiceResponse = userService.ChangeUserStatus(this.CurrentUser, model);

            if (this.ServiceResponse.IsOK)
            {
                var emailModel = new UserServices().GetUserEmailModel(this.CurrentUser, model).Model as SendEmailModel;

                emailModel.Subject = "New Project Office Registration Approval";
                emailModel.RenderTextVersion = true;

                emailModel.BodyTextVersion = RenderView(this, "SendEmailUserApproval", emailModel);

                emailModel.RenderTextVersion = false;
                emailModel.BodyHtmlVersion = RenderView(this, "SendEmailUserApproval", emailModel);

                var emailservice = new EmailServices();
                emailservice.SendEmail(emailModel);

                this.ServiceResponse.AddSuccess("Email sent to user");
            }

            ProcessServiceResponse(this.ServiceResponse);

            this.ServiceResponse.Messages.Clear();

            return AJAXRedirectTo("ApprovalRequests", "UserDashboard", null);
        }

        [HttpPost]
        [Authorise(Access = SystemAccessEnum.UndeleteUser)]
        public ActionResult Disable(UserModel model)
        {
            model.Enabled = false;
            this.ServiceResponse = userService.ChangeUserStatus(this.CurrentUser, model);
            ProcessServiceResponse(this.ServiceResponse);

            //get UsersModel 
            UsersModel usersModel = new UsersModel();
            usersModel.ReturnTotals = true;

            usersModel.Approved = true;

            this.ServiceResponse = userService.GetUserListModel(this.CurrentUser, usersModel);

            if (this.ServiceResponse.IsOK)
            {
                var items = this.ServiceResponse.Model as List<UserListModel>;

                usersModel.Items = new PagedList<UserListModel>(items, model);
            }

            return AJAXRedirectTo("Users", "UserDashboard",usersModel);
           
        }

        [HttpPost]
        [Authorise(Access = SystemAccessEnum.UndeleteUser)]
        public ActionResult Enable(UserModel model)
        {
            model.Enabled = true;
            this.ServiceResponse = userService.ChangeUserStatus(this.CurrentUser, model);
            ProcessServiceResponse(this.ServiceResponse);

            //get UsersModel 
            UsersModel usersModel = new UsersModel();
            usersModel.ReturnTotals = true;

            usersModel.Approved = true;

            this.ServiceResponse = userService.GetUserListModel(this.CurrentUser, usersModel);

            if (this.ServiceResponse.IsOK)
            {
                var items = this.ServiceResponse.Model as List<UserListModel>;

                usersModel.Items = new PagedList<UserListModel>(items, model);
            }
            return AJAXRedirectTo("Users", "UserDashboard", usersModel);
        }

        [HttpPost]
        [Authorise(Access = SystemAccessEnum.ApproveUsers)]
        public ActionResult Reject(UserModel model)
        {

            model.Approved = false;

            model.Rejected = true;

            this.ServiceResponse = userService.ChangeUserStatus(this.CurrentUser, model);

            if (this.ServiceResponse.IsOK)
            {
                var emailModel = new UserServices().GetUserEmailModel(this.CurrentUser, model).Model as SendEmailModel;

                emailModel.Subject = "New Project Office Registration Rejection";
                emailModel.RenderTextVersion = true;

                emailModel.BodyTextVersion = RenderView(this, "SendEmailUserRejection", emailModel);

                emailModel.RenderTextVersion = false;
                emailModel.BodyHtmlVersion = RenderView(this, "SendEmailUserRejection", emailModel);

                var emailservice = new EmailServices();
                emailservice.SendEmail(emailModel);

                this.ServiceResponse.AddSuccess("Email sent to user");
            }

            ProcessServiceResponse(this.ServiceResponse);

            this.ServiceResponse.Messages.Clear();

            return AjaxReloadPage();

        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditUser })]
        public ActionResult UserEdit(long? userId)
        {
            this.ServiceResponse = userService.GetUserModel(this.CurrentUser, userId, false);

            ProcessServiceResponse(this.ServiceResponse);

            return View("UserEdit", this.ServiceResponse.Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Access = SystemAccessEnum.EditUser)]
        public ActionResult UserEdit(UserModel model)
        {
            
            this.ServiceResponse = userService.PostModel(this.CurrentUser, model);

            if (ProcessServiceResponse(this.ServiceResponse))
            {
                return AjaxRedirectToReferrer();
            }

            return PartialView("UserEdit", this.ServiceResponse.Model);
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewUsers })]
        public ActionResult Users(UsersModel model)
        {
            model.ReturnTotals = true;

            model.Approved = true;

            this.ServiceResponse = userService.GetUserListModel(this.CurrentUser, model);

            if (this.ServiceResponse.IsOK)
            {
                var items = this.ServiceResponse.Model as List<UserListModel>;

                model.Items = new PagedList<UserListModel>(items, model);
            }

            this.RouteData.Values["action"] = "Users";

            return (IsPostRequest) ? (ViewResultBase)PartialView("Users", model) : (ViewResultBase)View("Users", model);

        }

        #endregion Users

        #region User Basket

        public ActionResult Basket()
        {
            this.ServiceResponse = basketService.GetUserBasketModel(this.CurrentUser);

            ProcessServiceResponse(this.ServiceResponse);

            return PartialView("Basket", this.ServiceResponse.Model);
        }

        [HttpPost]
        public ActionResult BasketClear()
        {
            this.ServiceResponse = basketService.Clear(this.CurrentUser);

            ProcessServiceResponse(this.ServiceResponse);

            return Basket();
        }

        [HttpPost]
        public ActionResult BasketRemoveItem(BasketItemModel basketItem)
        {
            basketItem.Quantity = 0;

            this.ServiceResponse = basketService.RemoveItem(this.CurrentUser, basketItem);

            ProcessServiceResponse(this.ServiceResponse);

            return Basket();
        }

        [HttpPost]
        public ActionResult BasketUpdateItem(BasketItemModel basketItem)
        {
            this.ServiceResponse = basketService.UpdateBasketItem(this.CurrentUser, basketItem);

            ProcessServiceResponse(this.ServiceResponse);

            return Basket();
        }
        [HttpPost]
        [Authorise(Access = SystemAccessEnum.UndeleteBusiness)]
        public ActionResult BusinessDisable(BusinessModel model)
        {
            model.Enabled = false;

            this.ServiceResponse = businessService.EnableDisable(this.CurrentUser, model);

            ProcessServiceResponse(this.ServiceResponse);

            return AJAXRedirectTo("Businesses", "UserDashboard", null);
        }

        [HttpPost]
        [Authorise(Access = SystemAccessEnum.UndeleteBusiness)]
        public ActionResult BusinessEnable(BusinessModel model)
        {
            model.Enabled = true;

            this.ServiceResponse = businessService.EnableDisable(this.CurrentUser, model);

            ProcessServiceResponse(this.ServiceResponse);

            return AJAXRedirectTo("Businesses", "UserDashboard", null);

        }
        #endregion

        #region Business

        [HttpPost]
        [Authorise(NoSecurityRequired = true)]
        public ActionResult BusinessAddressLookup(string accountId)
        {
            this.ServiceResponse = businessService.GetBusinessModelByAccountId(this.CurrentUser, accountId);

            BusinessModel model = this.ServiceResponse.Model as BusinessModel;

            return (ViewResultBase)PartialView("_AddressContactPartial", model);
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditBusiness })]
        public ActionResult BusinessEdit(long? businessId)
        {
            this.ServiceResponse = businessService.GetBusinessModel(this.CurrentUser, businessId, true);
            ProcessServiceResponse(this.ServiceResponse);
            return View("BusinessEdit", this.ServiceResponse.Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditBusiness })]
        public ActionResult BusinessEdit(BusinessModel model)
        {
            this.ServiceResponse = businessService.PostModel(this.CurrentUser, model);

            if (ProcessServiceResponse(this.ServiceResponse))
            {
                return AJAXRedirectTo("Businesses", "Userdashboard", new { UserId = (userService.NewRecordAdded) ? (long?)null : model.BusinessId });
            }

            return PartialView("BusinessEdit", this.ServiceResponse.Model);
        }

        [HttpGet]
        [Authorise(Access = SystemAccessEnum.ViewBusiness)]
        public ActionResult Businesses(BusinessesModel model)
        {
            model.ReturnTotals = true;

            this.ServiceResponse = businessService.GetBusinessListModel(this.CurrentUser, model);

            if (this.ServiceResponse.IsOK)
            {
                var items = this.ServiceResponse.Model as List<BusinessListModel>;

                model.Items = new PagedList<BusinessListModel>(items, model);
            }

            this.RouteData.Values["action"] = "Businesses";

            return (IsPostRequest) ? (ViewResultBase)PartialView("Businesses", model) : (ViewResultBase)View("Businesses", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditBusiness })]
        public ActionResult BusinessLogoUpload(FormCollection formCollection, BusinessModel model)
        {
            this.ServiceResponse = new ServiceResponse();

            if (Request != null && Request.Files.Count == 1 && model.BusinessId.HasValue)
            {
                var file = Request.Files[0];
                if (file.ContentLength > 75 * 1024)
                {
                    this.ServiceResponse.AddError(Resources.ResourceUI.ImageToLarge);
                }
                else if (file.ContentLength < 100 || !file.IsImage())
                {
                    this.ServiceResponse.AddError(Resources.ResourceUI.ImageInvalid);
                }

                if (this.ServiceResponse.IsOK)
                {
                    var imageLocation = this.Server.MapPath("~/Images/BusinessLogos/");
                    if (!Directory.Exists(imageLocation)) Directory.CreateDirectory(imageLocation);

                    var ext = Path.GetExtension(file.FileName);
                    var path = Path.Combine(imageLocation, model.BusinessId.Value.ToString() + ext);
                    file.SaveAs(path);
                    this.ServiceResponse.AddSuccess("Logo uploaded");
                }
            }

            ProcessServiceResponse(this.ServiceResponse);

            this.ServiceResponse = businessService.GetBusinessModel(this.CurrentUser, model.BusinessId, true);

            return View("BusinessEdit", this.ServiceResponse.Model);
        }

        #endregion

        #region Wizard

        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.ApproveUsers, SystemAccessEnum.EditUser })]
        public ActionResult WizardSaveUserEdit(UserModel model)
        {
            // Approve User
            Approve(model);
            Enable(model);

            // Save Business
            //if (model.Business != null)
            //{
            //    this.ServiceResponse = businessService.PostModel(this.CurrentUser, model.Business);
            //    if (!this.ServiceResponse.IsOK)
            //    {
            //        // TODO:  Return message
            //    }
            //}

            // Save User
            this.ServiceResponse = userService.PostModel(this.CurrentUser, model);
            if (!this.ServiceResponse.IsOK)
            {
                // TODO:  Return message
            }

            // Save Group
            if (model.GroupId.HasValue)
            {
                this.ServiceResponse = groupsService.GroupUsersMove(this.CurrentUser, new long[] { model.UserId.Value }, model.GroupId.Value);
                if (!this.ServiceResponse.IsOK)
                {
                    // TODO:  Return message
                }
            }

            // HACK: Kind of hackish
            return ApprovalRequests(new UsersModel(), false);
        }

        [ValidateAntiForgeryToken]
        [Authorise(Access = SystemAccessEnum.EditUser)]
        public ActionResult WizardUserEdit(UserModel model, bool isEditing = false)
        {
            var tmpModel = model;
            var userId = model.UserId.Value;

            this.ServiceResponse = userService.GetUserModel(this.CurrentUser, model, true);
            this.ProcessServiceResponse(this.ServiceResponse);

            model = this.ServiceResponse.Model as UserModel;

            // HACK:  Should probably do this a bit differently
            this.ServiceResponse = groupsService.GroupsListModel(this.CurrentUser, String.Empty);
            this.ProcessServiceResponse(this.ServiceResponse);
            model.Groups = this.ServiceResponse.Model as UserGroupsModel;

            return PartialView("WizardUserEdit", model);
        }

        #endregion Wizard

        #region User Groups
        [HttpGet]
        public ActionResult AssignUserHasAccountIdToGroup(UserVM model)
        {
            this.ServiceResponse = groupsService.GroupsListModel(this.CurrentUser, null);
            model.UserModel.IsAccountAndUserTypeId = true;
            //return PartialView("AssignUserToGroup", model);

            return View("UserGroups", model);
        }

        [HttpGet]
        public ActionResult AssignUserToGroup()
        {
            //return PartialView("AssignUserToGroup", model);
            //return UserGroupsList(null);
            this.ServiceResponse = groupsService.GroupsListModel(this.CurrentUser, null);
            return (ViewResultBase)PartialView("MyUserGroupList", this.ServiceResponse.Model);
        }

        [HttpPost]
        public ActionResult AssignUserToGroup(UserVM model)
        {
            long[] usersId = new long[1];

            if (model.UserModel.UserId != null)
            {
                usersId[0] = (long)model.UserModel.UserId;
            }

            this.ServiceResponse = groupsService.GroupUsersMove(this.CurrentUser, usersId, (long)model.UserModel.GroupId);
            return UserGroupsUsers(null, model.UserModel.GroupId);
        }

        [HttpPost]
        public ActionResult MoveUserToGroup(UserVM model, long UsergroupId = 0)
        {
            if (model.UserModel == null)
            {
                //this.ServiceResponse = businessService.GetBusinessModel(this.CurrentUser, model.BusinessId.Value, false);
                //BusinessModel Businessmodel = this.ServiceResponse.Model as BusinessModel;
                //model.UserModel.Business = Businessmodel;

                this.ServiceResponse = userService.GetUserModel(this.CurrentUser, model.UserId.Value, false, false);
                UserModel UserModel = this.ServiceResponse.Model as UserModel;
                model.UserModel = UserModel;
            }

            if (model.UserGroupsModel == null)
            {
                UserGroupsModel UserGroupsModel = new UserGroupsModel();
                this.ServiceResponse = groupsService.GroupsListModel(this.CurrentUser, null);
                UserGroupsModel = this.ServiceResponse.Model as UserGroupsModel;
                model.UserGroupsModel = UserGroupsModel;
            }

            //if (model.UserModel.Business.AccountId == null)
            //{
            //    model = Session["UserVM"] as UserVM;
            //}

            long[] usersId = new long[1];

            //UserGroupItemModel GroupItem = model.UserGroupsModel.UserGroups.FirstOrDefault();

            //if (model.UserModel.UserId != null )
            //{
            //    usersId[0] = (long)model.UserModel.UserId;
            //}

            //long groupId = 0;

            //if( GroupItem != null && GroupItem.GroupId != null)
            //{
            //    groupId = GroupItem.GroupId; 
            //}

            if (model.UserId != null)
            {
                usersId[0] = (long)model.UserId;
            }

            this.ServiceResponse = groupsService.GroupUsersMove(this.CurrentUser, usersId, UsergroupId);

            //this.ServiceResponse = groupsService.GroupUsersMove(this.CurrentUser, usersId, groupId);
            //return UserGroupsUsers(null, model.UserModel.GroupId);
            return UserGroupsUsers(null, UsergroupId);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.ManageGroups })]
        public ActionResult UserGroup(BusinessModel model)
        {
            return View(model);
        }

        public ActionResult UserGroupList(string filter, bool DisplayRadioButton = true)
        {
            this.ServiceResponse = groupsService.GroupsListModel(this.CurrentUser, filter);
            ViewBag.DisplayRadioButton = DisplayRadioButton;
            return PartialView("UserGroupsList", this.ServiceResponse.Model);
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ManageGroups })]
        public ActionResult UserGroups()
        {
            var model = new UserGroupsModel();

            if (this.CurrentUser.HasAccess(SystemAccessEnum.AdminAccessRights))
            {
                model.UserGroupId = this.CurrentUser.GroupId;
            }

            return View(model);
        }


        [HttpPost]
        public ActionResult UserGroupsGroupCreate(string name, long? parentId)
        {
            if (this.CurrentUser.IsGroupOwner)
            {
                this.ServiceResponse = groupsService.GroupCreate(this.CurrentUser, name, parentId);
            }

            return UserGroupsList(null);
        }

        [HttpPost]
        public ActionResult UserGroupsGroupDelete(long groupId)
        {
            if (this.CurrentUser.IsGroupOwner)
            {
                this.ServiceResponse = groupsService.GroupDelete(this.CurrentUser, groupId);
            }

            return UserGroupsList(null);
        }

        [HttpPost]
        public ActionResult UserGroupsGroupMove(long fromGroupId, long toGroupId)
        {
            if (this.CurrentUser.IsGroupOwner)
            {
                this.ServiceResponse = groupsService.GroupMove(this.CurrentUser, fromGroupId, toGroupId);
            }

            return UserGroupsList(null);
        }

        [HttpPost]
        public ActionResult UserGroupsGroupRename(long groupId, string newName)
        {
            if (this.CurrentUser.IsGroupOwner)
            {
                this.ServiceResponse = groupsService.GroupRename(this.CurrentUser, groupId, newName);
            }

            return UserGroupsList(null);
        }

        [Authorise(Accesses = new[] { SystemAccessEnum.ManageGroups })]
        public ActionResult UserGroupsList(string filter)
        {
            this.ServiceResponse = groupsService.GroupsListModel(this.CurrentUser, filter);

            return PartialView("UserGroupsList", this.ServiceResponse.Model);
        }

        [HttpPost]
        public ActionResult UserGroupsUserMakeOwner(UserModel model, long groupId, long userId, bool makerOwner)
        {
            if (this.CurrentUser.IsGroupOwner)
            {
                this.ServiceResponse = groupsService.GroupUserMakeOwner(this.CurrentUser, groupId, userId, makerOwner);
            }

            return UserGroupsUsers(null, groupId);
        }

        [Authorise(Accesses = new[] { SystemAccessEnum.ManageGroups })]
        public ActionResult UserGroupsUsers(string filter, long? groupId)
        {
            this.ServiceResponse = groupsService.GroupUsersListModel(this.CurrentUser, filter, groupId);

            return PartialView("UserGroupsUsers", this.ServiceResponse.Model);
        }

        [HttpPost]
        public ActionResult UserGroupsUsersMove(UserModel model, long[] userIds, long groupId)
        {
            if (this.CurrentUser.IsGroupOwner)
            {
                this.ServiceResponse = groupsService.GroupUsersMove(this.CurrentUser, userIds, groupId);
            }

            return UserGroupsUsers(null, groupId);
        }

        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.ManageGroups })]
        public ActionResult UserGroupsUsersUnallocate(UserModel model, long[] userIds)
        {
            this.ServiceResponse = groupsService.GroupUsersMove(this.CurrentUser, userIds, 0);

            return UserGroupsUsers(null, 0);
        }

        #endregion

        #region Discount Requests

        [Authorise(Accesses = new[] { SystemAccessEnum.ApproveDiscounts })]
        public ActionResult DiscountRequests(DiscountRequestListModel model)
        {
            this.ServiceResponse = discountRequestService.GetDiscountRequestListModel(this.CurrentUser, model);

            if (this.ServiceResponse.IsOK)
            {
                var items = this.ServiceResponse.Model as List<DiscountRequestModel>;

                model.Items = new PagedList<DiscountRequestModel>(items, model);
            }

            return (IsPostRequest) ? (ViewResultBase)PartialView("DiscountRequests", model) : (ViewResultBase)View("DiscountRequests", model);
        }

        #endregion

        #region Commission Requests

        [Authorise(Accesses = new[] { SystemAccessEnum.ApprovedRequestCommission})]
        public ActionResult CommissionRequests(CommissionRequestListModel model)
        {
            this.ServiceResponse = commissionRequestService.GetCommissionRequestListModel(this.CurrentUser, model);
            if(this.ServiceResponse.IsOK)
            {
                var items = this.ServiceResponse.Model as List<CommissionRequestModel>;
                model.Items = new PagedList<CommissionRequestModel>(items, model);
            }

            foreach(var item in model.Items)
            {
               
                Console.WriteLine(item.CommissionRequestId);
            }

            return (IsPostRequest) ? (ViewResultBase)PartialView("CommissionRequests", model) : (ViewResultBase)View("CommissionRequests", model);
        }

        #endregion
    }
}
