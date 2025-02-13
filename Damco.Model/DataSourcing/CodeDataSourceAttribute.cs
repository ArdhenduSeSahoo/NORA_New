using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model.DataSourcing
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CodeDataSourceAttribute: Attribute
    {
        public CodeDataSourceAttribute(string uniqueTag)
        {
            this.Tag = uniqueTag;
        }
        public string Tag { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class CodeDataSourceParameterAttribute : Attribute
    {
        public CodeDataSourceParameterAttribute(string uniqueTag)
        {
            this.Tag = uniqueTag;
        }
        public string Tag { get; private set; }
    }
}
