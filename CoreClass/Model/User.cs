using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreClass.Model
{
    public class User
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string UserName { get; set; }
        public string UserNumber { get; set; }
        public int Grade { get; set; }
    }
}
