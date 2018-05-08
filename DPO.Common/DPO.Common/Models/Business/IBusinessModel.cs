//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================

namespace DPO.Common
{
    public interface IBusinessModel
    {
        long? BusinessId { get; set; }

        string BusinessName { get; set; }

        string AccountId { get; set; }

        int BusinessTypeId { get; set; }

        bool ShowPricing { get; set; }

        AddressModel Address { get; set; }

        ContactModel Contact { get; set; }
    }
}
