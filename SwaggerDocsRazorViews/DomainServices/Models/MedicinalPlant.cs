using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DomainServices.Models;

namespace DomainServices
{
    /// <summary>
    /// Eintrag einer Pflanze
    /// </summary>
    public class MedicinalPlant
    {
        public MedicinalPlant()
        {            
            this.Tags = new List<string>();            
        }

        #region Properties

        /// <summary>
        /// Das ist die ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name der Pflanze
        /// </summary>
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Synonyms der Pflanze
        /// </summary>
        public string Synonyms { get; set; }

        /// <summary>
        /// Lateinischer Name der Pflanze
        /// </summary>
        public string LatinName { get; set; }

        public string Description { get; set; }
    
        public List<string> Tags { get; set; }
        
        public DateTime CreatedAt { get; set; }

        public LanguageCode LanguageCode { get; set; }

        #endregion
    }    
}