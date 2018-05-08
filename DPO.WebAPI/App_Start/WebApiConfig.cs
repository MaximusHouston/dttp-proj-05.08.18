using DPO.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.OData.Builder;
using System.Web.Http.OData.Extensions;

namespace DPO.WebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.AddODataQueryFilter();
            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Product>("Products");
            builder.EntitySet<DocumentProductLink>("DocumentProductLinks");
            builder.EntitySet<Document>("Documents");
            builder.EntitySet<ProductAccessory>("ProductAccessories");
            builder.EntitySet<ProductSpecification>("ProductSpecifications");
            builder.EntitySet<ProductSpecificationLabel>("ProductSpecificationLabels");
            builder.EntitySet<ProductSpecificationKeyLookup>("ProductSpecificationKeyLookups");
            builder.EntitySet<QuoteItem>("QuoteItems");
            builder.EntitySet<Quote>("Quotes");
           
            builder.EntitySet<Business>("Businesses");
            builder.EntitySet<Address>("Addresses");
            builder.EntitySet<State>("States");
            builder.EntitySet<Country>("Countries");
            builder.EntitySet<Project>("Projects");
            builder.EntitySet<ProjectTransfer>("ProjectTransfers");
            builder.EntitySet<User>("Users");
            builder.EntitySet<UserBasketItem>("UserBasketItems");

            builder.EntitySet<AccountMultiplier>("AccountMultipliers");
            builder.EntitySet<MultiplierType>("MultiplierTypes");
            builder.EntitySet<MultiplierTypesMultiplierCategoryType>("MultiplierTypesMultiplierCategoryTypes");
            builder.EntitySet<ProductFamily>("ProductFamilies");
            builder.EntitySet<ProductSubFamily>("ProductSubFamilies");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");
            //builder.EntitySet<Product>("Products");

            //builder.EntitySet<Product>("Products");




            config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
