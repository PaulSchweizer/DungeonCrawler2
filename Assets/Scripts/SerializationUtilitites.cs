using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public static class SerializationUtilitites
{
    public static string SerializeToJson(object obj)
    {
        string json = JsonConvert.SerializeObject(obj, Formatting.Indented,
            new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });

        using (var stringReader = new StringReader(json))
        using (var stringWriter = new StringWriter())
        {
            var jsonReader = new JsonTextReader(stringReader);
            var jsonWriter = new JsonTextWriter(stringWriter) { Formatting = Formatting.Indented };
            jsonWriter.Indentation = 4;
            jsonWriter.WriteToken(jsonReader);
            return stringWriter.ToString();
        }
    }

    //public static Dictionary<string, object> DeserializeFromJson(string json)
    //{
    //    return JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
    //}

    //public static Dictionary<string, object> DeserializeFromObject(object obj)
    //{
    //    return DeserializeFromJson(JsonConvert.SerializeObject(obj));
    //}

    public static T DeserializeFromJson<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }

    public static T DeserializeFromObject<T>(object obj)
    {
        return DeserializeFromJson<T>(JsonConvert.SerializeObject(obj));
    }
}

