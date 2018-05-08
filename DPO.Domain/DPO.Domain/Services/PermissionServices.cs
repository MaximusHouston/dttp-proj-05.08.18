using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
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
using System.IO;

namespace DPO.Domain
{
    public partial class PermissionServices : BaseServices
    {
        public PermissionServices() : base() { }
        public PermissionServices(DPOContext context) : base(context) { }

        public List<PermissionListModel> GetPermissonsForBusiness(UserSessionModel currentUser, long businessId, PermissionTypeEnum permissonType)
        {
            long defaultPermissionListId = 0;
            List<PermissionListModel> permissionListModel = new List<PermissionListModel>();

            if (currentUser != null)
            {

                if (currentUser.UserTypeId == UserTypeEnum.DaikinSuperUser)
                {
                    defaultPermissionListId = (long)BusinessTypeEnum.Daikin;
                }
                else
                {
                    defaultPermissionListId = (long)this.Db.BusinessQueryByBusinessId(currentUser, businessId)
                                                        .Select(g => g.BusinessTypeId)
                                                        .FirstOrDefault();
                }

                permissionListModel = this.Db.GetPermissionResultListModel(defaultPermissionListId, businessId, permissonType, currentUser.UserId);
            }
            else
            {
                permissionListModel = this.Db.GetPermissionResultListModel(defaultPermissionListId, businessId, permissonType, null);
            }

            return permissionListModel;
        }

        public List<PermissionListModel> GetPermissonsForUser(UserSessionModel currentUser, long userBeingEditedId, PermissionTypeEnum permissonType, bool isEditing = false)
        {
            long defaultPermissionListId = default(long);

            switch (permissonType)
            {
                case PermissionTypeEnum.Brand:
                case PermissionTypeEnum.CityArea:
                case PermissionTypeEnum.ProductFamily:
                    defaultPermissionListId = currentUser.BusinessId.Value;
                    break;

                case PermissionTypeEnum.SystemAccess:
                case PermissionTypeEnum.Tool:
                    defaultPermissionListId = currentUser.UserId;
                    break;
            }

            return this.Db.GetPermissionResultListModel(defaultPermissionListId, userBeingEditedId, permissonType, null);
        }


        public List<PermissionListModel> GetPermissonsForUserType(UserSessionModel currentUser, long businessId, UserTypeEnum? userType, PermissionTypeEnum permissonType, bool isEditing = false)
        {
            long defaultPermissionListId = default(long);

            if (!userType.HasValue) return new List<PermissionListModel>();

            switch (permissonType)
            {
                case PermissionTypeEnum.Brand:
                case PermissionTypeEnum.CityArea:
                case PermissionTypeEnum.ProductFamily:
                    defaultPermissionListId = (long)businessId;
                    break;
                case PermissionTypeEnum.SystemAccess:
                case PermissionTypeEnum.Tool:
                    defaultPermissionListId = currentUser.UserId;
                    break;
            }

            return this.Db.GetPermissionResultListModel(defaultPermissionListId, (long)userType, permissonType, null);
        }

        public ServiceResponse PostTool(ToolEditModel tool, HttpRequestBase Request)
        {
            this.Db.ReadOnly = false;
            this.Response.Model = tool;

            if (string.IsNullOrEmpty(tool.Name))
            {
                this.Response.AddError("Tool Name is required");
                return this.Response;
            }

            if (tool.ToolId == null)
            {
                //add new tool

                if (Request == null || Request.Files.Count != 1 || Request.Files[0] == null || Request.Files[0].ContentLength == 0)
                {
                    this.Response.AddError("Please select a file to upload");
                    return this.Response;
                }

                var file = Request.Files[0];

                if (Path.GetExtension(file.FileName) != ".zip")
                {
                    this.Response.AddError("Only zip files can be uploaded");
                    return this.Response;
                }

                string targetFilePath = Utilities.GetDocumentDirectory() + "\\Tools\\" + file.FileName;

                if (File.Exists(targetFilePath))
                {
                    this.Response.AddError("Zip file with name \"" + file.FileName + "\" already exists");
                    return this.Response;
                }

                try
                {
                    file.SaveAs(targetFilePath);
                }
                catch (Exception)
                {
                    this.Response.AddError("Unable to upload file, please try again");
                    return this.Response;
                }

                if (tool.PostedBusinessTypeIds.Length == 0)
                {
                    this.Response.AddError("Please select at least one business type that can view this tool");
                    return this.Response;
                }

                var newTool = new Tool
                {
                    Name = tool.Name,
                    Description = (string.IsNullOrEmpty(tool.Description)) ? "" : tool.Description,
                    Filename = file.FileName
                };

                var nextToolId = 10;
                var lastExistingTool = this.Db.Tools.OrderByDescending(i => i.ToolId).FirstOrDefault();
                if (lastExistingTool != null) nextToolId = lastExistingTool.ToolId + 10;

                newTool.ToolId = nextToolId;

                this.Db.Context.Tools.Add(newTool);

                List<Permission> NewPermissions = new List<Permission>();

                //loop through business types, add permissions
                //to that type as well as all of their users
                foreach (int selectedBusinessType in tool.PostedBusinessTypeIds)
                {
                    Permission businessTypeLevelPermission = new Permission
                    {
                        PermissionId = this.Context.GenerateNextLongId(),
                        ObjectId = selectedBusinessType,
                        PermissionTypeId = PermissionTypeEnum.Tool,
                        ReferenceId = newTool.ToolId
                    };

                    NewPermissions.Add(businessTypeLevelPermission);

                    List<Business> BusinessesInType = (from b in this.Db.Businesses
                                                       where (int)b.BusinessTypeId == selectedBusinessType
                                                       select b).ToList();

                    foreach (var business in BusinessesInType)
                    {
                        Permission businessPermission = new Permission
                        {
                            PermissionId = this.Context.GenerateNextLongId(),
                            ParentPermissionId = businessTypeLevelPermission.PermissionId,
                            ObjectId = business.BusinessId,
                            PermissionTypeId = PermissionTypeEnum.Tool,
                            ReferenceId = newTool.ToolId
                        };

                        NewPermissions.Add(businessPermission);

                        List<User> usersInBusiness = (from u in this.Db.Users
                                                      where u.BusinessId == business.BusinessId
                                                      select u).ToList();

                        foreach (var user in usersInBusiness)
                        {
                            Permission userPermission = new Permission
                            {
                                PermissionId = this.Context.GenerateNextLongId(),
                                ParentPermissionId = businessPermission.PermissionId,
                                ObjectId = user.UserId,
                                PermissionTypeId = PermissionTypeEnum.Tool,
                                ReferenceId = newTool.ToolId
                            };

                            NewPermissions.Add(userPermission);
                        }

                    }

                }

                this.Db.Context.Permissions.AddRange(NewPermissions);

                this.Db.SaveChanges();

                this.Response.AddSuccess("Tool \"" + tool.Name + "\" Added Successfully");
            }
            else
            {
                //save existing tool
                Tool existingTool = this.Db.Tools.Where(m => m.ToolId == tool.ToolId).FirstOrDefault();

                if (existingTool == null)
                {
                    this.Response.AddError("Tool no longer exists");
                    return this.Response;
                }

                //save new zip file if included in post
                if (Request != null && Request.Files.Count == 1 && Request.Files[0] != null && Request.Files[0].ContentLength > 0)
                {
                    //save new zip file
                    var file = Request.Files[0];

                    if (Path.GetExtension(file.FileName) != ".zip")
                    {
                        this.Response.AddError("Only zip files can be uploaded");
                        return this.Response;
                    }

                    string targetFilePath = Utilities.GetDocumentDirectory() + "\\Tools\\" + file.FileName;

                    if (File.Exists(targetFilePath))
                    {
                        this.Response.AddError("Zip file with name \"" + file.FileName + "\" already exists");
                        return this.Response;
                    }

                    try
                    {
                        file.SaveAs(targetFilePath);
                    }
                    catch (Exception)
                    {
                        this.Response.AddError("Unable to upload file, please try again");
                        return this.Response;
                    }

                    //delete out old zip file

                    string oldTargetFilePath = Utilities.GetDocumentDirectory() + "\\Tools\\" + existingTool.Filename;

                    if (targetFilePath != oldTargetFilePath)
                    {
                        try
                        {
                            File.Delete(oldTargetFilePath);
                        }
                        catch (Exception) { }
                    }

                    existingTool.Filename = file.FileName;
                }

                existingTool.Name = tool.Name;
                existingTool.Description = (string.IsNullOrEmpty(tool.Description)) ? "" : tool.Description;

                this.Response.AddSuccess("Tool changes saved");
            }

            this.Db.SaveChanges();

            return this.Response;
        }

        public List<ToolModel> GetToolLinksForEdit()
        {
            this.Response.Messages.Clear();

            List<ToolModel> allTools = (from tool in this.Db.Context.Tools
                                        select new ToolModel
                                        {
                                            ToolId = tool.ToolId,
                                            Name = tool.Name,
                                            Filename = tool.Filename,
                                            Description = tool.Description
                                        }).ToList();

            return allTools;

        }

        public ToolEditModel GetToolForEdit(long? toolId)
        {
            List<BusinessModel> businessTypes = (from b in this.Db.BusinessTypes
                                                 select new BusinessModel
                                                 {
                                                     BusinessTypeId = (int)b.BusinessTypeId,
                                                     BusinessTypeDescription = b.Description
                                                 }).ToList();

            if (toolId == null) return new ToolEditModel
            {
                BusinessTypes = businessTypes
            };

            ToolEditModel tool = (from t in this.Db.Context.Tools
                                  where t.ToolId == toolId
                                  select new ToolEditModel
                                  {
                                      ToolId = t.ToolId,
                                      Name = t.Name,
                                      Description = t.Description,
                                      Filename = t.Filename
                                  }).FirstOrDefault();

            tool.BusinessTypes = businessTypes;

            return tool;
        }

        public ServiceResponse DeleteTool(int toolId)
        {
            this.Db.ReadOnly = false;

            Tool tool = (from t in this.Db.Context.Tools
                         where t.ToolId == toolId
                         select t).FirstOrDefault();

            if (tool == null)
            {
                this.Response.AddError("Tool does not exist");
                return this.Response;
            }

            //delete file from server
            string targetFilePath = Utilities.GetDocumentDirectory() + "\\Tools\\" + tool.Filename;

            if (File.Exists(targetFilePath))
            {
                try
                {
                    File.Delete(targetFilePath);
                }
                catch (Exception)
                { }
            }

            //delete permissions in the db that reference the deleted tool
            IEnumerable<Permission> permissionsToRemove = (from p in this.Db.Context.Permissions
                                                           where p.PermissionTypeId == PermissionTypeEnum.Tool
                                                           && p.ReferenceId == tool.ToolId
                                                           select p);

            this.Db.Context.Permissions.RemoveRange(permissionsToRemove);

            //delete tool from db
            this.Db.Context.Tools.Remove(tool);

            this.Db.SaveChanges();
            this.Response.AddSuccess("Tool \"" + tool.Name + "\" Deleted");

            return this.Response;
        }


        public List<ToolModel> GetToolLinksForUser(UserSessionModel user)
        {
            using (var permissionServices = new PermissionServices())
            {
                var permissions = permissionServices.GetPermissonsForUser(user, user.UserId, PermissionTypeEnum.Tool).Where(p => p.IsSelected).ToList();

                var usersTools = new List<ToolModel>();

                var allTools = this.Db.Context.Tools.OrderBy(t => t.Order).ToList();

                //Add by tool order
                for (int t = 0; t < allTools.Count; t++)
                {
                    for (int p = 0; p < permissions.Count; p++)
                    {
                        if (allTools[t].ToolId == permissions[p].ReferenceId)
                        {
                            usersTools.Add(new ToolModel
                            {
                                ToolId = allTools[t].ToolId,
                                Name = allTools[t].Name,
                                Filename = allTools[t].Filename,
                                Description = allTools[t].Description,
                                Order = allTools[t].Order,
                                AddToQuote = allTools[t].AddToQuote,
                                AccessUrl = allTools[t].AccessUrl
                            });
                        }
                    }
                };

                return usersTools;
            }
        }

    }

}
