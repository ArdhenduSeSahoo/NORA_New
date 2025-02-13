using Newtonsoft.Json.Linq;
using System.Collections;

namespace Damco.Common
{
    public static class ConvertUtils
    {
        public static object ChangeType(object value, Type conversionType)
        {
            //Fix issues that the normal Convert.ChangeType has with nullables
            if (Nullable.GetUnderlyingType(conversionType) != null) //it is a nullable type
            {
                if (value == null)
                    return null;
                else
                    conversionType = Nullable.GetUnderlyingType(conversionType);
            }
            if (conversionType.IsEnum)
            {
                if (value is string)
                    return Enum.Parse(conversionType, (string)value);
                else
                    return Enum.ToObject(conversionType, ConvertUtils.ChangeType(value, conversionType.GetEnumUnderlyingType()));
            }
            else if (conversionType != null && value != null && conversionType.IsAssignableFrom(value.GetType()))
                return value;
            else if (conversionType.IsArray && value is IEnumerable)
            {
                var arrayType = conversionType.GetElementType();
                var result = new ArrayList();
                foreach (var elementValue in ((IEnumerable)value))
                    result.Add(ConvertUtils.ChangeType(elementValue, arrayType));
                return result.ToArray(arrayType);
            }
            else if (value is IDictionary dictionary)
                return value.CopyMatchingPropertiesToNew(conversionType, true);
            else if (conversionType.FullName.StartsWith("NORA.Model.") && value != null && value.GetType().FullName == "Newtonsoft.Json.Linq.JObject") 
            {
                JObject jObject = (JObject) value;
                return jObject.ToObject(conversionType);
            }
            else
                return Convert.ChangeType(value, conversionType);
        }
    }
}
