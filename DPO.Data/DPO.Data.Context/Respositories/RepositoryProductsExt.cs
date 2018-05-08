using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using EntityFramework.Extensions;
using DPO.Common;
using System.Reflection;
using System.Transactions;
using System.Threading.Tasks;
using DPO.Common.Linq;
using DPO.Data.Models;

namespace DPO.Data
{

    public partial class Repository
    {
        public IQueryable<AccountMultiplier> AccountMultipliers
        {
            get { return this.GetDbSet<AccountMultiplier>(); }
        }

        public IQueryable<ProductCategory> ProductCategories
        {
            get { return this.GetDbSet<ProductCategory>(); }
        }

        public IQueryable<Product> Products
        {
            get { return this.GetDbSet<Product>(); }
        }

        public IQueryable<ProductSpecificationLabel> ProductSpecificationLabels
        {
            get { return this.GetDbSet<ProductSpecificationLabel>(); }
        }

        public IQueryable<ProductSpecification> ProductSpecifications
        {
            get { return this.GetDbSet<ProductSpecification>(); }
        }
        public IQueryable<VerticalMarketType> VerticalMarketTypes
        {
            get { return this.GetDbSet<VerticalMarketType>(); }
        }

        public IQueryable<VwProductSpecification> VwProductSpecifications
        {
            get
            {
                return this.GetDbSet<VwProductSpecification>();
            }
        }

        public IQueryable<VwProductSystemComponent> VwProductSystemComponents
        {
            get
            {
                return this.GetDbSet<VwProductSystemComponent>();
            }
        }

        //public long? GetSystemComponentProductId(long? systemProductId, ProductModelTypeEnum type)
        //{
        //    if (!systemProductId.HasValue || (type != ProductModelTypeEnum.Indoor && type != ProductModelTypeEnum.Outdoor)) return null;

        //    var result = this.Context .VwProductSystemComponents
        //                              .Where(p => p.ProductId == systemProductId.Value && p.ComponentModelTypeId == (int)type)
        //                              .Select(p => p.ComponentProductId)
        //                              .FirstOrDefault();
        //    return result;
        //}

        public AccountMultiplier AccountMultiplierCreate(string classcode, Business business, decimal multiplier)
        {
            var entity = new AccountMultiplier();

            entity.AccountMultiplierId = this.Context.GenerateNextLongId();

            entity.BusinessId = business.BusinessId;

            entity.ProductClassCode = classcode;

            entity.Multiplier = multiplier;

            this.Context.AccountMultipliers.Add(entity);

            return entity;
        }

        public IQueryable<AccountMultiplier> AccountMultipliersByBusinessAndProductClassCode(long businessId, string[] productclassCodes)
        {
            var query = this.AccountMultipliers.Where(a => a.BusinessId == businessId && productclassCodes.Contains(a.ProductClassCode));
            return query;
        }

        public IQueryable<AccountMultiplier> AccountMultiplierByBusinessAndMultiplierTypeId(long businessId, int?[] multiplierTypeId)
        {
            var query = this.AccountMultipliers
                             .Where(ac => ac.BusinessId == businessId &&
                                   multiplierTypeId.Contains(ac.MultiplierTypeId));

            return query;
        }

        public IQueryable<AccountMultiplier> AccountMultipliersByIds(long[] ids)
        {
            return this.AccountMultipliers.Where(a => ids.Contains(a.AccountMultiplierId));
        }

        public Dictionary<long, List<ProductAccessoryModel>> GetProductAccessories(long[] productIds)
        {
            var result = (from a in this.Context.ProductAccessories
                          join p in this.Products on a.ProductId equals p.ProductId
                          where a.Product.ProductTypeId == (int)ProductTypeEnum.Accessory && productIds.Contains(a.ParentProductId)
                          select new ProductAccessoryModel
                          {
                              ParentProductId = a.ParentProductId,
                              Quantity = a.Quantity,
                              Accessory = new ProductModel
                              {
                                  ProductId = p.ProductId,
                                  ProductNumber = p.ProductNumber,
                                  Price = p.ListPrice,
                                  Name = p.Name,
                                  MultiplierTypeId = p.MultiplierTypeId.HasValue ? p.MultiplierTypeId.Value : 0,
                                  ProductClassCode = p.ProductClassCode
                              }
                          })
                         .ToArray()
                         .GroupBy(p => p.ParentProductId.Value)
                         .ToDictionary(g => g.Key, g => g.ToList());

            return result;
        }

        public Dictionary<long, List<DocumentModel>> GetProductDocuments(long[] productIds)
        {
            var result = (
                          from d in this.Context.FnProductDocuments(null, null)
                          where productIds.Contains(d.ProductId) &&
                          d.DocumentTypeId != (int)DocumentTypeEnum.ProductImageLowRes &&
                          d.DocumentTypeId != (int)DocumentTypeEnum.ProductImageHighRes &&
                          d.DocumentTypeId != (int)DocumentTypeEnum.ProductLogos && d.Rank != 0
                          select new DocumentModel
                          {
                              ProductId = d.ProductId,
                              Rank = d.Rank.HasValue ? d.Rank.Value : (short)0,
                              DocumentId = d.DocumentId,
                              FileName = d.FileName,
                              Name = d.Name,
                              Type = d.DocumentTypeDescription,
                              DocumentTypeId = (int)d.DocumentTypeId
                          }
                          )
                         .ToArray()
                         .GroupBy(p => p.ProductId)
                         .ToDictionary(g => g.Key, g => g.ToList());
            return result;
        }

        ///<summary> 
        ///get the images for products to display on products page
        ///</summary>
        ///<param name="productIds">products 'Id items </param>
        ///<typeparam>long array</typeparam>
        ///<param name="types">document type items</param>
        ///<typeparam>int enum array</typeparam>
        ///<returns> the document model as dictionary</returns> 
        public Dictionary<long, List<DocumentModel>> GetProductImages(long[] productIds, DocumentTypeEnum[] types)
        {
            var typeIds = types.Cast<int>().ToArray();
            var result =
            (
                         from img in this.Context.FnProductDocuments(null, null)
                         where productIds.Contains(img.ProductId) && typeIds.Contains(img.DocumentTypeId)
                         select img
            )
            .ToArray()
            .GroupBy(p => p.ProductId)
            .ToDictionary(
                          g => g.Key,
                          g => g.Select(s => new DocumentModel { Rank = s.Rank.HasValue ? s.Rank.Value : (short)0, DocumentTypeId = s.DocumentTypeId, FileName = s.FileName ?? null, Type = s.DocumentTypeDescription ?? null }).ToList()
                         );
            return result;
        }

        public List<ProductModel> GetProductModelsForSubmittalSheetGeneration()
        {
            var result = (
                          from p in this.Context.Products
                          select new ProductModel { ProductId = p.ProductId, Timestamp = p.Timestamp, ProductNumber = p.ProductNumber, SubmittalSheetTypeId = (SubmittalSheetTypeEnum)p.SubmittalSheetTypeId }
                          ).ToList();

            //return result.Where(p => p.GetSubmittalSheetTemplateName != "SubmittalTemplate").ToList();
            //LMW Added OrderBy 02/18
            return result.Where(p => p.GetSubmittalSheetTemplateName != "SubmittalTemplate").OrderBy(p => p.ProductNumber).ToList();
        }

        public Dictionary<long, List<ProductNoteModel>> GetProductNotes(long[] productIds)
        {
            var result = (
                          from d in this.Context.FnProductNotes(null, null)
                          where productIds.Contains(d.ProductId)
                          select new ProductNoteModel
                          {
                              ProductId = d.ProductId,
                              Description = d.Description,
                              ProductNoteTypeId = d.ProductNoteTypeId,
                              Rank = d.Rank,
                          }
                          )
                         .ToArray()
                         .GroupBy(p => p.ProductId)
                         .ToDictionary(g => g.Key, g => g.ToList());
            return result;
        }

        public Dictionary<long, ProductSpecificationsModel> GetProductSpecifications(long[] productIds, string[] labels)
        {
            IQueryable<VwProductSpecification> query;
            if (labels != null)
            {
                var specIds = this.Context
                    .ProductSpecificationLabels.Cache()
                    .Where(l => labels.Contains(l.Name))
                    .Select(l => l.ProductSpecificationLabelId).ToArray();

                query = from ps in this.Context.FnProductSpecifications(null)
                        where productIds.Contains(ps.ProductId) && specIds.Contains(ps.SpecificationId)
                        select ps;
            }
            else
            {
                query = from ps in this.Context.FnProductSpecifications(null)
                        where productIds.Contains(ps.ProductId)
                        select ps;
            }
            var result = query
                        .ToArray()
                        .GroupBy(p => p.ProductId)
                        .ToDictionary(g => g.Key,
                                      g => new ProductSpecificationsModel
                                      {
                                          All = g.Select(s => new ProductSpecificationModel
                                          {
                                              ProductId = s.ProductId,
                                              Name = s.SpecificationName,
                                              Key = s.SpecificationKey,
                                              Value = s.Value
                                          }).ToDictionary(p => p.Name)
                                      });
            return result;
        }

        public Dictionary<long, List<ProductAccessoryModel>> GetProductSubProductModel(long[] productIds)
        {
            var result = (from a in this.Context.ProductAccessories
                          join p in this.Products on a.ProductId equals p.ProductId
                          where (a.Product.ProductModelTypeId == (int)ProductModelTypeEnum.Indoor || a.Product.ProductModelTypeId == (int)ProductModelTypeEnum.Outdoor)
                                && productIds.Contains(a.ParentProductId)
                          select new ProductAccessoryModel
                          {
                              ParentProductId = a.ParentProductId,
                              Quantity = a.Quantity,
                              Accessory = new ProductModel
                              {
                                  ProductId = p.ProductId,
                                  ProductNumber = p.ProductNumber,
                                  Price = p.ListPrice,
                                  ProductClassCode = p.ProductClassCode,
                                  Name = p.Name,
                                  MultiplierTypeId = p.MultiplierTypeId.HasValue ? p.MultiplierTypeId.Value : 0
                              }
                          })
                         .ToArray()
                         .GroupBy(p => p.ParentProductId.Value)
                         .ToDictionary(g => g.Key, g => g.ToList());

            return result;
        }

        public bool IsProduct(UserSessionModel admin, long productId)
        {
            return this.Products.Where(e => e.ProductId == productId).Any();
        }

        public IQueryable<ProductAccessory> ProductAccesoriesByProductId(long productId)
        {
            var result = from a in this.Context.ProductAccessories
                         where productId == a.ParentProductId && a.Product.ProductModelTypeId == (int)ProductModelTypeEnum.Accessory
                         select a;

            return result;
        }

        public IQueryable<Product> ProductByProductIds(long[] ids)
        {
            return this.Products.Where(u => ids.Contains(u.ProductId));
        }

        public IQueryable<Product> ProductByProductNumbers(UserSessionModel admin, string[] productNumbers)
        {
            return GetProducts(admin).Where(p => productNumbers.Contains(p.ProductNumber)).Select(p => p);
        }

        public Product ProductCreate(long productId)
        {
            var entity = new Product();

            entity.ProductId = productId;

            return entity;
        }

        public IQueryable<Product> ProductQueryByProductId(long productId)
        {
            return this.Products.Where(u => u.ProductId == productId);
        }

        public IQueryable<ProductSpecification> ProductSpecificationByProductId(long productId)
        {
            var result = from spec in this.Context.ProductSpecifications
                         join prod in this.Context.Products on spec.ProductId equals prod.ProductId
                         where prod.ProductId == productId
                         select spec;

            return result;
        }

        public IQueryable<Product> ProductsQueryBySearch(UserSessionModel admin, SearchProduct search, Func<SearchProduct, IQueryable<Product>, IQueryable<Product>> prePaging = null)
        {
            IQueryable<Product> query = GetProducts(admin);

            query = Filter(query, search);

            if (search != null && search.ReturnTotals)
            {
                search.TotalRecords = query.Count();
            }

            query = Sort(query, search);

            if (prePaging != null)
            {
                query = prePaging(search, query);
            }

            query = Paging(admin, query, search); // Must be Last

            return query;
        }

        //This is used by Product grid in angular 2
        public IQueryable<Product> ProductsQueryBySearchNoPaging(UserSessionModel admin, SearchProduct search)
        {
            IQueryable<Product> query = GetProducts(admin);

            query = FilterV2(query, search);

            if (search != null && search.ReturnTotals)
            {
                search.TotalRecords = query.Count();
            }

            query = Sort(query, search);

            return query;
        }

        //Daikin Equip App
        public IQueryable<Product> ProductsQueryBySearch(SearchProduct search)
        {
            IQueryable<Product> query = GetAllProducts();

            query = FilterV2(query, search);

            if (search != null && search.ReturnTotals)
            {
                search.TotalRecords = query.Count();
            }

            //query = Sort(query, search);

            return query;
        }

        //Daikin Equip App
        public IQueryable<DocumentProductLinkModel> GetAllProductDocumentLinks(DocumentProductLinksModel searchModel)
        {

            var query = from link in this.DocumentProductLinks
                        join product in this.Products on link.ProductId equals product.ProductId
                        orderby link.ProductId
                        select new DocumentProductLinkModel
                        {
                            DocumentId = link.DocumentId,
                            ProductId = link.ProductId,
                            ProductNumber = product.ProductNumber,
                            ProductFamilyId = product.ProductFamilyId,
                            ProductStatusTypeId = product.ProductStatusId
                        };

            if (searchModel.ProductFamilyId.HasValue)
            {
                query = query.Where(s => s.ProductFamilyId == searchModel.ProductFamilyId);
            }
            if (searchModel.ProductStatusTypeId.HasValue)
            {
                query = query.Where(s => s.ProductStatusTypeId == searchModel.ProductStatusTypeId);
            }


            return query;
        }

        public IQueryable<ProductAccessory> ProductSystemProductsByProductId(long productId)
        {
            var result = from a in this.Context.ProductAccessories
                         where productId == a.ParentProductId && a.Product.ProductModelTypeId == (int)ProductModelTypeEnum.System
                         select a;

            return result;
        }

        public IQueryable<Product> QueryProductSystemSubProductsByProductIds(long[] productIds)
        {
            var query = from sub in this.Context.VwProductSystemComponents
                        join e in this.Context.Products on sub.ComponentProductId equals e.ProductId
                        where productIds.Contains(sub.ProductId)
                        select e;

            return query;
        }

        #region Product Accessories Update

        public void UpdateProductAccessories(List<ProductAccessory> imports)
        {
            var Db = this;

            // Update any Accessorys
            Db.Context.ReadOnly = false;
            Db.Context.Configuration.AutoDetectChangesEnabled = false;

            // Get all current Accessorys which match imported records
            var currentAccessorysLookup = Db.Context.ProductAccessories.ToArray()
                                         .GroupBy(p => p.ParentProductId)
                                         .ToDictionary(p => p.Key, p => p.ToList());

            var importsLookup = imports.GroupBy(p => p.ParentProductId).ToDictionary(p => p.Key, p => p.ToList());

            var importListAdding = new List<ProductAccessory>();

            var importListUpdating = new List<ProductAccessory>();

            var importListDeleting = new List<ProductAccessory>();

            foreach (var item in importsLookup)
            {
                var importAccessories = item.Value;
                List<ProductAccessory> currentAccessories = null;
                if (currentAccessorysLookup.TryGetValue(item.Key, out currentAccessories))
                {
                    // New to add
                    var add = (from i in importAccessories
                               join c in currentAccessories on i.ProductId equals c.ProductId into lc
                               from c in lc.DefaultIfEmpty()
                               where c == null
                               select i).AsEnumerable();

                    importListAdding.AddRange(add);

                    // Not there anymore so delete
                    var del = (from c in currentAccessories
                               join i in importAccessories on c.ProductId equals i.ProductId into lc
                               from i in lc.DefaultIfEmpty()
                               where i == null
                               select c).AsEnumerable();

                    importListDeleting.AddRange(del);

                    // Update Matches so with differences 
                    var upt = (from c in currentAccessories
                               join i in importAccessories on c.ProductId equals i.ProductId
                               select new { c, i }
                              ).ToList();

                    upt.ForEach(m =>
                    {
                        if (m.c.Quantity != m.i.Quantity) m.c.Quantity = m.i.Quantity;
                        if (m.c.RequirementTypeId != m.i.RequirementTypeId) m.c.RequirementTypeId = m.i.RequirementTypeId;
                        if (m.c.ModifiedOn != m.i.ModifiedOn) m.c.ModifiedOn = m.i.ModifiedOn;
                    });
                }
                else
                {
                    importListAdding.AddRange(importAccessories);
                }
            }

            // Prevent duplicate from being inserted
            var duplicateLookup = importListAdding.GroupBy(p => p.ParentProductId + ":" + p.ProductId)
                                  .ToDictionary(p => p.Key, p => p.ToList());

            var duplicates = duplicateLookup.Where(p => p.Value.Count > 1).ToList();

            foreach (var duplicate in duplicates)
            {
                for (int d = 1; d < duplicate.Value.Count; d++) // remove all apart from first one
                {
                    importListAdding.Remove(duplicate.Value[d]);
                }
            }
            duplicateLookup = importListAdding.GroupBy(p => p.ParentProductId + ":" + p.ProductId)
                              .ToDictionary(p => p.Key, p => p.ToList());

            duplicates = duplicateLookup.Where(p => p.Value.Count > 1).ToList();

            using (var scope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(0, 15, 0)))
            {

                Db.Context.ReadOnly = true;

                foreach (var removeItem in importListDeleting)
                {
                    Db.Context.ProductAccessories.Remove(removeItem);

                    try
                    {
                        Db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        // Modified changed from FatalFormat due to all other procs after not running when this fails due to bad refs in PIM!
                        // LMW 01/05/2018
                        //Log.InfoFormat("UpdateProductAccessories Remove accessory error: ParentProductId=" + removeItem.ParentProductId + " ProductId=" + removeItem.ProductId);
                        Log.ErrorFormat("UpdateProductAccessoies error. ProductNumber: {0}, ProductId: {1}",
                                       (removeItem.Product != null) ? removeItem.Product.ProductNumber : string.Empty,
                                       (!string.IsNullOrEmpty(removeItem.ProductId.ToString())) ? removeItem.ProductId : 0);

                        Log.ErrorFormat("Exception details: {0} ", (ex.InnerException != null) ? ex.InnerException.InnerException.Message : ex.Message);
                        continue;
                    }
                }

                foreach (var addItem in importListAdding)
                {
                    Db.Context.ProductAccessories.Add(addItem);

                    try
                    {
                        Db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Log.ErrorFormat("UpdateProductAccessoies error. ProductNumber: {0}, ProductId: {1}",
                                      (addItem.Product != null) ? addItem.Product.ProductNumber : string.Empty,
                                      (!string.IsNullOrEmpty(addItem.ProductId.ToString())) ? addItem.ProductId : 0);

                        Log.ErrorFormat("Exception details: {0} ", (ex.InnerException != null) ? ex.InnerException.InnerException.Message : ex.Message);
                        continue;
                    }
                }

                scope.Complete();
            }


        }

        #endregion

        //#######################################################################
        // First add any new products in async
        //#######################################################################
        public void DatabaseAddProduct(Product product)
        {
            ProductSpecification productSpecifications = new ProductSpecification();

            Repository db = new Repository();
            product.ProductId = db.Context.GenerateNextLongId();

            //#######################################################################
            // add product specifications
            //#######################################################################
            foreach (var spec in product.ProductSpecifications)
            {
                spec.ProductId = product.ProductId;
                productSpecifications.ProductId = spec.ProductId;
                productSpecifications.Value = spec.Value;
            }

            db.Context.ReadOnly = true;
            db.Context.Products.Add(product);
            db.Context.ProductSpecifications.Add(productSpecifications);

            try
            {
                db.Context.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.Error("Update ProductSpecification failed.");
                _log.ErrorFormat("Exception Details: {0}",
                    ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
        }
        public void DatabaseUpdateProduct(Product product)
        {
            Repository Db = new Repository();

            Db.Context.ReadOnly = false;
            Db.Context.Configuration.AutoDetectChangesEnabled = false; // set this on increase the performance and reduce the time consume

            //#######################################################################
            // Get product specs for the updates
            //#######################################################################
            var updatingProductId = product.ProductId;

            var removeList = new List<ProductSpecification>();

            Product current = Db.Products.Where(p => p.ProductId == product.ProductId).FirstOrDefault();

            if (current != null)
            {
                if (current.BrandId != product.BrandId)
                {
                    current.BrandId = product.BrandId;
                }
                if (current.ProductCategoryId != product.ProductCategoryId)
                {
                    current.ProductCategoryId = product.ProductCategoryId;
                }
                if (current.ProductClassCode != product.ProductClassCode)
                {
                    current.ProductClassCode = product.ProductClassCode;
                }
                if (current.ProductFamilyId != product.ProductFamilyId)
                {
                    current.ProductFamilyId = product.ProductFamilyId;
                }
                if (current.ProductMarketTypeId != product.ProductMarketTypeId)
                {
                    current.ProductMarketTypeId = product.ProductMarketTypeId;
                }
                if (current.ProductModelTypeId != product.ProductModelTypeId)
                {
                    current.ProductModelTypeId = product.ProductModelTypeId;
                }
                if (current.AllowCommissionScheme != product.AllowCommissionScheme)
                {
                    current.AllowCommissionScheme = product.AllowCommissionScheme;
                }
                if (current.ModifiedOn != product.ModifiedOn)
                {
                    current.ModifiedOn = product.ModifiedOn;
                }
                if (current.ProductStatusId != product.ProductStatusId)
                {
                    current.ProductStatusId = product.ProductStatusId;
                }
                if (current.SubmittalSheetTypeId != product.SubmittalSheetTypeId)
                {
                    current.SubmittalSheetTypeId = product.SubmittalSheetTypeId;
                }
                if (current.Name != product.Name)
                {
                    current.Name = product.Name;
                }
                if (current.ProductPowerVoltageTypeId != product.ProductPowerVoltageTypeId)
                {
                    current.ProductPowerVoltageTypeId = product.ProductPowerVoltageTypeId;
                }
                if (current.IEERNonDucted != product.IEERNonDucted)
                {
                    current.IEERNonDucted = product.IEERNonDucted;
                }
                if (current.EERNonDucted != product.EERNonDucted)
                {
                    current.EERNonDucted = product.EERNonDucted;
                }
                if (current.SEERNonDucted != product.SEERNonDucted)
                {
                    current.SEERNonDucted = product.SEERNonDucted;
                }
                if (current.HSPFNonDucted != product.HSPFNonDucted)
                {
                    current.HSPFNonDucted = product.HSPFNonDucted;
                }
                if (current.COP47NonDucted != product.COP47NonDucted)
                {
                    current.COP47NonDucted = product.COP47NonDucted;
                }
                if (current.CoolingCapacityRated != product.CoolingCapacityRated)
                {
                    current.CoolingCapacityRated = product.CoolingCapacityRated;
                }
                if (current.HeatingCapacityRated != product.HeatingCapacityRated)
                {
                    current.HeatingCapacityRated = product.HeatingCapacityRated;
                }
                if (current.UnitInstallationTypeId != product.UnitInstallationTypeId)
                {
                    current.UnitInstallationTypeId = product.UnitInstallationTypeId ?? 1;
                }
                if (current.ProductCompressorStageId != product.ProductCompressorStageId)
                {
                    current.ProductCompressorStageId = product.ProductCompressorStageId ?? 0;
                }
                if (current.ProductGasValveTypeId != product.ProductGasValveTypeId)
                {
                    current.ProductGasValveTypeId = product.ProductGasValveTypeId ?? 0;
                }
                if (current.ProductMotorSpeedTypeId != product.ProductMotorSpeedTypeId)
                {
                    current.ProductMotorSpeedTypeId = product.ProductMotorSpeedTypeId ?? 0;
                }
                if (current.ProductInstallationConfigurationTypeId != product.ProductInstallationConfigurationTypeId)
                {
                    current.ProductInstallationConfigurationTypeId = product.ProductInstallationConfigurationTypeId ?? 0;
                }
                if (current.CoolingCapacityNominal != product.CoolingCapacityNominal)
                {
                    current.CoolingCapacityNominal = product.CoolingCapacityNominal;
                }
                if (current.AirFlowRateHighCooling != product.AirFlowRateHighCooling)
                {
                    current.AirFlowRateHighCooling = product.AirFlowRateHighCooling;
                }
                if (current.AirFlowRateHighHeating != product.AirFlowRateHighHeating)
                {
                    current.AirFlowRateHighHeating = product.AirFlowRateHighHeating;
                }

                // TODO:  CHECK HERE FOR PRODUCT CHANGES

                Db.Entry(current).State = EntityState.Modified;
            }
            //#######################################################################
            // Update product specifications
            //#######################################################################

            var currentlookup = Db.Context.ProductSpecifications.Where(ps => ps.ProductId == updatingProductId).ToArray().ToDictionary(s => s.ProductSpecificationLabelId);

            foreach (var importSpec in product.ProductSpecifications)
            {
                ProductSpecification currentSpec;

                if (currentlookup.TryGetValue(importSpec.ProductSpecificationLabelId, out currentSpec))
                {
                    if (currentSpec.Value != importSpec.Value)
                    {
                        currentSpec.Value = importSpec.Value;
                    }
                }
                else
                {
                    importSpec.ProductId = current.ProductId;
                    Db.Context.ProductSpecifications.Add(importSpec);
                }
            }

            //#######################################################################
            // Remove any current specs which now dont exist (or have become blank)
            //#######################################################################
            var importlookup = product.ProductSpecifications.ToList().Distinct().ToDictionary(s => s.ProductSpecificationLabelId);

            foreach (var currentSpec in current.ProductSpecifications)
            {
                if (!importlookup.ContainsKey(currentSpec.ProductSpecificationLabelId))
                {
                    removeList.Add(currentSpec);
                }
            };

            Db.Context.ChangeTracker.DetectChanges();

            Db.Context.SaveChanges();

            Db.ReadOnly = true;

            Db.Context.ProductSpecifications.RemoveRange(removeList);

            Db.Context.ChangeTracker.DetectChanges();

            Db.Context.SaveChanges();

        }
        public static void DatabaseAdd(Repository Db, List<Product> addList)
        {
            List<ProductSpecification> addSpecList = new List<ProductSpecification>();
            List<ProductNote> addNotesList = new List<ProductNote>();

            foreach (var importItem in addList)
            {
                importItem.ProductId = Db.Context.GenerateNextLongId();

                //#######################################################################
                // add product specifications
                //#######################################################################

                foreach (var spec in importItem.ProductSpecifications)
                {
                    spec.ProductId = importItem.ProductId;

                    if (addSpecList.Count == 0)
                    {
                        addSpecList.Add(spec);
                    }
                    else
                    {
                        foreach (var item in addSpecList)
                        {
                            if (addSpecList.Where(apl => apl.ProductSpecificationLabelId == spec.ProductSpecificationLabelId).ToList().Count == 0)
                            {
                                addSpecList.Add(spec);
                            }
                        }
                    }
                }

                foreach (var note in importItem.ProductNotes)
                {
                    note.ProductId = importItem.ProductId;
                    note.ProductNoteId = Guid.NewGuid();
                    addNotesList.Add(note);
                }
            }

            Db.Context.ReadOnly = true;
            try
            {
                Db.Context.Products.AddRange(addList);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.InnerException.Message);
            }
            try
            {
                Db.Context.ProductSpecifications.AddRange(addSpecList);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.InnerException.Message);
            }


            if (addNotesList.Count > 0)
            {
                try
                {
                    Db.Context.ProductNotes.AddRange(addNotesList);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.InnerException.Message);
                }
            }

            addSpecList.Clear();
            addNotesList.Clear();

            Db.Context.Configuration.AutoDetectChangesEnabled = false;


            try
            {
                Db.Context.SaveChanges();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.InnerException.Message);
            }

        }
        //#######################################################################
        // Load specifications for updating products
        //#######################################################################
        public static void DatabaseUpdate(Repository Db, List<Product> updateList, Dictionary<string, Product> dbCurrentList)
        {
            Db.Context.ReadOnly = false;
            Db.Context.Configuration.AutoDetectChangesEnabled = false; // set this on increase the performance and reduce the time consume

            //#######################################################################
            // Get product specs for the updates
            //#######################################################################
            var updatingProductIds = updateList.Select(p => p.ProductId).ToList();

            Db.Context.ProductSpecifications.Where(p => updatingProductIds.Contains(p.ProductId)).Load();

            var removeList = new List<ProductSpecification>();

            var removeNoteList = new List<ProductNote>();

            foreach (var import in updateList)
            {
                Product current;

                // TODO:  Why the heck did we do this?
                if (dbCurrentList.TryGetValue(import.ProductNumber, out current))
                {
                    //#######################################################################
                    // Map Product values
                    //#######################################################################
                    if (current.BrandId != import.BrandId) current.BrandId = import.BrandId;

                    if (current.ProductCategoryId != import.ProductCategoryId) current.ProductCategoryId = import.ProductCategoryId;
                    if (current.ProductClassCode != import.ProductClassCode) current.ProductClassCode = import.ProductClassCode;
                    if (current.ProductFamilyId != import.ProductFamilyId) current.ProductFamilyId = import.ProductFamilyId;
                    if (current.ProductMarketTypeId != import.ProductMarketTypeId) current.ProductMarketTypeId = import.ProductMarketTypeId;
                    if (current.ProductModelTypeId != import.ProductModelTypeId) current.ProductModelTypeId = import.ProductModelTypeId;
                    if (current.AllowCommissionScheme != import.AllowCommissionScheme) current.AllowCommissionScheme = import.AllowCommissionScheme;
                    if (current.ModifiedOn != import.ModifiedOn) current.ModifiedOn = import.ModifiedOn;
                    if (current.ProductStatusId != import.ProductStatusId) current.ProductStatusId = import.ProductStatusId;
                    if (current.SubmittalSheetTypeId != import.SubmittalSheetTypeId) current.SubmittalSheetTypeId = import.SubmittalSheetTypeId;
                    if (current.Name != import.Name) current.Name = import.Name;

                    if (current.ProductSubFamilyId != import.ProductSubFamilyId)
                    {
                        current.ProductSubFamilyId = import.ProductSubFamilyId;
                    }

                    if (current.ProductFunctionCategoryId != import.ProductFunctionCategoryId)
                    {
                        current.ProductFunctionCategoryId = import.ProductFunctionCategoryId;
                    }

                    if (current.Tonnage != import.Tonnage)
                    {
                        current.Tonnage = import.Tonnage;
                    }

                    if (current.ProductPowerVoltageTypeId != import.ProductPowerVoltageTypeId) current.ProductPowerVoltageTypeId = import.ProductPowerVoltageTypeId;
                    if (current.IEERNonDucted != import.IEERNonDucted) current.IEERNonDucted = import.IEERNonDucted;
                    if (current.EERNonDucted != import.EERNonDucted) current.EERNonDucted = import.EERNonDucted;
                    if (current.SEERNonDucted != import.SEERNonDucted) current.SEERNonDucted = import.SEERNonDucted;
                    if (current.HSPFNonDucted != import.HSPFNonDucted) current.HSPFNonDucted = import.HSPFNonDucted;
                    if (current.COP47NonDucted != import.COP47NonDucted) current.COP47NonDucted = import.COP47NonDucted;
                    if (current.CoolingCapacityRated != import.CoolingCapacityRated) current.CoolingCapacityRated = import.CoolingCapacityRated;
                    if (current.HeatingCapacityRated != import.HeatingCapacityRated) current.HeatingCapacityRated = import.HeatingCapacityRated;
                    if (current.ProductHeatExchangerTypeId != import.ProductHeatExchangerTypeId)
                    {
                        current.ProductHeatExchangerTypeId = import.ProductHeatExchangerTypeId;
                    }
                    if (current.UnitInstallationTypeId != import.UnitInstallationTypeId)
                    {
                        current.UnitInstallationTypeId = import.UnitInstallationTypeId ?? 1;
                    }
                    if (current.ProductCompressorStageId != import.ProductCompressorStageId)
                    {
                        current.ProductCompressorStageId = import.ProductCompressorStageId ?? 0;
                    }
                    if (current.ProductGasValveTypeId != import.ProductGasValveTypeId)
                    {
                        current.ProductGasValveTypeId = import.ProductGasValveTypeId ?? 0;
                    }
                    if (current.ProductMotorSpeedTypeId != import.ProductMotorSpeedTypeId)
                    {
                        current.ProductMotorSpeedTypeId = import.ProductMotorSpeedTypeId ?? 0;
                    }
                    if (current.ProductInstallationConfigurationTypeId != import.ProductInstallationConfigurationTypeId)
                    {
                        current.ProductInstallationConfigurationTypeId = import.ProductInstallationConfigurationTypeId ?? 0;
                    }
                    if (current.CoolingCapacityNominal != import.CoolingCapacityNominal)
                    {
                        current.CoolingCapacityNominal = import.CoolingCapacityNominal;
                    }
                    if (current.AirFlowRateHighCooling != import.AirFlowRateHighCooling)
                    {
                        current.AirFlowRateHighCooling = import.AirFlowRateHighCooling;
                    }
                    if (current.AirFlowRateHighHeating != import.AirFlowRateHighHeating)
                    {
                        current.AirFlowRateHighHeating = import.AirFlowRateHighHeating;
                    }


                    Db.Entry(current).State = EntityState.Modified;

                    //#######################################################################
                    // Update product specifications
                    //#######################################################################
                    var currentlookup = current.ProductSpecifications.ToArray().ToDictionary(s => s.ProductSpecificationLabelId);

                    if (currentlookup.Any(i => i.Key == 146))
                    {
                        //break;
                    }

                    foreach (var importSpec in import.ProductSpecifications)
                    {
                        ProductSpecification currentSpec;

                        if (currentlookup.TryGetValue(importSpec.ProductSpecificationLabelId, out currentSpec))
                        {
                            if (currentSpec.Value != importSpec.Value)
                            {
                                currentSpec.Value = importSpec.Value;
                            }
                        }
                        else
                        {
                            importSpec.ProductId = current.ProductId;
                            Db.Context.ProductSpecifications.Add(importSpec);
                        }
                    };

                    // Remove any current specs which now dont exist (or have become blank)
                    var importlookup = import.ProductSpecifications.ToArray().ToDictionary(s => s.ProductSpecificationLabelId);

                    foreach (var currentSpec in current.ProductSpecifications)
                    {
                        if (!importlookup.ContainsKey(currentSpec.ProductSpecificationLabelId))
                        {
                            removeList.Add(currentSpec);
                        }
                    };

                    var currentNotes = Db.Context.ProductNotes.Where(pn => pn.ProductId == current.ProductId)
                                                 .ToDictionary(pn => pn.ProductNoteId);

                    foreach (var importNote in import.ProductNotes)
                    {
                        ProductNote note = new ProductNote();

                        if (currentNotes.TryGetValue(importNote.ProductNoteId, out note))
                        {
                            if (note.Description != importNote.Description)
                            {
                                note.Description = importNote.Description ?? string.Empty;
                            }
                            if (note.ProductNoteTypeId != importNote.ProductNoteTypeId)
                            {
                                note.ProductNoteTypeId = (int)importNote.ProductNoteTypeId;
                            }
                            if (note.Rank != importNote.Rank)
                            {
                                note.Rank = (short)(importNote.Rank);
                            }
                            if (note.ModifiedOn != importNote.ModifiedOn)
                            {
                                note.ModifiedOn = importNote.ModifiedOn;
                            }
                            if (note.ProductId != importNote.ProductId)
                            {
                                note.ProductId = importNote.ProductId;
                            }
                        }
                        else
                        {
                            note = new DPO.Data.ProductNote
                            {
                                ProductNoteId = importNote.ProductNoteId,
                                Description = importNote.Description,
                                ProductNoteTypeId = importNote.ProductNoteTypeId,
                                ModifiedOn = importNote.ModifiedOn,
                                Rank = (short)importNote.Rank,
                                ProductId = importNote.ProductId
                            };

                            currentNotes.Add(importNote.ProductNoteId, note);
                        }

                        Db.Context.ProductNotes.Add(note);
                    }

                    // Remove any current notes which now dont exist (or have become blank)
                    var importNotelookup = import.ProductNotes.ToArray().ToDictionary(n => n.ProductNoteId);

                    foreach (var importNote in current.ProductNotes)
                    {
                        if (!currentNotes.ContainsKey(importNote.ProductNoteId))
                        {
                            removeNoteList.Add(importNote);
                        }
                    };
                }
            }

            // Db.Context.ChangeTracker.DetectChanges();

            Db.Context.Configuration.AutoDetectChangesEnabled = false;

            Db.Context.SaveChanges();

            Db.ReadOnly = true;

            Db.Context.ProductSpecifications.RemoveRange(removeList);

            Db.Context.Configuration.AutoDetectChangesEnabled = false;

            Db.Context.SaveChanges();

            Db.Context.ProductNotes.RemoveRange(removeNoteList);

            Db.Context.SaveChanges();

        }
        public void DatabaseUpdatePrice(Repository Db, List<Product> updateList, Dictionary<string, Product> dbCurrentList)
        {
            _log.Info("Enter DatabaseUpdatePrice()");

            Db.Context.ReadOnly = false;

            foreach (var import in updateList)
            {
                Product current;

                if (dbCurrentList.TryGetValue(import.ProductNumber, out current))
                {
                    if (current.ListPrice != import.ListPrice) current.ListPrice = import.ListPrice;
                }
            }

            try
            {
                Db.Context.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.FatalFormat("Update Product List Price to database Failed");
                _log.FatalFormat("Excetion Details: {0}", (ex.InnerException != null) ?
                                 ex.InnerException.InnerException.Message : ex.Message);
            }

            Db.ReadOnly = true;

            Db.Context.SaveChanges();

        }
        public Dictionary<string, Product> GetCurrentProducts()
        {
            var Db = this;

            // Update any products
            Db.Context.ReadOnly = true;

            // Get all current products which match imported records
            var currentProducts = Db.Context.Products.ToList();

            //var duplicateRecords = from p in currentProducts
            //                       group p by new { p.ProductNumber } into grp
            //                       where grp.Count() > 1
            //                       select grp.Key;

            currentProducts = currentProducts.GroupBy(p => p.ProductNumber)
                                                     .Select(g => g.First())
                                                     .ToList();

            //currentProducts.RemoveAll(p => duplicateRecords.Any(dp => dp.ProductNumber == p.ProductNumber));

            return currentProducts.ToDictionary(d => d.ProductNumber);
        }

        public void UpdateSingleProductAndProductSpecifications(Product product)
        {
            _log.Info("Enter UpdateProductAndProductSpecifications");

            var Db = this;
            Db.Context.ReadOnly = true;

            bool isUpdate = false;

            if (this.Context.Products.Any(p => p.ProductId == product.ProductId))
            {
                isUpdate = true;
            }

            if (isUpdate)
            {
                try
                {
                    DatabaseUpdateProduct(product);
                }
                catch (Exception ex)
                {
                    _log.ErrorFormat("Add Product data error. ProductNumber: {0}", product.ProductNumber);
                    _log.ErrorFormat("Exception details: {0}", (ex.InnerException != null) ?
                                     ex.InnerException.InnerException.Message : ex.Message);
                    _log.ErrorFormat("productId: {0}, productNumber: {1}", product.ProductId, product.ProductNumber);
                }
            }
            else
            {
                try
                {
                    DatabaseAddProduct(product);
                }
                catch (Exception ex)
                {
                    _log.ErrorFormat("Update Product data error. ProductNumber: {0}", product.ProductNumber);
                    _log.ErrorFormat("Exception details: {0}", (ex.InnerException != null) ?
                                     ex.InnerException.InnerException.Message : ex.Message);
                    _log.ErrorFormat("productId: {0}, productNumber: {1}", product.ProductId, product.ProductNumber);
                }
                _log.Info("Finished execute UpdateProductAndProductSpecifications()");
            }

        }
        public void UpdateProductAndProductSpecifications(List<Product> imports, Dictionary<string, Product> currentProductsLookup)
        {
            _log.Info("Enter UpdateProductAndProductSpecifications");

            var Db = this;
            Db.Context.ReadOnly = true;

            var importListUpdating = new List<Product>();

            var importListAdding = new List<Product>();

            var itemCount = 0;
            var cursor = 0;

            foreach (var importItem in imports)
            {
                if (currentProductsLookup.ContainsKey(importItem.ProductNumber))
                {
                    importItem.ProductId = currentProductsLookup[importItem.ProductNumber].ProductId;
                    importListUpdating.Add(importItem);
                }
                else
                {
                    importListAdding.Add(importItem);
                }

                itemCount += 1;
                cursor += 1;

                if ((itemCount == 100) && (imports.Count() - cursor > 100))
                {
                    using (var scope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(0, 5, 0)))
                    {
                        try
                        {
                            DatabaseAdd(Db, importListAdding);
                            DatabaseUpdate(Db, importListUpdating, currentProductsLookup);
                        }
                        catch (Exception ex)
                        {
                            _log.ErrorFormat("Import Product data error. ProductNumber: {0}", importItem.ProductNumber);
                            _log.ErrorFormat("Exception details: {0}", (ex.InnerException != null) ?
                                             ex.InnerException.InnerException.Message : ex.Message);
                            _log.ErrorFormat("productId: {0}, productNumber: {1}", importItem.ProductId, importItem.ProductNumber);

                            continue;
                        }

                        itemCount = 0;
                        importListUpdating.Clear();
                        importListAdding.Clear();
                        scope.Complete();
                    }
                }

                if (cursor == imports.Count())
                {
                    try
                    {
                        DatabaseAdd(Db, importListAdding);
                        DatabaseUpdate(Db, importListUpdating, currentProductsLookup);
                    }
                    catch (Exception ex)
                    {
                        _log.ErrorFormat("Import Product data error. ProductNumber: {0}", importItem.ProductNumber);
                        _log.ErrorFormat("Exception details: {0}", (ex.InnerException != null) ?
                                      ex.InnerException.InnerException.Message : ex.Message);
                        _log.ErrorFormat("productId: {0}, productNumber: {1}", importItem.ProductId, importItem.ProductNumber);
                        continue;
                    }
                }
            }
            _log.Info("Finished execute UpdateProductAndProductSpecifications()");
        }

        //    #region New code

        //    using (var scope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(0, 25, 0)))
        //    {
        //        var itemCount = 0;
        //        var cursor = 0;

        //        foreach (var importItem in imports)
        //        {
        //            if (currentProductsLookup.ContainsKey(importItem.ProductNumber))
        //            {
        //                importItem.ProductId = currentProductsLookup[importItem.ProductNumber].ProductId;
        //                importListUpdating.Add(importItem);
        //            }
        //            else
        //            {
        //                importListAdding.Add(importItem);
        //            }

        //            itemCount += 1;
        //            cursor += 1;

        //            if ((itemCount == 100) && (imports.Count() - cursor > 100))
        //            {
        //                try
        //                {
        //                    DatabaseAdd(Db, importListAdding);
        //                    DatabaseUpdate(Db, importListUpdating, currentProductsLookup);
        //                }
        //                catch (Exception ex)
        //                {
        //                    continue;
        //                }

        //                itemCount = 0;
        //                importListUpdating.Clear();
        //                importListAdding.Clear();
        //            }

        //            if (itemCount != 100 && (imports.Count() - cursor < 100))
        //            {
        //                try
        //                {
        //                    DatabaseAdd(Db, importListAdding);
        //                    DatabaseUpdate(Db, importListUpdating, currentProductsLookup);
        //                }
        //                catch (Exception ex)
        //                {

        //                }
        //            }
        //        }

        //        scope.Complete();
        //    }
        //}

        #region Update Product Prices
        public void UpdateProductPrices(List<Product> imports)
        {
            _log.Info("Enter UpdateProductPrices()");
            var Db = this;

            // Update any products
            Db.Context.ReadOnly = true;


            var currentProducts = Db.Context.Products.ToList();

            var duplicateRecords = from p in currentProducts
                                   group p by new { p.ProductNumber } into grp
                                   where grp.Count() > 1
                                   select grp.Key;

            currentProducts.RemoveAll(p => duplicateRecords.Any(dp => dp.ProductNumber == p.ProductNumber));

            // Get all current products which match imported records
            var currentProductsLookup = currentProducts.ToDictionary(d => d.ProductNumber);

            using (var scope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(0, 15, 0)))
            {

                DatabaseUpdatePrice(Db, imports, currentProductsLookup);

                scope.Complete();
            }

            _log.Info("Finished UpdateProductPrices()");
        }
        #endregion

        #region Documents Product Update

        public void UpdateDocumentProductLinksByProductNumber(List<DocumentProductLink> imports, Dictionary<string, Product> currentProductsLookup)
        {
            _log.Info("Enter UpdateDocumentProductLinksByProductNumber()");

            this.ReadOnly = false;

            // verify all product/document ids remove any not found
            var documentLookup = this.Context.Documents.Select(d => d.DocumentId).ToArray().ToDictionary(d => d);

            var currentProducts = this.Context.Products.ToList();

            var duplicateRecords = from p in currentProducts
                                   group p by new { p.ProductNumber } into grp
                                   where grp.Count() > 1
                                   select grp.Key;

            currentProducts.RemoveAll(p => duplicateRecords.Any(dp => dp.ProductNumber == p.ProductNumber));

            var verfiedImports = new List<DocumentProductLink>();

            // verify import records have a valid document and product
            foreach (var import in imports)
            {
                if (documentLookup.ContainsKey(import.DocumentId) && currentProductsLookup.ContainsKey(import.ImportProductNumber))
                {
                    import.ProductId = currentProductsLookup[import.ImportProductNumber].ProductId;
                    verfiedImports.Add(import);
                }
            }

            // all document links for all products in import
            var currentDocumentProductLinks = this.Context.DocumentProductLinks.ToArray()
                                                  .GroupBy(p => p.ProductId)
                                                  .ToDictionary(g => g.Key, g => g.ToList());

            // group import by product number , import and update db
            var linksToAdd = new List<DocumentProductLink>();
            var linksToDelete = new List<DocumentProductLink>();

            var importLinks = verfiedImports.GroupBy(p => p.ProductId).ToDictionary(g => g.Key, g => g.ToList());

            foreach (var importProductLinks in importLinks)
            {
                List<DocumentProductLink> currentProductlinks = null;

                if (currentDocumentProductLinks.TryGetValue(importProductLinks.Key, out currentProductlinks))
                {
                    // Find any that need to be added
                    var add = (from i in importProductLinks.Value
                               join c in currentProductlinks on
                               new { i.ImportProductNumber, i.DocumentId } equals
                               new { c.ImportProductNumber, c.DocumentId } into lc
                               from c in lc.DefaultIfEmpty()
                               where c == null
                               select i).AsEnumerable();

                    this.Context.ReadOnly = true;
                    if (add.Count() > 0)
                    {
                        linksToAdd.AddRange(add);
                    }

                    // Find any that need to be deleted
                    var del = (from c in currentProductlinks
                               join i in importProductLinks.Value on new { c.ImportProductNumber, c.DocumentId } equals new { i.ImportProductNumber, i.DocumentId } into lc
                               from i in lc.DefaultIfEmpty()
                               where i == null
                               select c).AsEnumerable();

                    if (del.Count() > 0)
                    {
                        linksToDelete.AddRange(del);
                    }

                    // Find any that have the ranks changed
                    var upts =
                    (from c in currentProductlinks
                     join i in importProductLinks.Value on new { c.ImportProductNumber, c.DocumentId } equals new { i.ImportProductNumber, i.DocumentId }
                     where c.Rank != i.Rank
                     select new { current = c, import = i }
                    )
                    .ToList();

                    upts.ForEach(u => { u.current.Rank = u.import.Rank; });

                }
                else
                {
                    linksToAdd.AddRange(importProductLinks.Value);
                }
            }

            this.Context.SaveChanges();

            this.Context.ReadOnly = true;

            this.Context.DocumentProductLinks.RemoveRange(linksToDelete);

            this.Context.SaveChanges();

            this.Context.DocumentProductLinks.AddRange(linksToAdd);

            this.Context.SaveChanges();

        }

        #endregion

        #region Documents Update

        public void UpdateDocuments(List<Document> imports)
        {
            var Db = this;

            // Get all current products which match imported records
            var currentDocumentsLookup = Db.Context.Documents.ToArray().ToDictionary(d => d.DocumentId);

            var documentsAdding = new List<Document>();

            this.Context.ReadOnly = false;

            using (var scope = new TransactionScope(TransactionScopeOption.Required, new System.TimeSpan(0, 15, 0)))
            {
                foreach (var import in imports)
                {
                    Document current;
                    if (currentDocumentsLookup.TryGetValue(import.DocumentId, out current))
                    {
                        if (current.CreatedOn != import.CreatedOn) current.CreatedOn = import.CreatedOn;
                        if (current.DocumentTypeId != import.DocumentTypeId) current.DocumentTypeId = import.DocumentTypeId;
                        if (current.FileName != import.FileName) current.FileName = import.FileName;
                        if (current.ModifiedOn != import.ModifiedOn) current.ModifiedOn = import.ModifiedOn;
                    }
                    else
                    {
                        documentsAdding.Add(import);
                    }
                }
            }

            this.Context.ReadOnly = true;

            this.Context.Documents.AddRange(documentsAdding);

            this.Context.SaveChanges();

        }

        #endregion

        #region Product Familes

        public IQueryable<ProductModel> GetParentSystemComponents(long? productId)
        {
            if (!productId.HasValue)
            {
                return null;
            }

            var result = from pa in this.Context.ProductAccessories
                         join p in this.Context.Products on pa.ParentProductId equals p.ProductId
                         where pa.ProductId == productId.Value
                         select new ProductModel
                         {
                             ProductId = p.ProductId,
                             Name = p.Name,
                             Price = p.ListPrice,
                             ProductModelTypeId = (ProductModelTypeEnum)p.ProductModelTypeId,
                             ProductNumber = p.ProductNumber,
                             Image = new DocumentModel
                             (

                             )
                         };

            return result;
        }

        public IQueryable<Product> GetProducts(UserSessionModel admin)
        {
            IQueryable<Product> result;
            if (admin.UserTypeId >= UserTypeEnum.DaikinSuperUser)
            {
                result = from product in this.Products
                         join perm1 in this.Permissions
                            on new { a = admin.UserId, b = (byte)PermissionTypeEnum.ProductFamily, c = product.ProductFamilyId }
                            equals new { a = perm1.ObjectId, b = (byte)perm1.PermissionTypeId, c = perm1.ReferenceId }
                         join perm2 in this.Permissions
                            on new { a = admin.UserId, b = (byte)PermissionTypeEnum.Brand, c = product.BrandId }
                            equals new { a = perm2.ObjectId, b = (byte)perm2.PermissionTypeId, c = perm2.ReferenceId }
                         select product;
            }
            else
            {
                result = from product in this.Products
                         join perm1 in this.Permissions
                           on new { a = admin.UserId, b = (byte)PermissionTypeEnum.ProductFamily, c = product.ProductFamilyId }
                           equals new { a = perm1.ObjectId, b = (byte)perm1.PermissionTypeId, c = perm1.ReferenceId }
                         join perm2 in this.Permissions
                           on new { a = admin.UserId, b = (byte)PermissionTypeEnum.Brand, c = product.BrandId }
                           equals new { a = perm2.ObjectId, b = (byte)perm2.PermissionTypeId, c = perm2.ReferenceId }
                         where product.ProductStatusId == (int)ProductStatusTypeEnum.Active
                            || product.ProductStatusId == (int)ProductStatusTypeEnum.Abolished
                         select product;
            }

            return result.Distinct();  // return only one product if the product return is duplicate because permission is duplicate
        }

        //Daikin Equip App
        //returns all products - no user permissions 
        public IQueryable<Product> GetAllProducts()
        {
            IQueryable<Product> result;
            result = from product in this.Products
                     select product;
            return result;
        }


        public IQueryable<Product> GetProductsByProductId(UserSessionModel user, long? productId)
        {
            var result = from products in GetProducts(user) where products.ProductId == productId select products;
            return result;
        }

        public IQueryable<Product> GetProductsByProductNumber(UserSessionModel user, string productNumber)
        {
            var result = from products in GetProducts(user) where products.ProductNumber == productNumber select products;
            return result;
        }

        public IQueryable<Product> GetProductsByProductNumbers(UserSessionModel user, string[] productNumbers)
        {
            var result = from products in GetProducts(user)
                         where productNumbers.Contains(products.ProductNumber)
                         select products;
            return result;
        }
        public long? GetSystemComponentProductId(long? systemProductId, ProductModelTypeEnum type)
        {
            if (!systemProductId.HasValue || (type != ProductModelTypeEnum.Indoor && type != ProductModelTypeEnum.Outdoor)) return null;

            var result = this.Context.VwProductSystemComponents
                                      .Where(p => p.ProductId == systemProductId.Value && p.ComponentModelTypeId == (int)type)
                                      .Select(p => p.ComponentProductId)
                                      .FirstOrDefault();
            return result;
        }
        public IQueryable<ProductCategory> ProductCategoryQuery(UserSessionModel admin, long productFamilyId)
        {
            var result = (from product in this.GetProducts(admin)
                          where product.ProductFamilyId == productFamilyId
                          select product.ProductCategory).Distinct();

            return result;
        }

        public IQueryable<ProductFamily> ProductFamilyQuery(UserSessionModel admin)
        {
            var result = (from product in this.GetProducts(admin)
                          select product.ProductFamily).Distinct();

            return result;
        }

        public IQueryable<ProductStatus> GetProductStatuses(UserSessionModel user)
        {
            IQueryable<ProductStatus> result;
            if (user.UserTypeId >= UserTypeEnum.DaikinSuperUser)
            {
                result = from productStatus in this.Context.ProductStatuses
                         where productStatus.ProductStatusId != (int)ProductStatusTypeEnum.New
                         select productStatus;
            }
            else
            {
                result = from productStatus in this.Context.ProductStatuses
                         where productStatus.ProductStatusId != (int)ProductStatusTypeEnum.New
                                && productStatus.ProductStatusId != (int)ProductStatusTypeEnum.HiddenModuleUnit
                         select productStatus;
            }

            return result;
        }

        public IQueryable<InventoryStatus> GetInventoryStatuses(UserSessionModel user)
        {
            IQueryable<InventoryStatus> result = from inventoryStatus in this.Context.InventoryStatuses
                                                 select inventoryStatus;

            return result;
        }



        //public IQueryable<ProductFamily> ProductFamilyQueryWithOrder(UserSessionModel admin)
        //{
        //    var query = from p in this.GetProducts(admin)
        //                join f in this.ProductFamilies on p.ProductFamilyId equals f.ProductFamilyId
        //                select new ProductFamily
        //                {
        //                    ProductFamilyId = p.ProductFamilyId,
        //                    Name = f.Name,
        //                    Description = f.Name,
        //                    Order = f.Order
        //                };

        //    var distinctFamilies = query.Distinct().OrderBy(f => f.Order).AsQueryable();

        //    return distinctFamilies;
        //}
        #endregion

        public void UpdateUnitInstallationType(List<UnitInstallationType> unitInstallationTypeList)
        {
            var Db = this;
            Db.Context.ReadOnly = false;
            foreach (var item in unitInstallationTypeList)
            {
                UnitInstallationType current = item;
                Db.Entry(current).State = EntityState.Modified;
            }
            try
            {
                Db.Context.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.FatalFormat("Update UnitInstallationType to database Failed");
                _log.FatalFormat("Excetion Details: {0}", (ex.InnerException != null) ? ex.InnerException.InnerException.Message : ex.Message);
            }
        }
        public void AddUnitInstallationType(List<UnitInstallationType> unitInstallationTypeList)
        {
            var Db = this;
            Db.Context.ReadOnly = false;
            Db.Context.UnitInstallationTypes.AddRange(unitInstallationTypeList);
            try
            {
                Db.Context.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.FatalFormat("Add UnitInstallationType to database Failed");
                _log.FatalFormat("Excetion Details: {0}", (ex.InnerException != null) ? ex.InnerException.InnerException.Message : ex.Message);
            }
        }


        public void UpdateSubmittalSheetType(List<SubmittalSheetType> submittalSheetTypeList)
        {
            var Db = this;
            Db.Context.ReadOnly = false;
            foreach (var item in submittalSheetTypeList)
            {
                SubmittalSheetType current = item;
                Db.Entry(current).State = EntityState.Modified;
            }
            try
            {
                Db.Context.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.FatalFormat("Update SubmittalSheetType to database Failed");
                _log.FatalFormat("Excetion Details: {0}", (ex.InnerException != null) ? ex.InnerException.InnerException.Message : ex.Message);
            }
        }

        public void AddSubmittalSheetType(List<SubmittalSheetType> submittalSheetTypeList)
        {
            var Db = this;
            Db.Context.ReadOnly = false;
            Db.Context.SubmittalSheetTypes.AddRange(submittalSheetTypeList);
            try
            {
                Db.Context.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.FatalFormat("Add SubmittalSheetType to database Failed");
                _log.FatalFormat("Excetion Details: {0}", (ex.InnerException != null) ? ex.InnerException.InnerException.Message : ex.Message);
            }
        }
        private IQueryable<Product> Filter(IQueryable<Product> query, SearchProduct search)
        {
            if (search == null) return query;

            if (!string.IsNullOrWhiteSpace(search.ProductNumber))
            {
                search.ProductNumber = search.ProductNumber.Trim();
                query = query.Where(s => s.ProductNumber.Contains(search.ProductNumber));
            }

            if (search.ProductModelTypeId.HasValue)
            {
                query = query.Where(s => s.ProductModelTypeId == (int)search.ProductModelTypeId.Value);
            }

            if (search.UnitInstallationTypeId.HasValue)
            {
                query = query.Where(s => s.UnitInstallationTypeId == (int)search.UnitInstallationTypeId.Value);
            }

            if (search.ProductFamilyId.HasValue)
            {
                query = query.Where(s => s.ProductFamilyId == search.ProductFamilyId);
            }

            if (search.ProductCategoryId.HasValue)
            {
                query = FilterOnProductCategory(query, ProductModelTypeEnum.Indoor, search.ProductCategoryId);
            }


            if (search.ProductPowerVoltageTypeId.HasValue)
            {
                query = query.Where(s => s.ProductPowerVoltageTypeId == search.ProductPowerVoltageTypeId);
            }

            if (search.HeatingCapacityRatedMin.HasValue)
            {
                query = query.Where(s => s.HeatingCapacityRated >= search.HeatingCapacityRatedMin);
            }
            if (search.HeatingCapacityRatedMax.HasValue)
            {
                query = query.Where(s => s.HeatingCapacityRated <= search.HeatingCapacityRatedMax);
            }

            if (search.HeatingCapacityRatedValue.HasValue)
            {
                query = query.Where(s => s.HeatingCapacityRated == search.HeatingCapacityRatedValue);
            }

            if (search.CoolingCapacityNominalValue.HasValue)
            {
                query = query.Where(s => s.CoolingCapacityNominal == search.CoolingCapacityNominalValue);
            }

            // TODO:  Airflow rate issue

            if (search.AirFlowRateHighValue.HasValue)
            {
                query = query.Where(s => s.AirFlowRateHighCooling == search.AirFlowRateHighValue);
            }

            //if (search.CoolingCapacityRatedMin.HasValue)
            //{
            //    query = query.Where(s => s.CoolingCapacityRated >= search.CoolingCapacityRatedMin);
            //}
            //if (search.CoolingCapacityRatedMax.HasValue)
            //{
            //    query = query.Where(s => s.CoolingCapacityRated <= search.CoolingCapacityRatedMax);
            //}

            if (search.CoolingCapacityRatedValue.HasValue)
            {
                query = query.Where(s => s.CoolingCapacityRated == (decimal)search.CoolingCapacityRatedValue);
            }

            if (search.ProductCompressorStageId.HasValue)
            {
                query = query.Where(s => s.ProductCompressorStageId == (int?)search.ProductCompressorStageId);
            }

            if (search.ProductGasValveTypeId.HasValue)
            {
                query = query.Where(s => s.ProductGasValveTypeId == (int?)search.ProductGasValveTypeId);
            }

            if (search.ProductMotorSpeedTypeId.HasValue)
            {
                query = query.Where(s => s.ProductMotorSpeedTypeId == (int?)search.ProductMotorSpeedTypeId);
            }

            if (search.ProductInstallationConfigurationTypeId.HasValue)
            {
                query = query.Where(s => s.ProductInstallationConfigurationTypeId == (int?)search.ProductInstallationConfigurationTypeId);
            }



            if (search.EERNonDuctedMin.HasValue)
            {
                query = query.Where(s => s.EERNonDucted >= search.EERNonDuctedMin);
            }
            if (search.EERNonDuctedMax.HasValue)
            {
                query = query.Where(s => s.EERNonDucted <= search.EERNonDuctedMax);
            }

            if (search.IEERNonDuctedMin.HasValue)
            {
                query = query.Where(s => s.IEERNonDucted >= search.IEERNonDuctedMin);
            }
            if (search.IEERNonDuctedMax.HasValue)
            {
                query = query.Where(s => s.IEERNonDucted <= search.IEERNonDuctedMax);
            }

            if (search.SEERNonDuctedMin.HasValue)
            {
                query = query.Where(s => s.SEERNonDucted >= search.SEERNonDuctedMin);
            }
            if (search.SEERNonDuctedMax.HasValue)
            {
                query = query.Where(s => s.SEERNonDucted <= search.SEERNonDuctedMax);
            }

            if (search.COP47NonDuctedMin.HasValue)
            {
                query = query.Where(s => s.COP47NonDucted >= search.COP47NonDuctedMin);
            }
            if (search.COP47NonDuctedMax.HasValue)
            {
                query = query.Where(s => s.COP47NonDucted <= search.COP47NonDuctedMax);
            }

            if (search.HSPFNonDuctedMin.HasValue)
            {
                query = query.Where(s => s.HSPFNonDucted >= search.HSPFNonDuctedMin);
            }
            if (search.HSPFNonDuctedMax.HasValue)
            {
                query = query.Where(s => s.HSPFNonDucted <= search.HSPFNonDuctedMax);
            }

            if (search.ProductHeatExchangerTypeId.HasValue)
            {
                query = FilterOnSpecification(query, "ProductHeatExchangerType", ProductModelTypeEnum.Outdoor, search.ProductHeatExchangerTypeId);
            }

            if (search.UnitInstallationTypeId.HasValue)
            {
                query = FilterOnSpecification(query, "UnitInstallationType", ProductModelTypeEnum.Outdoor, (int?)search.UnitInstallationTypeId);
            }

            if (!string.IsNullOrEmpty(search.Filter))
            {
                search.Filter = search.Filter.Trim();
                query = query.Where(s => s.ProductNumber.Contains(search.Filter) || s.Name.Contains(search.Filter));
            }

            return query;
        }

        /*This function filters products by Product Family, Model Type, Installation Type && Search params
         * other filters is done on client side.
        */
        private IQueryable<Product> FilterV2(IQueryable<Product> query, SearchProduct search)
        {
            if (search == null) return query;

            if (!string.IsNullOrWhiteSpace(search.ProductNumber))
            {
                search.ProductNumber = search.ProductNumber.Trim();
                query = query.Where(s => s.ProductNumber.Contains(search.ProductNumber));
            }

            if (search.ProductModelTypeId.HasValue && search.ProductModelTypeId != ProductModelTypeEnum.All)
            {
                query = query.Where(s => s.ProductModelTypeId == (int)search.ProductModelTypeId.Value);
            }

            if (search.UnitInstallationTypeId.HasValue && search.UnitInstallationTypeId != UnitInstallationTypeEnum.All)
            {
                query = query.Where(s => s.UnitInstallationTypeId == (int)search.UnitInstallationTypeId.Value);
            }

            if (search.ProductFamilyId.HasValue)
            {
                query = query.Where(s => s.ProductFamilyId == search.ProductFamilyId);

            }

            if (search.ProductClassPIMId.HasValue && search.ProductClassPIMId != ProductClassPIMEnum.All)
            {
                query = query.Where(s => s.ProductClassPIMId == (int)search.ProductClassPIMId.Value);
            }

            if (search.ProductTypeId.HasValue)
            {
                query = query.Where(s => s.ProductTypeId == search.ProductTypeId);
            }

            if (search.ProductStatusTypeId.HasValue)
            {
                query = query.Where(s => s.ProductStatusId == (int)search.ProductStatusTypeId);
            }

            if (!string.IsNullOrEmpty(search.Filter))
            {
                search.Filter = search.Filter.Trim();
                query = query.Where(s => s.ProductNumber.Contains(search.Filter) || s.Name.Contains(search.Filter));
            }

            return query;
        }


        private IQueryable<Product> FilterOnProductCategory(IQueryable<Product> query, ProductModelTypeEnum filterSubProduct, int? selectedId)
        {
            if (selectedId.HasValue)
            {
                query = query
                        .Where(w =>
                                (w.ProductModelTypeId != (int)ProductModelTypeEnum.System
                                && w.ProductCategoryId == selectedId.Value)
                                || (w.ParentProductAccessories
                                        .Any(wppa => wppa.RequirementTypeId == (int)RequirementTypeEnums.Standard
                                                && wppa.Product.ProductModelTypeId == (int)filterSubProduct
                                                && wppa.Product.ProductCategoryId == selectedId.Value)
                                    )
                        );
            }

            return query;
        }
        private IQueryable<Product> FilterOnSpecification(IQueryable<Product> query, string specificationName, ProductModelTypeEnum filterModelType, int? selectedSpecification)
        {
            if (selectedSpecification.HasValue)
            {
                query = query
                        .Where(w =>
                                w.ProductSpecifications.Any(
                                    wps =>
                                        wps.Product.ProductModelTypeId != (int)ProductModelTypeEnum.System
                                        && wps.ProductSpecificationLabel.Name == specificationName
                                        && wps.Value == selectedSpecification.ToString())
                                || (w.ParentProductAccessories
                                        .Any
                                        (
                                            wppa => wppa.RequirementTypeId == (int)RequirementTypeEnums.Standard
                                                && wppa.Product.ProductModelTypeId == (int)filterModelType
                                                && wppa.Product.ProductSpecifications
                                                    .Any(ops =>
                                                        ops.ProductSpecificationLabel.Name == specificationName
                                                        && ops.Value == selectedSpecification.ToString())
                                        )
                                    )
                        );
            }

            return query;
        }
        private IQueryable<Product> Sort(IQueryable<Product> query, SearchProduct search)
        {
            if (search == null) return query;

            string sortcolumn = (search.SortColumn + "").ToLower();

            bool desc = search.IsDesc;

            switch (sortcolumn)
            {
                case "productnumber":
                    query = (desc) ? query.OrderByDescending(s => s.ProductNumber) : query.OrderBy(s => s.ProductNumber);
                    break;
                default:
                    query = (desc) ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name);
                    break;
            }

            return query;
        }

        //private IQueryable<DocumentProductLink> Filter(IQueryable<DocumentProductLink> query, SearchDocumentProductLink search) {
        //    if (search == null) return query;

        //    if (search.ProductId.HasValue)
        //    {
        //        query = query.Where(s => s.ProductId == search.ProductId);

        //    }
        //    if (search.DocumentId != null)
        //    {
        //        query = query.Where(s => s.DocumentId == search.DocumentId);
        //    }
        //    //if (search.ProductFamilyId != 0)
        //    //{
        //    //    query = query.Where(s => s.ProductFamilyId == search.ProductFamilyId);
        //    //}
        //    //if (search.DocumentId != null)
        //    //{
        //    //    query = query.Where(s => s.DocumentId == search.DocumentId);
        //    //}

        //    return query;
        //}


    }
}