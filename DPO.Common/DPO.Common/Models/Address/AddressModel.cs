//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================
namespace DPO.Common
{
    public class AddressModel
    {
        public long? AddressId { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string Location { get; set; }
        public string PostalCode { get; set; }

        public int? StateId { get; set; }
        public string StateName { get; set; }
        public DropDownModel States { get; set; }

        public string CountryCode { get; set; }
        public DropDownModel Countries { get; set; }


        public void Copy(AddressModel from)
        {
            this.AddressLine1 = from.AddressLine1;
            this.AddressLine2 = from.AddressLine2;
            this.AddressLine3 = from.AddressLine3;
            this.Location = from.Location;
            this.PostalCode = from.PostalCode;
            this.StateId = from.StateId;
            this.CountryCode = from.CountryCode;
        }
    }

}
