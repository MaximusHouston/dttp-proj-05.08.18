using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DPO.Common;
using DPO.Data;
using System.Data.Entity;
using DPO.Domain.DaikinWebServices;

namespace DPO.Domain
{
    public class CRMAccountImport : BaseServices
    {
        public readonly DPOContext context;
        public readonly DaikinServices daikinServices;

        public CRMAccountImport(DPOContext dpoContext)
        {
            context = dpoContext;
            daikinServices = new DaikinServices();
            this.Db = new Repository(context);
        }

        public Dictionary<string, State> states => Db.States.Where(s => s.CountryCode != "GB").ToArray()
                            .ToDictionary(b => b.CountryCode + ":" + b.Code);

        public void InsertNewRecordsIntoDC(List<Account> newRecordList)
        {
            Console.WriteLine($"Number of new records that were not found in DC {newRecordList.Count()}");

            Business dpoBusiness;
            foreach (var newRecord in newRecordList)
            {
                Console.WriteLine($"Attempting to insert {newRecord.CRMAccountNumber} into DC");

                var type = BusinessTypeEnum.Other;

                if (newRecord.AccountCategoryCode != null)
                    type = (BusinessTypeEnum)newRecord.AccountCategoryCode;
                else
                    Console.WriteLine("Business type for {0} is null, defaulting to 'Unknown'", newRecord.Name);

                dpoBusiness = Db.BusinessCreate(type);
                dpoBusiness.AccountId = newRecord.CRMAccountNumber;
                dpoBusiness.ERPAccountId = newRecord.BillingAccountNumber;
                dpoBusiness.BusinessName = newRecord.Name;

                if (!string.IsNullOrEmpty(newRecord.YearToDateSales))
                    dpoBusiness.YearToDateSales = decimal.Parse(newRecord.YearToDateSales);

                if (!string.IsNullOrEmpty(newRecord.OpenOrderTotals))
                    dpoBusiness.OpenOrdersTotal = decimal.Parse(newRecord.OpenOrderTotals);

                //Identify if VRVPro DCpro
                dpoBusiness.IsVRVPro = newRecord.VRVPro;
                dpoBusiness.IsDaikinComfortPro = newRecord.DaikinComfortPro;

                //check if non po Business address and mapics Business address both are provided
                //if so just do straight comparison on physical address first and mapics address second
                if (newRecord.Address1Line1 != null)
                {
                    dpoBusiness.Address.AddressLine1 = newRecord.Address1Line1;
                    dpoBusiness.Address.AddressLine2 = newRecord.Address1Line2;
                    dpoBusiness.Address.AddressLine3 = newRecord.Address1Line3;
                    dpoBusiness.Address.PostalCode = newRecord.Address1PostalCode;
                    dpoBusiness.Address.Location = newRecord.Address1City;
                    dpoBusiness.Address.State = daikinServices.GetState(states, newRecord.Address1Country, newRecord.Address1StateOrProvinceName);
                    dpoBusiness.Address.StateId = dpoBusiness.Address.State?.StateId;
                }
                else if (newRecord.Address2Line1 != null)
                {
                    dpoBusiness.Address.AddressLine1 = newRecord.Address2Line1;
                    dpoBusiness.Address.AddressLine2 = newRecord.Address2Line2;
                    dpoBusiness.Address.AddressLine3 = newRecord.Address2Line3;
                    dpoBusiness.Address.PostalCode = newRecord.Address2PostalCode;
                    dpoBusiness.Address.Location = newRecord.Address2City;
                    dpoBusiness.Address.State = daikinServices.GetState(states, newRecord.Address2Country, newRecord.Address2StateOrProvince);
                    dpoBusiness.Address.StateId = dpoBusiness.Address.State?.StateId;
                }

                dpoBusiness.Contact.ContactEmail = newRecord.EMailAddress1;
                dpoBusiness.Contact.Website = newRecord.WebAddress;
                dpoBusiness.Contact.Phone = newRecord.Telephone1;

                // Status 1 indicates disabled according to Charles
                var enabled = !(newRecord.StatusCode.HasValue && newRecord.StatusCode.Value == 1);
                if (enabled != dpoBusiness.Enabled)
                    dpoBusiness.Enabled = enabled;

                if (newRecord.AllowExternalCommission.HasValue)
                    dpoBusiness.CommissionSchemeAllowed = newRecord.AllowExternalCommission.Value;

                dpoBusiness.DaikinModifiedOn = newRecord.ModifiedOn;
                dpoBusiness.AccountManagerEmail = newRecord.AccountManagerEmail;
                dpoBusiness.AccountManagerFirstName = newRecord.AccountManager.FirstName;
                dpoBusiness.AccountManagerLastName = newRecord.AccountManager.LastName;
                dpoBusiness.AccountOwnerEmail = newRecord.AccountOwnerEmail;
                dpoBusiness.AccountOwnerFirstName = newRecord.AccountOwner.FirstName;
                dpoBusiness.AccountOwnerLastName = newRecord.AccountOwner.LastName;
                dpoBusiness.AccountOwningGroupName = newRecord.OwningGroup.Name;
                dpoBusiness.DaikinModifiedOn = DateTime.UtcNow;

                daikinServices.businessService.RulesOnAdd(daikinServices.daikinSuperUser, dpoBusiness);

                if (daikinServices.businessService.Response != null && !daikinServices.businessService.Response.IsOK)
                    LogAndNotifyError(newRecord, dpoBusiness, daikinServices.businessService.Response);

                try
                {
                    Db.SaveChanges();
                    Console.WriteLine($"Completed insert of {newRecord.CRMAccountNumber} into DC");
                }
                catch (Exception ex)
                {
                    var errorMessage = $"Exception occurred while inserting new record {newRecord} due to {ex.InnerException?.Message}";
                    LogAndSendExceptionMessage(errorMessage, ex);
                }
            }
        }

        public void NotifyFailureToImport(List<Account> newAcctsButSameBusinessNames, IQueryable<Business> businesses)
        {
            Console.WriteLine($"Number of records with new account ids but has similar business name - {newAcctsButSameBusinessNames.Count()}");

            foreach (var newCRMAccount in newAcctsButSameBusinessNames)
            {
                var dcBusinessAccount = businesses?
                                .FirstOrDefault(x => x.BusinessName.Trim().ToUpper() == newCRMAccount.Name.Trim().ToUpper());

                this.Response.AddError("BusinessName", Resources.ResourceModelBusiness.BM004);

                //Error inserting new account ids,  because the business name already exists with different account
                LogAndNotifyError(newCRMAccount, dcBusinessAccount, this.Response);
            }
        }

        public void UpdateExistingRecordsInDC(List<Account> similarRecordList, IQueryable<Business> businesses)
        {
            foreach (var crmRecord in similarRecordList)  //AccountId Exists 
            {
                var dcBusinessRecord = businesses?.Include(b => b.Address).Include(c => c.Contact)
                                        .FirstOrDefault(x => x.AccountId == crmRecord.CRMAccountNumber);
                 
                var propsHaveChanged = false;

                //perfect match to current record in DC
                if (dcBusinessRecord != null && (crmRecord.Name.Trim().ToUpper() == dcBusinessRecord.BusinessName.Trim().ToUpper())) //perfect match to current record
                {
                    //compares the values to determine the rows that needs update
                    propsHaveChanged = CompareCRMandDaikinCityValues(crmRecord, dcBusinessRecord);

                    //Apply business rules for add edit remove
                    daikinServices.businessService.ApplyBusinessRules(daikinServices.daikinSuperUser, dcBusinessRecord);

                    if (daikinServices.businessService.Response != null && !daikinServices.businessService.Response.IsOK)
                    {
                        //Technically never should hit this line
                        LogAndNotifyError(crmRecord, dcBusinessRecord, daikinServices.businessService.Response);
                        propsHaveChanged = false;
                    }
                }
                //Match in Account ID but Business Name has either been modified, changed or NOT similar
                else if (dcBusinessRecord != null && (crmRecord.Name.ToUpperTrim() != dcBusinessRecord.BusinessName.ToUpperTrim()))
                {
                    if (businesses.Any(x => x.BusinessName.Trim().ToUpper() == dcBusinessRecord.BusinessName.Trim().ToUpper()))
                    {
                        propsHaveChanged = false;

                        this.Response.AddError("BusinessName", Resources.ResourceModelBusiness.BM004);

                        //Error in updating because the business name already exists with different account
                        LogAndNotifyError(crmRecord, dcBusinessRecord, this.Response);
                    }
                }

                //Go to DC Db
                try
                {
                    if (propsHaveChanged)
                        Db.SaveChanges();
                }
                catch (Exception ex)
                {
                    var errorMessage = $"Exception occurred while updating new record {dcBusinessRecord.AccountId }due to {ex.InnerException?.Message}";
                    LogAndSendExceptionMessage(errorMessage, ex);
                }
            }
        }

        private bool CompareCRMandDaikinCityValues(Account crmAccount, Business dcBusinessRecord)
        {
            var propsHaveChanged = false;
            if (dcBusinessRecord.ERPAccountId != crmAccount.BillingAccountNumber)
            {
                dcBusinessRecord.ERPAccountId = crmAccount.BillingAccountNumber;
                propsHaveChanged = true;
            }

            if (dcBusinessRecord.BusinessName.ToUpperTrim() != crmAccount.Name.ToUpperTrim())
            {
                dcBusinessRecord.BusinessName = crmAccount.Name.Substring(0, Math.Min(crmAccount.Name.Length, 50));
                propsHaveChanged = true;
            }
            
            if (dcBusinessRecord.BusinessTypeId != (BusinessTypeEnum)crmAccount.AccountCategoryCode)
                dcBusinessRecord.BusinessTypeId = (BusinessTypeEnum)crmAccount.AccountCategoryCode;

            if (!string.IsNullOrEmpty(crmAccount.YearToDateSales))
            {
                dcBusinessRecord.YearToDateSales = decimal.Parse(crmAccount.YearToDateSales);
                propsHaveChanged = true;
            }

            if (!string.IsNullOrEmpty(crmAccount.OpenOrderTotals))
            {
                dcBusinessRecord.OpenOrdersTotal = decimal.Parse(crmAccount.OpenOrderTotals);
                propsHaveChanged = true;
            }

            //check if non po Business address and mapics Business address both are provided
            //if so just do straight comparison on physical address first and mapics address second
            if (crmAccount.Address1Line1 != null)
            {
                if (dcBusinessRecord.Address.AddressLine1 != crmAccount.Address1Line1)
                {
                    dcBusinessRecord.Address.AddressLine1 = crmAccount.Address1Line1;
                    propsHaveChanged = true;
                }

                if (dcBusinessRecord.Address.AddressLine2 != crmAccount.Address1Line2)
                {
                    dcBusinessRecord.Address.AddressLine2 = crmAccount.Address1Line2;
                    propsHaveChanged = true;
                }

                if (dcBusinessRecord.Address.AddressLine3 != crmAccount.Address1Line3)
                {
                    dcBusinessRecord.Address.AddressLine3 = crmAccount.Address1Line3;
                    propsHaveChanged = true;
                }

                if (dcBusinessRecord.Address.PostalCode != crmAccount.Address1PostalCode)
                {
                    dcBusinessRecord.Address.PostalCode = crmAccount.Address1PostalCode;
                    propsHaveChanged = true;
                }

                if (dcBusinessRecord.Address.Location != crmAccount.Address1City)
                {
                    dcBusinessRecord.Address.Location = crmAccount.Address1City;
                    propsHaveChanged = true;
                }

                var state = daikinServices.GetState(states, crmAccount.Address1Country, crmAccount.Address1StateOrProvinceName);
                if (state != null && dcBusinessRecord.Address.StateId != state.StateId)
                {
                    dcBusinessRecord.Address.StateId = state.StateId;
                    propsHaveChanged = true;
                }
            }
            else if (crmAccount.Address2Line1 != null)
            {
                if (dcBusinessRecord.Address.AddressLine1 != crmAccount.Address2Line1)
                {
                    dcBusinessRecord.Address.AddressLine1 = crmAccount.Address2Line1;
                    propsHaveChanged = true;
                }

                if (dcBusinessRecord.Address.AddressLine2 != crmAccount.Address2Line2)
                {
                    dcBusinessRecord.Address.AddressLine2 = crmAccount.Address2Line2;
                    propsHaveChanged = true;
                }

                if (dcBusinessRecord.Address.AddressLine3 != crmAccount.Address2Line3)
                {
                    dcBusinessRecord.Address.AddressLine3 = crmAccount.Address2Line3;
                    propsHaveChanged = true;
                }

                if (dcBusinessRecord.Address.PostalCode != crmAccount.Address2PostalCode)
                {
                    dcBusinessRecord.Address.PostalCode = crmAccount.Address2PostalCode;
                    propsHaveChanged = true;
                }

                if (dcBusinessRecord.Address.Location != crmAccount.Address2City)
                {
                    dcBusinessRecord.Address.Location = crmAccount.Address2City;
                    propsHaveChanged = true;
                }

                var state = daikinServices.GetState(states, crmAccount.Address2Country, crmAccount.Address2StateOrProvince);
                if (state != null && dcBusinessRecord.Address.StateId != state.StateId)
                {
                    dcBusinessRecord.Address.StateId = state.StateId;
                    propsHaveChanged = true;
                }
            }

            if (dcBusinessRecord.Contact.ContactEmail != crmAccount.EMailAddress1)
            {
                dcBusinessRecord.Contact.ContactEmail = crmAccount.EMailAddress1;
                propsHaveChanged = true;
            }

            if (dcBusinessRecord.Contact.Website != crmAccount.WebAddress)
            {
                dcBusinessRecord.Contact.Website = crmAccount.WebAddress;
                propsHaveChanged = true;
            }

            if (dcBusinessRecord.Contact.Phone != crmAccount.Telephone1)
            {
                dcBusinessRecord.Contact.Phone = crmAccount.Telephone1;
                propsHaveChanged = true;
            }

            // Status 1 indicates disabled according to Charles
            var enabled = !(crmAccount.StatusCode.HasValue && crmAccount.StatusCode.Value == 1);
            if (enabled != dcBusinessRecord.Enabled)
            {
                dcBusinessRecord.Enabled = enabled;
                propsHaveChanged = true;
            }

            if (crmAccount.AllowExternalCommission.HasValue && dcBusinessRecord.CommissionSchemeAllowed != crmAccount.AllowExternalCommission)
            {
                dcBusinessRecord.CommissionSchemeAllowed = crmAccount.AllowExternalCommission.Value;
                propsHaveChanged = true;
            }

            if (dcBusinessRecord.DaikinModifiedOn != crmAccount.ModifiedOn)
            {
                dcBusinessRecord.DaikinModifiedOn = crmAccount.ModifiedOn;
                propsHaveChanged = true;
            }

            if (!dcBusinessRecord.IsWebServiceImport) dcBusinessRecord.IsWebServiceImport = true;

            if (dcBusinessRecord.AccountManagerEmail != crmAccount.AccountManagerEmail)
            {
                dcBusinessRecord.AccountManagerEmail = crmAccount.AccountManagerEmail;
                propsHaveChanged = true;
            }

            if (dcBusinessRecord.AccountManagerFirstName != crmAccount.AccountManager.FirstName)
            {
                dcBusinessRecord.AccountManagerFirstName = crmAccount.AccountManager.FirstName;
                propsHaveChanged = true;
            }

            if (dcBusinessRecord.AccountManagerLastName != crmAccount.AccountManager.LastName)
            {
                dcBusinessRecord.AccountManagerLastName = crmAccount.AccountManager.LastName;
                propsHaveChanged = true;
            }

            if (dcBusinessRecord.AccountOwnerEmail != crmAccount.AccountOwnerEmail)
            {
                dcBusinessRecord.AccountOwnerEmail = crmAccount.AccountOwnerEmail;
                propsHaveChanged = true;
            }

            if (dcBusinessRecord.AccountOwnerFirstName != crmAccount.AccountOwner.FirstName)
            {
                dcBusinessRecord.AccountOwnerFirstName = crmAccount.AccountOwner.FirstName;
                propsHaveChanged = true;
            }

            if (dcBusinessRecord.AccountOwnerLastName != crmAccount.AccountOwner.LastName)
            {
                dcBusinessRecord.AccountOwnerLastName = crmAccount.AccountOwner.LastName;
                propsHaveChanged = true;
            }

            if (dcBusinessRecord.AccountOwningGroupName != crmAccount.OwningGroup.Name)
            {
                dcBusinessRecord.AccountOwningGroupName = crmAccount.OwningGroup.Name;
                propsHaveChanged = true;
            }

            if (dcBusinessRecord.IsVRVPro != crmAccount.VRVPro || dcBusinessRecord.IsDaikinComfortPro != crmAccount.DaikinComfortPro)
            {
                dcBusinessRecord.IsVRVPro = crmAccount.VRVPro;
                dcBusinessRecord.IsDaikinComfortPro = crmAccount.DaikinComfortPro;
                propsHaveChanged = true;
            }

            return propsHaveChanged;
        }

        private void LogAndNotifyError(Account crmAccount, Business dcBusinessRecord, ServiceResponse serviceResponse)
        {
            var sb = new StringBuilder();
            serviceResponse.GetMessages(MessageTypeEnum.Error).ForEach(m => sb.AppendLine("\t" + m.Text));
            Log.Debug(serviceResponse.Messages);
            Console.WriteLine("AccountId: " + crmAccount.CRMAccountNumber + "\n " + sb.ToString());

            //notify sales team here that there exists another record with different account Id but same business name
            // the reason for failure in account import
            if (serviceResponse.GetMessages(MessageTypeEnum.Error).Any(x => x.Key == "BusinessName"))
            {
                var dcAccountId = Db.Businesses.Where(x => x.BusinessName.Trim().ToUpper() == crmAccount.Name.Trim().ToUpper())
                                                .Select(y => y.AccountId).FirstOrDefault();
                var dcBusinessName = Db.Businesses.Where(x => x.BusinessName.Trim().ToUpper() == crmAccount.Name.Trim().ToUpper())
                                                .Select(y => y.BusinessName).FirstOrDefault();

                var errorMessage = "CRM Account: " + Environment.NewLine + "CRM Account Number: " + crmAccount.CRMAccountNumber +
                                        Environment.NewLine + "Name: " + crmAccount.Name + Environment.NewLine + Environment.NewLine +
                                    "Daikin City Account: " + Environment.NewLine + "Account Id: " + dcAccountId +
                                        Environment.NewLine + "Business Name: " + dcBusinessName;

                var emailValue = Utilities.Config("dpo.sales.team.email");
                var subject = "Daikin Import Errors - Account Import";
                WebImportError.NotifyErrorViaEmail(errorMessage, this.GetType().Name, emailValue, subject); //notify concerned parties..                 
            }

            //this.Context.Entry(dcBusinessRecord).State = EntityState.Unchanged;
            serviceResponse.Messages.Clear();
        }

        private void LogAndSendExceptionMessage(string errorMessage, Exception ex)
        {
            if (System.DateTime.Now.Hour >= 5 && System.DateTime.Now.Hour <= 6)
            {
                var errorMsg = errorMessage;
                var emailValue = Utilities.Config("dpo.dev.team.email");
                var subject = "Daikin Import Errors - Account Import";

                WebImportError.NotifyErrorViaEmail(errorMessage, this.GetType().Name, emailValue, subject);
                daikinServices._log.Fatal("Inserting Business into DC database failed");
            }

            // Remove errored record.  This could be done better with transactions
            //Db.Context.Entry(dpoBusiness).State = EntityState.Detached;
            Console.WriteLine(ex.InnerException?.Message + ex.InnerException);
        }
    }
}
