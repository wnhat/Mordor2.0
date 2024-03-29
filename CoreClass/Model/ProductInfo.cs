﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using CoreClass.DICSEnum;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CoreClass.Model
{
    [BsonIgnoreExtraElements]
    public class ProductInfo
    {
        [BsonId]
        public ObjectId Id;
        public string PrefixId; //761L;
        [JsonProperty("name")]
        public string Name;     //D2 Porto;
        public string[] InspectImageNames;
        [BsonRepresentation(BsonType.String)]
        [JsonProperty("producttype")]
        //[JsonConverter(typeof(StringEnumConverter))]
        public ProductType[] OnInspectTypes;
        [JsonProperty("fgcode")]
        public string FGcode;
        [JsonProperty("modelid")]
        public string ModelId;
        public DateTime LastAddTime;
        [JsonProperty("img")]
        public ImageContainer Img { get; set; }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            return Id.Equals(((ProductInfo)obj).Id);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}
