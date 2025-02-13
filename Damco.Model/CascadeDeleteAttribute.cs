using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CascadeDeleteAttribute: Attribute 
    {
    }
}
