using CoreClass;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sauron
{
    public interface IMesLotService
    {

    }
    public class MesLotService: IMesLotService
    {
        private static readonly IMongoCollection<MesLot> Collection = DBconnector.DICSDB.GetCollection<MesLot>("MesLot");

        /// <summary>
        /// 向mongodb中添加新的lot，但该lot并没有进行初始化（仅包含从mes获取到的信息）；
        /// </summary>
        /// <param name="lot"></param>
        public static void Insert(MesLot lot)
        {
            // todo: 校验是否有在检lot被重复添加的情况；
            var builder = Builders<MesLot>.Filter;
            var filter = builder.And(builder.Eq("CoverTrayId", lot.CoverTrayId), builder.Eq("Update2MES", false));
            var result = Collection.Find(filter).FirstOrDefault();
            if (result == null)
            {
                Collection.InsertOneAsync(lot);
            }
            else
            {
                var panelidEnum = result.Panels.GetEnumerator();
                // 当 lot 中的panelid全部相同时当作同一lot处理；
                int equalCount = 0;
                for (int i = 0; i < result.Panels.Length; i++)
                {
                    if (lot.Panels[i] == result.Panels[i])
                    {
                        equalCount++;
                    }
                }
                if (equalCount == result.Panels.Length)
                {
                    // do nothing;
                }
                else
                {
                    // TODO: 记录该可能存在异常的情况；
                    // 当lot中panel信息不完全相同时添加lot，仅能避免作业员操作lot勿解hold的情况,无法解决被人工操作的lot MES 信息与DICS信息不匹配的问题；
                    Collection.InsertOneAsync(lot);
                }
            }
        }
    }
}
