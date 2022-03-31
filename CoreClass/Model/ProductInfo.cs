using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using CoreClass.DICSEnum;

namespace CoreClass.Model
{
    public class ProductInfo
    {
        [BsonId]
        public ObjectId id;
        public string PrefixId; //761L;
        public string Name;     //D2 Porto;
        public string[] InspectImageNames;
        public ProductType[] OnInspectTypes;
        public string FGcode;
        public string ModelId;
        public DateTime LastAddTime;

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return id.Equals(((ProductInfo)obj).id);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
