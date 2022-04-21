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
        Task<List<YieldData>> GetYield();
    }

    public class YieldService : IYieldService
    {
        private static readonly IMongoCollection<YieldData> _yieldData = DBconnector.DICSDB.GetCollection<YieldData>("");

        public Task<List<YieldData>> GetYield()
        {
            throw new NotImplementedException();
        }
    }
}
