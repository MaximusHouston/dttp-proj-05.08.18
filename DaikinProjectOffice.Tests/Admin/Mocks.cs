//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================

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
using DPO.Web.Controllers;
using Moq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Specialized;
using System.Web.Routing;
using NUnit.Framework;
using NUnit.Common;

namespace DaikinProjectOffice.Tests
{
   public partial class TestAdmin
   {

      public HttpContextBase FakeHttpContext()
      {
         var browser = new Mock<HttpBrowserCapabilitiesBase>(MockBehavior.Strict);
         var context = new Mock<HttpContextBase>(MockBehavior.Strict);
         var request = new Mock<HttpRequestBase>(MockBehavior.Strict);
         var response = new Mock<HttpResponseBase>(MockBehavior.Strict);
         var session = new Mock<HttpSessionStateBase>(MockBehavior.Strict);
         var server = new Mock<HttpServerUtilityBase>(MockBehavior.Strict);
         var cookies = new HttpCookieCollection() { new HttpCookie("ResponseCookieTest") };
         var items = new ListDictionary();

         request.Setup(r => r.Cookies).Returns(cookies);
         response.Setup(r => r.Cookies).Returns(cookies);

         context.Setup(ctx => ctx.Items).Returns(items);

         context.SetupGet(ctx => ctx.Request).Returns(request.Object);
         context.SetupGet(ctx => ctx.Response).Returns(response.Object);
         context.SetupGet(ctx => ctx.Session).Returns(session.Object);
         context.SetupGet(ctx => ctx.Server).Returns(server.Object);

         request.Setup(r => r.ValidateInput());
         request.Setup(r => r.UserAgent).Returns("Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.64 Safari/537.11");
         browser.Setup(b => b.IsMobileDevice).Returns(false); 
         request.Setup(r => r.Browser).Returns(browser.Object);

         session.Setup(x => x["UserSessionModel"]).Returns(new UserSessionModel { Email = "Reg1@@somewhere.com" } );
         session.Setup(m => m.SessionID).Returns(Guid.NewGuid().ToString());

         request.Setup(m => m.UserHostAddress).Returns("127.0.0.1");
         request.Setup(m => m.ApplicationPath).Returns("/");
         request.Setup(m => m.AppRelativeCurrentExecutionFilePath).Returns("/");
         request.Setup(m => m.PathInfo).Returns(string.Empty);
         request.Setup(m => m.Form).Returns(new NameValueCollection());
         request.Setup(m => m.QueryString).Returns(new NameValueCollection());

         return context.Object;

      }

      public T SetFakeController<T>(T controller, string controllerName, string action)
      {
         var httpContext = FakeHttpContext();

         var routeData = new RouteData();
         routeData.Values.Add("controller", controllerName);
         routeData.Values.Add("action", action);

         ControllerContext context = new ControllerContext(httpContext, routeData, controller as Controller);

         (controller as Controller).ControllerContext = context;

         return controller;
      }
   }
}