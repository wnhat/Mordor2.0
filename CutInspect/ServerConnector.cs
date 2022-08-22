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

        static RestClient restClient = new RestClient("http://10.141.34.78:28108/EAC/")
        { 
            
        };

        static ServerConnector()
        {
            restClient.Timeout = TimeSpan.FromSeconds(10);
        }
        public static JObject GetInfo(DateTime starttime,DateTime endtime)
        {            
            var request = new RestRequest("getImageInfo", Method.Get);

            request.AddQueryParameter("startTime", starttime.ToString("yyyy-MM-dd HH:mm:ss"));
            request.AddQueryParameter("endTime", endtime.ToString("yyyy-MM-dd HH:mm:ss"));

            var response = restClient.Get(request);
            Console.WriteLine(response.Content);

            JObject result = JObject.Parse(response.Content);
            return result;
        }
        public static void GetGroupedData(JObject data)
        {

        }
        public static void SendResult(string id,int status)
        {

        }
        public static Stream GetImage(string id)
        {
            var client = new HttpClient();

            Uri uri = GetImageUri(id);
            HttpResponseMessage response = client.GetAsync(uri).Result;

            response.EnsureSuccessStatusCode();
            var responseBody = response.Content.ReadAsStream();
            return responseBody;
        }

        static Uri GetImageUri(string id)
        {
            var builder = new UriBuilder(null,ip,port,part);
            return builder.Uri;
        }
    }
}
