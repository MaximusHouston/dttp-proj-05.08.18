using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
//using EntityFramework;
//using EntityFramework.Extensions;
//using System.Web.Mvc;
using System.Web;
using System.IO;
//using NPOI.HSSF.UserModel;
//using NPOI.HPSF;
//using NPOI.SS.UserModel;
using DPO.Resources;
using DPO.Domain;
using DPO.Model.Light;
using DPO.Common;

namespace DPO.Services.Light
{
    public class CommonServiceLight : BaseServices
    {
        public CommonServiceLight() : base() { }

        #region Get ContructionTypes
        public ServiceResponse GetContructionTypes(UserSessionModel user)
        {
            var query = from entity in this.Context.ConstructionTypes
                        select new EnumItem
                        {
                            KeyId = entity.ConstructionTypeId,
                            DisplayText = entity.Description
                        };

            var model = query.ToList();

            this.Response.Model = model;

            return this.Response;
        }
        #endregion

        #region Get ProjectStatus Types
        public ServiceResponse GetProjectStatusTypes(UserSessionModel user)
        {
            var query = from entity in this.Context.ProjectStatusTypes
                        select new EnumItem
                        {
                            KeyId = (int)entity.ProjectStatusTypeId,
                            DisplayText = entity.Description
                        };

            var model = query.ToList();

            this.Response.Model = model;

            return this.Response;
        }
        #endregion

        #region Get ProjectTypes
        public ServiceResponse GetProjectTypes(UserSessionModel user)
        {
            var query = from projectType in this.Context.ProjectTypes
                        select new EnumItem
                         {
                             KeyId = projectType.ProjectTypeId,
                             DisplayText = projectType.Description
                         };

            var model = query.ToList();

            this.Response.Model = model;

            return this.Response;
        }
        #endregion

        #region Get ProjectOpenStatus Types
        public ServiceResponse GetProjectOpenStatusTypes(UserSessionModel user)
        {
            var query = from entity in this.Context.ProjectOpenStatusTypes
                        select new EnumItem
                        {
                            KeyId = (int)entity.ProjectOpenStatusTypeId,
                            DisplayText = entity.Description
                        };

            var model = query.ToList();

            this.Response.Model = model;

            return this.Response;
        }
        #endregion

        #region Get VerticalMarket Types
        public ServiceResponse GetVerticalMarketTypes(UserSessionModel user)
        {
            var query = from entity in this.Context.VerticalMarketTypes
                        select new EnumItem
                        {
                            KeyId = (int)entity.VerticalMarketTypeId,
                            DisplayText = entity.Description
                        };

            var model = query.ToList();

            this.Response.Model = model;

            return this.Response;
        }
        #endregion

        #region Get States
        public ServiceResponse GetStates(UserSessionModel user)
        {
            var query = from state in this.Context.States
                        select new EnumItem
                        {
                            KeyId = state.StateId,
                            DisplayText = state.Name
                        };

            var model = query.ToList();

            this.Response.Model = model;

            return this.Response;
        }
        #endregion

        #region GetStateId By StateCode
        public ServiceResponse GetStateIdByStateCode(UserSessionModel user, string stateCode)
        {
            var stateId = this.Db.States.Where(s => s.Code == stateCode).Select(s => s.StateId).First();

            this.Response.Model = stateId;
            return this.Response; 
        }
        #endregion

        #region Get States By Country
        public ServiceResponse GetStatesByCountry(UserSessionModel user, string countryCode)
        {
            var query = from state in this.Context.States
                        where state.CountryCode == countryCode
                        select new StateModel
                        {
                            StateId = state.StateId,
                            CountryCode = state.CountryCode,
                            Name = state.Name,
                            Code = state.Code
                        };

            var model = query.ToList();

            this.Response.Model = model;

            return this.Response;
        }
        #endregion


    }
}


