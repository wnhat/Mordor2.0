using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace CutInspect
{
    public static class ServerConnector
    {
        static string ip = "10.141.34.78";
        static int port = 28108;
        static string part = "EAC";

        static Uri uri = new UriBuilder(Uri.UriSchemeHttp, ip, port, part).Uri;

        // ver 2
        static string getImageInfo { get { return uri.ToString() + "/getImageInfo"; } }
        static string modifyImageStatus { get { return uri.ToString() + "/modifyImageStatus"; } }
        static string getImage { get { return uri.ToString() + "/getImage"; } }

        static RestClient restClient = new RestClient("http://10.141.34.78:28108/EAC/");

        static ServerConnector()
        {
            restClient.Options.MaxTimeout = 10000;
        }
        public static JObject GetInfo(DateTime starttime,DateTime endtime)
        {            
            var request = new RestRequest("getImageInfo");

            request.AddQueryParameter("startTime", starttime.ToString("yyyy-MM-dd HH:mm:ss"));
            request.AddQueryParameter("endTime", endtime.ToString("yyyy-MM-dd HH:mm:ss"));

            var response = restClient.Get(request);
            if (response.IsSuccessful)
            {
                if (response.Content == null)
                {
                    throw new Exception("获取的产品信息为空；");
                }
                else
                {
                JObject result = JObject.Parse(response.Content);
                return result;
                }
            }
            else
            {
                throw new Exception("连接失败；");
            }
            
        }
        public static void GetGroupedData(JObject data)
        {

        }
        public static void SendResult(string id,int status)
        {
            var request = new RestRequest("modifyImageStatus",Method.Post);
            request.AddBody("id", id);
            request.AddBody("status", status.ToString());
            var response = restClient.Post(request);
            if (response.IsSuccessful)
            {
            }
            else
            {
                throw new Exception("上传检查结果失败，请检查服务器连接；");
            }
        }
        public static Stream GetImage(string id)
        {
            var request = new RestRequest("getImage");

            request.AddQueryParameter("imageId", id);

            var response = restClient.Get(request);
            if (response.IsSuccessful)
            {
                if (response.RawBytes == null)
                {
                    throw new Exception("获取的图像为空；");
                }
                else
                {
                return new MemoryStream(response.RawBytes);
                }
            }
            else
            {
                throw new Exception("连接失败；");
            }
        }
    }
}
