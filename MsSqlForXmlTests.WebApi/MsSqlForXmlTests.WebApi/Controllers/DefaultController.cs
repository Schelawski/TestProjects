using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Xml;

namespace MsSqlForXmlTests.WebApi.Controllers
{
    public class DefaultController : ApiController
    {
        [Route("Plants")]
        public HttpResponseMessage GetPlants()
        {
            /*
            SELECT TOP (200) * FROM Dictionary FOR XML RAW ('plaints'), ELEMENTS;
            */
            var doc = new XmlDocument();
            doc.Load(System.AppDomain.CurrentDomain.BaseDirectory + @"App_Data\Plants.xml");

            var jsonString = Newtonsoft.Json.JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.None, true);

            var resp = new HttpResponseMessage()
            {
                Content = new StringContent(jsonString)
            };

            resp.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return resp;
        }
    }
}
