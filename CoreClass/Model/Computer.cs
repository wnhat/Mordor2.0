using CoreClass.DICSEnum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreClass.Model
{
    public class PC
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public int EqId { get; set; }
        public string PcIp { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Pcinfo PcName { get; set; }
        [BsonRepresentation(BsonType.String)]
        public Side PcSide { get; set; }

        public bool IsPcInType(List<int> eq_id_list, Pcinfo[] pc_name_list, Side[] pc_side_list)
        {
            if (eq_id_list.Contains(EqId) & pc_name_list.Contains(PcName) & pc_side_list.Contains(PcSide))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string CellLogPath 
        { 
            // 由于设备端log的写入现阶段独占handle，因此刷取moniter server(60s刷新) 储存的log副本；
            get 
            {
                return @"\\172.16.150.200\eDiasLogs\TEST\" + EqName + @"\LOG\{0}\Cell.csv";
            }
        }
        public string EqName
        {
            get
            {
                if (EqId > 9)
                {
                    return "7CTCT" + EqId.ToString();
                }
                else
                {
                    return "7CTCT0" + EqId.ToString();
                }
            }
        }
        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            if (Id == ((PC)obj).Id)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
