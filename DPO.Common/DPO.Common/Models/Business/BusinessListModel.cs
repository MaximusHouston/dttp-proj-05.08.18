//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================
using System.Diagnostics;

namespace DPO.Common
{
    [DebuggerDisplay("BusinessType={BusinessType}, BusinessName={BusinessName}")]
   public class BusinessListModel
   {
      public long BusinessId { get; set; }

      public string BusinessName { get; set; }

      public string BusinessType { get; set; }

      public bool Enabled { get; set; }

      public string AccountId { get; set; }

      public string DaikinCityId { get; set; }

      public bool CommissionSchemeAllowed { get; set; }

      public string Location { get; set; }

      public string State { get; set; }

      public string Country { get; set; }

      public string WebSite { get; set; }

      public bool? IsVRVPro { get; set; }

      public bool? IsDaikinComfortPro { get; set; }

      public bool? IsDaikinBranch { get; set; }

   }
}


