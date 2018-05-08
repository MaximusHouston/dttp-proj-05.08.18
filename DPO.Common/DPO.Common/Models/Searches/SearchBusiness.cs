//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================
 

namespace DPO.Common
{

    public class SearchBusiness : Search
    {
        public long? BusinessId { get; set; }

        public string AccountId { get; set; }

        public string BusinessName { get; set; }

        public bool? Enabled { get; set; }

        public string Address { get; set; }

        public string ExactBusinessName { get; set; }

        public string PostalCode { get; set; }

        public int? StateId { get; set; }

        public string StateCode { get; set; }

        public string CountryCode { get; set; }

        public bool? IsVRVPro { get; set; }

        public bool? IsDaikinComfortPro { get; set; }

        public bool? IsDaikinBranch { get; set; }

    }

}