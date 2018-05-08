using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
//using EntityFramework;
//using EntityFramework.Extensions;
//using System.Web.Mvc;
using System.Web;
using System.IO;
//using NPOI.HSSF.UserModel;
//using NPOI.HPSF;
//using NPOI.SS.UserModel;
using DPO.Resources;
using DPO.Domain;
using DPO.Model.Light;
using DPO.Common;


namespace DPO.Services.Light
{
    public class ProjectServiceLight : BaseServices
    {
        public ProjectServiceLight() : base() { }

        //public ServiceResponse GetProjectModel(UserSessionModel userSessionModel, long? projectId)
        //{
        //    if (projectId.HasValue)
        //    {
        //        var query = from project in this.Context.Projects
        //                    where project.ProjectId == projectId
        //                    select new ProjectViewModel
        //                    {
        //                        ProjectId = project.ProjectId,
        //                        Name = project.Name,
        //                        ProjectDate = project.ProjectDate,
        //                        BidDate = project.BidDate,
        //                        EstimatedClose = project.EstimatedClose,
        //                        EstimatedDelivery = project.EstimatedDelivery,
        //                        ConstructionTypeId = (ConstructionTypeEnum)project.ConstructionTypeId,
        //                        ProjectStatusTypeId = project.ProjectStatusTypeId,
        //                        ProjectTypeId = (ProjectTypeEnum)project.ProjectTypeId,
        //                        ProjectOpenStatusTypeId = (ProjectOpenStatusTypeEnum)project.ProjectOpenStatusTypeId,
        //                        VerticalMarketTypeId = (VerticalMarketTypeEnum)project.VerticalMarketTypeId,
        //                        Description = project.Description
        //                    };

        //        var model = query.FirstOrDefault();

        //        this.Response.Model = model;
        //    }

        //    return this.Response;
        //}

        //public ServiceResponse GetProjectModelWithQuote(UserSessionModel userSessionModel, long? projectId, long? quoteId)
        //{
        //    if (projectId.HasValue && quoteId.HasValue)
        //    {
        //        var query = from project in this.Context.Projects
        //                    join quote in this.Context.Quotes on project.ProjectId equals quote.ProjectId
        //                    where project.ProjectId == projectId && quote.QuoteId == quoteId
        //                    select new ProjectViewModel
        //                    {
        //                        ProjectId = project.ProjectId,
        //                        Name = project.Name,
        //                        ProjectDate = project.ProjectDate,
        //                        BidDate = project.BidDate,
        //                        EstimatedClose = project.EstimatedClose,
        //                        EstimatedDelivery = project.EstimatedDelivery,
        //                        ConstructionTypeId = (ConstructionTypeEnum)project.ConstructionTypeId,
        //                        ProjectStatusTypeId = project.ProjectStatusTypeId,
        //                        ProjectTypeId = (ProjectTypeEnum)project.ProjectTypeId,
        //                        ProjectOpenStatusTypeId = (ProjectOpenStatusTypeEnum)project.ProjectOpenStatusTypeId,
        //                        VerticalMarketTypeId = (VerticalMarketTypeEnum)project.VerticalMarketTypeId,
        //                        Description = project.Description,
        //                        PricingTypeId = quote.IsCommission ? (byte)2 : (byte)1
        //                    };

        //        var model = query.FirstOrDefault();

        //        this.Response.Model = model;
        //    }

        //    return this.Response;
        //}

        public ServiceResponse GetProjectLocation(UserSessionModel userSessionModel, long? projectId)
        {
            if (projectId.HasValue)
            {
                var query = from project in this.Context.Projects
                            join address in this.Context.Addresses on project.ShipToAddressId equals address.AddressId
                            where project.ProjectId == projectId
                            select new ShipToAddressViewModel
                            {
                                AddressId = address.AddressId,
                                ShipToName = project.ShipToName,
                                AddressLine1 = address.AddressLine1,
                                AddressLine2 = address.AddressLine2,
                                AddressLine3 = address.AddressLine3,
                                Location = address.Location,
                                PostalCode = address.PostalCode,
                                StateId = address.StateId,
                                CountryCode = address.State.CountryCode,
                                SquareFootage = project.SquareFootage,
                                NumberOfFloor = project.NumberOfFloors
                            };

                var model = query.FirstOrDefault();

                this.Response.Model = model;
            }
            return this.Response;
        }

        public ServiceResponse GetSellerInfo(UserSessionModel userSessionModel, long? projectId)
        {
            if (projectId.HasValue)
            {
                var query = from project in this.Context.Projects
                            join address in this.Context.Addresses on project.SellerAddressId equals address.AddressId
                            where project.ProjectId == projectId
                            select new SellerInfoViewModel
                            {
                                AddressId = address.AddressId,
                                SellerName = project.SellerName,
                                AddressLine1 = address.AddressLine1,
                                AddressLine2 = address.AddressLine2,
                                AddressLine3 = address.AddressLine3,
                                Location = address.Location,
                                PostalCode = address.PostalCode,
                                StateId = address.StateId,
                                CountryCode = address.State.CountryCode
                            };

                var model = query.FirstOrDefault();

                this.Response.Model = model;
            }
            return this.Response;
        }

        public ServiceResponse GetDealerContractorInfo(UserSessionModel userSessionModel, long? projectId)
        {
            if (projectId.HasValue)
            {
                var query = from project in this.Context.Projects
                            join address in this.Context.Addresses on project.CustomerAddressId equals address.AddressId
                            where project.ProjectId == projectId
                            select new DealerContractorInfoViewModel
                            {
                                AddressId = address.AddressId,
                                DealerContractorName = project.DealerContractorName,
                                CustomerName = project.CustomerName,
                                AddressLine1 = address.AddressLine1,
                                AddressLine2 = address.AddressLine2,
                                AddressLine3 = address.AddressLine3,
                                Location = address.Location,
                                PostalCode = address.PostalCode,
                                StateId = address.StateId,
                                CountryCode = address.State.CountryCode
                            };

                var model = query.FirstOrDefault();

                this.Response.Model = model;
            }
            return this.Response;
        }

        public ServiceResponse HasOrder(UserSessionModel user, long? projectId)
        {
            var query = from order in this.Context.Orders
                        join quote in this.Context.Quotes on order.QuoteId equals quote.QuoteId
                        where quote.ProjectId == projectId
                        select order;
            var existedOrder = query.FirstOrDefault();

            bool HasOrder = false;
            if (existedOrder != null)
            {
                HasOrder = true;
            }
            this.Response.Model = HasOrder;
            return this.Response;
        }

    }
}
