using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPO.Common;
using System.Configuration;
using System.Data.Entity.Core.EntityClient;
using System.IO;
using StackExchange.Profiling;

// https://github.com/loresoft/EntityFramework.Extended
// PM> nstall-Package EntityFramework.Extended

namespace DPO.Data
{
   public partial class DPOContext
   {

      public static DomainEnum GetDomain()
      {
          switch (ConfigurationManager.AppSettings["dpo.sys.domain"].ToLower())
         {
            case "live":
               return DomainEnum.Live;
            case "staging":
               return DomainEnum.Staging;
            case "development":
               return DomainEnum.Development;
            case "qa":
               return DomainEnum.QA;
            case "fuse":
               return DomainEnum.Fuse;
            default:
               return DomainEnum.Local;
         }
      }

      public static string GenerateConnectionString(DomainEnum domain)
      {

         string sDomain = domain.ToString().ToLower();

         string connection = null;

         connection = ConfigurationManager.ConnectionStrings[sDomain].ConnectionString;
          
         if (connection == null)
         {
            throw new ConfigurationErrorsException(string.Format(DPO.Resources.SystemMessages.SM001, sDomain));
         }

         var efProvider = new EntityConnectionStringBuilder(connection);

         var sqlProvider = new SqlConnectionStringBuilder(efProvider.ProviderConnectionString);

         if (String.IsNullOrEmpty(sqlProvider.UserID) || sqlProvider.UserID == ".")
         {
             sqlProvider.IntegratedSecurity = true;
         }

        sqlProvider.PersistSecurityInfo = false;
        sqlProvider.MultipleActiveResultSets = true;
        //sqlProvider.ConnectTimeout = 5;
        //sqlProvider.ConnectTimeout = 200;

        if (domain != DomainEnum.Local)
        {
            sqlProvider.IntegratedSecurity = false;
            sqlProvider.UserID = "DaikinAdmin";
            sqlProvider.Password = "Da1k1n20L4";
        }

        efProvider.ProviderConnectionString = sqlProvider.ToString();
         
         var efConnection = efProvider.ToString();


        if (domain == DomainEnum.Local)
        {
            using (var db = new DPOContext(efConnection))
            {
                // Recreate schema if no test db or test db is older than schema
                db.Database.Initialize(true);
            }
        }

         return efConnection;

      }
   }


      

}
