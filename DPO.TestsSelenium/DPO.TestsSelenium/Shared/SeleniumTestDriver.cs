using System;
using System.Diagnostics;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.IE;

namespace DPO.TestsSelenium
{
 
    [TestClass]
    public abstract class SeleniumTestDriver {
 
        public const string _siteUrl = "http://localhost:50781";
        private Process _iisProcess;
        private string _iisSiteName = "DPO.Web";
        private string SELENIUM_PATH = @"C:\Selenium";
        public SeleniumTestDriver() { }

        public ChromeDriver ChromeDriver { get; set; }
        public FirefoxDriver FireFoxDriver { get; set; }
        public InternetExplorerDriver InternetExplorerDriver { get; set; }
        public string GetAbsoluteUrl(string relativeUrl)
        {
            if (!relativeUrl.StartsWith("/"))
            {
                relativeUrl = "/" + relativeUrl;
            }
            return String.Format(_siteUrl + relativeUrl);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            // Ensure IISExpress is stopped
            if (_iisProcess.HasExited == false)
            {
                _iisProcess.Kill();
            }

            // Stop all Selenium drivers
            this.InternetExplorerDriver.Quit();
            this.FireFoxDriver.Quit();
            this.ChromeDriver.Quit();
        }

        [TestInitialize]
        public void TestInitialize() {
            // Start IISExpress
            StartIIS();

            //DPO.TestsSelenium\Resources
            string path = AppDomain.CurrentDomain.BaseDirectory + @"\..\..\Resources";
            // Start Selenium drivers
            this.InternetExplorerDriver = new InternetExplorerDriver(path);
            this.FireFoxDriver = new FirefoxDriver();
            this.ChromeDriver = new ChromeDriver(path);
        }
        protected virtual string GetApplicationPath(string applicationName)
        {
            var solutionFolder = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory)));
            return Path.Combine(solutionFolder, applicationName);
        }

        private void StartIIS()
        {
            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
 
            _iisProcess = new Process();
            _iisProcess.StartInfo.FileName = programFiles + "/IIS Express/iisexpress.exe";
            _iisProcess.StartInfo.Arguments = string.Format("/site:{0}", _iisSiteName);
            _iisProcess.Start();
        }
    }
}