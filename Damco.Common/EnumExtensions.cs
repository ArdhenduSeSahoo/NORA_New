using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Common
{
    public static class EnumExtensions
    {
        public static bool HasAnyFlag<T>(this T enum1, T enum2)
            where T : struct, IConvertible //can't do only enums
        {
            Enum enm1 = (Enum)(object)enum1;
            Enum enm2 = (Enum)(object)enum2;
            foreach (Enum value in Enum.GetValues(typeof(T)))
            {
                if (!object.Equals(value, default(T)) && enm1.HasFlag(value) && enm2.HasFlag(value))
                    return true;
            }
            return false;
        }
    }
}
