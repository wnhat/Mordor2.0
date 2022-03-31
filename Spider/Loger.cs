using CoreClass;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spider
{
    public static class Loger
    {
        public static ILogger Logger;
        public static ILogger Testlogger;
        static Loger()
        {
            Logger = new LoggerConfiguration()
                .WriteTo.MongoDB(DBconnector.DICSDB, collectionName:"SpiderLog")
                .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
                .CreateLogger();
            Testlogger = new LoggerConfiguration()
                .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose)
                .CreateLogger();
        }
    }
}
