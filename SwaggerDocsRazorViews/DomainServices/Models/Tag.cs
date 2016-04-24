using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainServices.Models
{
    public class Tag
    {
        public string Name { get; set; }
        public List<dynamic> _Links { get; set; }
    }
}
