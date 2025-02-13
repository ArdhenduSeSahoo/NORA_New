using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Damco.Common
{
    public static class JsonUtils
    {
        public static T GetOneValueFromJson<T>(string json, string fieldName)
        {
            JToken jToken = null;
            JArray jArray = null;
            try
            {
                jArray = JArray.Parse(json);
            }
            catch (JsonReaderException)
            {
                try
                {
                    jToken = JToken.Parse(json);
                }
                catch (JsonReaderException)
                {
                    return default(T);
                }
            }

            if (jArray != null && jArray.Count > 0)
                jToken = jArray[0];
            if (jToken == null)
                return default(T);

            return jToken.Value<T>(fieldName);
        }
    }
}
