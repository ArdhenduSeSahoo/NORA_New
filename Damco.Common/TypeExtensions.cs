using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Common
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Returns a friendly string representing the type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetFriendlyName(Type type)
        {
            if (type == typeof(int))
                return "int";
            else if (type == typeof(short))
                return "short";
            else if (type == typeof(byte))
                return "byte";
            else if (type == typeof(bool))
                return "boolean";
            else if (type == typeof(long))
                return "long";
            else if (type == typeof(float))
                return "float";
            else if (type == typeof(double))
                return "double";
            else if (type == typeof(decimal))
                return "decimal";
            else if (type == typeof(string))
                return "string";
            else if (type.IsGenericType)
                return type.GenericTypeArguments[0].Name.ToLower();
            else
                return type.Name;
        }
    }
}
