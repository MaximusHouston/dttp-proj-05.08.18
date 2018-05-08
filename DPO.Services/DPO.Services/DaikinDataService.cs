using DPO.Common;
using DPO.Data;
using DPO.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.IO;
using CsvHelper;
using System.Globalization;
using System.Net.Mail;
using log4net;
using Renci.SshNet;
using Renci.SshNet.Common;
using Renci.SshNet.Sftp;

namespace DPO.Services
{
    class DaikinDataService
    {
        static void Main(string[] args)
        {
            var startTime = DateTime.Now.Ticks;
            using (var data = new Repository())
            {
                var currentProductsLookup = data.GetCurrentProducts();

                //setting log4net
                log4net.Config.XmlConfigurator.Configure();

                if (args.Length == 1)
                {
                    args[0] = args[0].Replace("\r\n", "");

                    if (string.Compare(args[0], "SystemSetUpOnDevOrQA", true) == 0)
                    {
                        using (var systemServices = new SystemServices())
                        {
                            systemServices.SeedSystemDataDefaults();
                        }
                    }

                    if (string.Compare(args[0], "SystemSetUp", true) == 0)
                    {
                        using (var systemServices = new SystemServices())
                        {
                            systemServices.SeedSystemDataDefaults();
                        }
                    }

                    if (string.Compare(args[0], "SubmittalDataGenerate", true) == 0)
                    {
                        SubmittalDataGenerate();
                    }
                    //LMW Added 02/18
                    if (string.Compare(args[0], "SubmittalDataAutoGenerate", true) == 0)
                    {
                        SubmittalDataAutoGenerate();
                    }
                    //LMW Added 02/18 
                    if (string.Compare(args[0], "SFTPtoSDSTransfer", true) == 0)
                    {
                        SFTPtoSDSTransfer();
                    }
                    //LMW Added 02/18 
                    if (string.Compare(args[0], "DeleteSFTPfiles", true) == 0)
                    {
                        DeleteSFTPfiles();
                    }

                    if (string.Compare(args[0], "DaikinImportDocumentFiles", true) == 0)
                    {
                        DaikinImportDocumentFiles();
                    }

                    if (string.Compare(args[0], "DaikinImport", true) == 0)
                    {
                        DaikinImport(currentProductsLookup);
                    }

                    if (string.Compare(args[0], "DaikinImportOrderStatuses", true) == 0)
                    {
                        DaikinImportOrderStatuses();
                    }

                    if (string.Compare(args[0], "DaikinImportOrders", true) == 0)
                    {
                        DaikinImportOrders(null);
                    }

                    if (string.Compare(args[0], "DaikinImportInvoices", true) == 0)
                    {
                        DaikinImportInvoices(null);
                    }

                    if (args[0].Contains("-"))
                    {
                        var inputVal = args[0];
                        var inputArray = inputVal.Split('-');

                        if (inputArray[0].Contains("Orders"))
                            DaikinImportOrders(inputArray[1]);

                        if (inputArray[0].Contains("Invoices"))
                            DaikinImportInvoices(inputArray[1]);
                    }

                    if (string.Compare(args[0], "DaikinPricing", true) == 0)
                    {
                        DaikinPricing();
                    }

                    if (string.Compare(args[0], "DaikinImportMultipliers", true) == 0)
                    {
                        DaikinImportMultipliers();
                    }

                    if (string.Compare(args[0], "AddProjectTransferToCustomerAdmins", true) == 0)
                    {
                        Add_Project_Transfer_To_Customer_Admins();
                    }

                    if (string.Compare(args[0], "AddRequestDiscountPermissionToAllDaikinEmployees", true) == 0)
                    {
                        Add_Request_Discount_Permission_To_All_Daikin_Employees();
                    }

                    if (string.Compare(args[0], "AddCMSAccessToDaikinSuperUsers", true) == 0)
                    {
                        Add_CMS_Access_To_Daikin_Super_Users();
                    }

                    if (string.Compare(args[0], "RecalculateQuoteUnitCounts", true) == 0)
                    {
                        RecalculateQuoteUnitCounts();
                    }

                    if (string.Compare(args[0], "ResetSuperUserPermissions", true) == 0)
                    {
                        ResetSuperUserPermissions();
                    }

                    if (string.Compare(args[0], "Releasefix", true) == 0)
                    {
                        Release11072014_Permissions_Adjust_City_Setup();
                    }
                }

                var timeTaken = new TimeSpan(DateTime.Now.Ticks - startTime).TotalMilliseconds / 1000D;

                Console.WriteLine(string.Format("Time taken {0:N4} secs", timeTaken));
            }
        }

        static void ResetSuperUserPermissions()
        {
            Console.WriteLine("Resetting Super User Permissions");
            var username = Utilities.Config("dpo.setup.superuser.username");
            long? userId = null;

            using (var Db = new Repository())
            {
                userId = (from u in Db.Users
                          where u.Email == username
                          select u.UserId).FirstOrDefault();
            }

            if (userId != null)
            {
                AddAllPermissions(userId.Value, PermissionTypeEnum.Brand);
                AddAllPermissions(userId.Value, PermissionTypeEnum.CityArea);
                AddAllPermissions(userId.Value, PermissionTypeEnum.ProductFamily);
                AddAllPermissions(userId.Value, PermissionTypeEnum.SystemAccess);
                AddAllPermissions(userId.Value, PermissionTypeEnum.Tool);
            }
        }

        static void AddAllPermissions(long userId, PermissionTypeEnum permType)
        {
            using (var Db = new Repository())
            {
                var perms = GetAllPermissions(Db, permType);

                EntityEnum? refEntity = null;

                switch (permType)
                {
                    case PermissionTypeEnum.Brand:
                        refEntity = EntityEnum.Brand;
                        break;
                    case PermissionTypeEnum.CityArea:
                        refEntity = EntityEnum.CityArea;
                        break;
                    case PermissionTypeEnum.ProductFamily:
                        refEntity = EntityEnum.ProductFamily;
                        break;
                    case PermissionTypeEnum.SystemAccess:
                        refEntity = EntityEnum.SystemAccess;
                        break;
                    case PermissionTypeEnum.Tool:
                        refEntity = EntityEnum.Tool;
                        break;
                }
                Db.ReadOnly = false;

                foreach (var perm in perms)
                {
                    Db.AddPermission(null, EntityEnum.User, userId, refEntity, (int)perm, permType);
                }

                Db.SaveChanges();
            }
        }

        static List<int> GetAllPermissions(Repository repo, PermissionTypeEnum permType)
        {
            var sysAccessPerms = new List<int>();

            switch (permType)
            {
                case PermissionTypeEnum.Brand:
                    return repo.Brands.Select(s => s.BrandId).ToList();
                case PermissionTypeEnum.CityArea:
                    return repo.CityAreas.Select(s => s.CityAreaId).ToList();
                case PermissionTypeEnum.ProductFamily:
                    return repo.ProductFamilies.Select(s => s.ProductFamilyId).ToList();
                case PermissionTypeEnum.SystemAccess:
                    return repo.SystemAccesses.Select(s => s.SystemAccessId).ToList();
                case PermissionTypeEnum.Tool:
                    return repo.Tools.Select(s => s.ToolId).ToList();
            }

            return sysAccessPerms;
        }

        static void Add_CMS_Access_To_Daikin_Super_Users()
        {
            Console.WriteLine("Adding CMS Access Permissions to Daikin Super Users");

            using (var Db = new Repository())
            {
                var CMSPermissions = new List<SystemAccessEnum>{
                    SystemAccessEnum.ContentManagementApplicationBuildings,
                    SystemAccessEnum.ContentManagementApplicationProducts,
                    SystemAccessEnum.ContentManagementCommsCenter,
                    SystemAccessEnum.ContentManagementFunctionalBuildings,
                    SystemAccessEnum.ContentManagementHomeScreen,
                    SystemAccessEnum.ContentManagementLibrary,
                    SystemAccessEnum.ContentManagementProductFamilies,
                    SystemAccessEnum.ContentManagementTools
                };

                Console.WriteLine("Adding New System Access Types to System Accesses Table");

                var CMSPermissionLabels = new List<string>{
                    "Content Management - Application Buildings",
                    "Content Management - Application Products",
                    "Content Management - Comms Center",
                    "Content Management - Functional Buildings",
                    "Content Management - Home Screen",
                    "Content Management - Library",
                    "Content Management - Product Families",
                    "Content Management - Tools"
                };

                var ExistingCMSSsystemAccesses = (from s in Db.SystemAccesses
                                                  where s.SystemAccessId >= (int)SystemAccessEnum.ContentManagementHomeScreen
                                                  select s).ToList();

                if (ExistingCMSSsystemAccesses.Count == 0)
                {
                    for (var i = 0; i < CMSPermissions.Count; i++)
                    {
                        var newAccess = new SystemAccess
                        {
                            Name = CMSPermissionLabels[i],
                            SystemAccessId = (int)CMSPermissions[i],
                            Timestamp = DateTime.UtcNow
                        };

                        Db.Context.SystemAccesses.Add(newAccess);
                    }

                    Db.SaveChanges();
                }

                var ExistingUserTypeLevelCMSAccesses = (from p in Db.Context.Permissions
                                                        where (p.ObjectId == (long)UserTypeEnum.DaikinSuperUser || p.ObjectId == (long)UserTypeEnum.Systems)
                                                        && p.PermissionTypeId == PermissionTypeEnum.SystemAccess
                                                        && p.ReferenceId >= (int)SystemAccessEnum.ContentManagementHomeScreen
                                                        select p).ToList();

                if (ExistingUserTypeLevelCMSAccesses.Count == 0)
                {
                    //add for daikin super user type
                    foreach (SystemAccessEnum access in CMSPermissions)
                    {
                        //add user-type level permission
                        var userLevelPermission = new Permission
                        {
                            PermissionId = Db.Context.GenerateNextLongId(),
                            ParentPermissionId = null,
                            ObjectId = (long)UserTypeEnum.DaikinSuperUser,
                            PermissionTypeId = PermissionTypeEnum.SystemAccess,
                            ReferenceId = (int)access
                        };

                        Db.Context.Permissions.Add(userLevelPermission);
                    }

                    //add for systems usertype too
                    foreach (SystemAccessEnum access in CMSPermissions)
                    {
                        //add user-type level permission
                        var userLevelPermission = new Permission
                        {
                            PermissionId = Db.Context.GenerateNextLongId(),
                            ParentPermissionId = null,
                            ObjectId = (long)UserTypeEnum.Systems,
                            PermissionTypeId = PermissionTypeEnum.SystemAccess,
                            ReferenceId = (int)access
                        };

                        Db.Context.Permissions.Add(userLevelPermission);
                    }

                    Db.SaveChanges();
                }
                else
                {
                    Console.WriteLine("Daikin Super User UserType Level CMS Access Permissions Already Applied");
                }

                Console.WriteLine("Applying User-Level CMS Access Permissions");

                var supersAndSystems = (from cu in Db.Context.Users
                                        where cu.UserTypeId == UserTypeEnum.DaikinSuperUser
                                        || cu.UserTypeId == UserTypeEnum.Systems
                                        select cu).ToList();

                //add permission for each user individually
                foreach (var admin in supersAndSystems)
                {
                    foreach (SystemAccessEnum access in CMSPermissions)
                    {
                        var ExistingUserLevelPermission = (from p in Db.Context.Permissions
                                                           where p.ObjectId == admin.UserId
                                                           && p.PermissionTypeId == PermissionTypeEnum.SystemAccess
                                                           && p.ReferenceId >= (int)access
                                                           select p).FirstOrDefault();

                        if (ExistingUserLevelPermission == null)
                        {
                            var parentPermission = (from p in Db.Permissions
                                                    where p.ReferenceId == (int)access
                                                     && p.ObjectId == (int)admin.UserTypeId
                                                     && p.PermissionTypeId == PermissionTypeEnum.SystemAccess
                                                    select p).FirstOrDefault();

                            Db.Context.Permissions.Add(new Permission
                            {
                                ObjectId = admin.UserId,
                                ParentPermissionId = parentPermission.PermissionId,
                                PermissionId = Db.Context.GenerateNextLongId(),
                                PermissionTypeId = PermissionTypeEnum.SystemAccess,
                                ReferenceId = (int)access
                            });
                        }
                        else
                        {
                            Console.WriteLine("User " + admin.UserId + " already has permission");
                        }
                    }
                }

                Db.SaveChanges();
            }
        }

        static void Add_Project_Transfer_To_Customer_Admins()
        {
            using (var Db = new Repository())
            {
                var userLevelPermission = (from p in Db.Permissions
                                           where p.ObjectId == (long)UserTypeEnum.CustomerAdmin
                                           && p.PermissionTypeId == PermissionTypeEnum.SystemAccess
                                           && p.ReferenceId == (int)SystemAccessEnum.TransferProject
                                           select p).FirstOrDefault();

                if (userLevelPermission == null)
                {
                    //add user-type level permission
                    userLevelPermission = new Permission
                    {
                        PermissionId = Db.Context.GenerateNextLongId(),
                        ParentPermissionId = null,
                        ObjectId = (long)UserTypeEnum.CustomerAdmin,
                        PermissionTypeId = PermissionTypeEnum.SystemAccess,
                        ReferenceId = (int)SystemAccessEnum.TransferProject
                    };

                    Db.Context.Permissions.Add(userLevelPermission);
                    Db.SaveChanges();
                }

                var customerAdmins = (from cu in Db.Context.Users
                                      where cu.UserTypeId == UserTypeEnum.CustomerAdmin
                                      select cu).ToList();

                //add permission for each user individually
                foreach (var admin in customerAdmins)
                {
                    var userSpecificPermission = (from p in Db.Permissions
                                                  where p.ObjectId == admin.UserId
                                                  && p.PermissionTypeId == PermissionTypeEnum.SystemAccess
                                                  && p.ReferenceId == (int)SystemAccessEnum.TransferProject
                                                  select p).FirstOrDefault();

                    if (userSpecificPermission == null)
                    {
                        Db.Context.Permissions.Add(new Permission
                        {
                            ObjectId = admin.UserId,
                            ParentPermissionId = userLevelPermission.PermissionId,
                            PermissionId = Db.Context.GenerateNextLongId(),
                            PermissionTypeId = PermissionTypeEnum.SystemAccess,
                            ReferenceId = (int)SystemAccessEnum.TransferProject
                        });

                        Db.SaveChanges();
                    }
                }
            }
        }

        static void Add_Request_Discount_Permission_To_All_Daikin_Employees()
        {
            using (var Db = new Repository())
            {
                var DaikinUserTypes = new List<UserTypeEnum>
                {
                    UserTypeEnum.DaikinEmployee,
                    UserTypeEnum.DaikinAdmin,
                    UserTypeEnum.DaikinSuperUser
                };

                for (var i = 0; i < DaikinUserTypes.Count; i++)
                {
                    var daikinUserType = DaikinUserTypes[i];
                    var userLevelPermission = (from p in Db.Permissions
                                               where p.ObjectId == (long)daikinUserType
                                               && p.PermissionTypeId == PermissionTypeEnum.SystemAccess
                                               && p.ReferenceId == (int)SystemAccessEnum.RequestDiscounts
                                               select p).FirstOrDefault();

                    //add user-type level permission if not exists
                    if (userLevelPermission == null)
                    {
                        var newUserLevelPermission = new Permission
                        {
                            PermissionId = Db.Context.GenerateNextLongId(),
                            ParentPermissionId = null,
                            ObjectId = (long)daikinUserType,
                            PermissionTypeId = PermissionTypeEnum.SystemAccess,
                            ReferenceId = 60
                        };

                        Db.Context.Permissions.Add(newUserLevelPermission);
                        Db.SaveChanges();

                        userLevelPermission = newUserLevelPermission;
                    }

                    //get list of all users at this level
                    var usersAtThisLevel = (from cu in Db.Users
                                            where cu.UserTypeId == daikinUserType
                                            select cu).ToList();

                    //add permission for each user individually
                    foreach (var admin in usersAtThisLevel)
                    {
                        var userSpecificPermission = (from p in Db.Permissions
                                                      where p.ObjectId == admin.UserId
                                                      && p.PermissionTypeId == PermissionTypeEnum.SystemAccess
                                                      && p.ReferenceId == (int)SystemAccessEnum.RequestDiscounts
                                                      select p).FirstOrDefault();

                        if (userSpecificPermission == null)
                        {
                            Db.Context.Permissions.Add(new Permission
                            {
                                ObjectId = admin.UserId,
                                ParentPermissionId = userLevelPermission.PermissionId,
                                PermissionId = Db.Context.GenerateNextLongId(),
                                PermissionTypeId = PermissionTypeEnum.SystemAccess,
                                ReferenceId = (int)SystemAccessEnum.RequestDiscounts
                            });
                        }
                    }

                    Db.SaveChanges();
                }
            }
        }

        static void DaikinImport(Dictionary<string, Product> currentProductsLookup)
        {
            int daysfromtoday;
            long lap = 0;
            using (var service = new DaikinServices())
            {
                var config = Utilities.Config("dpo.sys.data.import.daysfromtoday");
                if (!int.TryParse(config, out daysfromtoday)) daysfromtoday = 0;
                var fromdate = (daysfromtoday == 0) ? (DateTime?)null : DateTime.Today.AddDays(daysfromtoday);

                try
                {
                    Console.WriteLine("Starting Import Group Data");
                    lap = DateTime.Now.Ticks;
                    service.ImportGroupData(fromdate);
                    Console.WriteLine(
                        $"Time taken {new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D:N4} secs");
                }
                catch (Exception ex)
                {
                    LogError(ex, "ImportGroupData");
                }

                try
                {
                    Console.WriteLine("Starting Import Business Data");
                    lap = DateTime.Now.Ticks;
                    service.ImportBusinessData(fromdate);
                    Console.WriteLine(
                        $"Time taken {new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D:N4} secs");
                }
                catch (Exception ex)
                {
                    LogError(ex, "ImportBusinessData");
                }
            }

            #region
            /*
            try
            {
                Console.WriteLine("Starting Import Document Data");
                lap = DateTime.Now.Ticks;
                service.ImportDocumentData(fromdate);
                Console.WriteLine(string.Format("Time taken {0:N4} secs", new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D));
            }
            catch (Exception ex)
            {
                LogError(ex, "ImportDocumentData");
            }

            try
            {
                Console.WriteLine("Starting Import Product Data");
                lap = DateTime.Now.Ticks;
                service.ImportProductData(fromdate, currentProductsLookup);// don't update product price here
                Console.WriteLine(string.Format("Time taken {0:N4} secs", new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D));
            }
            catch (Exception ex)
            {
                LogError(ex, "ImportProductData");
            }

            try
            {
                Console.WriteLine("Starting Import Product Accessories Data");
                lap = DateTime.Now.Ticks;
                service.ImportProductAccessoriesData(fromdate, currentProductsLookup);
                Console.WriteLine(string.Format("Time taken {0:N4} secs", new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D));
            }
            catch (Exception ex)
            {
                LogError(ex, "ImportProductAccessoriesData");
            }

            try
            {
                Console.WriteLine("Starting Database Maintenance Routines");
                lap = DateTime.Now.Ticks;
                service.RunDatabaseMaintenanceRoutines();
                Console.WriteLine(string.Format("Time taken {0:N4} secs", new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D));
            }
            catch (Exception ex)
            {
                LogError(ex, "RunDatabaseMaintenanceRoutines");
            }

            try
            {
                Console.WriteLine("Starting Import Product Document Data");
                lap = DateTime.Now.Ticks;
                service.ImportProductDocumentData(fromdate, currentProductsLookup);
                Console.WriteLine(string.Format("Time taken {0:N4} secs", new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D));
            }
            catch (Exception ex)
            {
                LogError(ex, "ImportProductDocumentData");
            }

            try
            {
                Console.WriteLine("Starting Import Product Notes Data");
                lap = DateTime.Now.Ticks;
                service.ImportProductNoteData(fromdate, currentProductsLookup);
                Console.WriteLine(string.Format("Time taken {0:N4} secs", new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D));
            }
            catch (Exception ex)
            {
                LogError(ex, "ImportProductNoteData");
            }

            try
            {
                Console.WriteLine("Starting import UnitInstallationType Data");
                lap = DateTime.Now.Ticks;
                service.ImportUnitInstallationTypeData(fromdate);
                Console.WriteLine(string.Format("Time taken {0:N4} secs", new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D));
            }
            catch(Exception ex)
            {
                LogError(ex, "ImportUnitInstallationTypeData");
            }
            */
            #endregion
        }

        static void DaikinImportOrderStatuses()
        {
            long lap = 0;
            using (var service = new DaikinServices())
            {
                try
                {
                    Console.WriteLine("Starting ORDER STATUS import from Mapics.");
                    lap = DateTime.Now.Ticks;

                    service.ImportOrderStatuses();

                    Console.WriteLine(string.Format("Time taken by ORDER STATUS process is {0:N4} secs",
                        new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D));
                }
                catch (Exception ex)
                {
                    LogError(ex, "Import ORDER STATUS from Mapics");
                }
            }
        }

        static void DaikinImportOrders(string datetime)
        {
            long lap = 0;
            using (var service = new DaikinServices())
            {
                try
                {
                    Console.WriteLine("Starting ORDERS import from Mapics.");
                    lap = DateTime.Now.Ticks;
             
                    service.ImportOrdersByDateTime(string.IsNullOrEmpty(datetime) ? null : datetime);  

                    Console.WriteLine(string.Format("Time taken by Orders import process is {0:N4} secs",
                        new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D));
                }
                catch (Exception ex)
                {
                    LogError(ex, "Import Orders from Mapics");
                }
            }
        }

        static void DaikinImportInvoices(string datetime)
        {
            long lap = 0;
            using (var service = new DaikinServices())
            {
                try
                {
                    Console.WriteLine("Starting INVOICES import from Mapics.");
                    lap = DateTime.Now.Ticks;
                    
                    service.ImportInvoicesByDateTime(string.IsNullOrEmpty(datetime) ? null : datetime);

                    Console.WriteLine(string.Format("Time taken from Invoices import process is {0:N4} secs",
                        new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D));
                }
                catch (Exception ex)
                {
                    LogError(ex, "Import Invoices from Mapics");
                }
            }
        }
 
        static void DaikinPricing()
        {
            int daysfromtoday;
            using (var service = new DaikinServices())
            {
                var config = Utilities.Config("dpo.sys.data.import.daysfromtoday");
                if (!int.TryParse(config, out daysfromtoday)) daysfromtoday = 0;
                var fromdate = (daysfromtoday == 0) ? (DateTime?)null : DateTime.Today.AddDays(daysfromtoday);
                var lap = DateTime.Now.Ticks;

                try
                {
                    Console.WriteLine("Starting Import Product Price");
                    lap = DateTime.Now.Ticks;
                    service.ImportProductPrices(fromdate);
                    Console.WriteLine(string.Format("Time taken {0:N4} secs", new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D));
                }
                catch (Exception ex)
                {
                    LogError(ex, "ImportProductPrices");
                }

                try
                {
                    Console.WriteLine("Starting Database Maintenance Routines");
                    lap = DateTime.Now.Ticks;
                    service.RunDatabaseMaintenanceRoutines();
                    Console.WriteLine(string.Format("Time taken {0:N4} secs", new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D));
                }
                catch (Exception ex)
                {
                    LogError(ex, "RunDatabaseMaintenanceRoutines");
                }
            }
        }

        static void LogError(Exception ex, string serviceName)
        {
            var errorMessage = "Service: " + serviceName + "\t Time: "
                                + DateTime.Now.ToString(CultureInfo.InvariantCulture)
                                + Environment.NewLine
                                + "Exeption: " + ex.Message + Environment.NewLine
                                + "StackTrace: " + ex.StackTrace + Environment.NewLine;

            var errorDetails = string.Empty;

            if (ex.InnerException != null)
            {
                errorDetails = "Error Details: " +
                                ex.InnerException.Message;
            }

            System.Diagnostics.Trace.TraceError(errorMessage);

            var emailVal = Utilities.Config("dpo.sys.email.to");
            var subject = "Daikin Import Errors - Group Data Import";
            WebImportError.NotifyErrorViaEmail(errorMessage, serviceName, emailVal, subject);

            if (string.IsNullOrEmpty(errorDetails)) return;
            System.Diagnostics.Trace.TraceError(ex.InnerException?.Message);
            Console.WriteLine(errorDetails);
        }

        static void DaikinImportDocumentFiles()
        {
            using (var service = new DaikinServices())
            {
                var config = Utilities.Config("dpo.sys.data.import.daysfromtoday");
                int daysfromtoday;

                if (!int.TryParse(config, out daysfromtoday)) daysfromtoday = 0;

                var fromdate = (daysfromtoday == 0) ? (DateTime?)null : DateTime.Today.AddDays(daysfromtoday);

                Console.WriteLine("Starting Import Document Files Data");
                var lap = DateTime.Now.Ticks;
                service.ImportDocumentFiles(fromdate);
                Console.WriteLine(string.Format("Time taken {0:N4} secs", new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D));
            }
        }

        private static void DaikinImportDropDowns()
        {
            using (var service = new DaikinServices())
            {
                var config = Utilities.Config("dpo.sys.data.import.daysfromtoday");
                int daysfromtoday;

                if (!int.TryParse(config, out daysfromtoday)) daysfromtoday = 0;
                var fromdate = (daysfromtoday == 0) ? (DateTime?)null : DateTime.Today.AddDays(daysfromtoday);

                Console.WriteLine("Starting Import Drop Downs");
                var lap = DateTime.Now.Ticks;
                service.ImportDropDownData(fromdate);
                Console.WriteLine(string.Format("Time taken {0:N4} secs", new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D));
            }
        }

        static void DaikinImportMultipliers()
        {
            using (var service = new DaikinServices())
            {
                var config = Utilities.Config("dpo.sys.data.import.daysfromtoday");
                int daysfromtoday;

                if (!int.TryParse(config, out daysfromtoday)) daysfromtoday = 0;

                var fromdate = (daysfromtoday == 0) ? (DateTime?)null : DateTime.Today.AddDays(daysfromtoday);

                Console.WriteLine("Starting Import Account Multipliers");
                var lap = DateTime.Now.Ticks;
                service.ImportAccountMultipliers(fromdate);
                Console.WriteLine(string.Format("Time taken {0:N4} secs", new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D));
            }
        }

        private static ProjectModel getProjectModelByName(string projectName, UserSessionModel user, Repository Db)
        {
            if ((from p in Db.Projects
                 where p.Name.ToLower().Trim() == projectName.ToLower().Trim()
                 && p.OwnerId == user.UserId
                 select p).FirstOrDefault() != null)
            {
                return null;
            }

            using (var addressService = new AddressServices(Db.Context))
            {
                var project = new ProjectModel(user);
                project.Name = projectName;

                project.SellerAddress = addressService.GetAddressModel(user, project.SellerAddress);
                var businessData = Db.BusinessQueryByBusinessId(user, user.BusinessId).Select(u => new { u.AddressId, u.BusinessName }).FirstOrDefault();
                project.SellerAddress.Copy(addressService.GetAddressModel(user, new AddressModel { AddressId = businessData.AddressId }));
                project.SellerName = businessData.BusinessName;

                project.CustomerAddress = new AddressModel();
                project.EngineerAddress = new AddressModel();
                project.ShipToAddress = new AddressModel();

                return project;
            }
        }

        static void MassUpload()
        {
            using (var Db = new Repository())
            {
                var watch = new System.Diagnostics.Stopwatch();
                watch.Start();

                Db.Context.ObjectContext.Connection.Open();

                var location = Utilities.GetDocumentDirectory() + "\\Mass Uploads\\import-test.csv";
                Console.WriteLine("Looking for CSV in location: " + location);

                if (!System.IO.File.Exists(location))
                {
                    Console.WriteLine("CSV file does not exist in Documents Directory - exiting");
                    return;
                }

                Console.WriteLine("Starting Mass upload");

                var sr = new StreamReader(location);
                using (var reader = new CsvReader(sr))
                {
                    var Errors = new List<string>();

                    using (var accountServices = new AccountServices(Db.Context))
                    {
                        var i = 1;

                        var allProducts = Db.Products.ToList();

                        UserSessionModel userModel = null;
                        ProjectModel project = null;
                        QuoteModel quote = null;

                        var projects = new List<ProjectModel>();

                        while (reader.Read())
                        {
                            string[] columns = reader.CurrentRecord;
                            i++;

                            //current line as variables
                            var userId = long.Parse(columns[0].Trim());
                            var userName = columns[1].Trim();
                            var projectName = columns[2].Trim();
                            var projectDate = DateTime.Parse(columns[3].Trim(), new CultureInfo("en-US"));
                            var expirationDate = DateTime.Parse(columns[4].Trim(), new CultureInfo("en-US"));
                            var bidDate = DateTime.Parse(columns[5].Trim(), new CultureInfo("en-US"));
                            var closeDate = DateTime.Parse(columns[6].Trim(), new CultureInfo("en-US"));
                            var deliveryDate = DateTime.Parse(columns[7].Trim(), new CultureInfo("en-US"));
                            var constructionType = columns[8].Trim();
                            //string status             = columns[9].Trim();
                            byte projectOpenStatus = Convert.ToByte(columns[10].Trim());
                            byte projectTypeId = 2; // column 11
                            byte projectStatus = Convert.ToByte(columns[13].Trim());
                            //string verticalMarket     = columns[14].Trim();
                            byte verticalMarketId = Convert.ToByte(columns[15].Trim());
                            var projectNotes = columns[16].Trim();
                            var quoteTitle = columns[17].Trim();
                            var quoteNotes = columns[20].Trim();
                            var totalFreight = String.IsNullOrEmpty(columns[21].Trim()) ? 0 : Convert.ToDecimal(columns[21].Trim());
                            //int commissionPercentage    = 0;
                            //int discountPercentage      = 0;
                            var productNumber = columns[24].Trim();
                            int quantity = String.IsNullOrEmpty(columns[25].Trim()) ? 0 : Convert.ToInt32(columns[25].Trim());
                            string trlQuoteId = columns[27].Trim();

                            //extra variables used
                            byte constructionTypeId = 1;
                            var startTime = DateTime.Now.Ticks;

                            //remove last entry in error list if project uploaded successfully
                            if (Errors.Count > 0 && Errors[Errors.Count - 1].IndexOf("***") > -1)
                            {
                                Errors.RemoveAt(Errors.Count - 1);
                            }

                            if (userModel == null || (userModel != null && userModel.UserId != userId))
                            {
                                userModel = (UserSessionModel)accountServices.GetUserSessionModel(userId).Model;
                            }

                            if (userModel == null)
                            {
                                Trace("Could not find user with id: " + userId, Errors, i);
                                continue;
                            }

                            if (project == null || (project != null && project.Name != projectName))
                            {
                                project = getProjectModelByName(projectName, userModel, Db);

                                if (project == null)
                                {
                                    Trace("Project '" + projectName + "' already exists", Errors, i);
                                    continue;

                                }
                                else
                                {
                                    projects.Add(project);
                                    Trace("*** Project: " + project.Name, Errors, i);
                                }
                            }

                            if (closeDate < bidDate)
                            {
                                Errors.Add(i + " - Estimated Close Date less than bid date, defaulted to bid date");
                                closeDate = bidDate;
                            }

                            if (deliveryDate < closeDate)
                            {
                                Errors.Add(i + " - Estimated Delivery Date less than estimated close date, defaulted to estimated close date");
                                deliveryDate = closeDate;
                            }

                            switch (constructionType.ToLower())
                            {
                                case "new":
                                    constructionTypeId = 1;
                                    break;
                                case "refurbished":
                                case "renovation":
                                    constructionTypeId = 2;
                                    break;
                                case "replacement":
                                    constructionTypeId = 3;
                                    break;
                                default:
                                    Errors.Add(i + " - Invalid construction type, defaulted to new");
                                    break;
                            }

                            project.ProjectDate = projectDate;
                            project.BidDate = bidDate;
                            project.EstimatedClose = closeDate;
                            project.EstimatedDelivery = deliveryDate;
                            project.ConstructionTypeId = constructionTypeId;
                            project.ProjectStatusTypeId = projectStatus;
                            project.ProjectTypeId = projectTypeId;
                            project.ProjectOpenStatusTypeId = projectOpenStatus;
                            project.VerticalMarketTypeId = verticalMarketId;
                            project.Description = projectNotes + "***Imported From TRL***";
                            project.Expiration = expirationDate;


                            if (quote == null || (quote != null && quoteTitle != quote.Title))
                            {
                                // create new
                                quote = new QuoteModel
                                {
                                    Project = project,
                                    Title = quoteTitle,
                                    Description = trlQuoteId.Length == 0 ? "***Imported from TRL***" : "TRL Quote ID " + trlQuoteId + " - ***Imported from TRL***",
                                    IsCommissionSchemeAllowed = false,
                                    TotalFreight = totalFreight,
                                    DiscountPercentage = 0,
                                    CommissionPercentage = 0
                                };

                                project.Quotes.Add(quote);
                                Trace("*** Quote: " + quote.Title, null, i);
                            }

                            if (String.IsNullOrEmpty(productNumber))
                            {
                                Trace("Missing Product Number", Errors, i);
                                continue;
                            }

                            var foundProduct = allProducts.FirstOrDefault(p => p.ProductNumber == productNumber);

                            if (foundProduct == null)
                            {
                                Trace("Product Not Found", Errors, i);
                            }
                            else
                            {
                                quote.Items.Add(new QuoteItemModel
                                {
                                    ProductId = foundProduct.ProductId,
                                    ProductNumber = productNumber,
                                    Quantity = quantity
                                });
                            }
                        }

                        File.WriteAllLines(Utilities.GetDocumentDirectory() + "\\Mass Uploads\\mass-upload-validation-errors.txt", Errors);
                        Errors.Clear();

                        Trace("*** SAVING PROJECTS ***", Errors);
                        //end of while loop
                        UploadProjects(projects, Db, Errors);

                        watch.Stop();

                        if (Errors.Count > 0)
                        {
                            int errorCount = 0;
                            for (var p = 0; p < Errors.Count; p++)
                            {
                                if (Errors[p].IndexOf("***") == -1)
                                {
                                    errorCount++;
                                }
                            }

                            var outPut = "Total Errors: " + errorCount + "\n";

                            Errors.Add(outPut);
                            Trace(String.Format("{0:00}:{1:00}:{2:00}.{3:00}", watch.Elapsed.Hours, watch.Elapsed.Minutes, watch.Elapsed.Seconds, watch.Elapsed.Milliseconds / 10), Errors);

                            File.WriteAllLines(Utilities.GetDocumentDirectory() + "\\Mass Uploads\\mass-upload-insert-errors.txt", Errors);

                            Console.WriteLine(outPut);
                            Console.WriteLine("Press any key to continue...");
                            Console.ReadKey(true);
                            return;
                        }

                        Console.WriteLine("Mass upload complete - no errors");

                        Db.Context.ObjectContext.Connection.Close();
                    }
                }
            }
        }

        static void ProductImportTest()
        {
            using (var service = new DaikinServices())
            {
                using (var data = new Repository())
                {
                    var currentProductsLookup = data.GetCurrentProducts();

                    var config = Utilities.Config("dpo.sys.data.import.daysfromtoday");
                    int daysfromtoday;

                    if (!int.TryParse(config, out daysfromtoday)) daysfromtoday = 0;

                    var fromdate = (daysfromtoday == 0) ? (DateTime?)null : DateTime.Today.AddDays(daysfromtoday);

                    Console.WriteLine("Starting Import Product Data Test...");
                    var lap = DateTime.Now.Ticks;
                    service.ImportProductData(fromdate, currentProductsLookup);
                    Console.WriteLine(string.Format("Time taken {0:N4} secs", new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D));
                }
            }
        }

        static void RecalculateQuoteUnitCounts()
        {
            using (var service = new DaikinServices())
            {
                var config = Utilities.Config("dpo.sys.data.import.daysfromtoday");
                int daysfromtoday;

                if (!int.TryParse(config, out daysfromtoday)) daysfromtoday = 0;

                var fromdate = (daysfromtoday == 0) ? (DateTime?)null : DateTime.Today.AddDays(daysfromtoday);

                Console.WriteLine("Recalculating Quotes");
                var lap = DateTime.Now.Ticks;
                service.RecalculateQuotes();
                Console.WriteLine(string.Format("Time taken {0:N4} secs", new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D));
            }
        }

        static void Release11072014_Permissions_Adjust_City_Setup()
        {
            using (var Db = new Repository())
            {
                // reset permissions
                Db.Context.Database.ExecuteSqlCommand("DELETE FROM CITYAREAS");

                Db.Context.Database.ExecuteSqlCommand("DELETE FROM PERMISSIONS WHERE PermissionTypeId = " + ((int)PermissionTypeEnum.CityArea).ToString());

                Db.SaveChanges();
            }

            using (var systemServices = new SystemServices())
            {
                systemServices.SeedSystemCityAreas();


                using (var Db = new Repository())
                {
                    var cities = Db.CityAreas.ToList();

                    var businessTypes = Db.BusinessTypes.ToList();

                    foreach (var bt in businessTypes)
                    {
                        cities.ForEach(ca => Db.AddPermission(bt, ca));
                    }

                    Db.SaveChanges();

                    Console.WriteLine(cities.Count.ToString());

                    var NonPricingPermissions = cities.Where(c => c.Name != "Library (other)").Select(c => new PermissionListModel { ReferenceId = c.CityAreaId, IsSelected = true }).ToList();
                    var PricingPermissions = cities.Select(c => new PermissionListModel { ReferenceId = c.CityAreaId, IsSelected = true }).ToList();

                    var businesses = Db.Businesses.ToList();

                    foreach (var bu in businesses)
                    {
                        var permissions = (bu.ShowPricing) ? PricingPermissions : NonPricingPermissions;

                        Db.PermissionsUpdate(EntityEnum.BusinessType, (int)bu.BusinessTypeId, EntityEnum.Business, bu.BusinessId, permissions, PermissionTypeEnum.CityArea);
                    }

                    Db.SaveChanges();

                    var user = Db.Users.Include(b => b.Business).ToList();

                    foreach (var us in user)
                    {
                        if (us.Business != null)
                        {
                            var permissions = (us.Business.ShowPricing) ? PricingPermissions : NonPricingPermissions;

                            Db.PermissionsUpdate(EntityEnum.Business, us.BusinessId, EntityEnum.User, us.UserId, permissions, PermissionTypeEnum.CityArea);
                        }
                    }

                    Db.SaveChanges();
                }
            }
        }

        static void Release15062014_AddAccessoryFamily()
        {
            using (var Db = new Repository())
            {
                var ctx = Db.Context;

                if (ctx.ProductFamilies.Any(p => p.ProductFamilyId == 100000010) == false)
                {
                    ctx.ProductFamilies.Add(new ProductFamily { ProductFamilyId = 100000010, Name = "Accessories" });
                    ctx.SaveChanges();
                }
                else
                {
                    return;
                }

                var accessory = ctx.ProductFamilies.Where(p => p.ProductFamilyId == 100000010).Select(a => a).FirstOrDefault();

                var businessTypes = ctx.BusinessTypes.ToList();

                foreach (var bType in businessTypes)
                {
                    Db.AddPermission(bType, accessory);
                }
                ctx.SaveChanges();

                ctx.Businesses.ToList().ForEach(b =>
                {
                    Db.ReplacePermissions(EntityEnum.BusinessType, (long)b.BusinessTypeId, EntityEnum.Business, b.BusinessId, PermissionTypeEnum.ProductFamily);
                });
                ctx.SaveChanges();

                ctx.Users.Include(u => u.Business).ToList().ForEach(u =>
                {
                    if (u.BusinessId != null)
                    {
                        Db.ReplacePermissions(EntityEnum.Business, u.BusinessId.Value, EntityEnum.User, u.UserId, PermissionTypeEnum.ProductFamily);
                    }
                });

                ctx.SaveChanges();
            }
        }

        static void Release20062014_ProductSpecifications()
        {
            using (var Db = new Repository())
            {
                var ctx = Db.Context;

                string[] data = {
                "AccessLevel","100000000","1",
                "AccessLevel","100000001","2",
                "AccessLevel","100000002","3",
                "AccessLevel","100000003","4",
                "AccessLevel","100000004","5",
                "AccessLevel","100000005","6",
                "AccessLevel","100000006","7",
                "AccessLevel","100000007","8",
                "AccessLevel","100000008","9",

                "UnitInstallationType","100000151","Wall Mounted",
                "UnitInstallationType","100000152","Ceiling Suspended",
                "UnitInstallationType","100000153","Ducted",
                "UnitInstallationType","100000154","Floor Standing",
                "UnitInstallationType","100000155","Ceiling Cassette",
                "UnitInstallationType","100000156","Gas Furnace",
                "UnitInstallationType","100000157","Cased Coil",
                "UnitInstallationType","100000158","Coil Only",
                "UnitInstallationType","100000301","Cooling Only",
                "UnitInstallationType","100000302","Heat Pump",
                "UnitInstallationType","100000303","Heat Recovery",
                "UnitInstallationType","100000304","Air Conditioner"};

                var labels = ctx.ProductSpecificationLabels.ToArray().ToDictionary(l => l.Name);

                for (int i = 0; i < data.Length; i += 3)
                {
                    ProductSpecificationLabel label = null;
                    if (labels.TryGetValue(data[i], out label))
                    {
                        var newLookup = new ProductSpecificationKeyLookup
                        {
                            ProductSpecificationLabelId = label.ProductSpecificationLabelId,
                            Key = data[i + 1],
                            Value = data[i + 2]
                        };
                        ctx.ProductSpecificationKeyLookups.Add(newLookup);
                    }
                }

                Db.SaveChanges();
            }
        }

        static void Release22052014_Permissions_Setup()
        {
            using (var Db = new Repository())
            {
                // reset permissions
                Db.Context.Database.ExecuteSqlCommand("DELETE FROM TOOLS");
                Db.Context.Database.ExecuteSqlCommand("DELETE FROM CITYAREAS");
                Db.Context.Database.ExecuteSqlCommand("DELETE FROM SystemAccess");
                Db.Context.Database.ExecuteSqlCommand("DELETE FROM PERMISSIONS");

                // Business type changes
                //var bt = Db.BusinessTypes.Where(b => b.BusinessTypeId == BusinessTypeEnum.Other).FirstOrDefault();

                //bt.Description = "Unknown";
                //bt.Description = "Other";

                if (!Db.BusinessTypes.Any(b => b.BusinessTypeId == BusinessTypeEnum.Other))
                {
                    Db.Context.BusinessTypes.Add(new BusinessType { BusinessTypeId = BusinessTypeEnum.Other, Description = "Other" });
                }

                Db.SaveChanges();
            }
            using (var systemServices = new SystemServices())
            {
                systemServices.SeedSystemDataDefaults();

                using (var Db = new Repository())
                {
                    Db.Businesses.ToList().ForEach(b =>
                    {
                        Db.CopyPermissions(EntityEnum.BusinessType, (long)b.BusinessTypeId, EntityEnum.Business, b.BusinessId);
                    });

                    Db.Context.SaveChanges();

                }

                using (var Db = new Repository())
                {
                    Db.Users.Include(u => u.Business).ToList().ForEach(u =>
                    {
                        if (u.BusinessId != null)
                        {
                            Db.ReplacePermissions(EntityEnum.Business, u.BusinessId.Value, EntityEnum.User, u.UserId);
                        }
                        Db.ReplacePermissions(EntityEnum.UserType, (long)u.UserTypeId, EntityEnum.User, u.UserId);
                    });

                    Db.Context.SaveChanges();
                }
            }
        }

        static void Release23062014_ProductSpecifications()
        {
            using (var Db = new Repository())
            {
                var ctx = Db.Context;

                string[] data = {

                "CompressorType","100000000","Standard",
                "CompressorType","100000001","Inverter",
                "CompressorType","100000002","Stage",};

                var labels = ctx.ProductSpecificationLabels.ToArray().ToDictionary(l => l.Name);

                for (int i = 0; i < data.Length; i += 3)
                {
                    ProductSpecificationLabel label = null;
                    if (labels.TryGetValue(data[i], out label))
                    {
                        var newLookup = new ProductSpecificationKeyLookup
                        {
                            ProductSpecificationLabelId = label.ProductSpecificationLabelId,
                            Key = data[i + 1],
                            Value = data[i + 2]
                        };
                        ctx.ProductSpecificationKeyLookups.Add(newLookup);
                    }
                }

                Db.SaveChanges();
            }
        }

        static void SubmittalDataGenerate()
        {

            Console.WriteLine("Starting Generating Submittal sheets");

            var lap = DateTime.Now.Ticks;

            using (var service = new ProductServices())
            {
                var products = service.GetProductModelsForSubmittalSheetGeneration();

                var productNumbers = new List<string>();

                foreach (var p in products)
                {
                    var start = DateTime.Now.Ticks;
                    string file = null;

                    try
                    {
                        file = service.GetSubmittalDataFile(p.ProductNumber);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Failed on product " + p.ProductNumber);
                        Console.WriteLine(ex.Message);
                        productNumbers.Add(p.ProductNumber);
                    }

                    var message = (file != null) ? "Generated" : "Not Generated: SDS-DC_" + p.ProductNumber;

                    Console.WriteLine(string.Format("{0} {2} ms : {1}", message, Path.GetFileName(file), new TimeSpan(DateTime.Now.Ticks - start).TotalMilliseconds));
                }

                Console.WriteLine(string.Format("Time taken {0:N2} secs", new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D));
            }

        }

        //LMW added 02/18
        static void SubmittalDataAutoGenerate()
        {
            PurgeAutoSDSfiles();

            Console.WriteLine("Starting Generating Submittal sheets");
            int fileCounter = 0;

            var lap = DateTime.Now.Ticks;
            var service = new ProductServices();
            var products = service.GetProductModelsForSubmittalSheetGeneration();

            List<string> productNumbers = new List<string>();

            foreach (var p in products)
            {
                var start = DateTime.Now.Ticks;
                string file = null;
                try
                {
                    fileCounter = fileCounter + 1;
                    file = service.GetAutoSubmittalDataFile(p.ProductNumber);  //LMW modified service call 02/18
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed on product " + p.ProductNumber);
                    Console.WriteLine(ex.Message);
                    productNumbers.Add(p.ProductNumber);
                }
                string message = (file != null) ? "Generated" : "Not Generated: " + p.ProductNumber;
                Console.WriteLine(string.Format("{0} {2} ms : {1}", message, Path.GetFileName(file), new TimeSpan(DateTime.Now.Ticks - start).TotalMilliseconds));
            }
            Console.WriteLine(string.Format("(" + fileCounter + " files). Time taken {0:N2} secs", new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D));

            createPIMcsvFile();  //create the PIM .CSV file for the PIM Hot Folders

        }

        //LMW added 02/18 
        static void PurgeAutoSDSfiles()
        {
            Console.WriteLine("Purging Submittal Data AuoGen files for next PIM pickup.");

            var elapzed = DateTime.Now.Ticks;
            int fileCounter = 0;

            var PIM_CSVdir = Utilities.Config("dpo.setup.submittaldatasheets.location");

            foreach (var file in System.IO.Directory.GetFiles(PIM_CSVdir, "*.*"))
            {
                var fName = string.Format(Path.GetFileName(file));
                if (System.IO.File.Exists(PIM_CSVdir + @"/" + fName))
                {
                    try
                    {
                        fileCounter = fileCounter + 1;
                        System.IO.File.Delete(PIM_CSVdir + @"/" + fName);
                        Console.WriteLine("Removing: " + fName);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }

                };
            }
            Console.WriteLine(string.Format("(" + fileCounter + ") AuoGen SDS files removed. Time taken {0:N2} secs", new TimeSpan(DateTime.Now.Ticks - elapzed).TotalMilliseconds / 1000D));
        }

        //LMW added 02/18 
        static void createPIMcsvFile()
        {
            // Create the CSV file to associate the SDS-DC pdf files - The files were generated by SubmittalDataAutoGenerate()
            // Note: the CSV file MUST be named exactly as this per PIM inbound process
            var PIM_CSVdir = Utilities.Config("dpo.setup.submittaldatasheets.location");
            string PIM_CSVfile = PIM_CSVdir + @"PIM-Assets upload.csv";

            if (System.IO.File.Exists(PIM_CSVfile))
            {
                System.IO.File.Delete(PIM_CSVfile);
            };

            using (StreamWriter sw = System.IO.File.CreateText(PIM_CSVfile))
            {
                // Note: the first (2) lines of CSV MUST be the header, as follows per PIM inbound process requirements
                sw.WriteLine("Filename,Hierarchy,Product");
                sw.WriteLine("name of the file with extension,Brand of the product,SKU of the product");

                foreach (var file in System.IO.Directory.GetFiles(PIM_CSVdir, "SDS-DC_*.pdf"))
                {
                    var csvData = string.Format(Path.GetFileName(file));
                    int posy = (csvData.Length - 11);
                    string csvData1 = csvData.Substring(7, posy);
                    sw.WriteLine("" + csvData + ",Daikin," + csvData1);
                }
                sw.Close();
            }
            Console.WriteLine("PIM-Assets upload.csv file created.");
        }

        //LMW added 02/18 to delete SDS files on sftp.Goodmanmfg.com
        static void DeleteSFTPfiles()
        {
            Console.WriteLine("Removing SDStransfer files on SFTP server.");
            var elapzed = DateTime.Now.Ticks;

            var hostName = Utilities.Config("dpo.sys.sftp.host");
            var username = Utilities.Config("dpo.sys.sftp.username");
            var password = Utilities.Config("dpo.sys.sftp.password");
            var portString = Utilities.Config("dpo.sys.sftp.port");
            int port;
            if (!Int32.TryParse(portString, out port))
            {
                port = 22; // Default SSH Port              
            }

            using (var sftpk = new SftpClient(hostName, port, username, password))
            {
                try
                {
                    sftpk.Connect();
                    sftpk.KeepAliveInterval = TimeSpan.FromMinutes(3);
                    sftpk.ConnectionInfo.Timeout = TimeSpan.FromMinutes(80);
                    sftpk.OperationTimeout = TimeSpan.FromMinutes(80);
                    sftpk.BufferSize = 3072;
                    Console.WriteLine("Valid SFTP Connection to " + hostName + " is " + sftpk.IsConnected.ToString());

                    string SFTPFolderDest = sftpk.WorkingDirectory.ToString() + "DaikinCity/SDSTransfer";

                    var filezr = sftpk.ListDirectory(SFTPFolderDest);
                    var fileCount2clear = filezr.Count();
                    foreach (var filer2 in filezr)
                    {
                        var localFileName = filer2.FullName;
                        try
                        {
                            sftpk.DeleteFile(localFileName);
                            Console.WriteLine("Deleting " + localFileName);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Deleting " + ex.Message);
                            string errMessage = "SFTP Connection Exeption: " + ex.Message + Environment.NewLine + "StackTrace: " + ex.StackTrace + Environment.NewLine;
                            System.Diagnostics.Trace.TraceError(errMessage);
                        }
                    }
                }
                catch (Exception ex)
                {
                    string errMessage = "SFTP Connection Exeption: " + ex.Message + Environment.NewLine + "StackTrace: " + ex.StackTrace + Environment.NewLine;
                    System.Diagnostics.Trace.TraceError(errMessage);
                }
                if (sftpk.IsConnected)
                {
                    sftpk.Disconnect();
                }
            }
            Console.WriteLine("All SDS files were Deleted on sftp.Goodmanmfg.com/DaikinCity/SDStransfer");
            Console.WriteLine(string.Format("Time taken {0:N2} secs", new TimeSpan(DateTime.Now.Ticks - elapzed).TotalMilliseconds / 1000D));
            //Console.ReadLine();
        }


        //LMW added 02/18 to SFTP SDS files to sftp.Goodmanmfg.com
        static void SFTPtoSDSTransfer()
        {
            Console.WriteLine("Preparing to SFTP Submittal Data Sheets for PIM pickup.");

            var elapzed = DateTime.Now.Ticks;

            createPIMcsvFile();     //create the PIM .CSV file for the PIM Hot Folders - recreate just in case

            DeleteSFTPfiles();      //delete all the files in DaikinCity/SDStransfer on the sftp server first!

            var sdt_sourceDir = Utilities.Config("dpo.setup.submittaldatasheets.location");

            // SFTP all files found in the \websites\DaikinDocuments\Submittal Data AutoGen directory to the sftp.Goodmanmfg.com 
            try
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

                using (var sftp = new SftpClient(hostName, port, username, password))
                {
                    int SDSfilecount = 0;

                    try
                    {
                        sftp.Connect();
                        sftp.KeepAliveInterval = TimeSpan.FromMinutes(3);
                        sftp.ConnectionInfo.Timeout = TimeSpan.FromMinutes(80);
                        sftp.OperationTimeout = TimeSpan.FromMinutes(80);
                        sftp.BufferSize = 3072;
                        Console.WriteLine("Valid SFTP Connection to " + hostName + " is " + sftp.IsConnected.ToString());

                        var srcFilez = Directory.GetFiles(sdt_sourceDir);
                        var sftpPIMdocsDir = Directory.GetFiles(sdt_sourceDir);
                        string sftpSDSDir = sftp.WorkingDirectory.ToString() + "DaikinCity/SDSTransfer";

                        var files = Directory.GetFiles(sdt_sourceDir);

                        foreach (var file in files)
                        {
                            SDSfilecount = SDSfilecount + 1;
                            string localFileName = System.IO.Path.GetFileName(file.ToString());
                            string remoteFileName = file.ToString();
                            string renamedFileName = sftpSDSDir + @"/" + localFileName;

                            using (FileStream fs = new FileStream(remoteFileName, FileMode.OpenOrCreate))
                            {
                                try
                                {
                                    Console.WriteLine("Uploading {0} ({1:N0} bytes)", renamedFileName, fs.Length);
                                    sftp.BufferSize = 4 * 1024;
                                    sftp.UploadFile(fs, renamedFileName);
                                }
                                catch (Exception ex)
                                {
                                    string errMessage = "SFTP Exeption0: " + ex.Message + Environment.NewLine + "StackTrace: " + ex.StackTrace + Environment.NewLine;
                                    Console.WriteLine("SFTP - errMessage0: " + errMessage);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        string errMessage = "SFTP Connection Exeption: " + ex.Message + Environment.NewLine + "StackTrace: " + ex.StackTrace + Environment.NewLine;
                        System.Diagnostics.Trace.TraceError(errMessage);
                    }

                    if (sftp.IsConnected)
                    {
                        sftp.Disconnect();
                    }
                    Console.WriteLine("Total files Copied: " + SDSfilecount);
                }
            }
            catch (Exception ex)
            {
                string errMessage = "Exeption: " + ex.Message + Environment.NewLine + "StackTrace: " + ex.StackTrace + Environment.NewLine;
                System.Diagnostics.Trace.TraceError(errMessage);
            }

            Console.WriteLine("All Submittal Data Sheets and .CSV were copied to sftp.Goodmanmfg.com/DaikinCity/SDStransfer");
            Console.WriteLine(string.Format("Time taken {0:N2} secs", new TimeSpan(DateTime.Now.Ticks - elapzed).TotalMilliseconds / 1000D));
        }


        static void Trace(string msg, List<string> Errors = null, int row = 0)
        {
            var newMsg = (row == 0) ? msg : row + " - " + msg;

            if (Errors != null)
            {
                Errors.Add(newMsg);
            }

            Console.WriteLine(newMsg);
        }

        private static void UploadProjects(List<ProjectModel> projects, Repository Db, List<string> Errors)
        {
            Db.ReadOnly = false;

            using (var projectService = new ProjectServices(Db.Context))
            {
                using (var quoteService = new QuoteServices(Db.Context))
                {
                    //var productService = new ProductServices(Db.Context);
                    var watch = new System.Diagnostics.Stopwatch();

                    ProjectModel p = null;
                    QuoteModel q = null;
                    QuoteItemModel qim = null;
                    var qi = new List<QuoteItem>();

                    for (int i = 0; i < projects.Count; i++)
                    {
                        p = projects[i];

                        watch.Restart();
                        projectService.Response = new ServiceResponse();
                        //projectService.PostModel(p.Owner,p);

                        for (int j = 0; j < p.Quotes.Count; j++)
                        {
                            q = p.Quotes[j];
                            q.ProjectId = p.ProjectId;

                            quoteService.Response = new ServiceResponse();
                            // quoteService.PostModel(p.Owner, q);

                            qi.Clear();
                            for (int k = 0; k < q.Items.Count; k++)
                            {
                                qim = q.Items[k];

                                qi.Add(new QuoteItem
                                {
                                    ProductId = qim.ProductId,
                                    Quantity = (decimal)qim.Quantity
                                });
                            }

                            //quoteService.AdjustQuoteItems(p.Owner, q, qi, true, quoteService.Response.Model as Quote);
                        }

                        watch.Stop();
                        Trace("*** Project: " + p.Name + " - Time taken : " + String.Format("{0:00}:{1:00}:{2:00}.{3:00}", watch.Elapsed.Hours, watch.Elapsed.Minutes, watch.Elapsed.Seconds, watch.Elapsed.Milliseconds / 10), Errors);

                    }
                }
            }
        }
    }
}
