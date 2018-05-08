//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================

using System;
using System.Collections.Generic;
using System.Data.Entity;

using System.Linq;
using System.Linq.Expressions;
using EntityFramework.Extensions;
using DPO.Common;
using DPO.Common.Models.General;

namespace DPO.Data
{



    public partial class Repository
    {
        #region Dataset definitions


        public IQueryable<Brand> Brands
        {
            get { return this.GetDbSet<Brand>(); }
        }

        public IQueryable<Tool> Tools
        {
            get { return this.GetDbSet<Tool>(); }
        }
        public IQueryable<CityArea> CityAreas
        {
            get { return this.GetDbSet<CityArea>(); }
        }
        public IQueryable<Permission> Permissions
        {
            get { return this.GetDbSet<Permission>(); }
        }

        public IQueryable<ProductFamily> ProductFamilies
        {
            get { return this.GetDbSet<ProductFamily>(); }
        }


        public IQueryable<SystemAccess> SystemAccesses
        {
            get { return this.GetDbSet<SystemAccess>(); }
        }
        #endregion

        public IQueryable<UserType> GetUserTypes(UserSessionModel admin)
        {
            if (admin.HasAccess(SystemAccessEnum.AdminAccessRights))
                return (from a in this.UserTypes where a.UserTypeId <= admin.UserTypeId select a);
            else
                return (from a in this.UserTypes where a.UserTypeId < admin.UserTypeId select a);
        }

        public IQueryable<Permission> GetSystemAccesses(IQueryable<User> user)
        {
            var query = from defaults in this.Permissions
                        join u in user on defaults.ObjectId equals u.UserId
                        where defaults.PermissionTypeId == PermissionTypeEnum.SystemAccess
                        select defaults;

            return query;
        }

        public List<int> GetPermissionList(long objectId, PermissionTypeEnum type)
        {
            var query = from defaults in this.Permissions
                        where defaults.PermissionTypeId == type && defaults.ObjectId == objectId
                        select defaults.ReferenceId;

            return query.ToList();
        }

        public IQueryable<Permission> GetUserCityAccess(long userId)
        {
            var query = from defaults in this.Permissions
                        where defaults.PermissionTypeId == PermissionTypeEnum.CityArea && defaults.ObjectId == userId
                        select defaults;

            return query;
        }

        public IQueryable<ToolModel> GetUserToolAccess(long userId)
        {
            var query = from permission in this.Permissions
                        join tool in this.Tools on permission.ReferenceId equals tool.ToolId
                        where permission.PermissionTypeId == PermissionTypeEnum.Tool && permission.ObjectId == userId
                        select new ToolModel {
                            ToolId = tool.ToolId,
                            Name = tool.Name,
                            Order = tool.Order,
                            AddToQuote = tool.AddToQuote == null ? false: tool.AddToQuote
                        };

            return query.Distinct().OrderBy(t => t.ToolId);
        }

        public IQueryable<ProductFamilyModel> GetProductFamilyAccess(long userId)
        {
            var query = from permission in this.Permissions
                        join family in this.ProductFamilies on permission.ReferenceId equals family.ProductFamilyId
                        where permission.PermissionTypeId == PermissionTypeEnum.ProductFamily && permission.ObjectId == userId
                        select new ProductFamilyModel
                        {
                            ProductFamilyId = family.ProductFamilyId,
                            Name = family.Name,
                            Order = family.Order
                        };

            return query.Distinct().OrderBy(p => p.Order);
        }

        public IQueryable<BrandModel> GetBrandAccess(long userId)
        {
            var query = from permission in this.Permissions
                        join brand in this.Brands on permission.ReferenceId equals brand.BrandId
                        where permission.PermissionTypeId == PermissionTypeEnum.Brand && permission.ObjectId == userId
                        select new BrandModel
                        {
                            BrandId = brand.BrandId,
                            Name = brand.Name
                        };

            return query;
        }


        public IQueryable<Permission> GetPermissionList(long objectId)
        {
            var query = from defaults in this.Permissions
                        where defaults.ObjectId == objectId
                        select defaults;

            return query;
        }

       
        public void CopyPermissions(EntityEnum fromEntity, long fromId, EntityEnum toEntity, long toId)
        {
            var permissionsToAdd = GetPermissionList(fromId)
                .Select(p => new
                {
                    permissionId = p.PermissionId,
                    type = p.PermissionTypeId,
                    referenceEntityId = p.ReferenceEntityId,
                    referenceId = p.ReferenceId
                }).ToList();

            permissionsToAdd
                .ForEach(p => AddPermission(p.permissionId, toEntity, toId, p.referenceEntityId, (int)p.referenceId, p.type));

        }

        public void ReplacePermissions(EntityEnum fromEntity, long fromId, EntityEnum toEntity, long toId)
        {
            var permissionsToRemove = GetPermissionList(toId).Select(p => p).ToList();

            this.Context.Permissions.RemoveRange(permissionsToRemove);

            var permissionsToAdd = GetPermissionList(fromId)
                .Select(p => new { permissionId = p.PermissionId, type = p.PermissionTypeId, referenceId = p.ReferenceId, referenceEntityId = p.ReferenceEntityId }).ToList();

            permissionsToAdd.ForEach(p => AddPermission(p.permissionId, toEntity, toId, p.referenceEntityId, (int)p.referenceId, p.type));

        }

        public void ReplacePermissions(EntityEnum fromEntity, long fromId, EntityEnum toEntity, long toId, PermissionTypeEnum type)
        {
            var permissionsToRemove = GetPermissionList(toId).Select(p => p)
                                     .Where(p => p.PermissionTypeId == type)
                                     .ToList();

            this.Context.Permissions.RemoveRange(permissionsToRemove);

            var permissionsToAdd = GetPermissionList(fromId)
                .Where(p => p.PermissionTypeId == type)
                .Select(p => new { permissionId = p.PermissionId, referenceId = p.ReferenceId, referenceEntityId = p.ReferenceEntityId }).ToList();

            permissionsToAdd.ForEach(p => AddPermission(p.permissionId, toEntity, toId, p.referenceEntityId, (int)p.referenceId, type));

        }

        /// <summary>
        /// Returns the list of permissions for a given permission type
        /// </summary>
        /// <param name="defaultId">The id of the default list</param>
        /// <param name="selectedId">The id of the selected list</param>
        /// <param name="permissionType">The permission type</param>
        /// <returns></returns>
        /// 
        public List<PermissionListModel> GetPermissionResultListModel(long defaultId, long selectedId, PermissionTypeEnum permissionType, long? userId)
        {
            IQueryable<PermissionListModel> results = null;

            // get the default permissions and mark any as active if the active list contains identical records
            var query = (from defaults in this.Permissions
                         where defaults.PermissionTypeId == permissionType && defaults.ObjectId == defaultId

                         join selected in this.Permissions on new { a = selectedId, b = permissionType, c = defaults.ReferenceId } equals
                                                              new { a = selected.ObjectId, b = selected.PermissionTypeId, c = selected.ReferenceId } into Lca
                         from selected in Lca.DefaultIfEmpty()

                         select new { ReferenceId = defaults.ReferenceId, isActive = selected != null })

                          .Distinct();

            switch (permissionType)
            {
                case PermissionTypeEnum.CityArea:
                    results = from perm in query
                              join reference in this.CityAreas on perm.ReferenceId equals reference.CityAreaId
                              select new PermissionListModel
                              {
                                  ReferenceId = perm.ReferenceId,
                                  Description = reference.Name,
                                  IsSelected = perm.isActive
                              };
                    break;

                case PermissionTypeEnum.Brand:
                    results = from perm in query
                              join reference in this.Brands on perm.ReferenceId equals reference.BrandId
                              select new PermissionListModel
                              {
                                  ReferenceId = perm.ReferenceId,
                                  Description = reference.Name,
                                  IsSelected = perm.isActive
                              };
                    break;

                case PermissionTypeEnum.ProductFamily:
                    results = from perm in query
                              join reference in this.ProductFamilies on perm.ReferenceId equals reference.ProductFamilyId
                              select new PermissionListModel
                              {
                                  ReferenceId = perm.ReferenceId,
                                  Description = reference.Name,
                                  IsSelected = perm.isActive
                              };
                    break;

                case PermissionTypeEnum.Tool:
                    results = from perm in query
                              join reference in this.Tools on perm.ReferenceId equals reference.ToolId
                              select new PermissionListModel
                              {
                                  ReferenceId = perm.ReferenceId,
                                  Description = reference.Name,
                                  IsSelected = perm.isActive
                              };
                    break;

                case PermissionTypeEnum.SystemAccess:
                    results = from perm in query
                              join reference in this.SystemAccesses on perm.ReferenceId equals reference.SystemAccessId
                              select new PermissionListModel
                              {
                                  ReferenceId = perm.ReferenceId,
                                  Description = reference.Name,
                                  IsSelected = perm.isActive
                              };
                    break;
            }

            IQueryable<PermissionListModel> newResults = null;
             if(userId == null)
            {
                userId = selectedId;
            }

            if (permissionType == PermissionTypeEnum.ProductFamily)
            {
                var newQuery = (from defaults in this.Context.Permissions
                                where defaults.PermissionTypeId == permissionType && defaults.ObjectId == userId
                                select new
                                {
                                    ReferenceId = defaults.ReferenceId,
                                    isActive = defaults != null
                                }).Distinct();

                newResults = from q in newQuery
                              join reference in this.ProductFamilies on
                              q.ReferenceId equals reference.ProductFamilyId
                              select new PermissionListModel
                              {
                                  ReferenceId = q.ReferenceId,
                                  Description = reference.Name,
                                  IsSelected = q.isActive
                              };
            }

            List<PermissionListModel> values = new List<PermissionListModel>();
            values = results.ToList().OrderBy(o => o.ReferenceId).ToList();

            if (newResults != null)
            {
                foreach (var item in newResults)
                {
                    if (results.Where(r => r.ReferenceId == item.ReferenceId).Count() < 1)
                    {
                        values.Add(item);
                    }
                }
            }

            //return results.ToList().OrderBy(o => o.ReferenceId).ToList();
            return values;
            

        }

        public List<long> PermissionsUpdate(EntityEnum parentObjectEntity, long? parentObjectId, EntityEnum objectEntity, long? objectId, List<PermissionListModel> permissionList, PermissionTypeEnum type)
        {

            if (objectId == null || default(long) == objectId || permissionList == null) return null;

            var dbSelection = this.Context.Permissions.Where(p => p.ObjectId == objectId && p.PermissionTypeId == type).Select(r => r).ToList();

            var dbSelectionIds = dbSelection.Select(p =>
                new
                {
                    p.ReferenceId,
                    p.ReferenceEntityId
                }).ToList();

            var newSelection = permissionList.Where(s => s.IsSelected).Select(i => new
            {
                i.ReferenceId,
                i.ReferenceEntityId
            }).ToList();

            // Remove any permissions not selected and in permissions table.
            dbSelection.ForEach(p =>
            {
                if (newSelection.Any(w => w.ReferenceId == p.ReferenceId) == false)
                {
                    var list = this.Context.FnGetPermissionsUnderPermissionId(p.PermissionId).ToList();
                    list.ForEach(p1 => this.Context.Permissions.Remove(p1));
                }
            });

            List<long> newPermissionId = new List<long>();

            // Add any permissions selected but not in permissions table
            newSelection.ForEach(p =>
            {
                if (dbSelectionIds.Any(w => w.ReferenceId == p.ReferenceId) == false)
                {
                   newPermissionId.Add( this.AddPermission(parentObjectId, objectEntity, objectId.Value, p.ReferenceEntityId, p.ReferenceId, type));
                }
            });


            return newPermissionId;
        }

        public void UpdatePermissionAudit( EntityEnum UserType,
                                           User entity,
                                           List<PermissionListModel> permissionList, 
                                           PermissionTypeEnum type,
                                           UserSessionModel admin,
                                           List<Permission> dbSelection
                                          )
        {

            var newSelection = permissionList.Where(s => s.IsSelected).Select(i => new
            {
                i.ReferenceId,
                i.ReferenceEntityId
            }).ToList();

          
            var removeList = dbSelection.Where(dbs => !newSelection.Any(ns => ns.ReferenceId == dbs.ReferenceId)).ToList();
            var addList = newSelection.Where(ns => !dbSelection.Any(dbs => dbs.ReferenceId == ns.ReferenceId)).ToList();

            foreach (var item in removeList)
            {

                PermissionAudit permissionAuditModel = new PermissionAudit();
                permissionAuditModel.EffectedUserId = entity.UserId;
                permissionAuditModel.EffectedUserTypeId = (int)entity.UserTypeId;
                permissionAuditModel.ModifyByUserId = admin.UserId;
                permissionAuditModel.ModifyByUserTypeId = (int)admin.UserTypeId;
                permissionAuditModel.ModifyDate = DateTime.Now;
                permissionAuditModel.ObjectEntityId = (int)UserType;
                permissionAuditModel.ObjectId = entity.UserId;
                permissionAuditModel.ReferenceId = item.ReferenceId;
                permissionAuditModel.ReferenceEntityId = (int)item.ReferenceEntityId.Value;
                permissionAuditModel.PermissionTypeId = (byte)item.PermissionTypeId;
                permissionAuditModel.PermissionId = item.PermissionId;
                permissionAuditModel.ParentPermissionId = item.ParentPermissionId;
                permissionAuditModel.TypeOfAction = "Removed";
                permissionAuditModel.BusinessId = entity.BusinessId.Value;

                this.Context.PermissionAudits.Add(permissionAuditModel);
            }

            foreach(var item in addList)
            {
                var Id = this.Context.Permissions
                         .Where(p => p.ObjectId == entity.UserId &&
                                p.PermissionTypeId == PermissionTypeEnum.SystemAccess &&
                                p.ReferenceId == item.ReferenceId
                               ).Select(P => P.PermissionId).FirstOrDefault();

                PermissionAudit permissionAuditModel = new PermissionAudit();
                permissionAuditModel.EffectedUserId = entity.UserId;
                permissionAuditModel.EffectedUserTypeId = (int)entity.UserTypeId;
                permissionAuditModel.ModifyByUserId = admin.UserId;
                permissionAuditModel.ModifyByUserTypeId = (int)admin.UserTypeId;
                permissionAuditModel.ModifyDate = DateTime.Now;
                permissionAuditModel.ObjectEntityId = (int)entity.UserTypeId;
                permissionAuditModel.ObjectId = entity.UserId;
                permissionAuditModel.ReferenceId = item.ReferenceId;
                permissionAuditModel.ReferenceEntityId = (int)item.ReferenceEntityId;
                permissionAuditModel.PermissionTypeId = (byte)type;
                permissionAuditModel.BusinessId = entity.BusinessId.Value;

                var parentPermissionId = this.Context.Permissions
                                         .Where(p => p.PermissionId == Id)
                                         .Select(p => p.ParentPermissionId).FirstOrDefault();

                permissionAuditModel.PermissionId = Id;

                if (parentPermissionId == null)
                {
                    parentPermissionId = this.Context.Permissions
                                             .Where(p => p.ObjectId == entity.UserId &&
                                             p.ParentPermissionId != null
                                         ).OrderByDescending(p => p.PermissionId)
                                         .Select(p => p.ParentPermissionId).FirstOrDefault();
                }

                permissionAuditModel.ParentPermissionId = parentPermissionId;

                permissionAuditModel.TypeOfAction = "Added";
           
                this.Context.PermissionAudits.Add(permissionAuditModel);

            }

            this.Context.SaveChanges();
        }

        public void UpdatePermissionAudit(EntityEnum UserType,
                                           User entity,
                                           List<Permission> permissionList,
                                           PermissionTypeEnum type,
                                           UserSessionModel admin,
                                           List<Permission> dbSelection
                                          )
        {
            var removeList = dbSelection.Where(dbs => !permissionList.Any(ns => ns.ReferenceId == dbs.ReferenceId)).ToList();
            var addList = permissionList.Where(ns => !dbSelection.Any(dbs => dbs.ReferenceId == ns.ReferenceId)).ToList();

            foreach (var item in removeList)
            {

                PermissionAudit permissionAuditModel = new PermissionAudit();
                permissionAuditModel.EffectedUserId = entity.UserId;
                permissionAuditModel.EffectedUserTypeId = (int)entity.UserTypeId;
                permissionAuditModel.ModifyByUserId = admin.UserId;
                permissionAuditModel.ModifyByUserTypeId = (int)admin.UserTypeId;
                permissionAuditModel.ModifyDate = DateTime.Now;
                permissionAuditModel.ObjectEntityId = (int)UserType;
                permissionAuditModel.ObjectId = entity.UserId;
                permissionAuditModel.ReferenceId = item.ReferenceId;
                permissionAuditModel.ReferenceEntityId = (int)item.ReferenceEntityId.Value;
                permissionAuditModel.PermissionTypeId = (byte)item.PermissionTypeId;
                permissionAuditModel.PermissionId = item.PermissionId;
                permissionAuditModel.ParentPermissionId = item.ParentPermissionId;
                permissionAuditModel.TypeOfAction = "Removed";
                permissionAuditModel.BusinessId = entity.BusinessId.Value;

                this.Context.PermissionAudits.Add(permissionAuditModel);
            }

            foreach (var item in addList)
            {
                var Id = this.Context.Permissions
                         .Where(p => p.ObjectId == entity.UserId &&
                                p.PermissionTypeId == PermissionTypeEnum.SystemAccess &&
                                p.ReferenceId == item.ReferenceId
                               ).Select(P => P.PermissionId).FirstOrDefault();

                PermissionAudit permissionAuditModel = new PermissionAudit();
                permissionAuditModel.EffectedUserId = entity.UserId;
                permissionAuditModel.EffectedUserTypeId = (int)entity.UserTypeId;
                permissionAuditModel.ModifyByUserId = admin.UserId;
                permissionAuditModel.ModifyByUserTypeId = (int)admin.UserTypeId;
                permissionAuditModel.ModifyDate = DateTime.Now;
                permissionAuditModel.ObjectEntityId = (int)entity.UserTypeId;
                permissionAuditModel.ObjectId = entity.UserId;
                permissionAuditModel.ReferenceId = item.ReferenceId;
                permissionAuditModel.ReferenceEntityId = (int)item.ReferenceEntityId;
                permissionAuditModel.PermissionTypeId = (byte)type;
                permissionAuditModel.BusinessId = entity.BusinessId.Value;

                var parentPermissionId = this.Context.Permissions
                                         .Where(p => p.PermissionId == Id)
                                         .Select(p => p.ParentPermissionId).FirstOrDefault();

                permissionAuditModel.PermissionId = Id;

                if (parentPermissionId == null)
                {
                    parentPermissionId = this.Context.Permissions
                                             .Where(p => p.ObjectId == entity.UserId &&
                                             p.ParentPermissionId != null
                                         ).OrderByDescending(p => p.PermissionId)
                                         .Select(p => p.ParentPermissionId).FirstOrDefault();
                }

                permissionAuditModel.ParentPermissionId = parentPermissionId;

                permissionAuditModel.TypeOfAction = "Added";

                this.Context.PermissionAudits.Add(permissionAuditModel);

            }

            this.Context.SaveChanges();
        }

        #region City Areas

        public CityArea CreateCityArea(string description)
        {
            var entity = new CityArea();

            entity.CityAreaId = this.Context.GenerateNextIntId();

            entity.Name = description;

            this.Context.CityAreas.Add(entity);

            return entity;
        }

        // public void AddPermission(long? parentObjectId, User entity, CityArea child)
        //{
        //    AddPermission(parentObjectId, entity.UserId, childAreaId, PermissionTypeEnum.CityArea);
        //}

        //public void AddPermission(Group entity, CityArea child)
        //{
        //    AddPermission(entity.GroupId, childAreaId, PermissionTypeEnum.CityArea);
        //}

        public void AddPermission(BusinessType entity, CityArea child)
        {
            AddPermission(null, EntityEnum.BusinessType, (long)entity.BusinessTypeId, EntityEnum.CityArea, child.CityAreaId, PermissionTypeEnum.CityArea);
        }

        //public void AddPermission(long? parentObjectId, Business entity, CityArea child)
        //{
        //    AddPermission(parentObjectId, entity.BusinessId, childAreaId, PermissionTypeEnum.CityArea);
        //}

        #endregion

        #region Brands

        public Brand CreateBrand(string description)
        {
            var entity = new Brand();

            entity.BrandId = this.Context.GenerateNextIntId();

            entity.Name = description;

            this.Context.Brands.Add(entity);

            return entity;
        }


        public long AddPermission(long? parentPermissionId, EntityEnum objectEntityType, long objectId, EntityEnum? entityReferenceType, int referenceId, PermissionTypeEnum type)
        {
            var newPermissionId = this.Context.GenerateNextLongId();

            this.Context.Permissions.Add(new Permission
            {
                PermissionId = newPermissionId,
                ParentPermissionId = parentPermissionId,
                ObjectEntityId = objectEntityType,
                ObjectId = objectId,
                PermissionTypeId = type,
                ReferenceEntityId = entityReferenceType,
                ReferenceId = referenceId
            });

            return newPermissionId;
        }

        //public void AddPermission(long? parentObjectId, User entity, Brand child)
        //{
        //    AddPermission(parentObjectId, entity.UserId, child.BrandId, PermissionTypeEnum.Brand);
        //}

        //public void AddPermission(Group entity, Brand child)
        //{
        //    AddPermission(entity.GroupId, child.BrandId, PermissionTypeEnum.Brand);
        //}

        public void AddPermission(BusinessType entity, Brand child)
        {
            AddPermission(null, EntityEnum.BusinessType, (long)entity.BusinessTypeId, EntityEnum.Brand, child.BrandId, PermissionTypeEnum.Brand);
        }

        //public void AddPermission(long? parentObjectId, Business entity, Brand child)
        //{
        //    AddPermission(parentObjectId,entity.BusinessId, child.BrandId, PermissionTypeEnum.Brand);
        //}

        #endregion

        #region Product families

        public ProductFamily ProductFamilyCreate(string description)
        {
            var entity = new ProductFamily();

            entity.ProductFamilyId = this.Context.GenerateNextIntId();

            entity.Description = description;

            this.Context.ProductFamilies.Add(entity);

            return entity;
        }

        //public void AddPermission(long? parentObjectId, User entity, ProductFamily child)
        //{
        //    AddPermission(parentObjectId, entity.UserId, child.ProductFamilyId, PermissionTypeEnum.ProductFamily);
        //}

        //public void AddPermission(Group entity, ProductFamily child)
        //{
        //    AddPermission(entity.GroupId, child.ProductFamilyId, PermissionTypeEnum.ProductFamily);
        //}

        public void AddPermission(BusinessType entity, ProductFamily child)
        {
            AddPermission(null, EntityEnum.BusinessType, (long)entity.BusinessTypeId, EntityEnum.ProductFamily, child.ProductFamilyId, PermissionTypeEnum.ProductFamily);
        }

        //public void AddPermission(long? parentObjectId, Business entity, ProductFamily child)
        //{
        //    AddPermission(parentObjectId,entity.BusinessId, child.ProductFamilyId, PermissionTypeEnum.ProductFamily);
        //}

        #endregion

        #region Tools

        public Tool ToolCreate(string description, int id)
        {
            var entity = new Tool();

            entity.ToolId = id;

            entity.Name = description;

            this.Context.Tools.Add(entity);

            return entity;
        }

        //public void AddPermission(long? parentObjectId, User entity, Tool child)
        //{
        //    AddPermission(parentObjectId, entity.UserId, child.ToolId, PermissionTypeEnum.Tool);
        //}

        //public void AddPermission(Group entity, Tool child)
        //{
        //    AddPermission(entity.GroupId, child.ToolId, PermissionTypeEnum.Tool);
        //}


        public void AddPermission(BusinessType entity, Tool child)
        {
            AddPermission(null, EntityEnum.BusinessType, (long)entity.BusinessTypeId, EntityEnum.Tool, child.ToolId, PermissionTypeEnum.Tool);
        }

        //public void AddPermission(long parentObjectId, Business entity, Tool child)
        //{
        //    AddPermission(parentObjectId, entity.BusinessId, child.ToolId, PermissionTypeEnum.Tool);
        //}

        #endregion

        #region System acess

        public SystemAccess SystemAccessCreate(SystemAccessEnum systemAccess, string description)
        {
            var entity = this.Context.SystemAccesses.Create();

            entity.SystemAccessId = (int)systemAccess;

            entity.Name = description;

            this.Context.SystemAccesses.Add(entity);

            return entity;
        }

        public void AddPermission(long? parentObjectId, UserType entity, SystemAccess child)
        {
            AddPermission(null, EntityEnum.UserType, (long)entity.UserTypeId, EntityEnum.SystemAccess, child.SystemAccessId, PermissionTypeEnum.SystemAccess);
        }

        #endregion
    }
}