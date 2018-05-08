
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data.Entity;
using DPO.Common;
using FlakeGen;
using System.Data.Entity.Core.Objects;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Data.Common;
using System.Data.Entity.Core.EntityClient;
using System.IO;
using System.Linq;
using EntityFramework.Extensions;
using EntityFramework.Caching;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Mapping;

namespace DPO.Data
{
    public static class DPOContextExtensions
    {
        public static IEnumerable<TEntity> Cache<TEntity>(this IQueryable<TEntity> query, long? seconds = null) where TEntity : class
        {
            seconds = seconds ?? int.Parse(Utilities.Config("dpo.sys.data.cache.secs"));
            var policy = CachePolicy.WithSlidingExpiration(TimeSpan.FromSeconds(seconds.Value));
            return query.FromCache(policy);
        }
    }

    /// <summary>
    /// The context for the MileageStats database.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly",
        Justification = "Inherited from IDbContext interface, which exists to support using.")]
    public partial class DPOContext : DbContext
    {
        public static readonly DomainEnum Domain = GetDomain();

        public bool IgnoreTimestampChecking = false;

        private bool IsTransactional = false;

        private DbContextTransaction scope = null;

        // Use config file 
        public DPOContext()
           : base(GenerateConnectionString(Domain))
        {
            this.DistributedIdentifier = new DistributedIndentifier();

            this.Configuration.LazyLoadingEnabled = false;

            this.Configuration.ValidateOnSaveEnabled = false;

            this.Configuration.ProxyCreationEnabled = false;

            this.Configuration.ValidateOnSaveEnabled = false;

            this.Configuration.AutoDetectChangesEnabled = false;


        }

        public DPOContext(string connection)
           : base(connection)
        {
            this.DistributedIdentifier = new DistributedIndentifier();

            this.Configuration.LazyLoadingEnabled = false;

            this.Configuration.ValidateOnSaveEnabled = false;

            this.Configuration.ProxyCreationEnabled = false;

            this.Configuration.ValidateOnSaveEnabled = false;

            this.Configuration.AutoDetectChangesEnabled = false;
        }

        public DPOContext(DbConnection connection)
           : base(connection, true)
        {
            this.DistributedIdentifier = new DistributedIndentifier();

            this.Configuration.LazyLoadingEnabled = false;

            this.Configuration.ValidateOnSaveEnabled = false;

            this.Configuration.ProxyCreationEnabled = false;

            this.Configuration.ValidateOnSaveEnabled = false;

            this.Configuration.AutoDetectChangesEnabled = false;
        }

        /// <summary>
        /// Unique Id generator that should produce unique ids given a machine
        /// </summary>
        public IDistributedIndentifier DistributedIdentifier { get; set; }

        public ObjectContext ObjectContext
        {
            get
            {
                return ((IObjectContextAdapter)this).ObjectContext;
            }
        }

        public bool ReadOnly
        {
            set
            {
                this.Configuration.AutoDetectChangesEnabled = !value;
            }
            get
            {
                return this.Configuration.AutoDetectChangesEnabled == false;
            }
        }
        public DbContextTransaction TransactionScope
        {
            get
            {
                return scope;
            }
        }

        [System.Data.Entity.DbFunction("DPO.Data", "ALMDateConvertToString")]
        public static string ALMDateConvertToString(DateTime date)
        {
            throw new NotSupportedException("Direct calls are not supported.");
        }

        [System.Data.Entity.DbFunction("DPO.Data", "ALMDecConvertToString")]
        public static string ALMDecConvertToString(long dec)
        {
            throw new NotSupportedException("Direct calls are not supported.");
        }

        public static bool GenerateSQLCEedmx(string path, string sqlCEedmxFileNamePath)
        {

            // file path of the context database schema
            var emdxFilePath = path + @"DPO.Data\\DPO.Data.Context\\Context\\DPOContext.edmx";

            var emdxFile = new FileInfo(emdxFilePath);

            var emdxSQLCEFile = path + sqlCEedmxFileNamePath;

            // Recreate schema if no test db or test db is older than schema
            var recreateEdmx = (File.GetLastWriteTime(emdxFilePath) > File.GetLastWriteTime(emdxSQLCEFile));

            if (recreateEdmx)
            {

                string text = File.OpenText(emdxFilePath).ReadToEnd();
                //text = text
                //        .Replace(@"Provider=""System.Data.SqlClient"" ProviderManifestToken=""2008""", @"Provider=""System.Data.SqlServerCe.4.0"" ProviderManifestToken=""4.0""")
                //        .Replace(@"DevModel", @"Local");

                File.WriteAllText(emdxSQLCEFile, text);

            }

            return recreateEdmx;

        }

        public string GetTableName(Type type)
        {
            var metadata = ((IObjectContextAdapter)this).ObjectContext.MetadataWorkspace;

            // Get the part of the model that contains info about the actual CLR types
            var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));

            // Get the entity type from the model that maps to the CLR type
            var entityType = metadata
                    .GetItems<EntityType>(DataSpace.OSpace)
                    .Single(e => objectItemCollection.GetClrType(e) == type);

            // Get the entity set that uses this entity type
            var entitySet = metadata
                .GetItems<EntityContainer>(DataSpace.CSpace)
                .Single()
                .EntitySets
                .Single(s => s.ElementType.Name == entityType.Name);

            // Find the mapping between conceptual and storage model for this entity set
            var mapping = metadata.GetItems<EntityContainerMapping>(DataSpace.CSSpace)
                    .Single()
                    .EntitySetMappings
                    .Single(s => s.EntitySet == entitySet);

            // Find the storage entity set (table) that the entity is mapped
            var table = mapping
                .EntityTypeMappings.Single()
                .Fragments.Single()
                .StoreEntitySet;

            // Return the table name from the storage entity set
            return (string)table.MetadataProperties["Table"].Value ?? table.Name;
        }

        public void BulkInsertAll<T>(IEnumerable<T> entities)
        {
            entities = entities.ToArray();

            string cs = this.Database.Connection.ConnectionString;
            var conn = new SqlConnection(cs);
            conn.Open();

            Type t = typeof(T);

            var tableName = GetTableName(t);
            var bulkCopy = new SqlBulkCopy(conn)
            {
                DestinationTableName = tableName
            };

            var properties = t.GetProperties().Where(EventTypeFilter).ToArray();
            var table = new DataTable();

            foreach (var property in properties)
            {
                Type propertyType = property.PropertyType;
                if (propertyType.IsGenericType &&
                    propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    propertyType = Nullable.GetUnderlyingType(propertyType);
                }

                table.Columns.Add(new DataColumn(property.Name, propertyType));
            }

            foreach (var entity in entities)
            {
                table.Rows.Add(properties.Select(
                  property => GetPropertyValue(
                  property.GetValue(entity, null))).ToArray());
            }

            bulkCopy.WriteToServer(table);
            conn.Close();
        }

        // TODO:  Test this.  We will have to change this once we go to code first
        private bool EventTypeFilter(System.Reflection.PropertyInfo p)
        {
            var metadata = ((IObjectContextAdapter)this).ObjectContext.MetadataWorkspace;

            // Get the part of the model that contains info about the actual CLR types
            var objectItemCollection = ((ObjectItemCollection)metadata.GetItemCollection(DataSpace.OSpace));

            // Get the entity type from the model that maps to the CLR type
            var entityType = metadata
                    .GetItems<EntityType>(DataSpace.OSpace)
                    .Single(e => objectItemCollection.GetClrType(e) == p.DeclaringType);

            if (entityType == null)
            {
                return false;
            }

            if (entityType.DeclaredProperties.Any(w => String.Compare(w.Name, p.Name, true) == 0))
            {
                return true;
            }

            return false;
        }

        private object GetPropertyValue(object o)
        {
            if (o == null)
                return DBNull.Value;
            return o;
        }

        public void Commit()
        {
            if (IsTransactional)
            {
                scope.Commit();

                this.ObjectContext.Connection.Close();
            }
        }

        public Guid GenerateNextGuid()
        {
            return DistributedIdentifier.GenerateSequentialGuid();
        }

        public int GenerateNextIntId()
        {
            return DistributedIdentifier.GenerateSequential32Bit();
        }

        /// <summary>
        /// Returns the next unique Id
        /// </summary>
        /// <returns></returns>
        public long GenerateNextLongId()
        {
            return DistributedIdentifier.GenerateSequential64Bit();
        }

        public void Rollback()
        {
            if (IsTransactional)
            {
                scope.Rollback();

                this.ObjectContext.Connection.Close();
            }
        }

        public override int SaveChanges()
        {

            var timestamp = DateTime.UtcNow;

            var concurrencyEntites = ChangeTracker.Entries<IConcurrency>();

            if (concurrencyEntites != null)
            {
                foreach (DbEntityEntry<IConcurrency> ose in concurrencyEntites)
                {
                    // Concurrency functionality
                    if (ose.Entity != null)
                    {
                        if (!IgnoreTimestampChecking && ose.State == EntityState.Modified && ((DbEntityEntry)ose).HasChanged("Timestamp"))
                        {
                            throw new ValidationException(string.Format(Resources.DataMessages.DM003, ose.Entity.GetType().BaseType.Name));
                        }

                        if (ose.State == EntityState.Modified || ose.State == EntityState.Added)
                        {
                            ose.Entity.Timestamp = timestamp;
                        }
                    }
                }
            }
            return base.SaveChanges();
        }

        public void SetTransactional(System.Data.IsolationLevel level)
        {
            scope = this.Database.BeginTransaction(level);
            this.IsTransactional = true;
        }

        public void UseTransaction(DbContextTransaction scope)
        {
            this.Database.UseTransaction(scope.UnderlyingTransaction);
            this.IsTransactional = true;
            this.scope = scope;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    }

}