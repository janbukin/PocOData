using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using Microsoft.OData.Edm;
using Microsoft.Owin.Security.OAuth;
using PocOData.Data.Models;
using Document = PocOData.Models.Document;

namespace PocOData
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.MapODataServiceRoute("OData", "odata", GetEdmModel());
        }

        private static IEdmModel GetEdmModel()
        {
            var modelBuilder = new ODataConventionModelBuilder();
            var documentsEntitySet = modelBuilder.EntitySet<Document>("Documents");
            
            // URI: ~/odata/Documents(1)/ODataActionsSample.Models.CheckOut
            var checkOutAction = modelBuilder.EntityType<Document>().Action("CheckOut");
            checkOutAction.ReturnsFromEntitySet<Document>("Documents");

            // URI: ~/odata/Documents(1)/ODataActionsSample.Models.Return
            // Binds to a single entity; no parameters.
            var returnAction = modelBuilder.EntityType<Document>().Action("Return");
            returnAction.ReturnsFromEntitySet<Document>("Documents");
            
            // URI: ~/odata/Documents/ODataActionsSample.Models.CheckOutMany
            // Binds to a collection of entities.  This action accepts a collection of parameters.
            var checkOutManyAction = modelBuilder.EntityType<Document>().Collection.Action("CheckOutMany");
            checkOutManyAction.CollectionParameter<int>("DocumentIDs");
            checkOutManyAction.ReturnsCollectionFromEntitySet<Document>("Documents");

            // URI: ~/odata/CreateDocument
            // Unbound action. It is invoked from the service root.
            var createDocumentAction = modelBuilder.Action("CreateDocument");
            createDocumentAction.Parameter<string>("Title");
            createDocumentAction.ReturnsFromEntitySet<Document>("Documents");

            modelBuilder.Namespace = typeof(Document).Namespace;
            return modelBuilder.GetEdmModel();
        }
    }
}
