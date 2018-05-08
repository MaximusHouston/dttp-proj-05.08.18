using DPO.Common;
using DPO.Domain;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DPO.Web.Controllers
{

    public class CityCMSController : BaseController
    {
        public CityCMSServices services;

        public CityCMSController()
        {
            this.services = new CityCMSServices();
        }

        public CityCMSController(CityCMSServices services)
        {
            this.services = services;
        }

        #region city cms

        #region homescreen

        [Authorise(Accesses = new[] { SystemAccessEnum.ContentManagementHomeScreen })]
        public ActionResult HomeScreen()
        {
            var model = services.GetHomeScreenModelWithBillboardPosters();
            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult HomeScreen(HomeScreenModel model)
        {
            if (ModelState.IsValid)
            {
                services.SaveHomeScreen(model);
                return RedirectToAction("HomeScreen");
            }
            return View(model);
        }

        #endregion

        #region homescreen billboards

        public ActionResult HomeScreenBillboardImageUpload(long billboardId)
        {
            var model = services.GetSingleBillboard(billboardId);
            return View(model);
        }

        [HttpGet]
        public ActionResult UploadBillboardImage()
        {
            return RedirectToAction("HomeScreen");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //todo - permissions
        public ActionResult UploadBillboardImage(FormCollection formCollection, BillboardModel model)
        {
            if(Request != null && Request.Files.Count == 1)
            {
                string targetFilePath = Utilities.GetDaikinCityDirectory() + "images\\" + model.SinglePoster.image;
                //bool fileExists = System.IO.File.Exists(targetFilePath);

                var file = Request.Files[0];

                if(file != null && file.ContentLength > 0 && file.ContentType == "image/jpeg")
                {
                    file.SaveAs(targetFilePath);
                }
            }
            return RedirectToAction("HomeScreen");
        }

        #endregion

        #region functional buildings

        [Authorise(Accesses = new[] { SystemAccessEnum.ContentManagementFunctionalBuildings })]
        public ActionResult FunctionalBuildings()
        {
            List<BuildingFloorModel> model = services.GetFunctionalFloors();
            return View(model);
        }

        [HttpGet]
        public ActionResult FunctionalBuilding(long FloorId)
        {
            BuildingLinksModel model = services.GetFloorLinks(FloorId);
            return View(model);
        }

        [HttpPost]
        public ActionResult FunctionalBuilding(BuildingLinksModel model)
        {
            if(ModelState.IsValid)
            {
                services.SaveFunctionalBuildingLinks(model);
                return RedirectToAction("FunctionalBuildings");
            }
            return View(model);
        }

        #endregion

        #region application buildings

        [Authorise(Accesses = new[] { SystemAccessEnum.ContentManagementApplicationBuildings })]
        public ActionResult ApplicationBuildings()
        {
            var model = services.GetApplicationBuildingsPageModel();
            return View(model);
        }

        [HttpGet]
        public ActionResult ApplicationBuildingFloor(long floorid, long buildingid)
        {
            BuildingFloorEditModel model = services.GetSingleBuildingFloorForEdit(floorid);
            model.buildingName = services.GetBuildingName(buildingid);
            model.systemsToPickFrom = services.GetSystemsList();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ApplicationBuildingFloor(BuildingFloorEditModel model)
        {
            if (ModelState.IsValid)
            {
                services.SaveApplicationBuildingFloorConfiguration(model);
                return RedirectToAction("ApplicationBuildingFloor", new { floorid = model.id, buildingid = model.buildingId });
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult ApplicationBuildingFloorConfiguration(int floorid, long buildingId, bool isAlternate = false)
        {
            var model = new FloorConfigurationModel
            {
                floorId = floorid,
                buildingId = buildingId,
                isAlternate = isAlternate,
                systemSize = "0",
                buildingName = services.GetBuildingName(buildingId),
                floorName = services.GetFloorName(floorid)
            };
            return View(model);
        }

        [HttpPost] 
        [ValidateAntiForgeryToken]
        public ActionResult ApplicationBuildingFloorConfiguration(FloorConfigurationModel model)
        {
            services.SaveFloorConfiguration(model);
            return RedirectToAction("ApplicationBuildingFloor", new { floorid = model.floorId, buildingid = model.buildingId });
        }
        
        [HttpGet]
        public ActionResult ApplicationBuildingFloorConfigurationDelete(FloorConfigurationModel model)
        {
            this.ServiceResponse = services.DeleteFloorConfiguration(model);
            return RedirectToAction("ApplicationBuildingFloor", new { floorid = model.floorId, buildingid = model.buildingId });
        }

        [HttpGet]
        public ActionResult ApplicationBuildingFloorConfigurationImageEdit(FloorConfigurationModel model)
        {
            this.ServiceResponse = services.GetFloorConfiguration(model.id);

            FloorConfigurationModel newModel = this.ServiceResponse.Model as FloorConfigurationModel;
            if (model.overlayImage == null) newModel.overlayImage = null;
            if (model.systemImage == null) newModel.systemImage = null;
           
            return View(newModel);
        }

        [HttpPost]
        public ActionResult ApplicationBuildingFloorConfigurationImageEdit(FormCollection formCollection, FloorConfigurationModel model)
        {
            this.ServiceResponse = services.PostFloorConfigurationImage(Request, model);

            ViewData["PageMessages"] = this.ServiceResponse.Messages;

            if (this.ServiceResponse.Messages.HasErrors == true)
            {
                return RedirectToAction("ApplicationBuildingFloorConfigurationImageEdit", model);
            }

            return RedirectToAction("ApplicationBuildingFloor", new { floorid = model.floorId, buildingid = model.buildingId });
        }

        [HttpGet]
        public ActionResult ApplicationBuildingFloorConfigurationProducts(long configid, long floorid, long buildingid)
        {
            FloorConfigurationLayoutsModel model = services.GetFloorConfigurationLayoutsForEdit(configid, floorid, buildingid);
            return View(model);
        }

        [HttpPost]
        public ActionResult ApplicationBuildingFloorConfigurationProducts(FloorConfigurationLayoutsModel model)
        {
            services.SaveDecisionTree(model.decisionTree, model.configId);
            return RedirectToAction("ApplicationBuildingFloor", new { floorid = model.floorId, buildingid = model.buildingId });
        }

        [HttpGet]
        public ActionResult ApplicationBuildingFloorConfigurationIndoorUnits(long configid, long floorid, long buildingid)
        {
            FloorConfigurationIndoorUnitsModel model = services.GetFloorConfigurationsIndoorUnitsForEdit(configid, floorid, buildingid);
            return View(model);
        }

        [HttpPost]
        public ActionResult ApplicationBuildingFloorConfigurationIndoorUnits(FloorConfigurationIndoorUnitsModel model)
        {
            services.SaveApplicationBuildingFloorConfigurationIndoorUnits(model);
            return RedirectToAction("ApplicationBuildingFloor", new { floorid = model.floorId, buildingid = model.buildingId });
        }

        [HttpPost]
        public ActionResult ApplicationBuildingFloorConfigurationIndoorUnit(FloorConfigurationIndoorUnitModel model)
        {
            services.SaveApplicationBuildingFloorConfigurationIndoorUnit(model);
            return Json(true);
        }
        #endregion

        #region application products

        [Authorise(Accesses = new[] { SystemAccessEnum.ContentManagementApplicationProducts })]
        public ActionResult ApplicationProducts()
        {
            var model = services.GetSystemsModel();
            return View(model);
        }

        [HttpGet]
        public ActionResult ApplicationProductEdit(long? systemId)
        {
            return View(services.GetSingleSystem(systemId));
        }

        [HttpPost]
        public ActionResult ApplicationProductEdit(CitySystemModel model)
        {
            if (ModelState.IsValid)
            {
                services.SaveApplicationProduct(model);
                return RedirectToAction("ApplicationProducts");
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult ApplicationProductImageUpload(long systemid)
        {
            return View(services.GetSingleSystem(systemid));
        }

        [HttpGet]
        public ActionResult UploadApplicationProductImage()
        {
            return RedirectToAction("ApplicationProducts");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //todo - permissions
        public ActionResult UploadApplicationProductImage(FormCollection formCollection, CitySystemModel model)
        {
            services.SaveApplicationProductImage(Request, model);
            return RedirectToAction("ApplicationProductEdit", new { systemid = model.id });
        }

        [HttpGet]
        public ActionResult ApplicationProductIconUpload(long systemid)
        {
            return View(services.GetSingleSystem(systemid));
        }

        [HttpGet]
        public ActionResult UploadApplicationProductIcon()
        {
            return RedirectToAction("ApplicationProducts");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //todo - permissions
        public ActionResult UploadApplicationProductIcon(FormCollection formCollection, CitySystemModel model)
        {
            services.SaveApplicationProductIcon(Request, model);
            return RedirectToAction("ApplicationProductEdit", new { systemid = model.id });
        }

        public ActionResult ApplicationProductDelete(long systemid)
        {
            services.DeleteApplicationProduct(systemid);
            return RedirectToAction("ApplicationProducts");
        }
        #endregion

        #region library

        [Authorise(Accesses = new[] { SystemAccessEnum.ContentManagementLibrary })]
        public ActionResult Library()
        {
            var model = services.GetLibraryPage();
            return View(model);
        }

        [HttpPost]
        public string LibraryUploadGetIds(string[] files, int directoryId)
        {
            List<dynamic> response = new List<dynamic>();
            for (int i = 0; i < files.Length; i++)
            {
                response.Add(services.AddLibraryDocumentBare(new LibraryDocumentEditModel
                {
                    DirectoryId = directoryId,
                    name = files[i],
                    path = "",
                    thumb = ""
                }));
            }
            return JsonConvert.SerializeObject(new { documents = response }, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        }

        [HttpPost]
        public string LibraryFileUpload(int documentId)
        {
            dynamic response = null;

            if (Request.Files.Count > 0)
            {
                response = services.UpdateLibraryDocumentBare(documentId, Request.Files);
            }
            else
            {
                response = new
                {
                    id = documentId,
                    success = false,
                    error = "No file provided"
                };
            }

            return JsonConvert.SerializeObject(response, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        }

        [HttpPost]
        public string LibraryDocumentTitlesUpdate(List<LibraryDocumentEditModel> documents)
        {
            for (int i = 0; i < documents.Count; i++)
            {
                services.UpdateLibraryDocumentTitle(documents[i]);
            }
            return JsonConvert.SerializeObject(new { success = true }, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        }

        public ActionResult LibraryDirectories()
        {
            var model = services.GetLibraryPage();
            return PartialView("_LibraryDirectories", model);
        }

        public ActionResult LibraryDirectoryDocuments(int dirId)
        {
            var model = services.GetLibraryDirectoryDocuments(dirId);
            return PartialView("LibraryDirectoryDocuments", model);
        }

        public ActionResult LibraryDocument(int docId, int currentDirectoryId)
        {
            if (docId == 0) return PartialView("LibraryDocumentEdit", new LibraryDocumentEditModel { DirectoryId = currentDirectoryId });

            var model = services.GetSingleLibraryDocument((int)docId);
            return PartialView("LibraryDocumentEdit", model);
        }

        public ActionResult LibraryDocumentsUploadModal(int currentDirectoryId)
        {
            return PartialView("LibraryDocumentsUploadModal", new LibraryDocumentEditModel { DirectoryId = currentDirectoryId });
        }

        [HttpPost]
        public ActionResult MoveLibraryDocument(int OldDirId, int DirId, int DocId)
        {
            return Json(services.MoveLibraryDocument(OldDirId, DirId, DocId));
        }

        [HttpPost]
        public ActionResult CopyLibraryDocument(int DirId, int DocId)
        {
            //don't copy if exists already
            return Json(services.CopyLibraryDocument(DirId, DocId));
        }

        [HttpPost]
        public ActionResult EditLibraryDocument(LibraryDocumentEditModel document)
        {
            return Content(JsonConvert.SerializeObject(services.EditLibraryDocument(document, Request.Files), new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore }));
        }
       
        [HttpPost]
        public ActionResult LibraryEditDirectory(long DirId, string DirName)
        {
            services.SaveLibraryDirectoryChange(DirId, DirName);
            return Json(true);
        }

        [HttpPost]
        public ActionResult LibraryToggleDirectoryProtection(int dirId)
        {
            services.ToggleDirectoryProtection(dirId);
            return Json(true);
        }

        [HttpPost]
        public ActionResult LibraryAddSubDirectory(int? DirId, string DirName)
        {
            services.SaveLibrarySubDirectory(DirId, DirName);
            return Json(true);
        }

        [HttpPost]
        public ActionResult LibraryDirectoryMove(int? DirId, int? NewParentDirId)
        {
            services.SaveLibraryDirectoryMove(DirId, NewParentDirId);
            return Json(true);
        }

        [HttpPost]
        public ActionResult LibraryDeleteDocument(int DocId, int DirId)
        {
            services.DeleteLibraryDocument(DocId, DirId);
            return Json(true);
        }

        [HttpPost]
        public ActionResult LibraryDeleteDocuments(List<int> DocIds, int DirId)
        {
            for (var i = 0; i < DocIds.Count; i++)
            {
                services.DeleteLibraryDocument(DocIds[i], DirId);
            }
               
            return Json(true);
        }

        [HttpPost]
        public ActionResult LibraryDeleteDirectory(int DirId)
        {
            return Json(services.DeleteLibraryDirectory(DirId));
        }

        #endregion

        #region communications center

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ContentManagementCommsCenter })]
        public ActionResult CommunicationsCenter()
        {
            var model = services.GetCommunicationsModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult CommunicationsCenter(CommunicationsCentreModel model)
        {
            services.SaveCommsCenterVideos(model);
            return RedirectToAction("CommunicationsCenter");
        }

        public ActionResult CommunicationsCenterUploadImage(long videoid)
        {
            CommunicationsCentreVideoModel model = services.GetSingleCommunicationsCenterVideo(videoid);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UploadCommsCenterVideoImage(FormCollection formCollection, CommunicationsCentreVideoModel model)
        {
            if (Request != null && Request.Files.Count == 1)
            {
                var file = Request.Files[0];

                if(!file.IsImage())
                {
                    return View("CommunicationsCenterUploadImage", model);
                }

                string thumbName = "thumb_" + model.id.ToString() + file.FileName.Substring(file.FileName.LastIndexOf("."));
                string targetFilePath = Utilities.GetDaikinCityDirectory() + @"images\buildings\communications-center\" + thumbName;

                if (file != null && file.ContentLength > 0)
                {
                    file.SaveAs(targetFilePath);
                    services.SaveCommsCenterVideoThumb(model.id, thumbName);
                }
            }

            return RedirectToAction("CommunicationsCenter");
        }

        [HttpGet]
        public ActionResult ContactUsFormEdit()
        {
            SendEmailContactUsModel model = new AccountServices().GetContactForm().Model as SendEmailContactUsModel;
            return View(model);
        }

        [HttpPost]
        public ActionResult ContactUsFormEdit(SendEmailContactUsModel model)
        {
            new AccountServices().SaveContactForm(model);
            return RedirectToAction("CommunicationsCenter");

            //TODO - validation message
        }

        #endregion

        #endregion

        #region product family images

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ContentManagementProductFamilies })]
        public ActionResult ProductFamilies()
        {
            this.ServiceResponse = new ProductServices().GetProductFamilyList(this.CurrentUser);
            return View(this.ServiceResponse.Model);
        }

        [HttpPost]
        public ActionResult ProductFamilyImageEdit(FormCollection formCollection, CatelogListModel item)
        {
            this.ServiceResponse = new ServiceResponse();

            if (Request != null && Request.Files.Count == 1)
            {
                var file = Request.Files[0];

                if (!file.IsImage())
                {
                    this.ServiceResponse.AddError("Please upload Image files only");
                }
                else
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        if(file.ContentLength > 2000000)
                        {
                            this.ServiceResponse.AddError("Image uploads can only be up to 2MB in size");
                        }
                        else
                        {
                            try
                            {
                                var targetFilePath = Server.MapPath("~/Images/Products/Family/" + item.Id + ".png");
                                file.SaveAs(targetFilePath);
                                this.ServiceResponse.AddSuccess("Image uploaded successfully");
                            }
                            catch(Exception)
                            {
                                this.ServiceResponse.AddError("Unable to save image, please try again");
                            }
                        }
                    }
                }

            }
            else
            {
                this.ServiceResponse.AddError("Please specify an Image to upload");
            }

            ViewData["PageMessages"] = this.ServiceResponse.Messages;
            return RedirectToAction("ProductFamilies");
        }

        #endregion

        #region tools
        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ContentManagementTools })]
        public ActionResult Tools()
        {
            var model = new PermissionServices().GetToolLinksForEdit();
            return View(model);
        }

        [HttpGet]
        public ActionResult ToolEdit(long? toolId)
        {
            var model = new PermissionServices().GetToolForEdit(toolId);
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.ContentManagementTools})]
        public ActionResult ToolEdit(FormCollection form, ToolEditModel tool)
        {
            var description = form["Description"];
            var createHyperLink = form["createHyperLink"];
            
            if(string.IsNullOrEmpty(description))
            {
                description = "VRV-WEB-Xpress is a new internet based systematic selection software for Daikin VRV systems (Air-Cooled) and VRV-W systems (Water-Cooled). It allows you to make quick or detailed system selection for quotation with easy operation. Detailed reports and mechanical schedules can be generated upon building complete selection.";
            }

            if (tool.Description == null || tool.Description.Length == 0)
            {
                tool.Description = (string)description;
            }

            if(description.Contains("href") && !description.Contains("https"))
            {
                int index = description.IndexOf("href") + 5;
                string newDescription =  description.Insert(index + 1, "https://");
                description = newDescription;
            }

            if (createHyperLink == "true" && !description.Contains("href"))
            {
                tool.Description += "<br /><br />";
                tool.Description += "<a href='https://webtools.daikin.eu' target='_blank'> Click here to access</a>";
            }

            this.ServiceResponse = new PermissionServices().PostTool(tool, Request);

            ViewData["PageMessages"] = this.ServiceResponse.Messages;

            if(this.ServiceResponse.Messages.HasErrors == true)
            {
                return View("ToolEdit", tool);
            }

            return RedirectToAction("Tools","CityCMS");
        }

        [HttpGet]
        public ActionResult ToolDelete(int toolId)
        {
            this.ServiceResponse = new PermissionServices().DeleteTool(toolId);

            ViewData["PageMessages"] = this.ServiceResponse.Messages;

            return RedirectToAction("Tools", "CityCMS");
        }

        #endregion

        #region project office cms

        //public ActionResult ProjectOffice_Menus()
        //{
        //    var model = new PageModel();
        //    return View(model);
        //}

        //public ActionResult ProjectOffice_DocumentVisibility()
        //{
        //    var model = new PageModel();
        //    return View(model);
        //}

        //public ActionResult ProjectOffice_ProductListings()
        //{
        //    var model = new PageModel();
        //    return View(model);
        //}

        //public ActionResult ProjectOffice_Templates()
        //{
        //    var model = new PageModel();
        //    return View(model);
        //}

        //public ActionResult ProjectOffice_UserGuides()
        //{
        //    var model = new PageModel();
        //    return View(model);
        //}

        #endregion

        #region output

        [HttpGet]
        [Authorise(NoSecurityRequired = true)]
        public string Config()
        {
            var model = services.GetConfig();
            model.home.privacypolicy = null;
            return JsonConvert.SerializeObject(new { config = model }, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        }

        [HttpGet]
        [Authorise(NoSecurityRequired = true)]
        public string Building(long? id)
        {
            var model = services.GetBuildingModelForJSON(id);
            return JsonConvert.SerializeObject(new { building = model }, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        }

        [HttpGet]
        [Authorise(NoSecurityRequired = true)]
        public string BuildingToJson(String buildingName)
        {
            var model = services.GetBuildingModelForJSON(buildingName);
            return JsonConvert.SerializeObject(new { building = model }, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });
        }

        [HttpGet]
        [Authorise(NoSecurityRequired = true)]
        public string LibraryDocuments()
        {
            var model = services.GetLibraryPageForJSON();

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Include
            };

            return JsonConvert.SerializeObject(model, settings);
        }

        [HttpGet]
        [Authorise(NoSecurityRequired = true)]
        public string HotelRoom()
        {
            // TODO is this getting CMSified?
            return System.IO.File.ReadAllText(Utilities.GetDaikinCityDirectory() + "/json/hotel_room.json");
        }

        #endregion

        #region library import
        //[HttpGet]
        //public string ImportLibrary()
        //{
        //    var json = System.IO.File.ReadAllText(Utilities.GetDaikinCityDirectory() + "/json/documents.json");

        //    JObject rootNode = JObject.Parse(json);

        //    this.traverse((JObject)rootNode["documents"], null);

        //    return "Import Successful";
        //}

        //private void traverse(JObject json, int? parentId)
        //{
        //    var dir = new LibraryDirectoryModel
        //    {
        //        name = (string)json["name"],
        //        parentId = parentId
        //    };

        //    this.services.saveImportedDirectory(dir);

        //    JArray directories = (JArray)json["documents"];

        //    if(directories != null)
        //    {
        //        for (var i = 0; i < directories.Count; i++)
        //        {
        //            this.traverse((JObject)directories[i], dir.id);
        //        }
        //    }

        //    JArray documents = (JArray)json["document"];

        //    if(documents != null)
        //    {
        //        for (var k = 0; k < documents.Count; k++)
        //        {
        //            var doc = new LibraryDocumentModel
        //            {
        //                name = (string)documents[k]["name"],
        //                path = (string)documents[k]["path"],
        //                thumb = (string)documents[k]["thumb"]
        //            };

        //            services.saveImportedDocument(doc, dir.id);
        //        }
        //    }

        //}
        #endregion
    }
}