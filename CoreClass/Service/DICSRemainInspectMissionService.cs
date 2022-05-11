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
    public interface IDICSRemainInspectMissionService
    {
        Task<List<BsonDocument>> GetRemainMissionCount();
        Task<List<BsonDocument>> GetRemainDetail(ObjectId id);
        Task<BsonDocument> GetMeslot(ObjectId id);
        Task<BsonDocument> GetInspectMission(ObjectId productid);
    }
    public class DICSRemainInspectMissionService : IDICSRemainInspectMissionService
    {
        private static readonly IMongoCollection<InspectMission> Collection = DBconnector.DICSDB.GetCollection<InspectMission>("InspectMission");
        private static readonly IMongoCollection<BsonDocument> LotCollection = DBconnector.DICSDB.GetCollection<BsonDocument>("MesLot");
        public async Task<List<BsonDocument>> GetRemainDetail(ObjectId id)
        {
            var filter = new BsonDocument[] {
                new BsonDocument{
                    {
                        "$match",
                        new BsonDocument{
                            {
                                "ProductInfo._id", id
                            },
                        }
                    } },
                new BsonDocument{
                    {
                        "$addFields",
                        new BsonDocument
                        {
                            {
                                "FinishedCount", new BsonDocument{
                                    {
                                        "$size", new BsonDocument{
                                            {
                                                "$filter", new BsonDocument{
                                                    {
                                                        "input", "$missions"
                                                    },
                                                    {
                                                        "as", "panel"
                                                    },
                                                    {
                                                        "cond", new BsonDocument{
                                                            {
                                                                "$eq", new BsonArray{
                                                                    "$$panel.Judge.finished", true
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            },
                            {   "Count", new BsonDocument{
                                    {
                                        "$size", "$Panels"
                                    }
                                }
                            },
                        }
                    },
                },
                new BsonDocument{
                    {
                        "$project", 
                        new BsonDocument
                        {
                            {"missions", 0}
                        } 
                    }
                }
            };
            var result = await LotCollection.Aggregate<BsonDocument>(filter).ToListAsync();
            return result;
        }

        // get all unfinished mission and group by productinfo;
        public async Task<List<BsonDocument>> GetRemainMissionCount()
        {
            // find unfinished and unrequested mission then aggregate by productinfo;
            var projection = Builders<InspectMission>.Projection.Exclude("Info.Img");
            ProjectionDefinition<InspectMission> group = "{_id : '$Info', count : {$sum : 1}}";
            var agg = Collection.Aggregate()
                .Match(x => x.Finished == false && x.Requested == false)
                .Project<InspectMission>(projection)
                .Group(group)
                .Sort("{ count: -1 }");
            var result = await agg.ToListAsync();
            return result;
        }

        // get specific product's mission count;
        public async Task<BsonDocument> GetRemainMissionCount(ObjectId id)
        {
            var projection = Builders<InspectMission>.Projection.Exclude("Info.Img");
            ProjectionDefinition<InspectMission> group = "{_id : '$Info', count : {$sum : 1}}";
            var result = Collection.Aggregate()
                .Match(x => x.Info.Id == id && x.Finished == false && x.Requested == false)
                .Project<InspectMission>(projection)
                .Group(group);
            return await result.FirstOrDefaultAsync();
        }

        public async Task<BsonDocument> GetMeslot(ObjectId id)
        {
            var filter = new BsonDocument{
                    {
                        "$match",
                        new BsonDocument{
                            {
                                "ProductInfo._id", id
                            },
                        }
                    } };
            var result = await LotCollection.FindAsync(filter);
            return result.FirstOrDefault();
        }

        public async Task<BsonDocument> GetInspectMission(ObjectId productid)
        {
            var filter = new BsonDocument{
                    {
                        "$match",
                        new BsonDocument{
                            {
                                "ProductInfo._id", productid
                            },
                        }
                    } };
            var result = await LotCollection.FindAsync(filter);
            return result.FirstOrDefault();
        }
    }
}
