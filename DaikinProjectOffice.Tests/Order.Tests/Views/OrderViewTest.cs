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

namespace DaikinProjectOffice.Tests
{
    [TestFixture]
    public class OrderViewTest: TestAdmin
    {
        private IWebDriver _driver;
        private ProjectServices _projectService;
        private AccountServices _accountService;
        private OrderServices _orderService;
        //private ProjectViewModel _projectVM;
        private ProjectModel _projectVM;
        private UserSessionModel user;
        private ProjectServiceLight _projectServiceLight;
        private long _projectId;
        private long _quoteId;
        private OrderServiceLight _orderServiceLight;
    
        public OrderViewTest() {
            
            _driver = new ChromeDriver(@"C:\Q2O\Source\Iterations\UnitTesting-PhaseI\Daikin Project Office\DaikinProjectOffice.Tests\libraries");

            _projectService = new ProjectServices();
            _accountService = new AccountServices();
            _orderService = new OrderServices();
            //_projectVM = new ProjectViewModel();
            _projectVM = new ProjectModel();
            _projectServiceLight = new ProjectServiceLight();
            _orderServiceLight = new OrderServiceLight();
     
            user = _accountService.GetUserSessionModel("User15@test.com").Model as UserSessionModel;

            _projectId = this.db.Context.Projects.Where(p => p.ProjectStatusTypeId == ProjectStatusTypeEnum.Open).OrderByDescending(p => p.ProjectTypeId).Select(p => p.ProjectId).FirstOrDefault();
            _quoteId = this.db.Context.Quotes.Where(q => q.ProjectId == _projectId).OrderByDescending(q => q.QuoteId).Select(q => q.QuoteId).FirstOrDefault();

            //_projectVM = _projectServiceLight.GetProjectModelWithQuote(user, _projectId, _quoteId).Model as ProjectViewModel;

            _projectVM = _projectService.GetProjectModel(user, _projectId).Model as ProjectModel;
        }
        
        [Test]
        [Category("OrderViews_OrderForm")]
        public void TestOrderViews_ShouldRenderProjectDetails()
        {
           
            _driver.Navigate().GoToUrl(@"http://tstsysdcity2/projectdashboard/OrderForm/" + _projectId + "/" + _quoteId +"#!/");
            
            Thread.Sleep(25000);//This is make sure we completed load the form before we manipulate DOM

            ReadOnlyCollection<IWebElement> projectDetailsLists = _driver
                                    .FindElement(By.Id("project-details"))
                                    .FindElements(By.ClassName("details-list"));

            ReadOnlyCollection<IWebElement> projectDetailsList1 = projectDetailsLists[0]
                                    .FindElement(By.TagName("ul"))
                                    .FindElements(By.TagName("li"));

            List<string> values = new List<string>();
            foreach(var item in projectDetailsList1)
            {
                ReadOnlyCollection<IWebElement> children = item.FindElements(By.XPath(".//*"));
                if(children[1].Text != string.Empty)
                values.Add(children[1].Text);
            }

            ReadOnlyCollection<IWebElement> projectDetailsList2 = projectDetailsLists[1]
                                    .FindElement(By.TagName("ul"))
                                    .FindElements(By.TagName("li"));

            foreach (var item in projectDetailsList2)
            {
                ReadOnlyCollection<IWebElement> children = item.FindElements(By.XPath(".//*"));
                if (children[1].Text != string.Empty)
                    values.Add(children[1].Text);
            }

            if (values.Count() > 1)
            {
                for (int a = 0; a < values.Count; a++)
                {
                    if (a != values.Count() - 1)
                    {
                        Assert.That(values[a], Is.Not.EqualTo(null));
                    }
                }

               for(int i =0; i < values.Count(); i++ )
               {
                    switch (i)
                    {
                        case 0:
                              Assert.That(values[i], Is.EqualTo(_projectVM.Name));
                            break;
                        case 1:
                              Assert.That(values[i], Is.EqualTo(_projectVM.ProjectId.ToString()));
                            break;
                        case 2:
                               Assert.That(values[i].Contains(_projectVM.ProjectDate.ToString()), Is.EqualTo(true));
                            break;
                        case 3:
                             Assert.That(values[i].Contains(_projectVM.BidDate.ToString()), Is.EqualTo(true));
                            break;
                        case 4:
                             Assert.That(values[i].Contains(_projectVM.EstimatedClose.ToString()), Is.EqualTo(true));
                            break;
                        case 5:
                             Assert.That(values[i].Contains(_projectVM.EstimatedDelivery.ToString()), Is.EqualTo(true));
                            break;          
                        case 7:
                            Assert.That(values[i], Is.EqualTo(_projectVM.ConstructionTypeDescription));
                            break;
                        case 8:
                            Assert.That(values[i], Is.EqualTo(_projectVM.ProjectStatusDescription));
                            break;
                        case 9:
                            Assert.That(values[i], Is.EqualTo(_projectVM.ProjectTypeDescription));
                            break;
                        case 10:
                            Assert.That(values[i], Is.EqualTo(_projectVM.ProjectOpenStatusDescription));
                            break;
                        case 11:
                            Assert.That(values[i], Is.EqualTo(_projectVM.VerticalMarketDescription));
                            break;
                        case 12:
                            Assert.That(values[i], Is.EqualTo(_projectVM.Description));
                            break;
                    }
                }
            }
        }

        [Test]
        [Category("OrderViews_OrderForm")]
        public void TestOrderViews_ShouldRenderProjectLocation()
        {
            ShipToAddressViewModel ShipToAddressModel = _projectServiceLight.GetProjectLocation(user, _projectId).Model as ShipToAddressViewModel;
            string state = this.db.Context.States.Where(s => s.StateId == ShipToAddressModel.StateId).Select(s => s.Name).FirstOrDefault();
            
            _driver.Navigate().GoToUrl(@"http://tstsysdcity2/projectdashboard/OrderForm/" + _projectId + "/" + _quoteId + "#!/");

            Thread.Sleep(25000);//This is make sure we completed load the form before we manipulate DOM

            ReadOnlyCollection<IWebElement> projectLocationLists = _driver
                                    .FindElement(By.Id("projectLocationDiv"))
                                    .FindElements(By.ClassName("details-list"));

            ReadOnlyCollection<IWebElement> projectLocationList1 = projectLocationLists[0]
                                    .FindElement(By.TagName("ul"))
                                    .FindElements(By.TagName("li"));

            List<string> values = new List<string>();
            foreach (var item in projectLocationList1)
            {
                ReadOnlyCollection<IWebElement> children = item.FindElements(By.XPath(".//*"));
                if(children[1].Text != string.Empty)
                values.Add(children[1].Text);
            }
           
            ReadOnlyCollection<IWebElement> projectLocationList2 = projectLocationLists[1]
                                   .FindElement(By.TagName("ul"))
                                   .FindElements(By.TagName("li"));
            
            foreach (var item in projectLocationList2)
            {
                ReadOnlyCollection<IWebElement> children = item.FindElements(By.XPath(".//*"));
                if (children[1].Text != string.Empty)
                    values.Add(children[1].Text);
            }

            if (values.Count() > 1)
            {
                for (int a = 0; a < values.Count; a++)
                {
                    if (a != values.Count() - 1)
                    Assert.That(values[a], Is.Not.EqualTo(null));
                }

                for (int i = 0; i < values.Count(); i++)
                {
                    switch (i)
                    {
                        case 0:
                            Assert.That(values[i], Is.EqualTo(ShipToAddressModel.ShipToName));
                            break;
                        case 1:
                            Assert.That(values[i], Is.EqualTo(ShipToAddressModel.CountryCode));
                            break;
                        case 2:
                            Assert.That(values[i], Is.EqualTo(ShipToAddressModel.AddressLine1));
                            break;
                        case 3:
                            Assert.That(values[i], Is.EqualTo(ShipToAddressModel.Location));
                            break;
                        case 4:
                            Assert.That(values[i], Is.EqualTo(state));
                            break;
                        case 5:
                            Assert.That(values[i], Is.EqualTo(ShipToAddressModel.PostalCode));
                            break;
                        case 6:
                            Assert.That(values[i], Is.EqualTo(ShipToAddressModel.SquareFootage.ToString()));
                            break;
                        case 7:
                            Assert.That(values[i], Is.EqualTo(ShipToAddressModel.NumberOfFloor.ToString()));
                            break;

                    }
                }

            }
        }
    
        [Test]
        [Category("OrderViews_OrderForm")]
        public void TestOrderViews_ShouldRenderDealerLocation()
        {
            DealerContractorInfoViewModel dealerAddressModel = _projectServiceLight.GetDealerContractorInfo(user, _projectId).Model as DealerContractorInfoViewModel;
            string state = this.db.Context.States.Where(s => s.StateId == dealerAddressModel.StateId).Select(s => s.Name).FirstOrDefault();

            _driver.Navigate().GoToUrl(@"http://tstsysdcity2/projectdashboard/OrderForm/" + _projectId + "/" + _quoteId + "#!/");

            Thread.Sleep(25000);//This is make sure we completed load the form before we manipulate DOM

            ReadOnlyCollection<IWebElement> dealerLocationLists = _driver
                                    .FindElement(By.Id("dealerContractorInfo"))
                                    .FindElements(By.ClassName("details-list"));

            ReadOnlyCollection<IWebElement> dealerLocationList1 = dealerLocationLists[0]
                                    .FindElement(By.TagName("ul"))
                                    .FindElements(By.TagName("li"));

            List<string> values = new List<string>();
            foreach (var item in dealerLocationList1)
            {
                ReadOnlyCollection<IWebElement> children = item.FindElements(By.XPath(".//*"));
                if (children[1].Text != string.Empty)
                    values.Add(children[1].Text);
            }

            ReadOnlyCollection<IWebElement> dealerLocationList2 = dealerLocationLists[1]
                                   .FindElement(By.TagName("ul"))
                                   .FindElements(By.TagName("li"));

            foreach (var item in dealerLocationList2)
            {
                ReadOnlyCollection<IWebElement> children = item.FindElements(By.XPath(".//*"));
                if (children[1].Text != string.Empty)
                    values.Add(children[1].Text);
            }

            if (values.Count() > 1)
            {
                for (int a = 0; a < values.Count; a++)
                {
                    if (a != values.Count() - 1)
                        Assert.That(values[a], Is.Not.EqualTo(null));
                }

                for (int i = 0; i < values.Count(); i++)
                {
                    switch (i)
                    {
                        case 0:
                            Assert.That(values[i], Is.EqualTo(dealerAddressModel.DealerContractorName));
                            break;
                        case 1:
                            Assert.That(values[i], Is.EqualTo(dealerAddressModel.CustomerName));
                            break;
                        case 2:
                            Assert.That(values[i], Is.EqualTo(dealerAddressModel.CountryCode));
                            break;
                        case 3:
                            Assert.That(values[i], Is.EqualTo(dealerAddressModel.AddressLine1));
                            break;
                        case 4:
                            Assert.That(values[i], Is.EqualTo(dealerAddressModel.Location));
                            break;
                        case 5:
                            Assert.That(values[i], Is.EqualTo(state));
                            break;
                        case 6:
                            Assert.That(values[i], Is.EqualTo(dealerAddressModel.PostalCode));
                            break;
                    }
                }

            }
        }

        [Test]
        [Category("OrderViews_OrderForm")]
        public void TestOrderViews_ShouldRenderSellerLocation()
        {
            SellerInfoViewModel sellerAddressModel = _projectServiceLight.GetSellerInfo(user, _projectId).Model as SellerInfoViewModel;
            string state = this.db.Context.States.Where(s => s.StateId == sellerAddressModel.StateId).Select(s => s.Name).FirstOrDefault();

            _driver.Navigate().GoToUrl(@"http://tstsysdcity2/projectdashboard/OrderForm/" + _projectId + "/" + _quoteId + "#!/");

            Thread.Sleep(25000);//This is make sure we completed load the form before we manipulate DOM

            ReadOnlyCollection<IWebElement> sellerLocationLists = _driver
                                    .FindElement(By.Id("sellerInfoDiv"))
                                    .FindElements(By.ClassName("details-list"));

            ReadOnlyCollection<IWebElement> sellerLocationList1 = sellerLocationLists[0]
                                    .FindElement(By.TagName("ul"))
                                    .FindElements(By.TagName("li"));

            List<string> values = new List<string>();
            foreach (var item in sellerLocationList1)
            {
                ReadOnlyCollection<IWebElement> children = item.FindElements(By.XPath(".//*"));
                if (children[1].Text != string.Empty)
                    values.Add(children[1].Text);
            }

            ReadOnlyCollection<IWebElement> sellerLocationList2 = sellerLocationLists[1]
                                   .FindElement(By.TagName("ul"))
                                   .FindElements(By.TagName("li"));

            foreach (var item in sellerLocationList2)
            {
                ReadOnlyCollection<IWebElement> children = item.FindElements(By.XPath(".//*"));
                if (children[1].Text != string.Empty)
                    values.Add(children[1].Text);
            }

            if (values.Count() > 1)
            {
                for (int a = 0; a < values.Count; a++)
                {
                    if (a != values.Count() - 1)
                        Assert.That(values[a], Is.Not.EqualTo(null));
                }

                for (int i = 0; i < values.Count(); i++)
                {
                    switch (i)
                    {
                        case 0:
                            Assert.That(values[i], Is.EqualTo(sellerAddressModel.SellerName));
                            break;
                        case 1:
                            Assert.That(values[i], Is.EqualTo(sellerAddressModel.CountryCode));
                            break;
                        case 2:
                            Assert.That(values[i], Is.EqualTo(sellerAddressModel.AddressLine1));
                            break;
                        case 3:
                            Assert.That(values[i], Is.EqualTo(sellerAddressModel.Location));
                            break;
                        case 4:
                            Assert.That(values[i], Is.EqualTo(state));
                            break;
                        case 5:
                            Assert.That(values[i], Is.EqualTo(sellerAddressModel.PostalCode));
                            break;
                    }
                }

            }
        }

        [Test]
        [Category("OrderViews_OrderForm")]
        public void TestOrderViews_ShouldRenderOrderDetails()
        {
            OrderViewModelLight orderVMLight = _orderServiceLight.GetNewOrder(user, _quoteId).Model as OrderViewModelLight;
      
            _driver.Navigate().GoToUrl(@"http://tstsysdcity2/projectdashboard/OrderForm/" + _projectId + "/" + _quoteId + "#!/");

            Thread.Sleep(25000);//This is make sure we completed load the form before we manipulate DOM

            ReadOnlyCollection<IWebElement> orderDetailsLists = _driver
                                    .FindElement(By.Id("orderDetailsDiv"))
                                    .FindElements(By.ClassName("details-list"));

            ReadOnlyCollection<IWebElement> orderDetailsList1 = orderDetailsLists[0]
                                    .FindElement(By.TagName("ul"))
                                    .FindElements(By.TagName("li"));

            List<string> values = new List<string>();
            foreach (var item in orderDetailsList1)
            {
                ReadOnlyCollection<IWebElement> children = item.FindElements(By.XPath(".//*"));
                if (children.Count() < 3)
                {
                    if (children[1].Text != string.Empty)
                        values.Add(children[1].Text);
                }
            }

            ReadOnlyCollection<IWebElement> orderDetailsList2 = orderDetailsLists[1]
                                   .FindElement(By.TagName("ul"))
                                   .FindElements(By.TagName("li"));

            foreach (var item in orderDetailsList2)
            {
                ReadOnlyCollection<IWebElement> children = item.FindElements(By.XPath(".//*"));
                if (children.Count() < 3)
                {
                    if (children[1].Text != string.Empty)
                    values.Add(children[1].Text);
                }
            }

            if (values.Count() > 1)
            {
                for (int a = 0; a < values.Count; a++)
                {
                    if (a != values.Count() - 1)
                        Assert.That(values[a], Is.Not.EqualTo(null));
                }

                for (int i = 0; i < values.Count(); i++)
                {
                    switch (i)
                    {
                        case 0:
                            Assert.That(values[i], Is.EqualTo(orderVMLight.SubmittedByUserName));
                            break;
                        case 1:
                            if (orderVMLight.OrderId != 0)
                            Assert.That(values[i], Is.EqualTo(orderVMLight.OrderReleaseDate.ToString("MM/dd/yyyy")));
                            break;
                        case 2:
                            if(orderVMLight.OrderId != 0)
                            Assert.That(values[i], Is.EqualTo(orderVMLight.PONumber));
                            break;
                        case 3:
                            Assert.That(values[i], Is.EqualTo(orderVMLight.TotalNetPrice.ToString()));
                            break;
                        case 4:
                            Assert.That(values[i], Is.EqualTo(orderVMLight.TotalDiscountPercent.ToString()));
                            break;
                        case 5:
                            if(orderVMLight.OrderId != 0)
                            Assert.That(values[i], Is.EqualTo(orderVMLight.Comments));
                            break;
                    }
                }

            }
        }

        [Test]
        [Category("OrderViews_OrderForm")]
        public void TestOrderViews_ShouldRenderOrderDetailsInReadOnly()
        {
           
            var query = from q in this.db.Context.Quotes
                         join p in this.db.Context.Projects
                         on q.ProjectId equals p.ProjectId
                         join o in this.db.Context.Orders
                         on q.QuoteId equals o.QuoteId
                         where p.OwnerId == user.UserId
                         select new 
                         {
                             QuoteId = q.QuoteId,
                             ProjectId = p.ProjectId
                         };

            var result = query.FirstOrDefault();
            OrderViewModel model = _orderService.GetOrderInQuote(user, result.ProjectId, result.QuoteId).Model as OrderViewModel;

            _driver.Navigate().GoToUrl(@"http://tstsysdcity2/projectdashboard/OrderForm/" + result.ProjectId + "/" + result.QuoteId + "#!/");

            Thread.Sleep(25000);//This is make sure we completed load the form before we manipulate DOM

            ReadOnlyCollection<IWebElement> orderDetailsLists = _driver
                                    .FindElement(By.Id("orderDetailsDiv"))
                                    .FindElements(By.ClassName("details-list"));

            ReadOnlyCollection<IWebElement> orderDetailsList1 = orderDetailsLists[0]
                                    .FindElement(By.TagName("ul"))
                                    .FindElements(By.TagName("li"));

            List<string> values = new List<string>();
            foreach (var item in orderDetailsList1)
            {
                ReadOnlyCollection<IWebElement> children = item.FindElements(By.XPath(".//*"));
                if (children.Count() < 3)
                {
                    if (children[0].Text.Contains("Purchase Order Number"))
                    {
                        IWebElement PONumber = _driver.FindElement(By.Name("PONumber"));
                        if (PONumber.GetAttribute("value") != string.Empty)
                            values.Add(PONumber.GetAttribute("value"));
                    }
                    else
                    {
                        if (children[1].Text != string.Empty)
                            values.Add(children[1].Text);
                    }
                }
                else if(children.Count() == 9)
                {
                    if(children[0].Text.Contains("Order Release Date"))
                    {
                        values.Add(children[8].Text);
                    }
                    else
                    {
                        values.Add(children[1].Text);
                    }
                }
            }

            ReadOnlyCollection<IWebElement> orderDetailsList2 = orderDetailsLists[1]
                                   .FindElement(By.TagName("ul"))
                                   .FindElements(By.TagName("li"));

            foreach (var item in orderDetailsList2)
            {
                ReadOnlyCollection<IWebElement> children = item.FindElements(By.XPath(".//*"));
                if (children.Count() < 3)
                {
                    if (children[1].Text != string.Empty)
                        values.Add(children[1].Text);
                }
                else
                {
                    if(children.Count() == 4)
                    {
                        IWebElement comments = _driver.FindElement(By.Name("Comments"));
                        if (comments.GetAttribute("value") != string.Empty)
                            values.Add(comments.GetAttribute("value"));
                    }
                }
            }

            if (values.Count() > 1)
            {
                for (int a = 0; a < values.Count; a++)
                {
                    if (a != values.Count() - 1)
                        Assert.That(values[a], Is.Not.EqualTo(null));
                }

                for (int i = 0; i < values.Count(); i++)
                {
                    switch (i)
                    {
                        case 0:
                            var userName = this.db.Context.Users.Where(u => u.UserId == model.SubmittedByUserId).Select(u => new { u.FirstName, u.LastName }).FirstOrDefault();
                            Assert.That(values[i], Is.EqualTo(userName.LastName + ", " + userName.FirstName));
                            break;
                        case 1:
                                Assert.That(values[i], Is.EqualTo(model.OrderReleaseDate.ToString("MM/dd/yyyy")));
                            break;
                        case 2:
                                Assert.That(values[i], Is.EqualTo(model.PONumber));
                            break;
                        case 3:
                                Assert.That(values[i], Is.EqualTo(model.POAttachmentFileName));
                            break;
                        case 4:
                            Assert.That(values[i], Is.EqualTo(string.Format("{0:C}",model.TotalNetPrice)));
                            break;
                        case 5:
                            Assert.That(values[i], Is.EqualTo(string.Format("{0:0.000%}", model.Quote.DiscountPercentage)));
                            break;
                        case 6:
                                Assert.That(values[i], Is.EqualTo(model.Comments));
                            break;
                    }
                }

            }
        }

        [Test]
        [Category("OrderViews_OrderFormEdit")]
        public void TestOrderViews_ShouldRenderProjectLocationEdit()
        {
            ShipToAddressViewModel ShipToAddressModel = _projectServiceLight.GetProjectLocation(user, _projectId).Model as ShipToAddressViewModel;
            string state = this.db.Context.States.Where(s => s.StateId == ShipToAddressModel.StateId).Select(s => s.Name).FirstOrDefault();

            _driver.Navigate().GoToUrl(@"http://tstsysdcity2/projectdashboard/OrderForm/" + _projectId + "/" + _quoteId + "#!/");

            Thread.Sleep(25000);//This is make sure we completed load the form before we manipulate DOM

            ReadOnlyCollection<IWebElement> projectLocationFields = _driver
                                    .FindElements(By.ClassName("ProjectLocationfields"));

            List<string> values = new List<string>();
            foreach (var item in projectLocationFields)
            {
                IWebElement child = item;
                if (child.GetAttribute("value") != string.Empty)
                    values.Add(child.GetAttribute("value"));
            }

            if (values.Count() > 1)
            {
                for (int a = 0; a < values.Count; a++)
                {
                    if (a != values.Count() - 1)
                        Assert.That(values[a], Is.Not.EqualTo(null));
                }

                for (int i = 0; i < values.Count(); i++)
                {
                    switch (i)
                    {
                        case 0:
                            Assert.That(values[i], Is.EqualTo(ShipToAddressModel.ShipToName));
                            break;
                        case 1:
                            Assert.That(values[i], Is.EqualTo(ShipToAddressModel.CountryCode));
                            break;
                        case 2:
                            Assert.That(values[i], Is.EqualTo(ShipToAddressModel.AddressLine1));
                            break;
                        case 3:
                            Assert.That(values[i], Is.EqualTo(ShipToAddressModel.Location));
                            break;
                        case 4:
                            Assert.That(values[i], Is.EqualTo(ShipToAddressModel.StateId.ToString()));
                            break;
                        case 5:
                            Assert.That(values[i], Is.EqualTo(ShipToAddressModel.PostalCode.ToString()));
                            break;
                        case 6:
                            Assert.That(values[i], Is.EqualTo(ShipToAddressModel.SquareFootage.ToString()));
                            break;
                        case 7:
                            Assert.That(values[i], Is.EqualTo(ShipToAddressModel.NumberOfFloor.ToString()));
                            break;
                    }
                }
                //check for Cancel and Submit button visible
                Assert.That(_driver.FindElement(By.ClassName("submit")).Displayed, Is.EqualTo(true));
                Assert.That(_driver.FindElement(By.ClassName("btn-default")).Displayed, Is.EqualTo(true));
                //Check for Cancel and Submit button State
                Assert.That(_driver.FindElement(By.ClassName("submit")).Enabled, Is.EqualTo(true));
                Assert.That(_driver.FindElement(By.ClassName("btn-default")).Enabled, Is.EqualTo(true));
            }
        }

        [Test]
        [Category("OrderViews_OrderFormEdit")]
        public void TestOrderViews_ShouldRenderSellerInfoEdit()
        {
            SellerInfoViewModel sellerAddressModel = _projectServiceLight.GetSellerInfo(user, _projectId).Model as SellerInfoViewModel;
            _driver.Navigate().GoToUrl(@"http://tstsysdcity2/projectdashboard/OrderForm/" + _projectId + "/" + _quoteId + "#!/");

            Thread.Sleep(25000);//This is make sure we completed load the form before we manipulate DOM

            ReadOnlyCollection<IWebElement> sellerInoFields = _driver
                                    .FindElements(By.ClassName("SellerInfoFields"));

            List<string> values = new List<string>();
            foreach (var item in sellerInoFields)
            {
                IWebElement child = item;
                if (child.GetAttribute("value") != string.Empty)
                    values.Add(child.GetAttribute("value"));
            }

            if (values.Count() > 1)
            {
                for (int a = 0; a < values.Count; a++)
                {
                    if (a != values.Count() - 1)
                        Assert.That(values[a], Is.Not.EqualTo(null));
                }

                for (int i = 0; i < values.Count(); i++)
                {
                    switch (i)
                    {
                        case 0:
                            Assert.That(values[i], Is.EqualTo(sellerAddressModel.SellerName));
                            break;
                        case 1:
                            Assert.That(values[i], Is.EqualTo(sellerAddressModel.CountryCode));
                            break;
                        case 2:
                            Assert.That(values[i], Is.EqualTo(sellerAddressModel.AddressLine1));
                            break;
                        case 3:
                            Assert.That(values[i], Is.EqualTo(sellerAddressModel.Location));
                            break;
                        case 4:
                            Assert.That(values[i], Is.EqualTo(sellerAddressModel.StateId.ToString()));
                            break;
                        case 5:
                            Assert.That(values[i], Is.EqualTo(sellerAddressModel.PostalCode.ToString()));
                            break;
                    }
                }
                //check for Cancel and Submit button visible
                Assert.That(_driver.FindElement(By.ClassName("submit")).Displayed, Is.EqualTo(true));
                Assert.That(_driver.FindElement(By.ClassName("btn-default")).Displayed, Is.EqualTo(true));
                //Check for Cancel and Submit button State
                Assert.That(_driver.FindElement(By.ClassName("submit")).Enabled, Is.EqualTo(true));
                Assert.That(_driver.FindElement(By.ClassName("btn-default")).Enabled, Is.EqualTo(true));
            }
        }

        [Test]
        [Category("OrderViews_OrderFormEdit")]
        public void TestOrderViews_ShouldRenderDealerInfoEdit()
        {
            DealerContractorInfoViewModel dealerAddressModel = _projectServiceLight.GetDealerContractorInfo(user, _projectId).Model as DealerContractorInfoViewModel;
            _driver.Navigate().GoToUrl(@"http://tstsysdcity2/projectdashboard/OrderForm/" + _projectId + "/" + _quoteId + "#!/");

            Thread.Sleep(25000);//This is make sure we completed load the form before we manipulate DOM

            ReadOnlyCollection<IWebElement> dealerInfoFields = _driver
                                    .FindElements(By.ClassName("DealerInfoFields"));

            List<string> values = new List<string>();
            foreach (var item in dealerInfoFields)
            {
                IWebElement child = item;
                if (child.GetAttribute("value") != string.Empty)
                    values.Add(child.GetAttribute("value"));
            }

            if (values.Count() > 1)
            {
                for (int a = 0; a < values.Count; a++)
                {
                    if (a != values.Count() - 1)
                        Assert.That(values[a], Is.Not.EqualTo(null));
                }

                for (int i = 0; i < values.Count(); i++)
                {
                    switch (i)
                    {
                        case 0:
                            Assert.That(values[i], Is.EqualTo(dealerAddressModel.DealerContractorName));
                            break;
                        case 1:
                            Assert.That(values[i], Is.EqualTo(dealerAddressModel.CustomerName));
                            break;
                        case 2:
                            Assert.That(values[i], Is.EqualTo(dealerAddressModel.AddressLine1));
                            break;
                        case 3:
                            Assert.That(values[i], Is.EqualTo(dealerAddressModel.CountryCode));
                            break;
                        case 4:
                            Assert.That(values[i], Is.EqualTo(dealerAddressModel.Location));
                            break;
                        case 5:
                            Assert.That(values[i], Is.EqualTo(dealerAddressModel.StateId.ToString()));
                            break;
                        case 6:
                            Assert.That(values[i], Is.EqualTo(dealerAddressModel.PostalCode.ToString()));
                            break;
                    }
                }
            }

            //check for Cancel and Submit button visible
            Assert.That(_driver.FindElement(By.ClassName("submit")).Displayed, Is.EqualTo(true));
            Assert.That(_driver.FindElement(By.ClassName("btn-default")).Displayed, Is.EqualTo(true));
            //Check for Cancel and Submit button State
            Assert.That(_driver.FindElement(By.ClassName("submit")).Enabled, Is.EqualTo(true));
            Assert.That(_driver.FindElement(By.ClassName("btn-default")).Enabled, Is.EqualTo(true));
        }

        [Test]
        [Category("OrderViews_SubmitEdit")]
        public void TestOrderViews_ShouldUpdateProjectLocationOnView()
        {
            _driver.Navigate().GoToUrl(@"http://tstsysdcity2/projectdashboard/OrderForm/" + _projectId + "/" + _quoteId + "#!/");
            Thread.Sleep(25000);//This is make sure we completed load the form before we manipulate DOM

            IWebElement projectBusinessName = _driver.FindElement(By.Name("BussinessName"));
            projectBusinessName.Clear(); // clear the old value before sending the new value
            projectBusinessName.SendKeys("Aaron Project Business US");
            _driver.FindElement(By.Id("ProjectLocationEditBtn")).Click();

            projectBusinessName = _driver.FindElement(By.Id("BussinessName"));
            Assert.That(projectBusinessName.GetAttribute("value"), Is.EqualTo("Aaron Project Business US"));
        }

        [Test]
        [Category("OrderViews_SubmitEdit")]
        public void TestOrderViews_ShouldUpdateDealerInfoOnView()
        {
            _driver.Navigate().GoToUrl(@"http://tstsysdcity2/projectdashboard/OrderForm/" + _projectId + "/" + _quoteId + "#!/");
            Thread.Sleep(25000);//This is make sure we completed load the form before we manipulate DOM
            
            IWebElement dealerName = _driver.FindElement(By.Name("DealerContractorName"));
            dealerName.Clear(); // clear the old value before sending the new value
            dealerName.SendKeys("Aaron Dealer US");
            _driver.FindElement(By.Id("DealerInfoEditBtn")).Click();
          
            dealerName = _driver.FindElement(By.Id("DealerContractorName"));
            Assert.That(dealerName.GetAttribute("value"), Is.EqualTo("Aaron Dealer US"));
        }

        [Test]
        [Category("OrderViews_SubmitEdit")]
        public void TestOrderViews_ShouldUpdateSellerInfoOnView()
        {
            _driver.Navigate().GoToUrl(@"http://tstsysdcity2/projectdashboard/OrderForm/" + _projectId + "/" + _quoteId + "#!/");
            Thread.Sleep(25000);//This is make sure we completed load the form before we manipulate DOM

            IWebElement sellerName = _driver.FindElement(By.Name("SellerName"));
            sellerName.Clear(); // clear the old value before sending the new value
            sellerName.SendKeys("Aaron Seller US");
            _driver.FindElement(By.Id("SellerInfoEditBtn")).Click();

            sellerName = _driver.FindElement(By.Id("SellerName"));
            Assert.That(sellerName.GetAttribute("value"), Is.EqualTo("Aaron Seller US"));
        }

        [Test]
        [Category("OrderViews_SubmitEdit")]
        [TestCase("ProjectLocationCancelBtn")]
        [TestCase("SellerEditCancelBtn")]
        [TestCase("DealerInfoCancelBtn")]
        public void TestOrderViews_CancelButton_ShouldReturnBackToOrderForm(string buttonId)
        {
            _driver.Navigate().GoToUrl(@"http://tstsysdcity2/projectdashboard/OrderForm/" + _projectId + "/" + _quoteId + "#!/");
            Thread.Sleep(25000);//This is make sure we completed load the form before we manipulate DOM

            IWebElement cancelBtn = _driver.FindElement(By.Id(buttonId));
            if (cancelBtn.Displayed && cancelBtn.Enabled)
            {
                cancelBtn.Click();
                IWebElement popupWindow = _driver.FindElement(By.ClassName("k-window"));
                Assert.That(popupWindow.GetCssValue("display"), Is.EqualTo("none"));
            }
        }

        [Test]
        [Category("OrderViews_SubmitOrder")]
        public void TestOrderViews_ShouldRenderProjectLocationValidationMessageBeforeSubmitOrder()
        {
            ShipToAddressViewModel ShipToAddressModel = _projectServiceLight.GetProjectLocation(user, _projectId).Model as ShipToAddressViewModel;
            _driver.Navigate().GoToUrl(@"http://tstsysdcity2/projectdashboard/OrderForm/" + _projectId + "/" + _quoteId + "#!/");
            Thread.Sleep(25000);//This is make sure we completed load the form before we manipulate DOM

            IWebElement submitBtn = _driver.FindElement(By.Id("OrderFormSubmitBtn"));

            
            ShipToAddressModel.ShipToName = null;

            if (submitBtn.Displayed && submitBtn.Enabled)
            {
                submitBtn.Click();

                if(ShipToAddressModel.ShipToName == string.Empty || ShipToAddressModel.ShipToName == null)
                {
                    IWebElement projectLocationDiv = _driver.FindElement(By.Id("projectLocationDiv"));
                    IReadOnlyCollection<IWebElement> projectDetailsLists = projectLocationDiv.FindElements(By.TagName("li"));

                    foreach(var item in projectDetailsLists)
                    {
                        Assert.That(item.Text.Contains("Business Name is required"), Is.EqualTo(true));
                        break;
                    }
                }
                
            }
        }

        [Test]
        [Category("OrderViews_SubmitOrder")]
        public void TestOrderViews_ShouldRenderDealorValidationMessageBeforeSubmitOrder()
        {
            DealerContractorInfoViewModel dealerAddressModel = _projectServiceLight.GetDealerContractorInfo(user, _projectId).Model as DealerContractorInfoViewModel;
            _driver.Navigate().GoToUrl(@"http://tstsysdcity2/projectdashboard/OrderForm/" + _projectId + "/" + _quoteId + "#!/");

            Thread.Sleep(25000);//This is make sure we completed load the form before we manipulate DOM

            IWebElement submitBtn = _driver.FindElement(By.Id("OrderFormSubmitBtn"));


            dealerAddressModel.DealerContractorName = null;

            if (submitBtn.Displayed && submitBtn.Enabled)
            {
                submitBtn.Click();

                if (dealerAddressModel.DealerContractorName == string.Empty || dealerAddressModel.DealerContractorName == null)
                {
                    IWebElement dealerContractorDiv = _driver.FindElement(By.Id("dealerContractorInfo"));
                    IReadOnlyCollection<IWebElement> dealerContractLists = dealerContractorDiv.FindElements(By.TagName("li"));

                    foreach (var item in dealerContractLists)
                    {
                        Assert.That(item.Text.Contains("Dealer/Contractor Name is required."), Is.EqualTo(true));
                        break;
                    }
                }

            }
        }

        [Test]
        [Category("OrderViews_SubmitOrder")]
        public void TestOrderViews_ShouldRenderSellerValidationMessageBeforeSubmitOrder()
        {
            SellerInfoViewModel sellerAddressModel = _projectServiceLight.GetSellerInfo(user, _projectId).Model as SellerInfoViewModel;
            _driver.Navigate().GoToUrl(@"http://tstsysdcity2/projectdashboard/OrderForm/" + _projectId + "/" + _quoteId + "#!/");

            Thread.Sleep(25000);//This is make sure we completed load the form before we manipulate DOM

            IWebElement submitBtn = _driver.FindElement(By.Id("OrderFormSubmitBtn"));

            sellerAddressModel.SellerName = null;

            if (submitBtn.Displayed && submitBtn.Enabled)
            {
                submitBtn.Click();

                if (sellerAddressModel.SellerName == string.Empty || sellerAddressModel.SellerName == null)
                {
                    IWebElement sellerInfoDiv = _driver.FindElement(By.Id("sellerInfoDiv"));
                    IReadOnlyCollection<IWebElement> sellerInfoLists = sellerInfoDiv.FindElements(By.TagName("li"));

                    foreach (var item in sellerInfoLists)
                    {
                        Assert.That(item.Text.Contains("Business Name is required."), Is.EqualTo(true));
                        break;
                    }
                }

            }
        }

        [Test]
        [Category("OrderViews_SubmitOrder")]
        public void TestOrderViews_ShouldRenderConfirmMessageIfSuccessfulSubmitOrder()
        {
            _driver.Navigate().GoToUrl(@"http://tstsysdcity2/projectdashboard/OrderForm/" + _projectId + "/" + _quoteId + "#!/");
            Thread.Sleep(25000);//This is make sure we completed load the form before we manipulate DOM

            IWebElement submitBtn = _driver.FindElement(By.Id("OrderFormSubmitBtn"));

            if (submitBtn.Displayed && submitBtn.Enabled)
            {
                submitBtn.Click();
            }

            IWebElement popupWindow = _driver.FindElement(By.ClassName("modal-dialog"));
            Assert.That(popupWindow.GetCssValue("display"), Is.EqualTo("block"));
        }

        [Test]
        [Category("OrderViews_OrderForm")]
        public void TestOrderViews_ShouldRenderFAQPageWhenClicktheFAQbutton()
        {
            IWebElement FAQBtn = _driver.FindElement(By.Id("orderFAQ"));
            if (FAQBtn.Enabled)
                Assert.That(FAQBtn.GetAttribute("href").Contains("OrderFAQ"), Is.EqualTo(true));
        }
    }
}
