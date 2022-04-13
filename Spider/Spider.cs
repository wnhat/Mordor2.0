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

        static List<EqpSpider> eqplist = new List<EqpSpider>();

        // Initial Poller;
        public static RouterSocket routerSocket = new RouterSocket(SpiderParameter.Pcip);
        static NetMQTimer PathRefreshTimer = new NetMQTimer(TimeSpan.FromSeconds(900));
        static NetMQTimer OtherTimer = new NetMQTimer(TimeSpan.FromSeconds(900));
        static NetMQPoller Poller = new NetMQPoller { PathRefreshTimer, Spider.routerSocket, OtherTimer};

        static IMongoCollection<EqpSpider> Spiders = DBconnector.DICSDB.GetCollection<EqpSpider>("SearchSpider");

        static Spider()
        {
            // 为poller绑定触发事件；
            routerSocket.ReceiveReady += OnMessageArrive;
            PathRefreshTimer.Elapsed += FilePathRefresh;
            OtherTimer.Elapsed += LaunchNewResultFile;
            // 初始化爬虫组件；
            InitialSpider();
        }
        internal static void Run()
        {
            Loger.Logger.Information("开始刷新文件夹路径；");
            FileManager.RefreshFileList();
            LaunchNewResultFile();
            Loger.Logger.Information("启动完成");
            Poller.Run();
        }
        public static void InitialSpider()
        {
            // 初始化爬虫时应注意历史上数据不要重复添加；
            for (int i = 1; i <= 32; i++)
            {
                var eqp = FindEqpSpider(i);
                if (eqp == null)
                {
                    // mongodb 在添加新document时会从服务器获取ID项；
                    var neweqp = new EqpSpider(i);
                    Spiders.InsertOne(neweqp);
                    eqp = neweqp;
                }
                eqp.InitialComponent();
                eqplist.Add(eqp);
            }
        }
        public static EqpSpider FindEqpSpider(int num)
        {
            var filter = Builders<EqpSpider>.Filter.Eq("EqpId", num);
            return Spiders.Find(filter).FirstOrDefault();
        }
        public static void UpdateSpider()
        {
            foreach (var item in eqplist)
            {
                var filter = Builders<EqpSpider>.Filter.Eq("EqpId", item.EqpId);
                Spiders.ReplaceOne(filter, item);
            }
        }
        static void FilePathRefresh(object sender, NetMQTimerEventArgs eventArgs)
        {
            FileManager.RefreshFileList();
        }
        static void LaunchNewResultFile(object sender, NetMQTimerEventArgs eventArgs)
        {
            LaunchNewResultFile();
        }
        static void LaunchNewResultFile()
        {
            Loger.Logger.Information("开始刷新cell log;");

            DateTime searchDate = DateTime.Now;
            Parallel.ForEach(eqplist, (eq) => { eq.SearchAuto(searchDate); });
            UpdateSpider();

            Loger.Logger.Information("开始上传新添加的resultfile");
            DateTime resultFileAddBeferDate = DateTime.Now - TimeSpan.FromMinutes(30);
            try
            {
                Parallel.ForEach(eqplist, (eq) => { eq.AddNewResultAuto(resultFileAddBeferDate); });
            }
            catch (Exception e)
            {
                Loger.Logger.Error(e, "添加新的resultfile时发生了异常，需调查异常发生的情况");
            }

            Loger.Logger.Information("添加完成；");
        }
        static void OnMessageArrive(object sender, NetMQSocketEventArgs eventArgs)
        {
            /* 对客户端发送的事件进行分Type响应（按照Message首位）*/
            NetMQMessage messageIn = eventArgs.Socket.ReceiveMultipartMessage();
            try
            {
                BaseMessage switchmessage = new BaseMessage(messageIn);
                if (switchmessage.TheMessageType == MessageType.CLINET_GET_PANEL_PATH)
                {
                    PanelPathMessage panelIdInfo = new PanelPathMessage(messageIn);
                    string[] panelid = panelIdInfo.panelPathDic.Keys.ToArray();
                    var pathDict = FileManager.GetPanelPathList(panelid).PathDict;
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
