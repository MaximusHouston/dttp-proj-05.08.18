using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestStack.Seleno.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace DaikinProjectOffice.Tests
{
    public static class BrowserHost
    {
        public static readonly SelenoHost Instance = new SelenoHost();
        public static readonly string RootUrl;
        public static IWebDriver driver;

        static BrowserHost()
        {
            driver = new ChromeDriver(@"C:\Q2O\Source\Iterations\UnitTesting-PhaseI\Daikin Project Office\DaikinProjectOffice.Tests\libraries");
            //Instance.Run("DPO.Web", 62801);
            RootUrl = Instance.Application.Browser.Url;
        }
    }
}
