using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TIBCO.Rendezvous;

namespace TIBListener
{
    class Program
    {
        static string service = "21200";
        static string network = ";225.21.21.2";
        static string daemon = "10.141.70.61:7500";
        static string subject = "BOE.B7.MEM.PRD.PEMsvr";
        static Transport transport = null;
        static Listener listeners = null;
        static List<ListenParameter> listenParameters = new List<ListenParameter>();
        static void Main(string[] args)
        {
            //AddInitialListenParam();
            InitialListener();
        }
        static void AddInitialListenParam()
        {
            ListenParameter newparam = new ListenParameter() { 
                Name="MDL Test",
                RedisBufferTarget= "tib:waitqueue:mdl",
                Detail = "监控MDL Test产生的良率信息，包含多个站点及复判站点；",
                 EditTime = DateTime.Now,
                  Parameters = new Dictionary<string, HashSet<string>>(),
                    MongoTarget = "",
                     RedisTarget = ""
            };
            newparam.Parameters.Add("PROCESSOPERATIONNAME", new HashSet<string> {"M27100N","M27110N","M33000N","M33010N","M42100N","M42110N" });
            string json = JsonConvert.SerializeObject(newparam);
            RedisConnector.Redis.SetAdd("tib:listen:param", json);
        }
        static void InitialListener()
        {
            // 初始化参数
            InitialParameter();
            try
            {
                TIBCO.Rendezvous.Environment.Open();
            }
            catch (RendezvousException exception)
            {
                FilePathLogClass.Logger.Error("Failed to open Rendezvous Environment: {0}", exception.Message);
                FilePathLogClass.Logger.Error(exception.StackTrace);
                System.Environment.Exit(1);
            }

            // Create Network transport
            try
            {
                transport = new NetTransport(service, network, daemon);
                FilePathLogClass.Logger.Information("initial transport success;");
            }
            catch (RendezvousException exception)
            {
                FilePathLogClass.Logger.Error("Failed to create NetTransport");
                FilePathLogClass.Logger.Error(exception.StackTrace);
                System.Environment.Exit(1);
            }

            try
            {
                listeners = new Listener(Queue.Default, transport, subject, null);
                listeners.MessageReceived += new MessageReceivedEventHandler(OnMessageReceived);
                FilePathLogClass.Logger.Information("Listening on;");
            }
            catch (RendezvousException exception)
            {
                FilePathLogClass.Logger.Error("Failed to create listener:");
                FilePathLogClass.Logger.Error(exception.StackTrace);
                System.Environment.Exit(1);
            }
            FilePathLogClass.Logger.Information("初始化完成");
            // dispatch Rendezvous events
            GC.KeepAlive(listeners);
            while (true)
            {
                try
                {
                    Queue.Default.Dispatch();
                }
                catch (Exception exception)
                {
                    FilePathLogClass.Logger.Error("Exception dispatching default queue:");
                    FilePathLogClass.Logger.Error(exception.StackTrace);
                }
            }
        }
        static void InitialParameter()
        {
            try
            {
                listenParameters = new List<ListenParameter>();
                var result = RedisConnector.Redis.SetMembers("tib:listen:param");
                foreach (var item in result)
                {
                    ListenParameter param = ListenParameter.Deserialize(item);
                    listenParameters.Add(param);
                }
            }
            catch (Exception e)
            {
                FilePathLogClass.Logger.Error(e, "初始化参数时发生错误；");
                System.Environment.Exit(1);
            }
        }
        static void OnMessageReceived(object listener, MessageReceivedEventArgs messageReceivedEventArgs)
        {

            Message message = messageReceivedEventArgs.Message;
            XmlDocument xml = new XmlDocument();
            MessageField field = message.GetFieldByIndex(0);
            if (field.Name == "xmlData")
            {
                string xmlString = message.GetField("xmlData").Value.ToString();
                try
                {
                    xml.LoadXml(xmlString);
                    foreach (var item in listenParameters)
                    {
                        bool result = CheckXmlParameterSuitable(xml, item);
                        if (result)
                        { 
                            var returnResult = RedisConnector.Redis.SetAdd(item.RedisBufferTarget, xml.InnerXml);
                            if (!returnResult)
                            {
                                FilePathLogClass.Logger.Error("未能正常添加key:{0},xml:{1}",item.RedisBufferTarget, xml.InnerXml);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    FilePathLogClass.Logger.Error(e, "添加新消息时发生错误：错误消息为：{0}", xmlString);
                    throw;
                }
            }
        }
        /// <summary>
        /// 如果xml文件中的项目与parameter中的项目完全匹配则返回true，有任何一项不匹配则返回false；
        /// </summary>
        /// <param name="xml"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        static bool CheckXmlParameterSuitable(XmlDocument xml, ListenParameter param)
        {
            foreach (var item in param.Parameters)
            {
                var node = xml.GetElementsByTagName(item.Key);
                if (node.Count != 0)
                {
                    var nodestring = node.Item(0).InnerText;
                    if (!item.Value.Contains(nodestring))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
    }
    public static class ConsoleLogClass
    {
        public static ILogger Logger;
        static ConsoleLogClass()
        {
            Logger = new LoggerConfiguration().WriteTo.Console()
                .CreateLogger();
        }
    }
    public static class FilePathLogClass
    {
        public static ILogger Logger;
        static FilePathLogClass()
        {
            Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File(@"D:\TIBCOTEST\log-.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
    public static class JsonSerializerSetting
    {
        public static JsonSerializerSettings Setting;
        public static JsonSerializerSettings FrontConvertSetting;
        static JsonSerializerSetting()
        {
            Setting = new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, ReferenceLoopHandling = ReferenceLoopHandling.Ignore, MaxDepth = 6 , MissingMemberHandling = MissingMemberHandling.Ignore};
            FrontConvertSetting = new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, ReferenceLoopHandling = ReferenceLoopHandling.Ignore, MaxDepth = 4 };
            FrontConvertSetting.Converters.Add(new StringEnumConverter());
        }
    }
}
