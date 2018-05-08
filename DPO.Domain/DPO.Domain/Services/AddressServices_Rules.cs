//===================================================================================
// Delphinium Limited 2014 - Alan Machado (Alan.Machado@delphinium.co.uk)
// 
//===================================================================================
// Copyright © Delphinium Limited , All rights reserved.
//===================================================================================

using DPO.Common;
using DPO.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace DPO.Domain
{

    public partial class AddressServices : BaseServices
    {
        // #################################################
        // Rules for model validation
        // #################################################
        public void RulesOnValidateModel(AddressModel model,bool required)
        {
            Validation.IsText(this.Response.Messages, model.AddressLine1, "AddressLine1", "Address Line 1", 50, required);

            Validation.IsText(this.Response.Messages, model.AddressLine2, "AddressLine2", "Address Line 2", 50, false);

            Validation.IsText(this.Response.Messages, model.AddressLine3, "AddressLine3", "Address Line 3", 50, false);

            Validation.IsText(this.Response.Messages, model.Location, "Location", "Location", 50, required);

            if (required && model.StateId == null) this.Response.Messages.AddError("StateId", Resources.DataMessages.DM016);

            if (required && model.CountryCode == null) this.Response.Messages.AddError("CountryCode", Resources.DataMessages.DM018);

            model.PostalCode = Utilities.Upper(Utilities.Trim(model.PostalCode));

            //mass upload change - turned this off
            Validation.IsPostalCode(this.Response.Messages, model.PostalCode, "PostalCode", required,model.CountryCode);
        }

        // #################################################
        // Rules when an add takes place
        // #################################################
        public override void RulesOnAdd(UserSessionModel admin, object entity)
        {
            var address = entity as Address;

            if (address == null)
            {
                throw new ArgumentException("Address entity not loaded");
            }
        }

        // #################################################
        // Rules when a modification takes place
        // #################################################
        public override void RulesOnEdit(UserSessionModel admin, object entity)
        {
            var address = entity as Address;

            if (address == null)
            {
                throw new ArgumentException("Address entity not loaded");
            }
        }

        // #################################################
        // Rules when a delete takes place
        // #################################################
        public override void RulesOnDelete(UserSessionModel admin, object entity)
        {
            throw new NotImplementedException("Address.RulesOnDelete");
        }

    }

}