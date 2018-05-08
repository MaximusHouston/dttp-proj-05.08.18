﻿//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DPO.Data
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Objects;
    using System.Data.Objects.DataClasses;
    using System.Linq;
    
    public partial class DPOContext : DbContext
    {
    /* Not needed by AM 2014
        public DPOContext()
            : base("name=DPOContext")
        {
            this.Configuration.LazyLoadingEnabled = false;
        }
    	
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
       */
    
        public DbSet<AccountMultiplier> AccountMultipliers { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<CityArea> CityAreas { get; set; }
        public DbSet<Tool> Tools { get; set; }
        public DbSet<VerticalMarketType> VerticalMarketTypes { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<SystemAccess> SystemAccesses { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<BusinessType> BusinessTypes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Business> Businesses { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<OpenProjectStatusType> OpenProjectStatusTypes { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<ProductFamily> ProductFamilies { get; set; }
        public DbSet<ProductSpecificationLabel> ProductSpecificationLabels { get; set; }
        public DbSet<ProductSpecification> ProductSpecifications { get; set; }
        public DbSet<ProjectType> ProjectTypes { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<ProductMarketType> ProductMarketTypes { get; set; }
        public DbSet<ProductModelType> ProductModelTypes { get; set; }
        public DbSet<QuoteItem> QuoteItems { get; set; }
        public DbSet<ProjectStatusType> ProjectStatusTypes { get; set; }
        public DbSet<CommissionMultiplier> CommissionMultipliers { get; set; }
        public DbSet<UserBasketItem> UserBasketItems { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<SubmittalSheetType> SubmittalSheetTypes { get; set; }
        public DbSet<RequirementType> RequirementTypes { get; set; }
        public DbSet<ProductAccessory> ProductAccessories { get; set; }
        public DbSet<DocumentType> DocumentTypes { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<DocumentProductLink> DocumentProductLinks { get; set; }
        public DbSet<ProductSpecificationKeyLookup> ProductSpecificationKeyLookups { get; set; }
        public DbSet<VwProductDocument> VwProductDocuments { get; set; }
        public DbSet<VwProductSpecification> VwProductSpecifications { get; set; }
        public DbSet<C__RefactorLog> C__RefactorLog { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<Group> Groups { get; set; }
    
        [EdmFunction("DPOContext", "GetPermissionsUnderPermissionId")]
        public virtual IQueryable<Permission> GetPermissionsUnderPermissionId(Nullable<long> permissionId)
        {
            var permissionIdParameter = permissionId.HasValue ?
                new ObjectParameter("PermissionId", permissionId) :
                new ObjectParameter("PermissionId", typeof(long));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<Permission>("[DPOContext].[GetPermissionsUnderPermissionId](@PermissionId)", permissionIdParameter);
        }
    
        [EdmFunction("DPOContext", "GetGroupsAboveGroupId")]
        public virtual IQueryable<Group> GetGroupsAboveGroupId(Nullable<long> groupId)
        {
            var groupIdParameter = groupId.HasValue ?
                new ObjectParameter("GroupId", groupId) :
                new ObjectParameter("GroupId", typeof(long));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<Group>("[DPOContext].[GetGroupsAboveGroupId](@GroupId)", groupIdParameter);
        }
    
        [EdmFunction("DPOContext", "GetGroupsBelowGroupId")]
        public virtual IQueryable<Group> GetGroupsBelowGroupId(Nullable<long> groupId)
        {
            var groupIdParameter = groupId.HasValue ?
                new ObjectParameter("GroupId", groupId) :
                new ObjectParameter("GroupId", typeof(long));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<Group>("[DPOContext].[GetGroupsBelowGroupId](@GroupId)", groupIdParameter);
        }
    
        public virtual int UpdateGroupMembersCount()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UpdateGroupMembersCount");
        }
    
        [EdmFunction("DPOContext", "FnProductDocuments")]
        public virtual IQueryable<VwProductDocument> FnProductDocuments(Nullable<long> productId, Nullable<int> documentTypeId)
        {
            var productIdParameter = productId.HasValue ?
                new ObjectParameter("ProductId", productId) :
                new ObjectParameter("ProductId", typeof(long));
    
            var documentTypeIdParameter = documentTypeId.HasValue ?
                new ObjectParameter("DocumentTypeId", documentTypeId) :
                new ObjectParameter("DocumentTypeId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<VwProductDocument>("[DPOContext].[FnProductDocuments](@ProductId, @DocumentTypeId)", productIdParameter, documentTypeIdParameter);
        }
    
        [EdmFunction("DPOContext", "FnProductSpecifications")]
        public virtual IQueryable<VwProductSpecification> FnProductSpecifications(Nullable<long> productId)
        {
            var productIdParameter = productId.HasValue ?
                new ObjectParameter("ProductId", productId) :
                new ObjectParameter("ProductId", typeof(long));
    
            return ((IObjectContextAdapter)this).ObjectContext.CreateQuery<VwProductSpecification>("[DPOContext].[FnProductSpecifications](@ProductId)", productIdParameter);
        }
    }
}
