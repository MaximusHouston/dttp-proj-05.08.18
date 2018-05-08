using AutoMapper;
using DPO.Common;
using DPO.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using DPO.Services.Light;
using DPO.Model.Light;
using System.Collections.Specialized;
using System.Net.Http.Formatting;
using log4net;
using System.IO;
using System.Net.Http;
using System.Net;

namespace DPO.Web.Controllers
{
    [Authorize]
    public class QuoteController : BaseApiController
    {
        
        public QuoteServices quoteService = new QuoteServices();
        public QuoteServiceLight quoteServiceLight = new QuoteServiceLight();

        
        [HttpGet]
        public ServiceResponse SetBasketQuoteId(long? quoteId = null)
        {
            var serviceResponse = new ServiceResponse();

            var session = HttpContext.Current.Session;
            session["BasketQuoteId"] = quoteId ?? 0;
            CurrentUser.BasketQuoteId = (long?)session["BasketQuoteId"] ?? 0;

            serviceResponse.Model = quoteId;
            return serviceResponse;

        }

        
        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ServiceResponse GetQuoteModel(long? projectId, long? quoteId)
        {
            if (projectId == null && quoteId != null)
            {
                projectId = quoteService.GetProjectIdByQuoteId(this.CurrentUser, quoteId.Value);
            }
            return quoteService.GetQuoteModel(this.CurrentUser, projectId, quoteId);
        }

        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ServiceResponse PostQuote(QuoteModel quoteModel) {
            return quoteService.PostModel(this.CurrentUser, quoteModel);
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ServiceResponse GetQuoteItems(long? quoteId)
        {
            return quoteService.GetQuoteItems(this.CurrentUser, quoteId);
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ServiceResponse GetQuoteItemsModel(long? quoteId)
        {
            QuoteItemsLoadOptions loadOptions = quoteService.getQuoteItemsLoadOptions(quoteId.Value);

            QuoteItemsModel model = new QuoteItemsModel()
            {
                QuoteId = quoteId,
                LoadQuoteItems = loadOptions.LoadQuoteItems,
                LoadDiscountRequests = loadOptions.LoadDiscountRequests,
                LoadCommissionRequests = loadOptions.LoadCommissionRequests,
                LoadQuoteOrders = loadOptions.LoadQuoteOrders
            };
            return quoteService.GetQuoteItemsModel(this.CurrentUser, model);
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ServiceResponse GetOptionItems(long quoteItemId)
        {
           return quoteService.GetOptionItemsModel(this.CurrentUser, quoteItemId);
        }

        [HttpPost]
        public ServiceResponse AdjustQuoteItems(QuoteItemsModel quoteItemsModel)
        {
            return new QuoteServices().AdjustQuoteItems(this.CurrentUser, quoteItemsModel);
        }

        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ServiceResponse QuoteSetActive(QuoteModel quoteModel)
        {
            var projectResponse = new ProjectServices().GetProjectModel(this.CurrentUser, quoteModel.ProjectId);

            if (quoteModel.Project == null)
            {
                quoteModel.Project = projectResponse.Model as ProjectModel;
            }

            return new QuoteServices().SetActive(this.CurrentUser, quoteModel);
        }

        public ServiceResponse QuoteRecalculate(QuoteModel quoteModel)
        {
            return new QuoteServices().QuoteRecalculate(this.CurrentUser, quoteModel);
        }



        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ServiceResponse DeleteQuote(QuoteModel quoteModel)
        {
            return quoteService.Delete(this.CurrentUser, quoteModel, true);
        }

        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public ServiceResponse UndeleteQuote(QuoteModel quoteModel)
        {
            return quoteService.UnDelete(this.CurrentUser, quoteModel, true);
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ServiceResponse HasOrder(long? quoteId)
        {
            return quoteServiceLight.HasOrder(this.CurrentUser, quoteId);
        }

        [HttpGet]
        [Authorise(Accesses = new[] { SystemAccessEnum.ViewProject })]
        public ServiceResponse GetQuoteOptions(long projectId, long quoteId)
        {
            return quoteServiceLight.GetQuoteOptions(this.CurrentUser, projectId, quoteId);
        }

        [HttpPost]
        [Authorise(Accesses = new[] { SystemAccessEnum.EditProject })]
        public HttpResponseMessage QuoteImport() {
            //var serviceResponse = new ServiceResponse();
            //HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Found);
            HttpResponseMessage response = new HttpResponseMessage();
            var httpRequest = HttpContext.Current.Request;
            if (httpRequest.Files.Count > 0)
            {
                HttpPostedFile file = httpRequest.Files[0];
                //return Request.CreateResponse(HttpStatusCode.BadRequest);

                if (file != null && file.ContentLength > 0)
                {
                    long projectId = Convert.ToInt64(httpRequest.Form["ProjectId"]);
                    long quoteId = Convert.ToInt64(httpRequest.Form["QuoteId"]);

                    QuoteModel quoteModel = new QuoteModel
                    {
                        ProjectId = projectId,
                        QuoteId = quoteId
                    };

                    if (Path.GetExtension(file.FileName).ToLower() == ".xls" || Path.GetExtension(file.FileName).ToLower() == ".xlsx" || Path.GetExtension(file.FileName).ToLower() == ".csv")
                    {
                        var csvReader = new StreamReader(file.InputStream);
                        var csv = new CsvHelper.CsvReader(csvReader);
                        //serviceResponse = quoteService.ImportProductsFromCSV(this.CurrentUser, csv, quoteModel);

                        quoteService.ImportProductsFromCSV(this.CurrentUser, csv, quoteModel);
                        response = Request.CreateResponse(HttpStatusCode.Accepted);
                        response.ReasonPhrase = "File imported successfully!";
                    }

                    else if (Path.GetExtension(file.FileName).ToLower() == ".xml")
                    {

                        //serviceResponse = quoteService.ImportProductsFromXML(this.CurrentUser, file, quoteModel);
                        quoteService.ImportProductsFromXML(this.CurrentUser, file, quoteModel);
                        response = Request.CreateResponse(HttpStatusCode.Accepted);
                        response.ReasonPhrase = "File imported successfully!";
                    }
                    else
                    {
                        //serviceResponse.Messages.AddError(Resources.ResourceUI.InvalidFile);

                        response = Request.CreateResponse(HttpStatusCode.NotAcceptable);
                        response.ReasonPhrase = "Invalid File Format!";
                    }
                }
            }else {
                //serviceResponse.Messages.AddWarning("Import file is missing!");
                response = Request.CreateResponse(HttpStatusCode.NotAcceptable);
                response.ReasonPhrase = "Import file is missing!";
            }

            //return serviceResponse;
            return response;
        }
    }
}