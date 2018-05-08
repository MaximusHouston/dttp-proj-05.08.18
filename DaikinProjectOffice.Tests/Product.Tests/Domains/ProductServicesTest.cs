using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using DPO.Common;
using DPO.Data;
using System.Transactions;
using System.Data.Entity.Core.Objects;
using System.Data.Common;
using DPO.Domain;
using System.Net.Mail;
using System.Configuration;
using NUnit.Framework;
using System.Web;
using Moq;

namespace DaikinProjectOffice.Tests
{
    [TestFixture]
    public class ProductServicesTest : TestAdmin
    {
        ProductServices productService;
        public ServiceResponse response = new ServiceResponse();

        static long _projectId;
        static long _quoteId;
        static long _quoteItemId;
        static string _productNumber;

        QuoteServices quoteService;
        BasketServices serviceBasket;

        UserSessionModel user = new UserSessionModel();

        ProjectServices projectService;
        SystemTestDataServices systemService;
        BusinessServices businessService;

        ServiceResponse Response = new ServiceResponse();
        ProjectModel projectModel = new ProjectModel();


        public ProductServicesTest() {
            productService = new ProductServices(this.TContext);

            projectService = new ProjectServices(this.TContext);
            businessService = new BusinessServices(this.TContext);
            quoteService = new QuoteServices(this.TContext);

            user = GetUserSessionModel("User15@test.com");

            _projectId = this.db.Context.Projects
                             .Where(p => p.OwnerId == user.UserId)
                             .OrderByDescending(p => p.ProjectId)
                             .Select(p => p.ProjectId)
                             .FirstOrDefault();

            projectModel = projectService.GetProjectModel(user, _projectId).Model as ProjectModel;

            _quoteId = this.db.Context.Quotes.Where(q => q.ProjectId == _projectId)
                           .OrderByDescending(q => q.QuoteId)
                           .Select(q => q.QuoteId)
                           .FirstOrDefault();

            _productNumber = this.db.Context.Products
                                 .Select(pr => pr.ProductNumber)
                                 .Where(pr => pr != null)
                                 .FirstOrDefault();

            _quoteItemId = this.db.Context.QuoteItems
                               .Where(qi => qi.Quantity > 0)
                               .Select(qi => qi.QuoteItemId)
                               .FirstOrDefault();
        }

        [Test]
        [Category("ProductServices")]
        public void Should_return_products()
        {
            //Arrange
            var user = GetUserSessionModel("test@123.com");

            SearchProduct search = new SearchProduct();
            search.ProductFamilyId = (int?)ProductFamilyEnum.VRV;

            //Act
            response = productService.GetProductList(user, search);


            //Assert
            ProductsModel productsModel = (ProductsModel)response.Model;
            Assert.That(response.IsOK, Is.EqualTo(true));
            Assert.That(productsModel.Products.Count(), Is.GreaterThan(0));

        }

        [Test]
        [Category("ProductServices")]
        public void Should_return_only_unitary_split() {
            //Arrange
            var user = GetUserSessionModel("daikincity@daikincomfort.com");
            SearchProduct search = new SearchProduct();
            search.ProductFamilyId = (int?)ProductFamilyEnum.UnitarySplitSystem;

            //Act
            response = productService.GetProductList(user, search);

            //Assert
            ProductsModel productsModel = (ProductsModel)response.Model;
            Assert.That(response.IsOK, Is.EqualTo(true));
            Assert.That(productsModel.Products.Count(), Is.GreaterThan(0));

            bool isUnitarySplit = true;
            foreach (var item in productsModel.Products) {
                if (item.Product.ProductFamilyId != (int?)ProductFamilyEnum.UnitarySplitSystem) {
                    isUnitarySplit = false;
                }
            }
            Assert.That(isUnitarySplit, Is.EqualTo(true));
        }

        [Test]
        [Category("ProductServices")]
        [TestCase("nullValue")]
        [TestCase("NotNull")]
        [Ignore("Need to make HttpContext.Current  != null")]
        public void GenerateSubmittalDataFileForPackage_ShouldReturnHtml(string testCase)
        {
            string html = string.Empty;
            string _param1;
            long? _param2;
            long? _param3;

            if (testCase.Contains("nullValue"))
            {
                _param1 = null;
                _param2 = null;
                _param3 = null;
                html = productService.GenerateSubmittalDataFileForPackage(_param1, _param2, _param3);
                Assert.AreEqual(html, null);
            }
            else
            {
                _param1 = _productNumber;
                _param2 = _quoteItemId;
                _param3 = _projectId;

                var httpContextMock = FakeHttpContext("GET");
                HttpRequest request = new HttpRequest("", string.Empty,
                                                          string.Empty);
                HttpResponse response = new HttpResponse(null);
                HttpContext.Current = new HttpContext(request, response);

                html = productService.GenerateSubmittalDataFileForPackage(_param1, _param2, _param3);
                Assert.That(html, Is.Not.EqualTo(null));
                Assert.That(html.Length, Is.GreaterThan(1));
                Assert.That(html.Contains("<body>"), Is.EqualTo(true));
            }
        }

        [Test]
        [Category("ProductServices")]
        public void GetDocuments_Should_Include_Documents_In_Product(string productNumber)
        {
            List<ProductModel> products = new List<ProductModel>();

            string[] productNumbers = new string[] { "REYQ120PBTJ", "REYQ120PBYD" };
            products = productService.GetProductModels(user, productNumbers);

            if (products != null && products.Count() > 0)
            {
                productService.GetDocuments(products);

                foreach (var product in products)
                {
                    Assert.That(product.Documents.Count, Is.GreaterThan(0));
                }
            }
        }

        [Test]
        [Category("ProductServices")]
        [TestCase("REYQ120PBTJ", false)]
        [TestCase("REYQ120PBTJ", true)]
        public void GetProduct_ShouldReturnProductByProductModel(string productNumber, bool isSubmittal)
        {
            ProductModel model = productService.GetProductModel(productNumber);

            if (model != null)
            {
                GetSubProduct_Should_Include_SubProduct_In_Product(productNumber);
                GetParentProduct_Should_Include_ParentProduct_In_Product(productNumber);
                GetAccessories_Should_Include_ProductAccessories_In_Product(productNumber);
                GetProductAndAccessoryImage_Should_Include_AccessoryAndImage_In_Product(productNumber);
                GetSepcifications_Should_Include_Specifications_In_Product(productNumber);

                if (isSubmittal)
                {
                    GetDocuments_Should_Include_Documents_In_Product(productNumber);
                }
                else
                {
                    GetProductNotes_Should_Include_Notes_In_Product(productNumber);
                    GetProductSubmittalImage_Should_Include_SubmittalImage_In_Product(productNumber);
                }

                Assert.That(model.ProductNumber, Is.EqualTo(productNumber));
                Assert.That(model.ProductSpecifications, Is.Not.EqualTo(null));
                Assert.That(model.SubProducts.Count, Is.EqualTo(2));
                Assert.That(model.ProductFamilyTabs.Count, Is.GreaterThan(0));
                Assert.That(model.ProductSpecifications.Count, Is.GreaterThan(0));
                Assert.That(model.Documents.Count, Is.GreaterThan(0));
            }
        }

        [Test]
        [Category("ProductServices")]
        [TestCase("FTXN09NMVJURXN09NMVJU")]
        public void GetSubProduct_Should_Include_SubProduct_In_Product(string productNumber)
        {
            var model = productService.GetProductModel(productNumber);

            List<ProductModel> products = new List<ProductModel>();
            products.Add(model);

            productService.GetSubProducts(products);

            Assert.That(products.Any(p => p.SubProducts.Count() >= 0), Is.EqualTo(true));
        }
        [Test]
        [Category("ProductServices")]
        [TestCase("FTXN09NMVJURXN09NMVJU")]
        public void GetParentProduct_Should_Include_ParentProduct_In_Product(string productNumber)
        {
            var model = productService.GetProductModel(productNumber);

            productService.GetParentProducts(model);

            Assert.That(model.ParentProducts, Is.Not.EqualTo(null));
            Assert.That(model.ParentProducts.Count, Is.GreaterThanOrEqualTo(0));
        }

        [Test]
        [Category("ProductServices")]
        [TestCase("FTXN09NMVJU")]
        public void GetAccessories_Should_Include_ProductAccessories_In_Product(string productNumber)
        {
            List<ProductModel> models = new List<ProductModel>();
            var model = productService.GetProductModel(productNumber);
            models.Add(model);

            productService.GetAccessories(models);

            foreach(var product in models)
            {
                Assert.That(product.Accessories, Is.Not.EqualTo(null));
                Assert.That(product.Accessories.Count(), Is.GreaterThanOrEqualTo(0));
            }
        }
        [Test]
        [Category("ProductServices")]
        public void GetProductAndAccessoryImage_Should_Include_AccessoryAndImage_In_Product(string productNumber)
        {
            GetProductImages_Should_Include_Images_In_Product(productNumber);
        }

        [Test]
        [Category("ProductServices")]
        [TestCase("FTXN09NMVJU")]
        public void GetSepcifications_Should_Include_Specifications_In_Product(string productNumber)
        {
            string[] productNumbers = new string[] {productNumber};
            bool inlcudeSubProducts = true;
            string[] specNames = new string[] { };

            IEnumerable<ProductModel> models = productService.GetProductModels(user, productNumbers) as IEnumerable<ProductModel>;

            productService.GetSpecifications(models, specNames, inlcudeSubProducts);

           foreach(var item in models)
            {
                Assert.That(item.Specifications, Is.Not.EqualTo(null));
                Assert.That(item.Specifications.All.Count(), Is.GreaterThanOrEqualTo(0));
            }
        }

        [Test]
        [Category("ProductServices")]
        [TestCase("FTXN09NMVJU")]
        public void GetProductNotes_Should_Include_Notes_In_Product(string productNumber)
        {
            var model = productService.GetProductModel(productNumber);
            List<ProductModel> models = new List<ProductModel>();
            if(model != null)
            {
                models.Add(model);
                productService.GetProductNotes(models);

                foreach(var item in models)
                {
                    Assert.That(item.StandardFeatures, Is.Not.EqualTo(null));
                    Assert.That(item.Benefits, Is.Not.EqualTo(null));
                    Assert.That(item.Notes, Is.Not.EqualTo(null));

                    Assert.That(item.Notes.Count, Is.GreaterThanOrEqualTo(1));
                    Assert.That(item.StandardFeatures.Count, Is.GreaterThanOrEqualTo(1));
                    Assert.That(item.Benefits.Count, Is.GreaterThanOrEqualTo(1));
                }
            }
        }

        [Test]
        [Category("ProductServices")]
        [TestCase("FTXN09NMVJU")]
        public void GetProductSubmittalImage_Should_Include_SubmittalImage_In_Product(string productNumber)
        {
            var model = productService.GetProductModel(productNumber);
            List<ProductModel> models = new List<ProductModel>();
            if (model != null)
            {
                models.Add(model);
                productService.GetProductNotes(models);

                foreach (var item in models)
                {
                    Assert.That(item.DimensionalDrawing, Is.Not.EquivalentTo(null));
                    Assert.That(item.Logos, Is.Not.EqualTo(null));
                    Assert.That(item.DimensionalDrawing.FileName, Is.Not.EqualTo(null));
                    Assert.That(item.Logos.Count, Is.GreaterThanOrEqualTo(1));
                }
            }
        }

        [Test]
        [Category("ProductServices")]
        [TestCase(100000004)]
        public void GetProductFamilyTabsModel_Should_Return_ListOfTabs(int productFamilyId)
        {
            List<TabModel> tabs = productService.GetProductFamilyTabsModel(user, productFamilyId);
            Assert.That(tabs, Is.Not.EqualTo(null));
            Assert.That(tabs.Count, Is.GreaterThan(0));

            Assert.That(tabs.Any(t => t.Description.Contains("VRV")), Is.EqualTo(true));
        }
        [Test]
        [Category("ProductServices")]
        public void GetProductImages_Should_Include_Images_In_Product(string productNumber)
        {
            var model = productService.GetProductModel(_productNumber);
            List<ProductModel> models = new List<ProductModel>();

            if (model != null)
            {
                models.Add(model);

                productService.GetProductImages(models);
            }

            Assert.That(models.Any(m => m.Image.HasImage), Is.EqualTo(true));
                
        }
        public HttpContextBase FakeHttpContext(string httpMethod)
        {
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new MockHttpSession();
            var server = new Mock<HttpServerUtilityBase>();

            context.Setup(ctx => ctx.Request).Returns(request.Object);
            context.Setup(ctx => ctx.Response).Returns(response.Object);
            context.Setup(ctx => ctx.Session).Returns(session);
            context.Setup(ctx => ctx.Server).Returns(server.Object);
            context.Setup(ctx => ctx.Request.HttpMethod).Returns(httpMethod);
            return context.Object;
        }
    }

    
}
