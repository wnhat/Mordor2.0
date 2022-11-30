using CoreClass;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskTest
{
    public static class Loger
    {
        public static ILogger Logger;
        public static ILogger Testlogger;
        static Loger()
        {
            Logger = new LoggerConfiguration()
                .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
                .CreateLogger();
        }
    }
}
