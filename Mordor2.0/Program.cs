using CoreClass;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using MongoDB;
using MongoDB.Driver;
using MongoDB.Bson;
using CoreClass;
using CoreClass.DICSEnum;
using MongoDB.Bson.Serialization.Attributes;
using CoreClass.Model;
using NetMQ;
using CoreClass.LogSpider;
using System.Threading.Tasks;
using System.Linq;
using System.Xml;
using Serilog;

namespace Mordor2._0
{
    class Program
    {
        public static ILogger Testlogger = new LoggerConfiguration()
                .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose)
                .CreateLogger();

        static string connectstring = "mongodb://172.16.200.100:27017";
        static MongoClient mongoClient = new MongoClient(connectstring);
        /// <summary>
        /// DICS 生产数据库
        /// </summary>
        public static IMongoDatabase DICSDB = mongoClient.GetDatabase("DICSAuto");
        static IMongoCollection<PC> IP = DICSDB.GetCollection<PC>("IP");
        static IMongoCollection<PanelInspectHistory> Result = DICSDB.GetCollection<PanelInspectHistory>("InspectResult");
        static IMongoCollection<ResultFile> AviResult = DICSDB.GetCollection<ResultFile>("AVIResult");
        static IMongoCollection<ResultFile> SviResult = DICSDB.GetCollection<ResultFile>("SVIResult");
        static IMongoCollection<ProductInfo> ProductInfoCollection = DBconnector.DICSDB.GetCollection<ProductInfo>("ProductInfo");
        /// <summary>
        /// DICS 测试数据库
        /// </summary>
        static IMongoDatabase DICS = mongoClient.GetDatabase("DICS");

        static void Main()
        {
            //, { "grade", "$LastJudge" }
            var aggregate = Result.Aggregate()
                .Group(new BsonDocument { { "_id", new BsonDocument {
                    {"StageID", "$StageID" },
                    {"EqpID", "$EqpID" }
                } },
                    { "count", new BsonDocument{ { "$sum", 1 } } },
                    { "IdList",new BsonDocument{ {"$push", "$PanelId" } }} })
                .Group(new BsonDocument { { "_id", "$_id.EqpID" }, { "StageCount",new BsonDocument{ { "$push",new BsonDocument{ { "Stage", "$_id.StageID" },{"count" , "$count" },{"IDlist", "$IdList" } }
            } } } });
                //.Group(new BsonDocument { { "Stage", "$StageID" }, { "count", new BsonDocument("$sum", "$PanelId") } });
            var result = aggregate.ToList();
            foreach (var item in result)
            {
                Testlogger.Information(item.ToJson());
            }
        }
        static void Test()
        {
            string path = @"D:\Program\Mordor2.0\missionTest.xml";
            FileInfo file = new FileInfo(path);
            string xmlstring = file.OpenText().ReadToEnd();
            XmlDocument returnxml = new XmlDocument();
            returnxml.LoadXml(xmlstring);

            var nodelist = returnxml.GetElementsByTagName("PANELID");
            foreach (var item in nodelist)
            {
                Testlogger.Information(item.ToJson());
            }
        }
    }
}
