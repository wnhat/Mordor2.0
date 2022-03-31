using CoreClass.DICSEnum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;

namespace CoreClass.Model
{
    public class PanelInspectHistory
    {
        public static IMongoCollection<PanelInspectHistory> Result = DBconnector.DICSDB.GetCollection<PanelInspectHistory>("InspectResult");

        [BsonId]
        public ObjectId ID;
        public int EqpID;
        public DateTime InspDate;
        public string ModelId;
        [BsonRepresentation(BsonType.String)]
        public Side Side;
        public int StageID;
        public string PanelId;
        [BsonRepresentation(BsonType.String)]
        public OperationID OperationID;
        [BsonRepresentation(BsonType.String)]
        public ProductType ProductType;

        [BsonRepresentation(BsonType.String)]
        public JudgeGrade MtpJudge;
        [BsonRepresentation(BsonType.String)]
        public JudgeGrade TspJudge;
        [BsonRepresentation(BsonType.String)]
        public JudgeGrade MviJudge;
        [BsonRepresentation(BsonType.String)]
        public JudgeGrade MergeToolJudge;
        [BsonRepresentation(BsonType.String)]
        public JudgeGrade LastJudge;

        public string MtpResult;
        public string MviResult;
        public string LastResult;

        public string Mtpcode;
        public string MergeToolCode;
        public string[] DefectName;

        public string MviUserName;
        public string MviUserID;
        public double Jncd;

        public string EqName
        {
            get
            {
                if (EqpID > 9)
                {
                    return "7CTCT" + EqpID.ToString();
                }
                else
                {
                    return "7CTCT0" + EqpID.ToString();
                }
            }
        }

        public PanelInspectHistory(string celllog,PC pC)
        {
            string[] fieldlist = celllog.Split(",");
            EqpID = pC.EqId;
            InspDate = DateTime.Parse(fieldlist[(int)FileHeaderEnum.DATE]);
            // TODO: model 切换成储存在DB中的类；
            ModelId = fieldlist[(int)FileHeaderEnum.MODEL];
            Side = InnerId2Side(fieldlist[(int)FileHeaderEnum.INNERID]);
            StageID = int.Parse(fieldlist[(int)FileHeaderEnum.STAGEID]);
            PanelId = fieldlist[(int)FileHeaderEnum.VCRID];
            OperationID = string2OperationID(fieldlist[(int)FileHeaderEnum.OPERATIONID]);
            ProductType = (ProductType)Enum.Parse(typeof(ProductType), fieldlist[(int)FileHeaderEnum.PRODUCTTYPE]);

            MtpJudge = string2Judge(fieldlist[(int)FileHeaderEnum.MTPJUDGE]);
            TspJudge = string2Judge(fieldlist[(int)FileHeaderEnum.TSPJUDGE]);
            MviJudge = string2Judge(fieldlist[(int)FileHeaderEnum.MVIJUDGE]);
            MergeToolJudge = string2Judge(fieldlist[(int)FileHeaderEnum.MERGETOOLJUDGE]);
            LastJudge = string2LastJudge(fieldlist[(int)FileHeaderEnum.LASTJUDGE]);

            MtpResult = fieldlist[(int)FileHeaderEnum.MTPRESULT];
            MviResult = fieldlist[(int)FileHeaderEnum.MVIRESULT];
            LastResult = fieldlist[(int)FileHeaderEnum.LASTRESULT];

            Mtpcode = fieldlist[(int)FileHeaderEnum.MTP_CODE];
            MergeToolCode = fieldlist[(int)FileHeaderEnum.MERGETOOLCODE];
            DefectName = fieldlist[(int)FileHeaderEnum.DEFECTCODE].Split(":");

            MviUserID = fieldlist[(int)FileHeaderEnum.MVIUSERID];
            MviUserName = fieldlist[(int)FileHeaderEnum.MVIUSER];

            if (fieldlist[(int)FileHeaderEnum.JNCD] == "")
            {
                Jncd = 0;
            }
            else
            {
                Jncd = double.Parse(fieldlist[(int)FileHeaderEnum.JNCD]);
            }
        }
        static Side InnerId2Side(string input)
        {
            string side = input.Substring(0,1);
            if (side == "1")
            {
                return Side.Left;
            }
            else
            {
                return Side.Right;
            }
        }
        static JudgeGrade string2Judge(string input)
        {
            if (input == "")
            {
                return JudgeGrade.PASS;
            }
            else if (input == "OK_S")
            {
                return JudgeGrade.S;
            }
            else if (input == "NG")
            {
                return JudgeGrade.N;
            }
            else if (input == "BP")
            {
                return JudgeGrade.PASS;
            }
            return (JudgeGrade)Enum.Parse(typeof(JudgeGrade), input);
        }
        static JudgeGrade string2LastJudge(string input)
        {
            string judge = input[input.Length - 2].ToString();
            return (JudgeGrade)Enum.Parse(typeof(JudgeGrade), judge);
        }
        static OperationID string2OperationID(string input)
        {
            if (input == "")
            {
                return OperationID.NULL;
            }
            return (OperationID)Enum.Parse(typeof(OperationID), input);
        }
        public static PanelInspectHistory Get(string panelid)
        {
            var filter = Builders<PanelInspectHistory>.Filter.Eq("PanelId", panelid);
            return Result.Find(filter).FirstOrDefault();
        }
        public static PanelInspectHistory[] Get(string[] panelid)
        {
            var filter = Builders<PanelInspectHistory>.Filter.All("PanelId", panelid);
            return Result.Find(filter).ToList().ToArray();
        }
        public static void InsertPanelHistory(PanelInspectHistory input)
        {
            Result.InsertOneAsync(input);
        }
        public static void InsertPanelHistory(PanelInspectHistory[] input)
        {
            Result.InsertManyAsync(input);
        }
    }
}
