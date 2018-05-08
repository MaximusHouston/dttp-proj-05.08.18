using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using EntityFramework.Extensions;
using DPO.Common;

namespace DPO.Data
{
    public partial class Repository
    {
        #region homescreen
        public IQueryable<HomeScreen> HomeScreen
        {
           get { return this.GetDbSet<HomeScreen>(); }
        }

        public HomeScreen QueryHomeScreen()
        {
            IQueryable<HomeScreen> query;

            query = from h in this.HomeScreen
                    where h.HomeScreenId == 1
                    select h;

            return query.FirstOrDefault();
        }

        #endregion

        #region buildings

        public IQueryable<Building> Buildings
        {
            get { return this.GetDbSet<Building>(); }
        }

        public IQueryable<BuildingFloor> BuildingFloors
        {
            get { return this.GetDbSet<BuildingFloor>(); }
        }

        public IQueryable<BuildingFloorLink> FloorLinks
        {
            get { return this.GetDbSet<BuildingFloorLink>(); }
        }

        public string QueryBuildingNameByFloorId(long? floorId)
        {
            var building = (from floor in BuildingFloors
                            join b in Buildings on
                                floor.BuildingId equals b.BuildingId
                            where floor.FloorId == floorId
                            select b.Name).First();

            return building ?? "";
        }

        public IQueryable<BuildingFloorConfiguration> BuildingFloorConfigurations
        {
            get { return this.GetDbSet<BuildingFloorConfiguration>(); }
        }

        public IQueryable<BuildingFloorConfigurationsIndoorUnit> BuildingFloorConfigurationIndoorUnits
        {
            get { return this.GetDbSet<BuildingFloorConfigurationsIndoorUnit>(); }
        }

        public BuildingFloorConfiguration QueryBuildingFloorConfiguration(long floorid)
        {
            IQueryable<BuildingFloorConfiguration> query = from bfc in this.BuildingFloorConfigurations
                                                           where bfc.FloorConfigId == floorid
                                                           select bfc;
            return query.First();
        }

        public BuildingFloorLink QueryBuildingLink(long id)
        {
            IQueryable<BuildingFloorLink> query = from bl in this.FloorLinks
                                             where bl.LinkId == id
                                             select bl;
            return query.First();
        }

        public IQueryable<DecisionTreeNode> DecisionTreeNodes
        {
            get { return this.GetDbSet<DecisionTreeNode>(); }
        }

        #endregion

        #region billboard posters

        public IQueryable<BillboardPoster> BillboardPosters
        {
            get { return this.GetDbSet<BillboardPoster>(); }
        }

        public BillboardPoster QueryBillboardPoster(long Id)
        {
            IQueryable<BillboardPoster> query = from p in this.BillboardPosters
                                                where p.PosterId == Id
                                                select p;

            return query.First();
        }

        #endregion

        #region systems

        public IQueryable<CitySystem> CitySystems
        {
            get { return this.GetDbSet<CitySystem>(); }
        }

        public CitySystem QueryCitySystem(long Id)
        {
            IQueryable<CitySystem> query = from cs in this.CitySystems
                                where cs.SystemId == Id
                                select cs;
            return query.First();
        }

        public void CitySystemCreate(CitySystem system)
        {
            this.Context.CitySystems.Add(system);
        }

        public void FloorConfigCreate(BuildingFloorConfiguration config)
        {
            this.Context.BuildingFloorConfigurations.Add(config);
        }

        public void FloorConfigIndoorUnitCreate(BuildingFloorConfigurationsIndoorUnit indoorunit)
        {
            this.Context.BuildingFloorConfigurationsIndoorUnits.Add(indoorunit);
        }
        #endregion

        #region library documents

        public IQueryable<LibraryDocument> LibraryDocuments
        {
            get { return this.GetDbSet<LibraryDocument>(); }
        }

        public IQueryable<LibraryDirectory> LibraryDirectories
        {
            get { return this.GetDbSet<LibraryDirectory>(); }
        }

        public IQueryable<LibraryDocumentRelationship> LibraryDocumentRelationships
        {
            get { return this.GetDbSet<LibraryDocumentRelationship>(); }
        }

        public void LibraryDirectoryCreate(LibraryDirectory entity)
        {
            this.Context.LibraryDirectories.Add(entity);
        }

        public void LibraryDocumentCreate(LibraryDocument entity)
        {
            this.Context.LibraryDocuments.Add(entity);
        }

        public void LibraryRelationshipCreate(LibraryDocumentRelationship entity)
        {
            this.Context.LibraryDocumentRelationships.Add(entity);
        }

        public void LibraryDocumentRelationshipDelete(LibraryDocumentRelationship entity)
        {
            this.Context.LibraryDocumentRelationships.Remove(entity);
        }

        public void LibraryDocumentDelete(LibraryDocument entity)
        {
            this.Context.LibraryDocuments.Remove(entity);
        }
        #endregion

        #region comms center videos

        public IQueryable<CommsCenterVideo> CommsCenterVideos
        {
            get { return this.GetDbSet<CommsCenterVideo>(); }
        }

        public CommsCenterVideo QueryCommsCenterVideo(long Id)
        {
            IQueryable<CommsCenterVideo>

            query = from p in this.CommsCenterVideos
                    where p.VideoId == Id
                    select p;

            return query.First();
        }
        #endregion

        #region decision trees

        public IQueryable<DecisionTreeSystem> DecisionTreeSystems
        {
            get { return this.GetDbSet<DecisionTreeSystem>(); }
        }

        public IQueryable<DecisionTreeDependancy> DecisionTreeDependancies
        {
            get { return this.GetDbSet<DecisionTreeDependancy>(); }
        }

        public void DecisionTreeSystemCreate(DecisionTreeSystem systemEntity)
        {
            this.Context.DecisionTreeSystems.Add(systemEntity);
        }

        public void DecisionTreeSystemRemove(DecisionTreeSystem systemEntity)
        {
            this.Context.DecisionTreeSystems.Remove(systemEntity);
        }

        public void DecisionTreeDependancyCreate(DecisionTreeDependancy dependancyEntity)
        {
            this.Context.DecisionTreeDependancies.Add(dependancyEntity);
        }

        public void DecisionTreeDependancyRemove(DecisionTreeDependancy dependancyEntity)
        {
            this.Context.DecisionTreeDependancies.Remove(dependancyEntity);
        }

        #endregion
    }
}
