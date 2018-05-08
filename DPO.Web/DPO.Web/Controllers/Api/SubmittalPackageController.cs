using DPO.Common;
using DPO.Domain;
using log4net; 
using System.Collections.Generic;
using System.Linq; 
using System.Web.Http; 
using System.IO.Compression;

namespace DPO.Web.Controllers 
{
    [Authorize]
    public class SubmittalPackageController : BaseApiController
    {
        public ProjectServices projectService;
        public SubmittalPackageServices submittalService;
        public QuoteServices quoteService;
        public HtmlServices htmlService;

        public ILog log;

        public SubmittalPackageController()
        {
            projectService = new ProjectServices();
            quoteService = new QuoteServices();
            htmlService = new HtmlServices();
            submittalService = new SubmittalPackageServices();

            //this.log = Log;
        }
         
        [HttpGet]
        public ServiceResponse GetQuotePackage(long quoteId)
        {
            this.ServiceResponse = new ServiceResponse();

            var model= new SubmittalRequestModel()
            {
                QuoteId = quoteId
            };

            this.ServiceResponse = submittalService.GetQuoteQuotePackage(this.CurrentUser, model);            

            return this.ServiceResponse;

        }
         
        public ServiceResponse QuotePackageCreate(SubmittalRequestModel model)
            {
            //bool chkAllSubmittalSheets = (Request.Form["chkAllSubmittalSheets"] != null);
            //bool chkAllInstallationManuals = (Request.Form["chkAllInstallationManuals"] != null);
            //bool chkAllOperationalManuals = (Request.Form["chkAllOperationalManuals"] != null);
            //bool chkAllGuideSpecs = (Request.Form["chkAllGuideSpecs"] != null);
            //bool chkAllProductBrochures = (Request.Form["chkAllProductBrochures"] != null);

            //bool chkAllRevitDrawing = (Request.Form["chkAllRevitDrawing"] != null);
            //bool chkAllCADDrawing = (Request.Form["chkAllCADDrawing"] != null);
            //bool chkAllProductFlyer = (Request.Form["chkAllProductFlyer"] != null);

            var quotePackage = submittalService.GetQuoteQuotePackage(this.CurrentUser, model).Model as SubmittalRequestModel;
            var currentProject = projectService.GetProjectModel(this.CurrentUser, model.ProjectId).Model as ProjectModel;

            //ViewData["CurrentUser"] = this.CurrentUser;

            var currentProjectNameAsFileName = currentProject.Name;

            //create valid filename out of project name
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                currentProjectNameAsFileName = currentProjectNameAsFileName.Replace(c, '_');
            }

            var coverPageModel = new QuotePackageModel
            {
                Quote = quoteService.GetQuoteModel(this.CurrentUser, model.ProjectId, model.QuoteId).Model as QuoteModel
            };
            coverPageModel.Quote.Project = currentProject;
            coverPageModel.AttachedFiles = quotePackage.QuotePackageAttachedFiles;
            coverPageModel.SelectedDocuments = new List<QuotePackageSelectedItemModel>();

            var documents = new List<string>();

            var existingDocs = new List<string>();

            foreach (var item in quotePackage.Items)
            {
                var itemForCoverPage = new QuotePackageSelectedItemModel
                {
                    ProductNumber = item.ProductNumber
                };

               // var itemForCoverPage = new QuotePackageSelectedItemModel();

                if (item.LineItemTypeId == (byte)LineItemTypeEnum.Configured)
                {
                    itemForCoverPage.ProductNumber = item.CodeString;
                }
                else
                {
                    itemForCoverPage.ProductNumber = item.ProductNumber;
                }

                foreach (var document in item.Documents)
                {
//                    var isSelected = (Request.Form["doc" + document.FileName] + "" != "");
//
//                    if (isSelected == false) isSelected = (chkAllSubmittalSheets && document.DocumentTypeId == (int)DocumentTypeEnum.SubmittalData);
//                    if (isSelected == false) isSelected = (chkAllInstallationManuals && document.DocumentTypeId == (int)DocumentTypeEnum.InstallationManual);
//                    if (isSelected == false) isSelected = (chkAllOperationalManuals && document.DocumentTypeId == (int)DocumentTypeEnum.OperationManual);
//                    if (isSelected == false) isSelected = (chkAllProductBrochures && document.DocumentTypeId == (int)DocumentTypeEnum.ProductBrochure);
//                    if (isSelected == false) isSelected = (chkAllGuideSpecs && document.DocumentTypeId == (int)DocumentTypeEnum.WrittenGuideSpec);
//                    if (isSelected == false) isSelected = (chkAllRevitDrawing && document.DocumentTypeId == (int)DocumentTypeEnum.RevitDrawing);
//                    if (isSelected == false) isSelected = (chkAllCADDrawing && document.DocumentTypeId == (int)DocumentTypeEnum.CADDrawing);
//                    if (isSelected == false) isSelected = (chkAllProductFlyer && document.DocumentTypeId == (int)DocumentTypeEnum.ProductFlyer);
                    var isSelected = false; //temporary fix
                    if (isSelected && !existingDocs.Exists(n => n == document.FileName))
                    {
                        if (item.LineItemTypeId == (byte)LineItemTypeEnum.Configured)
                        {
                            itemForCoverPage.Items.Add((int)document.DocumentTypeId);
                            documents.Add(item.CodeString + "@" + "Configured Submittl Datasheet" + "@" + document.AbsoultePath);
                        }
                        else
                        {
                            itemForCoverPage.Items.Add((int)document.DocumentTypeId);
                            documents.Add(item.ProductNumber + "@" + document.Type + "@" + document.AbsoultePath);
                        }

                        existingDocs.Add(document.FileName);
                    }
                }

                if (itemForCoverPage.Items.Count > 0)
                {
                    coverPageModel.SelectedDocuments.Add(itemForCoverPage);
                }
            }

            //bool chkAllAttachedFiles = (Request.Form["chkAllAttachedFiles"] != null);

            var quotePackageDirectory = Utilities.GetQuotePackageDirectory(model.QuoteId.Value);

            var quotePackageFilename = quotePackageDirectory + Utilities.QuotePackageFileName(model.QuoteId.Value);

            documents = documents.Distinct().ToList();            

            if (documents.Count > 0 || quotePackage.QuotePackageAttachedFiles.Count() > 0)
            {
                var locked = true;

                var lockFile = quotePackageFilename.Replace(".zip", ".lck");

                try
                {

                    lock (htmlService)
                    {
                        locked = (System.IO.File.Exists(lockFile));

                        if (!locked) System.IO.File.Create(lockFile).Close();
                    }

                    if (!locked)
                    {
                        if (System.IO.File.Exists(quotePackageFilename)) System.IO.File.Delete(quotePackageFilename);

                        using (var zip = ZipFile.Open(quotePackageFilename, ZipArchiveMode.Create))
                        {
                            var productNumbers = "";
                            var builder = new System.Text.StringBuilder();
                            builder.Append(productNumbers);

                            foreach (var doc in documents)
                            {
                                var split = doc.Split('@');

                                var productnumber = split[0];

                                var type = split[1];
                                var file = split[2];


                                if (type.ToLower().Contains("submittal"))
                                {
                                    type = "Submittals Sheets";
                                    builder.Append((productNumbers.Length == 0) ? productnumber : ("," + productnumber));
                                }
                                else
                                {
                                    var filename = System.IO.Path.GetFileName(file).Replace("GENERATED_", "");
                                    var fileRef = (type + "\\" + filename);
                                    if (System.IO.File.Exists(file))
                                    {
                                        zip.CreateEntryFromFile(file, fileRef, CompressionLevel.Optimal);
                                    }
                                }
                            }
                            productNumbers = builder.ToString();

                            if (productNumbers.Length > 0)
                            {
                                var sdsfile = quotePackageDirectory + "DPO_QuotePackage_SubmittalDataSheets.pdf";

                                var pdf = new PdfConvertor();
                                var productService = new ProductServices();

                                foreach (var productNumber in productNumbers.Split(','))
                                {
                                    //add product tags, project info and user info to header of each submittal sheet
                                    //if no specific template type if given, add in the external submittal sheet(if it exists)
                                    var product = quotePackage.Items.Where(x => x.ProductNumber == productNumber).FirstOrDefault();

                                    if (product.GetSubmittalSheetTemplateName != "SubmittalTemplate")
                                    {
                                        var file = productService.GenerateSubmittalDataFileForPackage(productNumber, product.QuoteItemId, currentProject.ProjectId);

                                        if (file != null)
                                        {
                                            pdf.AppendHtml(file);
                                        }
                                    }
                                    else
                                    {
                                        var submittalDocument = product.Documents.Where(d => d.DocumentTypeId == (int)DocumentTypeEnum.SubmittalData).FirstOrDefault();

                                        var fullFile = Utilities.GetSubmittalDirectory() + submittalDocument.FileName + @".pdf";

                                        if (System.IO.File.Exists(fullFile))
                                        {
                                            pdf.AppendFile(fullFile);
                                        }
                                    }
                                }

                                pdf.WriteToFile(sdsfile);

                                if (System.IO.File.Exists(sdsfile))
                                {
                                    zip.CreateEntryFromFile(sdsfile, "Submittals Sheets\\SDS_" + currentProjectNameAsFileName + ".pdf", CompressionLevel.Optimal);
                                }
                            }

                            foreach (var doc in quotePackage.QuotePackageAttachedFiles.ToList())
                            {
                                var fullFile = quotePackageDirectory + doc.FileName;

                                if (System.IO.File.Exists(fullFile))
                                {
                                    var filename = System.IO.Path.GetFileName(fullFile);

                                    //bool isSelected = (Request.Form["doc" + doc.FileName] + "" != "");

                                    //if (isSelected == false) isSelected = (chkAllAttachedFiles && doc.DocumentTypeId == (int)DocumentTypeEnum.QuotePackageAttachedFile);

                                    //f (isSelected)
                                    //{
                                        zip.CreateEntryFromFile(fullFile, "AttachedFiles\\" + filename, CompressionLevel.Optimal);
                                    //}
                                }
                            }
                        }

                        //var coverPageFile = projectService.GenerateQuotePackageCoverPageFile((long)model.QuoteId, base.RenderView(this, "QuotePackageCoverPage", coverPageModel));

                       // if (coverPageFile != null)
                       // {
                            using (var zip = ZipFile.Open(quotePackageFilename, ZipArchiveMode.Update))
                            {
                                //zip.CreateEntryFromFile(coverPageFile, "CoverSheet_" + currentProjectNameAsFileName + ".pdf", CompressionLevel.Optimal);
                            }
                        //}

                    }

                   // this.Response.ContentType = MimeMapping.GetMimeMapping(quotePackageFilename);
                    //Response.AddHeader("Content-Disposition", String.Format("attachment;filename=\"{0}\"", "QuotePackage_" + currentProjectNameAsFileName + ".zip"));
                    //Response.TransmitFile(quotePackageFilename);

                    return null;
                }
                finally
                {
                    lock (htmlService)
                    {
                        if (!locked)
                        {
                            System.IO.File.Delete(lockFile);
                        }

                    }
                }
            }

            // return base.RedirectToAction("QuotePackage", new { ProjectId = model.ProjectId, QuoteId = model.QuoteId });
            return null;
        }
    }
}