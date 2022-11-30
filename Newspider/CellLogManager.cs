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
            Parallel.ForEach(spiders, (spider) => {
                spider.StartSearchAuto(searchdate);
                UpdateCellLogSpider(spider);
            });
            Loger.Logger.Information("cell log 数据刷取结束；");
            Loger.Logger.Information("开始添加新的AET检查结果文件；");
            var eq = IpTransform.Name2IP(new Pcinfo[] { Pcinfo.MAIN }).ToList();
            Parallel.ForEach(eq, (eq) => {
                AddNewResultFile(eq.EqName,time);
            });
        }
        public static void InitialNewResultFile(PanelInspectHistory hist,List<PanelPathContainer> path)
        {
            try
            {
                // 正常文件可在500ms传输完成；
                AETresult newAETresult = new AETresult(hist, path.ToArray());
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
                        Loger.Testlogger.Information("新的检查结果文件mongodb size限制，{0},{1}", e.Message, hist.PanelId);
                    }
                }
            }
            catch (Exception e)
            {
                Loger.Testlogger.Error(e, "在创建新的检查结果文件时发生了错误，请检查详情确认发生错误的原因;{0}", hist);
            }
        }
        public static void AddNewResultFile(string eqname,DateTime time)
        {
            string rediskey = "spider:panel:waitqueue:" + eqname;
            var result = RedisConnector.Redis.SetMembers(rediskey,CommandFlags.PreferReplica);
            if (result.Length != 0)
            {
                Dictionary<RedisValue, PanelInspectHistory> paneldict = new Dictionary<RedisValue, PanelInspectHistory>();
                foreach (var item in result)
                {
                    paneldict.Add(item,PanelInspectHistory.Deserialize(item.ToString()));
                }
                var panelidArray = from panel in paneldict.Values
                                   select panel.PanelId;
                var pathdict = RedisConnector.GetPanelPathList(panelidArray.ToArray());

                foreach (var item in paneldict)
                {
                    if (item.Value.InspDate < time)
                    {
                        var path = pathdict[item.Value.PanelId];
                        if (path != null && path.Count() != 0)
                        {
                            InitialNewResultFile(item.Value, path);
                        }
                    }
                    else
                    {
                        Loger.Logger.Error("存在panel在添加AETresult时未返回路径信息（无路径也应返回null）；{0}", item);
                    }
                    // redis数据库set中删除该项目；
                    RedisConnector.Redis.SetRemoveAsync(rediskey, item.Key);
                }
            }
        }
    }
}
