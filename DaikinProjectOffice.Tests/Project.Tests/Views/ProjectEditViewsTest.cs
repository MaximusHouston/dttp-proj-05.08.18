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

namespace DaikinProjectOffice.Tests
{
    [TestFixture]
    public class ProjectEditViewsTest : TestAdmin
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
        public ProjectEditViewsTest()
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

        /// <summary>
        /// Test ShowConfirm popup Partial View whe nuser hit 'Save Changes' Button 
        /// on ProjectEditView
        /// </summary>
        public void TestProjectEditViews_ConfirmModelProjectCreatedView_ShouldShowConfirmModelProjectCreatedPopup()
        {
            IWebElement confirm_modal_project_created = driver.FindElement(By.Id("confirm_modal_project_created"));
            IWebElement message = confirm_modal_project_created.FindElement(By.TagName("p"));
            Assert.That(message.Text.Contains("Your project has been created and a new quote added."), Is.EqualTo(true));
        }

        /// <summary>
        /// Test ProjectDetail render section on ProjectEditView
        /// </summary>
        /// <param name="testValue">Test in both case 'View' and 'Edit'</param>
        public void TestProjectEditViews_ProjectEditView_ShouldRenderProjectDetail(string testValue)
        {
          
            //get the div contain the ul

            IWebElement projectDetailFirstColumn = driver.FindElement(By.Id("projectDetails"))
                                                     .FindElement(By.ClassName("row"))
                                                     .FindElements(By.TagName("div"))[1]; 
                                                   
            IWebElement[] projectDetailItemsFirstColumn = projectDetailFirstColumn
                                                          .FindElements(By.TagName("li"))
                                                          .ToArray();
            IWebElement projectDetailSecondColumn = driver.FindElement(By.Id("projectDetails"))
                                                     .FindElement(By.ClassName("row"))
                                                     .FindElements(By.TagName("div"))[4];

           IWebElement[] projectDetailItemsSecondColumn = projectDetailSecondColumn
                                                         .FindElements(By.TagName("li"))
                                                         .ToArray();

            Assert.That(projectDetailItemsFirstColumn.Count() + projectDetailItemsSecondColumn.Count() - 1, 
                        Is.EqualTo(14));

            if (testValue == "CreateProject")
            {
                Assert.That(projectDetailItemsFirstColumn[0].FindElement(By.TagName("input"))
                            .GetAttribute("name"), Is.EqualTo("Name"));
                Assert.That(projectDetailItemsFirstColumn[1].FindElement(By.TagName("input"))
                           .GetAttribute("name"), Is.EqualTo("ProjectDateDisplay"));
                Assert.That(projectDetailItemsFirstColumn[2].FindElement(By.TagName("select"))
                           .GetAttribute("name"), Is.EqualTo("ConstructionTypeId"));
                Assert.That(projectDetailItemsFirstColumn[3].FindElement(By.TagName("select"))
                           .GetAttribute("name"), Is.EqualTo("ProjectStatusTypeId"));
                Assert.That(projectDetailItemsFirstColumn[4].FindElement(By.TagName("select"))
                          .GetAttribute("name"), Is.EqualTo("ProjectTypeId"));
                Assert.That(projectDetailItemsFirstColumn[5].FindElement(By.TagName("select"))
                          .GetAttribute("name"), Is.EqualTo("ProjectOpenStatusTypeId"));
                Assert.That(projectDetailItemsFirstColumn[6].FindElement(By.TagName("select"))
                          .GetAttribute("name"), Is.EqualTo("VerticalMarketTypeId"));

                Assert.That(projectDetailItemsSecondColumn[0].FindElement(By.TagName("input"))
                           .GetAttribute("name"), Is.EqualTo("BidDate"));
                Assert.That(projectDetailItemsSecondColumn[1].FindElement(By.TagName("input"))
                           .GetAttribute("name"), Is.EqualTo("EstimatedClose"));
                Assert.That(projectDetailItemsSecondColumn[2].FindElement(By.TagName("input"))
                           .GetAttribute("name"), Is.EqualTo("EstimatedDelivery"));
                Assert.That(projectDetailItemsSecondColumn[3].FindElement(By.TagName("select"))
                           .GetAttribute("name"), Is.EqualTo("ShipToAddress.CountryCode"));
                Assert.That(projectDetailItemsSecondColumn[4].FindElement(By.TagName("select"))
                           .GetAttribute("name"), Is.EqualTo("ShipToAddress.StateId"));
                Assert.That(projectDetailItemsSecondColumn[5].FindElement(By.TagName("input"))
                          .GetAttribute("name"), Is.EqualTo("ShipToAddress.Location"));
                Assert.That(projectDetailItemsSecondColumn[6].FindElement(By.TagName("textarea"))
                          .GetAttribute("name"), Is.EqualTo("Description"));

                //Check the default values for fields 
                DateTime projectDate = Convert.ToDateTime(projectDetailItemsFirstColumn[1].FindElement(By.TagName("input")).GetAttribute("value"));
                Assert.That(projectDate.Date == DateTime.Now.Date, Is.EqualTo(true));

                Assert.That(projectDetailItemsFirstColumn[3].FindElement(By.TagName("select")).Text, 
                           Is.EqualTo(ProjectStatusTypeEnum.Open.ToString()));

                //tricker the selection event of ProjectType and ProjectOpenStatusType dropdownlist
                SelectElement projectTypeSelection = new SelectElement(projectDetailItemsFirstColumn[4].FindElement(By.TagName("select")));
                projectTypeSelection.SelectByIndex(1);

                SelectElement projectOpenStatusTypeSelection = new SelectElement(projectDetailItemsFirstColumn[5].FindElement(By.TagName("select")));
                projectOpenStatusTypeSelection.SelectByIndex(1);

                Assert.That( Convert.ToDateTime( projectDetailItemsSecondColumn[0]
                             .FindElement(By.TagName("input")).GetAttribute("value")),
                             Is.EqualTo( projectDate ));

                DateTime estimateCloseDate = projectDate.AddDays(60);
                Assert.That( Convert.ToDateTime( projectDetailItemsSecondColumn[1]
                             .FindElement(By.TagName("input")).GetAttribute("value")), 
                             Is.EqualTo(estimateCloseDate));

                DateTime estimateDelivery = estimateCloseDate.AddDays(30);
                Assert.That( Convert.ToDateTime(projectDetailItemsSecondColumn[2].FindElement(By.TagName("input")).GetAttribute("value")),
                             Is.EqualTo(estimateDelivery));

            }

            if(testValue == "EditProject")
            {

                Assert.That(projectDetailItemsFirstColumn[0].FindElement(By.TagName("input"))
                           .GetAttribute("value"), Is.EqualTo(projectVM.Name));
                Assert.That(Convert.ToDateTime(projectDetailItemsFirstColumn[1].FindElement(By.TagName("input"))
                           .GetAttribute("value")), Is.EqualTo(projectVM.ProjectDate));
                
                //get the selected value of constructionType selection option
                //check the selected value of construction type
                SelectElement selectedOption = new SelectElement(projectDetailItemsFirstColumn[2].FindElement(By.TagName("select")));
                //var constructionTypeSelectedText = (string)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].options[arguments[0].selectedIndex].text;", constructionTypeddl);
                var selectedValue = (string)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].options[arguments[0].selectedIndex].value;", selectedOption);
                Assert.That(selectedValue, Is.EqualTo( projectVM.ConstructionTypeId.ToString()));

                //check the selected value of projectStatusType
                selectedOption = new SelectElement (projectDetailItemsFirstColumn[3].FindElement(By.TagName("select")));
                selectedValue = (string)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].options[arguments[0].selectedIndex].value;", selectedOption);
                Assert.That(selectedValue, Is.EqualTo(projectVM.ProjectStatusTypeId.ToString()));

                //chekc the selected value of projectType
                selectedOption = new SelectElement(projectDetailItemsFirstColumn[4].FindElement(By.TagName("select")));
                selectedValue = (string)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].options[arguments[0].selectedIndex].value;", selectedOption);
                Assert.That(selectedValue, Is.EqualTo(projectVM.ProjectTypeId.ToString()));

                //chekc the selected value of projectOpenStatusType
                selectedOption = new SelectElement(projectDetailItemsFirstColumn[5].FindElement(By.TagName("select")));
                selectedValue = (string)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].options[arguments[0].selectedIndex].value;", selectedOption);
                Assert.That(selectedValue, Is.EqualTo(projectVM.ProjectOpenStatusTypeId.ToString()));

                //check the selected value of vertivalMarketType
                selectedOption = new SelectElement(projectDetailItemsFirstColumn[6].FindElement(By.TagName("select")));
                selectedValue = (string)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].options[arguments[0].selectedIndex].value;", selectedOption);
                Assert.That(selectedValue, Is.EqualTo(projectVM.VerticalMarketTypeId.ToString()));

                Assert.That(Convert.ToDateTime(projectDetailItemsSecondColumn[0].FindElement(By.TagName("input"))
                          .GetAttribute("value")), Is.EqualTo(projectVM.BidDate));

                Assert.That(Convert.ToDateTime(projectDetailItemsSecondColumn[1].FindElement(By.TagName("input"))
                           .GetAttribute("value")), Is.EqualTo(projectVM.EstimatedClose));

                Assert.That(Convert.ToDateTime(projectDetailItemsSecondColumn[2].FindElement(By.TagName("input"))
                           .GetAttribute("value")), Is.EqualTo(projectVM.EstimatedDelivery));

                //check selected value of shipToAddress State
                selectedOption = new SelectElement(projectDetailItemsSecondColumn[4].FindElement(By.TagName("select")));
                selectedValue = (string)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].options[arguments[0].selectedIndex].value;", selectedOption);
                Assert.That(selectedValue,Is.EqualTo(projectVM.ShipToAddress.StateId.ToString()));

                Assert.That(projectDetailItemsSecondColumn[5].FindElement(By.TagName("input"))
                          .GetAttribute("value"), Is.EqualTo(projectVM.ShipToAddress.Location));

                //if project has Description, check it
                if (projectVM.Description != null)
                {
                    Assert.That(projectDetailItemsSecondColumn[6].FindElement(By.TagName("textarea"))
                            .GetAttribute("value").Contains(projectVM.Description), Is.EqualTo(true));
                }
            }
        }

        /// <summary>
        /// Test Engineer Detail render section on ProjectEditView
        /// </summary>
        /// <param name="testValue">Test in both case 'View' and 'Edit'</param>
        public void TestProjectEditViews_ProjectEditView_ShouldRenderEngineeringDetails(string testValue)
        {
                
            if (testValue == "CreateProject")
            {
                IWebElement engineerName = driver.FindElement(By.Id("EngineerName"));
                Assert.That(engineerName, Is.Not.EqualTo(null));
                IWebElement businessName = driver.FindElement(By.Id("EngineerBusinessName"));
                Assert.That(businessName, Is.Not.EqualTo(null));
                IWebElement addressLine1 = driver.FindElement(By.Id("EngineerAddress_AddressLine1"));
                Assert.That(addressLine1, Is.Not.EqualTo(null));
            }
            else
            {
                IWebElement engineerName = driver.FindElement(By.Id("EngineerName"));
                if (projectVM.EngineerName != null)
                {
                    Assert.That(engineerName.Text, Is.EqualTo(projectVM.EngineerName));
                }

                IWebElement engineerBusName = driver.FindElement(By.Id("EngineerName"));
                if(projectVM.EngineerBusinessName != null)
                {
                    Assert.That(engineerBusName, Is.EqualTo(projectVM.EngineerBusinessName));
                }

                IWebElement addressLine1 = driver.FindElement(By.Id("EngineerAddress_AddressLine1"));
                if(projectVM.EngineerAddress.AddressLine1 != null)
                {
                    Assert.That(addressLine1, Is.EqualTo(projectVM.EngineerAddress.AddressLine1));
                }

                IWebElement location = driver.FindElement(By.Id("EngineerAddress_Location"));
                if(projectVM.EngineerAddress.Location != null)
                {
                    Assert.That(location, Is.EqualTo(projectVM.EngineerAddress.Location));
                }

                var selectedOption = new SelectElement(driver.FindElement(By.Id("EngineerAddress_StateId")));
                var selectedValue = (string)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].options[arguments[0].selectedIndex].value;", selectedOption);
                Assert.That(selectedValue, Is.EqualTo(projectVM.EngineerAddress.StateId.ToString()));

                IWebElement postalCode = driver.FindElement(By.Id("EngineerAddress_PostalCode"));
                if(projectVM.EngineerAddress.PostalCode != null)
                {
                    Assert.That(postalCode.GetAttribute("value"), Is.EqualTo(projectVM.EngineerAddress.PostalCode));
                }
            }

        }

        /// <summary>
        /// Test Dealer/Constractor Detail render section on ProjectEditView
        /// </summary>
        /// <param name="testValue">Test in both case 'View' and 'Edit'</param>
        public void TestProjectEditViews_ProjectEditView_ShouldRenderDealerDetails(string testValue)
        {
            if (testValue == "CreateProject")
            { 
                IWebElement dealerName = driver.FindElement(By.Id("DealerContractorName"));
                Assert.That(dealerName, Is.Not.EqualTo(null));
                IWebElement dealerBusName = driver.FindElement(By.Id("CustomerName"));
                Assert.That(dealerBusName, Is.Not.EqualTo(null));
                IWebElement addressLine1 = driver.FindElement(By.Id("CustomerAddress_AddressLine1"));
                Assert.That(addressLine1, Is.Not.EqualTo(null));
                IWebElement location = driver.FindElement(By.Id("CustomerAddress_Location"));
                Assert.That(location, Is.Not.EqualTo(null));
            }
            else
            {
                IWebElement dealerName = driver.FindElement(By.Id("DealerContractorName"));
                if (projectVM.DealerContractorName != null)
                {
                    Assert.That(dealerName.GetAttribute("value"), Is.EqualTo(projectVM.DealerContractorName));
                }

                IWebElement dealerBusName = driver.FindElement(By.Id("CustomerName"));
                if (projectVM.CustomerName != null)
                {
                    Assert.That(dealerBusName.GetAttribute("value"), Is.EqualTo(projectVM.CustomerName));
                }

                IWebElement addressLine1 = driver.FindElement(By.Id("CustomerAddress_AddressLine1"));
                if (projectVM.CustomerAddress.AddressLine1 != null)
                {
                    Assert.That(addressLine1.GetAttribute("value"), Is.EqualTo(projectVM.CustomerAddress.AddressLine1));
                }

                IWebElement location = driver.FindElement(By.Id("CustomerAddress_Location"));
                if (projectVM.CustomerAddress.Location != null)
                {
                    Assert.That(location.GetAttribute("value"), Is.EqualTo(projectVM.CustomerAddress.Location));
                }

                var selectedOption = new SelectElement(driver.FindElement(By.Id("CustomerAddress_StateId")));
                var selectedValue = (string)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].options[arguments[0].selectedIndex].value;", selectedOption);
                if (projectVM.CustomerAddress.StateId != null)
                    Assert.That(selectedValue, Is.EqualTo(projectVM.CustomerAddress.StateId.ToString()));


                IWebElement postalCode = driver.FindElement(By.Id("CustomerAddress_PostalCode"));
                if (projectVM.CustomerAddress.PostalCode != null)
                {
                    Assert.That(postalCode.GetAttribute("value"), Is.EqualTo(projectVM.CustomerAddress.PostalCode));
                }
            }
        }

        /// <summary>
        /// Test Seller Detail render section on ProjectEditView
        /// </summary>
        /// <param name="testValue">Test in both case 'View' and 'Edit'</param>
        public void TestProjectEditViews_ProjectEditView_ShouldRenderSellerDetails(string testValue)
        {
            if (testValue == "CreateProject")
            {
                IWebElement sellerName = driver.FindElement(By.Id("SellerName"));
                Assert.That(sellerName, Is.Not.EqualTo(null));
                IWebElement addressLine1 = driver.FindElement(By.Id("SellerAddress_AddressLine1"));
                Assert.That(addressLine1, Is.Not.EqualTo(null));
                IWebElement location = driver.FindElement(By.Id("SellerAddress_Location"));
                Assert.That(location, Is.Not.EqualTo(null));
            }
            else
            {
                IWebElement sellerName = driver.FindElement(By.Id("SellerName"));
                if (projectVM.SellerName != null)
                {
                    Assert.That(sellerName.GetAttribute("value"), Is.EqualTo(projectVM.SellerName));
                }

                IWebElement addressLine1 = driver.FindElement(By.Id("SellerAddress_AddressLine1"));
                if (projectVM.SellerAddress.AddressLine1 != null)
                {
                    Assert.That(addressLine1.GetAttribute("value"), Is.EqualTo(projectVM.SellerAddress.AddressLine1));
                }

                IWebElement location = driver.FindElement(By.Id("SellerAddress_Location"));
                if (projectVM.SellerAddress.Location != null)
                {
                    Assert.That(location.GetAttribute("value"), Is.EqualTo(projectVM.SellerAddress.Location));
                }

                var selectedOption = new SelectElement(driver.FindElement(By.Id("SellerAddress_StateId")));
                var selectedValue = (string)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].options[arguments[0].selectedIndex].value;", selectedOption);
                if (projectVM.SellerAddress.StateId != null)
                    Assert.That(selectedValue, Is.EqualTo(projectVM.SellerAddress.StateId.ToString()));


                IWebElement postalCode = driver.FindElement(By.Id("SellerAddress_PostalCode"));
                if (projectVM.SellerAddress.PostalCode != null)
                {
                    Assert.That(postalCode.GetAttribute("value"), Is.EqualTo(projectVM.SellerAddress.PostalCode));
                }
            }
        }

        /// <summary>
        /// Test Ship To Location Tab render section on ProjectEditView
        /// </summary>
        /// <param name="testValue">Test in both case 'View' and 'Edit'</param>
        public void TestProjectEditViews_ProjectEditView_ShouldRenderShipToLocationDetails(string testValue)
        {
            if (testValue == "CreateProject")
            {
                IWebElement shipToName = driver.FindElement(By.Id("ShipToName"));
                Assert.That(shipToName, Is.Not.EqualTo(null));
                IWebElement addressLine1 = driver.FindElement(By.Id("ShipToAddress_AddressLine1"));
                Assert.That(addressLine1, Is.Not.EqualTo(null));
                IWebElement location = driver.FindElement(By.Id("ShipToAddress_ProjectLocation"));
                Assert.That(location, Is.Not.EqualTo(null));
                IWebElement squareFootage = driver.FindElement(By.Id("SquareFootage"));
                Assert.That(squareFootage, Is.Not.EqualTo(null));
                IWebElement numberOfFloors = driver.FindElement(By.Id("NumberOfFloors"));
                Assert.That(numberOfFloors, Is.Not.EqualTo(null));
            }
            else
            {
                IWebElement shipToName = driver.FindElement(By.Id("ShipToName"));
                if (projectVM.ShipToName != null)
                {
                    Assert.That(shipToName.GetAttribute("value"), Is.EqualTo(projectVM.ShipToName));
                }

                IWebElement addressLine1 = driver.FindElement(By.Id("ShipToAddress_AddressLine1"));
                if (projectVM.ShipToAddress.AddressLine1 != null)
                {
                    Assert.That(addressLine1.GetAttribute("value"), Is.EqualTo(projectVM.ShipToAddress.AddressLine1));
                }

                IWebElement location = driver.FindElement(By.Id("ShipToAddress_ProjectLocation"));
                if (projectVM.ShipToAddress.Location != null)
                {
                    Assert.That(location.GetAttribute("value"), Is.EqualTo(projectVM.ShipToAddress.Location));
                }

                var selectedOption = new SelectElement(driver.FindElement(By.Id("ShipToAddress_StateId")));
                var selectedValue = (string)((IJavaScriptExecutor)driver).ExecuteScript("return arguments[0].options[arguments[0].selectedIndex].value;", selectedOption);
                if (projectVM.ShipToAddress.StateId != null)
                    Assert.That(selectedValue, Is.EqualTo(projectVM.ShipToAddress.StateId.ToString()));

                IWebElement postalCode = driver.FindElement(By.Id("ShipToAddress_PostalCode"));
                if (projectVM.ShipToAddress.PostalCode != null)
                {
                    Assert.That(postalCode.GetAttribute("value"), Is.EqualTo(projectVM.ShipToAddress.PostalCode));
                }

                IWebElement squareFootage = driver.FindElement(By.Id("SquareFootage"));
                Assert.That(squareFootage.GetAttribute("value"), Is.EqualTo(projectVM.SquareFootage.ToString()));

                IWebElement numberOfFloors = driver.FindElement(By.Id("NumberOfFloors"));
                Assert.That(numberOfFloors.GetAttribute("value"), Is.EqualTo(projectVM.NumberOfFloors.ToString()));
            }
        }

        public void TestProjectEditViews_ProjectEditView_ShouldRenderPipeLineDetails(string testValue)
        {
            if(testValue == "CreateProject")
            {
                IWebElement tab_internal = driver.FindElement(By.Id("tab_internalInformation"));
                Assert.That(tab_internal.Text.ToLower(), Is.EqualTo("internal"));
                IWebElement ERPFirstOrderNumber = driver.FindElement(By.Id("ERPFirstOrderNumber"));
                Assert.That(ERPFirstOrderNumber, Is.Not.EqualTo(null));
                IWebElement ERPFirstOrderDate = driver.FindElement(By.Id("ERPFirstOrderDate"));
                Assert.That(ERPFirstOrderDate, Is.Not.EqualTo(null));
                IWebElement ERPFirstPONumber = driver.FindElement(By.Id("ERPFirstPONumber"));
                Assert.That(ERPFirstPONumber, Is.Not.EqualTo(null));
                IWebElement ERPFirstOrderComment = driver.FindElement(By.Id("ERPFirstOrderComment"));
                Assert.That(ERPFirstOrderComment, Is.Not.EqualTo(null));
            }
            else
            {
                IWebElement tab_internal = driver.FindElement(By.Id("tab_internalInformation"));
                Assert.That(tab_internal.Text.ToLower(), Is.EqualTo("internal"));

                IWebElement ERPFirstOrderNumber = driver.FindElement(By.Id("ERPFirstOrderNumber"));
                if (projectVM.ERPFirstOrderNumber != null)
                {
                    Assert.That(ERPFirstOrderNumber.GetAttribute("value"), Is.EqualTo(projectVM.ERPFirstOrderNumber.ToString()));
                }

                IWebElement ERPFirstOrderDate = driver.FindElement(By.Id("ERPFirstOrderDate"));
                if (projectVM.ERPFirstOrderDate != null && projectVM.ERPFirstOrderDate > DateTime.MinValue)
                {
                    Assert.That(Convert.ToDateTime(ERPFirstOrderDate.GetAttribute("value")),
                                Is.EqualTo(projectVM.ERPFirstOrderDate));
                }

                IWebElement ERPFirstPONumber = driver.FindElement(By.Id("ERPFirstPONumber"));
                if (!string.IsNullOrWhiteSpace(projectVM.ERPFirstPONumber))
                {
                    Assert.That(ERPFirstPONumber.GetAttribute("value"), Is.EqualTo(projectVM.ERPFirstPONumber));
                }

                IWebElement ERPFirstOrderComment = driver.FindElement(By.Id("ERPFirstOrderComment"));
                if (!string.IsNullOrWhiteSpace(projectVM.ERPFirstOrderComment))
                {
                    Assert.That(ERPFirstOrderComment.GetAttribute("value").Contains(projectVM.ERPFirstOrderComment), Is.EqualTo(true));
                }
            }
        }

        /// <summary>
        /// Test the buttons on the ProjectEditView 
        /// </summary>
        /// <param name="testValue"> test in two cases 'View' and 'Edit'</param>
        /// <param name="projectId"></param>
        public void TestProjectEditViews_ProjectEditView_Buttons(string testValue, long projectId)
        {
            IWebElement cancelBtn = driver.FindElement(By.Id("btnProjectCancel"));
            Assert.That(cancelBtn, Is.Not.EqualTo(null));
            Assert.That(cancelBtn.Text.ToLower(), Is.EqualTo("cancel"));
            Assert.That(cancelBtn.Enabled, Is.EqualTo(true));

            IWebElement createBtn = driver.FindElement(By.Id("ProjectEditFormSubmitBtn"));
            Assert.That(createBtn, Is.Not.EqualTo(null));
            Assert.That(createBtn.Enabled, Is.EqualTo(true));

            if (testValue == "CreateProject")
            {
                Assert.That(createBtn.Text.ToLower(), Is.EqualTo("create"));
                Assert.That(createBtn.GetAttribute("data-sc-ajaxpost").ToLower(),
                            Is.EqualTo("/projectdashboard/projectedit"));
            }
            else
            {
                Assert.That(createBtn.Text.ToLower(), Is.EqualTo("save changes"));
                Assert.That(createBtn.GetAttribute("data-sc-ajaxpost").ToLower(), 
                            Is.EqualTo("/projectdashboard/projectedit/" + projectId));
            }

            //Test the Click event of Create/SaveChange btn
            //make sure the validation is show for missing fields
            TestProjectEditViews_ProjectEditView_ShowValidationMessage(true);
            TestProjectEditViews_ProjectEditView_ShowValidationMessage(false);
        }

        /// <summary>
        /// Test the submit/Save change event of ProjectEditView
        /// </summary>
        /// <param name="modelValidate"> test two case 'model is validation' and 'model is not validation'</param>
        [Test]
        [Category("ProjectEditViews")]
        [TestCase(true)]
        [TestCase(false)]
        public void TestProjectEditViews_ProjectEditView_ShowValidationMessage(bool modelValidate)
        {
            if (modelValidate)
            {
                //filling all the require fields before Submit
                IWebElement projectName = driver.FindElement(By.Id("Name"));
                projectName.SendKeys("AA PRO" + DateTime.Now.ToShortDateString());

                IWebElement constructionTypeddl = driver.FindElement(By.Id("ConstructionTypeId"));
                SelectElement constructionTypeSelect = new SelectElement(constructionTypeddl);
                constructionTypeSelect.SelectByIndex(0);

                IWebElement projectTypeddl = driver.FindElement(By.Id("ProjectTypeId"));
                SelectElement projectTypeSelect = new SelectElement(projectTypeddl);
                projectTypeSelect.SelectByIndex(0);

                IWebElement projectOpenStatusTypeddl = driver.FindElement(By.Id("ProjectOpenStatusTypeId"));
                SelectElement projectOpenStatusTypeSelect = new SelectElement(projectOpenStatusTypeddl);
                projectOpenStatusTypeSelect.SelectByIndex(0);

                IWebElement verticalMarketTypeddl = driver.FindElement(By.Id("VerticalMarketTypeId"));
                SelectElement verticalMarketTypeSelect = new SelectElement(verticalMarketTypeddl);
                verticalMarketTypeSelect.SelectByIndex(0);
            }
            
            IWebElement createBtn = driver.FindElement(By.Id("ProjectEditFormSubmitBtn"));
            createBtn.Click();

            if(modelValidate)
            {
                TestProjectEditViews_ConfirmModelProjectCreatedView_ShouldShowConfirmModelProjectCreatedPopup();
            }
            else
            {
                IWebElement keyMessageName = driver.FindElement(By.Id("keyMessage_Name"));
                Assert.That(keyMessageName, Is.Not.EqualTo(null));
                Assert.That(keyMessageName.Text.ToLower(), Is.EqualTo("project name is required."));

                IWebElement keyMessageConstructionType = driver.FindElement(By.Id("keyMessage_ConstructionTypeId"));
                Assert.That(keyMessageConstructionType, Is.Not.EqualTo(null));
                Assert.That(keyMessageConstructionType.Text.ToLower(), Is.EqualTo("construction type is required."));

                IWebElement keyMessageProjectType = driver.FindElement(By.Id("keyMessage_ProjectTypeId"));
                Assert.That(keyMessageProjectType, Is.Not.EqualTo(null));
                Assert.That(keyMessageProjectType.Text.ToLower(), Is.EqualTo("project type is required."));

                IWebElement keyMessageProjectOpenStatusType = driver.FindElement(By.Id("keyMessage_ProjectOpenStatusTypeId"));
                Assert.That(keyMessageProjectOpenStatusType, Is.Not.EqualTo(null));
                Assert.That(keyMessageProjectOpenStatusType.Text.ToLower(), Is.EqualTo("project open status type is required."));

                IWebElement keyMessageVerticalMarketType = driver.FindElement(By.Id("keyMessage_VerticalMarketTypeiId"));
                Assert.That(keyMessageVerticalMarketType, Is.Not.EqualTo(null));
                Assert.That(keyMessageVerticalMarketType.Text.ToLower(), Is.EqualTo("vertical market is required."));

                IWebElement keyMessageBidDate = driver.FindElement(By.Id("keyMessage_BidDate"));
                Assert.That(keyMessageBidDate, Is.Not.EqualTo(null));
                Assert.That(keyMessageBidDate.Text.ToLower(), Is.EqualTo("bid date is required."));

                IWebElement keyMessageEstimateClose = driver.FindElement(By.Id("keyMessage_EstimateClose"));
                Assert.That(keyMessageEstimateClose, Is.Not.EqualTo(null));
                Assert.That(keyMessageEstimateClose.Text.ToLower(), Is.EqualTo("estimate close is required."));

                IWebElement keyMessageEstimateDelivery = driver.FindElement(By.Id("keyMessage_EstimateDelivery"));
                Assert.That(keyMessageEstimateDelivery, Is.Not.EqualTo(null));
                Assert.That(keyMessageEstimateDelivery.Text.ToLower(), Is.EqualTo("estimate delivery is required."));
            }
        }

        /// <summary>
        /// This will make sure the ProjectEditView will be render correctly.
        /// We will test the ProjectDetail, EngineerDetail, DealorDetail,SellerDetail,
        /// ShipToLocationDetail and PipeLineDetail tab views.
        /// </summary>
        /// <param name="testValue">This used to indicate the View is in create mode or 
        /// in Edit mode to be test</param>
        /// <param name="pipeLinePermission">this will be set user PipeLinePermission 
        /// to test the PipeLine View </param>
        [Test]
        [Category("ProjectEditViews")]
        [TestCase("EditProject", false)]
        [TestCase("EditProject", true)]
        [TestCase("CreateProject", false)]
        [TestCase("CreateProject", true)]
        public void TestProjectEditViews_ProjectEditView(string testValue, bool pipeLinePermission)
        {
            if(pipeLinePermission) // if user has ViewPipeLine || editPipeLine Permission
            {
                //select user that has ViewPipeLineData
                var userId = this.db.Context.Users.Where(u => u.Email == "test@123.com").Select(u => u.UserId).FirstOrDefault();
                user = accountService.GetUserSessionModel(userId).Model as UserSessionModel;

                //get the project belong to user
                var projectId = this.db.Context.Projects.Where(p => p.OwnerId == user.UserId &&
                                                               p.ERPFirstOrderNumber != null)
                                                               .OrderByDescending(p => p.ProjectId)
                                                               .Select(p => p.ProjectId).FirstOrDefault();

                projectVM = projectService.GetProjectModel(user, projectId).Model as ProjectModel;

                if (testValue == "EditProject")
                {
                    NavigateToProjectEditView(user, projectId);
                }
                else
                {
                    NavigateToProjectEditView(user);
                }

                TestProjectEditViews_ProjectEditView_ShouldRenderProjectDetail(testValue);
                TestProjectEditViews_ProjectEditView_ShouldRenderEngineeringDetails(testValue);
                TestProjectEditViews_ProjectEditView_ShouldRenderDealerDetails(testValue);
                TestProjectEditViews_ProjectEditView_ShouldRenderSellerDetails(testValue);
                TestProjectEditViews_ProjectEditView_ShouldRenderShipToLocationDetails(testValue);
                TestProjectEditViews_ProjectEditView_ShouldRenderPipeLineDetails(testValue);
            }
            else
            {
                if (testValue == "EditProject")
                {
                    NavigateToProjectEditView(user, projectId);
                }
                else
                {
                    NavigateToProjectEditView(user);
                }

                TestProjectEditViews_ProjectEditView_ShouldRenderProjectDetail(testValue);
                TestProjectEditViews_ProjectEditView_ShouldRenderEngineeringDetails(testValue);
                TestProjectEditViews_ProjectEditView_ShouldRenderDealerDetails(testValue);
                TestProjectEditViews_ProjectEditView_ShouldRenderSellerDetails(testValue);
                TestProjectEditViews_ProjectEditView_ShouldRenderShipToLocationDetails(testValue);
            }

            if(testValue == "EditProject")
            {
                TestProjectEditViews_ProjectEditView_Buttons(testValue, projectId);
            }
            else
            {
                TestProjectEditViews_ProjectEditView_Buttons(testValue, this.projectId);
            }
        }

        /// <summary>
        /// automatic navigate to ProjectEditView.
        /// </summary>
        /// <param name="user"> user logon the Daikin City system</param>
        /// <param name="projectId"></param>
        public void NavigateToProjectEditView(UserSessionModel user, long projectId = 0)
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
                if(user.Email == "User15@test.com")
                {
                    driver.Navigate().GoToUrl("http://User15@test.com:123456@tstsysdcity2/projectdashboard/ProjectEdit/" + projectId);
                }
                else
                {
                    driver.Navigate().GoToUrl("http://test@123.com:123456@tstsysdcity2/projectdashboard/ProjectEdit/" + projectId);
                }
                Thread.Sleep(2000);
            }

            
        }

       
    }
}
