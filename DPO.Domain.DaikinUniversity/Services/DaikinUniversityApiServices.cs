using DPO.Common;
using DPO.Common.DaikinUniversity;
using DPO.Data;
using log4net;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace DPO.Domain.DaikinUniversity
{
    public class DaikinUniversityApiServices : BaseDaikinUniveristyServices
    {
        protected UserSessionModel daikinSuperUser;
        private static ILog _log = LogManager.GetLogger(typeof(DaikinUniversityApiServices));

        public DaikinUniversityApiServices()
             : base()
        {
        }

        public DaikinUniversityApiServices(DPOContext context)
            : base(context)
        {
            daikinSuperUser = new AccountServices().GetSuperUserSessionModel().Model as UserSessionModel;
        }

        protected string ConfigApiKey { get; set; }
        protected string ConfigApiRelativeUrl { get; set; }
        protected string ConfigApiSecret { get; set; }
        protected string ConfigApiUser { get; set; }
        protected string ConfigStsRelativeUrl { get; set; }

        public ServiceResponse AuthenticateApi()
        {
            Uri uri = GetApiUri(ConfigStsRelativeUrl);

            Guid uniqueId = Guid.NewGuid();
            string queryString = String.Format("userName={0}&alias=DC-{1}", ConfigApiUser, uniqueId.ToString("N"));
            string jsonContent = null; // String.Format(@"{{ userName: '{0}', alias: 'DC1' }}", ConfigApiUser);

            var resp = ExecuteApiRequest<DaikinUniversityApiResponse<ApiSessionToken>>(HttpMethod.Post, uri, queryString, jsonContent);

            if (resp.IsOK)
            {
                var model = resp.Model as DaikinUniversityApiResponse<ApiSessionToken>;
                if (model != null && model.Data.Count > 0)
                {
                    this.SessionToken = model.Data[0];
                }
            }

            return resp;
        }

        public override void LoadConfig()
        {
            base.LoadConfig();

            ConfigApiKey = Utilities.Config("dpo.daikinuniversity.api.key");
            ConfigApiRelativeUrl = Utilities.Config("dpo.daikinuniversity.api.relativeurl");
            ConfigApiSecret = Utilities.Config("dpo.daikinuniversity.api.secret");
            ConfigApiUser = Utilities.Config("dpo.daikinuniversity.api.user");
            ConfigStsRelativeUrl = Utilities.Config("dpo.daikinuniversity.api.sts.relativeurl");
        }

        public ServiceResponse SearchCatalog(SearchGlobalCatalog search)
        {
            EnsureAuthenticated();
            string queryString = DaikinUniversityUtilities.ConvertObjectToQueryString(search);
            var uri = GetApiUri("Catalog/GlobalSearch");

            var resp = ExecuteApiRequest<DaikinUniversityApiResponse<GlobalSearchTrainingItem>>(HttpMethod.Get, uri, queryString, null);

            return resp;
        }

        public ServiceResponse GetLearningObject(SearchLearningObject search)
        {
            EnsureAuthenticated();
            string queryString = DaikinUniversityUtilities.ConvertObjectToQueryString(search);
            var uri = GetApiUri("LO/GetDetails");

            var resp = ExecuteApiRequest<DaikinUniversityApiResponse<LearningObject>>(HttpMethod.Get, uri, queryString, null);

            return resp;
        }

        private void EnsureAuthenticated()
        {
            if (SessionToken == null
                || String.IsNullOrWhiteSpace(SessionToken.Token)
                || SessionToken.ExpiresOn <= DateTime.UtcNow)
            {
                var resp = AuthenticateApi();

                if (!resp.IsOK)
                {
                    string msg = "Unknown error in ensure authenticate";
                    if (resp.Messages != null
                        && resp.Messages.Items != null
                        && resp.Messages.Items.Count > 0)
                    {
                        resp.Messages.Items.Aggregate(String.Empty, (a, l) => a.Trim() + " | " + l.Key + ":" + l.Text);
                    }

                    throw new Exception(String.Format("Unable to authenticate: {0}: {1}",
                        resp.Status, msg));
                }
            }
        }

        private ServiceResponse ExecuteApiRequest<T>(HttpMethod method, Uri apiRequestUri, string queryString, string reqContent, string reqContentType = "application/json")
            where T : IDaikinUniversityApiResponse
        {
            var client = GetHttpClient();
            var resp = new ServiceResponse();

            Uri reqUri = new Uri(apiRequestUri + FormatQueryString(queryString));

            HttpRequestMessage reqMsg = GenerateRequest(method, reqUri, reqContent, reqContentType);
            var sessSign = GenerateRequestAPISessionSignature(method, reqUri.AbsolutePath, reqMsg.Headers);
            reqMsg = GenerateRequestApplyAPISignature(reqMsg, sessSign);

            HttpResponseMessage resMsg = client.SendAsync(reqMsg).Result;

            string resContent = resMsg.Content.ReadAsStringAsync().Result;
            T respModel = resMsg.Content.ReadAsAsync<T>().Result;
            resp.Model = respModel;

            if (resp.Model == null)
            {
                // TODO:  Add resp content
                resp.AddError("Unable to parse response: " + resContent);
            }

            if (respModel.Error != null)
            {
                resp.AddError(respModel.Error.Message);
            }

            if (!resMsg.IsSuccessStatusCode)
            {
                resp.AddError("Failed to call API: " + resMsg.StatusCode + " - " + resContent);
            }

            return resp;
        }

        private HttpRequestMessage GenerateRequest(HttpMethod method, Uri reqUri, string content, string contentType)
        {
            var reqMsg = new HttpRequestMessage(method, reqUri);

            reqMsg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            reqMsg.Headers.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));

            // Order of these matters, must be in alpha order
            if (SessionToken == null
                || String.IsNullOrWhiteSpace(SessionToken.Token))
                reqMsg.Headers.Add("x-csod-api-key", ConfigApiKey);

            reqMsg.Headers.Add("x-csod-date", GetCurrentCsodDate());

            if (SessionToken != null
                && !String.IsNullOrWhiteSpace(SessionToken.Token))
                reqMsg.Headers.Add("x-csod-session-token", SessionToken.Token);

            if (!String.IsNullOrWhiteSpace(content))
                reqMsg.Content = new StringContent(content, Encoding.UTF8, contentType);

            return reqMsg;
        }

        private DaikinUniversityApiSignature GenerateRequestAPISessionSignature(HttpMethod method, string apiRelativeUrl, HttpRequestHeaders headers)
        {
            StringBuilder sbToSign = new StringBuilder();

            // Get all the x-csod headers that we must include in signature except the signature itself
            var csodHeaders = headers.Where(w => w.Key.StartsWith("x-csod-")
                                    && w.Key != "x-csod-signature")
                                 .Select(sm => new
                                 {
                                     Key = sm.Key,
                                     Value = sm.Value.Aggregate(string.Empty, (a, l) => a.Trim() + l.Trim())
                                 })
                                 .Distinct()
                                 .OrderBy(o => o.Key);

            sbToSign.Append(method);

            foreach (var csodHeader in csodHeaders)
            {
                sbToSign.AppendFormat("\n{0}:{1}", csodHeader.Key, csodHeader.Value);
            }

            sbToSign.Append("\n").Append(apiRelativeUrl);

            var signature = new DaikinUniversityApiSignature()
            {
                ApiSecret = SessionToken != null ? SessionToken.Secret : ConfigApiSecret,
                RequestedSignature = sbToSign.ToString()
            };

            byte[] secretKeyBytes = Convert.FromBase64String(signature.ApiSecret);
            byte[] inputBytes = Encoding.UTF8.GetBytes(sbToSign.ToString());

            using (var hmac = new HMACSHA512(secretKeyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);

                signature.Signature = Convert.ToBase64String(hashValue);
            }

            return signature;
        }

        private HttpRequestMessage GenerateRequestApplyAPISignature(HttpRequestMessage reqMsg, DaikinUniversityApiSignature signature)
        {
            reqMsg.Headers.Add("x-csod-signature", signature.Signature);

            return reqMsg;
        }

        private Uri GetApiUri(string apiRequestUri)
        {
            return CombineUri(CombineUri(ConfigDaikinUniversityBaseUrl, ConfigApiRelativeUrl), apiRequestUri);
        }
    }
}