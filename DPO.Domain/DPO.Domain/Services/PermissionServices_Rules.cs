using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPO.Common;
using DPO.Common.Interfaces;
using DPO.Data;
using DPO.Domain.Properties;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Collections;
using System.Net.Mail;
using DPO.Resources;
using DPO.Common.Models.General;

namespace DPO.Domain
{
    public partial class PermissionServices 
   {
        private List<PermissionListModel> permissions;

        public void ApplyBusinessRules(User entity, UserModel model, UserSessionModel admin)
        {
            Db.PermissionsUpdate(EntityEnum.Business, entity.BusinessId, 
                                 EntityEnum.User, model.UserId, 
                                 CheckBoxListModel.ToPermissionListModel(model.CityAreas), 
                                 PermissionTypeEnum.CityArea);

            Db.PermissionsUpdate(EntityEnum.Business, entity.BusinessId, 
                                 EntityEnum.User, model.UserId, 
                                 CheckBoxListModel.ToPermissionListModel(model.Brands), 
                                 PermissionTypeEnum.Brand);

            Db.PermissionsUpdate(EntityEnum.Business, entity.BusinessId, 
                                 EntityEnum.User, model.UserId, 
                                 CheckBoxListModel.ToPermissionListModel(model.ProductFamilies), 
                                 PermissionTypeEnum.ProductFamily);

            Db.PermissionsUpdate(EntityEnum.Business, entity.BusinessId, 
                                 EntityEnum.User, model.UserId, 
                                 CheckBoxListModel.ToPermissionListModel(model.Tools), 
                                 PermissionTypeEnum.Tool);

            permissions = CheckBoxListModel.ToPermissionListModel(model.SystemAccesses);

            // all users have this unset when being created
            if (IsSet(SystemAccessEnum.AdminAccessRights) && this.Db.Entry(entity).State == EntityState.Added)
            {
                UnSet(SystemAccessEnum.AdminAccessRights);
            }

            if (entity.IsGroupOwner.HasValue && entity.IsGroupOwner.Value) CreateOrSet(SystemAccessEnum.ManageGroups);


            if (IsSet(SystemAccessEnum.EditUser)) CreateOrSet(SystemAccessEnum.ViewUsers);

            if (IsSet(SystemAccessEnum.UndeleteUser)) CreateOrSet(SystemAccessEnum.ViewUsers);


            if (IsSet(SystemAccessEnum.EditBusiness)) CreateOrSet(SystemAccessEnum.ViewBusiness);

            if (IsSet(SystemAccessEnum.UndeleteBusiness)) CreateOrSet(SystemAccessEnum.ViewBusiness);


            if (IsSet(SystemAccessEnum.EditProject)) CreateOrSet(SystemAccessEnum.ViewProject);

            if (IsSet(SystemAccessEnum.UndeleteProject)) CreateOrSet(SystemAccessEnum.ViewProject);

            //if (IsSet(SystemAccessEnum.ShareProject)) CreateOrSet(SystemAccessEnum.ViewProject);

            if (IsSet(SystemAccessEnum.TransferProject)) CreateOrSet(SystemAccessEnum.ViewProject);

            Db.PermissionsUpdate(EntityEnum.UserType, (long)entity.UserTypeId, 
                                 EntityEnum.User, model.UserId, permissions, 
                                 PermissionTypeEnum.SystemAccess);
        }

        public override void RulesOnAdd(UserSessionModel admin, object entity)
        {
            if (!(entity is User))
            {
                return;
            }

            base.RulesOnAdd(admin, entity);
        }

        private void CreateOrSet(SystemAccessEnum referenceId)
        {
            var permission = permissions.Where(p => p.ReferenceId == (int)referenceId).FirstOrDefault();

            if (permission == null)
            {
                permission = new PermissionListModel { ReferenceId = (int)referenceId};

                permissions.Add(permission);
            }

            permission.IsSelected = true;
        }

        private bool IsSet(SystemAccessEnum referenceId)
        {
            return (permissions.Any(p => p.ReferenceId == (int)referenceId && p.IsSelected));
        }

        private void UnSet(SystemAccessEnum referenceId)
        {
            var permission = permissions.Where(p => p.ReferenceId == (int)referenceId).FirstOrDefault();

            permission.IsSelected = false;
        }

        
   }

}
