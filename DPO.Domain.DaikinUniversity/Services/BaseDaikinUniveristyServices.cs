using DPO.Common;
using DPO.Common.DaikinUniversity;
using DPO.Data;
using System;
using System.Net.Http;

namespace DPO.Domain.DaikinUniversity
{
    public class BaseDaikinUniveristyServices : BaseServices
    {
        public BaseDaikinUniveristyServices()
            : base()
        {
            LoadConfig();
        }

        public BaseDaikinUniveristyServices(DPOContext context)
            : base(context)
        {
            LoadConfig();
        }

        public ApiSessionToken SessionToken { get; set; }

        protected Uri ConfigDaikinUniversityBaseUrl { get; set; }

        public HttpClient GetHttpClient()
        {
            var client = new HttpClient();
            client.BaseAddress = ConfigDaikinUniversityBaseUrl;

            return client;
        }

        public virtual void LoadConfig()
        {
            ConfigDaikinUniversityBaseUrl = new Uri(Utilities.Config("dpo.daikinuniversity.base.url"));
        }

        protected virtual Uri CombineUri(string uri1, string uri2)
        {
            return new Uri(new Uri(uri1), uri2);
        }

        protected virtual Uri CombineUri(Uri uri1, string uri2)
        {
            return new Uri(uri1, uri2);
        }

        protected virtual Uri CombineUri(Uri uri1, Uri uri2)
        {
            return new Uri(uri1, uri2);
        }

        protected virtual string FormatQueryString(string queryString)
        {
            if (!String.IsNullOrWhiteSpace(queryString))
            {
                if (!queryString.StartsWith("?"))
                {
                    queryString = "?" + queryString;
                }
            }
            else
            {
                queryString = String.Empty;
            }

            return queryString;
        }

        protected virtual string GetCurrentCsodDate()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.000");
        }
    }
}