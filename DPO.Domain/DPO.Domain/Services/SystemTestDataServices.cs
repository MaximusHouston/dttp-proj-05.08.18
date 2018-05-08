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
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using DPO.Resources;


namespace DPO.Domain
{

    // ####################################################################################
    // Group/User test info set up
    // ####################################################################################
    //                                      Daikin[GRP0]
    //                                      USSA0  USB0
    //                                     
    //              Central[GRP1]                                      Eastern[GRP2]
    //              USRM1 USB0                                         USRM2 USB0
    //              
    //   Baltimore[GRP3]            Florida[GRP4]                       Chicago[GRP5]            Dallas[GRP6]
    //
    //   USAM1  USB1                USAM3 [USB3](group owner)           USAM4  USB4              USAM6  USB6
    //   US1    USB1(Dealer)        US3   [USB3]                        US4    USB4              US6    USB6*
    //   USAM2  USB2                                                    USAM5  USB5*
    //   US2    USB2*                                                   US5    USB5(Dealer)
    //   US7    USB2(unallocated group)                                 USDE1  USB0(group owner)
    //                                                                  USDA2  USB0(group owner)
    //                                                                  US8    USB5(unallocated group)
    //                                                                  US9    USB5(unallocated group)
    //   US99-199 USB9 (Other)
    // * Not approved
    // ####################################################################################

   
    public partial class SystemTestDataServices : BaseServices
    {
        public Group GRP0, GRP1, GRP2, GRP3, GRP4, GRP5, GRP6;
        public Business USB0, USB1, USB2, USB3, USB4, USB5, USB6, USB9;
        public User USSA0, USRM1, USRM2;
        public User USAM1, USAM2, USAM3, USAM4, USAM5, USAM6;
        public User US1, US2, US3, US4, US5, US6, US9;
        public AccountMultiplier ACM1, ACM2, ACM3, ACM4, ACM5, ACM6;
  

        public SystemTestDataServices() : base(true) { }

        public SystemTestDataServices(DPOContext context) : base(context) { }

        public void SeedSystemTestData()
        {
            SeedSystemTestGroupsAndBusinesses();

            SeedSystemTestUsers();

            SeedSystemTestProducts();

            SeedSystemTestProjects();

            new DaikinServices().RunDatabaseMaintenanceRoutines();

        }

        private void SeedSystemTestGroupsAndBusinesses()
        {

            GRP0 = GetOrCreateTestGroup("Test Daikin", null);

            GRP1 = GetOrCreateTestGroup("Test Central", GRP0.GroupId);
            GRP3 = GetOrCreateTestGroup("Test Baltimore", GRP1.GroupId);
            GRP4 = GetOrCreateTestGroup("Test Florida", GRP1.GroupId);

            GRP2 = GetOrCreateTestGroup("Test Eastern", GRP0.GroupId);
            GRP5 = GetOrCreateTestGroup("Test Chicago", GRP2.GroupId);
            GRP6 = GetOrCreateTestGroup("Test Dallas", GRP2.GroupId);

            Db.UpdateGroupInformation();

            Db.SaveChanges();

            //new DaikinServices().ImportBusinessData(null);

            USB0 = GetOrCreateTestBusiness("USB0", "DaikinTestAccount", BusinessTypeEnum.Daikin);
            USB1 = GetOrCreateTestBusiness("USB1", "", BusinessTypeEnum.Dealer);
            USB2 = GetOrCreateTestBusiness("USB2", "A222", BusinessTypeEnum.Other);
            USB3 = GetOrCreateTestBusiness("USB3", "A333", BusinessTypeEnum.ManufacturerRep);
            USB4 = GetOrCreateTestBusiness("USB4", "A444", BusinessTypeEnum.Distributor);
            USB5 = GetOrCreateTestBusiness("USB5", "", BusinessTypeEnum.Dealer);
            USB6 = GetOrCreateTestBusiness("USB6", "A666", BusinessTypeEnum.ManufacturerRep);

            USB9 = GetOrCreateTestBusiness("USB9", "A999", BusinessTypeEnum.Other);

            Db.SaveChanges();

        }

        private void SeedSystemTestUsers()
        {

            // New registration awaiting approval
            USSA0 = GetOrCreateTestUser(this.Db.Context, GRP0, USB0, "USSA0", UserTypeEnum.DaikinSuperUser, true,true);
            USRM1 = GetOrCreateTestUser(this.Db.Context, GRP1, USB0, "USRM1", UserTypeEnum.DaikinAdmin, true, true);
            USRM2 = GetOrCreateTestUser(this.Db.Context, GRP2, USB0, "USRM2", UserTypeEnum.DaikinAdmin, true, true);
            USAM1 = GetOrCreateTestUser(this.Db.Context, GRP3, USB1, "USAM1", UserTypeEnum.CustomerAdmin, true, true);
            USAM2 = GetOrCreateTestUser(this.Db.Context, GRP3, USB2, "USAM2", UserTypeEnum.CustomerAdmin, true, false);
            USAM3 = GetOrCreateTestUser(this.Db.Context, GRP4, USB3, "USAM3", UserTypeEnum.CustomerAdmin, true, true);
            USAM4 = GetOrCreateTestUser(this.Db.Context, GRP5, USB4, "USAM4", UserTypeEnum.CustomerAdmin, true, true);
            USAM5 = GetOrCreateTestUser(this.Db.Context, GRP5, USB5, "USAM5", UserTypeEnum.CustomerAdmin, false, true);
            USAM6 = GetOrCreateTestUser(this.Db.Context, GRP6, USB6, "USAM6", UserTypeEnum.CustomerAdmin, true, true);

            US1 = GetOrCreateTestUser(this.Db.Context, GRP3, USB1, "US1", UserTypeEnum.CustomerUser, true,false);
            US2 = GetOrCreateTestUser(this.Db.Context, GRP3, USB2, "US2", UserTypeEnum.CustomerUser, false, false);
            US3 = GetOrCreateTestUser(this.Db.Context, GRP4, USB3, "US3", UserTypeEnum.CustomerUser, true, false);

            US4 = GetOrCreateTestUser(this.Db.Context, GRP5, USB4, "US4", UserTypeEnum.CustomerUser, true, false);
            US5 = GetOrCreateTestUser(this.Db.Context, GRP5, USB5, "US5", UserTypeEnum.CustomerUser, true, false);

            GetOrCreateTestUser(this.Db.Context, GRP5, USB9, "USDE1", UserTypeEnum.DaikinEmployee, true, true);
            GetOrCreateTestUser(this.Db.Context, GRP5, USB9, "USDA2", UserTypeEnum.DaikinAdmin, true, true);

            US6 = GetOrCreateTestUser(this.Db.Context, GRP6, USB6, "US6", UserTypeEnum.CustomerUser, false, false);

            US6 = GetOrCreateTestUser(this.Db.Context, null, USB2, "US7", UserTypeEnum.CustomerUser, true, false);
            US6 = GetOrCreateTestUser(this.Db.Context, null, USB5, "US8", UserTypeEnum.CustomerUser, true, false);
            US6 = GetOrCreateTestUser(this.Db.Context, null, USB5, "US9", UserTypeEnum.CustomerUser, true, false);

            if (!Db.IsUser("USSA0@Somewhere.com"))
            {
                //Approved Pap data
                for (int i = 99; i <= 199; i++)
                {
                    var user = GetOrCreateTestUser(this.Db.Context, GRP3, USB9, "US" + i.ToString(), UserTypeEnum.CustomerUser, true,false);
                }
            }

            Db.SaveChanges();

            Db.SystemRoutinesUpdateMembersCountForGroups();

        }

        private void SeedSystemTestProducts()
        {
            ACM1 = Db.Context.AccountMultipliers.Where(a => a.ProductClassCode == "AM1").FirstOrDefault() ?? Db.AccountMultiplierCreate("AM1", USB1, 0.4M);
            ACM2 = Db.Context.AccountMultipliers.Where(a => a.ProductClassCode == "AM2").FirstOrDefault() ?? Db.AccountMultiplierCreate("AM2", USB2, 0.35M);
            ACM3 = Db.Context.AccountMultipliers.Where(a => a.ProductClassCode == "AM3").FirstOrDefault() ?? Db.AccountMultiplierCreate("AM3", USB3, 0.12M);
            ACM4 = Db.Context.AccountMultipliers.Where(a => a.ProductClassCode == "AM4").FirstOrDefault() ?? Db.AccountMultiplierCreate("AM4", USB4, 0.5M);
            ACM5 = Db.Context.AccountMultipliers.Where(a => a.ProductClassCode == "AM5").FirstOrDefault() ?? Db.AccountMultiplierCreate("AM5", USB5, 0.3M);
            ACM6 = Db.Context.AccountMultipliers.Where(a => a.ProductClassCode == "AM6").FirstOrDefault() ?? Db.AccountMultiplierCreate("AM6", USB6, 0.2M);
            
            //new DaikinServices().ImportProductData(null);

            //new DaikinServices().ImportProductAccessoriesData(null);

            //new DaikinServices().ImportDocumentData(null);

            //new DaikinServices().ImportProductDocumentData(null);
            
            Db.SaveChanges();
        }

        private void SeedSystemTestProjects()
        {
            if (Db.Projects.Any(u => u.Name == "Project 1")) return;

            var projecttypes = Db.ProjectTypes.FirstOrDefault();
            var ProjectOpenStatus = Db.ProjectOpenStatusTypes.FirstOrDefault();
            var projectstatus = Db.ProjectStatusTypes.FirstOrDefault();
            var verticaltype = Db.VerticalMarketTypes.FirstOrDefault();

            var QTPJ1 = CreateTestProject(Db.Context, US1, "building works", "Project 1", DateTime.Now, projecttypes, ProjectOpenStatus, projectstatus, verticaltype);
            var QTPJ2 = CreateTestProject(Db.Context, US1, "building works", "Project 2", DateTime.Now, projecttypes, ProjectOpenStatus, projectstatus, verticaltype);
            var QTPJ3 = CreateTestProject(Db.Context, US1, "building works", "Project 3", DateTime.Now, projecttypes, ProjectOpenStatus, projectstatus, verticaltype);
            var QTPJ4 = CreateTestProject(Db.Context, US4, "building works", "Project 4", DateTime.Now, projecttypes, ProjectOpenStatus, projectstatus, verticaltype);
            var QTPJ5 = CreateTestProject(Db.Context, US4, "building works", "Project 5", DateTime.Now, projecttypes, ProjectOpenStatus, projectstatus, verticaltype);
            var QTPJ6 = CreateTestProject(Db.Context, US4, "building works", "Project 6", DateTime.Now, projecttypes, ProjectOpenStatus, projectstatus, verticaltype);
            var QTPJ7 = CreateTestProject(Db.Context, US6, "building works", "Project 7", DateTime.Now, projecttypes, ProjectOpenStatus, projectstatus, verticaltype);
            var QTPJ8 = CreateTestProject(Db.Context, US6, "building works", "Project 8", DateTime.Now, projecttypes, ProjectOpenStatus, projectstatus, verticaltype);
            var QTPJ9 = CreateTestProject(Db.Context, US6, "building works", "Project 9", DateTime.Now, projecttypes, ProjectOpenStatus, projectstatus, verticaltype);


            var QTP1Q1 = CreateTestQuote(Db.Context, QTPJ1, "Quote 1",  false, 1);
            var QTP1Q2 = CreateTestQuote(Db.Context, QTPJ1, "Quote 2",  true, 2);
            var QTP2Q3 = CreateTestQuote(Db.Context, QTPJ2, "Quote 3",  false, 1);
            var QTP2Q4 = CreateTestQuote(Db.Context, QTPJ2, "Quote 4",  true, 2);
            var QTP3Q5 = CreateTestQuote(Db.Context, QTPJ3, "Quote 5",  true, 1);
            var QTP4Q6 = CreateTestQuote(Db.Context, QTPJ4, "Quote 6",  true, 1);
            var QTP5Q7 = CreateTestQuote(Db.Context, QTPJ5, "Quote 7",  true, 1);
            var QTP6Q8 = CreateTestQuote(Db.Context, QTPJ6, "Quote 8",  true, 1);
            var QTP6Q9 = CreateTestQuote(Db.Context, QTPJ6, "Quote 9",  false, 1);
            var QTP7Q10 = CreateTestQuote(Db.Context, QTPJ7, "Quote 10",  true, 1);
            var QTP8Q11 = CreateTestQuote(Db.Context, QTPJ8, "Quote 11",  true, 1);
            var QTP9Q12 = CreateTestQuote(Db.Context, QTPJ9, "Quote 12",  true, 1);

            var products = Db.Context.Products.ToList();

            AddTestProductsToQuote(QTP1Q1, products, 1, ACM1);
            AddTestProductsToQuote(QTP1Q2, products, 2, ACM1);
            AddTestProductsToQuote(QTP2Q3, products, 3, ACM1);
            AddTestProductsToQuote(QTP2Q4, products, 4, ACM1);
            AddTestProductsToQuote(QTP3Q5, products, 5, ACM2);
            AddTestProductsToQuote(QTP4Q6, products, 6, ACM2);
            AddTestProductsToQuote(QTP5Q7, products, 7, ACM2);
            AddTestProductsToQuote(QTP6Q8, products, 8, ACM2);
            AddTestProductsToQuote(QTP6Q9, products, 9, ACM2);
            AddTestProductsToQuote(QTP7Q10, products, 10, ACM3);
            AddTestProductsToQuote(QTP8Q11, products, 11, ACM3);
            AddTestProductsToQuote(QTP9Q12, products, 12, ACM4);

            Db.Context.SaveChanges();

        }

        #region Testing helpers

        
        // ####################################################################################
        // Create business
        // ####################################################################################
        public Group GetOrCreateTestGroup(string groupName,long? parentGroupId)
        {
            var group = Db.Groups.Where(g => g.Name == groupName).FirstOrDefault();

            if (group == null)
            {
                group = Db.GroupCreate(groupName, parentGroupId);
            }

            return group;
        }

        // ####################################################################################
        // Create business
        // ####################################################################################
        public Business GetOrCreateTestBusiness(string name, string accountId, BusinessTypeEnum businessType)
        {
            var businessName = name + " " + businessType.ToString();

            var business = Db.Businesses.Where(g => g.BusinessName == businessName).FirstOrDefault();

            if (business == null)
            {
                business = Db.BusinessCreate(BusinessTypeEnum.Daikin);

                business.BusinessName = businessName;
                business.BusinessTypeId = businessType;
                business.AccountId = accountId;
                business.ShowPricing = (businessType == BusinessTypeEnum.Dealer || businessType == BusinessTypeEnum.Other);
                business.Enabled = true;

                business.CommissionSchemeAllowed = (businessType == BusinessTypeEnum.Distributor || businessType == BusinessTypeEnum.ManufacturerRep);

                business.Address.AddressLine1 = "18th Floor";
                business.Address.AddressLine2 = "475 Fifth Avenue";
                business.Address.Location = "New York";
                business.Address.PostalCode = "10017";
                business.Address.StateId = Db.States.Where(c => c.Name == "New York").Select(c => c.StateId).FirstOrDefault();

                business.Contact.Website = "www.daikincity.com";

                // Add all permissions to daikin to business
                Db.CopyPermissions(EntityEnum.BusinessType, (int)businessType, EntityEnum.Business, business.BusinessId);
            }
            return business;

        }

        // ####################################################################################
        // Create user
        // ####################################################################################
        public User GetOrCreateTestUser(DPOContext context, Group group, Business business, string name, UserTypeEnum userType, bool approved,bool isGroupOwner)
        {
            string email = string.Format("{0}@somewhere.com", name);

            var user = Db.UserQueryByEmail(email).FirstOrDefault();
           
            if (user == null)
            {
                user = Db.UserCreate(business, group, userType);

                user.FirstName = name;
                user.LastName = userType.ToString(); ;
                user.RegisteredOn = DateTime.Now;
                user.Email = email;
                user.Salt = 234544543;
                user.Password = Crypto.Hash("test", 234544543);
                user.LastLoginOn = DateTime.Now;
                user.UseBusinessAddress = true;
                user.IsGroupOwner = isGroupOwner;
                if (approved)
                {
                    user.Approved = true;
                    user.ApprovedOn = DateTime.Now;
                    user.Enabled = true;
                }

                // Add all permissions to user
                Db.CopyPermissions(EntityEnum.Business, business.BusinessId, EntityEnum.User, user.UserId);

                Db.CopyPermissions(EntityEnum.UserType, (long)userType, EntityEnum.User, user.UserId);
            }
            return user;

        }

        // ####################################################################################
        // Create project
        // ####################################################################################
        public Project CreateTestProject(DPOContext context, User owner, string customername, string title, DateTime projectDate, ProjectType type, ProjectOpenStatusType status, ProjectStatusType projectstatus, VerticalMarketType vertical)
        {
            var project = Db.ProjectCreate(owner.UserId);

            project.CustomerName = customername;

            project.Name = title;
            project.ProjectTypeId = type.ProjectTypeId;
            project.ConstructionTypeId = (int)ConstructionTypeEnum.New;
            project.ProjectOpenStatusTypeId = status.ProjectOpenStatusTypeId;
            project.ProjectStatusTypeId = projectstatus.ProjectStatusTypeId;
            project.VerticalMarketTypeId = vertical.VerticalMarketTypeId;
            project.ProjectDate = projectDate;
            project.BidDate = projectDate.AddDays(1);
            project.EstimatedClose = projectDate.AddDays(3);
            project.EstimatedDelivery = project.EstimatedClose.AddDays(7);
            project.Expiration = project.EstimatedClose.AddDays(30);

            return project;
        }

        // ####################################################################################
        // Create Quote
        // ####################################################################################
        public Quote CreateTestQuote(DPOContext context, Project project, string title, bool active, int version)
        {
            var quote = Db.QuoteCreate(project.ProjectId);

            quote.Title = title;
            quote.QuoteId = context.GenerateNextLongId();
            quote.ProjectId = project.ProjectId;
            quote.Title = title;
            quote.Active = active;
            quote.Revision = version;

            if (active)
            {
                project.ActiveVersion = version;
            }
            return quote;
        }

        // ####################################################################################
        // Create quote item
        // ####################################################################################
        public QuoteItem CreateTestQuoteItem(DPOContext context, Quote quote, Product product, decimal quantity, AccountMultiplier multiplier)
        {
            var item = Db.QuoteItemCreate(quote);

            item.QuoteId = quote.QuoteId;
            item.ProductId = product.ProductId;
            item.ProductNumber = product.ProductNumber;
            item.Description = product.Name;
            item.Quantity = quantity;
            item.Multiplier = multiplier.Multiplier;
            item.AccountMultiplierId = multiplier.AccountMultiplierId;
            item.ListPrice = product.ListPrice;

            return item;
        }

        // ####################################################################################
        // Add products to a quote
        // ####################################################################################
        public void AddTestProductsToQuote(Quote quote, List<Product> products, int productSelectionVariance, AccountMultiplier multiplier)
        {
            for (int i = 0; i < products.Count; i++)
            {
                if (productSelectionVariance % (i + 1) == 0)
                {
                    var item = CreateTestQuoteItem(Db.Context, quote, products[i], i + 1, multiplier);

                    quote.QuoteItems.Add(item);
                    if (products[i].AllowCommissionScheme)
                    {
                        quote.TotalListCommission += item.ListPrice * item.Quantity;
                        quote.TotalNetCommission += quote.TotalListCommission * ((item.Multiplier == 0) ? 1 : item.Multiplier);
                    }
                    else
                    {
                        quote.TotalListNonCommission += item.ListPrice * item.Quantity;
                        quote.TotalNetNonCommission += quote.TotalListNonCommission * ((item.Multiplier == 0) ? 1 : item.Multiplier);
                    }
                    quote.TotalNet = quote.TotalNetCommission;
                    quote.TotalList = quote.TotalListCommission;
                }
            }
        }

        #endregion
    }

}