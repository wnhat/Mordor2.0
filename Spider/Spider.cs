using CoreClass.LogSpider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using CoreClass;
using System.Threading;
using MongoDB.Driver;

namespace Spider
{
    /// <summary>
    /// 每半小时进行一次硬盘搜索，每10分钟进行一次硬盘文件刷新；
    /// </summary>
    public static class Spider
    {
        static DateTime RestartTime = DateTime.Now;

        // Initial Poller;
        public static RouterSocket routerSocket = new RouterSocket(SpiderParameter.Pcip);
        static NetMQTimer PathRefreshTimer = new NetMQTimer(TimeSpan.FromSeconds(900));
        static NetMQTimer OtherTimer = new NetMQTimer(TimeSpan.FromSeconds(900));
        static NetMQPoller Poller = new NetMQPoller { PathRefreshTimer, Spider.routerSocket, OtherTimer};


        static Spider()
        {
            // 为poller绑定触发事件；
            //OtherTimer.Elapsed += LaunchNewResultFile;
            // 初始化爬虫组件；
            InitialSpider();
        }
        internal static void Run()
        {
            Loger.Logger.Information("开始启动；");
            for (int i = 0; i < 20; i++)
            {
                MDLhistoryManager.Run();
            }
            
            Loger.Logger.Information("启动完成；");
            Poller.Run();
        }
        public static void InitialSpider()
        {
            
        }
    }
}
