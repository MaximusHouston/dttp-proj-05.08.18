using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Common;
using NUnit.Engine;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Support.Events;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.PageObjects;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Firefox;
using System.Threading;
using DPO.Common;
using DPO.Data;
using DPO.Domain;
using DPO.Model.Light;
using DPO.Services.Light;
using System.Globalization;
using RazorGenerator.Mvc;
using RazorGenerator.Testing;

namespace DaikinProjectOffice.Tests
{
    [TestFixture]
    public class ProjectViewTest : TestAdmin
    {
        private IWebDriver driver;
        private ProjectServices projectService;
        private AccountServices accountService;
        private OrderServices orderService;
        private ProjectModel projectVM;
        private UserSessionModel user;
        private ProjectServiceLight projectServiceLight;
        private long projectId;
        private long quoteId;
        private OrderServiceLight orderServiceLight;
        private Project project;
        private AddressServices addressService;

        public ProjectViewTest()
        {

            driver = new ChromeDriver(@"C:\Q2O\Source\Iterations\Iteration 8\Daikin Project Office\DaikinProjectOffice.Tests\libraries");

            projectService = new ProjectServices();
            accountService = new AccountServices();
            orderService = new OrderServices();
            projectVM = new ProjectModel();
            projectServiceLight = new ProjectServiceLight();
            orderServiceLight = new OrderServiceLight();
            addressService = new AddressServices();

            user = accountService.GetUserSessionModel("User15@test.com").Model as UserSessionModel;

            projectId = this.db.Context.Projects
                .Where(p => p.ProjectStatusTypeId == ProjectStatusTypeEnum.Open && 
                       p.OwnerId == user.UserId).OrderByDescending(p => p.ProjectTypeId)
                       .Select(p => p.ProjectId).FirstOrDefault();

            project = this.db.Context.Projects
                               .Where(p => p.OwnerId == user.UserId &&
                               p.ProjectStatusTypeId == ProjectStatusTypeEnum.Open
                               ).OrderByDescending(p => p.ProjectTypeId)
                              .FirstOrDefault();

            quoteId = this.db.Context.Quotes.Where(q => q.ProjectId == projectId).OrderByDescending(q => q.QuoteId).Select(q => q.QuoteId).FirstOrDefault();

            projectVM = projectService.GetProjectModel(user, projectId).Model as ProjectModel;
        }

        [Test]
        [Category("ProjectView")]
        [TestCase("HasViewProjectPermission")]
        [TestCase("NotHasViewProjectPermission")]
        public void TestProjectView_ProjectTabPartial_ShouldRenderProjectTabsBasedOnPermission(string testCase)
        {
          //  ProjectModel projectModel = projectService.GetProjectModel(user, projectId).Model as ProjectModel;

          //  if (testCase == "HasViewProjectPermission")
          //  {
          //      driver.Navigate().GoToUrl(@"http://tstsysdcity2/projectdashboard/Project/" + projectId + "#!/");

          //      Thread.Sleep(25000);//This is make sure we completed load the form before we manipulate DOM

          //      ReadOnlyCollection<IWebElement> projectTabs = driver
          //                           .FindElements(By.ClassName("tab-bar"));

          //      ReadOnlyCollection<IWebElement> tabsList = projectTabs[0]
          //                        .FindElements(By.TagName("li"));

          //      List<string> values = new List<string>();
          //      foreach (var item in tabsList)
          //      {
          //          if (!string.IsNullOrWhiteSpace(item.Text))
          //              values.Add(item.Text);
          //      }

          //      if (values.Count() > 1)
          //      {
          //          for (int a = 0; a < values.Count; a++)
          //          {
          //              if (a != values.Count() - 1)
          //                  Assert.That(values[a], Is.Not.EqualTo(null));
          //          }

          //          for (int i = 0; i < values.Count(); i++)
          //          {

          //              switch (i)
          //              {
          //                  case 0:
          //                      Assert.That(values[i].ToLower(), Is.EqualTo("overview"));
          //                      break;
          //                  case 1:
          //                      Assert.That(values[i].ToLower(), Is.EqualTo("projects"));
          //                      break;
          //                  case 2:
          //                      Assert.That(values[i].ToLower(), Is.EqualTo("tools"));
          //                      break;
          //                  case 3:
          //                      Assert.That(values[i].ToLower(), Is.EqualTo("browse products"));
          //                      break;
          //                  case 4:
          //                      Assert.That(values[i].ToLower(), Is.EqualTo("orders"));
          //                      break;

          //              }
          //          }
          //     }
          //}
            
            //if(testCase == "HasViewProjectPermission")
            //{
            //    var sut = new ASP._Views_ProjectDashboard__ProjectTabsPartial_cshtml();
            //    sut.ViewData["CurrentUser"] = user;

            //    ProjectModel model = new ProjectModel();
            //    model.CurrentUser = user;

            //    HtmlAgilityPack.HtmlDocument doc = sut.RenderAsHtml(model);
              
            //    string[] stringSeparators = new string[] { "\r\n" };
            //    List<string> lines = RemoveSpecificCharacterFromString(stringSeparators, doc);

            //    Assert.That(lines.Count(), Is.EqualTo(5));
            //    Assert.That(lines[0].ToLower().Contains("overview"), Is.EqualTo(true));
            //    Assert.That(lines[1].ToLower().Contains("projects"), Is.EqualTo(true));
            //    Assert.That(lines[2].ToLower().Contains("tools"), Is.EqualTo(true));
            //    Assert.That(lines[3].ToLower().Contains("browse products"), Is.EqualTo(true));
            //    Assert.That(lines[4].ToLower().Contains("orders"), Is.EqualTo(true));
            //}
            //if(testCase == "NotHasViewProjectPermission")
            //{
            //    user.SystemAccesses.Remove(SystemAccessEnum.ViewProject);
            //    var sut = new ASP._Views_ProjectDashboard__ProjectTabsPartial_cshtml();
            //    sut.ViewData["CurrentUser"] = user;

            //    ProjectModel model = new ProjectModel();
            //    model.CurrentUser = user;

            //    HtmlAgilityPack.HtmlDocument doc = sut.RenderAsHtml(model);
               
            //    string[] stringSeparators = new string[] { "\r\n" };
            //    List<string> lines = RemoveSpecificCharacterFromString(stringSeparators, doc);

            //    Assert.That(lines.Count(), Is.EqualTo(3));
            //    Assert.That(lines[0].ToLower().Contains("tools"), Is.EqualTo(true));
            //    Assert.That(lines[1].ToLower().Contains("browse products"), Is.EqualTo(true));
            //    Assert.That(lines[2].ToLower().Contains("orders"), Is.EqualTo(true));
            //}
        }

        /// <summary>
        /// Test ProjectButtonBar PartialView.
        /// </summary>
        /// <param name="testValue1">UndeletePermission </param>
        /// <param name="testValue2">ProjectModel.Delete  </param>
        /// <param name="testValue3">ProjectModel.IsTransfer</param>
        [Test]
        [Category("ProjectView")]
        [TestCase("HasUndeleteProjectPermission", "ModelDeletedIsTrue",false)]
        [TestCase("UndeleteProjectPermissionIsFalse","ModelDeletedIsFalse",true)]
        [TestCase("UndeleteProjectPermissionIsFalse", "ModelDeletedIsFalse", false)]
        public void TestProjectView_ProjectButtonBar_ShouldRenderAllButtonsBelongToProject(string testValue1, string testValue2, bool testValue3)
        {
            if(testValue1 == "HasUndeleteProjectPermission" && testValue2 == "ModelDeletedIsTrue")
            {
                var sut = new ASP._Views_ProjectDashboard_ProjectButtonBar_cshtml();
                sut.ViewData["CurrentUser"] = user;

                ProjectModel model = new ProjectModel();
                model.CurrentUser = user;
                model.Deleted = true;

                HtmlAgilityPack.HtmlDocument doc = sut.RenderAsHtml(model);
            
                string[] stringSeparators = new string[] { "\n","\r" };
                List<string> lines = RemoveSpecificCharacterFromString(stringSeparators, doc);

                Assert.That(lines.Count(), Is.EqualTo(1));
                Assert.That(lines[0].ToLower().Contains("undelete project"), Is.EqualTo(true));
            }
            if(testValue1 == "UndeleteProjectPermissionIsFalse" && testValue2 == "ModelDeletedIsFalse")
            {
                if(testValue3 == true)
                {
                    user.SystemAccesses.Remove(SystemAccessEnum.UndeleteProject);

                    var sut = new ASP._Views_ProjectDashboard_ProjectButtonBar_cshtml();
                    sut.ViewData["CurrentUser"] = user;

                    ProjectModel model = new ProjectModel();
                    model.CurrentUser = user;
                    model.Deleted = false;
                    model.IsTransferred = testValue3;

                    HtmlAgilityPack.HtmlDocument doc = sut.RenderAsHtml(model);
                   
                    string[] stringSeparators = new string[] { "\r\n" };
                    List<string> lines = RemoveSpecificCharacterFromString(stringSeparators, doc);

                    Assert.That(lines.Count(), Is.EqualTo(1));
                  
                    Assert.That(lines[0].ToLower().Contains("delete project"), Is.EqualTo(true));
                }
                else
                {
                    user.SystemAccesses.Remove(SystemAccessEnum.UndeleteProject);
                    user.SystemAccesses.Remove(SystemAccessEnum.EditProject);

                    var sut = new ASP._Views_ProjectDashboard_ProjectButtonBar_cshtml();
                    sut.ViewData["CurrentUser"] = user;

                    ProjectModel model = new ProjectModel();
                    model.CurrentUser = user;
                    model.Deleted = false;
                    model.IsTransferred = testValue3;

                    HtmlAgilityPack.HtmlDocument doc = sut.RenderAsHtml(model);
                  
                    string[] stringSeparators = new string[] { "\r\n" };
                    List<string> lines = RemoveSpecificCharacterFromString(stringSeparators, doc);

                    Assert.That(lines.Count(), Is.EqualTo(1));
                    Assert.That(lines[0].ToLower().Contains("export"), Is.EqualTo(true));

                    /* testing the project's button */
                    #region testProject'sButtons
                   
                    driver.Navigate().GoToUrl("http://tstsysdcity2/Account/Login");
                    NavigateToLogin(user,driver);
                    driver.Navigate().GoToUrl("http://User15@test.com:123456@tstsysdcity2/projectdashboard/Project/" + projectId);

                    List<IWebElement> projectButtons = driver.FindElement(By.ClassName("btn-bar"))
                                                         .FindElements(By.TagName("a")).ToList();
                    if(projectButtons.Count > 1)
                    {
                        IWebElement projectExportBtn = projectButtons.First();
                        Assert.That(projectExportBtn.Text.ToLower(), Is.EqualTo("export"));
                        Assert.That(projectExportBtn.GetAttribute("href").Contains("/projectdashboard/ProjectExport"),Is.EqualTo(true));
                        projectExportBtn.Click();

                        Thread.Sleep(5000);
                      
                        //IWebElement exportProjectStatus = driver.FindElement(By.CssSelector("#exportProjectStatus"));
                        //Assert.That(exportProjectStatus.GetAttribute("status"), Is.EqualTo("export project success"));

                        IWebElement projectDeleteBtn = projectButtons.Last();
                        Assert.That(projectDeleteBtn.Text.ToLower(), Is.EqualTo("delete project"));
                        Assert.That(projectDeleteBtn.GetAttribute("data-sc-ajaxpost").Contains("/ProjectDashboard/ProjectDelete"), Is.EqualTo(true));
                        //projectDeleteBtn.Click();

                        Thread.Sleep(5000);
                    }

                    #endregion
                }
            }

        }

        /*
        /// <summary>
        /// Test the ActiveQuoteInfoBar Partial View.
        /// </summary>
        /// <param name="value1">EditProject Permission</param>
        /// <param name="value2">IActiveQuoteInfoBarModel.Deleted</param>
        /// <param name="value3">IActiveQuoteInfoBarModel.IsTransfer</param>
        /// <param name="value4">IActiveQuoteInfoBarModel.HasDAR</param>
        /// <param name="value5">IActiveQuoteInfoBarModel.ProjectStatusTypeId</param>
        /// <param name="value6">IActiveQuoteInfoBarModel.ProjectStatusTypeId</param>
        /// <param name="value7">IActiveQuoteInfoBarModel.ActiveQuoteSummary.OrderStatusTypeId</param>
        /// <param name="value8">IActiveQuoteInfoBarModel.ActiveQuoteSummary.OrderStatusTypeId</param>
        /// <param name="value9">ShowPrice Permission</param>
        //[Test]
        //[Category("ProjectView")]
        //[TestCase(true, false,false,4,2,3,2,0,true )]
        //[TestCase(true, false, false, 4, 2, 3, 2, 1, true)]
        //[TestCase(true, false, false, 4,2,3,2,1,false)]
        //public void TestProjectView_ActiveQuoteInfoBar_ShouldRenderAllButtonsBelongToQuote(
        // bool value1, bool value2, bool value3, int value4, int value5, int value6, int value7, int value8, bool value9 )
        //{
        //    if(value1 == true && value2 == false && value3 == false 
        //        && value4 == 4 && value5 == 2 
        //        && value6 == 3 && value7 == 2 
        //        && value8 == 0 )
        //    {
        //        var sut = new ASP._Views_ProjectDashboard_ActiveQuoteInfoBar_cshtml();
        //        sut.ViewData["CurrentUser"] = user;

        //        ProjectModel model = new ProjectModel();
        //        model.CurrentUser = user;
        //        model.Deleted = value2;
        //        model.IsTransferred = value3;
        //        model.ProjectStatusTypeId = 3;
        //        model.ActiveQuoteSummary.OrderStatusTypeId = 1;
        //        model.ActiveQuoteSummary.QuoteId = value8;

        //        HtmlAgilityPack.HtmlDocument doc = sut.RenderAsHtml(model);

        //        string[] stringSeparators = new string[] { "\n", "\r" };
        //        List<string> lines = RemoveSpecificCharacterFromString(stringSeparators, doc);

        //        Assert.That(lines.Any( i => i.ToLower().Contains("active quote")), Is.EqualTo(true));
        //        Assert.That(lines.Any(i => i.ToLower().Contains("add new quote")), Is.EqualTo(true));
        //        Assert.That(lines.Any(i => i.ToLower().Contains("no active quote")), Is.EqualTo(true));
        //    }
        //    if (value1 == true && value2 == false && value3 == false
        //       && value4 == 4 && value5 == 2
        //       && value6 == 3 && value7 == 2
        //       && value8 == 1)
        //    {
                
        //        driver.Navigate().GoToUrl("http://tstsysdcity2/Account/Login");

        //        if (value9 == true) // if user has ShowPrice Permission
        //        {
        //            //get the projectId of the project that has Dar
        //            long _projectId = this.db.Context.Projects.Where(p => p.OwnerId == user.UserId &&
        //                                                             p.ProjectStatusTypeId == ProjectStatusTypeEnum.Open)
        //                                                             .OrderByDescending(p => p.ProjectId)
        //                                                             .Select(p => p.ProjectId).FirstOrDefault();

        //            NavigateToLogin(user,driver);
        //            driver.Navigate().GoToUrl("http://User15@test.com:123456@tstsysdcity2/projectdashboard/Project/" + _projectId);

        //            ProjectModel model = projectService.GetProjectModel(user, _projectId).Model as ProjectModel;

        //            model.CurrentUser = user;
        //            model.Deleted = value2;
        //            model.IsTransferred = value3;
        //            model.ProjectStatusTypeId = 3;
        //            model.ActiveQuoteSummary.OrderStatusTypeId = 1;
        //            model.ActiveQuoteSummary.QuoteId = value8;

        //            model.ActiveQuoteSummary.Alert = true;
        //            user.ShowPrices = true;
        //            model.ActiveQuoteSummary.Revision = 1234;

        //            var sut = new ASP._Views_ProjectDashboard_ActiveQuoteInfoBar_cshtml();
        //            model.ActiveQuoteSummary.Alert = true;
        //            sut.ViewData["CurrentUser"] = user;

        //            var view = sut.RenderAsHtml(model);

        //            string[] stringSeparators = new string[] { "\n", "\r" };
        //            List<string> lines = RemoveSpecificCharacterFromString(stringSeparators, view);

        //            Assert.That(lines.Any(i => i.ToLower().Contains("net price")), Is.EqualTo(true));
        //            Assert.That(lines.Any(i => i.ToLower().Contains("list price")), Is.EqualTo(true));
        //            Assert.That(lines.Any(i => i.ToLower().Contains("total price")), Is.EqualTo(true));

        //            //Get the Project that has Commission Request
        //            var project = (from p in this.db.Context.Projects
        //                           join q in this.db.Context.Quotes
        //                           on p.ProjectId equals q.ProjectId
        //                           where p.OwnerId == user.UserId &&
        //                           p.ProjectStatusTypeId == ProjectStatusTypeEnum.Open &&
        //                           q.IsCommission == true &&
        //                           q.CommissionRequestId != 0
        //                           select new
        //                           {
        //                               ProjectId = p.ProjectId,
        //                               QuoteId = q.QuoteId,
        //                               CommissionRequestId = q.CommissionRequestId
        //                           }).FirstOrDefault();

        //            model = projectService.GetProjectModel(user, project.ProjectId).Model as ProjectModel;

        //            model.CurrentUser = user;
        //            model.Deleted = value2;
        //            model.IsTransferred = value3;
        //            model.ProjectStatusTypeId = 3;
        //            model.ActiveQuoteSummary.OrderStatusTypeId = 1;
        //            model.ActiveQuoteSummary.QuoteId = value8;

        //            model.ActiveQuoteSummary.Alert = true;
        //            user.ShowPrices = true;
        //            model.ActiveQuoteSummary.Revision = 1234;
        //            model.IsCommission = true;

        //             sut = new ASP._Views_ProjectDashboard_ActiveQuoteInfoBar_cshtml();
        //            model.ActiveQuoteSummary.Alert = true;
        //            sut.ViewData["CurrentUser"] = user;

        //            view = sut.RenderAsHtml(model);

        //            stringSeparators = new string[] { "\n", "\r" };
        //            lines = RemoveSpecificCharacterFromString(stringSeparators, view);

        //            //Test the Columns render for ActiveQuoteSummaryTable 
        //            Assert.That(lines.Any(i => i.ToLower().Contains("total list")), Is.EqualTo(true));
        //            Assert.That(lines.Any(i => i.ToLower().Contains("total net")), Is.EqualTo(true));
        //            Assert.That(lines.Any(i => i.ToLower().Contains("commission amount")), Is.EqualTo(true));
        //            Assert.That(lines.Any(i => i.ToLower().Contains("net material value")), Is.EqualTo(true));

        //        }
        //        if (value9 == false) // User does not have ShowPricing Permission
        //        {
        //            bool responseFromDriver = true;

        //            try
        //            {
        //                driver.FindElement(By.Id("logoutForm"));
        //                driver.FindElement(By.Id("loginLink"));
        //                responseFromDriver = true;
        //            }
        //            catch
        //            {
        //                responseFromDriver = false;
        //            }

        //            if (responseFromDriver)
        //            {
        //                driver.FindElement(By.Id("logoutForm")).Submit();
        //                driver.FindElement(By.Id("loginLink")).Click();

        //            }

        //            var username = driver.FindElement(By.Id("Email"));
        //            var password = driver.FindElement(By.Id("Password"));

        //            var userWithoutShowPricingPermission = (from b in this.db.Context.Businesses
        //                                                    join u in this.db.Context.Users
        //                                                    on b.BusinessId equals u.BusinessId
        //                                                    where b.ShowPricing == false && u.Email.Contains("user")
        //                                                    select new 
        //                                                    {
        //                                                        UserId = u.UserId,
        //                                                        Email = u.Email
        //                                                    }).OrderByDescending(u => u.UserId).FirstOrDefault();

        //            string userId = userWithoutShowPricingPermission.Email;
        //            string userPass = userId.Contains("user30") ? "123456": "test";

        //            username.SendKeys(userId);
        //            password.SendKeys(userPass);

        //            driver.FindElement(By.Id("loginButton")).Click();

        //            //get the projectId of the project that has Dar
        //            long _projectId = this.db.Context.Projects.Where(p => p.OwnerId == userWithoutShowPricingPermission.UserId &&
        //                                                             p.ProjectStatusTypeId == ProjectStatusTypeEnum.Open)
        //                                                             .OrderByDescending(p => p.ProjectId)
        //                                                             .Select(p => p.ProjectId).FirstOrDefault();

        //            driver.Navigate().GoToUrl("http://" + userId + ":" 
        //                                      + userPass + "@tstsysdcity2/projectdashboard/Project/" + _projectId);

        //            ReadOnlyCollection<IWebElement> ActiveQuoteSummaryTableRows = null;
        //            ReadOnlyCollection<IWebElement> ActiveQuoteSummaryTableColumns = null;

        //            try
        //            {
        //                ReadOnlyCollection<IWebElement> activeQuoteSummaryTable = driver
        //                               .FindElement(By.ClassName("active-quote-bar"))
        //                               .FindElements(By.TagName("table"));
        //                ActiveQuoteSummaryTableRows = activeQuoteSummaryTable[0].FindElements(By.TagName("tr"));
        //                ActiveQuoteSummaryTableColumns = ActiveQuoteSummaryTableRows[0].FindElements(By.TagName("td"));
        //                responseFromDriver = true;
        //            }
        //            catch
        //            {
        //                responseFromDriver = false;
        //            }
                   
        //            if(responseFromDriver)
        //            {
        //                //make sure no price columns will be show on Table
        //                Assert.That(ActiveQuoteSummaryTableColumns.Count, Is.EqualTo(2));
        //                Assert.That(ActiveQuoteSummaryTableColumns[0].Text, Is.EqualTo("Revision"));
        //                Assert.That(ActiveQuoteSummaryTableColumns[1].Text, Is.EqualTo("Date"));
        //            }

        //        }

        //    }
        //}

    */

        /// <summary>
        /// Test the ProjectDetail partial, make sure it render all project information requirements
        /// </summary>
        [Test]
        [Category("ProjectView")]
        public void TestProjectView_ProjectDetails_ShouldRenderProjectInfoInDetails()
        {
            NavigateToLogin(user, driver); 
           
            driver.Navigate().GoToUrl("http://User15@test.com:123456@tstsysdcity2/projectdashboard/Project/" + project.ProjectId);

            IWebElement[] projectDetailsColumns = driver.FindElements(By.ClassName("details-list")).ToArray();

            IWebElement projectDetailsFirstColumn = projectDetailsColumns[0].FindElements(By.TagName("ul")).FirstOrDefault();

            if (projectDetailsFirstColumn.Size != null) // make sure element existed
            {
                IWebElement[] FirstColumnItems = projectDetailsFirstColumn.FindElements(By.TagName("li")).ToArray();

                if (FirstColumnItems.Count() > 0)
                {
                    Assert.That(FirstColumnItems.Count, Is.EqualTo(6));

                    //retrive the text value of the seond paragrah for each li element
                    IWebElement projectNameValue = FirstColumnItems[0].FindElements(By.TagName("p")).Last();
                    IWebElement projectIdValue = FirstColumnItems[1].FindElements(By.TagName("p")).Last();
                    IWebElement projectDateValue = FirstColumnItems[2].FindElements(By.TagName("p")).Last();
                    IWebElement projectBidDate = FirstColumnItems[3].FindElements(By.TagName("p")).Last();
                    IWebElement projectEstimateCloseValue = FirstColumnItems[4].FindElements(By.TagName("p")).Last();
                    IWebElement projectEstimateDeliveryValue = FirstColumnItems[5].FindElements(By.TagName("p")).Last();

                    Assert.That(projectNameValue.Text, Is.EqualTo(project.Name));
                    Assert.That(projectIdValue.Text, Is.EqualTo(project.ProjectId.ToString()));
                    Assert.That(projectDateValue.Text, Is.EqualTo(project.ProjectDate.ToShortDateString()));
                    Assert.That(projectBidDate.Text, Is.EqualTo(project.BidDate.ToString()));
                    Assert.That(projectEstimateCloseValue.Text, Is.EqualTo(project.EstimatedClose.ToShortDateString()));
                    Assert.That(projectEstimateDeliveryValue.Text, Is.EqualTo(project.EstimatedDelivery.ToShortDateString()));
                }
            }

            IWebElement projectDetailsSecondColumn = projectDetailsColumns[1].FindElements(By.TagName("ul")).FirstOrDefault();

            if (projectDetailsSecondColumn.Size != null) // make sure element existed
            {
                IWebElement[] SecondColumnItems = projectDetailsSecondColumn.FindElements(By.TagName("li")).ToArray();

                if (SecondColumnItems.Count() > 0)
                {
                    Assert.That(SecondColumnItems.Count, Is.EqualTo(6));

                    //retrive the text value of the seond paragrah for each li element
                    IWebElement projectConstructionTypeValue = SecondColumnItems[0].FindElements(By.TagName("p")).Last();
                    IWebElement projectStatusValue = SecondColumnItems[1].FindElements(By.TagName("p")).Last();
                    IWebElement projectTypeValue = SecondColumnItems[2].FindElements(By.TagName("p")).Last();
                    IWebElement projectOpenStatusValue = SecondColumnItems[3].FindElements(By.TagName("p")).Last();
                    IWebElement projectVerticalMarketValue = SecondColumnItems[4].FindElements(By.TagName("p")).Last();
                    IWebElement projectNotesValue = SecondColumnItems[5].FindElements(By.TagName("p")).Last();

                    Assert.That(projectConstructionTypeValue.Text, 
                        Is.EqualTo(((ConstructionTypeEnum)project.ConstructionTypeId).ToString()));
                    Assert.That(projectStatusValue.Text, 
                        Is.EqualTo(((ProjectStatusTypeEnum)project.ProjectStatusTypeId).ToString()));
                    Assert.That(projectTypeValue.Text.Replace("/",""), 
                        Is.EqualTo(((ProjectTypeEnum)project.ProjectTypeId).ToString()));
                    Assert.That(projectOpenStatusValue.Text.Contains(((ProjectOpenStatusTypeEnum)project.ProjectOpenStatusTypeId).ToString()), 
                        Is.EqualTo(true));
                    Assert.That(projectVerticalMarketValue.Text, 
                        Is.EqualTo(((VerticalMarketTypeEnum)project.VerticalMarketTypeId).ToString()));
                    Assert.That(projectNotesValue.Text, Is.EqualTo((project.Description != null)? project.Description : string.Empty));

                    if(project.ProjectStatusNotes != null)
                    {
                        IWebElement projectStatusNoteValue = SecondColumnItems[6].FindElements(By.TagName("p")).Last();
                        Assert.That(projectStatusNoteValue.Text.Contains(project.ProjectStatusNotes), Is.EqualTo(true));
                    }
                }
            }

        }


        /// <summary>
        /// This test method will make sure the Engineer Address, Customer Address,
        /// Seller Address and ShipToLocation Address will render correctly on the
        /// ProjectAddressDetails partial view
        /// </summary>
        [Test]
        [Category("ProjectView")]
        public void TestProjectView_ProjectAddressDetails_ShouldRenderProjectAddresses()
        {
            NavigateToLogin(user,driver);

            //get any projectId that has at least one Address 
            var result = (from p in this.db.Context.Projects
                          join add in this.db.Context.Addresses
                          on p.ShipToAddressId equals add.AddressId
                          where p.OwnerId == user.UserId &&
                          p.ProjectStatusTypeId == ProjectStatusTypeEnum.Open
                          orderby p.ProjectId descending
                          select new
                          {
                              ProjectId = p.ProjectId
                          }).FirstOrDefault();

            driver.Navigate().GoToUrl("http://User15@test.com:123456@tstsysdcity2/projectdashboard/Project/" + result.ProjectId);

            IWebElement[] addressColumns = driver.FindElements(By.ClassName("details-descript")).ToArray();

            IWebElement[] engineerValues = addressColumns[0].FindElements(By.TagName("p")).ToArray();

            ProjectModel model = projectService.GetProjectModel(user, result.ProjectId).Model as ProjectModel;

            #region engineerAddress

            if (model.EngineerName != null)
                Assert.That(engineerValues[1].Text, Is.EqualTo(model.EngineerName));

            if (model.EngineerBusinessName != null)
                Assert.That(engineerValues[2].Text, Is.EqualTo(model.EngineerBusinessName));

            if (model.EngineerAddress != null)
            {
                AddressModel engineerAddress = addressService.GetAddressModel(user, model.EngineerAddress) as AddressModel;

                if (engineerAddress.AddressLine1 != null)
                    Assert.That(engineerValues[3].Text, Is.EqualTo(engineerAddress.AddressLine1));

                if (engineerAddress.Location != null)
                    Assert.That(engineerValues[6].Text, Is.EqualTo(engineerAddress.Location));

                if (engineerAddress.StateId != null)
                {
                    string state = projectService.GetStateByStateId(engineerAddress.StateId.Value);
                    Assert.That(engineerValues[7].Text, Is.EqualTo(state));
                }
                if (engineerAddress.PostalCode != null)
                    Assert.That(engineerValues[8].Text, Is.EqualTo(engineerAddress.PostalCode.ToString()));
            }
            #endregion

            #region DealerAddress

            IWebElement[] customerValues = addressColumns[1].FindElements(By.TagName("p")).ToArray();

            if (model.DealerContractorName != null)
                Assert.That(customerValues[1].Text, Is.EqualTo(model.DealerContractorName));

            if (model.CustomerName != null)
                Assert.That(customerValues[2].Text, Is.EqualTo(model.CustomerName));

            if (model.CustomerAddress != null && model.CustomerName != null)
            {
                AddressModel customerAddress = addressService.GetAddressModel(user, model.CustomerAddress) as AddressModel;

                if (customerAddress.AddressLine1 != null)
                    Assert.That(customerValues[3].Text, Is.EqualTo(customerAddress.AddressLine1));

                if (customerAddress.Location != null)
                    Assert.That(customerValues[6].Text, Is.EqualTo(customerAddress.Location));

                if (customerAddress.StateId != null)
                {
                    string state = projectService.GetStateByStateId(customerAddress.StateId.Value);
                    Assert.That(customerValues[7].Text, Is.EqualTo(state));
                }
                if (customerAddress.PostalCode != null)
                    Assert.That(customerValues[8].Text, Is.EqualTo(customerAddress.PostalCode.ToString()));
            }
            #endregion

            #region sellerAddress
            IWebElement[] sellerValues = addressColumns[2].FindElements(By.TagName("p")).ToArray();

            if (model.SellerName != null)
                Assert.That(sellerValues[1].Text, Is.EqualTo(model.SellerName));

            if (model.SellerAddress != null)
            {
                AddressModel sellerAddress = addressService.GetAddressModel(user, model.SellerAddress) as AddressModel;

                if (sellerAddress.AddressLine1 != null)
                    Assert.That(sellerValues[2].Text, Is.EqualTo(sellerAddress.AddressLine1));

                if (sellerAddress.Location != null)
                    Assert.That(sellerValues[5].Text, Is.EqualTo(sellerAddress.Location));

                if (sellerAddress.StateId != null)
                {
                    string state = projectService.GetStateByStateId(sellerAddress.StateId.Value);
                    Assert.That(sellerValues[6].Text, Is.EqualTo(state));
                }
                if (sellerAddress.PostalCode != null)
                    Assert.That(sellerValues[7].Text, Is.EqualTo(sellerAddress.PostalCode.ToString()));
            }
            #endregion

            #region shipToAddress
            IWebElement[] shipToValues = addressColumns[3].FindElements(By.TagName("p")).ToArray();

            if (model.ShipToName != null)
                Assert.That(shipToValues[1].Text, Is.EqualTo(model.ShipToName));

            if (model.ShipToAddress != null )
            {
                AddressModel shipToAddress = addressService.GetAddressModel(user, model.ShipToAddress) as AddressModel;

                if (shipToAddress.AddressLine1 != null)
                    Assert.That(shipToValues[2].Text, Is.EqualTo(shipToAddress.AddressLine1));

                if (shipToAddress.Location != null)
                    Assert.That(shipToValues[5].Text, Is.EqualTo(shipToAddress.Location));

                if (shipToAddress.StateId != null)
                {
                    string state = projectService.GetStateByStateId(shipToAddress.StateId.Value);
                    Assert.That(shipToValues[6].Text, Is.EqualTo(state));
                }
                if (shipToAddress.PostalCode != null)
                    Assert.That(shipToValues[7].Text, Is.EqualTo(shipToAddress.PostalCode.ToString()));
            }
            #endregion
        }

        /// <summary>
        /// Test the ProjectOrderDetails Partial Views. make  sure irt show ERP Order infos and allow user to add 
        /// project pipe line note type.
        /// </summary>
        [Test]
        [Category("ProjectView")]
        public void TestProjectView_ProjectOrderDetails_ShouldRenderProjectPipeLineInfo()
        {
            //need to perform logout before calling the test method
            bool responseFromDriver = true;

            try
            {
                driver.FindElement(By.Id("logoutForm"));
                driver.FindElement(By.Id("loginLink"));
                responseFromDriver = true;
            }
            catch
            {
                responseFromDriver = false;
            }

            if (responseFromDriver)
            {
                driver.FindElement(By.Id("logoutForm")).Submit();
                driver.FindElement(By.Id("loginLink")).Click();

            }

            //set user equal to user that has PipeLine Permissipon. User has to has either ViewPipe line
            // or Edit Pipe Line permission to see the ProjectPipeLineInfo
            long userId = this.db.Context.Users.Where(u => u.UserTypeId == UserTypeEnum.DaikinSuperUser && u.Email.Contains("daikincity"))
                              .OrderByDescending(u => u.UserId)
                              .Select(u => u.UserId).FirstOrDefault();

            var superUser = accountService.GetUserSessionModel(userId).Model as UserSessionModel;

            NavigateToLogin(superUser,driver);

            //get any projectId that has ERP Order Number
            var result = (from p in this.db.Context.Projects
                          where p.ProjectStatusTypeId == ProjectStatusTypeEnum.Open &&
                          p.ERPFirstOrderNumber != null
                          orderby p.ProjectId descending
                          select new
                          {
                              ProjectId = p.ProjectId
                          }).FirstOrDefault();

            driver.Navigate().GoToUrl("http://" + superUser.Email + ":" + "test" + "@tstsysdcity2/projectdashboard/Project/" + result.ProjectId);
            ProjectModel model = projectService.GetProjectModel(superUser, result.ProjectId).Model as ProjectModel;

            IWebElement[] projectPipeLineColumns = driver.FindElements(By.ClassName("details-list")).ToArray();

            IWebElement projectPipeLineFirstColumn = projectPipeLineColumns[3];

            IWebElement[] firstList = projectPipeLineFirstColumn.FindElement(By.TagName("ul"))
                                      .FindElements(By.TagName("li")).ToArray();

            IWebElement textValue;
            string value;

            if (model.ProjectLeadStatusTypeDescription != null)
            {
                 textValue = firstList[0].FindElements(By.TagName("p")).LastOrDefault();
                 value = textValue.Text;
                 Assert.That(value, Is.EqualTo(model.ProjectLeadStatusTypeDescription));
            }
            if(model.ERPFirstOrderNumber != null)
            {
                textValue = firstList[1].FindElements(By.TagName("p")).LastOrDefault();
                value = textValue.Text;
                Assert.That(value, Is.EqualTo(model.ERPFirstOrderNumber.ToString()));
            }
            if (model.ERPFirstOrderDate != null)
            {
                textValue = firstList[2].FindElements(By.TagName("p")).LastOrDefault();
                value = textValue.Text;
                Assert.That(value, Is.EqualTo(model.ERPFirstOrderDate.Value.ToString("G"))); 
            }

            IWebElement projectPipeLineSecondColumn = projectPipeLineColumns[4];

            IWebElement[] secondList = projectPipeLineSecondColumn.FindElement(By.TagName("ul"))
                                      .FindElements(By.TagName("li")).ToArray();

            if(model.ERPFirstPONumber != null)
            {
                textValue = secondList[0].FindElements(By.TagName("p")).LastOrDefault();
                value = textValue.Text;
                Assert.That(value, Is.EqualTo(model.ERPFirstPONumber.ToString()));
            }
            if(model.ERPFirstOrderComment != null)
            {
                textValue = secondList[1].FindElements(By.TagName("p")).LastOrDefault();
                value = textValue.Text;
                Assert.That(value.Replace(" ","").ToLower() , Is.EqualTo(model.ERPFirstOrderComment.Replace(" ","").ToLower()));
            }

            IWebElement projectPipeLineNoteTypeddl = driver.FindElement(By.TagName("select"));
            IWebElement[] projectPipeLineNoteOptions = projectPipeLineNoteTypeddl.FindElements(By.TagName("option")).ToArray();

            //get the list of ProjectPipeLineNoteType
            ProjectPipelineNoteType[] projectPipeLineNoteTypes = this.db.Context.ProjectPipelineNoteTypes.ToArray();

            //make sure the ProjectPipeLineNoteType drop down box populate correctly
            Assert.That(projectPipeLineNoteOptions.Count, Is.EqualTo(8));
            Assert.That(projectPipeLineNoteOptions[1].Text, Is.EqualTo(projectPipeLineNoteTypes[0].Name));
            Assert.That(projectPipeLineNoteOptions[2].Text, Is.EqualTo(projectPipeLineNoteTypes[1].Name));
            Assert.That(projectPipeLineNoteOptions[3].Text, Is.EqualTo(projectPipeLineNoteTypes[2].Name));
            Assert.That(projectPipeLineNoteOptions[4].Text, Is.EqualTo(projectPipeLineNoteTypes[3].Name));
            Assert.That(projectPipeLineNoteOptions[5].Text, Is.EqualTo(projectPipeLineNoteTypes[4].Name));
            Assert.That(projectPipeLineNoteOptions[6].Text, Is.EqualTo(projectPipeLineNoteTypes[5].Name));
            Assert.That(projectPipeLineNoteOptions[7].Text, Is.EqualTo(projectPipeLineNoteTypes[6].Name));
        }

        /*
        /// <summary>
        /// Test Project View, make sure ProjectDetails Partial View, ProjectButtonBar Partial View
        /// ActiveQuoteInfoBar Partial View, ProjectAddressDetails PartialView,
       ///  ProjectPipeLineNote Partial View render correctly
       /// <param name="HasEditProjectPermission"> if user has edit project permission</param>
       /// <param name="isDeletedProject"> if project status == deleted</param>
       /// <param name="isTransferedProject"> if project status == transfer</param>
        /// </summary>
        [Test]
        [Category("ProjectView")]
        [TestCase(true, false,false)]
        public void TestProjectView_Project_ShouldRenderProjectViewWithAllOfItPartialViews(bool HasEditProjectPermission, 
            bool isDeletedProject, bool isTransferedProject )
        {
            TestProjectView_ProjectButtonBar_ShouldRenderAllButtonsBelongToProject("HasUndeleteProjectPermission", "ModelDeletedIsTrue", false);
            //TestProjectView_ActiveQuoteInfoBar_ShouldRenderAllButtonsBelongToQuote(true, false, false, 4, 2, 3, 2, 1, true);
            TestProjectView_ProjectAddressDetails_ShouldRenderProjectAddresses();
            TestProjectView_ProjectDetails_ShouldRenderProjectInfoInDetails();
            TestProjectView_ProjectOrderDetails_ShouldRenderProjectPipeLineInfo();
            TestProjectView_ProjectTabPartial_ShouldRenderProjectTabsBasedOnPermission("HasViewProjectPermission");

            if(HasEditProjectPermission == true && isDeletedProject == false && isTransferedProject == false)
            {
                //get the project that has the order status type == Accepeted
                var project = (from p in this.db.Context.Projects
                               join quote in this.db.Context.Quotes
                               on p.ProjectId equals quote.ProjectId
                               join o in this.db.Context.Orders
                               on quote.QuoteId equals o.QuoteId
                               where p.OwnerId == user.UserId &&
                               o.OrderStatusTypeId == (byte)OrderStatusTypeEnum.Accepted
                               orderby p.ProjectId descending
                               select new
                               {
                                   ProjectId = p.ProjectId
                               }
                              ).FirstOrDefault();

               
                driver.Navigate().GoToUrl("http://" + user.Email + ":" + "123456" + "@tstsysdcity2/projectdashboard/Project/" + project.ProjectId);
                ProjectModel model = projectService.GetProjectModel(user, project.ProjectId).Model as ProjectModel;

                bool driverResponse = false;

                try
                {
                    List<IWebElement> anchorTags = driver.FindElements(By.TagName("a")).ToList();

                    foreach (IWebElement anchor in anchorTags)
                    {
                        if (anchor.Text.ToLower().Contains("edit project details"))
                        {
                            driverResponse = true;
                        }
                        else
                        {
                            driverResponse = false;
                        }
                    }
                }
                catch
                {
                    driverResponse = false;
                }

                //if order status type == submitted, the Edit Project Details button should visible
                Assert.That(driverResponse, Is.EqualTo(true));

            }
        }

        
        */
    }
}
