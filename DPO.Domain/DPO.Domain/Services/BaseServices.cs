using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPO.Common;
using DPO.Data;
using DPO.Domain.Properties;
using System.Data.Entity;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Web.Mvc;
using System.Linq.Expressions;
using log4net;

namespace DPO.Domain
{
    public class BaseServices : IDisposable
    {
        public Repository Db { get; set; }

        public DPOContext Context { get; set; }

        public ServiceResponse Response { get; set; }

        public ModelStateDictionary ModelState { get; set; }

        public DropDownMode DropDownMode { get; set; }

        //public ERPClient ERPClient => new ERPClient();
        public ERPServiceProvider erpServiceProvider => new ERPServiceProvider();
        public FinaliseModelService finaliseModelSvc => new FinaliseModelService();

        public BaseServices() : this(null)
        {
            this.Log = LogManager.GetLogger(this.GetType());
        }

        public BaseServices(DPOContext context)
        {
            this.Db = new Repository(context);
            this.Context = this.Db.Context;
            this.Response = new ServiceResponse();

            DropDownMode = Common.DropDownMode.Filtering;
            this.Log = LogManager.GetLogger(this.GetType());
        }

        public BaseServices(bool readOnly)
        {
            this.Db = new Repository(null);
            this.Context = this.Db.Context;
            this.Context.ReadOnly = readOnly;
            this.Response = new ServiceResponse();

            DropDownMode = Common.DropDownMode.Filtering;
            Log = LogManager.GetLogger(this.GetType());
        }

        /// <summary>
        /// Will track property reference for messages that occur during service usage
        /// </summary>
        /// <param name="referenceName"></param>
        public void BeginPropertyReference(BaseServices service, string referenceName)
        {
            if (service != null)
            {
                this.Response = service.Response;
                this.Context = service.Context;
                this.Db = service.Db;
            }
            this.Response.PropertyReference = referenceName;
        }

        /// <summary>
        /// Will clear property reference for messages that occur during service usage
        /// </summary>
        public void EndPropertyReference()
        {
            this.Response.PropertyReference = String.Empty;
        }

        public void ModelToEntityConcurrenyProcessing(IConcurrency entity, IConcurrency model)
        {
            if (entity == null && model == null)
            {
                return;
            }

            if (entity != null && model == null)
            {
                throw new Exception("Add timestamp to model");
            }

            var currentTimestamp = entity.Timestamp;

            entity.Timestamp = model.Timestamp; // see if has changed

            model.Timestamp = currentTimestamp;
        }

        public void SaveToDatabase(string description)
        {
            try
            {
                this.Db.SaveChanges();
                this.Response.AddSuccess(string.Format(Resources.DataMessages.DM020, description));
            }
            catch (Exception e)
            {
                this.Response.AddSuccess(string.Format(Resources.DataMessages.DM026, description));
                this.Response.Messages.AddAudit(e);
            }
        }

        public void PartialSaveToDatabase(object model, object entity, string description)
        {
            var tmpEntry = Db.Entry(entity);

            NewRecordAdded = tmpEntry != null && this.Response.IsOK && Entry.State == EntityState.Added;

            string savedMessage = null;

            if (tmpEntry.State == EntityState.Modified)
            {
                if (tmpEntry.HasChanged("Deleted") && (bool)tmpEntry.PreviousValue<bool>("Deleted") == false)
                {
                    savedMessage = string.Format(Resources.DataMessages.DM022, description);
                }
                else
                {
                    savedMessage = string.Format(Resources.DataMessages.DM020, description);
                }
            }

            if (tmpEntry.State == EntityState.Added)
            {
                savedMessage = string.Format(Resources.DataMessages.DM021, description);
            }

            if (tmpEntry.State == EntityState.Deleted)
            {
                savedMessage = string.Format(Resources.DataMessages.DM022, description);
            }

            try
            {

                this.Db.SaveChanges();

                if (!string.IsNullOrWhiteSpace(savedMessage))
                {
                    this.Response.AddSuccess(savedMessage);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

            var tmodel = model as IConcurrency;
            var tentity = entity as IConcurrency;

            if (tmodel != null && tentity != null)
            {
                tmodel.Timestamp = tentity.Timestamp;
            }
        }

        /// <summary>
        /// Update some properties on an entity
        /// </summary>
        /// <typeparam name="T">Entity Type</typeparam>
        /// <param name="entity">Entity with updated values</param>
        /// <param name="updatedProperties">List of properties to be updated</param>
        public virtual void Update<T>(T entity, params Expression<Func<T, object>>[] updatedProperties)
        {
            //dbEntityEntry.State = EntityState.Modified; --- I cannot do this.

            //Ensure only modified fields are updated.
            var dbEntityEntry = Db.Entry(entity);

            if (updatedProperties.Any())
            {
                //update explicitly mentioned properties
                foreach (var property in updatedProperties)
                {
                    var propertyName = property.GetMemberName();

                    dbEntityEntry.Property(propertyName).IsModified = true;

                    if (dbEntityEntry.State == EntityState.Added)
                    {
                        dbEntityEntry.Property(propertyName).IsModified = false;
                    }
                    else if (dbEntityEntry.State == EntityState.Modified)
                    {
                        dbEntityEntry.Property(propertyName).IsModified = true;
                    }

                }
            }
            else
            {
                //no items mentioned, so find out the updated entries
                foreach (var property in dbEntityEntry.OriginalValues.PropertyNames)
                {
                    var original = dbEntityEntry.OriginalValues.GetValue<object>(property);
                    var current = dbEntityEntry.CurrentValues.GetValue<object>(property);
                    if (original != null && !original.Equals(current))
                        dbEntityEntry.Property(property).IsModified = true;
                }
            }

            this.Db.SaveChanges();
        }

        public void SaveToDatabase(object model, object entity, string description)
        {
            if (Log == null)
            {
                Log = log4net.LogManager.GetLogger(typeof(BaseServices));
            }

            if (Log != null)
            {
                Log.InfoFormat("Enter SaveToDatabase for Model: {0} Entity: {1} Description: {2}",
                                model.GetType(), entity.GetType(), description);

                Log.Debug("Get the Entry");
            }

            Entry = Db.Entry(entity);

            NewRecordAdded = Entry != null && this.Response.IsOK && Entry.State == EntityState.Added;

            if (Log != null)
                Log.DebugFormat("NewRecordAdded: {0}", NewRecordAdded);

            string savedMessage = null;

            if (Entry.State == EntityState.Modified)
            {
                if (Log != null)
                    Log.Debug("Entry.State = 'modified'");

                if (this.Entry.HasChanged("Deleted") && (bool)this.Entry.PreviousValue<bool>("Deleted") == false)
                {
                    savedMessage = string.Format(Resources.DataMessages.DM022, description);

                    if (Log != null)
                        Log.Info(savedMessage);
                }
                else
                {
                    savedMessage = string.Format(Resources.DataMessages.DM020, description);

                    if (Log != null)
                        Log.Info(savedMessage);
                }
            }

            if (Entry.State == EntityState.Added)
            {
                if (Log != null) Log.Debug("Entry.State == 'Added'");

                savedMessage = string.Format(Resources.DataMessages.DM021, description);

                if (Log != null) Log.Info(savedMessage);
            }

            if (Entry.State == EntityState.Deleted)
            {
                if (Log != null) Log.Debug("Entry.State == 'Deleted'");
                savedMessage = string.Format(Resources.DataMessages.DM022, description);
                if (Log != null) Log.Info(savedMessage);
            }

            try
            {
                if (Log != null) Log.Debug("try to save to database");

                //this.Db.SaveChanges();
                this.Db.ReadOnly = true;
                this.Db.Context.IgnoreTimestampChecking = true;
                this.Context.SaveChanges();
                                

                if (!string.IsNullOrWhiteSpace(savedMessage))
                {
                    this.Response.AddSuccess(savedMessage);
                    if (Log != null) Log.Debug(this.Response.Messages.Items.Last());
                }
            }
            catch (Exception e)
            {
                if (Log != null)
                {
                    Log.FatalFormat("Exception Source for {0}: {1}", model.GetType(), e.Source);
                    Log.FatalFormat("Exception for {0}: {1}", model.GetType(), e.InnerException.Message);
                    Log.FatalFormat("Inner Exception for {0}: {1}", model.GetType(), e.InnerException.Message);
                }
            }

            var tmodel = model as IConcurrency;
            var tentity = entity as IConcurrency;

            if (tmodel != null && tentity != null)
            {
                tmodel.Timestamp = tentity.Timestamp;
            }

            if (Log != null) Log.InfoFormat("SaveToDatabase finished for Model: {0}", model.GetType());
        }

        DbEntityEntry entry;
        protected DbEntityEntry Entry
        {
            get { return entry; }
            set
            {
                if (value.State == EntityState.Added)
                {
                    DropDownMode = Common.DropDownMode.NewRecord;
                }
                if (value.State == EntityState.Modified)
                {
                    DropDownMode = Common.DropDownMode.EditRecord;
                }
                entry = value;
            }
        }

        public bool NewRecordAdded { get; set; }

        // #################################################
        // Rules main entry
        // #################################################
        public virtual void ApplyBusinessRules(UserSessionModel admin, object entity)
        {
            Entry = Db.Entry(entity);

            switch (Entry.State)
            {
                case EntityState.Added:
                    RulesOnAdd(admin, entity);
                    break;
                case EntityState.Deleted:
                    RulesOnDelete(admin, entity);
                    break;
                case EntityState.Modified:
                    RulesOnEdit(admin, entity);
                    break;
                default:
                    RulesOnEdit(admin, entity);
                    break;
            }
        }

        public virtual void RulesOnAdd(UserSessionModel admin, object entity) { throw new NotImplementedException(); }
        public virtual void RulesOnDelete(UserSessionModel admin, object entity) { throw new NotImplementedException(); }
        public virtual void RulesOnEdit(UserSessionModel admin, object entity) { throw new NotImplementedException(); }


        public void Dispose()
        {
            this.Context.Dispose();
        }


        public ILog Log { get; set; }
    }
}
