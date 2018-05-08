using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DPO.Common;
using DPO.Common.Interfaces;
using DPO.Data;
using DPO.Domain.Properties;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Collections;
using System.Net.Mail;
using DPO.Resources;
using System.Web;
using EntityFramework.Extensions;
using System.IO;
using System.Web.Mvc;
using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;
using System.Drawing;
using System.Drawing.Imaging;


namespace DPO.Domain
{
    public partial class CityCMSServices : BaseServices
    {
        public CityCMSServices() : base() { }
        public CityCMSServices(DPOContext context) : base(context) { }

        //todo - find out where these live
        private string DCVideoURL = "/daikincityweb/video/";
        private string DCImageRootURL = "/daikincityweb/images/";
        private string DCBuildingImageURL = "/daikincityweb/images/buildings/";

        #region home screen

        public HomeScreenModel GetHomeScreenModelWithBillboardPosters()
        {
            var model = this.GetHomeScreenModel();
            model.BillboardItems = this.GetBillboardModel();
            return model;
        }

        public HomeScreenModel GetHomeScreenModel()
        {
            var query = this.Db.QueryHomeScreen();
            var model = new HomeScreenModel
            {
                title = query.Title,
                bodytext = query.BodyText,
                privacypolicy = query.PrivacyPolicy
            };

            return model;
        }

        public void SaveHomeScreen(HomeScreenModel model)
        {
            this.Db.ReadOnly = false;

            HomeScreen entity = this.Db.QueryHomeScreen();

            entity.Title = model.title;
            entity.BodyText = model.bodytext;
            entity.PrivacyPolicy = model.privacypolicy;

            for (var i = 0; i < model.BillboardItems.poster.Count; i++ )
            {
                PosterModel p = model.BillboardItems.poster[i];
                BillboardPoster bp = this.Db.QueryBillboardPoster(p.id);
                bp.Image = p.image;
                bp.Url = p.url;
                bp.enabled = p.enabled;
            }

           this.Db.SaveChanges();
        }

        #endregion

        #region buildings
        public BuildingsModel GetBuildingsModel()
        {
            var model = new BuildingsModel();
            model.building = (from b in this.Db.Buildings
                              select new BuildingModel
                              {
                                  id = b.BuildingId,
                                  name = b.Name,
                                  typeId = b.TypeId,
                                  videoIn = b.VideoIn,
                                  videoInPoster = b.VideoInPoster,
                                  hotspotX = b.HotSpotX,
                                  hotspotY = b.HotSpotY,
                                  menuImage = b.MenuImage,
                                  buildingFolderName = b.BuildingFolderName
                              }).ToList();


            foreach(BuildingModel building in model.building)
            {
                this.prepareBuilding(building);
            };

            return model;
        }

        private void prepareBuilding(BuildingModel building)
        {
            if (building.menuImage != null)
            {
                building.menuImage = this.DCBuildingImageURL + building.buildingFolderName + '/' + building.menuImage;
            }

            building.videoIn = this.DCVideoURL + building.videoIn;
            building.videoInPoster = this.DCImageRootURL + building.videoInPoster;
            building.type = ((BuildingTypeEnums)building.typeId).ToString();
            building.path = "/citycms/building?id=" + building.id;
            building.floors = this.GetSingleBuildingFloors(building.id, building.buildingFolderName);
        }

        public string GetBuildingFolderName(long? buildingId)
        {
            var buildingFolderName = (from b in this.Db.Buildings
                            where b.BuildingId == buildingId
                            select b.BuildingFolderName).First();

            return buildingFolderName;
        }

        public string GetBuildingName(long? buildingId)
        {
            string buildingName = (from b in this.Db.Buildings
                                where b.BuildingId == buildingId
                                select b.Name).First();

            return buildingName;
        }

        public string GetFloorName(long? floorId)
        {
            string floorName = (from b in this.Db.BuildingFloors
                                where b.FloorId == floorId
                                select b.Name).First();

            return floorName;
        }

        public string GetConfigName(int configId)
        {
            string floorName = (from b in this.Db.BuildingFloorConfigurations
                                where b.FloorConfigId == configId
                                select b.Name).First();

            return floorName;
        }


        public BuildingModel GetBuildingModel(long? id)
        {
            BuildingModel building = (from b in this.Db.Buildings
                                      where b.BuildingId == id
                                      select new BuildingModel
                                      {
                                          id = b.BuildingId,
                                          name = b.Name,
                                          typeId = b.TypeId,
                                          videoIn = b.VideoIn,
                                          videoInPoster = b.VideoInPoster,
                                          hotspotX = b.HotSpotX,
                                          hotspotY = b.HotSpotY,
                                          menuImage = b.MenuImage,
                                          buildingFolderName = b.BuildingFolderName
                                      }).First();

            this.prepareBuilding(building);
            return building;
        }

        public BuildingModel GetBuildingModel(String buildingName)
        {
            List<BuildingModel> buildings = (from b in this.Db.Buildings
                                             select new BuildingModel
                                             {
                                                 id = b.BuildingId,
                                                 name = b.Name,
                                                 typeId = b.TypeId,
                                                 videoIn = b.VideoIn,
                                                 videoInPoster = b.VideoInPoster,
                                                 hotspotX = b.HotSpotX,
                                                 hotspotY = b.HotSpotY,
                                                 menuImage = b.MenuImage,
                                                 buildingFolderName = b.BuildingFolderName
                                             }).ToList();

            foreach (var building in buildings)
            {
                if(building.JsonName() == buildingName)
                {
                    this.prepareBuilding(building);
                    return building;
                }
            }

            return null;
        }

        public BuildingModel GetBuildingModelForJSON(String buildingName)
        {
            return prepareBuildingModelForJSON(this.GetBuildingModel(buildingName));
        }

        public BuildingModel GetBuildingModelForJSON(long? id)
        {
            return prepareBuildingModelForJSON(this.GetBuildingModel(id));
        }

        private BuildingModel prepareBuildingModelForJSON(BuildingModel building)
        {
            // clear out props not needed in json
            building.typeId = null;
            building.buildingFolderName = null;
            building.path = null;

            foreach (var f in building.floors.floor)
            {
                f.typeId = null;
                f.buildingId = null;

                if (f.alternativeConfigurations != null)
                {
                    foreach (var c in f.alternativeConfigurations.configuration)
                    {
                        c.isAlternate = null;
                        c.systemSize = null;
                    }
                }
            }

            return building;
        }

        public BuildingsModel GetBuildingsModelConfigFormatted()
        {
            List<BuildingModel> buildings = (from b in this.Db.Buildings
                                             select new BuildingModel
                                             {
                                                 name = b.Name
                                             }).ToList();

            foreach (var building in buildings)
            {
                building.path = "daikincityweb/json/" + building.JsonName() + ".json";
                building.name = null;
            }

            return new BuildingsModel {  building = buildings };
        }

        private BuildingFloorsModel GetSingleBuildingFloors(long? id, string buildingFolderLocation)
        {
            var floors = new BuildingFloorsModel();

            floors.floor = (from bf in this.Db.BuildingFloors
                            where bf.BuildingId == id
                            select new BuildingFloorModel
                            {
                                id = bf.FloorId,
                                buildingId = bf.BuildingId,
                                name = bf.Name,
                                typeId = bf.TypeId,
                                size = bf.size,
                                floorImage = bf.FloorImage,
                                applicationId = bf.ApplicationId,
                                backgroundImage = bf.BackgroundImage,
                                icon = bf.Icon

                            }).ToList();

            foreach (var f in floors.floor)
            {
                this.prepareFloor(f, buildingFolderLocation);
            }

            return floors;
        }

        private void prepareFloor(BuildingFloorModel f, string buildingFolderLocation)
        {
            f.type = ((BuildingTypeEnums)f.typeId).ToString();
            f.links = this.GetFloorLinks(f.id);
            f.configurations = this.GetFloorConfigurations(f.id, buildingFolderLocation);
            f.alternativeConfigurations = this.GetFloorAlternativeConfigurations(f.id, buildingFolderLocation);

            if (f.floorImage != null)
            {
                f.floorImage = this.DCBuildingImageURL + buildingFolderLocation + "/" + f.floorImage;
            }

            if (f.backgroundImage != null)
            {
                f.backgroundImage = this.DCBuildingImageURL + buildingFolderLocation + "/" + f.backgroundImage;
            }

            if (f.icon != null)
            {
                f.icon = this.DCBuildingImageURL + buildingFolderLocation + "/" + f.icon;
            }
        }

        public BuildingFloorModel GetSingleBuildingFloor(long floorid)
        {
            var floor = (from bf in this.Db.BuildingFloors
                         where bf.FloorId == floorid
                         select new BuildingFloorModel
                         {
                             id = bf.FloorId,
                             buildingId = bf.BuildingId,
                             name = bf.Name,
                             typeId = bf.TypeId,
                             size = bf.size,
                             floorImage = bf.FloorImage,
                             applicationId = bf.ApplicationId,
                             backgroundImage = bf.BackgroundImage,
                             icon = bf.Icon
                         }).First();

            var floorsBuildingFolderName = this.GetBuildingFolderName(floor.buildingId);

            this.prepareFloor(floor, floorsBuildingFolderName);

            return floor;
        }

        public BuildingFloorEditModel GetSingleBuildingFloorForEdit(long floorid)
        {
            var floor = (from bf in this.Db.BuildingFloors
                         where bf.FloorId == floorid
                         select new BuildingFloorEditModel
                         {
                             id = bf.FloorId,
                             buildingId = bf.BuildingId,
                             name = bf.Name,
                             typeId = bf.TypeId,
                             size = bf.size,
                             floorImage = bf.FloorImage,
                             applicationId = bf.ApplicationId,
                             backgroundImage = bf.BackgroundImage,
                             icon = bf.Icon
                         }).First();

            var floorsBuildingFolderName = this.GetBuildingFolderName(floor.buildingId);

            this.prepareFloor(floor, floorsBuildingFolderName);

            return floor;
        }

        public BuildingLinksModel GetFloorLinks(long? floorId)
        {
            var buildingLinks = (from link in this.Db.FloorLinks
                                 where link.FloorId == floorId
                                 select new BuildingLinkModel
                                 {
                                     id = link.LinkId,
                                     floorId = link.FloorId,
                                     title = link.Title,
                                     description = link.Copy,
                                     url = link.Url,
                                     enabled = link.Enabled
                                 }).ToList();

            if (buildingLinks.Count == 0) return null;

            return new BuildingLinksModel
            {
                link = buildingLinks,
                buildingName = Db.QueryBuildingNameByFloorId(floorId)
            };
        }

        private void prepareFloorConfigurations(FloorConfigurationModel model, string buildingFolderName)
        {
            if(model.overlayImage != null)
            {
                model.overlayImage = this.DCBuildingImageURL + buildingFolderName + "/" + model.overlayImage;
            }

            if(model.systemImage != null)
            {
                model.systemImage = this.DCImageRootURL + "systems/" + model.systemImage;
            }

            model.indoorUnits = this.GetFloorConfigurationsIndoorUnits(model.id);
            model.layouts = this.GetFloorConfigurationLayouts(model.id);
        }

        public FloorConfigurationLayoutsModel GetFloorConfigurationLayoutsForEdit(long configid, long floorid, long buildingid)
        {
            List<CitySystemModel> systems = this.GetSystemsList();
            systems.Insert (0, new CitySystemModel { id = -1, name = "Not Selected" });

            return new FloorConfigurationLayoutsModel
            {
                buildingName = this.GetBuildingName(buildingid),
                floorName = this.GetFloorName(floorid),
                floorId = floorid,
                buildingId = buildingid,
                configId = configid,
                configName = this.GetConfigName((int)configid),
                systems = systems,
                dependancies = DecisionTreeDependancyMap.map(),
                decisionTree = GetDecisionTreeMapByConfigId(configid)
            };
        }

        public void SaveFloorConfiguration(FloorConfigurationModel model)
        {
            this.Db.ReadOnly = false;

            var entity = new BuildingFloorConfiguration
            {
                Alternate = model.isAlternate,
                Energy = model.energy,
                FloorId = model.floorId,
                Name = model.name,
                OverlayImage = model.overlayImage,
                SystemImage = model.systemImage,
                SystemName = model.systemName,
                SystemSize = model.systemSize,
                SystemType = model.systemType
            };

            this.Db.FloorConfigCreate(entity);
            this.Db.SaveChanges();
        }

        public ServiceResponse DeleteFloorConfiguration(FloorConfigurationModel model)
        {

            this.Db.ReadOnly = false;

            var entity = this.Db.BuildingFloorConfigurations.Where(b => b.FloorConfigId == model.id).FirstOrDefault();

            if (entity != null)
            {
                this.Db.Context.BuildingFloorConfigurations.Remove(entity);
                this.Db.SaveChanges();
                this.Response.AddSuccess("Floor Configuration Deleted");
            }

            return this.Response;
        }

        private FloorConfigurationLayoutsModel GetFloorConfigurationLayouts(long configId)
        {
            List<FloorConfigurationLayoutModel> layouts = new List<FloorConfigurationLayoutModel>();
            DecisionTreeMap decisionTree = GetDecisionTreeMapByConfigId(configId);

            // zone level
            if (decisionTree.Systems[0] != -1)
            {
                layouts.Add(new FloorConfigurationLayoutModel
                {
                    id = 1,
                    configId = (int)configId,
                    name = "Zone Level",
                    node = ParseDecisionTree(0, decisionTree)
                });
            }

            // system level
            if (decisionTree.Systems[3] != -1)
            {
                layouts.Add(new FloorConfigurationLayoutModel
                {
                    id = 2,
                    configId = (int)configId,
                    name = "System Level",
                    node = ParseDecisionTree(3, decisionTree)
                });
            }
            if (layouts.Count == 0) return null;
            return new FloorConfigurationLayoutsModel { layout = layouts };
        }

        public FloorConfigurationLayoutNodeModel ParseDecisionTree(int rootIndex, DecisionTreeMap decisionTree)
        {
            FloorConfigurationLayoutNodeModel rootNode = new FloorConfigurationLayoutNodeModel
            {
                id = decisionTree.Systems[rootIndex],
                node = nodeTreeFromDecisionTree(rootIndex, decisionTree)
            };

            return rootNode;
        }

        private List<FloorConfigurationLayoutNodeModel> nodeTreeFromDecisionTree(int nodeIndex, DecisionTreeMap decisionTree)
        {
            List<FloorConfigurationLayoutNodeModel> nodes = new List<FloorConfigurationLayoutNodeModel>();

            for (int i = 0; i < decisionTree.Dependancies.Count; i++ )
            {
                int dependancy = decisionTree.Dependancies[i];
                if (dependancy == (nodeIndex + 1))
                {
                    int mapped = mappedDependancy(i);
                    if (mapped > -1 && decisionTree.Systems[mapped] != -1)
                    {
                        nodes.Add(new FloorConfigurationLayoutNodeModel
                        {
                            id = decisionTree.Systems[mapped],
                            node = nodeTreeFromDecisionTree(mapped, decisionTree)
                        });
                    }
                }
            }

            return nodes.Count > 0 ? nodes : null;
        }

        private int mappedDependancy(int dependancy)
        {
            int map = -1;
            switch(dependancy)
            {
                case 0:
                    map = 1;
                    break;
                case 1:
                    map = 2;
                    break;
                case 2:
                    map = 4;
                    break;
                case 3:
                    map = 5;
                    break;
            }
            return map;
        }

        private FloorConfigurationIndoorUnitsModel GetFloorConfigurationsIndoorUnits(long configId)
        {
            List<int> indoorUnits = (from unit in this.Db.BuildingFloorConfigurationIndoorUnits
                                                         where unit.ConfigurationId == configId
                                                         where unit.SystemId > -1
                                                         select unit.SystemId).ToList();

            if (indoorUnits.Count == 0) return null;

            return new FloorConfigurationIndoorUnitsModel
            {
                indoorUnit = indoorUnits
            };
        }

        public FloorConfigurationIndoorUnitsModel GetFloorConfigurationsIndoorUnitsForEdit(long configid, long floorid, long buildingid)
        {
            List<FloorConfigurationIndoorUnitModel> indoorUnitListForEdit = (from bfcfgip in this.Db.BuildingFloorConfigurationIndoorUnits
                                                                             where bfcfgip.ConfigurationId == configid
                                                                             select new FloorConfigurationIndoorUnitModel
                                                                             {
                                                                                 configId = bfcfgip.ConfigurationId,
                                                                                 systemId = bfcfgip.SystemId,
                                                                                 id = bfcfgip.Id
                                                                             }).ToList();
            if (indoorUnitListForEdit.Count == 0) return null;

            List<CitySystemModel> systems = this.GetSystemsList();
            systems.Insert(0, new CitySystemModel
                {
                    id = -1,
                    name = "Not Selected"
                });

            FloorConfigurationIndoorUnitsModel indoorUnitsForEdit = new FloorConfigurationIndoorUnitsModel
            {
                configId = configid,
                indoorUnitsToEdit = indoorUnitListForEdit,
                systemsToPickFrom = systems,
                buildingName = this.GetBuildingName(buildingid),
                floorName = this.GetFloorName(floorid),
                configName = this.GetConfigName((int)configid),
                floorId = floorid,
                buildingId = buildingid
            };

            return indoorUnitsForEdit;
        }

        public void SaveApplicationBuildingFloorConfigurationIndoorUnits(FloorConfigurationIndoorUnitsModel model)
        {
            this.Db.ReadOnly = false;

            foreach(var iu in model.indoorUnitsToEdit)
            {
                BuildingFloorConfigurationsIndoorUnit entity = (from bfcgip in this.Db.BuildingFloorConfigurationIndoorUnits
                                  where bfcgip.Id == iu.id
                                  select bfcgip).First();
                entity.SystemId = iu.systemId;
                this.Db.SaveChanges();
            }
        }

        public void SaveApplicationBuildingFloorConfigurationIndoorUnit(FloorConfigurationIndoorUnitModel model)
        {
            this.Db.ReadOnly = false;

            BuildingFloorConfigurationsIndoorUnit entity = new BuildingFloorConfigurationsIndoorUnit
            {
                ConfigurationId = model.configId,
                SystemId = model.systemId
            };

            this.Db.FloorConfigIndoorUnitCreate(entity);
            this.Db.SaveChanges();
        }

        protected virtual FloorConfigurationsModel GetFloorConfigurations(long floorid, string buildingFolderName)
        {
            List<FloorConfigurationModel> configurationsForThisFloor = (from bfc in this.Db.BuildingFloorConfigurations
                                                                        where bfc.FloorId == floorid
                                                                        && bfc.Alternate != true
                                                                        select new FloorConfigurationModel
                                                                        { 
                                                                            id = bfc.FloorConfigId,
                                                                            floorId = bfc.FloorId,
                                                                            name = bfc.Name,
                                                                            systemName = bfc.SystemName,
                                                                            systemSize = bfc.SystemSize,
                                                                            systemType = bfc.SystemType,
                                                                            energy = bfc.Energy,
                                                                            overlayImage = bfc.OverlayImage,
                                                                            systemImage = bfc.SystemImage
                                                                        }).ToList();
            if (configurationsForThisFloor.Count == 0) return null;

            foreach(var fc in configurationsForThisFloor)
            {
                this.prepareFloorConfigurations(fc, buildingFolderName);
            }

            return new FloorConfigurationsModel
            {
                configuration = configurationsForThisFloor
            };
        }

        public ServiceResponse GetFloorConfiguration(long configId){

            FloorConfigurationModel model = (from bfc in this.Db.BuildingFloorConfigurations
                                   where bfc.FloorConfigId == configId
                                    && bfc.Alternate != true
                                   select new FloorConfigurationModel
                                    {
                                        id = bfc.FloorConfigId,
                                        floorId = bfc.FloorId,
                                        name = bfc.Name,
                                        systemName = bfc.SystemName,
                                        systemSize = bfc.SystemSize,
                                        systemType = bfc.SystemType,
                                        energy = bfc.Energy,
                                        overlayImage = bfc.OverlayImage,
                                        systemImage = bfc.SystemImage,
                                        isAlternate = false
                                    }).FirstOrDefault();

            //get info for breadcrumbs

            BuildingFloorModel floor = (from f in this.Db.BuildingFloors
                                        where f.FloorId == model.floorId
                                        select new BuildingFloorModel
                                        {
                                            name = f.Name,
                                            buildingId = f.BuildingId
                                        }).FirstOrDefault();

            model.floorName = floor.name;

            BuildingModel building = (from b in this.Db.Buildings
                                      where b.BuildingId == floor.buildingId
                                      select new BuildingModel
                                      {
                                          name = b.Name,
                                          id = b.BuildingId,
                                          buildingFolderName = b.BuildingFolderName
                                      }).FirstOrDefault();

            model.buildingId = building.id;
            model.buildingName = building.name;

            if(model.overlayImage != null)
            {
                model.overlayImage = this.DCBuildingImageURL + building.buildingFolderName + "/" + model.overlayImage;
            }

            if(model.systemImage != null)
            {
                model.systemImage = this.DCImageRootURL + "systems/" + model.systemImage;
            }
            
            this.Response.Model = model;

            return this.Response;
        }

        public ServiceResponse PostFloorConfigurationImage(HttpRequestBase Request, FloorConfigurationModel model)
        {
            this.Db.ReadOnly = false;

            //if(model.id == null)
            //{
            //    this.Response.AddError("Floor configuration ID is missing, please reload the page and try again");
            //    return this.Response;
            //}

            if (Request != null && Request.Files.Count == 1)
            {
                var file = Request.Files[0];

                if (!file.IsImage())
                {
                    this.Response.AddError("Please upload Image files only");
                    return this.Response;
                }

                    if (file != null && file.ContentLength > 0)
                    {
                        var targetFilePath = Utilities.GetDaikinCityDirectory();
                        var targetFileName = model.id.ToString();

                        if (model.overlayImage != null)
                        {
                            targetFilePath +=  ("images\\buildings\\" + this.GetBuildingName(model.buildingId) + "\\");
                            targetFileName += "_overlay_image";
                        }
                        else if (model.systemImage != null)
                        {
                            targetFilePath += "images\\systems\\";
                            targetFileName += "_system_image";
                        }
                        else
                        {
                            this.Response.AddError("Image type missing, please reload the page and try again");
                            return this.Response;
                        }
                        try
                        {
                            targetFileName += Path.GetExtension(file.FileName).ToLower();
                            file.SaveAs(targetFilePath + targetFileName);
                        }
                        catch (Exception)
                        {
                            this.Response.AddError("Unable to save image, please try again");
                        }

                        //save new filename back to db object
                        BuildingFloorConfiguration entity = (from fc in this.Db.BuildingFloorConfigurations
                                                             where fc.FloorConfigId == model.id
                                                             select fc).FirstOrDefault();

                        if (model.overlayImage != null)
                        {
                            entity.OverlayImage = targetFileName;
                        }
                        else if (model.systemImage != null)
                        {
                            entity.SystemImage = targetFileName;
                        }

                        this.Db.SaveChanges();
                            
                        this.Response.AddSuccess("Image uploaded successfully");
                    }

            }
            else
            {
                this.Response.AddError("Please specify an Image to upload");
            }

            return this.Response;
        }

        private FloorConfigurationsModel GetFloorAlternativeConfigurations(long floorid, string buildingFolderName)
        {
            List<FloorConfigurationModel> altConfigurationsForThisFloor = (from bfc in this.Db.BuildingFloorConfigurations
                                                                        where bfc.FloorId == floorid
                                                                        where bfc.Alternate == true
                                                                        select new FloorConfigurationModel
                                                                        { 
                                                                            id = bfc.FloorConfigId,
                                                                            floorId = bfc.FloorId,
                                                                            name = bfc.Name,
                                                                            systemName = bfc.SystemName,
                                                                            systemSize = bfc.SystemSize,
                                                                            systemType = bfc.SystemType,
                                                                            energy = bfc.Energy,
                                                                            overlayImage = bfc.OverlayImage,
                                                                            systemImage = bfc.SystemImage,
                                                                            isAlternate = true
                                                                        }).ToList();
            if (altConfigurationsForThisFloor.Count == 0) return null;

            foreach(var fc in altConfigurationsForThisFloor)
            {
                this.prepareFloorConfigurations(fc, buildingFolderName);
            }

            return new FloorConfigurationsModel
            {
                configuration = altConfigurationsForThisFloor
            };
        }

        public void SaveApplicationBuildingFloorConfiguration(BuildingFloorEditModel model)
        {
            this.Db.ReadOnly = false;

            if(model.configurations != null)
            {
                foreach (var bfc in model.configurations.configuration)
                {
                    BuildingFloorConfiguration entity = this.Db.QueryBuildingFloorConfiguration(bfc.id);
                    entity.Name = bfc.name;
                    entity.SystemName = bfc.systemName;
                    entity.SystemType = bfc.systemType;
                    entity.SystemSize = bfc.systemSize;
                    entity.Energy = bfc.energy;
                    entity.Alternate = bfc.isAlternate;

                    this.Db.SaveChanges();
                }
            }

            if(model.alternativeConfigurations != null)
            {
                foreach (var bfc in model.alternativeConfigurations.configuration)
                {
                    BuildingFloorConfiguration entity = this.Db.QueryBuildingFloorConfiguration(bfc.id);
                    entity.Name = bfc.name;
                    entity.SystemName = bfc.systemName;
                    entity.SystemType = bfc.systemType;
                    entity.SystemSize = bfc.systemSize;
                    entity.Energy = bfc.energy;
                    entity.Alternate = bfc.isAlternate;

                    this.Db.SaveChanges();
                }
            }
        }

        public BuildingsModel GetFunctionalBuildingsModel()
        {
            BuildingsModel model = new BuildingsModel
            {
                building = (from b in this.Db.Buildings
                            where b.TypeId == (int)BuildingTypeEnums.application
                            select new BuildingModel
                            {
                                id = b.BuildingId,
                                name = b.Name,
                                typeId = b.TypeId,
                                videoIn = b.VideoIn,
                                videoInPoster = b.VideoInPoster,
                                hotspotX = b.HotSpotX,
                                hotspotY = b.HotSpotY,
                                menuImage = b.MenuImage,
                                buildingFolderName = b.BuildingFolderName
                            }).ToList()
            };

            foreach(var b in model.building)
            {
                this.prepareBuilding(b);
            }

            return model;
        }

        public List<BuildingFloorModel> GetFunctionalFloors()
        {
            List<BuildingFloorModel> functionalFloors = (from b in this.Db.Buildings
                                   join f in Db.BuildingFloors on b.BuildingId equals f.BuildingId
                                   where b.TypeId == (int)BuildingTypeEnums.application
                                   select new BuildingFloorModel
                                   {
                                        buildingId = b.BuildingId,
                                        id = f.FloorId,
                                        name = f.Name,
                                        buildingName = b.Name
                                   }).ToList();

            return functionalFloors;
        }

        public BuildingModel GetFunctionalBuildingModel(int BuildingId)
        {
            BuildingModel building = (from b in this.Db.Buildings
                        where b.BuildingId == BuildingId
                        select new BuildingModel
                        {
                            id = b.BuildingId,
                            name = b.Name,
                            typeId = b.TypeId,
                            videoIn = b.VideoIn,
                            videoInPoster = b.VideoInPoster,
                            hotspotX = b.HotSpotX,
                            hotspotY = b.HotSpotY,
                            menuImage = b.MenuImage,
                            buildingFolderName = b.BuildingFolderName
                        }).FirstOrDefault();

            this.prepareBuilding(building);

            return building;
        }

        public BuildingsModel GetApplicationBuildingsModel()
        {
            BuildingsModel model = new BuildingsModel
            {
                building = (from b in this.Db.Buildings
                            where b.TypeId == (int)BuildingTypeEnums.functional
                            select new BuildingModel
                            {
                                id = b.BuildingId,
                                name = b.Name,
                                typeId = b.TypeId,
                                videoIn = b.VideoIn,
                                videoInPoster = b.VideoInPoster,
                                hotspotX = b.HotSpotX,
                                hotspotY = b.HotSpotY,
                                menuImage = b.MenuImage,
                                buildingFolderName = b.BuildingFolderName
                            }).ToList()
            };

            foreach (var b in model.building)
            {
                this.prepareBuilding(b);
                b.floors.floor = b.floors.floor.Where(f => f.typeId == (int)BuildingTypeEnums.functional).ToList();
            }
            return model;
        }

        public BuildingsModel GetApplicationBuildingsPageModel()
        {
            BuildingsModel model = new BuildingsModel
            {
                building = (from b in this.Db.Buildings
                            where b.TypeId == (int)BuildingTypeEnums.functional
                            select new BuildingModel
                            {
                                id = b.BuildingId,
                                name = b.Name
                            }).ToList()
            };

            foreach (var b in model.building)
            {
                b.floors = new BuildingFloorsModel();
                b.floors.floor = (from bf in this.Db.BuildingFloors
                                where bf.BuildingId == b.id
                                where bf.TypeId == (int)BuildingTypeEnums.functional
                                select new BuildingFloorModel
                                {
                                    id = bf.FloorId,
                                    buildingId = bf.BuildingId,
                                    name = bf.Name
                                }).ToList();
            }
            return model;
        }

        public void SaveFunctionalBuildingLinks(BuildingLinksModel model)
        {
            this.Db.ReadOnly = false;

            for(int i = 0; i < model.link.Count; i++)
            {
                BuildingLinkModel linkModel = model.link[i];
                BuildingFloorLink entityLink = Db.QueryBuildingLink(linkModel.id);
                entityLink.Title = linkModel.title ?? "";
                entityLink.Copy = linkModel.description ?? "";
                entityLink.Url = linkModel.url ?? "";
                entityLink.Enabled = linkModel.enabled;

                Db.SaveChanges();
            }
        }

        public DecisionTreeMap GetDecisionTreeMapByConfigId(long configId)
        {
            // pull from db
            List<int> systems = (from system in Db.DecisionTreeSystems
                                                   where system.ConfigId == configId
                                                    orderby system.index
                                                     select system.SystemId).ToList();

            List<int> dependancies = (from dependancy in Db.DecisionTreeDependancies
                                                   where dependancy.ConfigId == configId
                                                    orderby dependancy.index
                                                     select dependancy.SystemIndex).ToList();

            // enforce size
            while (systems.Count < 6) systems.Add(-1); // must have 6 systems
            while (dependancies.Count < 4) dependancies.Add(1); // must have 4 dependancies

            return new DecisionTreeMap
            {
                Systems = systems,
                Dependancies = dependancies
            };
        }

        public void SaveDecisionTree(DecisionTreeMap decisionTreeMap, long configId)
        {
            Db.ReadOnly = false;

            ClearDecisionTreeByConfigId(configId);

            // systems
            for (int i = 0; i < decisionTreeMap.Systems.Count; i++)
            {
                Db.DecisionTreeSystemCreate(new DecisionTreeSystem
                {
                    ConfigId = (int)configId,
                    SystemId = decisionTreeMap.Systems[i],
                    index = (i + 1),
                });
            }

            // dependancies
            for (int i = 0; i < decisionTreeMap.Dependancies.Count; i++)
            {
                Db.DecisionTreeDependancyCreate(new DecisionTreeDependancy
                {
                    ConfigId = (int)configId,
                    index = (i + 1),
                    SystemIndex = decisionTreeMap.Dependancies[i]
                });
            }

            Db.SaveChanges();
        }

        private void ClearDecisionTreeByConfigId(long configId)
        {
            // systems
            List<DecisionTreeSystem> systems = (from system in Db.DecisionTreeSystems
                                                    where system.ConfigId == configId
                                                    select system).ToList();

            foreach (var system in systems)
	        {
                Db.DecisionTreeSystemRemove(system);
	        }

            // dependancies
            List<DecisionTreeDependancy> dependancies = (from dependancy in Db.DecisionTreeDependancies
                                                         where dependancy.ConfigId == configId
                                                        select dependancy).ToList();

            foreach (var dependancy in dependancies)
            {
                Db.DecisionTreeDependancyRemove(dependancy);
            }
        }
        
        #endregion

        #region systems (aka application products)

        public CitySystemsModel GetSystemsModel()
        {
            var model = new CitySystemsModel
            {
                system = this.GetSystemsList()
            };

            return model;
        }

        public List<CitySystemModel> GetSystemsList()
        {
            return (from s in this.Db.CitySystems
                    where s.Deleted == null
                    select new CitySystemModel
                    {
                        id = s.SystemId,
                        name = s.Name,
                        description = s.Description,
                        image = s.Image,
                        icon = s.Icon
                    }).ToList();
        }

        public CitySystemsModel CitySystemsModelConfigFormatted()
        {
            var model = this.GetSystemsModel();

            foreach(var sys in model.system)
            {
                if(sys.image != null)
                {
                    sys.image = this.DCImageRootURL + "systems/" + sys.image;
                }

                if(sys.icon != null)
                {
                    sys.icon = this.DCImageRootURL + "icons/" + sys.icon;
                }
            }

            return model;
        }

        public CitySystemModel GetSingleSystem(long? id)
        {
            if (id == null) return new CitySystemModel();

            var sys = (from s in this.Db.CitySystems
                          where s.SystemId == id
                          where s.Deleted == null
                          select new CitySystemModel
                          {
                              id = s.SystemId,
                              name = s.Name,
                              description = s.Description,
                              image = s.Image,
                              icon = s.Icon
                          }).First();

            return sys;
        }

        public void SaveApplicationProduct(CitySystemModel model)
        {
            this.Db.ReadOnly = false;

            if(model.id == 0)
            {
                CitySystem entity = new CitySystem();
                entity.Name = model.name;
                entity.Description = model.description;
                entity.Image = "";
                entity.Icon = "";
                this.Db.CitySystemCreate(entity);
                this.Db.SaveChanges();

                entity.Image = "system_image_" + entity.SystemId + ".png";
                entity.Icon = "system_icon_" + entity.SystemId + ".gif";
                this.Db.SaveChanges();
            }
            else
            {
                CitySystem entity = this.Db.QueryCitySystem(model.id);
                entity.Name = model.name;
                entity.Description = model.description;

                this.Db.SaveChanges();
            }
        }

        public void SaveApplicationProductImage(HttpRequestBase Request, CitySystemModel model)
        {
            if (Request != null && Request.Files.Count == 1)
            {
                if (model.image == null)
                {
                    model.image = "system_image_" + model.id + ".png";
                }

                string targetFilePath = Utilities.GetDaikinCityDirectory() + "\\images\\systems\\" + model.image;

                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0 && file.ContentType == "image/png")
                {
                    file.SaveAs(targetFilePath);
                }
            }
        }

        public void SaveApplicationProductIcon(HttpRequestBase Request, CitySystemModel model)
        {
            if (Request != null && Request.Files.Count == 1)
            {
                if (model.icon == null)
                {
                    model.icon = "system_icon_" + model.id + ".gif";
                }

                string targetFilePath = Utilities.GetDaikinCityDirectory() + "\\images\\icons\\" + model.icon;

                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0 && file.ContentType == "image/gif")
                {
                    file.SaveAs(targetFilePath);
                }
            }
        }

        public void DeleteApplicationProduct(long systemid)
        {
            this.Db.ReadOnly = false;

            CitySystem entity = this.Db.QueryCitySystem(systemid);
            entity.Deleted = true;

            this.Db.SaveChanges();
        }

        #endregion

        #region billboards
        public BillboardModel GetBillboardModel()
        {
            var model = new BillboardModel
            {
                poster = (from p in this.Db.BillboardPosters
                          select new PosterModel
                          {
                              id = p.PosterId,
                              url = p.Url,
                              image = p.Image,
                              enabled = p.enabled
                          }).ToList()
            };

            return model;
        }

        public BillboardModel GetBillboardModelConfigFormatted()
        {
            var model = this.GetBillboardModel();

            for (var i = 0; i < model.poster.Count; i++)
            {
                model.poster[i].image = this.DCImageRootURL + model.poster[i].image;
            }

            return model;
        }

        public BillboardModel GetSingleBillboard(long id)
        {
            var model = new BillboardModel
            {
                SinglePoster = (from p in this.Db.BillboardPosters
                                where p.PosterId == id
                          select new PosterModel
                          {
                              id = p.PosterId,
                              url = p.Url,
                              image = p.Image,
                              enabled = p.enabled
                          }).First()
            };

            return model;

        }

        #endregion

        #region comms center

        public CommunicationsCentreModel GetCommunicationsModel()
        {
            var model = new CommunicationsCentreModel
            {
                ContactUsLink = "http://daikincity.co.uk/contact.php?local=us",
                YouTubeLink = "https://www.youtube.com/user/DaikinAC",
                videos = new CommunicationsCentreVideosModel
                {
                    video = (from ccv in this.Db.CommsCenterVideos
                             select new CommunicationsCentreVideoModel
                             {
                                 id = ccv.VideoId,
                                 title = ccv.Title,
                                 thumb = ccv.Thumbnail,
                                 url = ccv.Link

                             }).ToList()
                }
            };
            return model;
        }

        public CommunicationsCentreVideoModel GetSingleCommunicationsCenterVideo(long id)
        {
            return (from ccv in this.Db.CommsCenterVideos
                               where ccv.VideoId == id
                               select new CommunicationsCentreVideoModel
                               {
                                   id = ccv.VideoId,
                                   title = ccv.Title,
                                   thumb = ccv.Thumbnail,
                                   url = ccv.Link
                               }).First();
        }

        public CommunicationsCentreModel GetCommunicationsModelConfigFormatted()
        {
            var model = this.GetCommunicationsModel();

            for (var i = 0; i < model.videos.video.Count; i++ )
            {
                if (!model.videos.video[i].thumb.Contains("http"))
                {
                    model.videos.video[i].thumb = this.DCImageRootURL + "buildings/communications-center/" + model.videos.video[i].thumb;
                }
            }

            return model;
        }

        public void SaveCommsCenterVideos(CommunicationsCentreModel model)
        {
            this.Db.ReadOnly = false;

            for (var i = 0; i < model.videos.video.Count; i++)
            {
                CommunicationsCentreVideoModel v = model.videos.video[i];
                CommsCenterVideo ccv = this.Db.QueryCommsCenterVideo(v.id);
                ccv.Title = v.title;
                ccv.Thumbnail = v.thumb;
                ccv.Link = v.url;
            }

            this.Db.SaveChanges();
        }

        #endregion

        #region library

        public LibraryPageModel GetLibraryPage()
        {
            return new LibraryPageModel
            {
                documents = this.GetAllLibraryDirectories()
            };
        }

        public LibraryPageModel GetLibraryPageForJSON()
        {
            return new LibraryPageModel
            {
                documents = this.GetAllLibraryDirectories(true)
            };
        }

        public LibraryDirectoryModel GetAllLibraryDirectories(bool includedocs = false)
        {
            LibraryDirectoryModel model;

            model = (from ldir in this.Db.LibraryDirectories
                         select new LibraryDirectoryModel
                         {
                             depthLevel = 0,
                             id = ldir.LibraryDirectoryId,
                             name = ldir.Name,
                             parentId = ldir.ParentId,
                             Protected = ldir.Protected
                         }).FirstOrDefault();
         

            int depth = 1;

            model.documents = this.GetLibraryDirectories(model.id, depth, includedocs);
            model.childCount = model.documents.Count;

            return model;
        }

        private List<LibraryDirectoryModel> GetLibraryDirectories(int id, int depth, bool includedocs)
        {
            List<LibraryDirectoryModel> directories = (from ldir in this.Db.LibraryDirectories
                                          where ldir.ParentId == id
                                          select new LibraryDirectoryModel
                                          {
                                              id = ldir.LibraryDirectoryId,
                                              name = ldir.Name,
                                              parentId = ldir.ParentId,
                                              Protected = ldir.Protected
                                          }).ToList();

            if(includedocs)
            {
                foreach (var d in directories)
                {
                    d.document = this.GetLibraryDirectoryDocuments(d.id);
                }
            }

            if (directories.Count > 0)
            {
                depth++;

                foreach (var d in directories)
                {
                    d.depthLevel = depth;
                    d.childCount = 0;
                    d.documents = this.GetLibraryDirectories(d.id, depth, includedocs);

                    if(d.documents != null)
                    {
                        d.childCount = d.documents.Count();
                        d.documents = d.documents.OrderBy(x => x.childCount).ToList();
                        
                    }
                    
                }

                return directories;
            }

            return null;
        }

        public List<LibraryDocumentModel> GetLibraryDirectoryDocuments(int dirId)
        {
            List<LibraryDocumentModel> documents = (from relationship in this.Db.LibraryDocumentRelationships
                                                    join document in this.Db.LibraryDocuments
                                                    on relationship.LibraryDirectoryId equals dirId
                                                    where relationship.LibraryDocumentId == document.LibraryDocumentId
                                                    select new LibraryDocumentModel
                                                    {
                                                        id = document.LibraryDocumentId,
                                                        name = document.Name,
                                                        path = (document.Path == null) ? "" : document.Path,
                                                        thumb = (document.Thumb == null) ? "" : document.Thumb
                                                    }).ToList();

            if (documents.Count > 0) return documents;

            return null;
            
        }

        public LibraryDocumentEditModel GetSingleLibraryDocument(int docId)
        {
            LibraryDocumentEditModel doc = (from d in this.Db.LibraryDocuments
                                        where d.LibraryDocumentId == docId
                                        select new LibraryDocumentEditModel
                                        {
                                            id = d.LibraryDocumentId,
                                            name = d.Name,
                                            path = d.Path,
                                            thumb = d.Thumb
                                        }).FirstOrDefault();

            return doc;
        }

        public void UpdateLibraryDocumentTitle(LibraryDocumentEditModel document)
        {
            this.Db.ReadOnly = false;

            LibraryDocument entity = Db.LibraryDocuments.Where(d => d.LibraryDocumentId == document.id).First();
            if(entity != null && document.name != null)
            {
                entity.Name = document.name;
                this.Db.SaveChanges();
            }
        }

        public dynamic AddLibraryDocumentBare(LibraryDocumentEditModel document)
        {
            this.Db.ReadOnly = false;

            LibraryDocument entity = new LibraryDocument
            {
                Name = document.name,
                Path = "",
                Thumb = ""
            };

            this.Db.Context.LibraryDocuments.Add(entity);
            this.Db.SaveChanges();

            // create relationship
            this.Db.Context.LibraryDocumentRelationships.Add(new LibraryDocumentRelationship
            {
                LibraryDirectoryId = document.DirectoryId,
                LibraryDocumentId = entity.LibraryDocumentId
            });
            this.Db.SaveChanges();

            return new { id = entity.LibraryDocumentId, file = document.name };
        }

        public dynamic UpdateLibraryDocumentBare(int documentId, HttpFileCollectionBase files)
        {
            return EditLibraryDocument(GetSingleLibraryDocument(documentId), files);
        }

        public dynamic EditLibraryDocument(LibraryDocumentEditModel document, HttpFileCollectionBase files)
        {
            this.Db.ReadOnly = false;

            dynamic response = new { };

            if (document.name == null)
            {
                response = new
                {
                    id = document.id,
                    success = false,
                    error = "Please specify a document name"
                };

                return response;
            }

            if(document.id == 0)
            {
                LibraryDocument entity = new LibraryDocument();
                entity.Name = document.name;

                if(string.IsNullOrEmpty(document.path))
                {
                    if (files.Count == 0 || files[0].ContentLength == 0)
                    {
                        response = new
                        {
                            id = document.id,
                            success = false,
                            error = "Please specify a file to upload"
                        };

                        return response;
                    }

                    var file = files[0];
                    var fileName = Path.GetFileName(file.FileName);
                    var directory = Utilities.GetDocumentDirectory() + "/DaikinCityDocuments/library/";
                    var filePath = directory + fileName;

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    file.SaveAs(filePath);

                    var hasThumbnail = this.generateThumbnail(filePath);

                    entity.Name = document.name;
                    entity.Path = "daikincityweb/documents/library/" + fileName;

                    if (hasThumbnail)
                    {
                        entity.Thumb = "daikincityweb/documents/library/" + fileName.Replace("pdf", "jpg");
                    }
                }
                else
                {
                    //if not file, assuming it's a link

                     document.path = document.path.ToLower();
                     bool validURL = (document.path != null && (document.path.StartsWith("ftp") || document.path.StartsWith("http") || document.path.StartsWith("https")) );
                    if(!validURL)
                    {
                        response = new
                        {
                            id = document.id,
                            success = false,
                            error = "Please enter a valid URL (starting with http:// or https:// or ftp://)"
                        };

                        return response;
                    }

                    entity.Path = document.path;
                }
                
                this.Db.Context.LibraryDocuments.Add(entity);
                this.Db.SaveChanges();

                LibraryDocumentRelationship firstRelationship = new LibraryDocumentRelationship
                {
                    LibraryDirectoryId = document.DirectoryId,
                    LibraryDocumentId = entity.LibraryDocumentId
                };

                this.Db.Context.LibraryDocumentRelationships.Add(firstRelationship);
                this.Db.SaveChanges();
            }
            else
            {
                LibraryDocument entity = (from doc in this.Db.LibraryDocuments
                                              where doc.LibraryDocumentId == document.id
                                              select doc).FirstOrDefault();

                if (files.Count > 0 && files[0].ContentLength > 0)
                {
                    var file = files[0];
                    var fileName = Path.GetFileName(file.FileName);
                    var directory = Utilities.GetDocumentDirectory() + "/DaikinCityDocuments/library/";
                    var filePath = directory + fileName;

                    this.CheckLibraryDocumentFileReferences(entity.Path, entity.LibraryDocumentId);

                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    file.SaveAs(filePath);

                    entity.Thumb = (this.generateThumbnail(filePath)) ? "daikincityweb/documents/library/" + fileName.Replace("pdf", "jpg") : null;
                    entity.Path = "daikincityweb/documents/library/" + fileName;
                }
                else
                {
                    //if no path got sent back, it's just a title being edited
                    if(document.path != null)
                    {
                        //but if a path did get sent back, check it's valid
                        document.path = document.path.ToLower();
                         bool validURL = (document.path != null && (document.path.StartsWith("ftp") || document.path.StartsWith("http") || document.path.StartsWith("https")) );

                        if (!validURL)
                        {
                            response = new
                            {
                                id = document.id,
                                success = false,
                                error = "Please enter a valid URL (starting with http:// or https:// or ftp://)"
                            };

                            return response;
                        }
                        entity.Path = document.path;
                    }
                }

                entity.Name = document.name;
                this.Db.SaveChanges();
            }

            return new
            {
                id = document.id,
                success = false,
                error = ""
            };
        }

        private void CheckLibraryDocumentFileReferences(string path, int documentId)
        {
            var oldFile = path.Replace("daikincityweb/documents", Utilities.GetDocumentDirectory() + "/DaikinCityDocuments");

            List<LibraryDocument> OtherDocumentsReferencingFile = (from d in this.Db.LibraryDocuments
                                                                   where d.Path == path
                                                                   && d.LibraryDocumentId != documentId
                                                                   select d).ToList();

            if (OtherDocumentsReferencingFile.Count == 0)
            {
                if (File.Exists(oldFile))
                {
                    File.Delete(oldFile);
                    string oldThumbnail = oldFile.Replace("pdf", "jpg");
                    if (File.Exists(oldThumbnail))
                    {
                        File.Delete(oldThumbnail);
                    }
                }
            }
        }

        private Boolean generateThumbnail(String file)
        {
            if (!file.ToLower().Contains(".pdf"))
            {
                return false;
            }

            if (GhostscriptVersionInfo.IsGhostscriptInstalled)
            {
                GhostscriptVersionInfo info = GhostscriptVersionInfo.GetLastInstalledVersion(GhostscriptLicense.GPL | GhostscriptLicense.AFPL, GhostscriptLicense.GPL);
                GhostscriptRasterizer r = new GhostscriptRasterizer();
                r.Open(file, info, false);
                Image img = r.GetPage(72, 72, 1);

                // resize image
                float ratio = Math.Min(140 / (float)img.Width, 180 / (float)img.Height);
                Image thumb = (Image)(new Bitmap(img, new Size((int)(img.Width * ratio), (int)(img.Height * ratio))));
                thumb.Save(file.Replace("pdf", "jpg"), ImageFormat.Jpeg);

                r.Close();

                return true;
            }

            return false;
        }

        public void SaveLibraryDirectoryChange(long DirId, string Dirname)
        {
            this.Db.ReadOnly = false;

            LibraryDirectory entity = (from ld in this.Db.LibraryDirectories
                                           where ld.LibraryDirectoryId == DirId
                                           select ld).FirstOrDefault();

            entity.Name = Dirname;

            this.Db.SaveChanges();
        }

        public void ToggleDirectoryProtection(int dirId)
        {
            this.Db.ReadOnly = false;

            LibraryDirectory entity = (from dir in this.Db.LibraryDirectories
                                       where dir.LibraryDirectoryId == dirId
                                       select dir).FirstOrDefault();
            entity.Protected = !entity.Protected;

            this.Db.SaveChanges();
        }

        public void SaveLibraryDirectoryMove(int? DirId, int? NewParentId)
        {
            this.Db.ReadOnly = false;

            LibraryDirectory entity = (from ld in this.Db.LibraryDirectories
                                       where ld.LibraryDirectoryId == DirId
                                       select ld).FirstOrDefault();

            entity.ParentId = NewParentId;

            this.Db.SaveChanges();
        }

        public void SaveLibrarySubDirectory(int? ParentId, string NewDirName)
        {
            this.Db.ReadOnly = false;

            LibraryDirectory entity = new LibraryDirectory
            {
                Name = NewDirName,
                ParentId = ParentId
            };

            this.Db.LibraryDirectoryCreate(entity);
            this.Db.SaveChanges();
        }

        public bool DeleteLibraryDocument(int DocId, int DirId)
        {
            this.Db.ReadOnly = false;

            List<LibraryDocumentRelationship> relationships = (from ld in this.Db.LibraryDocumentRelationships
                                                         where ld.LibraryDocumentId == DocId
                                                         select ld).ToList();

            if (relationships == null) return false;

            List<LibraryDocumentRelationship> relationshipsToDelete = new List<LibraryDocumentRelationship>();

            foreach(var ldr in relationships)
            {
                if(ldr.LibraryDirectoryId == DirId)
                {
                    relationshipsToDelete.Add(ldr);
                }
            }

            foreach(var ldrtd in relationshipsToDelete)
            {
                this.Db.LibraryDocumentRelationshipDelete(ldrtd);
                this.Db.SaveChanges();
                relationships.Remove(ldrtd);
            }

            if(relationships.Count == 0)
            {
                LibraryDocument entity = (from ld in this.Db.LibraryDocuments
                where ld.LibraryDocumentId == DocId
                select ld).FirstOrDefault();

                if(entity != null)
                {
                    this.CheckLibraryDocumentFileReferences(entity.Path, entity.LibraryDocumentId);

                    this.Db.LibraryDocumentDelete(entity);
                    this.Db.SaveChanges();
                }
            }

            return true;
        }

        public bool DeleteLibraryDirectory(int DirId)
        {
            //delete all relationships involving this directory, and documents if orphaned
            List<LibraryDocumentRelationship> directoryRelationships = (from ld in
                                                                        this.Db.LibraryDocumentRelationships
                                                                        where ld.LibraryDirectoryId == DirId
                                                                        select ld).ToList();

            if(directoryRelationships != null && directoryRelationships.Count > 0)
            {
                foreach (var rel in directoryRelationships)
                {
                    this.DeleteLibraryDocument(rel.LibraryDocumentId, rel.LibraryDirectoryId);
                }
            }
           
            
            //then remove directory
            LibraryDirectory entity = (from ld in this.Db.LibraryDirectories
                                      where ld.LibraryDirectoryId == DirId
                                      select ld).FirstOrDefault();

            if (entity != null)
            {
                this.Db.Context.LibraryDirectories.Remove(entity);
                this.Db.SaveChanges();
            }
            return true;
        }

        public void saveImportedDirectory(LibraryDirectoryModel dir)
        {
            this.Db.ReadOnly = false;

            LibraryDirectory entity = new LibraryDirectory
            {
                Name = dir.name,
                ParentId = dir.parentId,
                Protected = dir.Protected
            };

            this.Db.LibraryDirectoryCreate(entity);
            this.Db.SaveChanges();

            dir.id = entity.LibraryDirectoryId;
        }

        public void saveImportedDocument(LibraryDocumentModel doc, int directoryId)
        {
            this.Db.ReadOnly = false;

            LibraryDocument entity = new LibraryDocument
            {
                Name = doc.name,
                Path = doc.path,
                Thumb = doc.thumb
            };

            this.Db.LibraryDocumentCreate(entity);
            this.Db.SaveChanges();

            LibraryDocumentRelationship cupid = new LibraryDocumentRelationship
            {
                LibraryDirectoryId = directoryId,
                LibraryDocumentId = entity.LibraryDocumentId
            };

            this.Db.LibraryRelationshipCreate(cupid);
            this.Db.SaveChanges();
        }

        #endregion

        #region config

        public virtual CityCMSConfigModel GetConfig()
        {
            var response = new CityCMSConfigModel();
            response.buildings = this.GetBuildingsModelConfigFormatted();
            response.systems = this.CitySystemsModelConfigFormatted();
            response.billboard = this.GetBillboardModelConfigFormatted();
            response.communications = this.GetCommunicationsModelConfigFormatted();
            response.home = this.GetHomeScreenModel();
            return response;
        }

        #endregion

        public void SaveCommsCenterVideoThumb(long videoId, string thumb)
        {
            this.Db.ReadOnly = false;
            CommsCenterVideo entity = Db.QueryCommsCenterVideo(videoId);
            if (entity == null) return;

            entity.Thumbnail = thumb;
            Db.SaveChanges();
        }

        public Boolean MoveLibraryDocument(int OldDirId, int DirId, int DocId)
        {
            Db.ReadOnly = false;
            LibraryDocumentRelationship rel = (from r in Db.LibraryDocumentRelationships
                                       where r.LibraryDirectoryId == OldDirId
                                       where r.LibraryDocumentId == DocId
                                       select r).First();

            if (rel == null) return false;

            rel.LibraryDirectoryId = DirId;
            Db.SaveChanges();
            return true;
        }

        public bool CopyLibraryDocument(int DirId, int DocId)
        {
            //don't copy if exists already
            Db.ReadOnly = false;
            LibraryDocumentRelationship rel = (from r in Db.LibraryDocumentRelationships
                                               where r.LibraryDirectoryId == DirId
                                               where r.LibraryDocumentId == DocId
                                               select r).FirstOrDefault();

            if (rel != null) return true;

            LibraryDocumentRelationship newRel = new LibraryDocumentRelationship
            {
                LibraryDirectoryId = DirId,
                LibraryDocumentId = DocId
            };

            Db.Context.LibraryDocumentRelationships.Add(newRel);
            this.Db.SaveChanges();

            return true;
        }
    }
}
