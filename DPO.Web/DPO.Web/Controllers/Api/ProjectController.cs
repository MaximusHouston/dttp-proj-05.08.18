using AutoMapper;
using DPO.Common;
using DPO.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using DPO.Model.Light;
using DPO.Services.Light;
using System.Net.Mail;
using System.Net.Http;
using System.Net;
using System.IO;
using System.Net.Http.Headers;
using DPO.Web.Controllers.Api.Filters;

namespace DPO.Web.Controllers
{
    [Authorize]
    [AuthenticationFilter]
    public class ProjectController : BaseApiController
    {
        public ProjectServices projectService = new ProjectServices();
        public ProjectServiceLight projectServiceLight = new ProjectServiceLight();

        //[HttpGet]
        //public ServiceResponse GetProject(long? projectId = null) {
        //    return projectService.GetProjectModel( this.CurrentUser, projectId);
        //    //return null;
        //}

        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ServiceResponse GetProjects(ProjectsGridQueryInfo queryInfo)
        {
            ProjectsGridViewModel model = new ProjectsGridViewModel();

            if (queryInfo.Filter != null && queryInfo.Filter.Filters.Find(x => x.Field == "alert") != null && queryInfo.Filter.Filters.Find(x => x.Field == "expirationDays") != null)
            {
                model.ExpirationDays = Int32.Parse(queryInfo.Filter.Filters.Find(x => x.Field == "expirationDays").Value);
            }

            return projectService.GetAllProjects(CurrentUser, model, queryInfo);
        }

        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ServiceResponse EditProjects(ProjectsGridViewModel model)
        {
            return projectService.PostModel(this.CurrentUser, model);
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ServiceResponse GetProject(long? projectId = null)
        {
            return projectService.GetProjectModel(this.CurrentUser, projectId);
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ServiceResponse GetProjectQuotes(long projectId)
        {
            ProjectQuotesModel model = new ProjectQuotesModel
            {
                ProjectId = projectId
            };
            return projectService.GetProjectQuotesModel(this.CurrentUser, model);
        }

        

        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ServiceResponse PostProject(ProjectModel model)
        {
            return projectService.PostModel(this.CurrentUser, model);
        }

        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ServiceResponse PostProjectAndVerifyAddress(ProjectModel model)
        {
            AddressModel addressToVerify = model.ShipToAddress;

            if (addressToVerify.AddressLine1 != null)
            {
                AddressServices addressSvc = new AddressServices();
                ServiceResponse response = addressSvc.VerifyAddress(model.ShipToAddress);
                if (response.IsOK)
                {
                    return projectService.PostModel(this.CurrentUser, model);
                }
                else
                {
                    return response;
                }
                
            }
            else
            {
                return projectService.PostModel(this.CurrentUser, model);
            }

        }

        [HttpDelete]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ServiceResponse DeleteProject(long projectId)
        {

            this.ServiceResponse = projectService.GetProjectModel(this.CurrentUser, projectId);

            //ProcessServiceResponse(this.ServiceResponse);

            if (this.ServiceResponse.IsOK)
            {
                ProjectModel model = this.ServiceResponse.Model as ProjectModel;
                return projectService.Delete(this.CurrentUser, model);
            }

            return this.ServiceResponse;

        }

        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.UndeleteProject })]
        public ServiceResponse UndeleteProject(ProjectModel model)
        {
            return projectService.Undelete(this.CurrentUser, model); ;
        }

        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ServiceResponse DeleteProjects(List<long> projectIds)
        {
            List<ProjectModel> deleteProjectsModel = new List<ProjectModel>();
            foreach (var projectId in projectIds)
            {
                this.ServiceResponse = projectService.GetProjectModel(this.CurrentUser, projectId);
                deleteProjectsModel.Add((ProjectModel)this.ServiceResponse.Model);
            }

            return this.ServiceResponse = projectService.DeleteProjects(this.CurrentUser, deleteProjectsModel);
        }

        [HttpGet]
        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditPipelineData })]
        public ServiceResponse GetNewProjectPipelineNote(long projectId)
        {
            ServiceResponse response = new ServiceResponse();
            var model = new ProjectPipelineNoteModel()
            {
                ProjectId = projectId,
                ProjectPipelineNoteType = new ProjectPipelineNoteTypeModel()

            };
            response.Model = model;
            return response;
        }

        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditPipelineData })]
        public ServiceResponse PostProjectPipelineNote(ProjectPipelineNoteModel model)
        {
            return projectService.AddProjectPipelineNote(this.CurrentUser, model);
        }
        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewPipelineData })]
        public ServiceResponse GetProjectPipelineNotes(long projectId)
        {
            return projectService.GetProjectPipelineNoteListModel(this.CurrentUser, projectId);
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewPipelineData })]
        public ServiceResponse GetProjectPipelineNoteTypes()
        {
            return projectService.GetProjectPipelineNoteTypes(this.CurrentUser);
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ServiceResponse GetProjectLocation(long? projectId)
        {
            return projectServiceLight.GetProjectLocation(this.CurrentUser, projectId);
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ServiceResponse GetSellerInfo(long? projectId)
        {
            return projectServiceLight.GetSellerInfo(this.CurrentUser, projectId);
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ServiceResponse GetDealerContractorInfo(long? projectId)
        {
            return projectServiceLight.GetDealerContractorInfo(this.CurrentUser, projectId);
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ServiceResponse HasOrder(long? projectId)
        {
            return projectServiceLight.HasOrder(this.CurrentUser, projectId);
        }

        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject, SystemAccessEnum.EditProject })]
        public ServiceResponse SaveGridState(GridModel data)
        {
            var grid = data;
            this.ServiceResponse = projectService.SaveGridState(this.CurrentUser, grid);
            return this.ServiceResponse;
        }
    }
}