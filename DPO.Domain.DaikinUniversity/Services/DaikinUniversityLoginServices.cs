using DPO.Common;
using DPO.Common.DaikinUniversity;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DPO.Domain.DaikinUniversity
{
    public class DaikinUniversityLoginServices : BaseDaikinUniveristyServices
    {
        private static ILog _log = LogManager.GetLogger(typeof(DaikinUniversityLoginServices));

        public AesSsoLoginViewModel GetAESLoginModel(string username)
        {
            Uri aesSsoUri = new Uri(ConfigDaikinUniversityBaseUrl, ConfigAesSsoHandler);

            return new AesSsoLoginViewModel()
            {
                Key = GetSecurityToken(username),
                LoginUrl = aesSsoUri.ToString(),
                OuId = ConfigAesSsoOuId
            };
        }

        protected string ConfigAesSsoHandler { get; set; }
        protected string ConfigAesSsoOuId { get; set; }

        /// <summary>
        /// Builds an encrypted security token for AES SSO into Daikin University
        /// </summary>
        /// <param name="username">The username (Email Address) of the user</param>
        /// <returns></returns>
        public string GetSecurityToken(string username)
        {
            var token = new StringBuilder();

            if (String.IsNullOrWhiteSpace(username))
            {
                return token.ToString();
            }

            var aesKey = Utilities.Config("dpo.daikinuniversity.aessso.aeskey");
            var aesIV = Utilities.Config("dpo.daikinuniversity.aessso.aesiv");

            var account = Utilities.Config("dpo.daikinuniversity.account");
            var dateFormat = Utilities.Config("dpo.daikinuniversity.timestamp.format");
            var userPrefix = Utilities.Config("dpo.daikinuniversity.user.prefix");

            var errorUrl = Utilities.Config("dpo.daikinuniversity.aessso.errorurl");
            var logoutUrl = Utilities.Config("dpo.daikinuniversity.aessso.logouturl");
            var timeoutUrl = Utilities.Config("dpo.daikinuniversity.aessso.timeouturl");
            var destUrl = Utilities.Config("dpo.daikinuniversity.aessso.desturl");

            token.Append("empid=").Append(userPrefix).Append(username);
            token.Append(";timestamp=").Append(DateTime.UtcNow.ToString(dateFormat));

            if (!String.IsNullOrWhiteSpace(errorUrl))
                token.Append(";ERROR=").Append(errorUrl);

            if (!String.IsNullOrWhiteSpace(logoutUrl))
                token.Append(";LOGOUT=").Append(logoutUrl);

            if (!String.IsNullOrWhiteSpace(timeoutUrl))
                token.Append(";TIMEOUT=").Append(timeoutUrl);

            if (!String.IsNullOrWhiteSpace(destUrl))
                token.Append(";DEST=").Append(destUrl);

            if (!String.IsNullOrWhiteSpace(ConfigAesSsoOuId))
                token.Append(";OUID=").Append(ConfigAesSsoOuId);

            return Encrypt(token.ToString(), aesKey, aesIV);
        }

        /// <summary>
        /// Not really used, returns raw HTML value back from the AES SSO Endpoint for Daikin University
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public string LoginAES(string username)
        {
            string token = GetSecurityToken(username);
            Uri aesSsoUri = new Uri(ConfigDaikinUniversityBaseUrl, ConfigAesSsoHandler);

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("key", token),
                new KeyValuePair<string, string>("ouid", ConfigAesSsoOuId)
            });

            using (var client = GetHttpClient())
            {
                HttpResponseMessage resp = client.PostAsync(aesSsoUri.ToString(), content).Result;

                if (resp.IsSuccessStatusCode)
                {
                    string respString = resp.Content.ReadAsStringAsync().Result;

                    return respString;
                }
            }

            return String.Empty;
        }

        public override void LoadConfig()
        {
            base.LoadConfig();

            ConfigAesSsoHandler = Utilities.Config("dpo.daikinuniversity.aessso.handler");
            ConfigAesSsoOuId = Utilities.Config("dpo.daikinuniversity.aessso.ouid");
        }

        private string Decrypt(string clear, string key, string iv)
        {
            string decoded = String.Empty;
            string errorDescription = String.Empty;

            UTF8Encoding textConverter = new UTF8Encoding();
            byte[] btoDecrypt = Utilities.ConvertHexstringToByte(clear);
            byte[] decrypted = null;

            System.Security.Cryptography.RijndaelManaged aes = new RijndaelManaged();
            aes.Key = Utilities.ConvertHexstringToByte(key);
            aes.IV = Utilities.ConvertHexstringToByte(iv);

            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;

            try
            {
                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                {
                    decrypted = new byte[btoDecrypt.Length];
                    cs.Write(btoDecrypt, 0, btoDecrypt.Length);
                    cs.FlushFinalBlock();
                    decoded = textConverter.GetString(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                _log.Exception(ex, "Daikin University", "Unable to decrypt Daikin SSO Token");

                return String.Empty;
            }

            return decoded;
        }

        private string Encrypt(string clearText, string key, string iv)
        {
            string encoded = String.Empty;

            UTF8Encoding textConverter = new UTF8Encoding();
            byte[] bToEncrypt = textConverter.GetBytes(clearText);

            System.Security.Cryptography.RijndaelManaged aes = new RijndaelManaged();
            aes.Key = Utilities.ConvertHexstringToByte(key);
            aes.IV = Utilities.ConvertHexstringToByte(iv);

            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;

            try
            {
                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                using (MemoryStream ms = new MemoryStream())
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(bToEncrypt, 0, bToEncrypt.Length);
                    cs.FlushFinalBlock();
                    encoded = Utilities.BytesToHexString(ms.ToArray());
                }
            }
            catch (Exception ex)
            {
                _log.Exception(ex, "Daikin University", "Unable to encrypt Daikin SSO Token");
                return String.Empty;
            }

            return encoded;
        }
    }
}
