using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DPO.Common;
using DPO.Domain;
using System.Net;
using System.IO;
using EO.Pdf;
using System.Drawing;
using EO.Pdf.Mvc5;
using System.Diagnostics;

namespace DPO.Web.Controllers
{
    public class ProductDashboardController : BaseController
    {
        public QuoteServices serviceQuote = new QuoteServices();
        public ProductServices services = new ProductServices();
        [HttpGet]
        public ActionResult Accessory()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Product(ProductModel model)
        {
            this.ServiceResponse = services.GetProduct(this.CurrentUser, model, false);

            ProcessServiceResponse(this.ServiceResponse);

            if (this.ServiceResponse.Model == null)
            {
                //TODO Cannot access product security page
                return null;
            }

            this.RouteData.Values["action"] = "Product";

            return (IsPostRequest) ? (ViewResultBase)PartialView("Product", this.ServiceResponse.Model) : (ViewResultBase)View("Product", this.ServiceResponse.Model);

        }

        [HttpPost]
        [Authorise(Accesses = new[] {  SystemAccessEnum.EditProject})]
        public ActionResult ProductsAddToQuote(QuoteModel quoteModel, ProductsModel productsModel)
        {
            ModelState.DumpErrors();

            quoteModel.QuoteId = Convert.ToInt64(Session["BasketQuoteId"]);

            this.ServiceResponse = new QuoteServices().AddProductsToQuote(this.CurrentUser, quoteModel, productsModel);

            ProcessServiceResponse(this.ServiceResponse);

            ////return base.AjaxReloadPage();
            //return RedirectToAction("Product", "ProductDashboard", quoteModel);

            return Redirect(Request.UrlReferrer.ToString());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ActionResult ProductAddToQuote(QuoteModel model, long productId, int quantity)
        {
            this.ServiceResponse = new QuoteServices().AddProductToQuote(this.CurrentUser, model, productId, quantity);

            ProcessServiceResponse(this.ServiceResponse);

            return base.AjaxReloadPage();
        }

        [HttpPost]
        [Authorise(Accesses = new[] {  SystemAccessEnum.EditProject})]
        public ActionResult AddProductsToQuote(ProductsModel model)
        {
            return View();
        }
        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        //[Authorise(Accesses = new[] {  SystemAccessEnum.EditProject})]
        //public ActionResult ProductsAddToQuote(QuoteModel model, ProductsModel productsModel)
        //{
        //    this.ServiceResponse = new QuoteServices().AddProductsToQuote(this.CurrentUser, model, productsModel);
        //    ProcessServiceResponse(this.ServiceResponse);

        //    return base.AjaxReloadPage();
        //}

        [HttpGet]
        public ActionResult ProductCategories(ProductCategoriesModel model)
        {
            this.ServiceResponse = services.GetProductCategoryList(this.CurrentUser, model.ProductFamilyId);

            ProcessServiceResponse(this.ServiceResponse);

            this.RouteData.Values["action"] = "ProductCategories";

            return (IsPostRequest) ? (ViewResultBase)PartialView("ProductCategories", this.ServiceResponse.Model) : (ViewResultBase)View("ProductCategories", this.ServiceResponse.Model);

        }

        [HttpGet]
        public ActionResult ProductFamilies(long? quoteId)
        {
            this.ServiceResponse = services.GetProductFamilyList(this.CurrentUser);

            this.Session["BasketQuoteId"] = quoteId ?? 0;

            ProcessServiceResponse(this.ServiceResponse);

            this.RouteData.Values["action"] = "ProductFamilies";

            return (IsPostRequest) ? (ViewResultBase)PartialView("ProductFamilies", this.ServiceResponse.Model) : (ViewResultBase)View("ProductFamilies", this.ServiceResponse.Model);

        }

        [HttpGet]
        public ActionResult UnitInstallationTypes(long? quoteId)
        {
            //this.ServiceResponse = services.GetProductFamilyList(this.CurrentUser);

            //ProcessServiceResponse(this.ServiceResponse);

            //this.RouteData.Values["action"] = "ProductFamilies";

            return (IsPostRequest) ? (ViewResultBase)PartialView("UnitInstallationTypes", this.ServiceResponse.Model) : (ViewResultBase)View("UnitInstallationTypes", this.ServiceResponse.Model);

        }

        [HttpGet]
        public ActionResult Products(ProductsModel model)
        {
            var viewOption = model.ViewOption ?? this.CurrentUser.UserSettings.ProductViewOption;

            if (viewOption == null)
            {
                viewOption = ProductsModel.ProductViewOption.List;
            }

            this.CurrentUser.UserSettings.ProductViewOption = viewOption;

            if (!model.FormSubmittedPreviously)
            {
                model.SortColumn = "ProductNumber";

                if (model.ProductModelTypeId == null
                    && (model.ProductFamilyId == (int)ProductFamilyEnum.MultiSplit
                        || model.ProductFamilyId == (int)ProductFamilyEnum.VRV))
                {
                    model.ProductModelTypeId = ProductModelTypeEnum.Indoor;
                }

                if (model.UnitInstallationTypeId == null
                    && (model.ProductFamilyId == (int)ProductFamilyEnum.LightCommercialSplitSystem)) {
                    model.UnitInstallationTypeId = UnitInstallationTypeEnum.CoolingOnly;
                }

                if (model.UnitInstallationTypeId == null
                    && (model.ProductFamilyId == (int)ProductFamilyEnum.UnitarySplitSystem))
                {
                    model.UnitInstallationTypeId = UnitInstallationTypeEnum.AirConditioner;
                }

                if (model.UnitInstallationTypeId == null
                    && (model.ProductFamilyId == (int)ProductFamilyEnum.UnitaryPackagedSystem))
                {
                    model.UnitInstallationTypeId = UnitInstallationTypeEnum.PackageAC;
                }

                model.FormSubmittedPreviously = true;
            }

             if (!String.IsNullOrWhiteSpace(model.Filter)
                && String.Compare(model.PreviousFilter, model.Filter, true) != 0)
            {
                model.ProductModelTypeId = null;
                model.PreviousFilter = model.Filter;
            }
            else if (String.IsNullOrWhiteSpace(model.Filter))

            {
                model.PreviousFilter = String.Empty;
            }

            this.ServiceResponse = services.GetProductList(this.CurrentUser, model);

            ProcessServiceResponse(this.ServiceResponse);

            this.RouteData.Values["action"] = "Products";

            ProductsModel productsModel = (ProductsModel)ServiceResponse.Model;
            if (this.CurrentUser.UserSettings.ProductViewOption.HasValue)
            {
                productsModel.ViewOption = this.CurrentUser.UserSettings.ProductViewOption;
            }

            return (IsPostRequest) ? (ViewResultBase)PartialView("Products", productsModel) : (ViewResultBase)View("Products", productsModel);

        }

        public ProductsModel ResetAllFilter(ProductsModel model)
        {
            if (model.ProductModelTypeId == ProductModelTypeEnum.Outdoor)
            {
                if (model.ProductFamilyId == (int)ProductFamilyEnum.VRV)
                {
                    if (model.IEERNonDuctedMin == 0 &&
                        model.IEERNonDuctedMax == 40 &&
                        model.HSPFNonDuctedMin == 0 &&
                        model.HSPFNonDuctedMax == 10 &&
                        model.EERNonDuctedMin == 0 &&
                        model.EERNonDuctedMax == 20)
                    {
                        model.Filter = string.Empty;
                        model.PreviousFilter = model.Filter;
                    }
                }

            }

            return model;
        }

        public ActionResult SubmittalTemplate(ProductModel model, bool PdfMode, long? quoteItemId, long? projectId)
        {
            var product = services.GetProductSubmittalData(this.CurrentUser, model.ProductNumber);

            if (product != null)
            {
                if (product.Name != null)
                {
                    product.Name = product.Name.Replace("’", "'");
                }
                var submittalDocument = product.Documents.Where(d => d.DocumentTypeId == (int)DocumentTypeEnum.SubmittalData).FirstOrDefault();

                if (submittalDocument != null)
                {
                    if (quoteItemId != null)
                    {
                        QuoteItemListModel productAsQuoteItem = serviceQuote.GetSingleQuoteItemListModel((long)quoteItemId);
                        product.Tags = productAsQuoteItem.Tags;

                        ProjectModel projectUsedInQuotePackage = new ProjectServices().GetProjectModel(this.CurrentUser, projectId).Model as ProjectModel;

                        ViewData["SubmittedTo"] = projectUsedInQuotePackage.EngineerName;
                        ViewData["ProjectName"] = projectUsedInQuotePackage.Name;
                    }

                    var template = product.GetSubmittalSheetTemplateName;

                    if (quoteItemId != null && template == "SubmittalTemplate")
                    {
                        template = "ExternalSubmittalTemplate";
                    }

                    ViewData["PdfMode"] = PdfMode;
                    ViewData["IsInQuotePackage"] = (quoteItemId != null);

                    return View(template, product);
                }
            }

            return new EmptyResult();
        }

        
        public String SubmittalTemplateHtml(ProductModel model, bool PdfMode, long? quoteItemId, long? projectId)
        {
            var product = services.GetProductSubmittalData(this.CurrentUser, model.ProductNumber);

            if (product != null)
            {
                if (product.Name != null)
                {
                    product.Name = product.Name.Replace("’", "'");
                }

                var template = product.GetSubmittalSheetTemplateNameV2;

                this.ViewData.Model = product;
                if (template != "")
                {
                    return this.ToHtml(template, this.ViewData);
                }
                else new EmptyResult().ToString();

                var submittalDocument = product.Documents.Where(d => d.DocumentTypeId == (int)DocumentTypeEnum.SubmittalData).FirstOrDefault();
                                
            }

            return new EmptyResult().ToString();
        }
    }
}
