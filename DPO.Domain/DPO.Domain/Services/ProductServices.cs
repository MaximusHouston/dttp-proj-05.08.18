
using DPO.Common;
using DPO.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using EO.Pdf;
using System.Drawing;
using System.Net;
using System.Diagnostics;
//using System.Globalization;

namespace DPO.Domain
{

    public partial class ProductServices : BaseServices
    {
        public ProductServices() : base() { }

        public ProductServices(DPOContext context) : base(context) { }

        public string GenerateSubmittalDataFileForPackage(string productNumber, long? quoteItemId, long? projectId)
        {
            if (string.IsNullOrEmpty(productNumber)) return null;
            if (quoteItemId == null) return null;

            var controller = string.Format("{0}/{1}", Utilities.DocumentServerURL(), "ProductDashboard");

            var web = new WebClientLocal(HttpContext.Current);

            var url = string.Format("{0}/{1}?ProductNumber={2}&QuoteItemId={3}&ProjectId={4}&PdfMode=true", controller, "SubmittalTemplate", productNumber, quoteItemId, projectId);

            var html = web.DownloadString(url);

            return html;
        }

        // Documents always at product level never sub products
        public void GetDocuments(List<ProductModel> products)
        {
            var range = products.ToList(); // Create a new list to prevent update of products list

            //// If it is a system then get the sub product specs
            //range.Where(s => s.IsSystem && s.SubProducts != null)
            //    .ToList()
            //    .ForEach(p => range.AddRange(p.SubProducts));

            var ids = products.Select(i => i.ProductId.Value).Distinct().ToArray();

            var lookup = Db.GetProductDocuments(ids);

            // Find list of specs for each product
            range.ForEach(i =>
            {
                if (i.LineItemTypeId != (byte)LineItemTypeEnum.Configured)
                {
                    List<DocumentModel> docs;
                    if (lookup.TryGetValue(i.ProductId.Value, out docs))
                    {
                        i.Documents = docs.ToList();

                        //i.Documents = new List<DocumentModel>(docs.ToList());

                        //var documents = new List<DocumentModel>(docs.ToList());
                        //i.Documents = documents;



                        //if (i.LineItemTypeId == (byte)LineItemTypeEnum.Configured)
                        //{
                        //    //Get LCST Submittal Data
                        //    //DocumentModel LCSTSubmittal = GetLCSTSubmittal((long)i.QuoteId,i.CodeString);
                        //    //String LCSTSubmittalURL = GetLCSTSubmittal((long)i.QuoteId, i.CodeString);


                        //    for (int k = 0; k < i.Documents.Count; k++)
                        //    {
                        //        if (i.Documents[k].DocumentTypeId == (int)DocumentTypeEnum.SubmittalData)
                        //        {
                        //            i.Documents[k].FileName = i.CodeString;
                        //            //i.Documents[k] = LCSTSubmittal;

                        //            //i.Documents[k].isLCSTSubmittal = true;
                        //            //i.Documents[k].URL = LCSTSubmittalURL;
                        //            //i.Documents[k].LCSTSubmittalURL = LCSTSubmittalURL;

                        //        }
                        //    }

                        //}
                    }
                    else
                    {
                        i.Documents = new List<DocumentModel>();
                    }
                }
                else {
                    var LCSTSubmitall = LCSTSubmittalData(i.CodeString);
                    i.Documents = new List<DocumentModel>();
                    i.Documents.Add(LCSTSubmitall);
                }

                //List<DocumentModel> docs;
                //if (lookup.TryGetValue(i.ProductId.Value, out docs))
                //{
                //    i.Documents = docs.ToList();
                //}
                //else
                //{
                //    i.Documents = new List<DocumentModel>();
                //}

            });


            //range.ForEach(i =>
            //{
            //    if (i.LineItemTypeId == (byte)LineItemTypeEnum.Configured)
            //    {
            //        for (int k = 0; k < i.Documents.Count; k++)
            //        {
            //            if (i.Documents[k].DocumentTypeId == (int)DocumentTypeEnum.SubmittalData)
            //            {
            //                i.Documents[k].FileName = i.CodeString;

            //            }
            //        }

            //    }
            //});

            //for (int i = 0; i < range.Count; i++)
            //{
            //    if (range[i].LineItemTypeId == (byte)LineItemTypeEnum.Configured)
            //    {
            //        //for (int j = 0; j < range[i].Documents.Count; j++)
            //        //{
            //        //    if (range[i].Documents[j].DocumentTypeId == (int)DocumentTypeEnum.SubmittalData)
            //        //    {
            //        //        range[i].Documents[j].FileName = range[i].CodeString;
            //        //        //test
            //        //        range[i].Documents[j].Rank = 100;
            //        //        range[i].Tags = "test";
            //        //    }
            //        //}

            //        var LCSTSubmitall = GetLCSTSubmittal(range[i].CodeString);
            //        range[i].Documents = new List<DocumentModel>();
            //        range[i].Documents.Add(LCSTSubmitall);
            //    }
            //}


            // Auto generating submittals
            range.ForEach(p =>
        {
            if (p.Documents != null && !p.Documents.Any(d => d.DocumentTypeId == (int)DocumentTypeEnum.SubmittalData))
            {
                if (p.GetSubmittalSheetTemplateName != "SubmittalTemplate") // Ignore generic template
                {
                    var submittalSheet = new DocumentModel
                    {
                        Rank = 1,
                        DocumentTypeId = (int)DocumentTypeEnum.SubmittalData,
                        Type = "SubmittalTemplate",
                        ProductId = p.ProductId.Value,
                        Description = "Submittal Datasheet",
                        FileName = p.ProductNumber,
                        Name = p.ProductNumber,
                        ProductNumber = p.ProductNumber
                    };

                    p.Documents.Add(submittalSheet);
                }
            }

            // order documents
            p.Documents = p.Documents.OrderBy(d => d.Rank).ToList();
        });


        }

        public string GetLCSTSubmittalURL(long quoteId, string modelCodeString)
        {
            string url = Utilities.GetQuotePackageDirectory(quoteId) + "Submittal Sheets\\" + modelCodeString;
            return url;
        }

        public DocumentModel LCSTSubmittalData(string modelCodeString)
        {
            DocumentModel doc = new DocumentModel()
            {
                FileName = modelCodeString,
                
                Name = modelCodeString,
                Type = "Submittal Data",
                DocumentTypeId = (int)DocumentTypeEnum.SubmittalData,
                Description = "Submittal Data",
                Rank = 1
                //ProductId = d.ProductId,
                //DocumentId = d.DocumentId,
           
        
            };
            return doc;
        }

        public ServiceResponse GetProduct(UserSessionModel user, ProductModel model, bool isSubmittial = false)
        {
            this.Db.ReadOnly = true;

            IQueryable<Product> query;

            if (model.ProductId.HasValue)
                query = from e in this.Db.GetProductsByProductId(user, model.ProductId) select e;
            else
                query = from e in this.Db.GetProductsByProductNumber(user, model.ProductNumber) select e;

            var querymodel = QueryToModel(query);

            model = querymodel.FirstOrDefault();

            this.Response.Model = model;

            if (model != null)
            {

                var product = new List<ProductModel> { model };

                GetSubProducts(product);

                GetParentProducts(model);

                GetAccessories(product);

                GetProductAndAccessoryImages(product);

                GetSpecifications(product, null);

                if (!isSubmittial)
                {
                    GetDocuments(product);
                }
                else
                {
                    GetProductNotes(product);

                    GetProductSubmittalImages(product);
                }

                // Get products ProductFamilyTabs headings
                if (isSubmittial == false)
                {
                    model.ProductFamilyTabs = GetProductFamilyTabsModel(user, model.ProductFamilyId);
                }
            }

            this.Response.Model = model;

            return this.Response;

        }

        public ServiceResponse GetProductByProductNumber(UserSessionModel user, ProductModel model)
        {
            var query = from e in this.Db.GetProductsByProductNumber(user, model.ProductNumber)
                        select new ProductModel
                        {
                            ProductId = e.ProductId,
                            ProductNumber = e.ProductNumber,
                            Name = e.Name

                        };
            this.Response.Model = query.FirstOrDefault();
            return this.Response;
        }

        public long GetProductId(UserSessionModel user, string productNumber)
        {
            var query = from e in this.Db.GetProductsByProductNumber(user, productNumber)
                        select e.ProductId;

            return query.FirstOrDefault();
        }

        public Product GetProductbyProductNumber(UserSessionModel user, string productNumber)
        {
            var query = from entity in this.Db.GetProductsByProductNumber(user, productNumber)
                        select entity;

            return query.FirstOrDefault();
        }

        public bool ProductExist(UserSessionModel user, string productNumber)
        {
            if (this.Db.GetProductsByProductNumber(user, productNumber).Any())
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public ServiceResponse GetProductCategoryList(UserSessionModel admin, int productFamilyId)
        {
            // get categories
            var query = from entity in this.Db.ProductCategoryQuery(admin, productFamilyId)
                        select new CatelogListModel
                        {
                            Id = entity.ProductCategoryId,
                            Name = entity.Name,
                            Description = entity.Description
                        };

            var model = new ProductCategoriesModel
            {
                Items = query.ToList()
            };

            // Get products headings
            model.ProductFamilyTabs = GetProductFamilyTabsModel(admin, productFamilyId);

            model.ProductFamilyId = productFamilyId;

            model.ProductFamilyName = model.ProductFamilyTabs.Where(p => p.Id == productFamilyId).Select(p => p.Description).FirstOrDefault();

            this.Response.Model = model;

            return this.Response;
        }

        public ServiceResponse GetProductFamilyList(UserSessionModel admin)
        {

            List<TabModel> tabs = new List<TabModel>();

            tabs = this.GetProductFamilyTabsModel(admin, null);

            var model = new ProductFamiliesModel
            {
                ProductFamilyTabs = tabs
            };

            model.Items = model.ProductFamilyTabs.Select(t => new CatelogListModel
            {
                Id = t.Id,
                Name = t.Description,
                Description = t.Description
            }).ToList();


            this.Response.Model = model;

            return this.Response;
        }

        public List<TabModel> GetProductFamilyTabsModel(UserSessionModel admin, int? productFamilyId)
        {
            List<TabModel> tabs = new List<TabModel>();

            var query = from p in this.Db.GetProducts(admin)
                        join f in this.Db.ProductFamilies on p.ProductFamilyId equals f.ProductFamilyId
                        select new
                        {
                            productFamilyId = p.ProductFamilyId,
                            description = f.Name,
                            Order = f.Order
                        };

            var distinctFamilies = query.Distinct().OrderBy(f => f.Order).ToList();

            foreach (var item in distinctFamilies)
            {
                if (item != null)
                {
                    TabModel tab = new TabModel();
                    tab.IsActive = (productFamilyId.HasValue && item.productFamilyId == productFamilyId);
                    tab.Id = item.productFamilyId;
                    tab.Description = item.description;
                    tabs.Add(tab);
                }
            }



            return tabs;
        }
        public List<TabModel> GetUnitInstallationTypes(UserSessionModel admin, int? productFamilyTypeId, int? unitInstallationTypeId)
        {
            List<TabModel> tabs = new List<TabModel>();

            if (productFamilyTypeId != null)
            {

                var query = from p in this.Context.Products
                            join u in this.Context.UnitInstallationTypes on p.UnitInstallationTypeId equals u.UnitInstallationTypeId
                            where p.ProductFamilyId == productFamilyTypeId && p.UnitInstallationTypeId != (int)UnitInstallationTypeEnum.Other
                            select new
                            {
                                UnitInstallationTypeId = p.UnitInstallationTypeId,
                                Order = u.Order
                            };

                var distinctInstallationTypes = query.Distinct().OrderBy(p => p.Order).Select(p => p.UnitInstallationTypeId).ToList();

                foreach (var item in distinctInstallationTypes)
                {
                    if (item != null)
                    {
                        TabModel tab = new TabModel();
                        tab.IsActive = (unitInstallationTypeId.HasValue && item == unitInstallationTypeId);
                        tab.Id = item;
                        tab.Description = ((UnitInstallationTypeEnum)item).GetDescription();
                        tabs.Add(tab);
                    }
                }
            }

            return tabs;
        }

        public List<TabModel> GetDistinctProductClassPIMs(UserSessionModel admin, int? productFamilyTypeId, int? productClassPIMId)
        {
            List<TabModel> tabs = new List<TabModel>();

            if (productFamilyTypeId != null)
            {

                var query = from p in this.Context.Products
                            join pi in this.Context.ProductClassPIMs on p.ProductClassPIMId equals pi.ProductClassPIMId
                            where p.ProductFamilyId == productFamilyTypeId
                            select new
                            {
                                ProductClassPIMId = p.ProductClassPIMId,
                                Order = pi.Order
                            };

                var distinctProductClassPIMs = query.Distinct().OrderBy(p => p.Order).Select(p => p.ProductClassPIMId).ToList();

                foreach (var item in distinctProductClassPIMs)
                {
                    if (item != null)
                    {
                        TabModel tab = new TabModel();
                        tab.IsActive = (productClassPIMId.HasValue && item == productClassPIMId);
                        tab.Id = item;
                        tab.Description = ((ProductClassPIMEnum)item).GetDescription();
                        tabs.Add(tab);
                    }
                }
            }

            return tabs;
        }

        public ServiceResponse GetInstallationTypes(UserSessionModel admin, int? productFamilyTypeId)
        {
            List<TabModel> tabs = new List<TabModel>();

            if (productFamilyTypeId != null)
            {

                var query = from p in this.Context.Products
                            join u in this.Context.UnitInstallationTypes on p.UnitInstallationTypeId equals u.UnitInstallationTypeId
                            where p.ProductFamilyId == productFamilyTypeId
                            select new
                            {
                                UnitInstallationTypeId = p.UnitInstallationTypeId,
                                Order = u.Order
                            };

                var distinctInstallationTypes = query.Distinct().OrderBy(p => p.Order).Select(p => p.UnitInstallationTypeId).ToList();

                foreach (var item in distinctInstallationTypes)
                {
                    if (item != null)
                    {
                        TabModel tab = new TabModel();
                        //tab.IsActive = (unitInstallationTypeId.HasValue && item == unitInstallationTypeId);
                        tab.Id = item;
                        tab.Description = ((UnitInstallationTypeEnum)item).GetDescription();
                        tabs.Add(tab);
                    }
                }
            }

            this.Response.Model = tabs;
            return this.Response;
        }

        //public List<TabModel> GetUnitInstallationTypes(UserSessionModel admin, List<ProductModel> products)
        //{
        //    List<TabModel> tabs = new List<TabModel>();

        //    List<UnitInstallationTypeEnum?> unitInstallationTypes = products.Select(o => o.UnitInstallationTypeId).Distinct().ToList();

        //    foreach (var type in unitInstallationTypes)
        //    {
        //        if (type != null)
        //        {
        //            TabModel tab = new TabModel();
        //            tab.Id = (long)type.Value;
        //            tab.Description = type.GetDescription();

        //            tabs.Add(tab);
        //        }

        //    }

        //    return tabs;
        //}

        public List<ProductListModel> FilterOnSpecification(IEnumerable<ProductListModel> productLists, ProductModelTypeEnum[] includeSystemModelTypes, string specificationName, string selectedId)
        {
            var productList = productLists.FirstOrDefault();
            var products = productLists.Select(p => p.Product);
            var systemModelSpecItems = GetProductSpecificationsForSystem(products, includeSystemModelTypes, specificationName);

            var otherSpecItems = GetProductSpecifications(
                 products.Where(p => p.ProductModelTypeId != ProductModelTypeEnum.System).ToList(),
                     specificationName
             );

            IEnumerable<ProductSpecificationModel> specs = systemModelSpecItems ?? new List<ProductSpecificationModel>();

            if (otherSpecItems != null)
            {
                specs = specs.Concat(otherSpecItems);
            }

            if (selectedId != null)
            {
                var productIds = specs.Where(p => p.Key == selectedId).Select(p => p.ProductId).ToList();
                products = products.Where(p => productIds.Contains(p.ProductId)
                      || p.SubProducts.Where(sp => productIds.Contains(sp.ProductId)).Count() > 0);
            }

            return products.Select(p => new ProductListModel { Product = p, Timestamp = p.Timestamp }).ToList();
        }

        private void SetEnabledDropDownValues(DropDownModel model, IQueryable<Product> query, ProductModelTypeEnum modelType, string specificationName)
        {
            var uiTypes = query
                            .Select(
                                s =>
                                    s.ParentProductAccessories
                                        .Where(
                                            wppa =>
                                                s.ProductModelTypeId == (int)ProductModelTypeEnum.System
                                                && wppa.Product.ProductModelTypeId == (int)modelType
                                                && wppa.RequirementTypeId == (int)RequirementTypeEnums.Standard)
                                        .FirstOrDefault()
                                        .Product.ProductSpecifications
                                        .Where(w => w.ProductSpecificationLabel.Name == specificationName)
                                        .Select(
                                            ps => ps.Value
                                        ).Distinct()
                            )
                            .ToList();

            foreach (var val in uiTypes.FirstOrDefault())
            {
                model.Items.All(p => p.Disabled = true);

                foreach (var ddi in model.Items)
                {
                    if (ddi.Value == val)
                    {
                        ddi.Disabled = false;
                    }
                }
            }
        }

        public ServiceResponse GetProductList(UserSessionModel admin, SearchProduct search)
        {
            search.ReturnTotals = true;

            var mainQuery = from e in this.Db.ProductsQueryBySearch(
                          admin,
                          search,
                          (innerSearch, innerQuery) =>
                          {
                              var service = new HtmlServices(this.Context);

                              // Dropdowns 
                              innerSearch.DropDownHeatExchangerType = service.DropDownModelSpecification("HeatExchangerType", innerQuery, ProductModelTypeEnum.Outdoor, search.ProductHeatExchangerTypeId);
                              innerSearch.DropDownUnitInstallationType = service.DropDownModelSpecification("UnitInstallationType", innerQuery, ProductModelTypeEnum.Outdoor, (int?)search.UnitInstallationTypeId);
                              innerSearch.DropDownPowerVoltage = service.DropDownModelSpecification("PowerVoltage", innerQuery, search.ProductHeatExchangerTypeId);
                              innerSearch.DropDownCompressorType = service.DropDownModelSpecification("CompressorType", innerQuery, (int?)search.ProductCompressorStageId);
                              innerSearch.DropDownGasVavleType = service.DropDownModelSpecification("GasVavleType", innerQuery, (int?)search.ProductGasValveTypeId);
                              innerSearch.DropDownMotorType = service.DropDownModelSpecification("MotorType", innerQuery, (int?)search.ProductMotorSpeedTypeId);
                              innerSearch.DropDownInstallationConfigurationType = service.DropDownModelSpecification("InstallationConfiguration", innerQuery, (int?)search.ProductInstallationConfigurationTypeId);
                              innerSearch.DropDownProductCategory = service.DropDownModelProductCategory(innerQuery, ProductModelTypeEnum.Indoor, search.ProductCategoryId);
                              innerSearch.DropDownCoolingCapacityRated = service.DropDownModelCoolingCapacity(innerQuery, search.CoolingCapacityRatedValue);
                              innerSearch.DropDownHeatingCapacityRated = service.DropDownModelHeatingCapacityRated(innerQuery, search.HeatingCapacityRatedValue);
                              innerSearch.DropDownCoolingCapacityNominal = service.DropDownModelCoolingCapacityNominal(innerQuery, search.CoolingCapacityNominalValue);
                              innerSearch.DropDownAirFlowRateHigh = service.DropDownModelAirFlowRateHigh(innerQuery, search.AirFlowRateHighValue);
                              innerSearch.DropDownSortBy = service.DropDownModelProductSortBy((search == null) ? null : search.SortColumn);

                              return innerQuery;
                          })
                            select e;

            var queryModel = QueryToModel(mainQuery);

            GetSubProducts(queryModel.Where(p => p.ProductModelTypeId == ProductModelTypeEnum.System).ToList());

            var result = queryModel.Select(p => new ProductListModel
            {
                Product = p,
                Timestamp = p.Timestamp,
                ProductClassCode = p.ProductClassCode
            }).ToList();

            var model = new ProductsModel(search);

            model.Products = result;

            var products = model.Products.Select(p => p.Product).ToList();

            GetSubProducts(products);

            GetProductImages(products);

            GetSpecifications(products,
                new string[] { "SEERNonducted", "IEERNonducted", "EERNonducted", "HSPFNonducted", "COP47Nonducted",
                                "SEERDucted", "IEERDucted", "EERDucted", "HSPFDucted" ,"COP47Ducted"});

            // Get products headings
            model.ProductFamilyTabs = GetProductFamilyTabsModel(admin, search.ProductFamilyId);

            model.UnitInstallationTypeTabs = GetUnitInstallationTypes(admin, model.ProductFamilyId, (int?)model.UnitInstallationTypeId);

            model.ProductFamilyName = model.ProductFamilyTabs.Where(p => p.Id == search.ProductFamilyId).Select(p => p.Description).FirstOrDefault();

            model.Products = new PagedList<ProductListModel>(result, model);

            this.Response.Model = model;

            return this.Response;

        }

        //This is used by Product grid in angular 2
        public ServiceResponse GetProductListNoPaging(UserSessionModel admin, SearchProduct search)
        {
            search.ReturnTotals = true;

            var mainQuery = from e in this.Db.ProductsQueryBySearchNoPaging(admin, search)
                            select e;

            var queryModel = QueryToModel(mainQuery);

            var result = queryModel.Select(p => new ProductListModel
            {
                Product = p,
                Timestamp = p.Timestamp,
                ProductClassCode = p.ProductClassCode
            }).ToList();


            var model = new ProductsModel(search);

            model.Products = result;

            var products = model.Products.Select(p => p.Product).ToList();

            GetSubProducts(products);

            GetProductImages(products);

            GetSpecifications(products,
               new string[] { "SEERNonDucted", "IEERNonDucted", "SCHENonDucted", "EERNonDucted", "HSPFNonDucted", "COP47NonDucted",
                                "SEERDucted", "IEERDucted", "EERDucted", "HSPFDucted" ,"COP47Ducted","AFUE"});

            // Get products headings
            model.ProductFamilyTabs = GetProductFamilyTabsModel(admin, search.ProductFamilyId);

            model.UnitInstallationTypeTabs = GetUnitInstallationTypes(admin, model.ProductFamilyId, (int?)model.UnitInstallationTypeId);// deprecated

            model.ProductClassPIMTabs = GetDistinctProductClassPIMs(admin, model.ProductFamilyId, (int?)model.ProductClassPIMId);

            model.ProductFamilyName = model.ProductFamilyTabs.Where(p => p.Id == search.ProductFamilyId).Select(p => p.Description).FirstOrDefault();

            model.Products = new PagedList<ProductListModel>(result, model);

            //postProcessModelType(model);

            this.Response.Model = model;

            return this.Response;

        }

        //Daikin Equip App
        public ServiceResponse GetAllProducts(SearchProduct search)
        {
            search.ReturnTotals = true;

            var mainQuery = from e in this.Db.ProductsQueryBySearch(search)
                            select e;

            var queryModel = QueryToModel(mainQuery);

            var result = queryModel.Select(p => new ProductListModel
            {
                Product = p,
                Timestamp = p.Timestamp,
                ProductClassCode = p.ProductClassCode
            }).ToList();


            var model = new ProductsModel(search);

            model.Products = result;

            var products = model.Products.Select(p => p.Product).ToList();

            GetSubProducts(products);

            GetProductImages(products);

            GetSpecifications(products,
               new string[] { "SEERNonDucted", "IEERNonDucted", "SCHENonDucted", "EERNonDucted", "HSPFNonDucted", "COP47NonDucted",
                                "SEERDucted", "IEERDucted", "EERDucted", "HSPFDucted" ,"COP47Ducted","AFUE"});

            model.Products = new PagedList<ProductListModel>(result, model);

            //postProcessModelType(model);

            this.Response.Model = model;

            return this.Response;

        }

        //ODATA
        public IQueryable<ProductListModel> GetProductListModels()
        {
            //Test
            //var query = from p in this.Db.Products
            //            join pdlk in this.Db.DocumentProductLinks
            //                on p.ProductId equals pdlk.ProductId
            //            join d in this.Db.Documents
            //                on pdlk.DocumentId equals d.DocumentId
            //            //on new {pdlk.DocumentId , DocumentTypeEnum.ProductImageLowRes} equals 
            //            //    new { d.DocumentId , d.DocumentTypeId}
            //            where d.DocumentTypeId == (int)DocumentTypeEnum.ProductImageLowRes

            //            select new ProductModel
            //            {
            //                ProductId = p.ProductId,
            //                ProductNumber = p.ProductNumber,
            //                Price = p.ListPrice,
            //                Image = new DocumentModel
            //                {
            //                    DocumentId = d.DocumentId,
            //                    DocumentTypeId = d.DocumentTypeId,
            //                    Type = "Product Image - LowRes",
            //                    FileName = d.FileName,
            //                    FileExtension = d.FileExtension
            //                }

            //            };

            //var result = query.AsQueryable();

            var mainQuery = this.Db.GetAllProducts();

            var queryModel = QueryToModel(mainQuery);

            var result = queryModel.Select(p => new ProductListModel
            {
                Product = p,
                Timestamp = p.Timestamp,
                ProductClassCode = p.ProductClassCode
            }).ToList();


            var model = new ProductsModel();

            model.Products = result;

            var products = model.Products.Select(p => p.Product).ToList();

            GetSubProducts(products);

            GetProductImages(products);

            GetSpecifications(products,
               new string[] { "SEERNonDucted", "IEERNonDucted", "SCHENonDucted", "EERNonDucted", "HSPFNonDucted", "COP47NonDucted",
                                "SEERDucted", "IEERDucted", "EERDucted", "HSPFDucted" ,"COP47Ducted","AFUE"});

            model.Products = new PagedList<ProductListModel>(result, model);

            var finalResult = (model.Products).AsQueryable();

            return finalResult;

        }

        public ServiceResponse GetAllProductDocumentLinks(DocumentProductLinksModel search) {
            search.ReturnTotals = true;

            var query = from e in this.Db.GetAllProductDocumentLinks(search)
                        select e;

            var result = new DocumentProductLinksModel()
            {
                ProductFamilyId = search.ProductFamilyId,
                ProductStatusTypeId = search.ProductStatusTypeId,
                Documents = query.ToList()
            };

            if (search != null && search.ReturnTotals)
            {
                result.TotalRecords = query.Count();
            }

            this.Response.Model = result;

            return this.Response;
        }

        public ProductsModel SetActiveTab(ProductsModel model)
        {

            if (model.ProductFamilyId == (int)ProductFamilyEnum.MultiSplit || model.ProductFamilyId == (int)ProductFamilyEnum.VRV || model.ProductFamilyId == (int)ProductFamilyEnum.AlthermaSplit || model.ProductFamilyId == (int)ProductFamilyEnum.AlthermaMonobloc)
            {
                if (model.ProductModelTypeId == null)
                {
                    model.ProductModelTypeId = ProductModelTypeEnum.Indoor;
                }
                //else if ((int)model.ProductModelTypeId == 100000999)
                //{// show All
                //    model.ProductModelTypeId = null;
                //}
            }

            if (model.ProductFamilyId == (int)ProductFamilyEnum.UnitarySplitSystem || model.ProductFamilyId == (int)ProductFamilyEnum.UnitaryPackagedSystem || model.ProductFamilyId == (int)ProductFamilyEnum.LightCommercialSplitSystem || model.ProductFamilyId == (int)ProductFamilyEnum.LightCommercialPackagedSystem)
            {
                if (model.ProductClassPIMId == null)
                {
                    if (model.ProductFamilyId == (int)ProductFamilyEnum.UnitarySplitSystem)
                    {
                        model.ProductClassPIMId = ProductClassPIMEnum.SplitAC;
                    }
                    else if (model.ProductFamilyId == (int)ProductFamilyEnum.UnitaryPackagedSystem)
                    {
                        model.ProductClassPIMId = ProductClassPIMEnum.PackagedAC;
                    }
                    else if (model.ProductFamilyId == (int)ProductFamilyEnum.LightCommercialSplitSystem)
                    {
                        model.ProductClassPIMId = ProductClassPIMEnum.SplitAC;
                    }
                    else if (model.ProductFamilyId == (int)ProductFamilyEnum.LightCommercialPackagedSystem)
                    {
                        model.ProductClassPIMId = ProductClassPIMEnum.LightCommercialPackagedAC;
                    }
                }


            }

            return model;
        }

        public ProductModel GetProductModel(string productNumber)
        {
            //mass upload change - turned this off
            var service = new ProductServices();
            this.Db.ReadOnly = true;

            var query = from e in this.Db.Products
                        where e.ProductNumber == productNumber
                        select e;

            var querymodel = QueryToModel(query);
            return (querymodel.Count != 1) ? null : querymodel[0];
        }

        public List<ProductModel> GetProductModels(UserSessionModel user, string[] productNumbers)
        {

            var service = new ProductServices();

            this.Db.ReadOnly = true;

            var query = from e in this.Db.GetProductsByProductNumbers(user, productNumbers) select e;

            var querymodel = QueryToModel(query);

            return querymodel;

        }

        public List<ProductModel> GetProductModelsForSubmittalSheetGeneration()
        {
            return Db.GetProductModelsForSubmittalSheetGeneration();
        }

        public Product GetProductRecord(string productNumber)
        {
            return this.Db.Products.Where(p => p.ProductNumber == productNumber).FirstOrDefault();
        }

        public ProductModel GetProductSubmittalData(UserSessionModel user, string productNumber)
        {
            var products = GetProductSubmittalData(user, new string[] { productNumber });

            //return (products.Count != 1) ? null : products[0]; this line cause the problem that not create the submittal pdf file
            return products[0];
        }

        public List<ProductModel> GetProductSubmittalData(UserSessionModel user, string[] productNumbers)
        {
            this.Db.ReadOnly = true;

            var query = from e in this.Db.GetProductsByProductNumbers(user, productNumbers) select e;

            var querymodel = QueryToModel(query);

            var products = querymodel.ToList();

            GetSubProducts(products);

            GetDocuments(products);

            GetAccessories(products);

            GetProductAndAccessoryImages(products);

            GetSpecifications(products, null);

            GetProductNotes(products);

            GetProductSubmittalImages(products);

            return products;
        }

        public string GetSubmittalDataFile(string productNumber)
        {
            var product = GetProductModel(productNumber);

            if (product == null) return null;

            GetDocuments(new List<ProductModel> { product });

            var submittalDocument = product.Documents.Where(d => d.DocumentTypeId == (int)DocumentTypeEnum.SubmittalData).FirstOrDefault();

            string file = null;

            if (submittalDocument != null)
            {
                if (submittalDocument.Type == "Submittal Data")
                {
                    file = Utilities.GetSubmittalDirectory() + submittalDocument.FileName + @".pdf";
                }
                else
                {
                    //LMW 02/18 This is an missing AutoGen SDS - rebuild it
                    file = Utilities.GetSubmittalDirectory() + "SDS-DC_" + product.ProductNumber + ".pdf";

                    GenerateSubmittalDataFile(product.ProductNumber);
                    
                }
            }

            return (File.Exists(file)) ? file : null;

        }

        //LMW Added 02/18
        public string GetAutoSubmittalDataFile(string productNumber)
        {
            var product = GetProductModel(productNumber);

            if (product == null) return null;

            GetDocuments(new List<ProductModel> { product });

            var submittalDocument = product.Documents.Where(d => d.DocumentTypeId == (int)DocumentTypeEnum.SubmittalData).FirstOrDefault();

            string file = null;

            if (submittalDocument != null)
            {
                file = Utilities.GetAutoSubmittalDirectory() + "SDS-DC_" + product.ProductNumber + ".pdf";

                GenerateAutoSubmittalDataFile(product.ProductNumber);
            }

            return (File.Exists(file)) ? file : null;

        }

        public List<ProductModel> QueryToModel(IQueryable<Product> query)
        {

            // TODO:  Modify Product Model
            var result = from e in query
                         select new ProductModel
                         {
                             ProductId = e.ProductId,
                             ProductNumber = e.ProductNumber,
                             ProductFamilyId = e.ProductFamilyId,
                             ProductFamilyName = e.ProductFamily.Name,
                             ProductSubFamilyId = e.ProductSubFamilyId,
                             ProductSubFamilyName = e.ProductSubFamily.Description,
                             ProductTypeId = e.ProductTypeId,
                             ProductTypeName = e.ProductType.Description,
                             Price = e.ListPrice,

                             ProductModelTypeId = (ProductModelTypeEnum)e.ProductModelTypeId,
                             //UnitInstallationTypeId = (UnitInstallationTypeEnum)e.UnitInstallationTypeId,
                             ProductModelTypeDescription = e.ProductModel.Description,
                             ProductClassCode = e.ProductClassCode,
                             SubmittalSheetTypeId = (SubmittalSheetTypeEnum)e.SubmittalSheetTypeId,
                             SubmittalSheetTypeDescription = e.SubmittialSheet.Description,
                             Name = e.Name,
                             ProductBrandId = e.BrandId,
                             ProductBrandName = e.Brand.Name,
                             ProductSpecifications = (from spec in e.ProductSpecifications
                                                      select new ProductSpecificationModel
                                                      {
                                                          Id = spec.ProductSpecificationLabelId,
                                                          Name = spec.ProductSpecificationLabel.Name,
                                                          Value = spec.Value
                                                      }).ToList(),


                             EERNonducted = e.EERNonDucted,
                             IEERNonDucted = e.IEERNonDucted,
                             SEERNonducted = e.SEERNonDucted,
                             COP47Nonducted = e.COP47NonDucted,
                             HSPFNonducted = e.HSPFNonDucted,

                             // TODO:  Charles - Do we need to actually change these names?
                             ProductPowerVoltageTypeId = e.ProductPowerVoltageTypeId,
                             ProductPowerVoltageTypeDescription = e.ProductPowerVoltageType.Description,

                             Tonnage = e.Tonnage,
                             HeatingCapacityRated = e.HeatingCapacityRated,
                             CoolingCapacityRated = e.CoolingCapacityRated,
                             CoolingCapacityNominal = e.CoolingCapacityNominal,
                             AirFlowRateHighCooling = e.AirFlowRateHighCooling,
                             AirFlowRateHighHeating = e.AirFlowRateHighHeating,

                             ProductCategoryId = e.ProductCategoryId,
                             ProductCategoryName = e.ProductCategory.Name,

                             ProductFunctionCategoryId = e.ProductFunctionCategoryId,
                             ProductFunctionCategoryName = e.ProductFunctionCategory.Name,


                             UnitInstallationTypeId = e.UnitInstallationTypeId,
                             UnitInstallationTypeDescription = e.UnitInstallationType.Name,

                             ProductClassPIMId = e.ProductClassPIMId,
                             ProductClassPIMDescription = e.ProductClassPIM.Description,

                             ProductHeatExchangerTypeId = e.ProductHeatExchangerTypeId,
                             ProductHeatExchangerTypeDescription = e.ProductHeatExchangerType.Description,

                             ProductCompressorTypeId = e.ProductCompressorStageId,
                             ProductCompressorTypeDescription = e.ProductCompressorStage.Description,

                             ProductGasValveTypeId = e.ProductGasValveTypeId,
                             ProductGasValveTypeDescription = e.ProductGasValveType.Description,

                             ProductMotorSpeedTypeId = e.ProductMotorSpeedTypeId,
                             ProductMotorSpeedTypeDescription = e.ProductMotorSpeedType.Description,

                             ProductInstallationConfigurationTypeId = e.ProductInstallationConfigurationTypeId,
                             ProductInstallationConfigurationTypeDescription = e.ProductInstallationConfigurationType.Description,

                             ProductAccessoryTypeId = e.ProductAccessoryTypeId,
                             ProductAccessoryTypeDescription = e.ProductAccessoryType.Description,

                             ProductEnergySourceTypeId = e.ProductEnergySourceTypeId,
                             ProductEnergySourceTypeDescription = e.ProductEnergySourceType.Description,

                             ProductStatusTypeId = e.ProductStatusId,
                             ProductStatusTypeDescription = e.ProductStatus.Description,

                             InventoryStatusId = e.InventoryStatusId,
                             InventoryStatusDescription = e.InventoryStatuses.Description,
                             InvAvailableDate = e.InvAvailableDate,

                             Timestamp = e.Timestamp,
                             Specifications = new ProductSpecificationsModel { }
                         };

            return result.ToList();
        }

        public IEnumerable<ProductSpecificationModel> GetProductSpecifications(IEnumerable<ProductModel> products, string specName)
        {
            GetSpecifications(products, new string[] { specName }, false);

            return products
                .Select(p =>
                    p.Specifications.All != null && p.Specifications.All.ContainsKey(specName) ?
                        p.Specifications.All[specName] : null).ToList();
        }

        public IEnumerable<ProductSpecificationModel> GetProductSpecificationsForSystem(IEnumerable<ProductModel> products, ProductModelTypeEnum[] includeSystemModelTypes, string specName)
        {
            var systemProducts = products.Where(p => p.ProductModelTypeId == ProductModelTypeEnum.System)
                .Select(p => p.SubProducts.Where(sp => includeSystemModelTypes.Contains(sp.ProductModelTypeId)).FirstOrDefault()).ToList();

            GetSpecifications(systemProducts, new string[] { specName }, false);

            return systemProducts
                .Select(p =>
                    p.Specifications.All != null && p.Specifications.All.ContainsKey(specName) ?
                        p.Specifications.All[specName] : null).ToList();
        }

        public void GetSpecifications(IEnumerable<ProductModel> products, string[] specNames, bool includeSubProducts = true)
        {
            var range = products.ToList(); // Create a new list to prevent update of products list

            // If it is a system then get the sub product specs

            if (includeSubProducts)
            {
                range.Where(s => (s.IsSystem || s.IsSystemTemplate) && s.SubProducts != null)
                    .ToList()
                    .ForEach(p => range.AddRange(p.SubProducts));
            }

            var ids = range.Select(i => i.ProductId.Value).Distinct().ToArray();

            var lookup = Db.GetProductSpecifications(ids, specNames);

            // Find list of specs for each product
            range.ForEach(i =>
            {
                ProductSpecificationsModel specs;
                if (lookup.TryGetValue(i.ProductId.Value, out specs))
                {
                    i.Specifications.All = specs.All;
                }
            });
        }

        public void GetSubProducts(List<ProductModel> products)
        {
            var ids = products.Select(i => i.ProductId.Value).Distinct().ToList();

            var specLookup =
            (
                from sub in this.Context.VwProductSystemComponents
                join e in this.Context.Products on sub.ComponentProductId equals e.ProductId
                where ids.Contains(sub.ProductId)
                select new ProductModel
                {
                    Quantity = sub.ComponentQuantity,
                    ParentProductId = sub.ProductId,
                    ProductId = sub.ComponentProductId,
                    ProductNumber = e.ProductNumber,
                    ProductFamilyName = e.ProductFamily.Name,
                    Price = e.ListPrice,
                    ProductFamilyId = e.ProductFamilyId,
                    ProductCategoryName = e.ProductCategory.Name,
                    ProductClassCode = e.ProductClassCode,
                    ProductModelTypeId = (ProductModelTypeEnum)e.ProductModelTypeId,
                    Name = e.Name,
                    Specifications = new ProductSpecificationsModel { }
                }
            )
            .GroupBy(p => p.ParentProductId)
            .ToArray()
            .ToDictionary(g => g.Key, g => g.ToList());
            // Find list of specs for each product
            products.ForEach(i =>
            {
                List<ProductModel> subs;
                specLookup.TryGetValue(i.ProductId.Value, out subs);
                i.SubProducts = subs ?? new List<ProductModel>();
            });

        }

        private string GenerateSubmittalDataFile(string productNumber)
        {
            if (string.IsNullOrEmpty(productNumber)) return null;

            PdfConvertor pdf = new PdfConvertor();

            var controller = string.Format("{0}/{1}", Utilities.DocumentServerURL(), "ProductDashboard");

            //var controller = string.Format("{0}/{1}", "http://localhost:62801" , "ProductDashboard");

            var submittalDirectory = Utilities.GetSubmittalDirectory();

            var web = new WebClientLocal(HttpContext.Current);

            var url = string.Format("{0}/{1}?ProductNumber={2}&PdfMode=true", controller, "SubmittalTemplate", productNumber);

            var file = submittalDirectory + "SDS-DC_" + productNumber + ".pdf";

            var start = DateTime.Now.Ticks;

            var html = web.DownloadString(url);

            var htmlTime = new TimeSpan(DateTime.Now.Ticks - start).TotalMilliseconds;

            if (!string.IsNullOrWhiteSpace(html))
            {
                try
                {
                    pdf.WriteToFile(html, file);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                Debug.WriteLine(string.Format("Html {2} ms , Generated {1} ms : {0}", Path.GetFileName(file), new TimeSpan(DateTime.Now.Ticks - start).TotalMilliseconds, htmlTime));
            }

            return file;

        }

        //LMW Added 02/18
        private string GenerateAutoSubmittalDataFile(string productNumber)
        {
            if (string.IsNullOrEmpty(productNumber)) return null;

            PdfConvertor pdf = new PdfConvertor();

            var controller = string.Format("{0}/{1}", Utilities.DocumentServerURL(), "ProductDashboard");

            //var controller = string.Format("{0}/{1}", "http://localhost:62801" , "ProductDashboard");

            var submittalDirectory = Utilities.GetAutoSubmittalDirectory();

            var web = new WebClientLocal(HttpContext.Current);

            var url = string.Format("{0}/{1}?ProductNumber={2}&PdfMode=true", controller, "SubmittalTemplate", productNumber);

            var file = submittalDirectory + "SDS-DC_" + productNumber + ".pdf";

            var start = DateTime.Now.Ticks;

            var html = web.DownloadString(url);

            var htmlTime = new TimeSpan(DateTime.Now.Ticks - start).TotalMilliseconds;

            if (!string.IsNullOrWhiteSpace(html))
            {
                try
                {
                    pdf.WriteToFile(html, file);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                Debug.WriteLine(string.Format("Html {2} ms , Generated {1} ms : {0}", Path.GetFileName(file), new TimeSpan(DateTime.Now.Ticks - start).TotalMilliseconds, htmlTime));
            }

            return file;

        }


        // Accessories returned for a product, systems do not have any only the sub products
        public void GetAccessories(List<ProductModel> products)
        {
            var range = products.ToList(); // Create a new list to prevent update of products list

            // If it is a system then get the sub product specs
            range.Where(s => (s.IsSystem || s.IsSystemTemplate) && s.SubProducts != null).ToList()
                .ForEach(p => range.AddRange(p.SubProducts));

            var ids = range.Select(i => i.ProductId.Value).Distinct().ToArray();

            var lookup = Db.GetProductAccessories(ids);

            // Find list of specs for each product
            range.ForEach(i =>
            {
                List<ProductAccessoryModel> ac;
                lookup.TryGetValue(i.ProductId.Value, out ac);
                i.Accessories = ac ?? new List<ProductAccessoryModel>();
            });
        }

        public void GetParentProducts(ProductModel product)
        {
            var lookup = Db.GetParentSystemComponents(product.ProductId);

            product.ParentProducts = lookup.ToList();
        }

        public void GetProductAndAccessoryImages(List<ProductModel> products)
        {
            // Accessories to get image for them
            var productAndAccessories = products.ToList();

            var range = products.ToList();

            range.Where(s => s.SubProducts != null).ToList().ForEach(p => range.AddRange(p.SubProducts));

            range.ForEach(p => { if (p.Accessories != null) p.Accessories.ForEach(a => productAndAccessories.Add(a.Accessory)); });

            GetProductImages(productAndAccessories);

        }

        public void GetProductImages(List<ProductModel> products)
        {
            var range = products.ToList(); // Create a new list to prevent update of products list

            range.Where(s => s.SubProducts != null).ToList().ForEach(p => range.AddRange(p.SubProducts));

            range.Where(s => s.ParentProducts != null).ToList().ForEach(p => range.AddRange(p.ParentProducts));

            var ids = range.Select(i => i.ProductId.Value).Distinct().ToArray();

            var lookup = Db.GetProductImages(ids, new DocumentTypeEnum[] { DocumentTypeEnum.ProductImageLowRes });

            // Find list of specs for each product
            range.ForEach(i =>
            {
                List<DocumentModel> imgs;
                lookup.TryGetValue(i.ProductId.Value, out imgs);
                if (imgs == null)
                {
                    i.Image = new DocumentModel();
                }
                else
                {
                    i.Image = imgs.FirstOrDefault() ?? new DocumentModel();
                }

            });
        }

        public void GetProductNotes(List<ProductModel> products)
        {
            var range = products.ToList(); // Create a new list to prevent update of products list

            // If it is a system then get the sub product specs
            range.Where(s => s.ProductModelTypeId == ProductModelTypeEnum.System && s.SubProducts != null)
                .ToList()
                .ForEach(p => range.AddRange(p.SubProducts));

            var ids = range.Select(i => i.ProductId.Value).Distinct().ToArray();

            var lookup = Db.GetProductNotes(ids);

            // Find list of specs for each product
            range.ForEach(i =>
            {
                List<ProductNoteModel> notes;
                lookup.TryGetValue(i.ProductId.Value, out notes);

                if (notes == null)
                {
                    i.StandardFeatures = new List<ProductNoteModel>();
                    i.Benefits = new List<ProductNoteModel>();
                    i.Notes = new List<ProductNoteModel>();
                    //TODO: Delete after Sep 01, 2017
                    //i.StandardAndFeature = new List<ProductNoteModel>();
                    i.CabinetFeatures = new List<ProductNoteModel>();
                }
                else
                {
                    i.StandardFeatures = notes.Where(n => n.ProductNoteTypeId == (int)ProductNoteTypeEnum.StandardFeature).OrderBy(n => n.Rank).ToList();
                    i.Benefits = notes.Where(n => n.ProductNoteTypeId == (int)ProductNoteTypeEnum.Benefit).OrderBy(n => n.Rank).ToList();
                    i.Notes = notes.Where(n => n.ProductNoteTypeId == (int)ProductNoteTypeEnum.Note).OrderBy(n => n.Rank).ToList();
                    //TODO: Delete after Sep 01, 2017
                    //i.StandardAndFeature = notes.Where(n => n.ProductNoteTypeId == (int)ProductNoteTypeEnum.StandardAndFeature).OrderBy(n => n.Rank).ToList();
                    i.CabinetFeatures = notes.Where(n => n.ProductNoteTypeId == (int)ProductNoteTypeEnum.CabinetFeature).OrderBy(n => n.Rank).ToList();
                }

            });
        }

        public void GetProductSubmittalImages(List<ProductModel> products)
        {
            var range = products.ToList(); // Create a new list to prevent update of products list

            range.Where(s => s.SubProducts != null).ToList().ForEach(p => range.AddRange(p.SubProducts));

            var ids = range.Select(i => i.ProductId.Value).Distinct().ToArray();

            var lookup = Db.GetProductImages(ids, new DocumentTypeEnum[] { DocumentTypeEnum.DimensionalDrawing, DocumentTypeEnum.ProductLogos });

            range.ForEach(p =>
            {
                List<DocumentModel> imgs;

                lookup.TryGetValue(p.ProductId.Value, out imgs);

                if (imgs == null)
                {
                    p.DimensionalDrawing = new DocumentModel();

                    p.Logos = new List<DocumentModel>();
                }
                else
                {
                    p.DimensionalDrawing = imgs.Where(i => i.DocumentTypeId == (int)DocumentTypeEnum.DimensionalDrawing).FirstOrDefault() ?? new DocumentModel();

                    p.Logos = imgs.Where(i => i.DocumentTypeId == (int)DocumentTypeEnum.ProductLogos).OrderBy(r => r.Rank).ToList();
                }



            });
        }

        //TODO: Delete after March 30, 2018
        //public ProductsModel BuildProductsModel(UserSessionModel user, LCSTPackagesModel packagesModel)
        //{
        //    ProductsModel productsModel = new ProductsModel();

        //    foreach (LCSTPackageModel pkg in packagesModel.Packages)
        //    {

        //        ProductListModel baseModel = new ProductListModel();
        //        //baseModel.ProductNumber = pkg.BaseModel;
        //        baseModel.Product.ProductId = GetProductId(user, pkg.BaseModel);
        //        if (baseModel.Product.ProductId != null && baseModel.Product.ProductId != 0)
        //        {
        //            baseModel.Product.Quantity = 1;
        //            productsModel.Products.Add(baseModel);
        //        }
        //        else
        //        {
        //            //what if product is not found?
        //            packagesModel.InValidProducts.Add(pkg.BaseModel);
        //            packagesModel.Message += "-- "+pkg.BaseModel+" is not found --";
        //        }


        //        foreach (string productNumber in pkg.Accessories)
        //        {
        //            ProductListModel accessory = new ProductListModel();
        //            //accessory.ProductNumber = item;
        //            accessory.Product.ProductId = GetProductId(user, productNumber);
        //            if (accessory.Product.ProductId != null && accessory.Product.ProductId != 0)
        //            {
        //                accessory.Product.Quantity = 1;
        //                productsModel.Products.Add(accessory);
        //            }
        //            else
        //            {
        //                //what if product is not found?
        //                packagesModel.InValidProducts.Add(productNumber);
        //                packagesModel.Message += "--" + productNumber + " is not found--";
        //            }

        //        }
        //    }
        //    return productsModel;
        //}

        public ProductsModel ValidateLCSTPackagesModel(UserSessionModel user, LCSTPackagesModel packagesModel)
        {
            ProductsModel productsModel = new ProductsModel();

            foreach (LCSTPackageModel pkg in packagesModel.Packages)
            {
                var baseModelProductId = GetProductId(user, pkg.BaseModel);
                if (baseModelProductId == null || baseModelProductId == 0)
                {
                    packagesModel.InValidProducts.Add(pkg.BaseModel);
                    packagesModel.Message += "-- " + pkg.BaseModel + " is not found --";
                }

                foreach (LCSTAccessory accessory in pkg.Accessories)
                {
                    var accessoryProductId = GetProductId(user, accessory.AccessoryModel);
                    if (accessoryProductId == null || accessoryProductId == 0)
                    {
                        packagesModel.InValidProducts.Add(accessory.AccessoryModel);
                        packagesModel.Message += "--" + accessory.AccessoryModel + " is not found--";
                    }

                }
            }
            return productsModel;
        }

        public ServiceResponse GetProductStatuses(UserSessionModel user) {
            var query = from productStatus in this.Db.GetProductStatuses(user)
                        select new DropdownOption
                        {
                            Text = productStatus.Description,
                            Value = productStatus.ProductStatusId.ToString(),
                            ValueDecimal = productStatus.ProductStatusId
                        };
            List<DropdownOption> result = query.ToList();
            result.Add(new DropdownOption {
                Text = "All",
                Value = null,
                ValueDecimal = 0
            });

            this.Response.Model = result;

            return this.Response;
        }

        public ServiceResponse GetInventoryStatuses(UserSessionModel user)
        {
            var query = from inventoryStatus in this.Db.GetInventoryStatuses(user)
                        select new DropdownOption
                        {
                            Text = inventoryStatus.Description,
                            Value = inventoryStatus.InventoryStatusId.ToString(),
                            ValueDecimal = inventoryStatus.InventoryStatusId
                        };
            List<DropdownOption> result = query.ToList();
            result.Add(new DropdownOption
            {
                Text = "All",
                Value = null,
                ValueDecimal = 0
            });

            this.Response.Model = result;

            return this.Response;
        }

    }

}