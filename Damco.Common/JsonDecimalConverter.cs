using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Damco.Common
{
    public class JsonDecimalConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(decimal) || objectType == typeof(decimal?));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is decimal)
            {
                var asString = ((decimal)value).ToString("#0.########################################", new CultureInfo("en-US"));
                if(!asString.Contains("."))
                    asString+=".0";
                writer.WriteRawValue(asString);
            }
            //value = (decimal)value / 1.0000000000000000000000000000M; //Remove trailing zeros
            else
                JToken.FromObject(value).WriteTo(writer);
        }
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

}
