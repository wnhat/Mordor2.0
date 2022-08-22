using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CoreClass
{
    public class CutServerConnector
    {
        static string ip = "10.141.34.78";
        static int port = 28108;
        static string part = "EAC";

        static Uri uri = new UriBuilder(Uri.UriSchemeHttp, ip, port, part).Uri;

        public JObject GetInfo(DateTime starttime, DateTime endtime)
        {
            var client = new HttpClient();

            // make url;
            string start = "startTime=" + starttime.ToString("yyyy-MM-dd HH:mm:ss");
            string end = "&endTime=" + endtime.ToString("yyyy-MM-dd HH:mm:ss");
            string url = uri.ToString() + "/getImageInfo?" + start + end;

            // request;
            HttpResponseMessage response = client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();
            string responseBody = response.Content.ReadAsStringAsync().Result;

            JObject result = JObject.Parse(responseBody);
            return result;
        }
        public void GetGroupedData(JObject data)
        {

        }
        public void SendResult(string id, int status)
        {

        }
        public Stream GetImage(string id)
        {
            var client = new HttpClient();

            Uri uri = GetImageUri(id);
            HttpResponseMessage response = client.GetAsync(uri).Result;

            response.EnsureSuccessStatusCode();
            var responseBody = response.Content.ReadAsStream();
            return responseBody;
        }

        Uri GetImageUri(string id)
        {
            var builder = new UriBuilder(null, ip, port, part);
            return builder.Uri;
        }
    }
}
