using System;
using OpenQA.Selenium;
using NUnit.Framework;
using NUnit.Common;

namespace DaikinProjectOffice.Tests
{
    [TestFixture]
    public class SeleniumAccountTests:SeleniumTestDriver
    {
        private string LandingPageUrl; //this needs to be set to real landing page url

        public void Login(IWebDriver driver)
        {
            LandingPageUrl = _siteUrl + "/";

            driver.Url = this.GetAbsoluteUrl("/Account/Login");
            driver.Navigate();
            driver.FindElement(By.Id("Email")).Click();
            driver.FindElement(By.Id("Email")).SendKeys("ussa0@somewhere.com");
            driver.FindElement(By.Id("Password")).Click();
            driver.FindElement(By.Id("Password")).SendKeys("test");
            driver.FindElement(By.CssSelector("input[type=\"submit\"]")).Click();
        }

        [Test]
        public void TestSelenium_User_Cannot_Log_In_With_Incorrect_Credentials()
        {
            User_Cannot_Log_In_With_Incorrect_Credentials(this.InternetExplorerDriver);
            User_Cannot_Log_In_With_Incorrect_Credentials(this.FireFoxDriver);
            User_Cannot_Log_In_With_Incorrect_Credentials(this.ChromeDriver);
        }

        private void User_Cannot_Log_In_With_Incorrect_Credentials(IWebDriver driver)
        {
            driver.Url = this.GetAbsoluteUrl("/Account/Login");
            driver.Navigate();
            driver.FindElement(By.Id("Email")).Click();
            driver.FindElement(By.Id("Email")).SendKeys("ionlycametosee@eboue.com");
            driver.FindElement(By.Id("Password")).Click();
            driver.FindElement(By.Id("Password")).SendKeys("test");
            driver.FindElement(By.CssSelector("input[type=\"submit\"]")).Click();

            Assert.AreEqual("Email or password invalid.",driver.FindElement(By.CssSelector("input[name=\"Email\"] + span[class=\"field-validation-error\"]")).Text);
        }

        [Test]
        public void TestSelenium_User_Cannot_Log_In_Without_Email_Address()
        {
            User_Cannot_Log_In_Without_Email_Address(this.InternetExplorerDriver);
            User_Cannot_Log_In_Without_Email_Address(this.FireFoxDriver);
            User_Cannot_Log_In_Without_Email_Address(this.ChromeDriver);
        }

        private void User_Cannot_Log_In_Without_Email_Address(IWebDriver driver)
        {
            driver.Url = this.GetAbsoluteUrl("/Account/Login");
            driver.Navigate();
            driver.FindElement(By.Id("Password")).Click();
            driver.FindElement(By.Id("Password")).SendKeys("test");
            driver.FindElement(By.CssSelector("input[type=\"submit\"]")).Click();

            Assert.AreEqual("Email or password invalid.", driver.FindElement(By.CssSelector("input[name=\"Email\"] + span[class=\"field-validation-error\"]")).Text);
        }

        [Test]
        public void TestSelenium_User_Cannot_Log_In_Without_Password()
        {
            User_Cannot_Log_In_Without_Password(this.InternetExplorerDriver);
            User_Cannot_Log_In_Without_Password(this.FireFoxDriver);
            User_Cannot_Log_In_Without_Password(this.ChromeDriver);
        }

        private void User_Cannot_Log_In_Without_Password(IWebDriver driver)
        {
            driver.Url = this.GetAbsoluteUrl("/Account/Login");
            driver.Navigate();
            driver.FindElement(By.Id("Email")).Click();
            driver.FindElement(By.Id("Email")).SendKeys("youcant@Cme.com");
            driver.FindElement(By.CssSelector("input[type=\"submit\"]")).Click();

            Assert.AreEqual("Email or password invalid.", driver.FindElement(By.CssSelector("input[name=\"Email\"] + span[class=\"field-validation-error\"]")).Text);
        }

        [Test]
        public void TestSelenium_User_Is_Authenticated_After_Successful_Login()
        {
            User_Is_Authenticated_After_Successful_Login(this.InternetExplorerDriver);
            User_Is_Authenticated_After_Successful_Login(this.FireFoxDriver);
            User_Is_Authenticated_After_Successful_Login(this.ChromeDriver);
        }

        private void User_Is_Authenticated_After_Successful_Login(IWebDriver driver)
        {
            Login(driver);

            var cookie = driver.Manage().Cookies.GetCookieNamed(".ASPXAUTH");
            Assert.IsNotNull(cookie);
        }

        [Test]
        public void TestSelenium_User_Is_Redirected_After_Login()
        {
            User_Is_Redirected_After_Login(this.InternetExplorerDriver);
            User_Is_Redirected_After_Login(this.FireFoxDriver);
            User_Is_Redirected_After_Login(this.ChromeDriver);
        }

        private void User_Is_Redirected_After_Login(IWebDriver driver)
        {
            Login(driver);
            //this url will change?
            Assert.AreEqual(LandingPageUrl, driver.Url);
        }
    }
}
