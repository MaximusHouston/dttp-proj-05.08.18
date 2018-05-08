using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DPO.Common;
using DPO.Domain; 
using System.IO; 
using CsvHelper; 
using Newtonsoft.Json;
using log4net;

namespace DPO.Web.Controllers
{
    public partial class ProjectDashboardController
    {
        
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ActionResult QuoteItemRemove(QuoteItemModel model)
        {
            log.InfoFormat("QuoteItemRemove Action staring...");
            log.Debug("expect parameter: QuoteItemModel");
            log.Debug("QuoteItemCount: " + model.TotalRecords);
            log.Debug("Quantity: " + model.Quantity + 
                      " NetPrice: " + model.NetPrice + 
                      " Multiplier: " + model.Multiplier);
            log.Debug("ListPrice " + model.ListPrice + " ExtendedNetPrice: " + model.ExtendedNetPrice);
            log.Debug("start remove Quote Items...");

            this.ServiceResponse = quoteService.QuoteItemRemove(this.CurrentUser, model);

            ProcessServiceResponse(this.ServiceResponse);

            foreach( var message in this.ServiceResponse.Messages.Items)
            {
                log.Info(message);
            }

            log.Info("Finish remove Quote Items");
            log.Debug("Quote item after remove: " + model.TotalRecords);
            
            return RedirectToAction("QuoteItems", new { ProjectId = model.ProjectId, QuoteId = model.QuoteId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ActionResult ProjectQuotesModelDeleteQuote(QuoteModel model)
        {
            this.ServiceResponse = quoteService.Delete(this.CurrentUser, model, true);

            ProcessServiceResponse(this.ServiceResponse);

            return ProjectQuotes(new ProjectQuotesModel { ProjectId = model.ProjectId });
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ActionResult Quote(long? projectId, long? quoteId, bool? showImportProductPopup, bool closePopup = false)
        {
            this.ServiceResponse = quoteService.GetQuoteModel(this.CurrentUser, projectId, quoteId);

            var model = this.ServiceResponse.Model as QuoteModel;

            bool  showCommissionConvertPopup = false;

            if (ProcessServiceResponse(this.ServiceResponse))
            {
               //if (model.TotalList > Convert.ToDecimal(WebConfigurationManager.AppSettings["dpo.sales.commission.default.totalList"]))
               //{
               //  if(!model.IsCommission && model.CommissionConvertNo == false)
               //  {
               //      if (closePopup != true)
               //      {
               //          showCommissionConvertPopup = true;
               //      }
               //  }
               //}

               this.RouteData.Values["action"] = "Quote";
            }

            model.ShowCommissionConvertPopup = showCommissionConvertPopup;

            model.ShowImportProductPopup = showImportProductPopup ?? false;

            return ((this.IsPostRequest) ? (ViewResultBase)PartialView("Quote", model) : View("Quote", model));

        }

        [Authorise(Accesses = new[] {  SystemAccessEnum.ViewProject})]
        //public ActionResult UpdateQuoteNotesForCommission(QuoteModel model)
        //{
        //    var QuoteNotes = model.Notes;
        //    var CommissionConvertNo = model.CommissionConvertNo;
        //    var CommissionConvertYes = model.CommissionConvertYes;

        //    this.ServiceResponse = quoteService.GetQuoteModel(this.CurrentUser, model.ProjectId, model.QuoteId);

        //    var quoteModel = this.ServiceResponse.Model as QuoteModel;
            
        //    if(ProcessServiceResponse(this.ServiceResponse))
        //    {
        //        quoteModel.Notes = QuoteNotes;
        //        quoteModel.CommissionConvertNo = CommissionConvertNo;
        //        quoteModel.CommissionConvertYes = CommissionConvertYes;
        //    }

        //    this.ServiceResponse = quoteService.PostModel(this.CurrentUser, quoteModel);

        //    if( ProcessServiceResponse(this.ServiceResponse))
        //    {
        //        return ((this.IsPostRequest) ? (ViewResultBase)PartialView("Quote", this.ServiceResponse.Model as QuoteModel) : View("Quote", this.ServiceResponse.Model as QuoteModel));
        //    }

        //   return new EmptyResult();
        //}

        public ActionResult UpdateQuoteNotesForCommission(long projectId, long quoteId, string notes, bool commissionConvertYes, bool commissionConvertNo)
        {
            var QuoteNotes = notes;
            var CommissionConvertNo = commissionConvertNo;
            var CommissionConvertYes = commissionConvertYes;

            this.ServiceResponse = quoteService.GetQuoteModel(this.CurrentUser, projectId, quoteId);

            var quoteModel = this.ServiceResponse.Model as QuoteModel;

            if (ProcessServiceResponse(this.ServiceResponse))
            {
                quoteModel.Notes += System.Environment.NewLine + QuoteNotes;
                quoteModel.CommissionConvertNo = CommissionConvertNo;
                quoteModel.CommissionConvertYes = CommissionConvertYes;
                if(commissionConvertYes == true)
                {
                    quoteModel.IsCommission = commissionConvertYes;
                }
            }

            this.ServiceResponse = quoteService.PostModel(this.CurrentUser, quoteModel);

            var model = this.ServiceResponse.Model as QuoteModel;

            if (ProcessServiceResponse(this.ServiceResponse))
            {
                if( this.IsPostRequest )
                {
                    //return (ViewResultBase)PartialView("QuoteItems", this.ServiceResponse.Model as QuoteModel);

                    return RedirectToAction("QuoteItems", "Projectdashboard", new QuoteItemsModel { ProjectId = model.ProjectId, QuoteId = model.QuoteId });
                }
                else
                {
                   return  View("Quote", this.ServiceResponse.Model as QuoteModel);
                }
                
            }

            return new EmptyResult();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ActionResult QuoteDelete(QuoteModel model)
        {
            this.ServiceResponse = quoteService.Delete(this.CurrentUser, model, true);

            ProcessServiceResponse(this.ServiceResponse);

            return AJAXRedirectTo("ProjectQuotes", "Projectdashboard", new { ProjectId = model.ProjectId });
        }

        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject, SystemAccessEnum.RequestDiscounts })]
        public ActionResult QuoteDiscountRequests(QuoteItemsModel model)
        {
            model.LoadDiscountRequests = true;

            this.ServiceResponse = quoteService.GetQuoteItemsModel(this.CurrentUser, model);

            model = this.ServiceResponse.Model as QuoteItemsModel;

            if (ProcessServiceResponse(this.ServiceResponse))
            {

                this.Session["ImportProjectId"] = model.ProjectId;

                this.Session["ImportQuoteId"] = model.QuoteId;
            }

            this.RouteData.Values["action"] = "QuoteDiscountRequests";

            return ((this.IsPostRequest) ? (ViewResultBase)PartialView("QuoteDiscountRequests", model) : View("QuoteDiscountRequests", model));
        }

        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject, SystemAccessEnum.RequestCommission })]
        public ActionResult QuoteCommissionRequests(QuoteItemsModel model)
        {
            model.LoadCommissionRequests = true;

            this.ServiceResponse = quoteService.GetQuoteItemsModel(this.CurrentUser, model);

            model = this.ServiceResponse.Model as QuoteItemsModel;

            if (ProcessServiceResponse(this.ServiceResponse))
            {

                this.Session["ImportProjectId"] = model.ProjectId;

                this.Session["ImportQuoteId"] = model.QuoteId;
            }

            this.RouteData.Values["action"] = "QuoteCommissionRequests";

            return ((this.IsPostRequest) ? (ViewResultBase)PartialView("QuoteCommissionRequests", model) : View("QuoteCommissionRequests", model));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ActionResult QuoteDuplicate(QuoteModel model)
        {
            this.ServiceResponse = quoteService.Duplicate(this.CurrentUser, model);

            ProcessServiceResponse(this.ServiceResponse);

            //return AJAXRedirectTo("ProjectQuotes", "Projectdashboard", new { ProjectId = model.ProjectId });

            //return RedirectToAction("ProjectQuotes", new { ProjectId = model.ProjectId });
            return Redirect("/v2/#/projectQuotes/" + model.ProjectId);
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ActionResult QuoteDuplicate(long QuoteId, long projectId)
        {
            this.ServiceResponse = quoteService.GetQuoteModel(this.CurrentUser, projectId, QuoteId);
            QuoteModel quoteModel = this.ServiceResponse.Model as QuoteModel;

            return QuoteDuplicate(quoteModel);
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ActionResult QuoteEdit(long? projectId, long? quoteId)
        {
            if (!projectId.HasValue) return RedirectToAction("Projects");

            this.ServiceResponse = quoteService.GetQuoteModel(this.CurrentUser, projectId.Value, quoteId);

            ProcessServiceResponse(this.ServiceResponse);
      
            return View("QuoteEdit", this.ServiceResponse.Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ActionResult QuoteEdit(QuoteModel model, string IsCommission)
        {
            
            //string baseUrl = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~"));

            if (IsCommission == "true")
            {
                model.IsCommission = true;
            }
            else
            {
                model.IsCommission = false;
            }

            //start logging
            log.Info("enter QuoteEdit Method");
            log.Debug("Title: " + model.Title + "IsCommission: " + model.IsCommission);
            log.Info("QuoteEdit Method starting...");

            this.ServiceResponse = quoteService.PostModel(this.CurrentUser, model);

            model = this.ServiceResponse.Model as QuoteModel;

            foreach(var message in this.ServiceResponse.Messages.Items)
            {
                if(this.ServiceResponse.HasError)
                {
                    log.Error(message.Text);
                }
                else
                {
                    log.Debug(message.Text);
                }
            }

            if (ProcessServiceResponse(this.ServiceResponse))
            {
                if (!quoteService.NewRecordAdded)
                {
                    log.Debug("Quote Edit Completed.return to Quote View");
                    //return AJAXRedirectTo("Quote", "Projectdashboard", new { ProjectId = model.ProjectId, QuoteId = model.QuoteId }); // Go back if editing

                    string url = "/v2/#/quote/" + model.QuoteId + "/existingRecord";
                    return Redirect(url);
                }
                else {

                    model.NewRecordAdded = true;
                    string url = "/v2/#/quote/" + model.QuoteId + "/newRecord";
                    return Redirect(url);
                    
                }

                //model.NewRecordAdded = true;
            }

            log.Info("Exit QuoteEdit Method");
            log.Debug("Quote Created Complete.Return to QuoteEdit View");

            return PartialView("QuoteEdit", this.ServiceResponse.Model);
        }

        //public HttpWebResponse QuoteV2(long quoteId) {
        //    HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create("http://localhost:62801/api/Quote/Quote?quoteId=" + quoteId);
        //    httpRequest.Method = "GET";
        //    return (HttpWebResponse)httpRequest.GetResponse();
        //}
        

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ActionResult QuoteImport(FormCollection formCollection, QuoteModel model)
        {
            log.InfoFormat("Enter QuoteImport Action: {0}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            this.ServiceResponse = new ServiceResponse();

            if (Request != null && Request.Files.Count == 1)
            {
                log.Debug("File import count is greater than 1.get the First file from file list");

                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    log.Debug("File length is greater than 0");
                    log.Debug("get file path extension");

                    if (Path.GetExtension(file.FileName).ToLower() == ".xls" || Path.GetExtension(file.FileName).ToLower() == ".xlsx" || Path.GetExtension(file.FileName).ToLower() == ".csv")
                    {
                        log.Debug("excel file");

                        var csvReader = new StreamReader(file.InputStream);
                        var csv = new CsvHelper.CsvReader(csvReader);

                        log.Info("ImportsProductsFromCSV() executing..");
                        this.ServiceResponse = quoteService.ImportProductsFromCSV(this.CurrentUser, csv, model);
                        log.Info("ImportsProductsFromCSV() finished");
                    }
                    //uncommment these line when go to VRV-Express branch
                    else if (Path.GetExtension(file.FileName).ToLower() == ".xml")
                    {
                        log.Debug("xml file");
                        log.Debug("ImportProductsFromXml() executing...");
                        this.ServiceResponse = quoteService.ImportProductsFromXml(this.CurrentUser, file, model);
                        log.Debug("ImportProductsFromXml finished");
                    }
                    else
                    {
                        this.ServiceResponse.Messages.AddError(Resources.ResourceUI.InvalidFile);
                        log.Error(this.ServiceResponse.Messages.Items.Last()); // get the latest error message
                    }
                }
            }

            var qim = new QuoteItemsModel { ProjectId = model.ProjectId.Value, QuoteId = model.QuoteId.Value, LoadQuoteItems = true };

            log.Debug("execute GetQuoteItemsModel");
            this.ServiceResponse.Model = quoteService.GetQuoteItemsModel(this.CurrentUser, qim).Model as QuoteItemsModel;
            log.Debug("GetQuoteItemsModel finished");

            this.ShowKeyMessagesOnPage = true;

            ProcessServiceResponse(this.ServiceResponse);

            if (this.ServiceResponse.HasError)
            {
                foreach(var message in this.ServiceResponse.Messages.Items)
                {
                    log.Error(message.Text);
                }
            }
            else
            {
                foreach (var message in this.ServiceResponse.Messages.Items)
                {
                    log.Debug(message.Text);
                }
            }

            this.RouteData.Values["action"] = "QuoteItems";
            this.RouteData.Values["id"] = model.ProjectId + "/" + model.QuoteId; // this line use to fixed the paging issues after imports

            //return View("QuoteItems", this.ServiceResponse.Model);

            log.Info("Finished QuoteImport Action");
            log.Info("Return to QuoteItems View");

            return base.RedirectToAction("QuoteItems", new { ProjectId = model.ProjectId, QuoteId = model.QuoteId });
        }

        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ActionResult QuoteImportItemsFromBasket(QuoteModel model)
        {
            this.ServiceResponse = quoteService.ImportProductsFromBasket(this.CurrentUser, model);

            ProcessServiceResponse(this.ServiceResponse);

            return QuoteItems(new QuoteItemsModel { QuoteId = model.QuoteId });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ActionResult QuoteItemAdjustments(QuoteModel model, string quoteItemAdjustments, UserSessionModel user)
        {
            this.ServiceResponse = quoteService.QuoteItemAdjustments(this.CurrentUser, model, quoteItemAdjustments );

            ProcessServiceResponse(this.ServiceResponse);

            return base.RedirectToAction("QuoteItems", new { ProjectId = model.ProjectId, QuoteId = model.QuoteId });
        }

        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ActionResult QuoteItems(QuoteItemsModel model)
        {
            model.LoadQuoteItems = true;

            this.ServiceResponse = quoteService.GetQuoteItemsModel(this.CurrentUser, model);

            model = this.ServiceResponse.Model as QuoteItemsModel;
            
            bool showCommissionConvertPopup = false;

            if (ProcessServiceResponse(this.ServiceResponse))
            {

                this.Session["ImportProjectId"] = model.ProjectId;

                this.Session["ImportQuoteId"] = model.QuoteId;

                //if(model.ActiveQuoteSummary.TotalList > Convert.ToDecimal(WebConfigurationManager.AppSettings["dpo.sales.commission.default.totalList"]))
                //{
                //    if(!model.IsCommission && model.CommissionConvertNo == false)
                //    {
                //        showCommissionConvertPopup = true;
                //    }
                //}
            }

            this.RouteData.Values["action"] = "QuoteItems";

            model.ShowCommissionConvertPopup = showCommissionConvertPopup;

            return ((this.IsPostRequest) ? (ViewResultBase)PartialView("QuoteItems", model) : View("QuoteItems", model));
        }

        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ActionResult QuoteItemsWithProductsHasNoClassCode(QuoteItemsModel model)
        {
            model.LoadQuoteItems = true;

            this.ServiceResponse = quoteService.GetQuoteItemsModel(this.CurrentUser, model);

            model = this.ServiceResponse.Model as QuoteItemsModel;

            var listOfProductsWithNoClassCode = new List<string>();

            foreach(var item in model.Items)
            {
                if(item.ProductClassCode == string.Empty)
                {
                    listOfProductsWithNoClassCode.Add(item.ProductNumber);
                }
            }

            foreach(string productNumber in listOfProductsWithNoClassCode)
            {
                this.ServiceResponse.AddError(productNumber, string.Format(Resources.ResourceModelProject.MP142, productNumber));
            }

            QuoteModel quoteModel = quoteService.GetQuoteModel(this.CurrentUser, model.ProjectId, model.QuoteId).Model as QuoteModel;

            ViewData["PageMessages"] = this.ServiceResponse.Messages as Messages;

            if (this.IsPostRequest)
            {
                return (ViewResultBase)PartialView("QuoteItems", model);
            }
            
            if(this.ServiceResponse.IsOK == false)
            {
                return View("QuoteItems", model);
            }

            return RedirectToAction("OrderForm", new { quoteModel.ProjectId, quoteModel.QuoteId });
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public string QuoteItemTags(long QuoteId, long QuoteItemId)
        {
            var items = quoteService.GetQuoteItemListModel(this.CurrentUser, QuoteId).Model as List<QuoteItemListModel>;
            var model = items.Where(i => i.QuoteItemId == QuoteItemId).FirstOrDefault();
            var str = JsonConvert.SerializeObject(new { tags = model.Tags }, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
            return str;
        }

        //[Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        //public ActionResult QuotePackage(QuoteItemsModel model)
        //{
        //    this.ServiceResponse = new ServiceResponse();

        //    this.ServiceResponse.Model = model;

        //    this.ServiceResponse = quoteService.GetQuoteQuotePackage(this.CurrentUser, model);

        //    model = this.ServiceResponse.Model as QuoteItemsModel;

        //    return View("QuotePackage", this.ServiceResponse.Model);
        //}

        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ActionResult QuotePackageAttachFile(FormCollection formCollection, QuoteItemsModel model)
        {
            this.ServiceResponse = new ServiceResponse();

            if (Request != null && Request.Files.Count == 1 && model.QuoteId.HasValue)
            {
                var filebase = Request.Files[0];

                var message = Utilities.SavePostedFile(filebase, Utilities.GetQuotePackageDirectory(model.QuoteId.Value), 25000);

                if (message != null)
                {
                    this.ServiceResponse.AddError(message);
                }
                else
                {
                    this.ServiceResponse.AddSuccess(string.Format("File {0} attached", filebase));
                }
            }

            return base.RedirectToAction("QuotePackage", new { ProjectId = model.ProjectId, QuoteId = model.QuoteId });
        }

        //[Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        //public ActionResult QuotePackageCreate(QuoteItemsModel model)
        //{
        //    bool chkAllSubmittalSheets = (Request.Form["chkAllSubmittalSheets"] != null);
        //    bool chkAllInstallationManuals = (Request.Form["chkAllInstallationManuals"] != null);
        //    bool chkAllOperationalManuals = (Request.Form["chkAllOperationalManuals"] != null);
        //    bool chkAllGuideSpecs = (Request.Form["chkAllGuideSpecs"] != null);
        //    bool chkAllProductBrochures = (Request.Form["chkAllProductBrochures"] != null);

        //    bool chkAllRevitDrawing = (Request.Form["chkAllRevitDrawing"] != null);
        //    bool chkAllCADDrawing = (Request.Form["chkAllCADDrawing"] != null);
        //    bool chkAllProductFlyer = (Request.Form["chkAllProductFlyer"] != null);

        //    var quotePackage = quoteService.GetQuoteQuotePackage(this.CurrentUser, model).Model as QuoteItemsModel;
        //    var currentProject = projectService.GetProjectModel(this.CurrentUser, model.ProjectId).Model as ProjectModel;

        //    ViewData["CurrentUser"] = this.CurrentUser;

        //    string currentProjectNameAsFileName = currentProject.Name;

        //    //create valid filename out of project name
        //    foreach (char c in System.IO.Path.GetInvalidFileNameChars())
        //    {
        //        currentProjectNameAsFileName = currentProjectNameAsFileName.Replace(c, '_');
        //    }

        //    var coverPageModel = new QuotePackageModel
        //    {
        //        Quote = quoteService.GetQuoteModel(this.CurrentUser, model.ProjectId, model.QuoteId).Model as QuoteModel
        //    };
        //    coverPageModel.Quote.Project = currentProject;
        //    coverPageModel.AttachedFiles = quotePackage.QuotePackageAttachedFiles;
        //    coverPageModel.SelectedDocuments = new List<QuotePackageSelectedItemModel>();

        //    var documents = new List<string>();

        //    var existingDocs = new List<string>();

        //    foreach (var item in quotePackage.Items)
        //    {
        //        //QuotePackageSelectedItemModel itemForCoverPage = new QuotePackageSelectedItemModel
        //        //{
        //        //    ProductNumber = item.ProductNumber
        //        //};

        //        var itemForCoverPage = new QuotePackageSelectedItemModel();

        //        if (item.LineItemTypeId == (byte)LineItemTypeEnum.Configured) {
        //            itemForCoverPage.ProductNumber = item.CodeString;
        //        } else {
        //            itemForCoverPage.ProductNumber = item.ProductNumber;
        //        }

        //        foreach (var document in item.Documents)
        //        {
        //            var isSelected = (Request.Form["doc" + document.FileName] + "" != "");

        //            if (isSelected == false) isSelected = (chkAllSubmittalSheets && document.DocumentTypeId == (int)DocumentTypeEnum.SubmittalData);
        //            if (isSelected == false) isSelected = (chkAllInstallationManuals && document.DocumentTypeId == (int)DocumentTypeEnum.InstallationManual);
        //            if (isSelected == false) isSelected = (chkAllOperationalManuals && document.DocumentTypeId == (int)DocumentTypeEnum.OperationManual);
        //            if (isSelected == false) isSelected = (chkAllProductBrochures && document.DocumentTypeId == (int)DocumentTypeEnum.ProductBrochure);
        //            if (isSelected == false) isSelected = (chkAllGuideSpecs && document.DocumentTypeId == (int)DocumentTypeEnum.WrittenGuideSpec);
        //            if (isSelected == false) isSelected = (chkAllRevitDrawing && document.DocumentTypeId == (int)DocumentTypeEnum.RevitDrawing);
        //            if (isSelected == false) isSelected = (chkAllCADDrawing && document.DocumentTypeId == (int)DocumentTypeEnum.CADDrawing);
        //            if (isSelected == false) isSelected = (chkAllProductFlyer && document.DocumentTypeId == (int)DocumentTypeEnum.ProductFlyer);

        //            if (isSelected && !existingDocs.Exists(n => n == document.FileName))
        //            {
        //                if (item.LineItemTypeId == (byte)LineItemTypeEnum.Configured) {
        //                    itemForCoverPage.Items.Add((int)document.DocumentTypeId);
        //                    documents.Add(item.CodeString + "@" + "Configured Submittl Datasheet" + "@" + document.AbsoultePath);
        //                } else {
        //                    itemForCoverPage.Items.Add((int)document.DocumentTypeId);
        //                    documents.Add(item.ProductNumber + "@" + document.Type + "@" + document.AbsoultePath);
        //                }

        //                existingDocs.Add(document.FileName);
                        
        //            }
        //        }

        //        if (itemForCoverPage.Items.Count > 0)
        //        {
        //            coverPageModel.SelectedDocuments.Add(itemForCoverPage);
        //        }
        //    }

        //    bool chkAllAttachedFiles = (Request.Form["chkAllAttachedFiles"] != null);

        //    var quotePackageDirectory = Utilities.GetQuotePackageDirectory(model.QuoteId.Value);

        //    var quotePackageFilename = quotePackageDirectory + Utilities.QuotePackageFileName(model.QuoteId.Value);

        //    documents = documents.Distinct().ToList();
        //    //documents = documents.GroupBy(i => i.).Select(i => i.First()).ToList();
        //    //finalItemsList = finalItemsList.GroupBy(i => i.ProductId).Select(i => i.First()).ToList();

        //    if (documents.Count > 0 || quotePackage.QuotePackageAttachedFiles.Count() > 0)
        //    {
        //        var locked = true;

        //        var lockFile = quotePackageFilename.Replace(".zip", ".lck");

        //        try
        //        {

        //            lock (htmlService)
        //            {
        //                locked = (System.IO.File.Exists(lockFile));

        //                if (!locked) System.IO.File.Create(lockFile).Close();
        //            }

        //            if (!locked)
        //            {
        //                if (System.IO.File.Exists(quotePackageFilename)) System.IO.File.Delete(quotePackageFilename);

        //                using (var zip = ZipFile.Open(quotePackageFilename, ZipArchiveMode.Create))
        //                {
        //                    var productNumbers = "";

        //                    foreach (var doc in documents)
        //                    {
        //                        var split = doc.Split('@');

        //                        var productnumber = split[0];

        //                        var type = split[1];
        //                        var file = split[2];
                                

        //                        if (type.ToLower().Contains("submittal"))
        //                        {
        //                            type = "Submittals Sheets";
        //                            productNumbers += (productNumbers.Length == 0) ? productnumber : ("," + productnumber);
        //                        }
        //                        else
        //                        {
        //                            var filename = System.IO.Path.GetFileName(file).Replace("GENERATED_", "");
        //                            var fileRef = (type + "\\" + filename);
        //                            if (System.IO.File.Exists(file))
        //                            {
        //                                zip.CreateEntryFromFile(file, fileRef, CompressionLevel.Optimal);
        //                            }
        //                        }
        //                    }

        //                    if (productNumbers.Length > 0)
        //                    {
        //                        string sdsfile = quotePackageDirectory + "DPO_QuotePackage_SubmittalDataSheets.pdf";

        //                        var pdf = new PdfConvertor();
        //                        var productService = new ProductServices();

        //                        foreach (var productNumber in productNumbers.Split(','))
        //                        {
        //                            //add product tags, project info and user info to header of each submittal sheet
        //                            //if no specific template type if given, add in the external submittal sheet (if it exists)
        //                            var product = quotePackage.Items.Where(x => x.ProductNumber == productNumber).FirstOrDefault();

        //                            if (product.GetSubmittalSheetTemplateName != "SubmittalTemplate")
        //                            {
        //                                var file = productService.GenerateSubmittalDataFileForPackage(productNumber, product.QuoteItemId, currentProject.ProjectId);

        //                                if (file != null)
        //                                {
        //                                    pdf.AppendHtml(file);
        //                                }
        //                            }
        //                            else
        //                            {
        //                                var submittalDocument = product.Documents.Where(d => d.DocumentTypeId == (int)DocumentTypeEnum.SubmittalData).FirstOrDefault();

        //                                var fullFile = Utilities.GetSubmittalDirectory() + submittalDocument.FileName + @".pdf";

        //                                if (System.IO.File.Exists(fullFile))
        //                                {
        //                                    pdf.AppendFile(fullFile);
        //                                }
        //                            }
        //                        }

        //                        pdf.WriteToFile(sdsfile);

        //                        if (System.IO.File.Exists(sdsfile))
        //                        {
        //                            zip.CreateEntryFromFile(sdsfile, "Submittals Sheets\\SDS_" + currentProjectNameAsFileName + ".pdf", CompressionLevel.Optimal);
        //                        }
        //                    }

        //                    foreach (var doc in quotePackage.QuotePackageAttachedFiles.ToList())
        //                    {
        //                        var fullFile = quotePackageDirectory + doc.FileName;

        //                        if (System.IO.File.Exists(fullFile))
        //                        {
        //                            var filename = System.IO.Path.GetFileName(fullFile);

        //                            bool isSelected = (Request.Form["doc" + doc.FileName] + "" != "");

        //                            if (isSelected == false) isSelected = (chkAllAttachedFiles && doc.DocumentTypeId == (int)DocumentTypeEnum.QuotePackageAttachedFile);

        //                            if (isSelected)
        //                            {
        //                                zip.CreateEntryFromFile(fullFile, "AttachedFiles\\" + filename, CompressionLevel.Optimal);
        //                            }
        //                        }
        //                    }
        //                }

        //                var coverPageFile = projectService.GenerateQuotePackageCoverPageFile((long)model.QuoteId, base.RenderView(this, "QuotePackageCoverPage", coverPageModel));

        //                if (coverPageFile != null)
        //                {
        //                    using (var zip = ZipFile.Open(quotePackageFilename, ZipArchiveMode.Update))
        //                    {
        //                        zip.CreateEntryFromFile(coverPageFile, "CoverSheet_" + currentProjectNameAsFileName + ".pdf", CompressionLevel.Optimal);
        //                    }
        //                }

        //            }

        //            this.Response.ContentType = MimeMapping.GetMimeMapping(quotePackageFilename);
        //            Response.AddHeader("Content-Disposition", String.Format("attachment;filename=\"{0}\"", "QuotePackage_" + currentProjectNameAsFileName + ".zip"));
        //            Response.TransmitFile(quotePackageFilename);

        //            return null;
        //        }
        //        finally
        //        {
        //            lock (htmlService)
        //            {
        //                if (!locked)
        //                {
        //                    System.IO.File.Delete(lockFile);
        //                }

        //            }
        //        }
        //    }

        //    return base.RedirectToAction("QuotePackage", new { ProjectId = model.ProjectId, QuoteId = model.QuoteId });
        //}

        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ActionResult QuotePackageDeleteAttachFile(QuoteItemsModel model)
        {
            this.ServiceResponse = new ServiceResponse();

            if (model.QuoteId.HasValue)
            {
                var quotePackageDirectory = Utilities.GetQuotePackageDirectory(model.QuoteId.Value);

                if (Request.Form["chkDeleteAllAttached"] != null)
                {
                    foreach (FileInfo file in new DirectoryInfo(quotePackageDirectory).GetFiles())
                    {
                        file.Delete();
                    }
                }
                else
                    foreach (var key in Request.Form.Keys)
                    {
                        var filename = key.ToString();
                        if (filename.StartsWith("chkAttached-"))
                        {
                            var file = quotePackageDirectory + filename.Replace("chkAttached-", "");
                            if (System.IO.File.Exists(file))
                            {
                                System.IO.File.Delete(file);
                            }
                        }
                    }
            }

            return base.RedirectToAction("QuotePackage", new { ProjectId = model.ProjectId, QuoteId = model.QuoteId });
        }

        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ActionResult QuotePrint(long? projectId, long? quoteId, bool? withCostPrices)
        {
            QuotePrintModel model = new QuotePrintModel();

            model.WithCostPrices = withCostPrices ?? false;

            var projectResponse = new ProjectServices().GetProjectModel(this.CurrentUser, projectId.Value);

            if (ProcessServiceResponse(projectResponse))
            {
                model.Project = projectResponse.Model as ProjectModel;

                var quoteResponse = quoteService.GetQuoteModel(this.CurrentUser, projectId.Value, quoteId);
              
                
                if (ProcessServiceResponse(quoteResponse))
                {
                    model.Quote = quoteResponse.Model as QuoteModel;
                    
                    var commissionResponse = new CommissionRequestServices().GetCommissionRequestModel(this.CurrentUser,
                                            new CommissionRequestModel { CommissionRequestId = model.Quote.CommissionRequestId, ProjectId = model.Project.ProjectId, QuoteId = model.Quote.QuoteId });

                    model.CommissionRequest = commissionResponse.Model as CommissionRequestModel;

                    var itemsRepsonse = quoteService.GetQuoteItemListModel(this.CurrentUser, quoteId.Value);

                    if (ProcessServiceResponse(itemsRepsonse))
                    {
                        var items = itemsRepsonse.Model as List<QuoteItemListModel>;
                        var pagedItems = new PagedList<QuoteItemListModel>(items, new Search { Page = 1, PageSize = Constants.DEFAULT_PAGESIZE_RETURN_ALL });
                        model.QuoteItems = new QuoteItemsModel { Items = pagedItems };

                        model.QuoteItems.WithCostPrice = model.WithCostPrices;
                    }
                }
            }

            this.RouteData.Values["action"] = "QuotePrint";

            return View("QuotePrint", model);
        }

        public EmptyResult QuotePrintExcel(long? projectId, long? quoteId, bool? withCostPrices)
        {
            QuotePrintModel model = new QuotePrintModel();

            withCostPrices = withCostPrices ?? false;

            var stream = new ProjectServices().QuotePrintExcelFile(this.CurrentUser, projectId.Value, quoteId.Value, withCostPrices.Value);

            this.Response.AddHeader("Content-Disposition", "inline; filename=Quote print.xls");
            this.Response.AddHeader("Cache-Control", "no-cache");
            this.Response.AddHeader("Content-Type", MimeMapping.GetMimeMapping("Quote print.xls"));

            stream.WriteTo(this.Response.OutputStream);

            return new EmptyResult();
        }

        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public EmptyResult QuotePrintExcelWithCostPrices(long? projectId, long? quoteId)
        {
            return QuotePrintExcel(projectId, quoteId, true);
        }

        public ActionResult QuotePrintFooter(long? projectId, long? quoteId)
        {
            return View("QuotePrintFooter", "_PrintPartialLayout");
        }

        public ActionResult QuotePrintHeader(long? projectId, long? quoteId)
        {
            QuotePrintModel model = new QuotePrintModel();

            var projectResponse = new ProjectServices().GetProjectModel(this.CurrentUser, projectId.Value);

            if (ProcessServiceResponse(projectResponse))
            {
                model.Project = projectResponse.Model as ProjectModel;
            }

            return View("QuotePrintHeader", "_PrintPartialLayout", model);
        }

        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ActionResult QuotePrintWithCostPrice(long? projectId, long? quoteId)
        {
            return QuotePrint(projectId, quoteId, true);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ActionResult QuoteRecalculate(QuoteModel model)
        {
            this.ServiceResponse = quoteService.QuoteRecalculate(this.CurrentUser, model);

            ProcessServiceResponse(this.ServiceResponse);

            return base.AjaxReloadPage();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ActionResult QuoteSetActive(QuoteModel model)
        {
            var projectResponse = new ProjectServices().GetProjectModel(this.CurrentUser, model.ProjectId);

            if (ProcessServiceResponse(projectResponse))
            {
                if(model.Project == null)
                {
                    model.Project = projectResponse.Model as ProjectModel;
                }
            }

            this.ServiceResponse = quoteService.SetActive(this.CurrentUser, model);

            ProcessServiceResponse(this.ServiceResponse);

            return ProjectQuotes(new ProjectQuotesModel { ProjectId = model.ProjectId });

            //return BackToActiveQuote(model.ProjectId, model.QuoteId, false);

            //return View("Quote", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorise(Accesses = new[] { SystemAccessEnum.UndeleteProject })]
        public ActionResult QuoteUndelete(QuoteModel model)
        {
            this.ServiceResponse = quoteService.UnDelete(this.CurrentUser, model, true);

            ProcessServiceResponse(this.ServiceResponse);

            return AJAXRedirectTo("ProjectQuotes", "Projectdashboard", new { ProjectId = model.ProjectId });
        }

    }
}