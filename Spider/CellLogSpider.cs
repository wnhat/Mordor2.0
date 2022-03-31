using CoreClass;
using CoreClass.LogSpider;
using CoreClass.Model;
using CsvHelper;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Spider
{
    /// <summary>
    /// cell log 搜索组件，可对cell log 的文件状态进行记录和刷新，新生产的panel将会产生新的PanelInspectHistory类；
    /// </summary>
    public class CellLogSpider
    {
        public DateTime SearchDate { get; private set; }
        public Queue<PanelInspectHistory> PanelIdQueue = new Queue<PanelInspectHistory>();
        public PC mainpc;
        public LogSpiderBase spider;

        public static event EventHandler<string> EInitialPanelInspectHistoryError;

        public CellLogSpider(PC mainpc)
        {
            this.mainpc = mainpc;
        }
        public PanelInspectHistory GetNewId()
        {
            if (PanelIdQueue.Count == 0)
            {
                return null;
            }
            else
            {
                return PanelIdQueue.Dequeue();
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
                ManageSearchData();
            }
            else if(date.Day != SearchDate.Day)
            {
                // 当爬虫距离上次运行时间超过一天时，刷取未记录的cell log；
                while (date.Day != SearchDate.Day)
                {
                    ManageSearchData();
                    // 将日期推进一天进行后更新新的logspiderbase；
                    SearchDate += TimeSpan.FromDays(1);

                    string logpath = String.Format(mainpc.CellLogPath, straptime(SearchDate));
                    if (File.Exists(logpath))
                    {
                        spider = new LogSpiderBase(logpath);
                    }
                }
                // 当日期相等时还应刷新当日的cell log 是否有更新
                ManageSearchData();
            }
            else
            {
                ManageSearchData();
            }
            SearchDate = date;
        }
        void ManageSearchData()
        {
            // 从Cell Log中生成新的history；
            PanelInspectHistory[] searchresult = Search();

            if (searchresult!= null)
            {
                // 记录生产的ID供刷取检查文件使用;
                foreach (var item in searchresult)
                {
                    PanelIdQueue.Enqueue(item);
                }

                // TODO: Add error handler;
                PanelInspectHistory.InsertPanelHistory(searchresult);
            }
        }
        PanelInspectHistory[] Search()
        {
            string returnstring;
            try
            {
                returnstring = spider.StartSpider();
            }
            catch (Exception e)
            {
                Loger.Logger.Error(e, "在启动log文件爬虫的时候的过程中发生了错误 filepath:{0}", spider.FilePath);
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
                        EInitialPanelInspectHistoryError?.Invoke(this,e.Message);
                        //TODO:ADD log when initial instance fail;
                    }
                }
            }
            return newPanelList.ToArray();
        }
    }
}
