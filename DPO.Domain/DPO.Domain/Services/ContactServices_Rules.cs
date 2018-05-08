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

using System.Linq;
using System.Linq.Expressions;

namespace DPO.Domain
{

    public partial class ContactServices 
    {
        // #################################################
        // Rules for model validation
        // #################################################
        public void RulesOnValidateModel(ContactModel model)
        {
            Validation.IsEmail(this.Response.Messages, model.ContactEmail, "ContactEmail", "Contact Email Address", 75, false);

            Validation.IsPhoneNumber(this.Response.Messages, model.OfficeNumber, "OfficeNumber", "Office Phone Number", false, model.ValidateForCountryCode);

            Validation.IsPhoneNumber(this.Response.Messages, model.MobileNumber, "MobileNumber", "Mobile Phone Number", false, model.ValidateForCountryCode);

            Validation.IsURL(this.Response.Messages, model.WebAddress, "WebAddress", "Web Address", 50, false);
        }

        // #################################################
        // Rules when an add takes place
        // #################################################
        public override void RulesOnAdd(UserSessionModel admin, object entity)
        {
            var contact = entity as Contact;

            if (contact == null)
            {
                throw new ArgumentException("Contact entity not loaded");
            }
        }

        // #################################################
        // Rules when a modification takes place
        // #################################################
        public override void RulesOnEdit(UserSessionModel admin, object entity)
        {
            var contact = entity as Contact;

            if (contact == null)
            {
                throw new ArgumentException("Contact entity not loaded");
            }
        }

        // #################################################
        // Rules when a delete takes place
        // #################################################
        public override void RulesOnDelete(UserSessionModel admin, object entity)
        {
            throw new NotImplementedException("Contact.RulesOnDelete");
        }



    }

}