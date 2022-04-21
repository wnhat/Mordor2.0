using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Sauron
{
    public static class OldDBconnector
    {
        static SqlConnection TheDataBase;
        static object reqlock = new object();
        static OldDBconnector()
        {
            TheDataBase = new SqlConnection("server=172.16.150.200;UID=sa;PWD=1qaz@WSX;Database=EDIAS_DB;Trusted_connection=False");
        }
        static string DateRange
        {
            get
            {
                var now = DateTime.Now;
                var lastmonth = now - TimeSpan.FromDays(30);
                return lastmonth.ToString("yyyyMMddHHmmss");
            }
        }
        static string DateNow
        {
            get
            {
                return DateTime.Now.ToString("yyyyMMddHHmmss");
            }
        }
        static string IdList2String(string[] IdList)
        {
            // 用于 VcrID in ({0}) 的字符串 format， 生成结果例如: VcrID in ("765H180139C4AAS03","765H180139C4AAS03") 
            bool Fisrt = true;
            string returnstring = "";
            foreach (var item in IdList)
            {
                if (Fisrt)
                {
                    returnstring = returnstring + "'" + item + "'";
                    Fisrt = false;
                }
                else
                {
                    returnstring = returnstring + ",'" + item + "'";
                }
            }
            return returnstring;
        }
        static public List<string> AddInspectResult2DB(MesLot lot, string ModelID, string ProductType)
        {
            string originstring = @"INSERT INTO [dbo].[TAX_PRODUCT_TEST]
           ([EqpID]
           ,[InspDate]
           ,[ModelID]
           ,[InnerID]
           ,[VcrID]
           ,[MviUser]
           ,[LastResult]
           ,[LastJudge]
           ,[MviUserID]
           ,[LastResultName]
           ,[ProductType]
           ,[OperationID])
     VALUES
           ('7CTCT33'
           ,'{0}'
           ,'{1}'
           ,'0000000'
           ,'{2}'
           ,N'{3}'
           ,'{4}'
           ,'{5}'
           ,'{6}'
           ,N'{7}'
           ,'{8}'
           ,'C52000E')";

            List<string> result = new List<string>();
            foreach (var item in lot.missions)
            {
                string commandstring = String.Format(originstring,
                DateNow,                        // 0
                ModelID,                        // 1
                item.PanelId,                   // 2
                item.Judge.FinalUsername,       // 3
                item.Judge.FinalDefect == null ? null : item.Judge.FinalDefect.DefectCode,     // 4
                item.FinalJudge,                // 5
                item.Judge.FinalUserId,         // 6
                item.Judge.FinalDefect == null ? null : item.Judge.FinalDefect.DefectName,     // 7
                ProductType                     // 8
                );
                SqlCommand newcommand = new SqlCommand(commandstring, TheDataBase);
                newcommand.CommandTimeout = 60;
                try
                {
                    lock (reqlock)
                    {
                        TheDataBase.Open();
                        int row = newcommand.ExecuteNonQuery();
                        if (row == 0)
                        {
                            string addresult = String.Format("panel:{0} ,添加结果影响{1}行", item.PanelId, row);
                            result.Add(addresult);
                        }
                        else
                        {
                            // 正常情况不进行记录；
                            // string addresult = String.Format("panel:{0} ,添加结果影响{1}行",item.PanelId, row);
                            // result.Add(addresult);
                        }
                        TheDataBase.Close();
                    }
                }
                catch (System.Exception e)
                {
                    string addresult = String.Format("panel:{0} ,添加结果发生异常{1}", item.PanelId, e.Message);
                    result.Add(addresult);
                    if (TheDataBase.State == ConnectionState.Open)
                    {
                        TheDataBase.Close();
                    }
                }
            }

            return result;
        }
    }
}
