using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.ScreenTemplating
{
    public class IncludeTemplate : Template
    {
        public int PageDesignId { get; set; }
        public PageDesign PageDesign { get; set; }
        public bool ShowOnlyIfLinked { get; set; }
        public int MaximumNestingLevel { get; set; }
    }
}
