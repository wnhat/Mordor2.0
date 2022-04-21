using CoreClass.LogSpider;
using CoreClass.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CoreClass
{
    public static class DBconnector
    {
        static string connectstring = "mongodb://172.16.200.100:27017";
        static MongoClient mongoClient = new MongoClient(connectstring);
        /// <summary>
        /// DICS 生产数据库
        /// </summary>
        public static IMongoDatabase DICSDB = mongoClient.GetDatabase("DICSAuto");
        /// <summary>
        /// DICS 测试数据库
        /// </summary>
        static IMongoDatabase DICSTest = mongoClient.GetDatabase("DICSTest");
        static DBconnector()
        {
            // initial mongodb collection;

            // check if collection AETresult exists;
            var collectionList = DICSDB.ListCollections().ToList();
            if (!collectionList.Any(x => x.GetValue("name") == "AETresult"))
            {
                DICSDB.CreateCollection("AETresult");
                // initial AETresult collection;
                DICSDB.GetCollection<AETresult>("AETresult").Indexes.CreateOne(new CreateIndexModel<AETresult>(Builders<AETresult>.IndexKeys.Descending("history._id")));
                DICSDB.GetCollection<AETresult>("AETresult").Indexes.CreateOne(new CreateIndexModel<AETresult>(Builders<AETresult>.IndexKeys.Ascending("PanelId")));
                // create ttl index with field history.InspDate;
                DICSDB.GetCollection<AETresult>("AETresult").Indexes.CreateOne(new CreateIndexModel<AETresult>(Builders<AETresult>.IndexKeys.Descending("history.InspDate"), new CreateIndexOptions { ExpireAfter = TimeSpan.FromDays(40) }));
            }
             // check if collection InspectResult exists;
            if (!collectionList.Any(x => x.GetValue("name") == "InspectResult"))
            {
                DICSDB.CreateCollection("InspectResult");
                // check existed indexes;
                var indexList = DICSDB.GetCollection<PanelInspectHistory>("InspectResult").Indexes.List().ToList();

                // initial InspectResult collection;
                DICSDB.GetCollection<PanelInspectHistory>("InspectResult").Indexes.CreateOne(new CreateIndexModel<PanelInspectHistory>(Builders<PanelInspectHistory>.IndexKeys.Descending("InspDate")));
                DICSDB.GetCollection<PanelInspectHistory>("InspectResult").Indexes.CreateOne(new CreateIndexModel<PanelInspectHistory>(Builders<PanelInspectHistory>.IndexKeys.Ascending("PanelId")));
                // add combine index [EqpID,InspDate] for InspectResult collection;
                DICSDB.GetCollection<PanelInspectHistory>("InspectResult").Indexes.CreateOne(
                    new CreateIndexModel<PanelInspectHistory>(Builders<PanelInspectHistory>.IndexKeys.Combine(
                        Builders<PanelInspectHistory>.IndexKeys.Ascending("EqpID"),
                         Builders<PanelInspectHistory>.IndexKeys.Descending("InspDate"))));
                // 向数据库添加索引，name：eqid;
            }
        }
    }
}