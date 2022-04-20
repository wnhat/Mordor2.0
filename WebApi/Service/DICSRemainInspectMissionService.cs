using CoreClass;
using CoreClass.Model;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApi.Service
{
    public interface IDICSRemainInspectMissionService
    {
        Task<List<BsonDocument>> GetRemainMissionCount();
    }
    public class DICSRemainInspectMissionService:IDICSRemainInspectMissionService
    {
        private static readonly IMongoCollection<InspectMission> Collection = DBconnector.DICSDB.GetCollection<InspectMission>("InspectMission");

        // get all unfinished mission and group by productinfo;
        public async Task<List<BsonDocument>> GetRemainMissionCount()
        {
            // find unfinished and unrequested mission then aggregate by productinfo;
            var projection = Builders<InspectMission>.Projection.Exclude("Info.Img");
            ProjectionDefinition<InspectMission> group = "{_id : '$Info', count : {$sum : 1}}";
            var agg = Collection.Aggregate()
                .Match(x => x.Finished == false && x.Requested == false)
                .Project<InspectMission>(projection)
                .Group(group);
            var result = await agg.ToListAsync();
            return result;
        }
    }
}
