using CoreClass.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreClass.Service
{
    public interface IHistoryService
    {
        Task<List<BsonDocument>> GetHistory(DateTime start, DateTime end, int[] eqplist);
        Task<List<BsonDocument>> GetHistoryByID(string[] panelIdList);
    }
    public class HistoryService : IHistoryService
    {
        private static readonly IMongoCollection<BsonDocument> Collection = DBconnector.DICSDB.GetCollection<BsonDocument>("InspectResult");
        private static readonly IMongoCollection<BsonDocument> DICSCollection = DBconnector.DICSDB.GetCollection<BsonDocument>("DICSInspectResult");

        public async Task<List<BsonDocument>> GetHistory(DateTime start, DateTime end, int[] eqplist)
        {
            var filter = new BsonDocument{
                { "InspDate", new BsonDocument
                {
                    { "$gte", start },
                    { "$lte", end }
                } },
                { "EqpID", new BsonDocument("$in", new BsonArray(eqplist)) }
            };
            var projection = Builders<BsonDocument>.Projection.Exclude("_id");
            var result = await Collection.Find(filter).Project<BsonDocument>(projection).ToListAsync();
            return result;
        }
        public async Task<List<BsonDocument>> GetHistoryByID(string[] panelIdList)
        {
            var filter = Builders<BsonDocument>.Filter.In("PanelId", panelIdList);
            var projection = Builders<BsonDocument>.Projection.Exclude("_id");
            var result = await Collection.Find(filter).Project<BsonDocument>(projection).ToListAsync();
            return result;
        }
        public async Task InsertDICSHistory()
        {
            BsonDocument history = new BsonDocument();
        }
    }
}
