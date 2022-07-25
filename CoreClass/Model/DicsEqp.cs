using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CoreClass.Model
{
    public class DicsEqp
    {
        private static readonly IMongoCollection<DicsEqp> Collection = DBconnector.DICSDB.GetCollection<DicsEqp>("EqpIp");
        [BsonId]
        public ObjectId id;
        public string Addr { get; private set; }
        public string EqpName { get; private set; }

        public DicsEqp(string addr,string name)
        {
            Addr = addr;
            EqpName = name;
        }
        public static DicsEqp GetByName(string name)
        {
            var result =  Collection.Find(x => x.EqpName == name).First();
            return result;
        }
        public static DicsEqp GetByIp(string addr)
        {
            var result = Collection.Find(x => x.Addr == addr).FirstOrDefault();
            return result;
        }
        public static void AddMany(List<DicsEqp> dicsEqps)
        {
            Collection.InsertMany(dicsEqps);
        } 
    }
}
