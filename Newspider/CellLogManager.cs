using CoreClass;
using CoreClass.DICSEnum;
using CoreClass.Model;
//using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using StackExchange.Redis;

namespace Newspider
{
    /// <summary>
    /// 刷取cell log 及 AET设备产生的检查结果文件（result file）
    /// </summary>
    public static class CellLogManager
    {
        static List<CellLogSpider> spiders = new List<CellLogSpider>();
        /// <summary>
        /// 初始化cell log spider 如果redis数据库中没有对应的信息，则按照今日进行spider初始化；
        /// </summary>
        static CellLogManager()
        {
            var pclist = IpTransform.Name2IP(new Pcinfo[] { Pcinfo.MAIN }).ToList();
            foreach (var item in pclist)
            {
                var spider = FindSpiderFromRedisDB(item.PcIp);
                if (spider != null)
                {
                    spiders.Add(spider);
                }
                else
                {
                    spider = new CellLogSpider(item);
                    spiders.Add(spider); 
                    UpdateCellLogSpider(spider);
                }
            }
        }
        static CellLogSpider FindSpiderFromRedisDB(string pcip)
        {
            try
            {
                string key = CellLogSpider.RedisInfoKey(pcip);
                var result = RedisConnector.Redis.StringGet(key);
                if (result.HasValue)
                {
                    CellLogSpider spider = BsonSerializer.Deserialize<CellLogSpider>(result.ToString());
                    return spider;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                Loger.Logger.Error("初始化cell log 时发生错误{0}", pcip);
                return null;
            }
        }
        static void UpdateCellLogSpider(CellLogSpider spider)
        {
            var json = CellLogSpider.Serialize(spider);
            RedisConnector.Redis.StringSet(spider.RedisKey, json);
        }
        /// <summary>
        /// 该方法查找并向mongodb数据库中插入日期早于time的AETresult；
        /// </summary>
        public static void RefreshData(DateTime time)
        {
            Loger.Logger.Information("开始刷新cell log；");
            DateTime searchdate = DateTime.Now;

            //Parallel.ForEach(spiders, (spider) => {
            //    spider.StartSearchAuto(searchdate);
            //    UpdateCellLogSpider(spider);
            //});
            foreach (var item in spiders)
            {
                item.StartSearchAuto(searchdate);
                UpdateCellLogSpider(item);
            }

            Loger.Logger.Information("cell log 数据刷取结束；");

            Loger.Logger.Information("AddNewResultFileTask");
            var eq = IpTransform.Name2IP(new Pcinfo[] { Pcinfo.MAIN }).ToList();
            Parallel.ForEach(eq, (eq) =>
            {
                AddNewResultFileTask(eq.EqName, time);
            });
        }
        public static void AddNewResultFileTask(string eqname,DateTime time)
        {
            string rediskey = "spider:panel:waitqueue:" + eqname;
            var result = RedisConnector.Redis.SetMembersAsync(rediskey,CommandFlags.PreferReplica).Result;
            if (result.Length != 0)
            {
                Dictionary<RedisValue, PanelInspectHistory> paneldict = new Dictionary<RedisValue, PanelInspectHistory>();
                foreach (var item in result)
                { 
                    paneldict.Add(item,PanelInspectHistory.Deserialize(item.ToString()));
                }
                var panelidArray = from panel in paneldict.Values
                                   select panel.PanelId;
                var pathdict = FilePathManager.GetPanelPathList(panelidArray.ToArray());

                foreach (var item in paneldict)
                {
                    if (item.Value.InspDate < time)
                    {
                        var path = pathdict[item.Value.PanelId];
                        if (path != null && path.Count() != 0)
                        {
                            //InitialNewResultFile(item.Value, path);
                            AETResultTask newtask = new AETResultTask()
                            {
                                RedisKey = rediskey,
                                InspectHistory = item.Value,
                                Paths = path,
                                Value = item.Key,
                            };
                            AETResultTaskManager.AddTask(newtask);
                        }
                    }
                    else
                    {
                        Loger.Logger.Error("存在panel在添加AETresult时未返回路径信息（无路径也应返回null）；{0}", item);
                    }
                    // redis数据库set中删除该项目；
                    // 切换至插入时
                    var returnValue = RedisConnector.Redis.SetRemoveAsync(rediskey, item.Key).Result;
                }
            }
        }
    }
}
