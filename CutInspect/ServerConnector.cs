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
            try
            {
                DateTime nowTime = DateTime.Now;
                var response = restClient.Get(request);
                TimeSpan execTime = DateTime.Now - nowTime;
                if (response.IsSuccessful)
                {
                    if (response.Content == null)
                    {
                        throw new Exception(String.Format("获取的产品信息为空；执行时间：{0}", execTime.ToString()));
                    }
                    else
                    {
                        string jsonstring = response.Content;
                        InspectItem[] deserializedProduct = JsonConvert.DeserializeObject<InspectItem[]>(jsonstring);
                        ServerLogClass.Logger.Information("：获取任务成功（{0}--{1}）；执行时间：{2}",starttime.ToString(),endtime.ToString(), execTime.ToString());
                        return deserializedProduct;
                    }
                }
                else
                {
                    string errString = String.Format("获取任务失败（{0}--{1}），response不成功；执行时间：{2}", starttime.ToString(), endtime.ToString(), execTime.ToString());
                    ServerLogClass.Logger.Error("：{0}",errString);
                    throw new Exception(errString);
                }
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                string errString = string.Format("服务器连接失败，无法获取任务信息，异常信息：{0}", ex.Message);
                ServerLogClass.Logger.Error("：{0}",errString);
                throw;
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
            try
            {
                var response = restClient.Post(request);
                if (response.IsSuccessful)
                {
                }
                else
                {
                    throw new Exception("上传检查结果失败，请检查服务器连接；");
                }
            }
            catch (System.Net.Http.HttpRequestException ex)
            {
                ServerLogClass.Logger.Error("：服务器连接失败，无法发送结果信息，异常信息：{0}", ex.Message);
                throw;
            }
        }
        public static MemoryStream GetImage(string id)
        {
            try
            {
                DateTime nowTime = DateTime.Now;
                var request = new RestRequest("getImage");
                request.AddQueryParameter("imageId", id);
                var response = restClient.Get(request);
                TimeSpan execTime = DateTime.Now - nowTime;
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
            catch (System.Net.Http.HttpRequestException ex)
            {
                ServerLogClass.Logger.Error("：服务器连接失败，无法获取图片信息，异常信息：{0}", ex.Message);
                throw;
            }
        }
    }
}
