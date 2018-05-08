using DPO.Common;
using DPO.Common.Models.General;
using DPO.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace DPO.Domain
{
    public class HtmlServices : BaseServices
    {
        private ProductServices mProductServices;
        public HtmlServices()
            : base()
        {
        }

        public HtmlServices(DPOContext context)
            : base(context)
        {
        }

        private ProductServices ProductServices
        {
            get
            {
                if (this.mProductServices == null)
                {
                    this.mProductServices = new ProductServices(this.Context);
                }

                return this.mProductServices;
            }
        }
        public CheckBoxListModel CheckBoxListModelBusinessPermissions(UserSessionModel admin, long businessId, CheckBoxListModel model, PermissionTypeEnum type)
        {
            var permissionService = new PermissionServices(this.Context);

            // Return default permissions with selected items
            var permissions = permissionService.GetPermissonsForBusiness(admin, businessId, type);

            var result = GetCheckBoxListModel(admin, permissions, model);
            result.EntityReferenceId = GetEntityReferenceByPermissionType(type);

            return result;
        }

        public CheckBoxListModel CheckBoxListModelUserPermissions(UserSessionModel admin, long userId, CheckBoxListModel model, PermissionTypeEnum type, bool isEditing = false)
        {
            var permissionService = new PermissionServices(this.Context);

            // Return default permissions with selected items
            var permissions = permissionService.GetPermissonsForUser(admin, userId, type, isEditing);

            var result = GetCheckBoxListModel(admin, permissions, model);
            result.EntityReferenceId = GetEntityReferenceByPermissionType(type);

            return result;
        }

        public CheckBoxListModel CheckBoxListModelUserPermissions(UserSessionModel admin, long businessId, UserTypeEnum? userTypeId, CheckBoxListModel model, PermissionTypeEnum type, bool isEditing = false)
        {
            var permissionService = new PermissionServices(this.Context);

            // Return default permissions with selected items
            var permissions = permissionService.GetPermissonsForUserType(admin, businessId, userTypeId, type, isEditing);

            var result = GetCheckBoxListModel(admin, permissions, model);
            result.EntityReferenceId = GetEntityReferenceByPermissionType(type);

            return result;
        }

        public DropDownModel DropDownDateTypes(IEnumerable<DateTypeModel> items, int? selectedId)
        {
            var result = new List<SelectListItemExt>();

            if (selectedId == null)
            {
                selectedId = items.Where(w => w.Name == "Registration Date").FirstOrDefault().Id;
            }

            var ddi = items.Select(s =>
                new DropDownItem
                {
                    ValueLong = (long)s.Id,
                    Text = s.Name
                }
            );

            return DropDown(ddi, selectedId);
        }

        public DropDownModel DropDownFinancialYears(UserSessionModel user, int? selectedId = null)
        {
            int startYear = 2013;
            int endYear = DateTime.Now.AddYears(3).Year;
            List<int> years = new List<int>();

            if (selectedId == null)
            {
                // Subtract 3 months for financial
                selectedId = DateTime.Now.AddMonths(-3).Year;
            }

            for (int i = startYear; i <= endYear; i++)
            {
                years.Add(i);
            }

            var ddi = years.Select(i => new DropDownItem { ValueLong = (long)i, Text = i.ToString() });

            return DropDown(ddi, selectedId);
        }

        public List<SelectListItemExt> DropDownListUserTypes(UserSessionModel admin)
        {
            var items = Db.GetUserTypes(admin).OrderBy(c => c.Description)
                 .Select(i => new SelectListItemExt { ValueLong = (byte)i.UserTypeId, Text = i.Description })
               .Cache()
               .ToList();

            return items;
        }

        public DropDownModel DropDownModel(List<SelectListItemExt> items, string selected)
        {
            selected = selected ?? "";

            items.ForEach(i => i.Selected = (i.Value == selected));

            var model = new DropDownModel { Items = items };

            return model;
        }

        public DropDownModel DropDownModelBrandCompetitorTypes(int? selectedId)
        {
            var result = Db.Context.BrandCompetitorTypes.AsNoTracking().OrderBy(c => c.Description)
                .Select(i => new DropDownItem { ValueLong = (long)i.BrandCompetitorTypeId, Text = i.Description });

            return DropDown(result, selectedId);
        }

        public DropDownModel DropDownModelBusineesForProjects(UserSessionModel admin, long? selectedId)
        {
            var result = (from b in this.Db.QueryBusinessViewableByUser(admin, true)
                          where b.Enabled == true
                          join u in this.Context.Users
                          on b.BusinessId equals u.BusinessId
                          join p in this.Context.Projects
                          on u.UserId equals p.OwnerId
                          where (p.ProjectStatusTypeId == ProjectStatusTypeEnum.Open)
                          select new DropDownItem
                         {
                             ValueLong = b.BusinessId,
                             Text = b.BusinessName
                         }).Distinct().OrderBy(b => b.Text);


            return DropDown(result, selectedId);

        }

        public DropDownModel DropDownModelBusinesses(UserSessionModel admin, long? selectedId)
        {

            var test = this.Db.QueryBusinessViewableByUser(admin, true).ToList();

            var result = this.Db.QueryBusinessViewableByUser(admin, true)
                        .Where(b => b.Enabled || b.BusinessId == selectedId)
                        .Select(i => new DropDownItem { ValueLong = i.BusinessId, Text = i.BusinessName });

            var temp = result.ToList();

            return DropDown(result, selectedId);
        }

        public DropDownModel DropDownModelBusinessTypes(int? selectedId)
        {
            var result = Db.BusinessTypes
                         .Select(
                            i => new DropDownItem
                            {
                                ValueLong = (long)i.BusinessTypeId,
                                Text = i.Description,
                                RequirementLevel = i.BusinessIdRequirementLevelId
                            });

            return DropDown(result, selectedId);
        }

        public DropDownModel DropDownModelCommissionMultiplerTypes(decimal? selectedId)
        {
            selectedId = selectedId ?? default(int);

            var result = Db.GetCommissionMultipliers()
                              .Select(i => new { ValueDecimal = i.Multiplier, Text = string.Format("{0} ({1:P2})", i.Multiplier, i.CommissionPercentage / 100M) })
                            .ToList();

            var items = result.Select(i => new SelectListItemExt { ValueDecimal = i.ValueDecimal, Text = i.Text, Selected = (i.ValueDecimal == selectedId) }).ToList();

            var model = new DropDownModel { Items = items };

            return model;
        }

        public DropDownModel DropDownModelConstructionTypes(int? selectedId)
        {
            var result = Db.Context.ConstructionTypes.AsNoTracking().OrderBy(c => c.Description)
                         .Select(i => new DropDownItem { ValueLong = (long)i.ConstructionTypeId, Text = i.Description });

            return DropDown(result, selectedId);
        }

        public DropDownModel DropDownModelCountries(AddressModel address)
        {
            return DropDownModelCountries((address == null) ? (string)null : address.CountryCode);
        }

        public DropDownModel DropDownModelDaikinEquipmentAtAdvantageTypes(int? selectedId)
        {
            var result = Db.Context.DaikinEquipmentAtAdvantageTypes.AsNoTracking().OrderBy(c => c.Description)
                .Select(i => new DropDownItem { ValueLong = (long)i.DaikinEquipmentAtAdvantageTypeId, Text = i.Description });

            return DropDown(result, selectedId);
        }

        public DropDownModel DropDownModelDiscountRequestStatusTypes(int? selectedId)
        {
            var result = Db.Context.DiscountRequestStatusTypes.AsNoTracking().OrderBy(c => c.Description)
                .Select(i => new DropDownItem { ValueLong = (long)i.DiscountRequestStatusTypeId, Text = i.Description });

            return DropDown(result, selectedId);
        }

        public DropDownModel DropDownModelCommissionRequestStatusTypes(int? selectedId)
        {
            var result = Db.Context.CommissionRequestStatusTypes.AsNoTracking().OrderBy(c => c.Description)
                .Select(i => new DropDownItem { ValueLong = (long)i.CommissionRequestStatusTypeId, Text = i.Description });

            return DropDown(result, selectedId);
        }

        public DropDownModel DropDownModelLinks(string category, string selected)
        {
            var items = Db.GetActiveLinks(category)
                .Select(l => new SelectListItemExt() { Value = l.LinkUrl, Text = l.LinkName })
                .ToList();

            return DropDown(items, selected);
        }

        public DropDownModel DropDownModelPowerVoltages(int? selectedId)
        {
            var result = (from ps in Db.Context.ProductSpecificationKeyLookups
                          join pl in Db.Context.ProductSpecificationLabels on ps.ProductSpecificationLabelId equals pl.ProductSpecificationLabelId
                          where pl.Name == "PowerVoltage"
                          select ps)
                         .AsNoTracking().OrderBy(c => c.Value)
                         .Select(i => new DropDownItem { Value = i.Key, Text = i.Value });

            return DropDown(result, selectedId);
        }

        public DropDownModel DropDownModelProbabilityOfCloseTypes(int? selectedId)
        {
            var result = Db.Context.ProbablilityOfCloseTypes.AsNoTracking().OrderBy(c => c.Description)
                .Select(i => new DropDownItem { ValueLong = (long)i.ProbablilityOfCloseTypeId, Text = i.Description });

            var dropDown = DropDown(result, selectedId);

            dropDown.Items = dropDown.Items.OrderBy(x => x.ValueLong).ToList();

            return dropDown;
        }

        public DropDownModel DropDownModelProductCategory(long? selectedId)
        {
            var result = (from cat in Db.Context.ProductCategories
                          select cat)
                        .AsNoTracking().OrderBy(c => c.Name)
                        .Select(i => new DropDownItem { ValueLong = (long)i.ProductCategoryId, Text = i.Name });

            return DropDown(result, selectedId);
        }

        public DropDownModel DropDownModelProductCategory(IQueryable<Product> query, long? selectedId)
        {
            var result = query.Select(i => new DropDownItem { ValueLong = (long)i.ProductCategoryId, Text = i.ProductCategory.Name })
                        .Distinct().AsNoTracking().OrderBy(c => c.Text);

            return DropDown(result, selectedId, true);
        }

        public DropDownModel DropDownModelProductCategory(IQueryable<Product> query, ProductModelTypeEnum? filterSubProduct, long? selectedId)
        {
            var primaryLookups = query
                .Where(
                    w => w != null
                        && (w.ProductModelTypeId != (int)ProductModelTypeEnum.System
                            || filterSubProduct == null))
                .Select(s => s.ProductCategory);



            if (filterSubProduct.HasValue)
            {
                var systemLookups = query
                    .Select(
                        s =>
                            s.ParentProductAccessories
                                .FirstOrDefault(
                                  ppa =>
                                        s.ProductModelTypeId == (int)ProductModelTypeEnum.System
                                        && ppa.RequirementTypeId == (int)RequirementTypeEnums.Standard
                                        && ppa.Product.ProductModelTypeId == (int)filterSubProduct
                                 )
                                .Product.ProductCategory
                    ).Where(w => w != null);

                primaryLookups = primaryLookups.Union(systemLookups);
            }

            var result = primaryLookups
                .Select(i => new DropDownItem
                {
                    ValueLong = (long)i.ProductCategoryId,
                    Text = i.Name
                })
                .Distinct().AsNoTracking().OrderBy(c => c.Text);

            return DropDown(result, selectedId, true);
        }

        public DropDownModel DropDownModelProductSortBy(string sortby)
        {
            var result = new List<SelectListItemExt>
          {
              new SelectListItemExt { Value= "Name", Text = "Product Name" },
              new SelectListItemExt { Value= "ProductNumber", Text = "Product Number" }
          };

            return DropDown(result, sortby);
        }

        public DropDownModel DropDownModelProjectExportTypes(ProjectExportTypeEnum? selectedId)
        {
            selectedId = selectedId ?? ProjectExportTypeEnum.Pipeline;

            var result = Db.ProjectExportTypes.Cache()
                        .Select(i => new DropDownItem { ValueLong = (long)i.ProjectExportTypeId, Text = i.Description }).OrderBy(c => c.ValueLong);

            return DropDown(result, (int)selectedId);
        }

        public DropDownModel DropDownModelProjectOpenTypes(int? selectedId)
        {
            var result = Db.ProjectOpenStatusTypes.AsNoTracking().Cache().OrderBy(c => c.Order).OrderBy(c => c.Description)
                         .Select(i => new DropDownItem { ValueLong = (long)i.ProjectOpenStatusTypeId, Text = i.Description });

            return DropDown(result, selectedId);
        }

        public DropDownModel DropDownModelProjectStatuses(int? selectedId, DropDownMode mode)
        {
            var result = Db.ProjectStatusTypes.AsNoTracking().Cache().OrderByDescending(c => c.Description)
                .Select(i => new DropDownItem { ValueLong = (long)i.ProjectStatusTypeId, Text = i.Description });

            if (mode == DropDownMode.NewRecord)
            {
                result = result.Where(d => d.ValueLong == (int)ProjectStatusTypeEnum.Open);
            }

            return DropDown(result, selectedId);
        }

        public DropDownModel DropDownModelWinLossConditionTypes(int? selectedId, DropDownMode mode)
        {
            var result = Db.Context.WinLossTypes.AsNoTracking()
                           .OrderByDescending(w => w.WinLossTypeDescription)
                           .Select(i => new DropDownItem { ValueLong = (long)i.WinLossTypeId, Text = i.WinLossTypeDescription });

            return DropDown(result, selectedId);
        }

        public DropDownModel DropDownModelFundingTypes(int? selectedId, DropDownMode mode)
        {
            var result = Db.Context.FundingTypes.AsNoTracking()
                           .OrderByDescending(f => f.FundingTypeDescription)
                           .Select(i => new DropDownItem { ValueLong = (long)i.FundingTypeId, Text = i.FundingTypeDescription });

            return DropDown(result, selectedId);
        }

        public DropDownModel DropDownModelCustomerTypes(int? selectedId, DropDownMode mode)
        {
            var result = Db.Context.CustomerTypes.AsNoTracking()
                           .OrderByDescending(c => c.CustomerTypeDescription)
                           .Select(i => new DropDownItem { ValueLong = (long)i.CustomerTypeId, Text = i.CustomerTypeDescription });

            return DropDown(result, selectedId);
        }

        public DropDownModel DropDownModelProjectTypes(int? selectedId)
        {
            var result = Db.ProjectTypes.Cache()
                           .Select(i => new DropDownItem { ValueLong = (long)i.ProjectTypeId, Text = i.Description });

            return DropDown(result, selectedId);
        }

        public DropDownModel DropDownModelSpecification(string specificationLabel, int? selectedId)
        {
            var result = (from ps in Db.Context.ProductSpecificationKeyLookups
                          join pl in Db.Context.ProductSpecificationLabels on ps.ProductSpecificationLabelId equals pl.ProductSpecificationLabelId
                          where pl.Name == specificationLabel
                          select ps)
                        .AsNoTracking().OrderBy(c => c.Value)
                        .Select(i => new DropDownItem { Value = i.Key, Text = i.Value });

            return DropDown(result, selectedId);
        }

        public DropDownModel DropDownModelSpecification(string specificationLabel, IQueryable<Product> query, int? selectedId = null)
        {
            return DropDownModelSpecification(specificationLabel, query, null, selectedId);
        }

        public DropDownModel DropDownModelSpecification(string specificationLabel, IQueryable<Product> query, ProductModelTypeEnum? filterSubProduct = null, int? selectedId = null)
        {
            var primaryLookups = query
                .Select(
                    s =>
                        s.ProductSpecifications
                            .Where(w => (s.ProductModelTypeId != (int)ProductModelTypeEnum.System || filterSubProduct == null)
                                && w.ProductSpecificationLabel.Name == specificationLabel)
                            .Select(
                                psl =>
                                    psl.ProductSpecificationLabel.ProductSpecificationKeyLookups
                                        .FirstOrDefault(wspkl => wspkl.Key.ToString() == psl.Value)
                            )
                            .FirstOrDefault()
                ).Distinct().Where(w => w != null);

            if (filterSubProduct.HasValue)
            {
                var systemLookups = query
                    .Select(
                        s =>
                            s.ParentProductAccessories
                                .FirstOrDefault(
                                  ppa =>
                                        s.ProductModelTypeId == (int)ProductModelTypeEnum.System
                                        && ppa.RequirementTypeId == (int)RequirementTypeEnums.Standard
                                        && ppa.Product.ProductModelTypeId == (int)filterSubProduct
                                 )
                                .Product.ProductSpecifications
                                    .Where(
                                        wpsl =>
                                            wpsl.ProductSpecificationLabel.Name == specificationLabel)
                                    .Select(
                                        psl =>
                                            psl.ProductSpecificationLabel.ProductSpecificationKeyLookups
                                                .FirstOrDefault(wspkl =>
                                                    wspkl.ProductSpecificationLabelId == psl.ProductSpecificationLabelId
                                                    && wspkl.Key.ToString() == psl.Value)
                                    )
                                    .FirstOrDefault()
                    ).Distinct().Where(w => w != null);

                primaryLookups = primaryLookups.Union(systemLookups);
            }

            var result = primaryLookups
                .Select(
                    s => new DropDownItem
                    {
                        Text = s.Value,
                        Value = s.Key
                    });

            return DropDown(result, selectedId, true);
        }

        public DropDownModel DropDownModelStates(AddressModel address)
        {
            return DropDownModelStates((address == null) ? (string)null : address.CountryCode, (address == null) ? (int?)null : address.StateId);
        }

        public DropDownModel DropDownModelSystemBasisDesignTypes(int? selectedId)
        {
            var result = Db.Context.SystemBasisDesignTypes.AsNoTracking().OrderBy(c => c.Description)
                .Select(i => new DropDownItem { ValueLong = (long)i.SystemBasisDesignTypeId, Text = i.Description });

            return DropDown(result, selectedId);
        }

        public DropDownModel DropDownModelUsersInGroup(UserSessionModel admin, long? selectedId)
        {
            selectedId = selectedId ?? default(int);

            var items = Db.QueryUsersViewableByProjectSearch(admin, new SearchUser { PageSize = Constants.DEFAULT_PAGESIZE_RETURN_ALL }, false, true).AsNoTracking().OrderBy(c => c.FirstName).ThenBy(c => c.LastName)
                         .Select(u => new UserModel { UserId = u.UserId, FirstName = u.FirstName, LastName = u.LastName })
                       .ToList();

            var result = items
                           .Select(i => new SelectListItemExt { ValueLong = i.UserId.Value, Text = Helpers.DisplayName(i.FirstName, null, i.LastName), Selected = (i.UserId == selectedId) })
                         .ToList();

            var model = new DropDownModel { Items = result };

            return model;
        }

        public DropDownModel DropDownModelUserTypes(UserSessionModel admin, UserTypeEnum? selectedId)
        {
            var items = DropDownListUserTypes(admin);

            selectedId = selectedId ?? UserTypeEnum.NotSet;

            string selected = ((selectedId.HasValue) ? ((int)selectedId).ToString() : "");

            return DropDownModel(items, selected);
        }

        public DropDownModel DropDownModelVerticalMarkets(int? selectedId)
        {
            var result = Db.VerticalMarketTypes.AsNoTracking().OrderBy(c => c.Description)
                .Select(i => new DropDownItem { ValueLong = (long)i.VerticalMarketTypeId, Text = i.Description });

            return DropDown(result, selectedId);
        }

        public DropDownModel DropDownModelZoneStrategyTypes(int? selectedId)
        {
            var result = Db.Context.ZoneStrategyTypes.AsNoTracking().OrderBy(c => c.Description)
                .Select(i => new DropDownItem { ValueLong = (long)i.ZoneStrategyTypeId, Text = i.Description });

            return DropDown(result, selectedId);
        }

        public DropDownModel DropDownModelProjectLeadStatusTypes(ProjectLeadStatusTypeEnum? selectedId)
        {
            var result = Db.Context.ProjectLeadStatusTypes.AsNoTracking().Cache().OrderBy(c => c.DisplayOrder)
                         .Select(i => new DropDownItem
                         {
                             ValueLong = (long)i.ProjectLeadStatusTypeId,
                             Text = i.Description,
                             Disabled = !i.UserEditable
                         });

            return DropDown(result, (int?)selectedId);
        }

        public DropDownModel DropDownModelProjectDarComStatusTypes(UserSessionModel admin, ProjectDarComStatusTypeEnum? selectedId)
        {
            DPO.Common.Models.Project.ProjectDarComTypesModel projectDarComTypeModel = new DPO.Common.Models.Project.ProjectDarComTypesModel();
            IEnumerable<ProjectDarComStatusTypeEnum> ProjectDarComTypes = Enum.GetValues(typeof(ProjectDarComStatusTypeEnum))
                                                       .Cast<ProjectDarComStatusTypeEnum>();

            IEnumerable<DropDownItem> result = null;

            var hasCommission = admin.HasAccess(SystemAccessEnum.RequestCommission) ||
                   admin.HasAccess(SystemAccessEnum.ApprovedRequestCommission) ||
                   admin.HasAccess(SystemAccessEnum.ViewRequestedCommission);

            var hasDiscountRequest = admin.HasAccess(SystemAccessEnum.RequestDiscounts) ||
                   admin.HasAccess(SystemAccessEnum.ApproveDiscounts) ||
                   admin.HasAccess(SystemAccessEnum.ViewDiscountRequest);


            if (hasCommission && !hasDiscountRequest)
            {
                result = from type in ProjectDarComTypes
                         where ((int)type == 2 || (int)type == 0)
                         select new DropDownItem
                        {
                            //ValueLong = (int)type,
                            Value = ((int)type).ToString(),
                            Text = type.ToString()
                        };

            }

            if (hasDiscountRequest && !hasCommission)
            {
                result = from type in ProjectDarComTypes
                         where ((int)type == 1 || (int)type == 0)
                         select new DropDownItem
                         {
                             //ValueLong = (int)type,
                             Value = ((int)type).ToString(),
                             Text = type.ToString()
                         };

            }

            if (hasDiscountRequest && hasCommission)
            {
                result = from type in ProjectDarComTypes
                         select new DropDownItem
                         {
                             // ValueLong = (int)type,
                             Value = ((int)type).ToString(),
                             Text = type.ToString()
                         };

            }

            //model.ProjectDarComTypes = projectDarComTypeModel;
            return DropDown(result, (int?)selectedId);
        }

        internal DropDownModel DropDownModelCoolingCapacity(IQueryable<Product> query, decimal? selectedId)
        {
            var result = query
                       .Join(
                           Db.Context.ProductSpecifications,
                           p => p.ProductId,
                           ps => ps.ProductId,
                           (p, ps) => new
                           {
                               ProductSpecLabel = ps.ProductSpecificationLabel,
                               ProductId = ps.ProductId,
                               ProductSpecificationValue = ps.Value,
                               Value = ps.Value
                           })
                       .Where(p => p.ProductSpecLabel.Name == "CoolingCapacityRated")
                       .OrderBy(p => p.Value)
                       .Select(p =>
                               new DropDownItem
                               {
                                   Value = p.Value,
                                   Text = p.Value
                               }).Distinct();

            return DropDown(result, selectedId);
        }

        internal DropDownModel DropDownModelHeatingCapacityRated(IQueryable<Product> query, decimal? selectedId)
        {
            var result = query
                       .Join(
                           Db.Context.ProductSpecifications,
                           p => p.ProductId,
                           ps => ps.ProductId,
                           (p, ps) => new
                           {
                               ProductSpecLabel = ps.ProductSpecificationLabel,
                               ProductId = ps.ProductId,
                               ProductSpecificationValue = ps.Value,
                               Value = ps.Value
                           })
                       .Where(p => p.ProductSpecLabel.Name == "HeatingCapacityRated")
                       .OrderBy(p => p.Value)
                       .Select(p =>
                               new DropDownItem
                               {
                                   Value = p.Value,
                                   Text = p.Value
                               }).Distinct();

            return DropDown(result, selectedId);
        }

        internal DropDownModel DropDownModelCoolingCapacityNominal(IQueryable<Product> query, decimal? selectedId)
        {
            var result = query
                       .Join(
                           Db.Context.ProductSpecifications,
                           p => p.ProductId,
                           ps => ps.ProductId,
                           (p, ps) => new
                           {
                               ProductSpecLabel = ps.ProductSpecificationLabel,
                               ProductId = ps.ProductId,
                               ProductSpecificationValue = ps.Value,
                               Value = ps.Value
                           })
                       .Where(p => p.ProductSpecLabel.Name == "CoolingCapacityNominal")
                       .OrderBy(p => p.Value)
                       .Select(p =>
                               new DropDownItem
                               {
                                   Value = p.Value,
                                   Text = p.Value
                               }).Distinct();

            return DropDown(result, selectedId);
        }

        internal DropDownModel DropDownModelAirFlowRateHigh(IQueryable<Product> query, decimal? selectedId)
        {
            var result = query
                       .Join(
                           Db.Context.ProductSpecifications,
                           p => p.ProductId,
                           ps => ps.ProductId,
                           (p, ps) => new
                           {
                               ProductSpecLabel = ps.ProductSpecificationLabel,
                               ProductId = ps.ProductId,
                               ProductSpecificationValue = ps.Value,
                               Value = ps.Value
                           })
                       .Where(p => p.ProductSpecLabel.Name == "AirFlowRateHigh")
                       .OrderBy(p => p.Value)
                       .Select(p =>
                               new DropDownItem
                               {
                                   Value = p.Value,
                                   Text = p.Value
                               }).Distinct();

            return DropDown(result, selectedId);
        }

        internal DropDownModel DropDownModelSpecification(string specificationName, IEnumerable<ProductModel> products, ProductModelTypeEnum[] includeSystemModelTypes, int? selectedItem)
        {
            var systemModelSpecItems = this.ProductServices.GetProductSpecificationsForSystem(products, includeSystemModelTypes, specificationName);

            var otherSpecItems = this.ProductServices.GetProductSpecifications(
                 products.Where(p => p.ProductModelTypeId != ProductModelTypeEnum.System).ToList(),
                     specificationName
             );

            IEnumerable<ProductSpecificationModel> specs = systemModelSpecItems ?? new List<ProductSpecificationModel>();

            if (otherSpecItems != null)
            {
                specs = specs.Concat(otherSpecItems);
            }

            var ddi = specs
                .Where(p => p != null)
                .Select(
                    p => new DropDownItem
                    {
                        Value = p.Key,
                        Text = p.Value
                    })
                .Distinct(new PropertyComparer<DropDownItem>("Value"));


            return DropDown(ddi, selectedItem);
        }

        private DropDownModel DropDown(IEnumerable<DropDownItem> query, string selectedId, bool noCache = false)
        {
            selectedId = selectedId ?? default(string);

            List<DropDownItem> ddItems = null;

            // Performance gains
            if (query is IQueryable)
            {
                IQueryable<DropDownItem> queryable = (query as IQueryable<DropDownItem>);
                queryable = queryable.AsNoTracking();

                if (!noCache)
                {
                    query = queryable.Cache();
                }
                else
                {
                    query = queryable as IEnumerable<DropDownItem>;
                }
            }

            ddItems = query
                .Distinct()
                .OrderBy(b => b.Text)
                .ToList();

            var items = ddItems.Select(i => new SelectListItemExt
            {
                Value = i.Value,
                Text = i.Text,
                DataRequirementLevel = i.RequirementLevel.HasValue ? i.RequirementLevel.ToString() : String.Empty,
                Selected = (i.Value == selectedId)
            }).ToList();

            var model = new DropDownModel { Items = items };

            return model;
        }

        private DropDownModel DropDown(IEnumerable<DropDownItem> query, long? selectedId, bool noCache = false)
        {
            selectedId = selectedId ?? default(long);

            List<DropDownItem> ddItems = null;

            // Performance gains
            if (query is IQueryable)
            {
                IQueryable<DropDownItem> queryable = (query as IQueryable<DropDownItem>);
                queryable = queryable.AsNoTracking();

                if (!noCache)
                {
                    query = queryable.Cache();
                }
                else
                {
                    query = queryable as IEnumerable<DropDownItem>;
                }
            }

            ddItems = query
                .Distinct()
                .OrderBy(b => b.Text)
                .ToList();

            var items = ddItems.Select(i => new SelectListItemExt
            {
                ValueLong = i.ValueLong,
                Text = i.Text,
                DataRequirementLevel = i.RequirementLevel.HasValue ? i.RequirementLevel.ToString() : String.Empty,
                Selected = (i.ValueLong == selectedId),
                Disabled = i.Disabled
            }).ToList();

            var model = new DropDownModel { Items = items };

            return model;
        }

        private DropDownModel DropDown(IEnumerable<DropDownItem> query, decimal? selectedId, bool noCache = false)
        {
            selectedId = selectedId ?? default(decimal);


            List<DropDownItem> ddItems = null;

            // Performance gains
            if (query is IQueryable)
            {
                IQueryable<DropDownItem> queryable = (query as IQueryable<DropDownItem>);
                queryable = queryable.AsNoTracking();

                if (!noCache)
                {
                    query = queryable.Cache();
                }
                else
                {
                    query = queryable as IEnumerable<DropDownItem>;
                }
            }

            ddItems = query
                .Distinct()
                //.OrderBy(b => b.Text)
                .ToList();

            var items = ddItems.Select(i => new SelectListItemExt
            {
                ValueDecimal = Decimal.Parse(i.Value),
                Text = i.Text,
                DataRequirementLevel = i.RequirementLevel.HasValue ? i.RequirementLevel.ToString() : String.Empty,
                Selected = (Decimal.Parse(i.Value) == selectedId)
            }).ToList();

            var model = new DropDownModel { Items = items };

            return model;
        }

        private DropDownModel DropDown(List<SelectListItemExt> query, string selectedId)
        {
            var result = query
                            .Distinct()
                            .OrderBy(b => b.Text)
                            .ToList();

            var items = result.Select(
                i => new SelectListItemExt
                {
                    Value = i.Value,
                    Text = i.Text,
                    DataRequirementLevel = i.DataRequirementLevel,
                    Selected = (i.Value == selectedId)
                }).ToList();

            var model = new DropDownModel { Items = items };

            return model;
        }

        private DropDownModel DropDownModelCountries(string selectedId)
        {
            var countries = (Utilities.Config("dpo.sys.countries") ?? "us").Split(',');

            selectedId = selectedId ?? "";

            var result = Db.Countries.AsNoTracking().Where(c => countries.Contains(c.CountryCode)).OrderBy(c => c.Name)
                    .Select(i => new { Value = i.CountryCode, Text = i.Name })
                    .Cache()
                    .ToList();

            var items = result.Select(i => new SelectListItemExt { Value = i.Value, Text = i.Text, Selected = (i.Value == selectedId) }).ToList();

            var model = new DropDownModel { Items = items };

            return model;
        }

        private DropDownModel DropDownModelStates(string countryCode, int? selectedId)
        {
            var query = Db.States;

            if (countryCode != null)
            {
                query = query.Where(d => d.CountryCode == countryCode);
            }

            var result = query.Select(i => new DropDownItem { ValueLong = i.StateId, Text = i.Name });

            return DropDown(result, selectedId);
        }

        private CheckBoxListModel GetCheckBoxListModel(UserSessionModel admin, List<PermissionListModel> permissions, CheckBoxListModel model)
        {
            model = model ?? new CheckBoxListModel();

            model.List = permissions.Select(p => new CheckBoxModel { Id = p.ReferenceId.ToString(), Name = p.Description }).ToList();

            // If a post back no need to look on DB
            if (model.PostedIds != null)
            {
                model.Selected = model.List.Where(l => model.PostedIds.Contains(l.Id)).ToList();
            }
            else
            {
                model.Selected = permissions.Where(p => p.IsSelected).Select(p => new CheckBoxModel { Id = p.ReferenceId.ToString(), Name = p.Description }).ToList();
            }

            model.PostedIds = (model == null) ? null : model.PostedIds;

            return model;
        }

        private EntityEnum GetEntityReferenceByPermissionType(PermissionTypeEnum type)
        {
            switch (type)
            {
                case PermissionTypeEnum.Brand:
                    return EntityEnum.Brand;
                case PermissionTypeEnum.CityArea:
                    return EntityEnum.CityArea;
                case PermissionTypeEnum.ProductFamily:
                    return EntityEnum.ProductFamily;
                case PermissionTypeEnum.SystemAccess:
                    return EntityEnum.SystemAccess;
                case PermissionTypeEnum.Tool:
                    return EntityEnum.Tool;
                default:
                    return EntityEnum.Unknown;
            }
        }
        private class DropDownItem
        {
            public bool Disabled { get; set; }
            public decimal ValueDecimal { get; set; }
            public int? RequirementLevel { get; set; }
            public long ValueLong { get; set; }
            public string Text { get; set; }
            public string Value { get; set; }
        }
    }
}