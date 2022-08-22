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
using CoreClass.Service;
using CoreClass.DICSEnum;
using MongoDB.Bson.Serialization.Attributes;
using CoreClass.Model;
using NetMQ;
using CoreClass.LogSpider;
using System.Threading.Tasks;
using System.Linq;
using System.Xml;
using Serilog;
using System.Net.Http;

namespace Mordor2._0
{
    class Program
    {
        //public static ILogger Testlogger = new LoggerConfiguration()
        //        .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose)
        //        .CreateLogger();

        //static string connectstring = "mongodb://172.16.200.100:27017";
        //static MongoClient mongoClient = new MongoClient(connectstring);
        ///// <summary>
        ///// DICS 生产数据库
        ///// </summary>
        //public static IMongoDatabase DICSDB = mongoClient.GetDatabase("DICSAuto");
        //static IMongoCollection<PC> IP = DICSDB.GetCollection<PC>("IP");
        //static IMongoCollection<PanelInspectHistory> Result = DICSDB.GetCollection<PanelInspectHistory>("InspectResult");
        //static IMongoCollection<ProductInfo> ProductInfoCollection = DBconnector.DICSDB.GetCollection<ProductInfo>("ProductInfo");
        ///// <summary>
        ///// DICS 测试数据库
        ///// </summary>
        //static IMongoDatabase DICS = mongoClient.GetDatabase("DICS");

        static void Main()
        {
            Test();
        }
        static void Test()
        {
            string ip = "10.141.34.78";
            int port = 28108;
            string part = "EAC";
            var builder = new UriBuilder(Uri.UriSchemeHttp, ip, port, part);
            
            Uri uri = builder.Uri;
            var aa = uri.ToString();

            var a = new CutServerConnector();
            DateTime start = DateTime.Parse("2022-06-17 00:20:01");
            DateTime end = DateTime.Parse("2022-06-17 18:10:00");
            var bb = a.GetInfo(DateTime.Now - TimeSpan.FromDays(8),DateTime.Now);
        }
    }
}
