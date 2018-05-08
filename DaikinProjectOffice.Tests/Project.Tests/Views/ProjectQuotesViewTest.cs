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
    public class ProjectQuotesViewTest : TestAdmin
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

        public ProjectQuotesViewTest()
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

            projectId = this.db.Projects
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

        /// <summary>
        /// Test ProjectQuote tables.
        /// Make sure it render the Quotes list table with all the columns available
        /// </summary>
        [Test]
        [Category("ProjectQuotesView")]
        public void TestProjectQuotesView_ShouldRenderQuotesTable()
        {
            NavigateToProjectQuotesView(user, projectId);

            IWebElement searchQuotes = driver.FindElement(By.Id("Filter"));
            Assert.That(searchQuotes, Is.Not.EqualTo(null));

            IWebElement searchQuoteBtn = driver.FindElement(By.Id("btnSearch"));
            Assert.That(searchQuoteBtn, Is.Not.EqualTo(null));

                IWebElement quotesTable = driver.FindElement(By.Id("ProjectQuotes_table"));
                Assert.That(quotesTable, Is.Not.EqualTo(null));

                IWebElement[] quotesTable_rows = quotesTable.FindElements(By.TagName("tr")).ToArray();
                Assert.That(quotesTable_rows.Count, Is.GreaterThanOrEqualTo(0));

                //make sure the Quotes table render correct columns and menus ( action menu)

                IWebElement columnsDisplay = driver.FindElement(By.ClassName("display-btn")); // Get the display column select option
                columnsDisplay.Click();

                IWebElement columnslist = driver.FindElement(By.ClassName("tbl-column-opts")); //Get the List of availble display columns
                Assert.That(columnslist, Is.Not.EqualTo(null));

                IWebElement[] availableDisplayColumns = columnslist.FindElements(By.TagName("li")).ToArray();
                Assert.That(availableDisplayColumns.Count, Is.GreaterThan(0));

                //Make sure the available columns are correct
                Assert.That(availableDisplayColumns[4].Text.ToLower(), Is.EqualTo("quote"));
                Assert.That(availableDisplayColumns[5].Text.ToLower(), Is.EqualTo("alert"));
                Assert.That(availableDisplayColumns[6].Text.ToLower(), Is.EqualTo("revision"));
                Assert.That(availableDisplayColumns[7].Text.ToLower(), Is.EqualTo("items"));
                Assert.That(availableDisplayColumns[8].Text.ToLower(), Is.EqualTo("date"));
                Assert.That(availableDisplayColumns[9].Text.ToLower(), Is.EqualTo("total list"));
                Assert.That(availableDisplayColumns[10].Text.ToLower(), Is.EqualTo("total net"));
                Assert.That(availableDisplayColumns[11].Text.ToLower(), Is.EqualTo("total sell"));
                Assert.That(availableDisplayColumns[12].Text.ToLower(), Is.EqualTo("active"));

            //make sure Quote Tbale only render the Selected Columns from Display Option checkboxes
            //Uncheck Quote, alert and revision columns
            IWebElement quoteCheckBox = availableDisplayColumns[4].FindElement(By.TagName("input"));
            IWebElement alertCheckBox = availableDisplayColumns[5].FindElement(By.TagName("input"));
            IWebElement RevisionCheckBox = availableDisplayColumns[6].FindElement(By.TagName("input"));
            quoteCheckBox.Click();
            alertCheckBox.Click();
            RevisionCheckBox.Click();

            IWebElement[] columnheaders = quotesTable.FindElement(By.TagName("thead"))
                                                         .FindElement(By.TagName("tr"))
                                                         .FindElements(By.TagName("th")).ToArray();
                
                //make sure the columns header "Quote, Alert and Revision are disapear
                Assert.That(columnheaders[1].GetAttribute("style"), Is.EqualTo("display: none;"));
                Assert.That(columnheaders[2].GetAttribute("style"), Is.EqualTo("display: none;"));
                Assert.That(columnheaders[3].GetAttribute("style"), Is.EqualTo("display: none;"));

                IWebElement tableBody = quotesTable.FindElement(By.TagName("tbody"));
                IWebElement tableRow = tableBody.FindElements(By.TagName("tr")).First(); // ge tthe first row of table data
                IWebElement[] tableData = tableRow.FindElements(By.TagName("td")).ToArray();
                
                //make sure table data for Quote, Alert and Revision are disappear
                Assert.That(tableData[1].Text, Is.EqualTo(""));
                Assert.That(tableData[2].Text, Is.EqualTo(""));
                Assert.That(tableData[3].Text, Is.EqualTo(""));

               //now check these three columns gain
               quoteCheckBox.Click();
               alertCheckBox.Click();
               RevisionCheckBox.Click();

            //now make sure they show up again on Quotes table
            //make sure the columns header "Quote, Alert and Revision are disapear
            Assert.That(columnheaders[1].GetAttribute("style"), Is.EqualTo("display: table-cell;"));
                Assert.That(columnheaders[2].GetAttribute("style"), Is.EqualTo("display: table-cell;"));
                Assert.That(columnheaders[3].GetAttribute("style"), Is.EqualTo("display: table-cell;"));

                //make sure table data for Quote, Alert and Revision are disappear
                Assert.That(tableData[1].Text, Is.GreaterThan(string.Empty));
            Assert.That(tableData[2].Text, Is.GreaterThanOrEqualTo(string.Empty));
                Assert.That(tableData[3].Text, Is.GreaterThan(string.Empty));
            
        }

        /// <summary>
        /// Test the Columns render of Proejct Quote table . this depend on the se
        /// selection of Display option list checkBoxes
        /// </summary>
        /// <param name="testValue">Project status. include two cases : ProjectStatus = Open and 
        /// ProjectStatus != Open</param>
        [Test]
        [Category("ProjectQuotesView")]
        [TestCase(true)]
        [TestCase(false)]
        public void TestProjEctQuotes_ShouldRenderActionMenu(bool testValue)
        {
            NavigateToProjectQuotesView(user, projectId);

            IWebElement quoteTable = driver.FindElement(By.Id("ProjectQuotes_table"));
            IWebElement actionMenu = quoteTable.FindElement(By.TagName("tbody"))
                                               .FindElements(By.ClassName("actions")).FirstOrDefault();

            IWebElement[] actionItems = actionMenu.FindElement(By.TagName("ul"))
                                                  .FindElements(By.TagName("li")).ToArray();

            actionMenu.Click();

            IWebElement quoteExportLink = null;
            IWebElement quoteEditLink = null;
            IWebElement quoteDuplicateLink = null;
            IWebElement quoteDeleteLink = null;

            switch (actionItems.Count())
            {
                case 4:
                    quoteExportLink = actionItems[0].FindElement(By.TagName("a"));
                    quoteEditLink = actionItems[1].FindElement(By.TagName("a"));
                    quoteDuplicateLink = actionItems[2].FindElement(By.TagName("a"));
                    quoteDeleteLink = actionItems[4].FindElement(By.TagName("a"));

                    Assert.That(quoteExportLink.Text.ToLower(), Is.EqualTo("export quote"));
                    Assert.That(quoteEditLink.Text.ToLower(), Is.EqualTo("edit quote"));
                    Assert.That(quoteDuplicateLink.Text.ToLower(), Is.EqualTo("duplicate quote"));
                    Assert.That(quoteDeleteLink.Text.ToLower(), Is.EqualTo("delete quote"));
                    break;
                case 3:
                    quoteExportLink = actionItems[0].FindElement(By.TagName("a"));
                    quoteEditLink = actionItems[1].FindElement(By.TagName("a"));
                    quoteDuplicateLink = actionItems[2].FindElement(By.TagName("a"));

                    Assert.That(quoteExportLink.Text.ToLower(), Is.EqualTo("export quote"));
                    Assert.That(quoteEditLink.Text.ToLower(), Is.EqualTo("edit quote"));
                    Assert.That(quoteDuplicateLink.Text.ToLower(), Is.EqualTo("duplicate quote"));
                    break;
                case 2:
                     quoteEditLink = actionItems[1].FindElement(By.TagName("a"));
                     quoteDuplicateLink = actionItems[2].FindElement(By.TagName("a"));

                    Assert.That(quoteEditLink.Text.ToLower(), Is.EqualTo("export quote"));
                    Assert.That(quoteDuplicateLink.Text.ToLower(), Is.EqualTo("edit quote"));
                    break;
                case 1:
                    Assert.That(quoteDuplicateLink.Text.ToLower(), Is.EqualTo("export quote"));
                    break;
            }

        }

        /// <summary>
        /// Test the Click Event of Export Quote Link on Action Menu
        /// </summary>
        [Test]
        [Category("ProjectQuotesView")]
        public void TestProjectQuotesView_TestExportQuoteOnActionMenu()
        {
            var quoteId = this.db.Context.Quotes.Where(q => q.ProjectId == projectId)
                              .OrderBy(q => q.QuoteId)
                              .Select(q => q.QuoteId).FirstOrDefault();

            NavigateToProjectQuotesView(user, projectId);

            IWebElement quoteTable = driver.FindElement(By.Id("ProjectQuotes_table"));
            IWebElement actionMenu = quoteTable.FindElement(By.TagName("tbody"))
                                               .FindElements(By.ClassName("actions")).FirstOrDefault();

            IWebElement[] actionItems = actionMenu.FindElement(By.TagName("ul"))
                                                  .FindElements(By.TagName("li")).ToArray();
            actionMenu.Click();

            IWebElement quoteExportLink = actionItems[0].FindElement(By.TagName("a"));

            Assert.That(quoteExportLink.GetAttribute("href").ToLower(),
                        Is.EqualTo("http://user15%40test.com:123456@tstsysdcity2/projectdashboard/quoteexport/" 
                        + projectId + "/" + quoteId));

            //quoteExportLink.Click();
        }

        /// <summary>
        /// Test the click event of EditQuote Link on Action Menu
        /// should navigate to QuoteEdit View
        /// </summary>
        [Test]
        [Category("ProjectQuotesView")]
        public void TestProjectQuotesView_TestEditQuoteOnActionMenu()
        {
            var quoteId = this.db.Context.Quotes.Where(q => q.ProjectId == projectId)
                              .OrderBy(q => q.QuoteId)
                              .Select(q => q.QuoteId).FirstOrDefault();

            NavigateToProjectQuotesView(user, projectId);

            IWebElement quoteTable = driver.FindElement(By.Id("ProjectQuotes_table"));
            IWebElement actionMenu = quoteTable.FindElement(By.TagName("tbody"))
                                               .FindElements(By.ClassName("actions")).FirstOrDefault();

            IWebElement[] actionItems = actionMenu.FindElement(By.TagName("ul"))
                                                  .FindElements(By.TagName("li")).ToArray();
            actionMenu.Click();

            IWebElement quoteEditLink = actionItems[1].FindElement(By.TagName("a"));

            Assert.That(quoteEditLink.GetAttribute("href").ToLower(),
                        Is.EqualTo("http://user15%40test.com:123456@tstsysdcity2/projectdashboard/quoteedit/" + 
                        projectId + "/" + quoteId));

            // this not working, raise exception "Element is not visible!"
            //quoteEditLink.Click(); 

            //excute the click event of quote edit link using JavaScript executer
            //somehow the normal way to execute the click event like quoteEditLink.Click() not working
            //so we need to use the JavaScript executor
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            js.ExecuteScript("arguments[0].click()", quoteEditLink);

            //make sure the view is navigate to QuoteEdit View
            //by checking the DOMs

            IWebElement quoteTitle = driver.FindElement(By.Id("Title"));
            Assert.That(quoteTitle.Text, Is.Not.EqualTo(null));
            IWebElement quoteDescription = driver.FindElement(By.Id("Description"));
            Assert.That(quoteDescription, Is.Not.EqualTo(null));
            IWebElement quoteNotes = driver.FindElement(By.Id("Notes"));
            Assert.That(quoteNotes, Is.Not.EqualTo(null));
        }

        /// <summary>
        /// Test the Click event of Duplicate Quote Link on Action Menu
        /// </summary>
        [Test]
        [Category("ProjectQuotesView")]
        public void TestProjectQuotesView_TestDuplicateQuoteOnActionMenu()
        {
            //get the old quoteId before duplicate
            long quoteId = 0;
            int  results = this.db.Quotes.Where(q => q.ProjectId == projectId).Count();

            if (results > 1)
            {
                //get the last quote before duplicate
                quoteId = this.db.Context.Quotes.Where(q => q.ProjectId == projectId)
                                 .OrderBy(q => q.QuoteId)
                                 .Select(q => q.QuoteId).ToArray()[results - 1];
                                 
            }
            else
            {
                //get the quote before duplicate
                quoteId = this.db.Context.Quotes.Where(q => q.ProjectId == projectId)
                                  .OrderBy(q => q.QuoteId)
                                  .Select(q => q.QuoteId).FirstOrDefault();
            }

            NavigateToProjectQuotesView(user, projectId);

            IWebElement quoteTable = driver.FindElement(By.Id("ProjectQuotes_table"));

            int ActionMenuCount = quoteTable.FindElement(By.TagName("tbody"))
                                               .FindElements(By.ClassName("actions")).Count();

            IWebElement actionMenu = quoteTable.FindElement(By.TagName("tbody"))
                                               .FindElements(By.ClassName("actions"))
                                               .ToArray()[ActionMenuCount -1];

            IWebElement[] actionItems = actionMenu.FindElement(By.TagName("ul"))
                                                  .FindElements(By.TagName("li")).ToArray();
            actionMenu.Click();

            IWebElement quoteDuplicateLink = actionItems[2].FindElement(By.TagName("a"));
            Assert.That(quoteDuplicateLink.GetAttribute("href").ToLower(), 
                        Is.EqualTo("http://user15%40test.com:123456@tstsysdcity2/projectdashboard/quoteduplicate/" + 
                        projectId + "/" + quoteId));

            ProjectQuotesModel projectQuotesModel = new ProjectQuotesModel();
            projectQuotesModel.ProjectId = projectId;

            projectQuotesModel = projectService.GetProjectQuotesModel(user, projectQuotesModel).Model as ProjectQuotesModel;
            var quoteCount = projectQuotesModel.Items.Count;

            var oldQuote = projectQuotesModel.Items.Where(i => i.QuoteId == quoteId)
                                             .OrderByDescending( I => I.QuoteId) 
                                             .FirstOrDefault();

            quoteDuplicateLink.Click();

            projectQuotesModel = projectService.GetProjectQuotesModel(user, projectQuotesModel).Model as ProjectQuotesModel;

            //make sure new quote has added to quote table.
            Assert.That(projectQuotesModel.Items.Count, Is.GreaterThan(quoteCount));
            
            //get the duplicate quote
            QuoteListModel duplicateQuote = projectQuotesModel.Items[projectQuotesModel.Items.Count() - 1] as QuoteListModel;
            
            //make sure new quote has properties value match old quote
            Assert.That(duplicateQuote.Title.ToLower(),Is.EqualTo(oldQuote.Title.ToLower()));
            Assert.That(duplicateQuote.TotalList, Is.EqualTo(oldQuote.TotalList));
            Assert.That(duplicateQuote.TotalNet, Is.EqualTo(oldQuote.TotalNet));
            Assert.That(duplicateQuote.TotalSell, Is.EqualTo(oldQuote.TotalSell));
            Assert.That(duplicateQuote.ProjectId, Is.EqualTo(oldQuote.ProjectId));
        }

        [Test]
        [Category("ProjectQuotesView")]
        public void TestProjectQuotesView_TestDeleteQuoteOnActionMenu()
        {
            NavigateToProjectQuotesView(user, projectId);

            IWebElement quoteTable = driver.FindElement(By.Id("ProjectQuotes_table"));
            IWebElement actionMenu = quoteTable.FindElement(By.TagName("tbody"))
                                               .FindElements(By.ClassName("actions")).ToList().Last();

            IWebElement[] actionItems = actionMenu.FindElement(By.TagName("ul"))
                                                  .FindElements(By.TagName("li")).ToArray();
            actionMenu.Click();

            IWebElement quoteDeleteLink = actionItems[3].FindElement(By.TagName("a"));

            Assert.That(quoteDeleteLink.GetAttribute("href").ToLower(),
                        Is.EqualTo("http://user15%40test.com:123456@tstsysdcity2/projectdashboard/projectquotes/" +
                        projectId + "#" ));
            Assert.That(quoteDeleteLink.GetAttribute("data-sc-ajaxpost").ToLower(),
                        Is.EqualTo("/projectdashboard/projectquotesmodeldeletequote"));
            Assert.That(quoteDeleteLink.GetAttribute("data-sc-quoteid"), Is.Not.EqualTo(string.Empty));

            // this not working, raise exception "Element is not visible!"
            //quoteDeleteLink.Click(); 

            //excute the click event of quote edit link using JavaScript executer
            //somehow the normal way to execute the click event like quoteDeleteLink.Click() not working
            //so we need to use the JavaScript executor
            IJavaScriptExecutor js = driver as IJavaScriptExecutor;
            js.ExecuteScript("arguments[0].click()", quoteDeleteLink);

            //Test to see if the Delete Quote Confirm Popup is show.
            IWebElement confirmModel = driver.FindElement(By.Id("confirm_modal"));
            Assert.That(confirmModel, Is.Not.EqualTo(null));

            IWebElement confirmMessage = confirmModel.FindElement(By.TagName("p"));
            Assert.That(confirmMessage.Text.ToLower(), Is.EqualTo("are you sure you want to delete this quote?"));

            IWebElement confirmYesBtn = confirmModel.FindElement(By.Id("confirm_modal_yes"));
            Assert.That(confirmYesBtn, Is.Not.EqualTo(null));
            Assert.That(confirmYesBtn.Displayed, Is.EqualTo(true));
            Assert.That(confirmYesBtn.Enabled, Is.EqualTo(true));

            IWebElement confirmCancelBtn = confirmModel.FindElement(By.ClassName("cancel-btn"));
            Assert.That(confirmCancelBtn, Is.Not.EqualTo(null));
            Assert.That(confirmCancelBtn.Displayed, Is.EqualTo(true));
            Assert.That(confirmCancelBtn.Enabled, Is.EqualTo(true));
        }

        /// <summary>
        /// Test the Set Active Quote button on the Quotes Table
        /// The Active Quote Button is only clickable when the quote is InActive
        /// </summary>
        [Test]
        [Category("ProjectQuotesView")]
        public void TestProjectQuotesView_TestSetActiveButton_ShouldSetTheSelectedQuoteToActive()
        {
            NavigateToProjectQuotesView(user, projectId);

            IWebElement quoteTable = driver.FindElement(By.Id("ProjectQuotes_table"));
            IWebElement quoteSetActiveBtnDiv = quoteTable.FindElement(By.TagName("tbody"))
                                               .FindElements(By.TagName("td")).ToList()
                                               .Where(i => i.GetAttribute("data-col-id") == "9").First();

            //get the row that contain the quoteId that we select. default will be first row
            IWebElement quoteSelectedId = quoteTable.FindElement(By.TagName("tbody"))
                                                    .FindElements(By.TagName("tr")).ToList()
                                                    .First();

            //now we need to check if the quote we select is active or not
            bool isActive = false;

            long quoteId = (Convert.ToInt64( quoteSelectedId.GetAttribute("id")));

            QuoteServices quoteService = new QuoteServices();
            QuoteModel selectedQuote = quoteService.GetQuoteModel(user, projectId, quoteId).Model as QuoteModel;

            if(selectedQuote != null)
            {
                if(selectedQuote.Active == true)
                {
                    isActive = true;
                }
            }

            IWebElement quoteSetActiveBtn = null;

            if (isActive == false)
            {
                quoteSetActiveBtn = quoteSetActiveBtnDiv.FindElement(By.TagName("a"));

                Assert.That(quoteSetActiveBtn.GetAttribute("href").ToLower(),
                            Is.EqualTo("http://user15%40test.com:123456@tstsysdcity2/projectdashboard/projectquotes/" +
                            projectId + "#"));
                Assert.That(quoteSetActiveBtn.GetAttribute("sc-ajaxpost").ToLower(),
                            Is.EqualTo("/projectdashboard/quotesetactive"));
                Assert.That(quoteSetActiveBtn.GetAttribute("sc-quoteid"), Is.Not.EqualTo(string.Empty));
                Assert.That(quoteSetActiveBtn.FindElement(By.TagName("img")).GetAttribute("src").ToLower(),
                            Is.EqualTo("http://user15%40test.com:123456@tstsysdcity2/images/switch-off.png"));

                //only need to check for the click event when quote is not Active.
                //when quote is Active , the SetActive button is unclickable.
                IJavaScriptExecutor js = driver as IJavaScriptExecutor;
                js.ExecuteScript("arguments[0].click()", quoteSetActiveBtn);
                
                //set the thread sleep for 5 seconds to make sure the Return message availble
                Thread.Sleep(5000);

                IWebElement quoteSetActiveMessage = driver.FindElement(By.ClassName("pagemessage-success"));
                Assert.That(quoteSetActiveMessage.Text.ToLower().Contains("has been updated."));
            }
            else
            {   
                quoteSetActiveBtn = quoteSetActiveBtnDiv.FindElement(By.TagName("img"));
                Assert.That(quoteSetActiveBtn.GetAttribute("src").ToLower(), 
                           Is.EqualTo("http://user15%40test.com:123456@tstsysdcity2/images/switch-on.png"));
            }
        }
        /// <summary>
        /// navigate the Ui to the ProjectQuotes View 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="projectId"></param>
        public void NavigateToProjectQuotesView(UserSessionModel user, long projectId = 0)
        {
            NavigateToLogin(user, driver);

            if (projectId == 0)
            {
                if (user.Email == "User15@test.com")
                {
                    driver.Navigate().GoToUrl("http://User15@test.com:123456@tstsysdcity2/");
                }
                else
                {
                    driver.Navigate().GoToUrl("http://test@123.com:123456@tstsysdcity2/");
                }

                Thread.Sleep(5000);

                IWebElement projectOverviewTab = driver.FindElement(By.Id("headerBtns"))
                                                       .FindElements(By.TagName("a")).ToArray()[1];
                // Get ProjectOverView Tab Element

                projectOverviewTab.Click();

                IWebElement[] tabBars = driver.FindElement(By.ClassName("tab-bar"))
                                              .FindElements(By.TagName("li"))
                                              .ToArray();

                IWebElement projectsTab = tabBars[1];

                projectsTab.Click();

                IWebElement newProjectBtn = driver.FindElement(By.ClassName("btn-bar"))
                                                  .FindElements(By.TagName("a")).First();

                newProjectBtn.Click();
            }
            else
            {
                if (user.Email == "User15@test.com")
                {
                    driver.Navigate().GoToUrl("http://User15@test.com:123456@tstsysdcity2/projectdashboard/ProjectQuotes/" + projectId);
                }
                else
                {
                    driver.Navigate().GoToUrl("http://test@123.com:123456@tstsysdcity2/projectdashboard/ProjectQuotes/" + projectId);
                }
                Thread.Sleep(2000);
            }

        }
    }
}
