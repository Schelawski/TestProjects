using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DomainServices;
using DomainServices.Models;
using SwaggerDocsRazorViews.Infrastructure;

namespace SwaggerDocsRazorViews.Models.Examples
{
    public class MedicinalPlantModelExample : IExamplesProvider
    {
        public object GetExamples()
        {
            return new MedicinalPlant
            {

                Id = 1,
                Name = "Brennessel",
                Synonyms = "Brenn, Nessel",
                LatinName = "",
                Description = "",
                Tags = new List<string>() { "Tag 1", "Tag 2" },
                CreatedAt = DateTime.Parse("05.05.2016"),
                LanguageCode = LanguageCode.de

            };
        }
    }
}