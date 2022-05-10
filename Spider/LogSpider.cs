using CoreClass.LogSpider;
using CoreClass.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spider
{
    public interface ILogSpider
    {
        void Start(DateTime date);
        string GetLog();
        string StrapTime(DateTime time);
        void ManageData();
    }
    public class LogSpider :ILogSpider
    {
        public LogSpiderBase spider;
        public DateTime SearchDate { get; private set; }
        // \\172.16.160.57\NetworkDrive\D_Drive\MaruTest\Log\2022-05-05
        public string FrontPath;
        // Cell.csv
        public string FileName;
        public Queue<string> Data = new Queue<string>();

        string Path4Format { get { return FrontPath + "\\" + "{0}" + "\\" +FileName; } }
        public LogSpider(string frontPath, string fileName)
        {
            FrontPath = frontPath;
            FileName = fileName;
        }

        public string StrapTime(DateTime time)
        {
            return time.ToString("yyyy-MM-dd");
        }
        public void Start(DateTime date)
        {
            if (spider == null)
            {
                // 第一次初始化爬虫时将会新建spider，并以当天的cell log 为基准上传数据库；
                string logpath = String.Format(Path4Format, StrapTime(date));
                SearchDate = date;
                spider = new LogSpiderBase(logpath);
                ManageData();
            }
            else if (date.Day != SearchDate.Day)
            {
                // 当爬虫距离上次运行时间超过一天时，刷取未记录的cell log；
                while (date.Day != SearchDate.Day)
                {
                    ManageData();
                    // 将日期推进一天进行后更新新的logspiderbase；
                    SearchDate += TimeSpan.FromDays(1);

                    string logpath = String.Format(Path4Format, StrapTime(SearchDate));
                    if (File.Exists(logpath))
                    {
                        spider = new LogSpiderBase(logpath);
                    }
                }
                // 当日期相等时还应刷新当日的cell log 是否有更新
                ManageData();
            }
            else
            {
                ManageData();
            }
            SearchDate = date;
        }

        public string GetLog()
        {
            if (Data.Count == 0)
            {
                return null;
            }
            else
            {
                return Data.Dequeue();
            }
        }

        public void ManageData()
        {
            try
            {
                string returnstring = spider.StartSpider();
                if (returnstring != null)
                {
                    Data.Enqueue(returnstring);
                }
            }
            catch (Exception e)
            {
                Loger.Logger.Error(e, "在启动log文件爬虫的时候的过程中发生了错误 filepath:{0}", spider.FilePath);
            }
        }
    }
}
