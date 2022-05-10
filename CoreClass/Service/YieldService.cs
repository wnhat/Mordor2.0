using CoreClass;
using CoreClass.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreClass.Service
{
    public interface IYieldService
    {
        // 获取本班次生产产品的良率
        Task<List<YieldData>> GetThisShiftYield();
    }

    public class YieldService : IYieldService
    {
        private static readonly IMongoCollection<YieldData> _yieldData = DBconnector.DICSDB.GetCollection<YieldData>("DailyYield");

        public Task<List<YieldData>> GetThisShiftYield()
        {
            return Task.Run(() =>
            {
                var year  = DateTime.Now.Year;
                var month = DateTime.Now.Month;
                var day   = DateTime.Now.Day;
                var hour = DateTime.Now.Hour;
                if (hour < 6 || hour >= 18)
                {
                    // 夜班
                    List<YieldData> result = _yieldData.Find(x =>
                                                             x.Time >= new DateTime(year, month, day-1, 18, 0, 0) &&
                                                             x.Time <  new DateTime(year, month, day,    6, 0, 0)).ToList();
                    return result;
                } else
                {
                    // 白班
                    List<YieldData> result = _yieldData.Find(x =>
                                                             x.Time >= new DateTime(year, month, day,  6, 0, 0) &&
                                                             x.Time <  new DateTime(year, month, day, 18, 0, 0)).ToList();
                    return result;
                }
            });
        }
    }
}
