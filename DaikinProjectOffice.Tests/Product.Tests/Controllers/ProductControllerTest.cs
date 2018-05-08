using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Common;
using DPO.Domain;
using DPO.Common;
using DPO.Model.Light;
using DPO.Services.Light;
using Resources = DPO.Resources;
using DPO.Data;
using DPO.Web.Controllers;
using System.Web;
using System.Web.Mvc;
using System.Web.Configuration;
using System.Web.Hosting;
using System.Web.SessionState;
using System.IO;
using System.Web.Routing;
using Moq;

namespace DaikinProjectOffice.Tests
{
    [TestFixture]
    class ProductControllerTest : TestAdmin
    {
        ProductServices productService = null;
        ProductDashboardController sut = new ProductDashboardController();
        AccountServices accountService = null;
        UserSessionModel user = null;
        RouteData routeData;
        public ProductControllerTest() {
            productService = new ProductServices(this.TContext);
            accountService = new AccountServices();
            user = accountService.GetUserSessionModel("test@123.com").Model as UserSessionModel;
            routeData = new RouteData();
        }

        [Test]
        [Category("ProductController")]
        [TestCase("GET")]
        public void Should_render_products_view(string httpMethod) {
            //Arrange
            SetUpProductControllerForTesting(httpMethod);

            ProductsModel model = new ProductsModel();
            model.ProductFamilyId = (int?)ProductFamilyEnum.VRV;
            model.ViewOption = ProductsModel.ProductViewOption.List;
          
            //Act
            var result = sut.Products(model) as ViewResult;

            //Assert
            Assert.That(result.Model, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo("Products"));
        }



        private void SetUpProductControllerForTesting(string httpMethod)
        {
            //Arrange
            sut.CurrentUser = user;

            var httpContextMock = FakeHttpContext(httpMethod);
            var controllerMock = new Mock<ControllerBase>(MockBehavior.Loose);

            var routeData = new RouteData();

            var controllerContext = new ControllerContext(httpContextMock, routeData, controllerMock.Object);
            sut.ControllerContext = controllerContext;
            

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
