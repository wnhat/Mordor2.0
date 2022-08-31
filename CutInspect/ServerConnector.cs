using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CutInspect.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace CutInspect
{
    public static class ServerConnector
    {
        static readonly RestClient restClient = new("http://10.141.34.78:26208/EAC/");

        static ServerConnector()
        {
            restClient.Options.MaxTimeout = 10000;
        }
        public static InspectItem[] GetInfo(DateTime starttime,DateTime endtime)
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
                    string jsonstring = response.Content;
                    InspectItem[] deserializedProduct = JsonConvert.DeserializeObject<InspectItem[]>(jsonstring);
                    return deserializedProduct;
                }
            }
            else
            {
                throw new Exception("连接失败；");
            }
        }
        public static List<GroupData> GetGroupedData(InspectItem[] data)
        {
            List<GroupData> groupedData = new();
            var eqplist = from item in data
                          group item.EquipmentId by item.EquipmentId into g
                          select new { equipmentId = g.Key, Count = g.Count() };

            foreach (var eq in eqplist)
            {
                var items = from item in data where item.EquipmentId == eq.equipmentId select item;

                var newgroup = new GroupData(eq.equipmentId, items.ToList());
                groupedData.Add(newgroup);
            }
            return groupedData;
        }
        public static void SendResult(string id,int status)
        {
            var request = new RestRequest("modifyImageStatus",Method.Post);
            var jsonresult = new JObject
            {
                ["id"] = id,
                ["status"] = status
            };

            request.AddJsonBody(jsonresult.ToString());
            var response = restClient.Post(request);
            if (response.IsSuccessful)
            {
            }
            else
            {
                throw new Exception("上传检查结果失败，请检查服务器连接；");
            }
        }
        public static MemoryStream GetImage(string id)
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
