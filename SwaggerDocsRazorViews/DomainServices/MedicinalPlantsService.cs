namespace DomainServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models;
    public class MedicinalPlantsService
    {
        List<MedicinalPlant> medicinalPlaints = null;

        public MedicinalPlantsService()
        {
            medicinalPlaints = new List<MedicinalPlant>();

            medicinalPlaints.Add(new MedicinalPlant()
                {
                    Id = 1,
                    Name = "Brennessel",
                    Synonyms = "Brenn, Nessel",
                    LatinName = "",
                    Description = "",
                    Tags = new List<string>() { "Tag 1", "Tag 2" },
                    CreatedAt = DateTime.Parse("05.05.2016"),
                    LanguageCode = Models.LanguageCode.de
                });

            medicinalPlaints.Add(new MedicinalPlant()
            {
                Id = 2,
                Name = "Jungdorn",
                Synonyms = "Jung, Dorn",
                LatinName = "",
                Description = "",
                Tags = new List<string>() { "Tag 1", "Tag 2" },
                CreatedAt = DateTime.Parse("06.05.2016"),
                LanguageCode = Models.LanguageCode.de
            });
        }

        public MedicinalPlant GetById(int id)
        {           
            return this.medicinalPlaints.FirstOrDefault(p => p.Id == id);
        }

        public MedicinalPlant GetByName(string name, LanguageCode languageCode)
        {
            return this.medicinalPlaints.FirstOrDefault(p => p.Name == name && p.LanguageCode == languageCode);
        }
              
        public List<MedicinalPlant> Find(string startWith, LanguageCode languageCode)
        {
            return this.medicinalPlaints.Where(p => p.Name.StartsWith(startWith) && p.LanguageCode == languageCode).ToList();
        }

        public int Create(MedicinalPlant plaint)
        {
            plaint.Id = this.medicinalPlaints.Select(x => x.Id).Max() + 1;
            this.medicinalPlaints.Add(plaint);
            return plaint.Id;
        }
    }
}