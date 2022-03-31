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
    }
}