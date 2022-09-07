using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace CutInspect.Model
{
    public class AppLogClass
    {
        public static ILogger Logger;
        static AppLogClass()
        {
            Logger = new LoggerConfiguration()
                .WriteTo.File(@"D:\CutInsp\LOG\RunTime\log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
    public class ServerLogClass
    {
        public static ILogger Logger;
        static ServerLogClass()
        {
            Logger = new LoggerConfiguration()
                .WriteTo.File(@"D:\CutInsp\LOG\Server\log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}
