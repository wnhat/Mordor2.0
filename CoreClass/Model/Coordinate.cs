using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;

namespace CoreClass.Model
{
    [BsonIgnoreExtraElements]
    public class Coordinate
    {
        public int X;
        public int Y;

        public Coordinate(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
        public Coordinate(decimal x, decimal y)
        {
            this.X = (int)x;
            this.Y = (int)y;
        }
    }
}
