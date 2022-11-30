using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;

namespace TIBListener
{
    public class ListenParameter
    {
        public string Name { get; set; }
        public string Detail { get; set; }
        public Dictionary<string, HashSet<string>> Parameters { get; set; }
        public string RedisBufferTarget { get; set; }
        public string RedisTarget { get; set; }
        public string MongoTarget { get; set; }
        public DateTime EditTime { get; set; }
        public static string Serialize(ListenParameter obj)
        {
            var json = JsonConvert.SerializeObject(obj, JsonSerializerSetting.Setting);
            return json;
        }
        public static ListenParameter Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<ListenParameter>(json, JsonSerializerSetting.Setting);
        }
    }
}
