using CoreClass;
using CoreClass.DICSEnum;
using CoreClass.Element;
using CoreClass.LogSpider;
using CoreClass.Model;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Spider
{
    public class EqpSpider
    {
        [BsonId]
        ObjectId _id;
        public int EqpId;
        public DateTime LastSearchDate;
        TimeSpan ImageSearchDelay = TimeSpan.FromMinutes(30);
        static IMongoCollection<EqpSpider> Spiders = DBconnector.DICSDB.GetCollection<EqpSpider>("SearchSpider");

        // 组件
        // 添加组件时请注意是否要在数据库中保存组件的状态；
        public CellLogSpider cellLogSpiders;
        public TactTimeSpider tactTimeSpider;

        // 事件
        public event EventHandler<SearchEventArgs> RaiseSearchEvent;

        public EqpSpider(int eqpId)
        {
            EqpId = eqpId;
            InitialComponent();
        }
        public EqpSpider()
        {
            InitialComponent();
        }
        /// <summary>
        /// 进行单台设备的log搜索组件初始化，后续添加组件请在这里进行初始化;
        /// </summary>
        public void InitialComponent()
        {
            if (cellLogSpiders == null)
            {
                var mainpc = IpTransform.Name2IP(new List<int>() { EqpId }, new Pcinfo[] { Pcinfo.MAIN }).FirstOrDefault();
                cellLogSpiders = new CellLogSpider(mainpc);
            }
            if (tactTimeSpider == null)
            {
                var mainpc = IpTransform.Name2IP(new List<int>() { EqpId }, new Pcinfo[] { Pcinfo.MAIN }).FirstOrDefault();
                tactTimeSpider = new TactTimeSpider(mainpc);
            }
        }
        /// <summary>
        /// 绑定爬虫的搜索事件，请将各组件的搜索加入到该方法中；
        /// </summary>
        /// <param name="time"> 搜索时间的发生时间</param>
        public void SearchAuto(DateTime time)
        {
            cellLogSpiders.StartSearchAuto(time);
            //UpdateSelf();
        }
        void UpdateSelf()
        {
            var filter = Builders<EqpSpider>.Filter.Eq("EqpId", this.EqpId);
            Spiders.ReplaceOne(filter,this);
        }
        /// <summary>
        /// 将新生产的panel结果文件储存在数据库中；
        /// </summary>
        /// <param name="timeBefer"> </param>
        public void AddNewResultAuto(DateTime timeBefer)
        {
            while (true)
            {
                PanelInspectHistory panel = cellLogSpiders.GetNewId();
                // 更新DB信息;

                if (panel != null)
                {
                    var filter = Builders<EqpSpider>.Filter.Eq("EqpId", this.EqpId);
                    var update = Builders<EqpSpider>.Update.PopFirst("cellLogSpiders.PanelIdQueue");
                    var Result = Spiders.UpdateOne(filter, update);
                    //之后运行的步骤报错时会导致部分ID丢失；
                    var pathcollection = FileManager.GetPanelPath(panel.PanelId);

                    if (pathcollection != null)
                    {
                        // 获取本设备的文件路径，剔除非本设备的路径；
                        var path = from item in pathcollection
                                   where item.EqId == this.EqpId
                                   select item;
                        if (path.Count() != 0)
                        {
                            try
                            {
                                // 正常文件可在500ms传输完成；
                                AETresult result = new AETresult(panel, path.ToArray());
                                if (result.ResultImages == null && result.DefectImages == null)
                                {
                                    // 说明文件为空，或着没有任何图像文件，没有上传意义；
                                }
                                else
                                {
                                    AETresult.AETresultCollection.InsertOneAsync(result);
                                }
                            }
                            catch (Exception e)
                            {
                                Loger.Testlogger.Error(e,"在创建新的检查结果文件时发生了错误，请检查详情确认发生错误的原因;");
                            }
                        }
                    }
                    else
                    {
                        // TODO: log the unmatched panel to find why there is no result;
                    }
                    // 当搜索间隔距离当前时间过近时跳出此添加循环；
                    if (panel.InspDate > timeBefer)
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }
    }
}