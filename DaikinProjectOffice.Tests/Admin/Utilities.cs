
using DPO.Common;
using DPO.Data;
using DPO.Domain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core.EntityClient;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NUnit.Framework;
using NUnit.Common;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Support.Events;
using OpenQA.Selenium.Support.Extensions;
using OpenQA.Selenium.Support.PageObjects;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Firefox;


namespace DaikinProjectOffice.Tests
{
   public partial class TestAdmin
   {

      public static string GetSqlCeConnectionString(string fileName)
      {
         var csBuilder = new EntityConnectionStringBuilder();

         csBuilder.Provider = "System.Data.SqlServerCe.4.0";
         csBuilder.ProviderConnectionString = string.Format("Data Source={0};", fileName);

         // Make sure DPOContextTest.edmx sits int the Test.Common project

         csBuilder.Metadata = string.Format("res://{0}/Context.DPOContextTest.csdl|res://{0}/Context.DPOContextTest.ssdl|res://{0}/Context.DPOContextTest.msl",
             typeof(TestAdmin).Assembly.FullName);

         return csBuilder.ToString();

      }

      public static string TestDatabasePath()
      {
         string path = (AppDomain.CurrentDomain.BaseDirectory ?? Environment.CurrentDirectory) + @"\\..\\..\\..\\..\\";

         string testDBFileName = @"DPO.Tests\\dbDaikinProjectOfficeTest.sdf";

         return path + testDBFileName;

      }

      public static DPOContext TestContext()
      {
          
          return new DPOContext(DPOContext.GenerateConnectionString(DPOContext.Domain));
          
          //return new DPOContext(DPOContext.GenerateConnectionString(DomainEnum.QA));

          //string connectionString = 
          //DPOContext dpoContext = new DPOContext();
      }

      public UserSessionModel GetUserSessionModel(string email)
      {
          return new AccountServices(this.TContext).GetUserSessionModel(email).Model as UserSessionModel;
      }

      public BusinessModel GetBusinessModel(UserSessionModel sa, string name)
      {
          var business = this.db.Context.Businesses.Where(b => b.BusinessName.Contains(name)).FirstOrDefault();

          var businessModel = new BusinessServices(this.TContext).GetBusinessModel(sa, business.BusinessId, false).Model as BusinessModel;

          return businessModel;
      }


      public ProjectModel GetProjectModel(UserSessionModel sa, string name)
      {
          var project = db.Projects.Where(q => q.Name == name).FirstOrDefault();

          var response = new ProjectServices(this.TContext).GetProjectModel(sa, project.ProjectId);

          return response.Model as ProjectModel;

      }

      public QuoteModel GetActiveQuoteModel(UserSessionModel user, string projectName)
      {
          var activequote = db.Quotes.Where(q => q.Project.Name.Contains(projectName) && q.Project.ActiveVersion == q.Revision).FirstOrDefault();

          var response = new QuoteServices(this.TContext).GetQuoteModel(user, activequote);

          return response.Model as QuoteModel;
      }

       public static bool AreSorted<T>(IEnumerable<T> ids)
       {
          return Enumerable.SequenceEqual(ids, ids.OrderBy(id => id));
       }

       public static bool AreUnique<T>(IEnumerable<T> ids)
       {
          return ids.Distinct().Count() == ids.Count();
       }

       public static bool AssertPropertiesThatMatchAreEqual(object entity1, object entity2, bool deep, params string[] ignoreList)
       {

          var prop1 = entity1.GetType().GetProperties().Where(s => !ignoreList.Contains(s.Name)).ToDictionary(k => k.Name);
          var prop2 = entity2.GetType().GetProperties().Where(s => !ignoreList.Contains(s.Name)).ToDictionary(k => k.Name);

          foreach (var s1 in prop1)
          {
             if (prop2.ContainsKey(s1.Value.Name))
             {

                object p1Value = s1.Value.GetValue(entity1, null);

                object p2Value = prop2[s1.Value.Name].GetValue(entity2, null);

                if (p1Value == null && p2Value == null)
                {
                   continue;
                }

                bool result = true;

                var collection1 = p1Value as IList;
                var collection2 = p2Value as IList;

                if (p1Value != null && p2Value != null && collection1 != null && collection2 != null)
                {
                   if (collection1.Count != collection2.Count)
                   {
                      result = false;
                   }
                   else
                   {
                      for (int i = 0; result == true && i < collection1.Count; i++)
                      {
                         result = AssertPropertiesThatMatchAreEqual(collection1[i], collection2[i], deep, ignoreList);
                      }
                   }
                }
                else
                   if (p1Value == null || p2Value == null)
                   {
                      result = false;
                   }
                   else
                   {
                      var isDotNet = s1.Value.PropertyType.Assembly.CodeBase.ToLower().Contains("/microsoft.net/");

                      if (deep && !isDotNet) // must be custom class
                      {
                         result = AssertPropertiesThatMatchAreEqual(p1Value, p2Value, deep, ignoreList);
                      }
                      else if ((p1Value as IComparable) != null)
                      {
                         result = ((p1Value as IComparable).CompareTo(p2Value) == 0);
                      }
                      else
                      {
                         // Only compare value types.
                         if (isDotNet && s1.Value.PropertyType.IsClass == false && !p1Value.Equals(p2Value))
                         {
                            result = false;
                         }
                         else
                         {
                            //throw new Exception(string.Format("Cannot compare {0} please add to ignorelist", s1.Value.Name));
                         }

                      }
                   }

                Assert.IsTrue(result, string.Format("Property '{0}' not the same.", s1.Value.Name));

             }
          }
          return true;
       }


       public void AssertViewExists(Controller controller, string viewName)
       {

          if (string.IsNullOrEmpty(viewName))
          {
             viewName = controller.ControllerContext.RouteData.GetRequiredString("action");
          }

          var result = ViewEngines.Engines.FindPartialView(controller.ControllerContext, viewName);

          if (result.View == null)
          {
             Assert.Fail();
          }

       }


        /// <summary>
        /// split the innerText value of HtmlDocument into list of string and remove the uneed items from List
        /// only keep the item that have correct values
        /// This method will be used in Views Test
        /// </summary>
        /// <param name="stringSeparators"> collection of charcters that need to split</param>
        /// <param name="doc"> an instance of HtmlDcument</param>
        /// <returns> The list of string that we want</returns>
        public List<String> RemoveSpecificCharacterFromString(string[] stringSeparators, HtmlAgilityPack.HtmlDocument doc)
        {
            List<string> lines = doc.DocumentNode.ChildNodes[1].InnerText.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries).ToList();

            lines.RemoveAll(l => string.IsNullOrWhiteSpace(l));

            List<string> chunkCharacters = new List<string>(){"{", "}", "[" , "]", "@", @"\", @"/" , "(", ")", "&nbsp"};

            foreach(string character in chunkCharacters)
            {
                lines.RemoveAll(l => l.Contains(character));
            }
            return lines;
        }

        /// <summary>
        /// this function will redirect the url to the Login page and automatic 
        /// login using default user info
        /// </summary>
        public void NavigateToLogin(UserSessionModel user, IWebDriver driver)
        {

            driver.Navigate().GoToUrl("http://tstsysdcity2/Account/Login");
            bool notLogOut = false;
            try
            {
                driver.FindElement(By.Id("logoutForm"));
                notLogOut = true;
            }
            catch
            {
                notLogOut = false;
            }

            if (notLogOut)
            {
                driver.FindElement(By.Id("logoutForm")).Submit();
                driver.FindElement(By.Id("loginLink")).Click();
            }

            var username = driver.FindElement(By.Id("Email"));
            var password = driver.FindElement(By.Id("Password"));

           
            username.SendKeys(user.Email);

            if (user.UserTypeId == UserTypeEnum.CustomerSuperUser || user.UserTypeId == UserTypeEnum.DaikinSuperUser)
            {
                if (user.Email == "test@123.com")
                {
                    password.SendKeys("123456");
                }
                else
                {
                    password.SendKeys("test");
                }
            }
            else
            {
                password.SendKeys("123456");
            }

            driver.FindElement(By.Id("loginButton")).Click();
        }
    }
}