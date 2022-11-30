using CoreClass;
using CoreClass.LogSpider;
using CoreClass.Model;
using CsvHelper;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Newspider
{
    /// <summary>
    /// cell log 搜索组件，可对cell log 的文件状态进行记录和刷新，新生产的panel将会产生新的PanelInspectHistory类；
    /// </summary>
    public class CellLogSpider
    {
        public DateTime SearchDate { get; private set; }
        public PC mainpc;
        public LogSpiderBase spider;
        public LogSpiderBase oldspider;

        public CellLogSpider(PC mainpc)
        {
            this.mainpc = mainpc;
        }
        public string RedisKey
        {
            get { return RedisInfoKey(mainpc.PcIp); }
        }
        public static string RedisInfoKey(string pcip)
        {
            return "spider:info:celllog:" + pcip;
        }
        public string RedisPanelHistoryWaitQueueKey
        {
            get
            {
                return "spider:panel:waitqueue:" + mainpc.EqName;
            }
        }
        static string straptime(DateTime time)
        {
            return time.ToString("yyyy-MM-dd");
        }
        public void StartSearchAuto(DateTime date)
        {
            if (spider == null)
            {
                // 第一次初始化爬虫时将会新建spider，并以当天的cell log 为基准上传数据库；
                string logpath = String.Format(mainpc.CellLogPath, straptime(date));
                SearchDate = date;
                spider = new LogSpiderBase(logpath);
                ManageSearchData(spider);
            }
            else if(date.Day != SearchDate.Day)
            {
                // 当爬虫距离上次运行时间超过一天时，刷取未记录的cell log；
                while (date.Day != SearchDate.Day)
                {
                    ManageSearchData(spider);
                    // 将日期推进一天进行后更新新的logspiderbase；
                    SearchDate += TimeSpan.FromDays(1);

                    string logpath = String.Format(mainpc.CellLogPath, straptime(SearchDate));
                    oldspider = spider;
                    spider = new LogSpiderBase(logpath);
                }
                // 当日期相等时还应刷新当日的cell log 是否有更新
                ManageSearchData(spider);
            }
            else
            {
                ManageSearchData(spider);
            }
            SearchDate = date;
            // 当时间处于凌晨日期交界时，应搜索旧log中的信息，防止由于设备之间的时间差异造成的log丢失问题；
            if (date - date.Date < TimeSpan.FromMinutes(15) && oldspider != null)
            {
                ManageSearchData(oldspider);
            }
        }
        void ManageSearchData(LogSpiderBase log)
        {
            // 从Cell Log中生成新的history；
            PanelInspectHistory[] searchresult = Search(log);

            if (searchresult!= null)
            {
                // 记录生产的ID供刷取检查文件使用;
                foreach (var item in searchresult)
                {
                    // PanelIdQueue.Enqueue(item);
                }

                // TODO: Add error handler, and add to redis server;
                PanelInspectHistory.MongoInsertPanelHistory(searchresult);
                // Add a Wait time to ensure that mongodb has added the objectid;
                Thread.Sleep(5000);
                PanelInspectHistory.RedisInsertPanelHistory(this.RedisPanelHistoryWaitQueueKey,searchresult);
            }
        }
        PanelInspectHistory[] Search(LogSpiderBase log)
        {
            string returnstring;
            try
            {
                returnstring = log.StartSpider();
            }
            catch (Exception e)
            {
                Loger.Logger.Error(e, "在启动log文件爬虫的时候的过程中发生了错误 filepath:{0}", log.FilePath);
                returnstring = null;
            }
            
            if (returnstring == null)
            {
                return null;
            }
            //var all = returnstring.ToCharArray();
            string[] collection = returnstring.Split("\r\n");
            List<PanelInspectHistory> newPanelList = new List<PanelInspectHistory>();

            foreach (var item in collection)
            {
                // the utf8 BOM file are start with char 65279;
                if (!item.StartsWith((char)65279))
                { 
                    try
                    {
                        var newpanel = new PanelInspectHistory(item, mainpc);
                        newPanelList.Add(newpanel);
                    }
                    catch (Exception e)
                    { 
                        //TODO:ADD log when initial instance fail;
                    }
                }
            }
            return newPanelList.ToArray();
        }
        public static string Serialize(CellLogSpider obj)
        {
            BsonDocument buffer = new BsonDocument();
            var writer = new BsonDocumentWriter(buffer);
            BsonSerializer.Serialize<CellLogSpider>(writer, obj);
            var json = buffer.ToJson();
            return json;
        }
        public static CellLogSpider Deserialize(string json)
        {
            return BsonSerializer.Deserialize<CellLogSpider>(json);
        }
    }
}
