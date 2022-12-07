using System;
using System.Threading.Tasks;
using CoreClass;
using CoreClass.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using NRediSearch;
using StackExchange.Redis;


namespace Newspider
{
    /// <summary>
    /// NewSpider 提供对设备端现存图像路径（panelpath）的查询功能，并对AET设备的celllog进行刷取和上传；
    /// 其余log的处理在 Spider（old）中进行；
    /// </summary>
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
            SpiderParameter.initialize(args);
            Spider.Run();

            Loger.Testlogger.Information("TT 测试 结束；");
            Loger.Testlogger.Information("=================================== ");
            Loger.Testlogger.Information("测试结束；");
            Loger.Testlogger.Information("=================================== ");
        }
    }
    public static class SpiderParameter
    {
        public static string Pcip;
        internal static void initialize(string[] args)
        {
            if (args.Length == 0)
            {
                args = new string[] { "172.16.200.100" };
            }
            // "@tcp://172.16.210.22:5554";
            // 产线内实时文件路径服务的端口设置
            Pcip = @"@tcp://" + args[0] + ":5554";
        }
    }
}
