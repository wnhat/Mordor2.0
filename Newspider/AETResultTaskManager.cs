using CoreClass;
using CoreClass.Model;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Newspider
{
    public static class AETResultTaskManager
    {
        public static long Count { get { return RedisConnector.Redis.SetLength("spider:result:waitqueue"); } }
        public static AETResultTask GetTask()
        {
            var task = RedisConnector.Redis.SetPop("spider:result:waitqueue");

            return BsonSerializer.Deserialize<AETResultTask>(task.ToString());
        }
        public static void AddTask(AETResultTask task)
        {
            BsonDocument buffer = new BsonDocument();
            var writer = new BsonDocumentWriter(buffer);
            BsonSerializer.Serialize<AETResultTask>(writer, task);
            var panel = buffer.ToJson();
            RedisConnector.Redis.SetAdd("spider:result:waitqueue", panel);
        }
    }
    public class AETResultTask
    {
        // redis waitqueue key "spider:panel:waitqueue:" + eqname;
        public string RedisKey { get; set; }
        public string Value { get; set; }
        public PanelInspectHistory InspectHistory { get; set; }
        public List<PanelPathContainer> Paths { get; set; }
    }
}
