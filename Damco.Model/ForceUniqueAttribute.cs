using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ForceUniqueAttribute : Attribute
    {
        public ForceUniqueAttribute() { }
        public ForceUniqueAttribute(string combinationKey)
        {
            this.CombinationKey = combinationKey;
        }
        public string CombinationKey { get; private set; }
    }
}
