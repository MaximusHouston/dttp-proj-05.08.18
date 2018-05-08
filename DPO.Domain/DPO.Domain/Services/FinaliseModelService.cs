using System;
using System.Collections.Generic;
using System.Linq;
using DPO.Common;
using DPO.Model.Light;
using StackExchange.Profiling;

namespace DPO.Domain 
{
    public class FinaliseModelService : BaseServices, IFinaliseModelService
    {
        public void FinaliseOrderModel(Messages messages, UserSessionModel admin, OrderViewModelLight model)
        {
            if (!string.IsNullOrEmpty(model.ProjectId.ToString()) &&
                !string.IsNullOrEmpty(model.QuoteId.ToString()))
            {
                var projectQuery = from project in this.Db.QueryProjectViewableByProjectId(admin, model.ProjectId)

                                   join quote in this.Db.Quotes on new { id = project.ProjectId, qId = model.QuoteId }
                                   equals new { id = quote.ProjectId, qId = quote.QuoteId } into Laq
                                   from quote in Laq.DefaultIfEmpty()
                                   select new ProjectModel
                                   {
                                       ProjectId = project.ProjectId,
                                       OwnerId = project.Owner.UserId,
                                       Name = project.Name,
                                       Description = project.Description,
                                       ProjectDate = project.ProjectDate,
                                       BidDate = project.BidDate,
                                       EstimatedClose = project.EstimatedClose,
                                       EstimatedDelivery = project.EstimatedDelivery,
                                       Expiration = project.Expiration,
                                       ProjectStatusTypeId = (byte)project.ProjectStatusTypeId,
                                       ProjectTypeId = project.ProjectTypeId,
                                       ProjectOpenStatusTypeId = project.ProjectOpenStatusTypeId,
                                       ConstructionTypeId = project.ConstructionTypeId,
                                       VerticalMarketTypeId = project.VerticalMarketTypeId,

                                       CustomerAddress = new AddressModel
                                       {
                                           AddressId = project.CustomerAddressId,
                                       },
                                       SellerAddress = new AddressModel
                                       {
                                           AddressId = project.SellerAddressId,
                                       },
                                       EngineerAddress = new AddressModel
                                       {
                                           AddressId = project.EngineerAddressId,
                                       },
                                       ShipToAddress = new AddressModel
                                       {
                                           AddressId = project.ShipToAddressId,
                                       },

                                       ActiveQuoteSummary = new QuoteListModel
                                       {
                                           ProjectId = project.ProjectId,
                                           QuoteId = (quote == null) ? 0 : quote.QuoteId,
                                           //ItemCount = (quote == null) ? 0 : quote.QuoteItems.Count(),
                                           Alert = (quote == null) ? false : quote.RecalculationRequired,
                                           Title = (quote == null) ? "" : quote.Title,
                                           Timestamp = (quote == null) ? (DateTime?)null : quote.Timestamp,
                                           TotalList = (quote == null) ? 0 : quote.TotalList,
                                           TotalListSplit = (quote == null) ? 0 : quote.TotalListSplit,
                                           TotalListVRV = (quote == null) ? 0 : quote.TotalListVRV,
                                           TotalMisc = (quote == null) ? 0 : quote.TotalMisc,
                                           TotalNet = (quote == null) ? 0 : quote.TotalNet,
                                           TotalSell = (quote == null) ? 0 : quote.TotalSell,
                                           TotalSellSplit = (quote == null) ? 0 : quote.TotalSellSplit,
                                           TotalSellVRV = (quote == null) ? 0 : quote.TotalSellVRV,
                                           TotalCountSplit = (quote == null) ? 0 : quote.TotalCountSplit,
                                           TotalCountVRV = (quote == null) ? 0 : quote.TotalCountVRV,
                                           TotalCountVRVIndoor = (quote == null) ? 0 : quote.TotalCountVRVIndoor,
                                           TotalCountVRVOutdoor = (quote == null) ? 0 : quote.TotalCountVRVOutdoor,
                                           ApprovedCommissionPercentage = (quote == null) ? 0 : quote.ApprovedCommissionPercentage,
                                           ApprovedDiscountPercentage = (quote == null) ? 0 : quote.ApprovedDiscountPercentage,
                                           ApprovedDiscountPercentageSplit = (quote == null) ? 0 : quote.ApprovedDiscountPercentageSplit,
                                           ApprovedDiscountPercentageVRV = (quote == null) ? 0 : quote.ApprovedDiscountPercentageVRV,
                                           TotalNetCommission = (quote == null) ? 0 : quote.TotalNetCommission,
                                           TotalNetNonCommission = (quote == null) ? 0 : quote.TotalNetNonCommission,
                                           TotalNetSplit = (quote == null) ? 0 : quote.TotalNetSplit,
                                           TotalNetVRV = (quote == null) ? 0 : quote.TotalNetVRV,
                                           IsGrossMargin = (quote == null) ? false : quote.IsGrossMargin,
                                           TotalFreight = (quote == null) ? 0 : quote.TotalFreight,
                                           DiscountPercentage = (quote == null) ? 0 : quote.DiscountPercentage,
                                           CommissionPercentage = (quote == null) ? 0 : quote.CommissionPercentage,
                                           Revision = (quote == null) ? 0 : quote.Revision
                                       },
                                       ConstructionTypeDescription = project.ConstructionType.Description,
                                       ProjectTypeDescription = project.ProjectType.Description,
                                       ProjectOpenStatusDescription = project.ProjectOpenStatusType.Description,
                                       ProjectStatusDescription = project.ProjectStatusType.Description,
                                       VerticalMarketDescription = project.VerticalMarketType.Description,
                                       Deleted = project.Deleted,
                                       Timestamp = project.Timestamp
                                   };

                var addressService = new AddressServices(this.Context);
            }

            if (model == null)
            {
                this.Response.AddError(Resources.DataMessages.DM007);
                return;
            }
        }

        public void FinaliseOrderModel(UserSessionModel admin, OrderViewModel model)
        {
            if (!string.IsNullOrEmpty(model.ProjectId.ToString()) &&
                !string.IsNullOrEmpty(model.QuoteId.ToString()))
            {
                var projectQuery = from project in this.Db.QueryProjectViewableByProjectId(admin, model.ProjectId)

                                   join quote in this.Db.Quotes on new { id = project.ProjectId, qId = model.QuoteId } equals new { id = quote.ProjectId, qId = quote.QuoteId } into Laq
                                   from quote in Laq.DefaultIfEmpty()

                                   select new ProjectModel
                                   {
                                       ProjectId = project.ProjectId,
                                       OwnerId = project.Owner.UserId,
                                       Name = project.Name,
                                       Description = project.Description,
                                       ProjectDate = project.ProjectDate,
                                       BidDate = project.BidDate,
                                       EstimatedClose = project.EstimatedClose,
                                       EstimatedDelivery = project.EstimatedDelivery,
                                       Expiration = project.Expiration,
                                       ProjectStatusTypeId = (byte)project.ProjectStatusTypeId,
                                       ProjectTypeId = project.ProjectTypeId,
                                       ProjectOpenStatusTypeId = project.ProjectOpenStatusTypeId,
                                       ConstructionTypeId = project.ConstructionTypeId,
                                       VerticalMarketTypeId = project.VerticalMarketTypeId,

                                       CustomerAddress = new AddressModel
                                       {
                                           AddressId = project.CustomerAddressId,
                                       },
                                       SellerAddress = new AddressModel
                                       {
                                           AddressId = project.SellerAddressId,
                                       },
                                       EngineerAddress = new AddressModel
                                       {
                                           AddressId = project.EngineerAddressId,
                                       },
                                       ShipToAddress = new AddressModel
                                       {
                                           AddressId = project.ShipToAddressId,
                                       },

                                       ActiveQuoteSummary = new QuoteListModel
                                       {
                                           ProjectId = project.ProjectId,
                                           QuoteId = (quote == null) ? 0 : quote.QuoteId,
                                           //ItemCount = (quote == null) ? 0 : quote.QuoteItems.Count(),
                                           Alert = (quote == null) ? false : quote.RecalculationRequired,
                                           Title = (quote == null) ? "" : quote.Title,
                                           Timestamp = (quote == null) ? (DateTime?)null : quote.Timestamp,
                                           TotalList = (quote == null) ? 0 : quote.TotalList,
                                           TotalListSplit = (quote == null) ? 0 : quote.TotalListSplit,
                                           TotalListVRV = (quote == null) ? 0 : quote.TotalListVRV,
                                           TotalMisc = (quote == null) ? 0 : quote.TotalMisc,
                                           TotalNet = (quote == null) ? 0 : quote.TotalNet,
                                           TotalSell = (quote == null) ? 0 : quote.TotalSell,
                                           TotalSellSplit = (quote == null) ? 0 : quote.TotalSellSplit,
                                           TotalSellVRV = (quote == null) ? 0 : quote.TotalSellVRV,
                                           TotalCountSplit = (quote == null) ? 0 : quote.TotalCountSplit,
                                           TotalCountVRV = (quote == null) ? 0 : quote.TotalCountVRV,
                                           TotalCountVRVIndoor = (quote == null) ? 0 : quote.TotalCountVRVIndoor,
                                           TotalCountVRVOutdoor = (quote == null) ? 0 : quote.TotalCountVRVOutdoor,
                                           ApprovedCommissionPercentage = (quote == null) ? 0 : quote.ApprovedCommissionPercentage,
                                           ApprovedDiscountPercentage = (quote == null) ? 0 : quote.ApprovedDiscountPercentage,
                                           ApprovedDiscountPercentageSplit = (quote == null) ? 0 : quote.ApprovedDiscountPercentageSplit,
                                           ApprovedDiscountPercentageVRV = (quote == null) ? 0 : quote.ApprovedDiscountPercentageVRV,
                                           TotalNetCommission = (quote == null) ? 0 : quote.TotalNetCommission,
                                           TotalNetNonCommission = (quote == null) ? 0 : quote.TotalNetNonCommission,
                                           TotalNetSplit = (quote == null) ? 0 : quote.TotalNetSplit,
                                           TotalNetVRV = (quote == null) ? 0 : quote.TotalNetVRV,
                                           IsGrossMargin = (quote == null) ? false : quote.IsGrossMargin,
                                           TotalFreight = (quote == null) ? 0 : quote.TotalFreight,
                                           DiscountPercentage = (quote == null) ? 0 : quote.DiscountPercentage,
                                           CommissionPercentage = (quote == null) ? 0 : quote.CommissionPercentage,
                                           Revision = (quote == null) ? 0 : quote.Revision
                                       },
                                       ConstructionTypeDescription = project.ConstructionType.Description,
                                       ProjectTypeDescription = project.ProjectType.Description,
                                       ProjectOpenStatusDescription = project.ProjectOpenStatusType.Description,
                                       ProjectStatusDescription = project.ProjectStatusType.Description,
                                       VerticalMarketDescription = project.VerticalMarketType.Description,
                                       Deleted = project.Deleted,
                                       Timestamp = project.Timestamp
                                   };

                model.Project = projectQuery.FirstOrDefault();

                var addressService = new AddressServices(this.Context);
                model.Project.SellerAddress = addressService.GetAddressModel(admin, model.Project.SellerAddress);
                model.Project.CustomerAddress = addressService.GetAddressModel(admin, model.Project.CustomerAddress);
                model.Project.EngineerAddress = addressService.GetAddressModel(admin, model.Project.EngineerAddress);
                model.Project.ShipToAddress = addressService.GetAddressModel(admin, model.Project.ShipToAddress);

                //model.QuoteItems = new QuoteServices(this.Context).GetQuoteItemListModel(admin, (long)model.QuoteId).Model as List<QuoteItemListModel>;
                model.QuoteItems = new QuoteServices(this.Context).GetQuoteItems(admin, (long)model.QuoteId).Model as List<QuoteItemModel>;

            }

            if (model == null)
            {
                this.Response.AddError(Resources.DataMessages.DM007);
                return;
            }

            #region commented
            //var service = new QuoteServices();

            //if (model.ApprovedDiscount > 0)
            //{
            //    model.ApprovedTotals = service.CalculateTotalDiscountsApproved(model.Quote, model.ApprovedDiscount,
            //        model.ApprovedDiscountSplit, model.ApprovedDiscountVRV, model.RequestedCommission);
            //}
            //else
            //{
            //    model.ApprovedTotals = service.CalculateTotalDiscountsApproved(model.Quote, model.RequestedDiscount,
            //        model.RequestedDiscountSplit, model.RequestedDiscountVRV, model.RequestedCommission);
            //}

            //model.StandardTotals = service.CalculateTotalStandard(model.Quote);
            //model.RequestedDiscount *= 100M;
            //model.RequestedDiscountSplit *= 100M;
            //model.RequestedDiscountVRV *= 100M;
            //model.ApprovedDiscount *= 100M;
            //model.ApprovedDiscountSplit *= 100M;
            //model.ApprovedDiscountVRV *= 100M;
            //model.RequestedCommission *= 100;

            // Dropdowns 
            //model.DiscountRequestStatusTypes = htmlService.DropDownModelDiscountRequestStatusTypes((model == null) ? null : model.DiscountRequestStatusTypeId);
            //model.SystemBasisDesignTypes = htmlService.DropDownModelSystemBasisDesignTypes((model == null) ? null : model.SystemBasisDesignTypeId);
            //model.ZoneStrategyTypes = htmlService.DropDownModelZoneStrategyTypes((model == null) ? null : model.ZoneStrategyTypeId);
            //model.BrandSpecifiedTypes = htmlService.DropDownModelBrandCompetitorTypes((model == null) ? null : model.BrandSpecifiedTypeId);
            //model.BrandApprovedTypes = htmlService.DropDownModelBrandCompetitorTypes((model == null) ? null : model.BrandApprovedTypeId);
            //model.DaikinEquipmentAtAdvantageTypes = htmlService.DropDownModelDaikinEquipmentAtAdvantageTypes((model == null) ? null : model.DaikinEquipmentAtAdvantageTypeId);
            //model.ProbabilityOfCloseTypes = htmlService.DropDownModelProbabilityOfCloseTypes((model == null) ? null : model.ProbabilityOfCloseTypeId);
            #endregion commented
        }

        //public void FinaliseProjectModel(Messages messages, UserSessionModel admin, ProjectModel model)
        //{
        //    //mass upload change - had to turn these off
        //    var service = new HtmlServices(this.Context);

        //    new AddressServices(this.Context).FinaliseModel(model.CustomerAddress);
        //    new AddressServices(this.Context).FinaliseModel(model.EngineerAddress);
        //    new AddressServices(this.Context).FinaliseModel(model.SellerAddress);
        //    new AddressServices(this.Context).FinaliseModel(model.ShipToAddress);

        //    model.ConstructionTypes = service.DropDownModelConstructionTypes((model == null) ? null : model.ConstructionTypeId);

        //    model.ProjectLeadStatusTypes = service.DropDownModelProjectLeadStatusTypes((model == null) ? null : model.ProjectLeadStatusTypeId);

        //    model.ProjectStatusTypes = service.DropDownModelProjectStatuses((model == null) ? null : model.ProjectStatusTypeId, this.DropDownMode);

        //    model.ProjectTypes = service.DropDownModelProjectTypes((model == null) ? null : model.ProjectTypeId);

        //    model.ProjectOpenStatusTypes = service.DropDownModelProjectOpenTypes((model == null) ? null : model.ProjectOpenStatusTypeId);

        //    model.VerticalMarketTypes = service.DropDownModelVerticalMarkets((model == null) ? null : model.VerticalMarketTypeId);
        //}

        //public void FinaliseProjectModel(Messages messages, UserSessionModel user, ProjectsModel model, HtmlServices htmlService)
        //{
        //    var profiler = MiniProfiler.Current;

        //    using (profiler.Step("Finalise ProjectsModel"))
        //    {
        //        var hasCommission = user.HasAccess(SystemAccessEnum.RequestCommission) ||
        //               user.HasAccess(SystemAccessEnum.ApprovedRequestCommission) ||
        //               user.HasAccess(SystemAccessEnum.ViewRequestedCommission);

        //        var hasDiscountRequest = user.HasAccess(SystemAccessEnum.RequestDiscounts) ||
        //            user.HasAccess(SystemAccessEnum.ApproveDiscounts) ||
        //            user.HasAccess(SystemAccessEnum.ViewDiscountRequest);

        //        model.ProjectOpenStatusTypes = htmlService.DropDownModelProjectOpenTypes((model == null) ? null : model.ProjectOpenStatusTypeId);

        //        model.ProjectStatusTypes = htmlService.DropDownModelProjectStatuses((model == null) ? null : model.ProjectStatusTypeId, DropDownMode.Filtering);

        //        model.ProjectLeadStatusTypes = htmlService.DropDownModelProjectLeadStatusTypes((model == null) ? null : model.ProjectLeadStatusTypeId);

        //        if (hasDiscountRequest || hasCommission)
        //        {
        //            model.ProjectDarComTypes = htmlService.DropDownModelProjectDarComStatusTypes(user, (model == null) ? null : model.ProjectDarComStatusTypeId);
        //        }

        //        model.ProjectTypes = htmlService.DropDownModelProjectTypes(null);

        //        model.UsersInGroup = htmlService.DropDownModelUsersInGroup(user, (model == null) ? null : model.UserId);

        //        model.BusinessesInGroup = htmlService.DropDownModelBusineesForProjects(user, (model == null) ? null : model.BusinessId);

        //        model.ProjectExportTypes = htmlService.DropDownModelProjectExportTypes(null);

        //        var projectServices = new ProjectServices();
        //        model.ProjectDateTypes = htmlService.DropDownDateTypes(projectServices.GetProjectDateTypes(), model.DateTypeId);
        //    }
        //}

        //public void FinaliseProjectModel(Messages messages, UserSessionModel user, ProjectsGridViewModel model, HtmlServices htmlService)
        //{
        //    var profiler = MiniProfiler.Current;
        //    using (profiler.Step("Finalise ProjectsGridViewModel"))
        //    {
        //        var hasCommission = user.HasAccess(SystemAccessEnum.RequestCommission) ||
        //               user.HasAccess(SystemAccessEnum.ApprovedRequestCommission) ||
        //               user.HasAccess(SystemAccessEnum.ViewRequestedCommission);

        //        var hasDiscountRequest = user.HasAccess(SystemAccessEnum.RequestDiscounts) ||
        //            user.HasAccess(SystemAccessEnum.ApproveDiscounts) ||
        //            user.HasAccess(SystemAccessEnum.ViewDiscountRequest);

        //        model.ProjectOpenStatusTypes = htmlService.DropDownModelProjectOpenTypes((model == null) ? null : model.ProjectOpenStatusTypeId);
        //        model.ProjectStatusTypes = htmlService.DropDownModelProjectStatuses((model == null) ? null : model.ProjectStatusTypeId, DropDownMode.Filtering);
        //        model.ProjectLeadStatusTypes = htmlService.DropDownModelProjectLeadStatusTypes((model == null) ? null : model.ProjectLeadStatusTypeId);

        //        if (hasDiscountRequest || hasCommission)
        //        {
        //            model.ProjectDarComTypes = htmlService.DropDownModelProjectDarComStatusTypes(user, (model == null) ? null : model.ProjectDarComStatusTypeId);
        //        }

        //        model.ProjectTypes = htmlService.DropDownModelProjectTypes(null);
        //        model.UsersInGroup = htmlService.DropDownModelUsersInGroup(user, (model == null) ? null : model.UserId);
        //        model.BusinessesInGroup = htmlService.DropDownModelBusineesForProjects(user, (model == null) ? null : model.BusinessId);
        //        model.ProjectExportTypes = htmlService.DropDownModelProjectExportTypes(null);
        //        var projectServices = new ProjectServices();
        //        model.ProjectDateTypes = htmlService.DropDownDateTypes(projectServices.GetProjectDateTypes(), model.DateTypeId);
        //    }
        //}
    }
}
