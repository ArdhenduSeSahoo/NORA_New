using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Damco.Model
{
    public class UTCDateTimeHandler : JsonConverter
    {

        TimeZoneInfo _userTimeZone;
        public UTCDateTimeHandler(TimeZoneInfo usertimezone)
        {           
            _userTimeZone = usertimezone;
        }

        public override bool CanConvert(Type objectType)
        {
            if (objectType == typeof(object) || objectType == typeof(DateTime) || objectType == typeof(DateTime?) || objectType == typeof(String))
                return true;
            else
                return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
             if (reader.Value == null) return null;

            if (reader.Value is DateTime)
            {
                //TODO: Check nulls
                
                return TimeZoneInfo.ConvertTimeToUtc(Convert.ToDateTime(reader.Value), _userTimeZone);
            }
            else
            {
                return reader.Value;
            }
            
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        { 
            if (value is DateTime)
            {
                //TODO: Check nulls
                var date = (DateTime)value;
                if (date.Kind != DateTimeKind.Local)
                    date = TimeZoneInfo.ConvertTimeFromUtc(date, _userTimeZone);
                writer.WriteValue(date);
            }
            else
            {
                writer.WriteValue(value);
            }
        }
    }
}
