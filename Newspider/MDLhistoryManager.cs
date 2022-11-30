using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using CoreClass;
using CoreClass.Model;
using NRediSearch;
using NReJSON;
using System.Globalization;

namespace Newspider
{
    public static class MDLhistoryManager
    {
        static Client _rediSearchClient = new Client("mdl_history_index", RedisConnector.Redis);
        public static void Run()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var result = Dispatch();
                    if (!result)
                    {
                        break;
                    }
                }
            });
        }
        public static bool Dispatch() 
        {
            try
            {
                var newstring = RedisConnector.Redis.SetPop("tib:waitqueue:mdl");
                if (newstring.HasValue)
                {
                    // initial new panel in mdl
                    XmlDocument xmlDocument = new XmlDocument();
                    xmlDocument.LoadXml(newstring);
                    PanelHistoryMDL panel = new PanelHistoryMDL(xmlDocument);
                    // add to mongodb;
                    PanelHistoryMDL.Collection.InsertOne(panel);
                    // add to redisdb;
                    string json = PanelHistoryMDL.Serialize(panel);
                    //RedisConnector.Redis.StringSet("panel:history:mdl:" + panel.ID.ToString(), json);
                    RedisConnector.Redis.JsonSet("panel:history:mdl:" + panel.ID.ToString(), json);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Loger.Logger.Error(e,"从redis MDL history buffer中获取记录时出现异常情况");
            }
            return true;
        }
        public static bool AddDailyYeild(PanelHistoryMDL panel)
        {
            string year = panel.InspectTime.Year.ToString();
            string month = panel.InspectTime.Month.ToString();
            string day = panel.InspectTime.Day.ToString();
            string week = ISOWeek.GetWeekOfYear(panel.InspectTime).ToString();
            string rediskeyday = "yeild:mdl:daily:" + year + "/" + month + "/" + day;
            string rediskeyweek = "yeild:mdl:week:" + year + "/" + week;
            string rediskeymonth = "yeild:mdl:month:" + year + "/" + month;
            string rediskeyyear = "yeild:mdl:year:"+ year;

            string[] keys = new string[] { rediskeyday, rediskeyweek, rediskeymonth, year };
            foreach (var key in keys)
            {
                //// 总览数据；
                //RedisConnector.Redis.JsonIncrementNumber(key, "$.total_input", 1);
                //RedisConnector.Redis.JsonIncrementNumber(key, panel.PanelGrade, 1);
                //RedisConnector.Redis.HashIncrement(key + ":product", panel.FGcode);
                //RedisConnector.Redis.json
                //// 型号别数据；
                //RedisConnector.Redis.HashIncrement(key + ":" + panel.FGcode, panel.FGcode);
                //// MDL线体别数据；

                //// 
            }


            return false;
        }
        // 检查该panel是否有受关注的事件，对事件的中良率数据进行修改；
        public static bool CheckPanelEvent()
        {
            return true;
        }
    }
}
