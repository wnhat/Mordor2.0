using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Converters;
using System.Text.Json.Serialization;

namespace CoreClass.DICSEnum
{
    public enum ProductType
    {
        // 大小写与MES保持严格一致；
        Production,
        PV,
        TPCN,
        Develop,
        Engineer,
    }
    public enum OperationID
    {
        C52000N,
        C52000E,
        C52000R,
        C520RTN,
        NULL,
    }
}