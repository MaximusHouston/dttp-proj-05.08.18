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

    public partial class ContactServices : BaseServices
    {
        public ContactServices() : base() { }

        public ContactServices(DPOContext context) : base(context) { }


        public ContactServices(BaseServices injectService, string propertyReference)
        {
            this.Response = injectService.Response;
            this.Context = injectService.Context;
            this.Db = injectService.Db;
            this.Response.PropertyReference = propertyReference;
        }

        public Contact ModelToEntity(ContactModel model)
        {
            var entity = GetEntity(model);

            if (this.Response.HasError) return null;

            if (model != null)
            {
                entity.ContactEmail = Utilities.Trim(model.ContactEmail);

                entity.Mobile = Utilities.Trim(model.MobileNumber);

                entity.Phone = Utilities.Trim(model.OfficeNumber);

                entity.Website = Utilities.Trim(model.WebAddress);
            }

            return entity;
        }

        private Contact GetEntity(ContactModel model)
        {
            var entity = (model == null || !model.ContactId.HasValue) ? Db.ContactCreate() : Db.GetContactByContactId(model.ContactId.Value).FirstOrDefault();

            if (entity == null)
            {
                this.Response.Messages.AddError(Resources.DataMessages.DM025);
            }

            return entity;

        }

        public ContactModel GetContactModel(UserSessionModel admin, ContactModel model)
        {
            var htmlService = new HtmlServices(this.Context);

            if (model != null && model.ContactId.HasValue)
            {
                var query =
                from contact in this.Db.GetContactQueryByContactId(model.ContactId)
                select new ContactModel
                {
                    ContactId = contact.ContactId,
                    MobileNumber = contact.Mobile,
                    OfficeNumber = contact.Phone,
                    WebAddress = contact.Website,
                    ContactEmail = contact.ContactEmail
                };

                model = query.FirstOrDefault();
            }

            return model ?? new ContactModel(); 

        }

        public void FinaliseModel(ContactModel model)
        {

        }

    }

}