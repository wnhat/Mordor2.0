using System;
using System.Threading.Tasks;
using CoreClass;
using CoreClass.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using NRediSearch;
using StackExchange.Redis;
//using Redis.OM;
//using Redis.OM.Modeling;

namespace Newspider
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Loger.Testlogger.Information("=================================== ");
            Loger.Testlogger.Information("测试开始；");
            Loger.Testlogger.Information("=================================== ");

            //Loger.Testlogger.Information("删除历史数据");
            //RedisConnector.DeleteALL();
            //Loger.Testlogger.Information("删除完成");

            Loger.Testlogger.Information("manager 启动");
            //Spider.Run();
            while (true)
            {
                MDLhistoryManager.Dispatch();
            }
            Loger.Testlogger.Information("TT 测试 结束；");
            Loger.Testlogger.Information("=================================== ");
            Loger.Testlogger.Information("测试结束；");
            Loger.Testlogger.Information("=================================== ");
        }
    }
}
