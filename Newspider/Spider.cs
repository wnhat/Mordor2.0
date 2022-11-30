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

namespace Newspider
{
    /// <summary>
    /// 每半小时进行一次硬盘搜索，每10分钟进行一次硬盘文件刷新；
    /// </summary>
    public static class Spider
    {
        static DateTime RestartTime = DateTime.Now;

        // Initial Poller;
        static NetMQTimer PathRefreshTimer = new NetMQTimer(TimeSpan.FromSeconds(900));
        static NetMQTimer OtherTimer = new NetMQTimer(TimeSpan.FromSeconds(900));
        static NetMQTimer MDLhistoryTimer = new NetMQTimer(TimeSpan.FromSeconds(60));

        static NetMQPoller Poller = new NetMQPoller { PathRefreshTimer, OtherTimer, MDLhistoryTimer };

        static Spider()
        {
            // 为poller绑定触发事件；
            PathRefreshTimer.Elapsed += FilePathRefresh;
            OtherTimer.Elapsed += CellLogRefresh;
            MDLhistoryTimer.Elapsed += MDLhistoryRefresh;
            // 初始化爬虫组件；

        }
        internal static void Run()
        {
            Loger.Logger.Information("开始插入MDL History");
            MDLhistoryManager.Run();
            Loger.Logger.Information("开始刷新文件夹路径；");
            FilePathManager.RefreshFileList();
            CellLogManager.RefreshData(DateTime.Now - TimeSpan.FromMinutes(16));
            Loger.Logger.Information("启动完成");
            Poller.Run();
        }
        static void FilePathRefresh(object sender, NetMQTimerEventArgs eventArgs)
        {
            FilePathManager.RefreshFileList();
        }
        static void CellLogRefresh(object sender, NetMQTimerEventArgs eventArgs)
        {
            CellLogManager.RefreshData(DateTime.Now - TimeSpan.FromMinutes(16));
        }
        static void MDLhistoryRefresh(object sender, NetMQTimerEventArgs eventArgs)
        {
            //MDLhistoryManager.Run();
        }
    }
}
