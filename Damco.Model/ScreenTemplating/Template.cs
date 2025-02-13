using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.ScreenTemplating
{
    public abstract class Template
    {
        public int Id { get; set; }
        [Required()]
        public string Description { get; set; }
    }
}
