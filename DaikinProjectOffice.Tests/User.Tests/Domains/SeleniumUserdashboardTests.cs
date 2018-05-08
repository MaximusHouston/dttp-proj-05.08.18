using System;
using OpenQA.Selenium;
using NUnit.Framework;
using NUnit.Common;


namespace DaikinProjectOffice.Tests
{
    [TestFixture]
    public class SeleniumUserdashboardTests:SeleniumTestDriver
    {
        private SeleniumAccountTests AccountTests;

        [Test]
        public void TestSelenium_User_Can_Navigate_To_Approval_Requests_When_Logged_In()
        {
            User_Can_Navigate_To_Approval_Requests_When_Logged_In(this.InternetExplorerDriver);
            User_Can_Navigate_To_Approval_Requests_When_Logged_In(this.FireFoxDriver);
        }

        private void User_Can_Navigate_To_Approval_Requests_When_Logged_In(IWebDriver driver)
        {
            AccountTests = new SeleniumAccountTests();
            AccountTests.Login(driver);

            driver.Url = this.GetAbsoluteUrl("/UserDashboard/ApprovalRequests");
            driver.Navigate();

            Assert.AreEqual(_siteUrl + "/UserDashboard/ApprovalRequests", driver.Url);
        }

    }
}
