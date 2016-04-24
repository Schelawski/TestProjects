using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using DomainServices;
using DomainServices.Models;
using SwaggerDocsRazorViews.Infrastructure;
using SwaggerDocsRazorViews.Models;
using SwaggerDocsRazorViews.Models.Examples;
using Swashbuckle.Swagger.Annotations;

namespace SwaggerDocsRazorViews
{
    /// <summary>
    /// Medicinal Plants API
    /// </summary>   
    [RoutePrefix("api/MedicinalPlants")]
    public class MedicinalPlantsController : ApiController
    {
        MedicinalPlantsService service = null;
        /// <summary>
        /// 
        /// </summary>
        public MedicinalPlantsController() {
            service = new MedicinalPlantsService();
        }

        /// <summary>
        /// Eine Pflanze anhand Id
        /// </summary>
        /// <remarks>
        /// Nach einer Pflanze anhand der Id suchen
        /// </remarks>
        /// <param name="id">Id der Pflanze</param>
        /// <returns></returns>        
        [Route("{id:int}")]
        [ResponseType(typeof(MedicinalPlant))]
        public async Task<HttpResponseMessage> GetById(int id)
        {
            var plaint = await Task.Run(() => service.GetById(id));

            if (plaint == null) {
                return Request.CreateResponse(HttpStatusCode.NotFound);
            }

            return Request.CreateResponse(HttpStatusCode.OK, plaint);
        }


        /// <summary>
        /// Liste der Pflanzen
        /// </summary>
        /// <remarks>
        /// Eine Pflanze oder mehrere Pflanzen mit dem angegebenen Namen oder Anfangsbuchstaben und Sprachcode zurückgeben oder leere Liste, wenn keine Übereinstimmung vorhanden ist.
        /// </remarks>
        /// <param name="startWith">Fängt mit dem Buchstaben an</param>
        /// <param name="name">Name der Pflanze</param>
        /// <param name="languageCode">Sprache</param>       
        /// <returns></returns>
        [HttpGet]       
        [ResponseType(typeof(List<MedicinalPlant>))]
        [SwaggerDefaultValue("name", "Judendorn")]        
        [SwaggerResponse(HttpStatusCode.InternalServerError, "Fehler aufgeteten", typeof(ApiError), Description = "InternalServerError")]
        [SwaggerResponse((HttpStatusCode)422, "Etwas stimmt nicht", typeof(ApiError), Description = "UnprocessableEntity")]
        [SwaggerOperation(Schemes = new string[] { "ApiError" })]
        //[ResponseCode(StatusCode = (int)HttpStatusCode.BadRequest, Description = "Value must be greater than zero.")]
        public async Task<HttpResponseMessage> Find(string startWith = null, string name = "Judendorn", LanguageCode languageCode = LanguageCode.de)
        {
            if (string.IsNullOrEmpty(name))
            {
                var plants = await Task.Run(() => service.GetByName(name, languageCode));
                return Request.CreateResponse(HttpStatusCode.OK, plants);
            }
            else
            {
                var plants = await Task.Run(() => service.Find(startWith, languageCode));
                return Request.CreateResponse(HttpStatusCode.OK, plants);
            }               
        }

        /// <summary>
        /// Pflanze einfügen
        /// </summary>
        /// <remarks>
        /// Fügt eine Pflanze hinzu
        /// </remarks>
        /// <param name="plaint">Neue Pflanze</param>
        /// <returns></returns>  
        [HttpPost]        
        [ResponseType(typeof(Int32))]
        [SwaggerRequestExamples(typeof(MedicinalPlant), typeof(MedicinalPlantModelExample))]
        public async Task<HttpResponseMessage> Create(MedicinalPlant plaint)
        {
            var id = await Task.Run(() => service.Create(plaint));

            return Request.CreateResponse(HttpStatusCode.OK, id);
        }       

    }    
}
