using System;
using System.Collections.Generic;
using DPO.Common;
using DPO.Domain;
using DPO.Model.Light;
using System.Web;
using System.Web.Http;
using System.Net;
using System.Net.Http;
using DPO.Web.Controllers.Api.Filters;
using System.Web.Script.Serialization;

namespace DPO.Web.Controllers
{

    [Authorize]
    [AuthenticationFilter]
    [UserActionFilter]
    public class ProductController : BaseApiController
    {
        AccountServices accountService = new AccountServices();
        ProductServices productservices = new ProductServices();
        QuoteServices quoteServices = new QuoteServices();
        BasketServices basketService = new BasketServices();
        
        
        [HttpGet]
        public ServiceResponse getBasketQuoteId()
        {
            var basketQuoteId = HttpContext.Current.Session["BasketQuoteId"];
            var serviceResponse = new ServiceResponse();
            serviceResponse.Model = basketQuoteId;
            return serviceResponse;
        }

        public ServiceResponse Basket()
        {
            return basketService.GetUserBasketModel(this.CurrentUser);
        }


        [HttpGet]
        public ServiceResponse ResetBasketQuoteId()
        {
            var session = HttpContext.Current.Session;
            session["BasketQuoteId"] = null;
            CurrentUser.BasketQuoteId = null;
            return this.ServiceResponse;
        }


        [HttpGet]
        public HttpResponseMessage Products(long? quoteId = null)
        {
            var session = HttpContext.Current.Session;
            session["BasketQuoteId"] = quoteId ?? 0;
            CurrentUser.BasketQuoteId = (long?)session["BasketQuoteId"] ?? 0;

            var response = Request.CreateResponse(HttpStatusCode.Found);
            var baseUrl = Request.RequestUri.GetLeftPart(UriPartial.Authority);
            response.Headers.Location = new Uri(baseUrl + "/v2/#/products");
            return response;

        }

        [HttpGet]
        public ServiceResponse BrowseProductsWithQuoteId(long? quoteId = null)
        {
            var serviceResponse = new ServiceResponse();

            var session = HttpContext.Current.Session;
            session["BasketQuoteId"] = quoteId ?? 0;
            CurrentUser.BasketQuoteId = (long?)session["BasketQuoteId"] ?? 0;

            serviceResponse.Model = quoteId;
            return serviceResponse;

        }

        [HttpGet]
        public ServiceResponse GetProductFamilies()
        {
            return productservices.GetProductFamilyList(this.CurrentUser);

        }


        [HttpPost]
        public ServiceResponse GetProducts(ProductsModel model)
        {

            //viewOption
            var viewOption = model.ViewOption ?? this.CurrentUser.UserSettings.ProductViewOption;

            if (model.ViewOption == null)
            {
                viewOption = ProductsModel.ProductViewOption.List;
            }

            CurrentUser.UserSettings.ProductViewOption = viewOption;

            if (!model.IsSearch)
            {
                productservices.SetActiveTab(model);
            }


            model.SortColumn = "ProductNumber";

            return productservices.GetProductListNoPaging(CurrentUser, model); ;
        }

        //Daikin Equip App
        [HttpPost]
        public ServiceResponse GetAllProducts(ProductsQueryModel queryModel)
        {
            if (queryModel == null)
            {
                queryModel = new ProductsQueryModel();
            }
            return productservices.GetAllProducts(queryModel);
        }

        //Daikin Equip App
        [HttpPost]
        public ServiceResponse GetAllProductDocumentLinks(DocumentProductLinksModel model)
        {
            if (model == null)
            {
                model = new DocumentProductLinksModel();
            }
            return productservices.GetAllProductDocumentLinks(model);
        }


        [HttpPost]
        public ServiceResponse GetInstallationTypes(ProductModel model)
        {
            return productservices.GetInstallationTypes(this.CurrentUser, model.ProductFamilyId);
        }

        [HttpPost]
        public ServiceResponse GetProduct(ProductModel model)
        {
            return productservices.GetProduct(this.CurrentUser, model, false);
        }

        //add multiple products
        [HttpPost]
        public ServiceResponse AddProductsToQuote(ProductsModel productsModel)
        {

            QuoteModel quoteModel = new QuoteModel();

            var session = HttpContext.Current.Session;

            quoteModel.QuoteId = (long?)session["BasketQuoteId"];

            return new QuoteServices().AddProductsToQuote(this.CurrentUser, quoteModel, productsModel);
        }

        [HttpGet]
        public ServiceResponse GetLCSTApiToken() {
            var response = new ServiceResponse();
            response.Model = Utilities.Config("lcst.api.token");
            return response;
        }

        //add products to quote from LC Submittal Tool
        [HttpPost]
        [AllowAnonymous]
        public ServiceResponse LCSTAddToQuote(LCSTPackagesModel packagesModel)
        {

            var serviceResponse = new ServiceResponse();

            if (packagesModel.Token != Utilities.Config("lcst.api.token"))
            {
                serviceResponse.AddError("Invalid Token!");
                return serviceResponse;
            }

            var user = accountService.GetUserSessionModel(packagesModel.UserId).Model as UserSessionModel;

            QuoteModel quoteModel = new QuoteModel()
            {
                QuoteId = packagesModel.QuoteId
            };

            productservices.ValidateLCSTPackagesModel(user, packagesModel);

            if (packagesModel.InValidProducts.Count == 0)
            {
                return quoteServices.AddConfiguredProductsToQuote(user, quoteModel, packagesModel);
            }
            else
            {
                serviceResponse.Messages.AddWarning("Products were not added to quote!");
                if (packagesModel.InValidProducts.Count > 0)
                {
                    serviceResponse.AddError(packagesModel.Message);
                }
                return serviceResponse;
            }

        }


        //add single product
        [HttpPost]
        public ServiceResponse AddProductToQuote(ProductModel productModel)
        {
            QuoteModel quoteModel = new QuoteModel();

            var session = HttpContext.Current.Session;

            quoteModel.QuoteId = (long?)session["BasketQuoteId"];

            return new QuoteServices().AddProductToQuote(this.CurrentUser, quoteModel, (long)productModel.ProductId, (int)productModel.Quantity);
        }

        //[HttpPost]
        //public ServiceResponse AdjustQuoteItems(QuoteItemsModel quoteItemsModel)
        //{

        //    //QuoteModel quoteModel = new QuoteModel();

        //    //var session = HttpContext.Current.Session;

        //    //quoteModel.QuoteId = quoteItemsModel.QuoteId;

        //    //return new QuoteServices().AddProductsToQuote(this.CurrentUser, quoteModel, productsModel); ;
        //    return new QuoteServices().AdjustQuoteItems(this.CurrentUser, quoteItemsModel); 
        //}

        [HttpPost]
        public ServiceResponse AddProductToQuoteByProductNumber(ProductModel productModel)
        {
            QuoteModel quoteModel = new QuoteModel();

            var session = HttpContext.Current.Session;

            quoteModel.QuoteId = (long?)session["BasketQuoteId"];

            ServiceResponse resp = productservices.GetProductByProductNumber(this.CurrentUser, productModel);

            if (resp.IsOK && resp.Model != null)
            {
                ProductModel product = resp.Model as ProductModel;
                return new QuoteServices().AddProductToQuote(this.CurrentUser, quoteModel, (long)product.ProductId, (int)productModel.Quantity);
            }
            else
            {
                var serviceResponse = new ServiceResponse();
                serviceResponse.AddError("Can not find product " + productModel.ProductNumber);
                return serviceResponse;
            }


        }

        [HttpPost]
        public ServiceResponse AddSystemToQuote(SystemMatchupModel systemMatchupModel)
        {
            var serviceResponse = new ServiceResponse();

            QuoteModel quoteModel = new QuoteModel();

            var session = HttpContext.Current.Session;

            quoteModel.QuoteId = (long?)session["BasketQuoteId"];



            //Check if products exist
            foreach (string productNumber in systemMatchupModel.ProductNumbers)
            {
                if (productservices.ProductExist(this.CurrentUser, productNumber))
                {
                    systemMatchupModel.ValidProducts.Add(productNumber);
                }
                else
                {
                    systemMatchupModel.InValidProducts.Add(productNumber);
                    serviceResponse.AddError("Can not find product " + productNumber);
                }
            }

            //Add products to Quote
            if (serviceResponse.IsOK || systemMatchupModel.ContinueAdding)
            {
                var addProductSvcResp = new ServiceResponse();
                //add valid products
                if (systemMatchupModel.ValidProducts.Count > 0)
                {
                    foreach (string productNumber in systemMatchupModel.ValidProducts)
                    {

                        var productId = productservices.GetProductId(this.CurrentUser, productNumber);
                        if (productId != 0)
                        {
                            addProductSvcResp = new QuoteServices().AddProductToQuoteWithTag(this.CurrentUser, quoteModel, productId, systemMatchupModel.Quantity, systemMatchupModel.Tags);
                        }
                        if (!addProductSvcResp.IsOK)
                        {
                            return addProductSvcResp;
                        }
                    }

                    return addProductSvcResp;// Added products successfully
                }
                else
                {
                    addProductSvcResp.AddError("There's no valid products to Add.");
                    return addProductSvcResp;// No valid products to Add
                }

            }
            else
            {
                serviceResponse.Model = systemMatchupModel;
                return serviceResponse; // return with error ("There is invalid product ...")
            }




        }

        [HttpGet]
        public ServiceResponse GetProductStatuses()
        {
            return productservices.GetProductStatuses(this.CurrentUser);
        }

        [HttpGet]
        public ServiceResponse GetInventoryStatuses()
        {
            return productservices.GetInventoryStatuses(this.CurrentUser);
        }
    }
}