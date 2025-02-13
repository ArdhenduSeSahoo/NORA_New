using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.Workflow
{
    public class ApplicationTag: IEntity
    {
        public int Id { get; set; }
        public string Tag { get; set; }
    }
}
