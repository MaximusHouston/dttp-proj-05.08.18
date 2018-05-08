using DPO.Common;
using DPO.Data;
using DPO.Domain.Services;
using DPO.Services.PIM.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Renci.SshNet;
using System.IO;
using log4net;
using System.IO.Compression;

namespace DPO.Services.PIM
{
    public class Program
    { 
        public static ILog sLog = LogManager.GetLogger(typeof(Program));

        static Dictionary<string, Product> GetCurrentProducts()
        {
            Dictionary<string, Product> currentProducts;
            using (Repository repo = new Repository())
            {
                currentProducts = repo.GetCurrentProducts();
            }

            return currentProducts;
        }

        static void Main(string[] args)
        {
            // Configure logging
            log4net.Config.XmlConfigurator.Configure();

            Dictionary<string, Product> currentProducts = GetCurrentProducts();
            bool parmsPassed = true;

            if (args.Length > 0 && !String.IsNullOrWhiteSpace(args[0]))
            {
                switch (args[0].ToLower())
                {
                    case "importallproductdata":
                        ImportAllProductData(currentProducts);
                        break;
                    default:
                        parmsPassed = false;
                        break;
                }
            }
            else
            {
                parmsPassed = false;
            }

            if (!parmsPassed)
            {
                Console.WriteLine("No arguments used");
            }
        }

        static void LogException(Exception ex, string msg = null)
        {
            StringBuilder sb = new StringBuilder();

            if (!String.IsNullOrWhiteSpace(msg))
            {
                sb.Append(msg).Append(Environment.NewLine);
            }

            if (ex != null)
            {
                sb.AppendFormat("{0} - {1}{2}{2}", ex.Message, ex.StackTrace, Environment.NewLine);
            }
            while ((ex = ex.InnerException) != null)
            {
                sb.AppendFormat("{0} - {1}{2}{2}", ex.Message, ex.StackTrace, Environment.NewLine);
            }

            sLog.Error(sb.ToString());
        }

        static XDocument LoadExportXml()
        {
            XDocument doc = null;
            PIMServices ps = new PIMServices();

            try
            {
                using (SftpClient sftp = ps.CreatePIMSftp())
                {
                    sftp.Connect();

                    var directory = "/" + ps.GetPIMExportDirectory();
                    var allFiles = sftp.ListDirectory(directory);

                    // Load expected files
                    var filesToSearch = (from file in allFiles
                                        where !String.IsNullOrWhiteSpace(file.Name)
                                            && file.Name.Contains(ps.GetPIMZipFileName())
                                        orderby file.LastWriteTime ascending
                                        select file).ToList();

                    // Remove old files
                    if (filesToSearch.Count > 1)
                    {
                        for (int i = 0; i < filesToSearch.Count - 1; i++)
                        {
                            var file = filesToSearch[i];
                            sftp.DeleteFile(file.FullName);
                        }
                    }

                    // Parse file (should now be only one file)
                    foreach(var file in filesToSearch)
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            sftp.DownloadFile(file.FullName, ms);
                            ms.Position = 0;

                            Stream unzippedMs;

                            // TODO:  Add zip capability
                            ZipArchive archive = new ZipArchive(ms);
                            foreach (ZipArchiveEntry entry in archive.Entries)
                            {
                                if (entry.FullName.IndexOf(ps.GetPIMExportFileName(), StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    unzippedMs = entry.Open(); // .Open will return a stream
                                                               //Process entry data here
                                    doc = XDocument.Load(unzippedMs);
                                }
                            }

                        }
                    }

                    sftp.Disconnect();
                }

            }
            catch (Exception ex)
            {
                LogException(ex);
                Console.WriteLine("Unable to load file");
            }

            return doc;
        }

        static void ImportAllProductData(Dictionary<string, Product> currentProducts)
        {
            XDocument doc = LoadExportXml();
            if (doc == null)
            {
                Console.WriteLine("Unable to load PIM Export XML");
                return;
            }

            long lap = 0;

            PIMParser pp = new PIMParser(doc);

            var lovs = pp.GetListOfValues();
            var pimProds = pp.GetProducts();
            var attrs = pp.GetAttributes();
            var assets = pp.GetAssets();

            // HACK:  Uncomment this
            Console.WriteLine("Importing LOVS");
            LoadListOfValues(lovs, attrs, pimProds);

            Console.WriteLine("Importing products");
            LoadProducts(pimProds, currentProducts);

            // Reload latest products
            Console.WriteLine("Reloading Current Daikin City Products");
            lap = DateTime.Now.Ticks;
            using (Repository repo = new Repository())
            {
                currentProducts = repo.GetCurrentProducts();
            }
            Console.WriteLine(string.Format("-- Time taken {0:N4} secs", new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D));

            Console.WriteLine("Import Documents and Product Documents");
            LoadDocuments(pimProds, assets, currentProducts);
  
            Console.WriteLine("Cleaning up old product information");
            CleanupProducts(pimProds, currentProducts);
        }

        static void LoadListOfValues(Dictionary<string, PIMListOfValue> lovs, 
            Dictionary<string, PIMAttribute> attributes, List<PIMProduct> pimProds)
        {
            PIMServices ps = new PIMServices();

            // Identify all attributes with list of values
            var qryLovs = from lov in lovs
                          where lov.Value != null
                          select lov.Value;

            long lap = 0;

            Console.WriteLine("-- Importing Mapped List of Values");
            lap = DateTime.Now.Ticks;
            ps.ImportLookupTablesMapped(qryLovs.ToDictionary(x => x.ID));
            Console.WriteLine(string.Format("-- Time taken {0:N4} secs", new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D));

            Console.WriteLine("-- Importing Unmapped List of Values");
            lap = DateTime.Now.Ticks;
            ps.ImportProductSpecificationLabelAndKeyLookup(attributes);
            Console.WriteLine(string.Format("-- Time taken {0:N4} secs", new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D));

            Console.WriteLine("-- Importing Custom List Of Values");
            lap = DateTime.Now.Ticks;
            ps.ImportLookupTablesCustom(pimProds);
            Console.WriteLine(string.Format("-- Time taken {0:N4} secs", new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D));
        }

        static void LoadProducts(List<PIMProduct> pimProds, Dictionary<string, Product> currentProducts)
        {
            PIMServices ps = new PIMServices();

            long lap = 0;

            // HACK:  Uncomment this
            Console.WriteLine("Import Products and Specifications");
            lap = DateTime.Now.Ticks;
            ps.ImportProducts(pimProds, currentProducts);
            Console.WriteLine(string.Format("Time taken {0:N4} secs", new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D));

            Console.WriteLine("Import Product Notes");
            lap = DateTime.Now.Ticks;
            ps.ImportProductNotes(pimProds, currentProducts);
            Console.WriteLine(string.Format("Time taken {0:N4} secs", new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D));

            Console.WriteLine("Import Product Components");
            lap = DateTime.Now.Ticks;
            ps.ImportProductComponents(pimProds, currentProducts);
            Console.WriteLine(string.Format("Time taken {0:N4} secs", new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D));

            Console.WriteLine("Starting Database Maintenance Routines");
            lap = DateTime.Now.Ticks;
            ps.RunDatabaseMaintenanceRoutines();
            Console.WriteLine(string.Format("Time taken {0:N4} secs", new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D));

        }

        static void LoadDocuments(List<PIMProduct> pimProds, Dictionary<string, PIMAsset> assets, Dictionary<string, Product> currentProducts)
        {
            PIMServices ps = new PIMServices();
            long lap = 0;

            Console.WriteLine("Import Product Documents");
            lap = DateTime.Now.Ticks;
            ps.ImportProductDocuments(pimProds, currentProducts);
            Console.WriteLine(string.Format("Time taken {0:N4} secs", new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D));
        }

        static void CleanupProducts(List<PIMProduct> pimProducts, Dictionary<string, Product> currentProducts)
        {
            PIMServices ps = new PIMServices();
            long lap = 0;

            Console.WriteLine("-- Disabling Old Products");
            lap = DateTime.Now.Ticks;
            ps.DisableProducts(pimProducts, currentProducts);
            Console.WriteLine(string.Format("-- Time taken {0:N4} secs", new TimeSpan(DateTime.Now.Ticks - lap).TotalMilliseconds / 1000D));

        }
    }
}
