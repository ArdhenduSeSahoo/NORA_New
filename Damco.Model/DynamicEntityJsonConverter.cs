using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Model
{
    public class DynamicEntityJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(DynamicEntity).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            List<string> propertyNames = new List<string>();
            List<object> values = new List<object>();


            if (reader.TokenType == JsonToken.Null) return null;

            JObject jsonObject = JObject.Load(reader);

            //if (jsonObject["DynamicEntity"] != null)
            //{

            foreach (var property in jsonObject.Properties())
            {
                propertyNames.Add(property.Name);
                if (property.Value is JValue)
                    values.Add(((JValue)property.Value).Value);
                else
                    values.Add(property.Value);
            }
            return new DynamicEntity(propertyNames.ToArray(), values.ToArray());
            //}
            //else
            //    return null;

        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dynamicEntity = (DynamicEntity)value;
            writer.WriteStartObject();
            foreach (var property in dynamicEntity.AllProperties())
            {
                if (serializer.NullValueHandling == NullValueHandling.Include || property.Value != null)
                {
                    writer.WritePropertyName(property.Key);
                    serializer.Serialize(writer, property.Value);
                }
            }
            writer.WriteEndObject();
        }
    }

}
