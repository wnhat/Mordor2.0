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
using CoreClass.Model;

namespace Newspider
{
    /// <summary>
    /// 每半小时进行一次硬盘搜索，每10分钟进行一次硬盘文件刷新；
    /// </summary>
    public static class Spider
    {
        static DateTime RestartTime = DateTime.Now;

        // Initial Poller;
        static NetMQTimer PathRefreshTimer = new NetMQTimer(TimeSpan.FromSeconds(600));
        static NetMQTimer OtherTimer = new NetMQTimer(TimeSpan.FromSeconds(600));
        public static RouterSocket routerSocket = new RouterSocket(SpiderParameter.Pcip);

        static NetMQPoller Poller = new NetMQPoller { PathRefreshTimer, OtherTimer };

        /// <summary>
        /// 初始化爬虫时，对各个触发事件进行绑定，注意触发的事件间隔设定；
        /// </summary>
        static Spider()
        {
            // 为poller绑定触发事件；
            PathRefreshTimer.Elapsed += FilePathRefresh;
            OtherTimer.Elapsed += CellLogRefresh;
            // 初始化爬虫组件；

        }
        /// <summary>
        /// 爬虫函数的入口，
        /// 在初始化爬虫时应该将各个组件初始化的先后顺序进行排序，
        /// 在各个组件初始化完成之后再进行服务端口的开启；
        /// </summary>
        internal static void Run()
        {
            Loger.Logger.Information("开始刷新文件夹路径；");
            FilePathManager.RefreshFileList();
            CellLogManager.RefreshData(DateTime.Now - TimeSpan.FromMinutes(16));
            Task.Run(ResultFileAddLoop);
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
        static void AddNewResultFile(AETResultTask task)
        {
            try
            {
                // 正常文件可在500ms传输完成；
                AETresult newAETresult = new AETresult(task.InspectHistory, task.Paths.ToArray());
                if (newAETresult.ResultImages == null && newAETresult.DefectImages == null)
                {
                    // 说明文件为空，或着没有任何图像文件，没有上传意义；
                }
                else
                {
                    // mongodb 添加AETresult；
                    try
                    {
                        AETresult.AETresultCollection.InsertOne(newAETresult);
                    }
                    catch (FormatException e)
                    {
                        Loger.Testlogger.Information("新的检查结果文件mongodb size限制，{0},{1}", e.Message, task.InspectHistory.PanelId);
                    }
                }
            }
            catch (Exception e)
            {
                Loger.Testlogger.Error(e, "在创建新的检查结果文件时发生了错误，请检查详情确认发生错误的原因;{0}", task.InspectHistory);
            }

            RedisConnector.Redis.SetRemove(task.RedisKey, task.Value,StackExchange.Redis.CommandFlags.FireAndForget);
        }
        static void ResultFileAddLoop()
        {
            Loger.Logger.Information("启动AET结果文件添加线程；");
            while (true)
            {
                ResultFileUpload();
                Thread.Sleep(1000);
            }
        }
        static void ResultFileUpload()
        {
            var queuelength = AETResultTaskManager.Count;
            if (queuelength > 0)
            {
                var loopRange = queuelength > 3000 ? 3000 : queuelength;
                Loger.Logger.Information("开始添加AET结果文件；数量为：{0}；剩余任务数量为：{1}",loopRange, queuelength);

                Parallel.For(1, loopRange, i => {
                    AddNewResultFile(AETResultTaskManager.GetTask());
                });
            }
        }
        /// <summary>
        /// 对客户端发送的事件进行分Type响应（按照Message首位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        static void OnMessageArrive(object sender, NetMQSocketEventArgs eventArgs)
        {
            NetMQMessage messageIn = eventArgs.Socket.ReceiveMultipartMessage();
            try
            {
                BaseMessage switchmessage = new BaseMessage(messageIn);
                if (switchmessage.TheMessageType == MessageType.CLINET_GET_PANEL_PATH)
                {
                    PanelPathMessage panelIdInfo = new PanelPathMessage(messageIn);
                    string[] panelid = panelIdInfo.panelPathDic.Keys.ToArray();
                    var pathDict = FilePathManager.GetPanelPathList(panelid);
                    PanelPathMessage newpanelinfomassage = new PanelPathMessage(pathDict);
                    eventArgs.Socket.SendMultipartMessage(newpanelinfomassage);
                }
            }
            catch (VersionException e)
            {
                var newmessage = new BaseMessage(MessageType.VERSION_ERROR);
                eventArgs.Socket.SendMultipartMessage(newmessage);
            }
            catch (Exception e)
            {
                Loger.Logger.Error(e.Message);
            }
        }
    }
}
