//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================

using CsvHelper;
using DPO.Common;
using DPO.Data;
using DPO.Resources;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Linq.Dynamic;
using Newtonsoft.Json;

namespace DPO.Domain
{
    /// <summary>
    /// Quote services. Provide functional support for quote management
    /// </summary>
    public partial class OverviewServices : BaseServices
    {
        public List<WidgetData> Data = new List<WidgetData>();
        private ProjectServices projectService = null;
        private HtmlServices htmlService = null;
        private ConversionServices conversionService = null;

        public OverviewServices()
            : base()
        {
            this.projectService = new ProjectServices();
            this.htmlService = new HtmlServices();
            this.conversionService = new ConversionServices();
        }

        public OverviewServices(DPOContext context)
            : base(context)
        {
            this.projectService = new ProjectServices(context);
            this.htmlService = new HtmlServices(context);
            this.conversionService = new ConversionServices(context);
        }

        public string getCacheKey(string currentUserId, string templateId , WidgetContainerModel container) {

            string key = "";

            OverViewCacheModel model = new OverViewCacheModel() {
                CurrentUserId = currentUserId,
                TemplateId = templateId,
                filter = new OverviewFilter() {
                    UserId = container.UserId,
                    BussinessId = container.BusinessId,
                    ProjectStatusTypeId = container.ProjectStatusTypeId,
                    ProjectOpenStatusTypeId = container.ProjectOpenStatusTypeId,
                    Year = container.Year
                }
            };

            key = JsonConvert.SerializeObject(model);

            return key;
        }

        public string getDefaultFilter()
        {
            string key = "";
            OverviewFilter model = new OverviewFilter();
            key = JsonConvert.SerializeObject(model);

            return key;
        }

        public string getCurrentFilter(WidgetContainerModel container)
        {
            string key = "";
            OverviewFilter model = new OverviewFilter()
            {
                UserId = container.UserId,
                BussinessId = container.BusinessId,
                ProjectStatusTypeId = container.ProjectStatusTypeId,
                ProjectOpenStatusTypeId = container.ProjectOpenStatusTypeId,
                Year = container.Year
            };

            key = JsonConvert.SerializeObject(model);

            return key;
        }

        public ServiceResponse GetOverviewSearchModel(UserSessionModel user, WidgetContainerModel model)
        {
            this.Response.Model = model;
            model.PageSize = null;
            model.ReturnTotals = false;

            this.FinaliseModel(this.Response.Messages, user, model);

            return this.Response;
        }

        public void FinaliseModel(Messages messages, UserSessionModel user, WidgetContainerModel model)
        {
            model.ProjectOpenStatusTypes = htmlService.DropDownModelProjectOpenTypes((model == null) ? null : model.ProjectOpenStatusTypeId);

            model.ProjectStatusTypes = htmlService.DropDownModelProjectStatuses((model == null) ? null : model.ProjectStatusTypeId, DropDownMode.Filtering);

            model.ProjectTypes = htmlService.DropDownModelProjectTypes(null);

            model.UsersInGroup = htmlService.DropDownModelUsersInGroup(user,
                (model == null && !String.IsNullOrEmpty(model.UserId)) ? null : CheckAndConvert(model.UserId));

            model.BusinessesInGroup = htmlService.DropDownModelBusinesses(user,
                (model == null && !String.IsNullOrEmpty(model.BusinessId)) ? null : CheckAndConvert(model.BusinessId));

            model.ProjectExportTypes = htmlService.DropDownModelProjectExportTypes(null);

            model.FinancialYears = htmlService.DropDownFinancialYears(user, (model == null) ? null : model.Year);

            model.ProjectDateTypes = htmlService.DropDownDateTypes(projectService.GetProjectDateTypes(), model.DateTypeId);
        }

        private long? CheckAndConvert(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return null;
            }

            long val;

            if (long.TryParse(value, out val))
            {
                return val;
            }

            return null;
        }

        public ServiceResponse GetActiveProjects(UserSessionModel user, ProjectsModel model)
        {
            var fromDate = GetFromDate(model, true);
            var toDate = GetToDate(model, true);

            var query = GetOverviewProjectQuery(user, model)
                .Where(w => w.Timestamp >= fromDate && w.Timestamp <= toDate);

            Data.Add(new WidgetData { Key = ResourceUI.OverviewActiveProjectsYTD, Value = query.Count() });

            this.Response.Model = Data;

            return this.Response;
        }

        public List<WidgetModel> GetAvailableWidgetTypes()
        {
            var additionalSettings = new Dictionary<string, WidgetSetting>();

            WidgetSetting setting = new WidgetSetting
            {
                SettingName = "AvailableCharts",
                SettingLabel = "Available Charts",
                SelectedOption = "OutdoorUnitCount",
                Options = new List<KeyValuePair<string, string>>()
            };

            setting.Options.AddRange(
                 new KeyValuePair<string, string>[]
                    {
                        new KeyValuePair<string, string>("ODUCount", "Outdoor Unit Count"),
                        new KeyValuePair<string, string>("TotalNetValue", "Total Net Value")
                    }
            );

            setting.SelectedOption = "OutdoorUnitCount";

            additionalSettings.Add("ChartOptions", setting);

            return new List<WidgetModel>
            {
                new WidgetModel{
                    Title = "Active Projects",
                    TemplateId = "ActiveProjectsTemplate"
                },
                new WidgetModel{
                    Title = "Open Projects",
                    TemplateId = "OpenProjectsTemplate"
                },
                new WidgetModel{
                    Title = "New Registrations",
                    TemplateId = "NewRegistrationsTemplate"
                },
                new WidgetModel{
                    Title = "Alerts",
                    TemplateId = "ProjectAlertsTemplate"
                },
                new WidgetModel{
                    Title = "Open Project Types",
                    TemplateId = "OpenProjectTypesTemplate"
                },
                new WidgetModel{
                    Title = "Vertical Markets",
                    TemplateId = "VerticalMarketsTemplate"
                },
                new WidgetModel{
                    Title = "Won Projects",
                    TemplateId = "WonProjectsTemplate",
                    AdditionalSettings = additionalSettings
                },
                new WidgetModel{
                    Title = "Lost Projects",
                    TemplateId = "LostProjectsTemplate",
                    AdditionalSettings = additionalSettings
                },
                new WidgetModel{
                    Title = "New Projects",
                    TemplateId = "NewProjectsTemplate",
                    AdditionalSettings = additionalSettings
                },
                new WidgetModel{
                    Title = "New Tile",
                    TemplateId = "EmptyWidgetTemplate"
                }
            };
        }

        /// <summary>
        /// Returns the start of the fiscal year based on the model search year
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public DateTime GetFromDate(ProjectsModel model, bool isFiscal)
        {
            int year = DateTime.Now.Year;

            if (model == null)
            {
                model = new ProjectsModel();
            }

            if (model.Year == null)
            {
                if (isFiscal)
                {
                    model.Year = this.conversionService.ToFiscal(DateTime.Now).Year;
                }
                else
                {
                    model.Year = DateTime.Now.Year;
                }
            }

            return new DateTime((int)model.Year, 4, 1);
        }

        public DateTime GetToDate(ProjectsModel model, bool isFiscal)
        {
            int year = DateTime.Now.Year;

            if (model == null)
            {
                model = new ProjectsModel();
            }

            if (model.Year == null)
            {
                if (isFiscal)
                {
                    model.Year = this.conversionService.ToFiscal(DateTime.Now).Year;
                }
                else
                {
                    model.Year = DateTime.Now.Year;
                }
            }

            return new DateTime((int)model.Year + 1, 3, 1);
        }

        public ServiceResponse GetLostProjects(UserSessionModel user, ProjectsModel model)
        {
            var fromDate = GetFromDate(model, true);
            var toDate = GetToDate(model, true);

            var query = GetOverviewProjectQuery(user, model)
                        .Where(p => p.Timestamp >= fromDate && p.Timestamp <= toDate)
                        .Where(p => p.ProjectStatusTypeId == ProjectStatusTypeEnum.ClosedLost);

            var result = GetAggregatesForUpdateDate(query, user);

            return CalendarMonthsWidget(result, fromDate, 12, user);
        }

        public ServiceResponse GetPendingProjects(UserSessionModel user, ProjectsModel model)
        {
            var fromDate = GetFromDate(model, true);
            var toDate = GetToDate(model, true);

            var query = GetOverviewProjectQuery(user, model)
                        .Where(p => p.Timestamp >= fromDate && p.Timestamp <= toDate);
            //.Where( p => p.ProjectStatusTypeId == ProjectStatusTypeEnum.pen)

            var result = GetAggregatesForProjectDate(query, user);
            return CalendarMonthsWidget(result, fromDate, 12, user);
        }

        public ServiceResponse GetNewProjects(UserSessionModel user, ProjectsModel model)
        {
            var fromDate = GetFromDate(model, true);
            var toDate = GetToDate(model, true);

            var query = GetOverviewProjectQuery(user, model)
                        .Where(p => p.ProjectDate >= fromDate && p.ProjectDate <= toDate);

            var result = GetAggregatesForProjectDate(query, user);

            return CalendarMonthsWidget(result, fromDate, 12, user);
        }

        public ServiceResponse GetNewRegistrations(UserSessionModel user, ProjectsModel model)
        {
            var fromDate = GetFromDate(model, true);
            var toDate = GetToDate(model, true);

            var query = this.Db.QueryUsersViewableByUser(user, true)
                        .Where(u => u.RegisteredOn >= fromDate && u.RegisteredOn <= toDate);

            Data.Add(new WidgetData { Key = ResourceUI.OverviewNewRegistrationsYTD, Value = query.Count() });

            this.Response.Model = Data;

            return this.Response;
        }

        public ServiceResponse GetOpenProjects(UserSessionModel user, ProjectsModel model)
        {
            var fromDate = GetFromDate(model, true);
            var toDate = GetToDate(model, true);

            var query = GetOverviewProjectQuery(user, model)
                        .Where(q => q.ProjectStatusTypeId == ProjectStatusTypeEnum.Open
                            && q.ProjectDate >= fromDate && q.ProjectDate <= toDate);

            Data.Add(new WidgetData { Key = ResourceUI.OverviewOpenProjects, Value = query.Count() });

            this.Response.Model = Data;

            return this.Response;
        }

        public IQueryable<Project> GetOverviewProjectQuery(UserSessionModel user, ProjectsModel model)
        {
            return this.Db.QueryProjectsViewableBySearch(user, model);
        }

        public ServiceResponse GetProjectAlerts(UserSessionModel user, int NumberOfDays, ProjectsModel model)
        {
            var date = DateTime.Today.AddDays(NumberOfDays);

            var results = GetOverviewProjectQuery(user, model)
                        .Where(p => p.ProjectStatusTypeId == ProjectStatusTypeEnum.Open
                            && p.Expiration < date).ToList();

            foreach (var project in results)
            {
                var days = new TimeSpan(project.Expiration.Ticks - DateTime.Today.Ticks).TotalDays;
                if (days == 0)
                {
                    Data.Add(new WidgetData { Key = string.Format(ResourceUI.OverviewProjectExpiresToday, project.Name), Value = 0 });
                }
                else if (days < 0)
                {
                    Data.Add(new WidgetData { Key = string.Format(ResourceUI.OverviewProjectExpired, project.Name), Value = 0 });
                }
                else
                {
                    Data.Add(new WidgetData { Key = string.Format(ResourceUI.OverviewProjectExpire, project.Name, days), Value = 0 });
                }
            }

            this.Response.Model = Data;

            return this.Response;
        }

        public ServiceResponse GetProjectTypes(UserSessionModel user, ProjectsModel model)
        {
            var fromDate = GetFromDate(model, true);
            var toDate = GetToDate(model, true);

            var results = GetOverviewProjectQuery(user, model)
                        .Where(q => q.ProjectDate >= fromDate && q.ProjectDate <= toDate)
                        .Include(p => p.ProjectType)
                        .GroupBy(p => p.ProjectType.Description)
                        .Select(p => new WidgetData { Key = p.Key, Value = p.Count() })
                        .ToList();

            return PercentageListWidget(results);
        }

        public ServiceResponse GetVerticalMarket(UserSessionModel user, ProjectsModel model)
        {
            var fromDate = GetFromDate(model, true);
            var toDate = GetToDate(model, true);

            var results = GetOverviewProjectQuery(user, model)
                        .Where(q => q.ProjectDate >= fromDate && q.ProjectDate <= toDate)
                        .Include(p => p.VerticalMarketType)
                        .GroupBy(p => p.VerticalMarketType.Description)
                        .Select(p => new WidgetData { Key = p.Key, Value = p.Count() })
                        .ToList();

            return PercentageListWidget(results);
        }

        public ServiceResponse GetWonProjects(UserSessionModel user, ProjectsModel model)
        {
            var fromDate = GetFromDate(model, true);

            var projectQuery = GetOverviewProjectQuery(user, model)
                        .Where(p => p.ProjectStatusTypeId == ProjectStatusTypeEnum.ClosedWon
                            && p.Timestamp >= fromDate);

            var aggQuery = GetAggregatesForUpdateDate(projectQuery, user);

            return CalendarMonthsWidget(aggQuery, fromDate, 12, user);
        }

        private IQueryable<ProjectListModel> GetAggregatesForProjectDate(IQueryable<Project> query, UserSessionModel user)
        {
            // HACK:  Probably should use the Dynamic LINQ library....   Woaaaah ugly

            return query.Select(p =>
                new
                {
                    ProjectDate = p.ProjectDate,
                    TotalNet = p.Quotes.Where(q => q.Active).Sum(s => s.TotalNet),
                    ActiveQuoteItems = p.Quotes
                        .Where(q => q.Active).FirstOrDefault().QuoteItems.Where(w => w.Product != null
                            && w.Product.ProductModelTypeId == (int)ProductModelTypeEnum.Outdoor
                            && w.Product.ProductFamilyId == (int)ProductFamilyEnum.VRV)
                        .Select(s =>
                            new
                            {
                                QuoteItemQuantity = (decimal?)(s.Quantity),
                                QuoteItemProduct = s.Product,
                                OutdoorAccessoryQuantity = (decimal?)s.Product.ParentProductAccessories
                                    .Where(a => a.Product.ProductModelTypeId == (int)ProductModelTypeEnum.Outdoor)
                                    .Sum(sumAcc => sumAcc.Quantity)
                            }
                        )
                })
            .Select(p =>
                new ProjectListModel
                {
                    ProjectDate = p.ProjectDate,
                    TotalNet = p.TotalNet,
                    ActiveQuoteSummary = new QuoteListModel
                    {
                        TotalVRVODUCount = (decimal?)
                            p.ActiveQuoteItems
                                .Where(w => w.OutdoorAccessoryQuantity == null
                                        || w.OutdoorAccessoryQuantity.Value == 0)
                                .Sum(sumOD =>
                                    sumOD.QuoteItemQuantity
                                ),
                        TotalVRVODUAccessoryCount = (decimal?)
                                p.ActiveQuoteItems.Sum(sumOD => sumOD.QuoteItemQuantity * sumOD.OutdoorAccessoryQuantity)
                    }
                });

        }

        private IQueryable<ProjectListModel> GetAggregatesForUpdateDate(IQueryable<Project> query, UserSessionModel user)
        {

            // HACK:  Probably should use the Dynamic LINQ library....   Woaaaah ugly

            return query.Select(p =>
                new
                {
                    ProjectDate = p.Timestamp,
                    TotalNet = p.Quotes.Where(q => q.Active).Sum(s => s.TotalNet),
                    ActiveQuoteItems = p.Quotes
                        .Where(q => q.Active).FirstOrDefault().QuoteItems.Where(w => w.Product != null
                            && w.Product.ProductModelTypeId == (int)ProductModelTypeEnum.Outdoor
                            && w.Product.ProductFamilyId == (int)ProductFamilyEnum.VRV)
                        .Select(s =>
                            new
                            {
                                QuoteItemQuantity = (decimal?)(s.Quantity),
                                QuoteItemProduct = s.Product,
                                OutdoorAccessoryQuantity = (decimal?)s.Product.ParentProductAccessories
                                    .Where(a => a.Product.ProductModelTypeId == (int)ProductModelTypeEnum.Outdoor)
                                    .Sum(sumAcc => sumAcc.Quantity)
                            }
                        )
                })
            .Select(p =>
                new ProjectListModel
                {
                    ProjectDate = p.ProjectDate,
                    TotalNet = p.TotalNet,
                    ActiveQuoteSummary = new QuoteListModel
                    {
                        TotalVRVODUCount = (decimal?)
                            p.ActiveQuoteItems
                                .Where(w => w.OutdoorAccessoryQuantity == null
                                        || w.OutdoorAccessoryQuantity.Value == 0)
                                .Sum(sumOD =>
                                    sumOD.QuoteItemQuantity
                                ),
                        TotalVRVODUAccessoryCount = (decimal?)
                                p.ActiveQuoteItems.Sum(sumOD => sumOD.QuoteItemQuantity * sumOD.OutdoorAccessoryQuantity)
                    }
                });
        }
        private ServiceResponse CalendarMonthsWidget(IEnumerable<ProjectListModel> dates, DateTime fromDate, int numberOfMonths, UserSessionModel user)
        {
            var results = dates
                            .GroupBy(p => p.ProjectDate.Year * 100 + p.ProjectDate.Month)
                            .Select(p =>
                                new
                                {
                                    Month = p.Key,
                                    ProjectCount = (int?)p.Count(),
                                    ODUCount = p.Sum(s => s.ActiveQuoteSummary.TotalVRVODUCount ?? 0)
                                        + p.Sum(s => s.ActiveQuoteSummary.TotalVRVODUAccessoryCount ?? 0),
                                    TotalNet = p.Sum(s => s.TotalNet ?? 0)
                                })
                            .ToList();

            var wData = new Dictionary<string, List<WidgetData>>();
            wData.Add("ProjectCount", new List<WidgetData>(20));
            wData.Add("ODUCount", new List<WidgetData>(20));
            wData.Add("TotalNetValue", new List<WidgetData>(20));

            for (int i = 0; i < numberOfMonths; i++)
            {
                var date = fromDate.AddMonths(i);

                var sumItems = results
                    .Where(m => m.Month == date.Year * 100 + date.Month)
                    .Select(v => new { v.ProjectCount, v.ODUCount, v.TotalNet }).FirstOrDefault();

                var projectCount = 0;
                var oduCount = 0;
                var totalNet = 0.0M;

                if (sumItems != null)
                {
                    projectCount = sumItems.ProjectCount ?? 0;
                    oduCount = (int)sumItems.ODUCount;
                    totalNet = sumItems.TotalNet;
                }

                wData["ProjectCount"].Add(new WidgetData { Key = date.ToString("MMM"), Value = projectCount });
                wData["ODUCount"].Add(new WidgetData { Key = date.ToString("MMM"), Value = oduCount });

                if (user.ShowPrices)
                {
                    wData["TotalNetValue"].Add(new WidgetData { Key = date.ToString("MMM"), Value = totalNet });
                }
            }

            this.Response.Model = wData;

            return this.Response;
        }

        private ServiceResponse PercentageListWidget(List<WidgetData> data)
        {

            var total = data.Sum(p => p.Value);

            foreach (var status in data.OrderByDescending(p => p.Value))
            {
                Data.Add(new WidgetData { Key = status.Key, Value = status.Value / total * 100M });
            }

            this.Response.Model = Data;

            return this.Response;
        }
    }

}