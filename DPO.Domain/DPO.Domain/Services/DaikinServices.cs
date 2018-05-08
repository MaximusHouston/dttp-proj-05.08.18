using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DPO.Common;
using DPO.Data;
using System.Data.Entity;
using System.Net.Mail;
using DPO.Domain.DaikinWebServices;
using System.Reflection;
using System.IO;
using log4net;

namespace DPO.Domain
{
    public partial class DaikinServices : BaseServices
    {
        public BusinessServices businessService;
        public UserSessionModel daikinSuperUser;
        public QuoteServices quoteServices;
        CrmServiceClient client;

        private string mTokenID = "2B761A29-0626-4881-8D46-B00D2C0726A4";
        public ILog _log;

        public DaikinServices()
            : base(true)
        {
            daikinSuperUser = new AccountServices(this.Context).GetSuperUserSessionModel().Model as UserSessionModel;
            businessService = new BusinessServices(this.Context);
            quoteServices = new QuoteServices(this.Context);

            client = new CrmServiceClient(Utilities.Config("dpo.webservices.endpoint"));
            _log = Log;

        }

        public DaikinServices(DPOContext context)
            : base(context)
        {
            daikinSuperUser = new AccountServices().GetSuperUserSessionModel().Model as UserSessionModel;
            quoteServices = new QuoteServices(this.Context);
            businessService = new BusinessServices(this.Context);

            client = new CrmServiceClient(Utilities.Config("dpo.webservices.endpoint"));

        }
        #region Daikin Web Services Calls

        public DPO.Domain.DaikinWebServices.Account GetAccountId(string accountId)
        {
            var req = new RetrieveAccountsRequest()
            {
                CRMAccountNumber = accountId,
                //CRMAccountNumber = "A251772",
                TokenID = this.mTokenID
            };

            var resp = client.RetrieveAccounts(req);

            var result = resp.Entities;

            return result.FirstOrDefault();
        }

        public List<DPO.Domain.DaikinWebServices.AccountMultiplier> GetAccountMultipliers(DateTime? fromDate)
        {
            DateTimeConditionExpression dateFrom = null;

            if (fromDate.HasValue)
            {
                dateFrom = new DateTimeConditionExpression
                {
                    Operator = DPOConditionOperator.GreaterEqual,
                    Values = new DateTime[] { fromDate.Value }
                };
            }

            var req = new RetrieveAccountMultipliersRequest()
            {
                TokenID = this.mTokenID,
                ModifiedOnCondition = dateFrom
            };


            var result = client.RetrieveAccountMultipliers(req);

            return result.Entities.ToList();
        }

        public List<DPO.Domain.DaikinWebServices.Account> GetAccounts(DateTime? fromDate)
        {

            DateTimeConditionExpression dateFrom = null;

            if (fromDate.HasValue)
            {
                dateFrom = new DateTimeConditionExpression
                {
                    Operator = DPOConditionOperator.GreaterEqual,
                    Values = new DateTime[] { fromDate.Value }
                };
            }

            var req = new RetrieveAccountsRequest()
            {
                TokenID = this.mTokenID,
                ModifiedOnCondition = dateFrom
            };

            var resp = client.RetrieveAccounts(req);

            var result = resp.Entities;

            return result.ToList();
        }

        public SalesLiterature GetDocumentFile(Guid guid)
        {
            var req = new RetrieveSalesLiteratureRequest()
            {
                TokenID = this.mTokenID,
                SalesLiteratureId = guid,
                RetrieveAttachments = true
            };

            var result = client.RetrieveSalesLiterature(req);

            return result.Entities.FirstOrDefault();

        }

        public List<DPO.Domain.DaikinWebServices.SalesLiterature> GetDocuments(DateTime? fromDate)
        {
            DateTimeConditionExpression dateFrom = null;

            if (fromDate.HasValue)
            {
                dateFrom = new DateTimeConditionExpression
                {
                    Operator = DPOConditionOperator.GreaterEqual,
                    Values = new DateTime[] { fromDate.Value }
                };
            }

            var req = new RetrieveSalesLiteratureRequest()
            {
                TokenID = this.mTokenID,
                ModifiedOnCondition = dateFrom
            };


            var result = client.RetrieveSalesLiterature(req);

            return result.Entities.ToList();
        }

        public List<DPO.Domain.DaikinWebServices.Group> GetGroups(DateTime? fromDate)
        {

            DateTimeConditionExpression dateFrom = null;

            if (fromDate.HasValue)
            {
                dateFrom = new DateTimeConditionExpression
                {
                    Operator = DPOConditionOperator.GreaterEqual,
                    Values = new DateTime[] { fromDate.Value }
                };
            }

            var req = new RetrieveGroupsRequest()
            {
                TokenID = this.mTokenID,
                ModifiedOnCondition = dateFrom
            };

            var resp = client.RetrieveGroups(req);

            var results = resp.Entities;

            return results.ToList();
        }

        public List<DPO.Domain.DaikinWebServices.Option> GetOptions(DateTime? fromDate)
        {

            DateTimeConditionExpression dateFrom = null;

            if (fromDate.HasValue)
            {
                dateFrom = new DateTimeConditionExpression
                {
                    Operator = DPOConditionOperator.GreaterEqual,
                    Values = new DateTime[] { fromDate.Value }
                };
            }

            var req = new RetrieveOptionsRequest()
            {
                TokenID = this.mTokenID,
                EntityName = "product"
            };

            var resp = client.RetrieveOptions(req);

            var result = resp.Entities;

            return result.ToList();
        }

        public List<DPO.Domain.DaikinWebServices.SystemComponent> GetProductAccessories(DateTime? fromDate)
        {
            DateTimeConditionExpression dateFrom = null;

            if (fromDate.HasValue)
            {
                dateFrom = new DateTimeConditionExpression
                {
                    Operator = DPOConditionOperator.GreaterEqual,
                    Values = new DateTime[] { fromDate.Value }
                };
            }

            var req = new RetrieveSystemComponentsRequest()
            {
                TokenID = this.mTokenID,
                ModifiedOnCondition = dateFrom
            };

            var result = client.RetrieveSystemComponents(req);

            return result.Entities.ToList();
        }

        public List<DPO.Domain.DaikinWebServices.ProductSalesLiterature> GetProductDocuments(DateTime? fromDate)
        {
            DateTimeConditionExpression dateFrom = null;

            if (fromDate.HasValue)
            {
                dateFrom = new DateTimeConditionExpression
                {
                    Operator = DPOConditionOperator.GreaterEqual,
                    Values = new DateTime[] { fromDate.Value }
                };
            }

            var req = new RetrieveProductSalesLiteratureRequest()
            {
                TokenID = this.mTokenID,
                ModifiedOnCondition = dateFrom
            };

            var result = client.RetrieveProductSalesLiterature(req);

            return result.Entities.ToList();
        }

        public List<DPO.Domain.DaikinWebServices.ProductNote> GetProductNotes(DateTime? fromDate)
        {
            DateTimeConditionExpression dateFrom = null;

            if (fromDate.HasValue)
            {
                dateFrom = new DateTimeConditionExpression
                {
                    Operator = DPOConditionOperator.GreaterEqual,
                    Values = new DateTime[] { fromDate.Value }
                };
            }

            var req = new RetrieveProductNotesRequest()
            {
                TokenID = this.mTokenID,
                ModifiedOnCondition = dateFrom
            };

            var result = client.RetrieveProductNotes(req);

            return result.Entities.ToList();
        }

        public List<DPO.Domain.DaikinWebServices.Product> GetProducts(DateTime? fromDate)
        {

            DateTimeConditionExpression dateFrom = null;

            if (fromDate.HasValue)
            {
                dateFrom = new DateTimeConditionExpression
                {
                    Operator = DPOConditionOperator.GreaterEqual,
                    Values = new DateTime[] { fromDate.Value }
                };
            }

            var req = new RetrieveProductsRequest()
            {
                TokenID = this.mTokenID,
                ModifiedOnCondition = dateFrom
            };

            var resp = client.RetrieveProducts(req);

            var results = resp.Entities;

            return results.ToList();


        }

        public ServiceResponse VerifyAccount(string accountNumber)
        {
            // 
            this.Response = new ServiceResponse();

            try
            {

                var request = new VerifyAccountRequest { AccountNumber = accountNumber, TokenID = mTokenID };

                var response = client.VerifyAccount(request);

                this.Response.Model = response.IsValid;

            }
            catch (Exception e)
            {
                this.Response.AddError(string.Format(Resources.DaikinWebServicesMessages.DWS01, e.Message));
            }

            return this.Response;
        }
        #endregion

        //===========
        /// <summary>
        /// Get all the properties of the web service product entity and
        /// add them to the database if any new ones found
        /// </summary>
        /// <param name="Db"></param>
        private List<Utilities.FastGetProperty<DPO.Domain.DaikinWebServices.Product>> ProcessProductSpecificationLabels()
        {

            // Get List of properties for sPecification table
            var specificationLabels = typeof(DPO.Domain.DaikinWebServices.Product)
                                    .GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // Excude properties which will be saved on product table
            var excludeLabels = new string[] {    "ProductNumber", "Brand", "Name", "ModifiedOn", "MarketType", "ModelType", "AllowExternalCommission",
                                                    "Price", "UnitCategory", "ProductClassCode", "Family", "ExtensionData","SubmittalDatasheetTemplate", "ReleaseStatus" };

            var daikinSpecificationLabels = specificationLabels.Where(p => !excludeLabels.Contains(p.Name)).ToList();

            /// Get all the properties of the web service product entity and
            /// add them to the database only for new ones found
            var currentLabels = Db.ProductSpecificationLabels.ToArray().Select(p => p).ToDictionary(p => p.Name);

            var expressionPropertyGetters = new List<Utilities.FastGetProperty<DPO.Domain.DaikinWebServices.Product>>();


            foreach (var importProperty in daikinSpecificationLabels)
            {
                ProductSpecificationLabel label;

                if (!currentLabels.TryGetValue(importProperty.Name, out label))
                {
                    var newLabel = new ProductSpecificationLabel
                    {
                        Name = importProperty.Name,
                        ProductSpecificationLabelId = (short)(currentLabels.Count() + 1)
                    };

                    currentLabels.Add(newLabel.Name, newLabel);

                    Db.Context.ProductSpecificationLabels.Add(newLabel);
                }

                expressionPropertyGetters.Add(Utilities.FastGetPropertySetup<DPO.Domain.DaikinWebServices.Product>(importProperty.Name));
            }

            Db.SaveChanges();
            return expressionPropertyGetters;
        }

        #region ProcessRequirementTypes

        private List<RequirementType> ProcessRequirementTypes(List<SystemComponent> importComponents)
        {

            var daikinRequirements = importComponents
                                  .Where(r => r.RequirementLevelName != null && r.RequirementLevel != null)
                                  .Select(r => new RequirementType { RequirementTypeId = r.RequirementLevel ?? 0, Description = r.RequirementLevelName })
                                  .ToList();

            var currentRequirements = Db.Context.RequirementTypes.ToList();

            foreach (var requirement in daikinRequirements)
            {
                var currentRequirement = currentRequirements.Where(r => r.RequirementTypeId == requirement.RequirementTypeId).FirstOrDefault();

                if (currentRequirement != null)
                {
                    if (currentRequirement.Description != requirement.Description) currentRequirement.Description = requirement.Description;
                }
                else
                {
                    Db.Context.RequirementTypes.Add(requirement);
                }
            }

            Db.SaveChanges();

            return Db.Context.RequirementTypes.Local.ToList();
        }

        #endregion

        #region Recalculate Quote Methods
        public void RecalculateQuotes()
        {
            var quotes = this.Db.Quotes
                .Include("QuoteItems.Product")
                .Select(s => s)
                .ToList(); // Can't do updates during query so have to do a list.

            foreach (var quote in quotes)
            {
                if (quote.QuoteItems.Count <= 0)
                {
                    continue;
                }

                Db.Entry(quote).State = EntityState.Modified;
                quoteServices.CalculateUnitCounts(this.daikinSuperUser, quote);
                quoteServices.SaveToDatabase(String.Empty);
            }
        }
        #endregion Recalculate Quote Methods

        private static string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }

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

            /*
               Determine if any quotes need to be alerted foe recalcuation.
             */

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
    }
}
