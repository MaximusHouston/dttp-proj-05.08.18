using System;
using System.Collections.Generic;
using System.Linq;
using DPO.Common;
using DPO.Data;
using System.Data.Entity;
using DPO.Domain.DaikinWebServices;
using System.IO;
using DPO.Data.Context;

namespace DPO.Domain
{
    public partial class DaikinServices : BaseServices
    {
        #region Import Documents data
        public void ImportDocumentData(DateTime? fromDate)
        {
            this.Db.ReadOnly = false;

            if (fromDate == null)
            {
                try { fromDate = this.Db.Context.Documents.Max(m => m.ModifiedOn); }
                catch { fromDate = null; }
            }

            if (fromDate < DateTime.Today.AddDays(-365)) fromDate = null;

            // load all documents 
            var import = this.GetDocuments(fromDate).Where(d => d.Id != null && d.TypeCode != null).ToList();

            if (import.Count == 0) return;

            var documentTypesLookup = Db.Context.DocumentTypes.ToDictionary(d => d.DocumentTypeId);

            // Add any document types
            foreach (var daikinDocument in import)
            {
                DocumentType type;

                if (documentTypesLookup.TryGetValue(daikinDocument.TypeCode.Value, out type))
                {
                    if (type.Description != daikinDocument.TypeCodeName) type.Description = daikinDocument.TypeCodeName;
                }
                else
                {
                    type = new DocumentType { DocumentTypeId = daikinDocument.TypeCode.Value, Description = daikinDocument.TypeCodeName };
                    Db.Context.DocumentTypes.Add(type);
                    documentTypesLookup.Add(daikinDocument.TypeCode.Value, type);
                }
            }

            Db.SaveChanges();

            this.Db.ReadOnly = true;

            // Add document NOTE** Never delete documents as they are used in Quote packages
            var documents = new List<DPO.Data.Document>();

            // Map imported products to DPO Product and Spec structure
            foreach (var daikinDocument in import)
            {
                var document = new DPO.Data.Document
                {
                    CreatedOn = daikinDocument.CreatedOn,
                    FileName = daikinDocument.Title ?? string.Empty,
                    DocumentId = daikinDocument.Id.Value,
                    DocumentTypeId = daikinDocument.TypeCode.Value,
                    Timestamp = daikinDocument.ModifiedOn,
                    ModifiedOn = daikinDocument.ModifiedOn,
                };

                documents.Add(document);
            }

            Db.UpdateDocuments(documents);

        }

        public void ImportDocumentFiles(DateTime? fromDate)
        {
            this.Db.ReadOnly = true;

            var documentTypes = this.Db.Context.DocumentTypes.ToList();

            var documents = this.Db.Context.Documents.ToList();

            var baseDirectory = Utilities.GetDocumentDirectory();

            foreach (var doc in documents)
            {
                var directory = baseDirectory + doc.DocumentType.Description + @"\";

                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

                try
                {
                    var file = Directory.GetFiles(directory, doc.FileName + ".*").FirstOrDefault();

                    if (file == null || Directory.GetLastWriteTime(file) < doc.ModifiedOn)
                    {
                        var literature = this.GetDocumentFile(doc.DocumentId);

                        if (literature != null && literature.Attachments.Length > 0)
                        {
                            var attachment = literature.Attachments.FirstOrDefault();

                            if (file == null)
                            {
                                file = directory + Path.ChangeExtension(doc.FileName, Path.GetExtension(attachment.FileName));
                            }

                            try
                            {
                                var data = Convert.FromBase64String(attachment.FileBytes64);

                                File.WriteAllBytes(file, data);

                                Directory.SetLastWriteTime(file, doc.ModifiedOn);

                                Console.WriteLine(string.Format("File saved {0} ({1} K).", Path.GetFileName(file), data.Length / 1204));
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(string.Format("Error pulling  : File {0}.", Path.GetFileName(file)));
                                Console.WriteLine(e.Message);
                            }

                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(string.Format("File name invalid  : File {0}.", doc.FileName));
                    Console.WriteLine(e.Message);
                }
            }
        }

        public void ImportProductDocumentData(DateTime? fromDate, Dictionary<string, DPO.Data.Product> currentProductsLookup)
        {
            _log.InfoFormat("Enter ImportProductDocumentData for fromDate: {0}", fromDate);

            this.Db.ReadOnly = true;

            if (fromDate == null)
            {
                try
                {
                    _log.Debug("fromDate is Null. Setting fromData equal to Latest Timestemp on DocumentProductLinks table");
                    fromDate = this.Db.Context.DocumentProductLinks.Max(m => m.Timestamp).AddHours(-1);
                    _log.DebugFormat("fromDate: {0}", fromDate);
                }
                catch { fromDate = null; }
            }

            if (fromDate < DateTime.Today.AddDays(-365))
            {
                _log.Debug("fromDate is one year older than current date. Setting fromDate to Null");
                fromDate = null;
            }

            fromDate = null; //TODO Charles need to fix service

            // load all documents 
            var import = this.GetProductDocuments(fromDate);

            if (import.Count == 0)
            {
                _log.Debug("No Product Documents return from CRM. Return to caller");
                return;
            }

            // Ignore documents without id
            var importedRecords = import.Where(d => d.ProductNumber != null && d.SalesLiteratureId.HasValue)
                                  .Select(i => new DocumentProductLink
                                  {
                                      DocumentId = i.SalesLiteratureId.Value,
                                      Rank = (short)i.Rank,
                                      ImportProductNumber = i.ProductNumber
                                  })
                                  .ToList();

            _log.Info("Start update DocumentProductLinks by ProductNumber");
            Db.UpdateDocumentProductLinksByProductNumber(importedRecords, currentProductsLookup);
        }
        #endregion

        #region Import Product data
        public void ImportProductData(DateTime? fromDate, Dictionary<string, DPO.Data.Product> currentProductsLookup)
        {
            _log.InfoFormat("Enter ImportProductData for fromDate: {0}", fromDate);

            this.Db.ReadOnly = true;

            if (fromDate == null)
            {
                try
                {
                    _log.Debug("fromDate is null. try Set fromDate equal to Maximum ModifiedOn date on Products table ");
                    fromDate = this.Db.Context.Products.Max(m => m.ModifiedOn);
                }
                catch(Exception ex)
                {
                    fromDate = null;
                    Log.Debug("can not set fromdate. default to null");
                }
            }

            if (fromDate < DateTime.Today.AddDays(-365))
            {
                fromDate = null;
                _log.Debug("fromDate is one year older than current date. Set fromDate to null");
            }

            // load all products 
            var import = this.GetProducts(fromDate);

            if (import.Count == 0)
            {
                _log.DebugFormat("No product have been loaded from CRM for fromDate: {0}. Return to caller", fromDate);
                return;
            }

            // Import any new and get all specification labels
            _log.Debug("Get ProductSpecificationLabels");
            var specficationLabels = ProcessProductSpecificationLabels();

            _log.Debug("Get ProductSpecificationLabelLookup");
            var specificationLabelLookup = Db.Context.ProductSpecificationLabels.Local.ToDictionary(d => d.Name);

            // Ignore products withj blank product numbers
            var daikinProducts = import.Where(p => !string.IsNullOrWhiteSpace(p.ProductNumber)).ToList();

            // Add/update products
            var products = new List<DPO.Data.Product>();

            // Map imported products to DPO Product and Spec structure
            foreach (var daikinProduct in daikinProducts)
            {
                var product = new DPO.Data.Product
                {
                    BrandId = daikinProduct.Brand ?? 1,
                    ProductCategoryId = daikinProduct.UnitCategory ?? 1,
                    ProductClassCode = daikinProduct.ProductClassCode ?? string.Empty,
                    ProductFamilyId = daikinProduct.Family ?? 1,
                    ProductNumber = daikinProduct.ProductNumber ?? string.Empty,
                    Name = daikinProduct.Name ?? string.Empty,
                    ModifiedOn = daikinProduct.ModifiedOn,

                    EERNonDucted = daikinProduct.EERNonducted,
                    IEERNonDucted = daikinProduct.IEERNonducted,
                    SEERNonDucted = daikinProduct.SEERNonducted,
                    COP47NonDucted = daikinProduct.COP47Nonducted,
                    HSPFNonDucted = daikinProduct.HSPFNonducted,
                    CoolingCapacityRated = daikinProduct.CoolingCapacityRated,
                    HeatingCapacityRated = daikinProduct.HeatingCapacityRated,
                    ProductPowerVoltageTypeId = daikinProduct.PowerVoltage,

                    ProductSpecifications = new List<ProductSpecification>(),
                    ProductModelTypeId = daikinProduct.ModelType ?? 1,
                    ProductMarketTypeId = daikinProduct.MarketType ?? 1,
                    ProductStatusId = daikinProduct.ReleaseStatus ?? (int)ProductStatusTypeEnum.Other,
                    SubmittalSheetTypeId = daikinProduct.SubmittalDatasheetTemplate ?? (int)SubmittalSheetTypeEnum.Other,
                    AllowCommissionScheme = (daikinProduct.AllowExternalCommission.HasValue && daikinProduct.AllowExternalCommission.Value == true),
                    UnitInstallationTypeId = daikinProduct.UnitInstallationType ?? 1,
                    ProductCompressorStageId = daikinProduct.CompressorType,
                    ProductMotorSpeedTypeId = daikinProduct.MotorType,
                    ProductGasValveTypeId = daikinProduct.GasValveType,
                    ProductInstallationConfigurationTypeId = daikinProduct.InstallationConfiguration,
                    ProductHeatExchangerTypeId = daikinProduct.HeatExchangerType,

                    CoolingCapacityNominal = daikinProduct.CoolingCapacityNominal.HasValue ? daikinProduct.CoolingCapacityNominal.Value : 0,
                    AirFlowRateHighCooling = daikinProduct.AirFlowRateHigh.HasValue ? daikinProduct.AirFlowRateHigh.Value : 0

                };

                // Fill Product specifications
                foreach (var prop in specficationLabels)
                {
                    var value = (prop.GetValue.Invoke(daikinProduct) + string.Empty).Trim();

                    // Ignore empty properties, if changed to empty they will be removed
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        product.ProductSpecifications.Add(new ProductSpecification
                        {
                            ProductSpecificationLabelId = specificationLabelLookup[prop.PropertyName].ProductSpecificationLabelId,
                            Value = value
                        });
                    }
                }

                products.Add(product);
            }

            _log.Debug("Start UpdateProductAndProductSpecifications for all products");
            Db.UpdateProductAndProductSpecifications(products, currentProductsLookup);
            _log.Info("Finished execute ImportProductData()");
        }

        public void ImportProductPrices(DateTime? fromDate)
        {
            _log.InfoFormat("Enter ImportProductPrices() for fromDate: {0}", fromDate);

            this.Db.ReadOnly = true;

            if (fromDate == null)
            {
                try
                {
                    _log.Debug("fromDate is Null.Setting fromDate equal to Latest ModifiedOn Date on Products table");
                    fromDate = this.Db.Context.Products.Max(m => m.ModifiedOn);
                    _log.DebugFormat("fromDate: {0}", fromDate);
                }
                catch (Exception ex)
                {
                    _log.Debug("Setting fromDate equal to Latest ModifiedOn Date on Products table Failed.Set fromdate to Null");
                    fromDate = null;
                }
            }

            if (fromDate < DateTime.Today.AddDays(-365))
            {
                Log.Debug("fromDate is one year older than Current Date.Set fromDate to Null");
                fromDate = null;
            }

            // load all products 
            var import = this.GetProducts(fromDate);

            if (import.Count == 0) return;

            // Ignore products with blank product numbers
            var daikinProducts = import.Where(p => !string.IsNullOrWhiteSpace(p.ProductNumber)).ToList();

            // Add/update products
            var products = new List<DPO.Data.Product>();

            // Map imported products to DPO Product and Spec structure
            foreach (var daikinProduct in daikinProducts)
            {
                var product = new DPO.Data.Product
                {
                    ListPrice = daikinProduct.Price ?? 0,
                    ProductNumber = daikinProduct.ProductNumber ?? string.Empty
                };

                products.Add(product);
            }

            _log.Info("Start Update Product Prices");
            Db.UpdateProductPrices(products);
            _log.Info("Finished ImportProductPrices()");
        }

        public void ImportProductNoteData(DateTime? fromDate, Dictionary<string, DPO.Data.Product> currentProductsLookup)
        {
            _log.InfoFormat("Enter importProductNoteData for fromDate: {0}", fromDate);
            this.Db.ReadOnly = true;

            if (fromDate == null)
            {
                try
                {
                    _log.Debug("fromDate is Null.Set fromDate equal to Latest ModifiedOn date on ProductNotes table");
                    fromDate = this.Db.Context.ProductNotes.Max(m => m.ModifiedOn);
                    _log.DebugFormat("fromDate: {0}", fromDate);
                }
                catch(Exception ex)
                {
                    _log.Debug("Setting fromDate equal to Latest ModifiedOn date on ProductNotes table failed. Set fromDate equal to Null");
                    fromDate = null;
                }
            }

            if (fromDate < DateTime.Today.AddDays(-365))
            {
                _log.Debug("fromDate is one year older than current date. Set fromDate to Null");
                fromDate = null;
            }

            // load all product notes 
            var import = this.GetProductNotes(fromDate).Where(d => d.Id != null && d.Type.HasValue).ToList();

            if (import.Count == 0) return;

            var current = Db.Context.ProductNotes.ToArray().ToDictionary(d => d.ProductNoteId);

            var productNumbers = import.Select(p => p.ProductNumber).Distinct().ToArray();

            // var products = Db.Context.Products.Where(p => productNumbers.Contains(p.ProductNumber)).ToArray().ToDictionary(p => p.ProductNumber);

            // Add any document types
            var addedRecords = new List<Data.ProductNote>();

            Db.ReadOnly = false;

            foreach (var irecord in import)
            {
                DPO.Data.Product product;

                if (currentProductsLookup.TryGetValue(irecord.ProductNumber, out product))
                {
                    DPO.Data.ProductNote note;

                    if (current.TryGetValue(irecord.Id.Value, out note))
                    {
                        if (note.Description != irecord.Description) note.Description = irecord.Description ?? string.Empty;
                        if (note.ProductNoteTypeId != irecord.Type.Value) note.ProductNoteTypeId = (int)irecord.Type.Value;
                        if (note.Rank != irecord.Rank) note.Rank = (short)(irecord.Rank ?? 0);
                        if (note.ModifiedOn != irecord.ModifiedOn) note.ModifiedOn = irecord.ModifiedOn;
                        if (note.ProductId != product.ProductId) note.ProductId = product.ProductId;
                    }
                    else
                    {
                        note = new DPO.Data.ProductNote
                        {
                            ProductNoteId = irecord.Id.Value,
                            Description = irecord.Description,
                            ProductNoteTypeId = irecord.Type.Value,
                            ModifiedOn = irecord.ModifiedOn,
                            Rank = (short)(irecord.Rank ?? 0),
                            ProductId = product.ProductId
                        };

                        addedRecords.Add(note);
                        current.Add(irecord.Id.Value, note);
                    }
                }
            }

            Db.SaveChanges();

            Db.ReadOnly = true;

            Db.Context.ProductNotes.AddRange(addedRecords);

            Db.SaveChanges();
        }
        #endregion

        #region Import Accessory data
        public void ImportProductAccessoriesData(DateTime? fromDate, Dictionary<string, DPO.Data.Product> currentProductsLookup)
        {
            _log.Info("Enter ImportProductAccessoriesData()");
            this.Db.ReadOnly = true;

            if (fromDate == null)
            {
                try
                {
                    _log.Debug("fromDate is null.Setting fromDate equal to the Latest ModifiedOn date on the ProductAccessories table");
                    fromDate = this.Db.Context.ProductAccessories.Max(m => m.ModifiedOn);
                    _log.DebugFormat("fromDate: {0}", fromDate);
                }
                catch(Exception ex)
                {
                    fromDate = null;
                    _log.DebugFormat("fromDate: {0}", fromDate);
                }
            }

            if (fromDate < DateTime.Today.AddDays(-365))
            {
                _log.Debug("fromDate is one year older than current date.Setting fromDate equal to Null");
                fromDate = null;
            }

            // load all product accessories
            var import = this.GetProductAccessories(fromDate);

            if (import.Count == 0) return;

            // Ignore products withj blank product numbers
            // TODO: change daikinAccessories -> systemComponents
            var daikinAccessories = import.Where(p => !string.IsNullOrWhiteSpace(p.ParentProductNumber)
                                                      && !string.IsNullOrWhiteSpace(p.ProductNumber)
                                                      && p.Quantity != null
                                                      && p.RequirementLevel != null).ToList();

            // Add/update products
            var accessories = new List<DPO.Data.ProductAccessory>();

            var data = new Repository();

            // Map imported products to DPO Product and Spec structure
            // TODO: Log notImportedList
            var notImportedList = new List<SystemComponent>();

            foreach (var daikinAccessory in daikinAccessories)
            {
                if (currentProductsLookup.ContainsKey(daikinAccessory.ParentProductNumber) && currentProductsLookup.ContainsKey(daikinAccessory.ProductNumber))
                {
                    var accessory = new DPO.Data.ProductAccessory
                    {
                        ParentProductId = currentProductsLookup[daikinAccessory.ParentProductNumber].ProductId,
                        ProductId = currentProductsLookup[daikinAccessory.ProductNumber].ProductId,
                        Quantity = daikinAccessory.Quantity.Value,
                        RequirementTypeId = daikinAccessory.RequirementLevel.Value,
                        ModifiedOn = daikinAccessory.ModifiedOn,
                    };

                    accessories.Add(accessory);
                }
                else
                {
                    notImportedList.Add(daikinAccessory);
                }
            }

            _log.Info("Start Update Product Accessoies");
            Db.UpdateProductAccessories(accessories);
            _log.Info("Finished Update Product Accessoies");
        }
        #endregion

        #region Import UnitInstallationType Data
        public void ImportUnitInstallationTypeData(DateTime? fromDate)
        {
            _log.InfoFormat("Start ImportUniInstallationTypeData from Date: {0}", fromDate);
            this.Db.ReadOnly = true;

            if (fromDate == null)
            {
                try
                {
                    _log.Debug("fromDate is Null.Set fromDate equal to Latest ModifiedOn date on UnitInstallationType table");
                    fromDate = this.Db.Context.UnitInstallationTypes.Max(m => m.Timestamp).AddHours(-1);
                    _log.DebugFormat("fromDate: {0}", fromDate);
                }
                catch(Exception ex)
                {
                    _log.Debug("Setting fromDate equal to Latest ModifiedOn date on UnitInstallationType table failed. Set fromDate equal to Null");
                    fromDate = null;
                }
            }

            if (fromDate < DateTime.Today.AddDays(-365))
            {
                _log.Debug("fromDate is one year older than current date. Set fromDate to Null");
                fromDate = null;
            }

            // load all products 
            var import = this.GetProducts(fromDate);

            if (import.Count == 0) return;

            // Ignore products withj blank product numbers
            var daikinProducts = this.Context.Products.Where(p => p.ProductNumber != null).ToList();

            int unitInstallationTypeId = this.Context.ProductSpecificationLabels.Where(psl => psl.Name == "UnitInstallationType")
                                         .Select(psl => psl.ProductSpecificationLabelId)
                                         .FirstOrDefault();

            var ImportUnitInstallationTypes = (from p in this.Context.Products
                                               join ps in this.Context.ProductSpecifications
                                               on p.ProductId equals ps.ProductId
                                               join pskl in this.Context.ProductSpecificationKeyLookups
                                               on ps.ProductSpecificationLabelId equals pskl.ProductSpecificationLabelId
                                               where ps.ProductSpecificationLabelId == unitInstallationTypeId
                                               select new
                                               {
                                                   Id = ps.ProductSpecificationLabelId,
                                                   Key = pskl.Key,
                                                   Value = pskl.Value,
                                               }).ToList().GroupBy(i => i.Value).Select(y => y.FirstOrDefault());

            var unitInstallationTypeAddList = new List<UnitInstallationType>();
            var unitInstallationTypeUpdateList = new List<UnitInstallationType>();

            var currentUnitInstallationTypes = (from p in this.Db.Context.UnitInstallationTypes
                                                select p
                                               ).ToList();

            //foreach (var itemImport in ImportUnitInstallationTypes)
            //{
            //    UnitInstallationType Item;

            //    if (!currentUnitInstallationTypes.Any(i => i.UnitInstallationTypeId == itemImport.Key))
            //    {
            //        Item = new UnitInstallationType();
            //        Item.UnitInstallationTypeId = itemImport.Key;
            //        Item.Name = itemImport.Value;
            //        unitInstallationTypeAddList.Add(Item);
            //    }
            //    else
            //    {
            //        Item = currentUnitInstallationTypes.Where(i => i.UnitInstallationTypeId == itemImport.Key).FirstOrDefault();
            //        if (Item.Name != itemImport.Value)
            //        {
            //            Item.Name = itemImport.Value;
            //        }
            //        unitInstallationTypeUpdateList.Add(Item);
            //    }
            //}

            if (unitInstallationTypeUpdateList.Count() > 0)
            {
                _log.Info("Start updateUnitinstallationType to database");
                Db.UpdateUnitInstallationType(unitInstallationTypeUpdateList);
            }
            if (unitInstallationTypeAddList.Count() > 0)
            {
                _log.Info("Start adding UnitInstallationType to database");
                Db.AddUnitInstallationType(unitInstallationTypeAddList);
            }

            _log.Info("Finished ImportUnitInstallationTypeData");
        }
        #endregion

        #region Import Account Multipliers data
        public void ImportAccountMultipliers(DateTime? fromDate)
        {
            _log.InfoFormat("Start ImportAccountMultipliers from Date: {0}", fromDate);
            this.Db.ReadOnly = true;

            if (fromDate == null)
            {
                try { fromDate = this.Db.Context.AccountMultipliers.Max(m => m.Timestamp).AddHours(-1); } // Use timestamp }
                catch { fromDate = null; }
            }

            if (fromDate < DateTime.Today.AddDays(-365)) fromDate = null;

            var import = this.GetAccountMultipliers(fromDate).Where(d => d.Id != null && d.CrmAccountNumber != null && d.CrmAccountNumber != "" && d.DiscountPercentage.HasValue)
                                      .ToList();

            if (import.Count == 0) return;

            // Get all current products which match imported records
            var currentsLookUp = Db.Context.AccountMultipliers.Distinct().ToList();

            var duplicateRecords = from ac in currentsLookUp
                                   group ac by new { ac.AccountMultiplierId } into grac
                                   where grac.Count() > 1
                                   select grac.Key;

            //currentsLookUp.RemoveAll(ac => duplicateRecords.Any(dac => dac.AccountMultiplierId == ac.AccountMultiplierId));

            var temp = (from am in currentsLookUp
                        join bs in Db.Businesses on am.BusinessId equals bs.BusinessId
                        where bs.AccountId != null
                        select new { bs.AccountId, am })
                          .ToList();

            var currents = new Dictionary<string, DPO.Data.AccountMultiplier>();

            var duplicates = new List<string>();
            int i = 0;

            foreach (var item in temp)
            {
                var key = item.AccountId + item.am.ProductClassCode;
                if (i <= 0)
                {
                    currents.Add(key, item.am);
                    i++;
                }
                else
                {
                    var found = currents.Any(dup => dup.Key.Contains(key));
                    if (!found)
                    {
                        currents.Add(key, item.am);
                    }
                }
            }

            var businessLookup = this.Db.Businesses.Where(b => b.AccountId != null && b.AccountId != "")
                                    .ToArray().ToDictionary(b => b.AccountId);

            // Add any document types
            var addedRecords = new List<Data.AccountMultiplier>();
            Db.ReadOnly = false;
            foreach (var irecord in import)
            {
                DPO.Data.AccountMultiplier multiplier;

                var key = irecord.CrmAccountNumber + irecord.ItemClassCode;

                if (currents.TryGetValue(key, out multiplier))
                {
                    decimal percentage = irecord.DiscountPercentage.Value;

                    if (multiplier.Multiplier != percentage)
                    {
                        multiplier.Multiplier = percentage;
                        multiplier.MultiplierTypeId = this.Db.Context.MultiplierTypes
                                                          .Where(m => m.Name == irecord.ItemClassCode)
                                                          .Select(m => m.MultiplierTypeId).FirstOrDefault();
                        multiplier.Timestamp = irecord.ModifiedOn;
                    }
                }
                else
                {
                    Business business;
                    if (businessLookup.TryGetValue(irecord.CrmAccountNumber, out business))
                    {
                        multiplier = new DPO.Data.AccountMultiplier
                        {
                            AccountMultiplierId = Db.Context.GenerateNextLongId(),
                            BusinessId = business.BusinessId,
                            Multiplier = irecord.DiscountPercentage.Value,
                            ProductClassCode = irecord.ItemClassCode,
                            MultiplierTypeId = this.Db.Context.MultiplierTypes
                                                   .Where(m => m.Name == irecord.ItemClassCode)
                                                   .Select(m => m.MultiplierTypeId).FirstOrDefault(),
                            Timestamp = irecord.ModifiedOn
                        };

                        addedRecords.Add(multiplier);
                        currents.Add(key, multiplier);
                    }
                }
            }

            Db.Context.IgnoreTimestampChecking = true;
            try
            {
                Db.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.FatalFormat("adding AccountMultiplier to database Failed");
                _log.FatalFormat("Excetion Details: {0}", (ex.InnerException != null) ? ex.InnerException.InnerException.Message : ex.Message);
            }

            Db.Context.IgnoreTimestampChecking = false;
            Db.ReadOnly = true;

            Db.Context.AccountMultipliers.AddRange(addedRecords);

            try
            {
                Db.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.FatalFormat("Update database Failed");
                _log.FatalFormat("Excetion Details: {0}", (ex.InnerException != null) ? ex.InnerException.InnerException.Message : ex.Message);
            }
        }
        #endregion

        #region Import Group data
        //TODO from date functionality
        public void ImportGroupData(DateTime? fromDate)
        {
            _log.InfoFormat("Start import GroupData from Date: {0}", fromDate);
            this.Db.ReadOnly = false;

            // load all groups making sure groups in correct order for adding db records.
            // i.e parent records must exist before child records
            var import = this.GetGroups(fromDate).Where(g => g.Id != null && g.IsDisabled == false).ToList();

            if (import.Count == 0) return;

            // Get all db groups
            var dpoGroupsByDaikinId = Db.Groups.Where(g => g.DaikinGroupId != null).ToArray().ToDictionary(g => g.DaikinGroupId);

            var topLevelGroupId = long.Parse(Utilities.Config("dpo.setup.toplevel.groupid"));
            var topLevelGroup = Db.Groups.Where(g => g.GroupId == topLevelGroupId).FirstOrDefault();
            if (topLevelGroup == null) throw new Exception(Resources.DaikinWebServicesMessages.DWS02);

            var daikinTopLevelGroupName = Utilities.Config("dpo.webservices.toplevel.groupname");

            // Create any new groups
            foreach (var daikinGroup in import)
            {
                DPO.Data.Group dpoGroup = null;

                if (dpoGroupsByDaikinId.TryGetValue(daikinGroup.Id, out dpoGroup))
                {
                    // Dont change top level group name during import
                    if (topLevelGroup.DaikinGroupId != daikinGroup.Id)
                    {
                        if (string.Compare(dpoGroup.Name, daikinGroup.Name) != 0)
                        {
                            dpoGroup.Name = daikinGroup.Name;
                        }
                    }
                }
                else
                {
                    // Map top level group to exisiting top level group
                    if (string.Compare(daikinGroup.Name, daikinTopLevelGroupName, true) == 0)
                    {
                        topLevelGroup.DaikinGroupId = daikinGroup.Id;
                    }
                    else
                    {
                        dpoGroup = Db.GroupCreate(daikinGroup.Name, null);
                        dpoGroup.DaikinGroupId = daikinGroup.Id;
                    }
                }
            }

            // Map parent group ids;
            var groups = Db.Context.Groups.Local.Where(g => g.DaikinGroupId != null).ToDictionary(g => g.DaikinGroupId);
            foreach (var daikinGroup in import)
            {
                var relatedGroup = groups[daikinGroup.Id];

                if (daikinGroup.ParentGroupId == null)
                {
                    if (relatedGroup.GroupId != topLevelGroupId)
                    {
                        relatedGroup.ParentGroupId = topLevelGroupId;
                    }
                }
                else
                {
                    DPO.Data.Group parentGroup;
                    if (groups.TryGetValue(daikinGroup.ParentGroupId, out parentGroup))
                    {
                        relatedGroup.ParentGroupId = parentGroup.GroupId;
                    }
                    else
                    {
                        relatedGroup.ParentGroupId = topLevelGroupId;
                    }
                }
            }

            _log.Debug("Start Update Group Information");

            Db.UpdateGroupInformation();

            try
            {
                Db.SaveChanges();
            }
            catch (Exception ex)
            {
                _log.Fatal("Update change to database failed.");
                _log.FatalFormat("Exception Details: {0}", (ex.InnerException != null) ? ex.InnerException.Message : ex.Message);
            }

            _log.Info("Finsished ImportGroupData");
        }
        #endregion

        #region Import Business data
        //TODO from date functionality

        public State GetState(Dictionary<string, State> states, string country, string stateCodeOrName)
        {
            if (String.IsNullOrWhiteSpace(stateCodeOrName))
            {
                return null;
            }

            State state;

            // Default to US
            if (String.IsNullOrWhiteSpace(country))
            {
                country = "US";
            }
            else if (string.Compare(country, "USA", true) == 0)
            {
                country = "US";
            }
            else if (string.Compare(country, "CAN", true) == 0)
            {
                country = "CA";
            }
            else if (country.Length > 2)
            {
                country = country.Substring(0, 2).ToUpper();
            }

            // Cleanup the name if it is longer than 2 characters
            if (stateCodeOrName.Length > 2)
            {
                stateCodeOrName = stateCodeOrName.Substring(0, 2);
            }

            //TODO Daikin should provide country
            states.TryGetValue(country + ":" + stateCodeOrName, out state);

            return state;
        }

        public void ImportBusinessData(DateTime? fromDate)
        {
            _log.InfoFormat("Start ImportBusinessData from Date: {0}", fromDate);
            this.Db.ReadOnly = false;

            if (fromDate == null)
            {
                try { fromDate = this.Db.Context.Businesses.Max(m => m.DaikinModifiedOn); }
                catch (Exception ex)
                { fromDate = null; }
            }

            if (fromDate < DateTime.Today.AddDays(-365)) fromDate = null;

            // load all groups making sure groups in correct order for adding db records.
            // i.e parent records must exist before child records
            var import = this.GetAccounts(fromDate).ToList();

            if (import.Count == 0) return;

            /// Anand : rewrite of CRM import ///
            ///* NEW CHANGES *///
            var businesquery = Db.Businesses.Include(b => b.Address).Include(b => b.Contact);  

            //contains account that doesn't exist in DC - as of this point might be brand new or could be same business with different account Id
            var newRecordList = import.Where(x => !businesquery.Select(y => y.AccountId).Contains(x.CRMAccountNumber)).ToList();  

            //contains account that has same account Id
            var similarRecordList = import.Where(x => businesquery.Select(y => y.AccountId).Contains(x.CRMAccountNumber)).ToList(); //list of all existing records

            //filter more to get the absolute new record (no similar account id's or names - case sensitive)
            var newAccounts= newRecordList.Where(x => !businesquery.Where(y=>y.BusinessName != null)
                                        .Select(y => y.BusinessName.Trim().ToUpper())
                                        .Contains(x.Name.ToUpperTrim())).ToList(); //filter it further that there is no such business name as well..

            //account Ids don't exist in DC but similar business name does.
            var newAcctsButSameBusinessNames = newRecordList.Where(x => businesquery.Where(y => y.BusinessName != null)
                                        .Select(y => y.BusinessName.Trim().ToUpper())
                                        .Contains(x.Name.ToUpperTrim())).ToList();

            using (var crmAccountImport = new CRMAccountImport(Context))
            {
                if (newAccounts != null && newAccounts.Count() > 0)
                    crmAccountImport.InsertNewRecordsIntoDC(newAccounts);

                if (newAcctsButSameBusinessNames != null && newAcctsButSameBusinessNames.Count() > 0)
                    crmAccountImport.NotifyFailureToImport(newAcctsButSameBusinessNames, businesquery);

                if (similarRecordList != null && similarRecordList.Count() > 0)
                    crmAccountImport.UpdateExistingRecordsInDC(similarRecordList, businesquery);
            }
        }

        public bool IsDaikinAccount(string accountId)
        {
            var daikinAccount = this.GetAccountId(accountId);
            return (daikinAccount != null);
        }
        #endregion

        #region old logic -- remove after 6 months - Anand 03.14.2018
        // var filteredNewRecordList = newRecordList.Where(x => !businesquery.Select(y => y.BusinessName.Trim().ToUpper()).Contains(x.Name.Trim().ToUpper()))?.ToList();

        //    using (var service = new BusinessServices())
        //    {
        //        var updateCount = 0;

        //        //Loop through businesses and update or add new ones
        //        foreach (var daikinBusiness in import)
        //        {
        //            var accountIdList = new List<string>();

        //            //Business dpoBusiness;
        //            businesses.TryGetValue(daikinBusiness.CRMAccountNumber, out dpoBusiness);

        //            // First try business State/Country
        //            // TODO:  CRM is passing in the ID which is not correct so switched to name
        //            var state = GetState(states, daikinBusiness.Address1Country, daikinBusiness.Address1StateOrProvinceName);
        //            if (state == null)
        //            {
        //                // No business state, check ERP/Accounting State/Country
        //                state = GetState(states, daikinBusiness.Address2Country, daikinBusiness.Address2StateOrProvince);
        //                _log.InfoFormat("Unable to find state for account: {0}", daikinBusiness.CRMAccountNumber);
        //            }

        //            //import, compare and update
        //            dpoBusiness = ImportDaikinBusinessRecord(daikinBusiness, dpoBusiness, state);

        //            try
        //            {
        //                Db.SaveChanges();
        //                updateCount++;
        //            }
        //            catch (Exception ex)
        //            {
        //                System.Diagnostics.Trace.TraceError(ex.InnerException?.Message);
        //                if (System.DateTime.Now.Hour >= 5 && System.DateTime.Now.Hour <= 6)
        //                {
        //                    var errorMessage = "Unable to import CRM Business Account '" + daikinBusiness.CRMAccountNumber + "'";
        //                    var key = "dpo.dev.team.email";
        //                    var subject = "Daikin Import Errors - Account Import";

        //                    WebImportError.NotifyErrorViaEmail(errorMessage, this.GetType().Name, key, subject);
        //                    _log.Fatal("saving Business into database failed");
        //                    _log.FatalFormat("Error Details: {0}", ex.InnerException?.Message ?? ex.Message);
        //                }

        //                // Remove errored record.  This could be done better with transactions
        //                Db.Context.Entry(dpoBusiness).State = EntityState.Detached;
        //                Console.WriteLine(ex.InnerException?.Message);
        //            }
        //        }

        //        Console.WriteLine("Updated " + updateCount + " Accounts");
        //    }
        //}


        //public Business UpdateBusinessDataFromDaikin(string accountId)
        //{
        //    //_log.Info("Start UpdateBusinessDataFromDaikin");

        //    this.Db.ReadOnly = false;
        //    var daikinAccount = this.GetAccountId(accountId);
        //    if (daikinAccount == null) return null;

        //    // Get db business if exists
        //    var dpoBusiness = Db.Businesses.Include(b => b.Address).Include(b => b.Contact)
        //                        .Where(b => b.AccountId == accountId).FirstOrDefault();

        //    //TODO Daikin should provide country
        //    var state = Db.States.FirstOrDefault(s => s.CountryCode != "GB" && s.Code == daikinAccount.Address2StateOrProvince);

        //    //_log.InfoFormat("Start ImportDaikinBusinessRecord. DaikinAccount: {0} , State: {1}", daikinAccount.AccountNumber, state);
        //    ImportDaikinBusinessRecord(daikinAccount, dpoBusiness, state);

        //    //_log.Info("Finished UpdateBusinessDataFromDaikin");
        //    return dpoBusiness;
        //}

        //private Business ImportDaikinBusinessRecord(DPO.Domain.DaikinWebServices.Account daikinAccount, Business dpoBusiness, State state)
        //{
        //    //_log.Info("Enter ImportDaikinBusinessRecord()");
        //    businessService.Response.Messages.Clear();

        //    if (daikinAccount == null) return null; //_log.Debug("daikinAccount is Null. Return to caller");
        //    if (dpoBusiness == null)
        //    {
        //        var type = BusinessTypeEnum.Other;

        //        if (daikinAccount.AccountCategoryCode != null)
        //            type = (BusinessTypeEnum)daikinAccount.AccountCategoryCode;
        //        else
        //            Console.WriteLine("Business type for {0} is null, defaulting to 'Unknown'", daikinAccount.Name);

        //        dpoBusiness = Db.BusinessCreate(type);
        //        dpoBusiness.AccountId = daikinAccount.CRMAccountNumber;
        //        dpoBusiness.ERPAccountId = daikinAccount.BillingAccountNumber;
        //    }

        //    //compares the values to determine the rows that needs update
        //    CompareCRMandDaikinCityValues(daikinAccount, dpoBusiness, state);

        //    //Apply business rules for add edit remove
        //    businessService.ApplyBusinessRules(daikinSuperUser, dpoBusiness);

        //    if (businessService.Response != null && !businessService.Response.IsOK)
        //        LogAndNotifyErrors(daikinAccount, dpoBusiness);

        //    return dpoBusiness;
        //}
        #endregion

        #region Import Orders and Invoices
        public void ImportOrderStatuses()
        {
            //get the orders
            Console.WriteLine("Start Orders import from Mapics for date and time: {0}, {1}",
                DateTime.Now.Date, DateTime.Now.TimeOfDay);

            this.Db.ReadOnly = false;

            ProcessOrderStatusImport();
        }

        public void ImportOrdersByDateTime(string datetime)
        {
            //get the orders
            Console.WriteLine("Start Orders import from Mapics for date and time: {0}, {1}",
                DateTime.Now.Date, DateTime.Now.TimeOfDay);

            this.Db.ReadOnly = false;
     
           ProcessOrdersImportByDateTime(string.IsNullOrEmpty(datetime) ? null : datetime);
        }

        public void ImportInvoicesByDateTime(string datetime)
        {
            //get the invoices
            Console.WriteLine("Start Invoices import from Mapics for date and time: {0}, {1}",
               DateTime.Now.Date, DateTime.Now.TimeOfDay);

            this.Db.ReadOnly = false;
            
            ProcessInvoicesImportByDateTime(string.IsNullOrEmpty(datetime) ? null : datetime);
        }

        public void LogAndSendErrorsOnOrderStatusUpdates(string identifier, string errorMessage)
        {
            System.Diagnostics.Trace.TraceError(errorMessage);

            var customErrorMessage = string.Concat($"Unable to update projects for {identifier} in DC due to : {errorMessage}");
            var emailVal = "dpo.dev.team.email";
            var subject = "Daikin Project Update Errors";

            WebImportError.NotifyErrorViaEmail(customErrorMessage, this.GetType().Name, emailVal, subject);
            _log.Fatal("Updating project from Mapics failed");
            _log.FatalFormat("Error Details: {0}", customErrorMessage);

            Console.WriteLine(customErrorMessage);
        }
        #endregion

        #region Import Drop Down data
        public void ImportDropDownData(DateTime? fromDate)
        {
            this.Db.ReadOnly = false;

            var ops = this.GetOptions(fromDate);

            foreach (var op in ops)
            {
            }
        }
        #endregion Import Drop Down data
    }
}
