using CoreClass.DICSEnum;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CoreClass
{
    public static class JsonSerializerSetting
    {
        public static JsonSerializerSettings Setting;
        public static JsonSerializerSettings FrontConvertSetting;
        static JsonSerializerSetting()
        {
            Setting = new JsonSerializerSettings() { StringEscapeHandling = StringEscapeHandling.EscapeNonAscii, ReferenceLoopHandling = ReferenceLoopHandling.Ignore, MaxDepth = 6 };
            FrontConvertSetting = new JsonSerializerSettings() { };
            FrontConvertSetting.Converters.Add(new StringEnumConverter());
        }
    }

}