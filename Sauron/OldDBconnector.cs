using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Threading;

namespace Sauron
{
    public static class OldDBconnector
    {
        static SqlConnection TheDataBase;
        static object reqlock = new object();
        static Queue<MesLot> logs = new Queue<MesLot>();
        
        static OldDBconnector()
        {
            TheDataBase = new SqlConnection("server=172.16.150.200;UID=sa;PWD=1qaz@WSX;Database=EDIAS_DB;Trusted_connection=False");
        }
        static string DateNow
        {
            get
            {
                return DateTime.Now.ToString("yyyyMMddHHmmss");
            }
        }
        static public void AddLog(MesLot lot)
        {
            logs.Enqueue(lot);
        }
        static public void MainCycle()
        {
            while (true)
            {
                while (logs.Count != 0)
                {
                    MesLot addlot = logs.Dequeue();
                    try
                    {
                        TheDataBase.Open();
                        AddInspectResult2DB(addlot);
                        TheDataBase.Close();
                    }
                    catch (Exception e)
                    {
                        if (TheDataBase.State == ConnectionState.Open)
                        {
                            TheDataBase.Close();
                        }
                        addlot.AddEvent(e.Message);
                    }
                }
                Thread.Sleep(10000);
            }
        }
        static public void AddInspectResult2DB(MesLot lot)
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
           ('7CTCT34'
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

            foreach (var item in lot.missions)
            {
                string commandstring = String.Format(originstring,
                DateNow,                        // 0
                lot.ModelId,                    // 1
                item.PanelId,                   // 2
                item.Judge.FinalUsername,       // 3
                item.Judge.FinalDefect == null ? null : item.Judge.FinalDefect.DefectCode,     // 4
                item.FinalJudge,                // 5
                item.Judge.FinalUserId,         // 6
                item.Judge.FinalDefect == null ? null : item.Judge.FinalDefect.DefectName,     // 7
                lot.ProductType.ToString()      // 8
                );
                SqlCommand newcommand = new SqlCommand(commandstring, TheDataBase);
                newcommand.CommandTimeout = 60;
                try
                {
                    int row = newcommand.ExecuteNonQuery();
                    if (row == 0)
                    {
                        string addresult = String.Format("panel:{0} ,添加结果影响{1}行", item.PanelId, row);
                        lot.AddEvent(addresult);
                    }
                    else
                    {
                        // 正常情况不进行记录；
                        // string addresult = String.Format("panel:{0} ,添加结果影响{1}行",item.PanelId, row);
                        // result.Add(addresult);
                    }
                }
                catch (System.Exception e)
                {
                    string addresult = String.Format("panel:{0} ,添加结果发生异常{1}", item.PanelId, e.Message);
                    Log.Testlogger.Information(addresult);
                    lot.AddEvent(addresult);
                }
            }
        }
    }
}
