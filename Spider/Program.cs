using System;
using CoreClass.LogSpider;
using CoreClass.Model;
using CoreClass;
using CoreClass.DICSEnum;
using System.Collections.Generic;
using CoreClass.Element;
using MongoDB.Driver;
using MongoDB.Bson;
using System.IO;
using NetMQ;
using NetMQ.Sockets;
using System.Threading.Tasks;
using System.Threading;

namespace Spider
{
    class Program
    {
        static void Main(string[] args)
        {
            Run();
        }
        static void Run()
        {
            Spider.Run();
        }
        static void Test()
        {
            Loger.Testlogger.Information("开始");
            FileManager.RefreshFileList();
            EqpSpider eqp = new EqpSpider(1);
            var DATE = DateTime.Parse("2020/1/24");
            eqp.SearchAuto(DateTime.Now - TimeSpan.FromDays(1));
            eqp.AddNewResultAuto(DateTime.Today - TimeSpan.FromDays(1) + TimeSpan.FromHours(1));
            var a = 1;
        }
    }
}
