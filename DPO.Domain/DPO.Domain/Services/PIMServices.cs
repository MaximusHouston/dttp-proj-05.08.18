using DPO.Common;
using DPO.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using log4net;
using System.Data;
using EntityFramework.Extensions;
using System.Transactions;
using Renci.SshNet;
using System.IO;
using Renci.SshNet.Sftp;

namespace DPO.Domain.Services
{
    /// <summary>
    /// Used to import PIM data into Dainkin City
    /// Product Images:   We assume a default image type of PNG based on the app.config 
    /// </summary>
    public class PIMServices : BaseServices
    {
        protected UserSessionModel daikinSuperUser;

        private static ILog _log = LogManager.GetLogger(typeof(PIMServices));

        public PIMServices()
             : base()
        {
        }

        public PIMServices(DPOContext context)
            : base(context)
        {
            daikinSuperUser = new AccountServices().GetSuperUserSessionModel().Model as UserSessionModel;
        }


        private object ConvertValue(Type type, object val)
        {
            // Nullables require special handling
            var nullableType = Nullable.GetUnderlyingType(type);
            var convType = nullableType ?? type;
            var actualVal = Convert.ChangeType(val, convType);

            return actualVal;
        }

        private IEnumerable<PIMProduct> QueryPIMProductList(List<PIMProduct> pimProducts)
        {
            // HACK:  Remove the criteria
            return from p in pimProducts
                   where (p.ReleaseStatus == (int)ProductStatusTypeEnum.HiddenModuleUnit
                         || p.ReleaseStatus == (int)ProductStatusTypeEnum.Active)
                   //&& p.ID == "KRP4A93"
                   select p;
        }

        private IEnumerable<PIMProduct> QueryDisabledPIMProductList(List<PIMProduct> pimProducts)
        {
            return from p in pimProducts
                   where (p.ReleaseStatus != (int)ProductStatusTypeEnum.HiddenModuleUnit
                         && p.ReleaseStatus != (int)ProductStatusTypeEnum.Active)
                   select p;
        }

        #region Import Lookup Tables

        public void ImportLookupTablesCustom(List<PIMProduct> pimProducts)
        {
            // TODO:  Could we just parse the families?
            ImportLookupTablesCustomSubFamily(pimProducts);
        }

        private void ImportLookupTablesCustomSubFamily(List<PIMProduct> pimProducts)
        {
            Dictionary<string, KeyValuePair<string, string>> newSubFamilies = ImportLookupTablesCustomSubFamilyGetValues(pimProducts);

            // Repo used here for speed purposes
            using (Repository repo = new Repository(null))
            {

                var currentSubFamilies = repo.Context.ProductSubFamilies.ToDictionary(d => d.Id);

                foreach (var newSubFam in newSubFamilies)
                {
                    if (String.IsNullOrWhiteSpace(newSubFam.Key))
                    {
                        _log.WarnFormat("Trying to load a sub family without a key {0} - {1}", newSubFam.Key, newSubFam.Value);
                        continue;
                    }

                    string subFamName = newSubFam.Value.Key; // Name
                    string subFamDesc = newSubFam.Value.Value; // Description

                    ProductSubFamily subFam;
                    if (!currentSubFamilies.TryGetValue(newSubFam.Key, out subFam))
                    {
                        subFam = new ProductSubFamily()
                        {
                            Id = newSubFam.Key,
                            Name = subFamName,
                            Description = subFamDesc
                        };

                        repo.Context.ProductSubFamilies.Add(subFam);
                    }
                    else
                    {
                        if (subFam.Name != subFamName
                            || subFam.Description != subFamDesc)
                        {
                            subFam.Name = subFamName;
                            subFam.Description = subFamDesc;

                            if (repo.Entry(subFam).State == EntityState.Detached)
                            {
                                repo.Context.ProductSubFamilies.Attach(subFam);
                            }

                            repo.Entry(subFam).State = EntityState.Modified;
                        }
                    }
                }

                repo.SaveChanges();
            }
        }

        private Dictionary<string, KeyValuePair<string, string>> ImportLookupTablesCustomSubFamilyGetValues(List<PIMProduct> pimProducts)
        {
            var subFamilies = new Dictionary<string, KeyValuePair<string, string>>();

            foreach (var pimProd in pimProducts)
            {
                // TODO: Get rid of strings
                var qryProdFam = (from s in pimProd.Specifications.Values
                                  where String.Compare(s.AttributeID, "FamilyDescription", true) == 0
                                      || String.Compare(s.AttributeID, "ProductFamily", true) == 0
                                  select s).ToDictionary(d => d.AttributeID);

                IPIMProductSpecification specProdFamily;
                // No family available
                if (!qryProdFam.TryGetValue("ProductFamily", out specProdFamily))
                {
                    continue;
                }

                IPIMProductSpecification specFamDesc;
                // No description available
                if (!qryProdFam.TryGetValue("FamilyDescription", out specFamDesc))
                {
                    continue;
                }

                string subFamilyId = specProdFamily.RawData != null ? specProdFamily.RawData.ToString() : String.Empty;
                string subFamilyDesc = specFamDesc.RawData != null ? specFamDesc.RawData.ToString() : String.Empty;
                string subFamilyName = String.Empty;

                PIMProductSpecification fullSpecProdFam = specProdFamily as PIMProductSpecification;
                if (fullSpecProdFam != null)
                {
                    subFamilyName = fullSpecProdFam.Text;
                }

                if (String.IsNullOrWhiteSpace(subFamilyName))
                {
                    subFamilyName = subFamilyId;
                }

                if (String.IsNullOrWhiteSpace(subFamilyDesc))
                {
                    subFamilyDesc = subFamilyId;
                }

                // Invalid family
                if (String.IsNullOrWhiteSpace(subFamilyId)
                    || String.IsNullOrWhiteSpace(subFamilyDesc))
                {
                    continue;
                }

                if (!subFamilies.ContainsKey(subFamilyId))
                {
                    subFamilies.Add(subFamilyId, new KeyValuePair<string, string>(subFamilyName, subFamilyDesc));
                }
            }

            return subFamilies;
        }

        public void ImportLookupTablesMapped(IDictionary<string, PIMListOfValue> listsOfValues)
        {
            // Load mappings
            var maps = from map in this.Context.PIMListOfValuesMaps
                       where !map.Disabled
                       select map;

            // Convert listsOfValuesDictionary
            var lovs = (from lov in listsOfValues
                        join map in maps on lov.Key equals map.PIMListOfValuesId
                        select new
                        {
                            Key = map.DaikinCityEntityName,
                            Map = map,
                            ListOfValue = lov.Value//,
                                                   //IDField = map.DaikinCityIdField,
                                                   //NameField = map.DaikinCityNameField,
                                                   //HasOtherOption = map.HasOtherOption,
                                                   //OtherOptionId = map.OtherOptionId ?? 1
                        }).ToDictionary(d => d.Key);

            // Load entity types that match mapping
            var entityTypes = this.Context
                .GetType()
                .GetProperties()
                .Where(p =>
                    p.PropertyType.IsGenericType &&
                    p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .Select(p => p.PropertyType.GetGenericArguments()[0])
                .Where(t => maps.Any(a => a.DaikinCityEntityName == t.Name))
                .Select(t => t)
                .ToArray();

            foreach (var entityType in entityTypes)
            {
                var entityLov = (from lov in lovs
                                 where String.Compare(lov.Key, entityType.Name, true) == 0
                                 select lov.Value).FirstOrDefault();

                var entityTypeName = entityType.Name;

                if (entityLov == null || entityLov.ListOfValue == null)
                {
                    continue;
                }

                Console.WriteLine("Loading " + entityType.Name);

                ImportLookupTablesMappedValuesDefaultOption(entityType, entityLov.Map);
                ImportLookupTablesMappedValues(entityType, entityLov.Map, entityLov.ListOfValue);

                // Save the changes
                this.Context.SaveChanges();
            }
        }

        private void ImportLookupTablesMappedValuesDefaultOption(Type entityType, PIMListOfValuesMap map)
        {
            // Load the set
            var set = this.Context.Set(entityType);

            // Create default option
            try
            {
                if (map.HasDefaultOption)
                {
                    var otherOption = set.Find(map.DefaultOptionId);

                    if (otherOption == null)
                    {
                        otherOption = Activator.CreateInstance(entityType);
                        SetObjectProperty(entityType, otherOption, map.DaikinCityIdField, map.DefaultOptionId);
                        SetObjectProperty(entityType, otherOption, map.DaikinCityNameField, map.DefaultOptionName ?? "Other");

                        set.Add(otherOption);

                        this.Context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                _log.WarnFormat("Unable to setup default option for {0} because {1}", entityType.Name, ex.Message);
            }
        }

        private void ImportLookupTablesMappedValues(Type entityType, PIMListOfValuesMap map, PIMListOfValue lov)
        {
            // Load the set
            var set = this.Context.Set(entityType);

            foreach (var value in lov.Values)
            {
                try
                {

                    var idFieldType = GetPropertyType(entityType, map.DaikinCityIdField);
                    // Unable to find ID?
                    if (idFieldType == null)
                    {
                        // TODO:  Log
                        _log.ErrorFormat("Unable to find id field '{0}' for entity '{1}'", map.DaikinCityIdField, entityType.Name);
                        continue;
                    }

                    var id = Convert.ChangeType(value.Key, idFieldType);
                    var item = set.Find(id);

                    // TODO:  Clean this up
                    if (item == null)// new value
                    {
                        //create new value with new Id & new Name
                        item = Activator.CreateInstance(entityType);
                        if (!SetObjectProperty(entityType, item, map.DaikinCityIdField, id))
                        {
                            _log.ErrorFormat("Unable to setup ID property for entity {0}", entityType.Name);
                            continue;
                        }

                        SetObjectProperty(entityType, item, map.DaikinCityNameField, value.Value);

                        set.Add(item);
                    }
                    else// existing value
                    {
                        // Update the name
                        // TODO: Update name if "UpdateNameField == true"
                        SetObjectProperty(entityType, item, map.DaikinCityNameField, value.Value);
                    }
                }
                catch (Exception ex)
                {
                    string message = String.Format("Unable to load values for {0} - {1}", entityType.Name, ex.Message);
                    Console.WriteLine(message);
                    _log.Fatal(message);
                }
            }
        }

        private bool SetObjectProperty(Type type, object item, string propertyName, object value)
        {
            var prop = type.GetProperty(propertyName);
            bool propertySet = false;

            if (prop == null)
            {
                _log.ErrorFormat("Unable to find property for {0} of name {1}", type.Name, propertyName);
            }
            else
            {
                object currentVal = prop.GetValue(item);

                // Only set those items that have changed
                bool setValue = false;

                // TODO:  I don't think this is working correctly due to them being objects...  Need to check
                if (currentVal != value)
                {
                    prop.SetValue(item, value);
                    propertySet = true;
                }

            }

            return propertySet;
        }

        #endregion Import Lookup Tables

        #region Import Spec Labels and Key Lookups

        public void ImportProductSpecificationLabelAndKeyLookup(IDictionary<string, PIMAttribute> attributes)
        {
            foreach (var attr in attributes.Values)
            {
                Console.WriteLine(String.Format("Specification Label {0}", attr.ID));
                var prodSpecLabel = ImportProductSpecificationLabel(attr);
                ImportProductSpecificationKeyLookup(prodSpecLabel, attr);
            }
        }

        private void ImportProductSpecificationKeyLookup(ProductSpecificationLabel prodSpecLabel, PIMAttribute attribute)
        {
            if (attribute.ListOfValues == null || prodSpecLabel == null)
            {
                return;
            }

            var existingKeyLookups = (from k in this.Context.ProductSpecificationKeyLookups
                                      where k.ProductSpecificationLabelId == prodSpecLabel.ProductSpecificationLabelId
                                      select k).ToDictionary(k => k.Key);

            foreach (var val in attribute.ListOfValues.Values)
            {
                if (String.IsNullOrWhiteSpace(val.Key))
                {
                    continue;
                }

                ProductSpecificationKeyLookup keyLookup;

                if (existingKeyLookups.TryGetValue(val.Key, out keyLookup))
                {
                    keyLookup.Value = val.Value;
                }
                else
                {
                    var pskl = new ProductSpecificationKeyLookup()
                    {
                        ProductSpecificationLabelId = prodSpecLabel.ProductSpecificationLabelId,
                        Key = val.Key,
                        Value = val.Value
                    };

                    prodSpecLabel.ProductSpecificationKeyLookups.Add(pskl);
                    this.Context.ProductSpecificationKeyLookups.Add(pskl);
                }
            }

            this.Context.SaveChanges();
        }

        private ProductSpecificationLabel ImportProductSpecificationLabel(PIMAttribute attribute)
        {
            var prodSpecLabel = (from psl in this.Context.ProductSpecificationLabels
                                 where psl.Name == attribute.ID
                                 select psl).FirstOrDefault();

            if (prodSpecLabel == null)
            {
                prodSpecLabel = new ProductSpecificationLabel()
                {
                    Name = attribute.ID,
                    Description = attribute.Name
                };

                this.Context.ProductSpecificationLabels.Add(prodSpecLabel);
                this.Context.SaveChanges();

                prodSpecLabel = (from psl in this.Context.ProductSpecificationLabels
                                 where psl.Name == attribute.ID
                                 select psl).FirstOrDefault();
            }

            return prodSpecLabel;
        }

        #endregion 

        #region Import Products

        public void ImportProducts(List<PIMProduct> productsForImport, Dictionary<string, Product> currentProducts)
        {
            Dictionary<string, PIMAttributeMap> maps = null;
            Dictionary<string, ProductSpecificationLabel> productSpecLabels = null;
            IEnumerable<PropertyInfo> productProps = null;

            var prodType = typeof(Product);

            // Load products to be updated
            // HACK:  Remove the criteria
            var pimProducts = QueryPIMProductList(productsForImport);

            // Using for speed
            using (Repository repo = new Repository(null))
            {
                // Load mappings
                maps = (from map in repo.Context.PIMAttributeMaps
                        where !map.Disabled && map.MapTypeId == PIMMapTypeEnum.Product
                        select map).ToDictionary(d => d.DaikinCityAttributeId);

                // Load entity types that match mapping
                productProps = prodType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(w => maps.Values.Any(a => a.DaikinCityAttributeId == w.Name));



                productSpecLabels = (from p in repo.Context.ProductSpecificationLabels
                                     select p).ToDictionary(k => k.Name);
            }

            // TODO:  This is going to be really slow right now
            int count = 0;
            foreach (var pimProd in pimProducts)
            {
                // For performance we are going to use a new repository and DB Context each time
                using (Repository repo = new Repository(null))
                {
                    try
                    {
                        ImportProduct(repo, currentProducts, pimProd, productProps, maps, productSpecLabels);
                        repo.Context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        var errMsg = String.Format("Unable to load product {0}", pimProd.ID);
                        _log.Exception(ex, "PIM PRODUCT", errMsg);
                    }

                }
                count++;
            }
        }

        private void ImportProduct(Repository repo, Dictionary<string, Product> currentProducts, PIMProduct pimProduct,
            IEnumerable<PropertyInfo> productPropertyInfo, Dictionary<string, PIMAttributeMap> maps,
            Dictionary<string, ProductSpecificationLabel> productSpecLabels)
        {
            // TODO:  Could be slow
            Product prod;

            if (!currentProducts.TryGetValue(pimProduct.ID, out prod))
            {
                prod = repo.ProductCreate(repo.Context.GenerateNextLongId());
                prod.ProductNumber = pimProduct.ID;
                prod.ProductCategoryId = 1; // TODO:  What should this be?
                repo.Context.Products.Add(prod);
            }
            else
            {
                repo.Context.Products.Attach(prod);
            }

            bool productChanged = ImportProductPropertyValues(repo, pimProduct, prod, productPropertyInfo, maps);
            bool additionalProductChanges = ImportProductPropertyValuesCustom(repo, pimProduct, prod);
            productChanged = productChanged || additionalProductChanges;

            // TODO:  Why do I have to do this?
            var entry = repo.Entry(prod);
            if (productChanged && entry.State == EntityState.Unchanged)
            {
                repo.Entry(prod).State = EntityState.Modified;
            }

            ImportProductSpecifications(repo, pimProduct, prod, productSpecLabels);
            DeleteUnusedSpecifications(repo, pimProduct);
        }

        private bool ImportProductPropertyValuesCustom(Repository repo, PIMProduct pimProduct, Product product)
        {
            bool productChanged = ImportProductPropertyValuesCustomFamily(repo, pimProduct, product);

            productChanged = productChanged || ImportProductPropertyValuesCustomPart(repo, pimProduct, product);


            return productChanged;
        }

        private bool ImportProductPropertyValuesCustomFamily(Repository repo, PIMProduct pimProduct, Product product)
        {

            // TODO:  Get rid of these string!!!@#$!%@  Sorry... 
            var productFamilySpec = (from spec in pimProduct.Specifications.Values
                                     where String.Compare(spec.AttributeID, "ProductFamily", true) == 0
                                     select spec).FirstOrDefault();

            if (productFamilySpec == null)
            {
                return false;
            }

            // HACK:  Will this be slow?
            var currentSubFamilies = repo.Context.ProductSubFamilies.Cache().ToDictionary(d => d.Id);

            ProductSubFamily subFam;

            // Handle Nulls
            if (productFamilySpec.Data == null)
            {
                if (product.ProductSubFamilyId != null)
                {
                    product.ProductSubFamilyId = null;
                    return true;
                }

                return false;
            }

            // Find the family
            if (!currentSubFamilies.TryGetValue(productFamilySpec.Data.ToString(), out subFam))
            {
                _log.WarnFormat("Unable to load product sub family for {0}", pimProduct.ID);

                return false;
            }

            // Check the family needs to be reset
            if (product.ProductSubFamilyId != subFam.ProductSubFamilyId)
            {
                product.ProductSubFamilyId = subFam.ProductSubFamilyId;
                return true;
            }

            return false;
        }

        private bool ImportProductPropertyValuesCustomPart(Repository repo, PIMProduct pimProduct, Product product)
        {
            if (product.ProductTypeId.HasValue
                && (product.ProductTypeId.Value == (int)ProductTypeEnum.Accessory
                    || product.ProductTypeId.Value == (int)ProductTypeEnum.Service)
                && product.ProductFamilyId != (int)ProductFamilyEnum.Accessories
                )
            {
                product.ProductFamilyId = (int)ProductFamilyEnum.Accessories;

                return true;
            }

            return false;
        }


        private bool ImportProductPropertyValues(Repository repo, PIMProduct pimProduct, Product product, IEnumerable<PropertyInfo> productPropertyInfo,
            Dictionary<string, PIMAttributeMap> maps)
        {
            var productType = typeof(Product);
            bool productChanged = false;

            foreach (var prop in productPropertyInfo)
            {
                var propType = prop.PropertyType;
                var propName = prop.Name;

                if (!maps.ContainsKey(propName))
                {
                    _log.InfoFormat("Unable to find specification with the name {0}", propName);
                    continue;
                }

                var map = maps[prop.Name];
                var pimAttrId = map.PIMAttributeId;

                if (String.IsNullOrWhiteSpace(pimAttrId))
                {
                    _log.ErrorFormat("Unable to find PIM Attribute ID for Property '{0}'", propName);
                    continue;
                }

                pimAttrId = pimAttrId.Trim().TrimEnd(System.Environment.NewLine.ToCharArray());

                IPIMProductSpecification prodSpec;
                if (!pimProduct.Specifications.TryGetValue(pimAttrId, out prodSpec))
                {
                    // TODO:  Check the drop down mapping to see if default value
                    if (!String.IsNullOrWhiteSpace(map.DefaultValue))
                    {
                        //Console.WriteLine("Unable to find specification with name " + prop.Name);
                       // _log.DebugFormat("Unable to find specification with name {0}", propName);

                        // Setup default
                        if (map.DefaultValue != null)
                        {
                            //_log.DebugFormat("Setting default value on {0}: {1}", pimProduct.ID, propName);
                            productChanged = true;
                            var actualVal = ConvertValue(propType, map.DefaultValue);
                            bool objectSet = SetObjectProperty(productType, product, propName, actualVal);

                            // Did the product change?
                            productChanged = productChanged || objectSet;
                        }
                    }
                    else
                    {
                        //_log.WarnFormat("Unable to find specification with name {0} and no default value available.", propName);
                    }

                    continue;
                }

                var objVal = prodSpec.Data;

                //LMW Added 01/19/2018 
                //Check the issue with AirFlowRateHighCooling NOT converting properly, we want to SUMM each unit for Multi-Mondules
                int sumAirFlowRateHighCooling = 0;
                if (propName.ToString() == "AirFlowRateHighCooling" && objVal.ToString().Contains("+"))
                {
                    string unitAF = objVal.ToString().Replace("+", ",");
                    string[] unitairflowhighcool = unitAF.Split(',');
                    var additup = from unitairflow in unitairflowhighcool select unitairflow;
                    foreach (string value in additup)
                    {
                        sumAirFlowRateHighCooling = sumAirFlowRateHighCooling + Convert.ToInt32(value);
                    }
                }
                // to here

                try
                {
                    //LMW Modified to check above 01/19/2018 
                    if (propName.ToString() == "AirFlowRateHighCooling" && objVal.ToString().Contains("+"))
                    {
                        var actualVal = ConvertValue(propType, sumAirFlowRateHighCooling);
                        bool objectSet = SetObjectProperty(productType, product, propName, actualVal);
                        productChanged = productChanged || objectSet;
                    }
                    else
                    {
                        var actualVal = ConvertValue(propType, objVal);
                        bool objectSet = SetObjectProperty(productType, product, propName, actualVal);
                        productChanged = productChanged || objectSet;
                    }
                }
                catch (Exception ex)
                {
                    var themsg = pimProduct.ID + " propName= " + propName + " objval=" + objVal.ToString() + " SpecificationsCount =" + pimProduct.Specifications.Count;
                    //_log.WarnFormat("Unable to save2 product {0} because '{1}'", themsg, ex.Message);
                }

                //try
                //{
                //    var actualVal = ConvertValue(propType, objVal);

                //    bool objectSet = SetObjectProperty(productType, product, propName, actualVal);
                //    // Did the product change?
                //    productChanged = productChanged || objectSet;

                //}
                //catch (Exception ex)
                //{
                //    _log.FatalFormat("Unable to save product {0} because '{1}'", pimProduct.ID, ex.Message);
                //}
            }

            return productChanged;
        }

        #endregion Import Products

        #region Disable Products

        public void DisableProducts(List<PIMProduct> pimProducts, Dictionary<string, Product> currentProds)
        {
            var disabledProds = this.QueryDisabledPIMProductList(pimProducts);
            var productsToDisable = new List<Product>();

            foreach (var pimProd in disabledProds)
            {
                Product curProd;
                if (!currentProds.TryGetValue(pimProd.ID, out curProd))
                {
                    continue;
                }

                if (curProd.ProductStatusId != (int)ProductStatusTypeEnum.Abolished)
                {
                    curProd.ProductStatusId = (int)ProductStatusTypeEnum.Abolished;

                    // TODO:  Could this be slow?
                    using (Repository repo = new Repository())
                    {
                        if (repo.Entry(curProd).State == EntityState.Detached)
                        {
                            repo.Context.Products.Attach(curProd);
                            repo.Entry(curProd).State = EntityState.Modified;
                            repo.SaveChanges();
                        }
                    }
                }
            }
        }

        #endregion Disable Products

        #region Import Product Specifications

        private void ImportProductSpecifications(Repository repo, PIMProduct pimProduct, Product product,
            Dictionary<string, ProductSpecificationLabel> productSpecLabels)
        {
            ImportProductSpecificationsSingleValue(repo, pimProduct, product, productSpecLabels);
        }

        private void ImportProductSpecificationsSingleValue(Repository repo, PIMProduct pimProduct, Product product,
                Dictionary<string, ProductSpecificationLabel> productSpecLabels)
        {
            var singleValueSpecs = from spec in pimProduct.Specifications.Values
                                   where typeof(PIMProductSpecification).IsAssignableFrom(spec.GetType())
                                   select spec;

            // TODO:  Will this be slow?
            var currentSpecs = (from spec in repo.Context.ProductSpecifications.Include("ProductSpecificationLabel")
                                where spec.ProductId == product.ProductId
                                select spec).ToDictionary(k => k.ProductSpecificationLabel.Name);

            foreach (var pimProdSpecBase in singleValueSpecs)
            {
                var pimProdSpec = pimProdSpecBase as PIMProductSpecification;

                if (pimProdSpec == null
                    || String.IsNullOrWhiteSpace(pimProdSpec.AttributeID))
                {
                    continue;
                }

                if (!productSpecLabels.ContainsKey(pimProdSpec.AttributeID))
                {
                    _log.WarnFormat("Unable to load product {0} specification {1} as the label does not exist", pimProduct.ID, pimProdSpec.AttributeID);
                    continue;
                }

                var productSpecLabel = productSpecLabels[pimProdSpec.AttributeID];

                // Current specification
                if (currentSpecs.ContainsKey(pimProdSpec.AttributeID))
                {
                    var currentSpec = currentSpecs[pimProdSpec.AttributeID];

                    // TODO:  Figure out why we have to attach and mark modified.  Why is it not tracking correctly?
                    if (currentSpec.Value != pimProdSpec.Value)
                    {

                        repo.Entry(currentSpec).State = EntityState.Modified;
                        currentSpec.Value = pimProdSpec.Value;
                    }
                }
                else // New Specification
                {
                    ProductSpecification newSpec = new ProductSpecification()
                    {
                        ProductSpecificationLabelId = productSpecLabel.ProductSpecificationLabelId,
                        ProductId = product.ProductId,
                        Value = pimProdSpec.Value
                    };

                    repo.Context.ProductSpecifications.Add(newSpec);
                }
            }
        }

        private int DeleteUnusedSpecifications(Repository repo, PIMProduct pimProduct)
        {
            if (pimProduct.Specifications == null)
            {
                return 0;
            }

            var allSpecs = pimProduct.Specifications.Keys.ToList();

            int count = repo.Context.ProductSpecifications.Where(w =>
               w.Product.ProductNumber == pimProduct.ID
                   && !allSpecs.Contains(w.ProductSpecificationLabel.Name)).Delete();

            return count;
        }

        #endregion Import Products

        #region Import Product Notes

        /// <summary>
        /// Is there a way to increase performance and not break everything if one fails
        /// </summary>
        /// <param name="pimProducts"></param>
        public void ImportProductNotes(List<PIMProduct> pimProducts, Dictionary<string, Product> currentProducts)
        {

            // TODO:  Transactions?
            // TODO:  Convert to bulk insert now that it works...
            using (Repository repo = new Repository(null))
            {

                // Load mappings
                var maps = (from map in repo.Context.PIMAttributeMaps
                            where !map.Disabled && map.MapTypeId == PIMMapTypeEnum.Note
                            select map).ToDictionary(d => d.PIMAttributeId);

                // Load note types
                var noteTypes = (from nt in repo.Context.ProductNoteTypes
                                 select nt).ToDictionary(d => d.Description);

                // TODO:  Move the removal of products without notes to the LINQ query
                var importablePIMProducts = QueryPIMProductList(pimProducts);

                foreach (var pimProd in importablePIMProducts)
                {
                    Product curProd;

                    if (!currentProducts.TryGetValue(pimProd.ID, out curProd))
                    {
                        _log.ErrorFormat("Unable to find product {0} to insert product notes", pimProd.ID);
                        continue;
                    }

                    // Delete Notes
                    int deleted = repo.Context.ProductNotes.Where(p => p.ProductId == curProd.ProductId).Delete();

                    // Get the note specifications
                    var productNoteSpecs = from noteSpec in pimProd.Specifications.Values
                                           where maps.Keys.Contains(noteSpec.AttributeID)
                                           select noteSpec;

                    short noteRank = 1;
                    foreach (var noteSpec in productNoteSpecs)
                    {
                        var map = maps[noteSpec.AttributeID];

                        ProductNoteType nt;

                        // Try to get the note type
                        if (!noteTypes.TryGetValue(map.DaikinCityAttributeId, out nt))
                        {
                            var msg = String.Format("Unable to load product {0} {1} notes", pimProd.ID, map.DaikinCityAttributeId);
                            _log.Warn(msg);
                            Console.WriteLine(msg);

                            continue;
                        }

                        // Skip empty notes
                        if (noteSpec.Data == null)
                        {
                            continue;
                        }

                        // HACK:  This smells pretty bad
                        if (noteSpec is PIMProductMultiValueSpecification)
                        {
                            var multiNoteSpec = noteSpec as PIMProductMultiValueSpecification;

                            foreach (var note in multiNoteSpec.Value)
                            {
                                if (String.IsNullOrWhiteSpace(note))
                                {
                                    continue;
                                }

                                ProductNote pn = new ProductNote();
                                pn.ProductNoteId = repo.Context.GenerateNextGuid();
                                pn.ProductId = curProd.ProductId;
                                pn.ProductNoteTypeId = nt.ProductNoteTypeId;
                                pn.Description = note;
                                pn.Rank = noteRank;

                                repo.Context.ProductNotes.Add(pn);
                                noteRank++;
                            }
                        }
                        else
                        {
                            ProductNote pn = new ProductNote();
                            pn.ProductNoteId = repo.Context.GenerateNextGuid();
                            pn.ProductId = curProd.ProductId;
                            pn.ProductNoteTypeId = nt.ProductNoteTypeId;
                            pn.Description = noteSpec.Data.ToString();
                            pn.Rank = noteRank;

                            repo.Context.ProductNotes.Add(pn);

                            noteRank++;
                        }

                    }

                    repo.SaveChanges();
                }
            }
        }

        #endregion Import Product Notes

        #region Import Product Components

        public void ImportProductComponents(List<PIMProduct> pimProducts, Dictionary<string, Product> currentProducts)
        {
            if (pimProducts == null)
            {
                return;
            }

            var prodsWithRef = QueryPIMProductList(pimProducts).Where(w => w.ProductReferences.Count > 0);

            // TODO:  Convert to bulk insert
            foreach (var pimProd in prodsWithRef)
            {
                var components = from p in pimProd.ProductReferences
                                 where String.Compare(p.ReferenceType, "Components", true) == 0
                                 select p;

                if (!components.Any())
                {
                    continue;
                }

                Product parentProduct;
                if (!currentProducts.TryGetValue(pimProd.ID, out parentProduct))
                {
                    _log.WarnFormat("PIM Product {0} is not in the Daikin City database", pimProd.ID);
                    continue;
                }
                using (Repository repo = new Repository())
                {

                    // Clear out the components
                    int deletedComps = repo.Context.ProductAccessories.Where(w => w.ParentProductId == parentProduct.ProductId).Delete();

                    foreach (var comp in pimProd.ProductReferences)
                    {
                        Product componentProduct;
                        if (!currentProducts.TryGetValue(comp.ProductNumber, out componentProduct))
                        {
                            continue;
                        }

                        int compReqTypeId;
                        if (!Int32.TryParse(comp.ComponentRequirementTypeID, out compReqTypeId))
                        {
                            _log.WarnFormat("Unable to identify requirement type '{0}' for product '{1}' and component '{2}'",
                                comp.ComponentRequirementTypeID, parentProduct.ProductNumber, componentProduct.ProductNumber);

                            continue;
                        }

                        ProductAccessory pa = new ProductAccessory()
                        {
                            ProductId = componentProduct.ProductId,
                            ParentProductId = parentProduct.ProductId,
                            Quantity = comp.Quantity,
                            RequirementTypeId = compReqTypeId,
                            ModifiedOn = DateTime.Now
                        };

                        repo.Context.ProductAccessories.Add(pa);
                    }

                    try
                    {
                        repo.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        _log.WarnFormat("Error saving Product Component: " + pimProd.ID);
                        //Console.WriteLine(ex.Message);
                    }

                }
            }
        }

        #endregion Import Product Components

        #region Import Document Types

        public Dictionary<string, PIMAssetType> ImportDocumentTypes(Dictionary<string, PIMAsset> assets)
        {
            // TODO:  Get rid of product image hard-code
            // TODO:  Finish this
            var newDocTypes = (from a in assets.Values
                               where a.AssetType != null
                                    && String.Compare(a.AssetType.Id, "ProductImage", true) != 0
                               select a.AssetType).Distinct().ToDictionary(k => k.Id);

            return newDocTypes;
        }

        #endregion Import Documents Types

        #region Import Product Documents

        public void ImportProductDocuments(List<PIMProduct> productsForImport, Dictionary<string, Product> currentProducts)
        {
            var pimProds = QueryPIMProductList(productsForImport);

            // Get products with asset references
            var allPIMAssetRefs = pimProds.Where(w => w.AssetReferences.Any())
                .SelectMany(s => s.AssetReferences);

            // Get products without asset references
            var prodsWithNoAssetRefs = pimProds.Where(w => !w.AssetReferences.Any());

            // Get all distinct assets
            var allPIMAssets = allPIMAssetRefs
                .Where(w => w.Asset != null && w.Asset.AssetType != null)
                .Select(ar => ar.Asset)
                .GroupBy(gb => gb.ID, (key, group) => group.First());

            ImportProductDocumentsAddDocuments(allPIMAssets);
            ImportProductDocumentsLinkProducts(allPIMAssetRefs, currentProducts);
            ImportProductDocumentsUnlinkAllProducts(prodsWithNoAssetRefs);
        }

        private void ImportProductDocumentsAddDocuments(IEnumerable<PIMAsset> assets)
        {
            // Get current PIM Documents
            Dictionary<string, Document> currentDocs;
            Dictionary<string, DocumentType> documentTypes;
            DocumentType imageDocType;
            const string imageDocumentTypeName = "ProductImageLowRes";

            using (Repository repo = new Repository())
            {
                currentDocs = repo.Context.Documents.ToDictionary(k => k.PIMDocumentId);
                documentTypes = repo.Context.DocumentTypes.ToDictionary(k => k.Name);
                imageDocType = (from dt in repo.Context.DocumentTypes
                                where String.Compare(dt.Name, imageDocumentTypeName, true) == 0
                                select dt).FirstOrDefault();
            }

            // Go through and identify assets to be added or updated
            List<Document> docs = new List<Document>();
            List<Document> images = new List<Document>();

            foreach (var asset in assets)
            {
                DocumentType dt;

                if (String.Compare(asset.AssetType.Id, "ProductImage", true) == 0)
                {
                    // Override the extension because PIM converts them
                    asset.Extension = GetPIMPrimaryImageExtension();
                    dt = imageDocType;
                }
                else if (!documentTypes.TryGetValue(asset.AssetType.Id, out dt))
                {
                    _log.ErrorFormat("Unable to upload asset {0} because asset type {1} does not exist in Daikin City.", asset.ID, asset.AssetType.Id);
                    continue;
                }

                Document doc = CreateDocument(asset, dt, currentDocs);

                if (doc != null)
                {
                    if (imageDocType == dt)
                    {
                        images.Add(doc);
                    }
                    else
                    {
                        docs.Add(doc);
                    }
                }
            }

            ImportProductDocumentsTransferFiles(GetPIMDocumentDirectory(), docs);
            SaveDocuments(docs, currentDocs);

            ImportProductDocumentsTransferFiles(GetPIMPrimaryImageDirectory(), images);
            SaveDocuments(images, currentDocs);
        }

        private List<Document> ImportProductDocumentsTransferFiles(string remoteDir, List<Document> docs)
        {
            var documentDirectory = Utilities.GetDocumentDirectory();
            var goodDocs = new List<Document>();
            var badDocs = new List<Document>();


            var docsByDocType = docs.GroupBy(gb => gb.DocumentTypeId).Select(s => new
            {
                Key = s.Key,
                Group = s
            });

            foreach (var uploadableDocs in docsByDocType)
            {
                using (SftpClient sftp = CreatePIMSftp())
                {
                    sftp.Connect();
                    sftp.ChangeDirectory(remoteDir);

                    foreach (var doc in uploadableDocs.Group)
                    {
                        var fileName = doc.PIMDocumentId + "." + doc.FileExtension;
                        string message = String.Empty;

                        bool transferred = false;

                        // 1 try + num of retries
                        for (int i = 0; i <= GetNumRetries(); i++)
                        {
                            transferred = TransferDocument(sftp, doc, remoteDir, out message);

                            if (transferred)
                            {
                                break;
                            }

                            _log.LogFormat(Log4NetLevel.Warn, "PIM DOCUMENT FTP", "Attempt {0} failed for file '{1}'.", i + 1, fileName);
                        }

                        // TODO:  Output this at the end of integration
                        if (transferred)
                        {
                            goodDocs.Add(doc);
                        }
                        else
                        {
                            _log.Log(Log4NetLevel.Error, "PIM DOCUMENT FTP", message);
                            badDocs.Add(doc);
                        }

                    }
                }
            }

            return goodDocs;
        }

        private bool TransferDocument(SftpClient sftp, Document doc, string remoteDir, out string message)
        {
            message = String.Empty;
            var documentDirectory = Utilities.GetDocumentDirectory();
            var fileName = doc.PIMDocumentId + "." + doc.FileExtension;

            try
            {
                var localWithDocType = Path.Combine(documentDirectory, doc.DocumentType.Description);
                if (!Directory.Exists(localWithDocType))
                {
                    Directory.CreateDirectory(localWithDocType);
                }

                var localDirWithFile = Path.Combine(localWithDocType, fileName);
                using (FileStream fs = new FileStream(localDirWithFile, FileMode.OpenOrCreate))
                {
                    sftp.DownloadFile(fileName, fs);
                }

                SftpFile sftpFile = sftp.Get(fileName);
                FileInfo localFile = new FileInfo(localDirWithFile);

                if (sftpFile == null || localFile == null)
                {
                    message = "Unable to find file in SFTP or file on the local system";
                    return false;
                }

                if (sftpFile.Attributes.Size != localFile.Length)
                {
                    message = String.Format("File size from sftp {0} does not match local file size {1}", sftpFile.Attributes.Size, localFile.Length);

                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                var msg = String.Format("Unable to upload asset {0} from {1} because file {1}",
                        doc.PIMDocumentId, remoteDir, fileName, ex.Message);
                _log.Exception(ex, "PIM DOCUMENT FTP", msg);

                return false;
            }
        }

        private void ImportProductDocumentsLinkProducts(IEnumerable<PIMAssetReference> assetRefs, Dictionary<string, Product> currentProducts)
        {
            var prodAssetRefs = assetRefs.GroupBy(gb => gb.ProductId).Select(s => new
            {
                Key = s.Key,
                Group = s
            });

            Dictionary<string, Document> currentDocs;

            using (Repository repo = new Repository())
            {
                // TODO:  All these dictionaries... that's a lot of memory
                currentDocs = repo.Context.Documents.ToDictionary(k => k.PIMDocumentId);
            }

            foreach (var prodAssetRef in prodAssetRefs)
            {
                using (Repository repo = new Repository())
                {

                    int deleteCount = repo.Context.DocumentProductLinks
                        .Where(w => String.Compare(w.Product.ProductNumber, prodAssetRef.Key, true) == 0)
                        .Delete();

                    Product prod;
                    if (!currentProducts.TryGetValue(prodAssetRef.Key, out prod))
                    {
                        _log.LogFormat(Log4NetLevel.Warn, "PIM PRODUCT DOCUMENTS", "Unable to load document links for product {0}", prodAssetRef.Key);
                        continue;
                    }

                    // TODO:  Why does inheritance add both assetRefs from parent and child to the assetRefs?  
                    // Made distinct by group by
                    var distinctAssetRefs = prodAssetRef.Group.GroupBy(gb => gb.AssetId, (key, group) => group.First());

                    foreach (var assetRef in prodAssetRef.Group)
                    {
                        Document doc;
                        if (!currentDocs.TryGetValue(assetRef.AssetId, out doc))
                        {
                            _log.LogFormat(Log4NetLevel.Error, "PIM PRODUCT DOCUMENTS", "Unable to find document for product {0} document {1}", prodAssetRef.Key, assetRef.AssetId);
                            continue;
                        }

                        var dpl = new DocumentProductLink()
                        {
                            ProductId = prod.ProductId,
                            Rank = 1,
                            DocumentId = doc.DocumentId
                        };

                        repo.Context.DocumentProductLinks.Add(dpl);
                    }

                    // TODO:  Could this be slow?
                    repo.SaveChanges();
                }
            }
        }

        private void ImportProductDocumentsUnlinkAllProducts(IEnumerable<PIMProduct> prodsWithNoAssets)
        {
            if (prodsWithNoAssets == null)
            {
                return;
            }

            _log.LogFormat(Log4NetLevel.Info, "PIM PRODUCT DOCUMENT LINKS", "Removing product links for {0} products", prodsWithNoAssets.Count());
            if (prodsWithNoAssets.Any())
            {
                foreach (var prodWithNoAsset in prodsWithNoAssets)
                {
                    using (Repository repo = new Repository())
                    {

                        _log.LogFormat(Log4NetLevel.Info, "PIM PRODUCT DOCUMENT LINKS", "Removing product links for product {0}", prodWithNoAsset.ID);

                        int deleteCount = repo.Context.DocumentProductLinks
                          .Where(w => String.Compare(w.Product.ProductNumber, prodWithNoAsset.ID, true) == 0)
                          .Delete();

                        _log.LogFormat(Log4NetLevel.Info, "PIM PRODUCT DOCUMENT LINKS", "Removed {0} product links for product {1}", deleteCount, prodWithNoAsset.ID);
                    }
                }
            }
        }

        #endregion Import Product Documents

        #region Document and Image Helpers

        private Document CreateDocument(PIMAsset asset, DocumentType docType, Dictionary<string, Document> currentDocs)
        {
            if (asset.AssetType == null)
            {
                _log.ErrorFormat("Unable to upload asset {0} because no asset type is available", asset.ID);
                return null;
            }

            Document doc;

            if (currentDocs.TryGetValue(asset.ID, out doc))
            {
                // Only upload if asset upload date is newer than current PIM upload date
                if (asset.UploadDate <= doc.PIMUploadDate
                    && doc.FileName == asset.ID
                    && doc.Name == asset.Name)
                {
                    return null;
                }

                doc.FileName = asset.ID;
                doc.Name = asset.Name;
                doc.DocumentType = docType;
                doc.DocumentTypeId = docType.DocumentTypeId;
                doc.PIMUploadDate = asset.UploadDate;
                doc.FileExtension = asset.Extension;
            }
            else
            {
                doc = new Document()
                {
                    DocumentId = Guid.NewGuid(),
                    Name = asset.Name,
                    FileName = asset.ID,
                    DocumentTypeId = docType.DocumentTypeId,
                    DocumentType = docType,
                    PIMDocumentId = asset.ID,
                    PIMUploadDate = asset.UploadDate,
                    CreatedOn = DateTime.Now,
                    FileExtension = asset.Extension
                };

            }

            return doc;
        }

        private void SaveDocuments(List<Document> docs, Dictionary<string, Document> currentDocs)
        {
            foreach (var doc in docs)
            {
                // TODO:  Has to be a better way for this...
                Document currentDoc;
                // Does the doc already exist?
                bool newDoc = !currentDocs.TryGetValue(doc.PIMDocumentId, out currentDoc);

                using (Repository repo = new Repository())
                {
                    try
                    {
                        if (repo.Entry(doc).State == EntityState.Detached)
                        {
                            repo.Context.Documents.Attach(doc);
                        }

                        repo.Entry(doc).State = newDoc ? EntityState.Added : EntityState.Modified;
                        repo.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        var msg = String.Format("Unable to save document {0} because {1}", doc.FileName, ex.Message);
                        Console.WriteLine(msg);
                        _log.Error(msg);
                    }
                }

            }
        }

        #endregion Document and Image Helpers

        #region SFTP Methods

        public SftpClient CreatePIMSftp()
        {
            var hostName = Utilities.Config("dpo.sys.sftp.host");
            var username = Utilities.Config("dpo.sys.sftp.username");
            var password = Utilities.Config("dpo.sys.sftp.password");
            var portString = Utilities.Config("dpo.sys.sftp.port");

            int port;
            if (!Int32.TryParse(portString, out port))
            {
                port = 22; // Default SSH Port
            }

            return new SftpClient(hostName, port, username, password);
        }

        public string GetPIMDocumentDirectory()
        {
            return Utilities.Config("dpo.sys.sftp.documentfolder");
        }

        public string GetPIMPrimaryImageDirectory()
        {
            return Utilities.Config("dpo.sys.sftp.primaryimagefolder");
        }

        private string GetPIMPrimaryImageExtension()
        {
            return Utilities.Config("dpo.sys.sftp.primaryimageextension");
        }

        public string GetPIMExportDirectory()
        {
            return Utilities.Config("dpo.sys.sftp.daikincityfolder");
        }

        public string GetPIMExportFileName()
        {
            return Utilities.Config("dpo.sys.sftp.productexportfilename");
        }

        public string GetPIMZipFileName()
        {
            return Utilities.Config("dpo.sys.sftp.productexportzipfile");
        }

        /// <summary>
        /// Return the number of retries that are configured for SFTP transfer.
        /// Defaults to 0.
        /// </summary>
        /// <returns></returns>
        public int GetNumRetries()
        {
            int numRetries;

            if (!Int32.TryParse(Utilities.Config("dpo.sys.sftp.numretries"), out numRetries))
            {
                numRetries = 0;
            }

            return numRetries;
        }

        #endregion SFTP Methods

        public void RunDatabaseMaintenanceRoutines()
        {

            this.Db.ReadOnly = true;
            //Find accessories which have product components, as these are bits making up a systems
            //We can't just search for systems as some systems are classed as products 
            //(which can made up from other components)

            try
            {
                this.Context.spUpdateProductListPriceForSystems();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            /* Net price calculations and multipliers are not applied for assembled 
             * products when there is no product class code. 
             * For now we are to move the first product class code from a sub component to the parent if none exists.
             * Some indoor products are made of a number of other indoor products, these are not in the ERP system so they don't have a product family. 
             * So for now we use the sub component's product family. This is needed for multipliers and net price calculations.
             * In the future we might need to revisit this as this assumes all sub products share the same multiplier, 
             * which for now is the case.*/

            try
            {
                this.Context.spUpdateProductClassCodeForSystems();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            //Check if any quotes need to be alerted foe recalcuation.
            try
            {
                this.Context.spUpdateRecalculationRequiredForQuotes();
            }
            catch (Exception ex)
            {
                throw ex;
            }


            this.Context.SaveChanges();
        }

        private Type GetPropertyType(Type type, string propertyName)
        {
            var prop = type.GetProperty(propertyName);
            if (prop == null)
            {
                return null;
            }

            return prop.PropertyType;
        }
    }
}
