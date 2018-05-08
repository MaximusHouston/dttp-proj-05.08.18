using System;

namespace DPO.Common.DaikinUniversity
{
    public class ApiSessionToken
    {
        public string Alias { get; set; }

        public DateTime ExpiresOn { get; set; }

        public string Secret { get; set; }

        public string Token { get; set; }
    }
}