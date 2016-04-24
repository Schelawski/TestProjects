using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Swashbuckle.Swagger;

namespace SwaggerDocsRazorViews.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            ViewBag.SwaggerDocument = GetSwaggerDocument();

            return View();
        }

        public ActionResult Details(string operationId)
        {
            ViewBag.Title = "Details";

            var doc = GetSwaggerDocument();
            ViewBag.SwaggerDocument = doc;
            ViewBag.OperationId = operationId;

            return View("Details");
        }

        public ActionResult SwaggerJson()
        {
            ViewBag.Title = "Swagger Json";

            var doc = GetSwaggerDocument();
            ViewBag.SwaggerDocument = doc;

            var json = JsonConvert.SerializeObject(doc, Formatting.Indented);            

            ViewBag.SwaggerJson = json.Replace(Environment.NewLine, "<br/>").Replace(Environment.NewLine, "<br/>").Replace(" ", "&nbsp;");

            return View("SwaggerJson");
        }

        public static SwaggerDocument GetSwaggerDocument()
        {
            string json = GetSwaggerJson();

            var doc = JsonConvert.DeserializeObject<SwaggerDocument>(json, new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                MetadataPropertyHandling = MetadataPropertyHandling.Ignore
            });

            return doc;
        }

        public static string GetSwaggerJson()
        {
            using (WebClient wc = new WebClient())
            {
                wc.Encoding = Encoding.UTF8;

                string baseUrl = string.Concat(System.Web.HttpContext.Current.Request.Url.Scheme, "://", System.Web.HttpContext.Current.Request.Url.Authority, System.Web.HttpContext.Current.Request.ApplicationPath.TrimEnd('/'), "/");

                return wc.DownloadString(baseUrl + "swagger/docs/v1");
            }
        }
    }
}
