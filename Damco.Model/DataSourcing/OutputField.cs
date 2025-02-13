using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.DataSourcing
{
    public class OutputField
    {
        public object RangeStart { get; set; }
        public object RangeEnd { get; set; }
        public bool IsRowType { get; set; }
        public DataField DataField { get; set; }
        public string OutputName { get; set; }
        public int? RequiredById { get; set;  }
        public string RequiredByTag { get; set; }
    }
}
