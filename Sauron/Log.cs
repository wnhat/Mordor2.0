using CoreClass;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sauron
{
    public static class Log
    {
        public static ILogger Logger;
        public static ILogger MesLog;
        public static ILogger Testlogger;
        static Log()
        {
            Logger = new LoggerConfiguration()
                .WriteTo.MongoDB(DBconnector.DICSDB, collectionName: "SauronLog")
                .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
                .CreateLogger();

            MesLog = new LoggerConfiguration()
                .WriteTo.MongoDB(DBconnector.DICSDB, collectionName: "MesLog")
                .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
                .CreateLogger();

            Testlogger = new LoggerConfiguration()
                .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Verbose)
                .CreateLogger();
        }
    }
}
