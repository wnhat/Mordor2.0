using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EyeOfSauron.ViewModel;
using MongoDB.Driver;
using CoreClass;
using CoreClass.Model;
using System.IO;
using NetMQ;
using NetMQ.Sockets;
using CoreClass.Service;

namespace EyeOfSauron
{
    public class MissionManager
    {

    }
    public class Mission
    {
        //TODO: 区分任务类型
        private Queue<PanelMission> PreDownloadedPanelMissionQueue = new();
        public PanelMission onInspPanelMission;
        private ProductInfo productInfo;
        readonly object Predownloadlock = new();
       
        public Mission(ProductInfo Info)
        {
            productInfo = Info;
            PreLoadOneMission();
            onInspPanelMission = PreDownloadedPanelMissionQueue.Dequeue();
        }
        private void FillPreDownloadMissionQueue()
        {
            lock (Predownloadlock)
            {
                Task.Run(() =>
                {
                    while (PreDownloadedPanelMissionQueue.Count <= Parameter.PreLoadQuantity)
                    {
                        if (PreLoadOneMission())
                        {
                        }
                        else
                        {
                            break;
                        }
                    }
                });
            }
        }
        public bool PreLoadOneMission()
        {
            PanelMission mission = new(InspectMission.GetMission(productInfo));
            //TODO: pre judge;
            if (mission != null)
            {
                PreDownloadedPanelMissionQueue.Enqueue(mission);
                return true;
            }
            else
            {
                return false;
            }
        }
        public static void PreJudge(ref InspectMission mission)
        {
            //Need User data to initialize OperatorJudgeMessage, this method shoud be implemented other place;
        }
    }
    public class PanelMission
    {
        public Dictionary<string, BitmapImage> resultImageDataDic = new();
        public Dictionary<string, BitmapImage> defectImageDataDic = new();
        public InspectMission inspectMission;
        public AETresult aetResult;
        public PanelMission(InspectMission mission)
        {
            inspectMission = mission;
            aetResult = AETresult.Get(inspectMission.HistoryID);
            IniResultImageDic(aetResult.ResultImages);
            IniDefectImageDic(aetResult.DefectImages);
        }
        public PanelMission(InspectMission mission, AETresult result)
        {
            inspectMission = mission;
            aetResult = result;
            IniResultImageDic(aetResult.ResultImages);
            IniDefectImageDic(aetResult.DefectImages);
        }
        public void IniResultImageDic(ImageContainer[] ResultImages)
        {
            Dictionary<string, BitmapImage> imageDataDic = new Dictionary<string, BitmapImage>();
            IniImageDic(ref imageDataDic, ResultImages);
            BitmapImage? bufferImage;
            foreach (string aviImageName in Parameter.AviImageNameList)
            {
                imageDataDic.TryGetValue(aviImageName, out bufferImage);
#pragma warning disable CS8604 // Possible null reference argument.
                resultImageDataDic.Add(aviImageName, bufferImage);
#pragma warning restore CS8604 // Possible null reference argument.
            }
            foreach (string sviImageName in Parameter.SviImageNameList)
            {
                imageDataDic.TryGetValue(sviImageName, out bufferImage);
#pragma warning disable CS8604 // Possible null reference argument.
                resultImageDataDic.Add(sviImageName, bufferImage);
#pragma warning restore CS8604 // Possible null reference argument.
            }
        }
        public void IniDefectImageDic(ImageContainer[] DefectImages)
        {
            IniImageDic(ref defectImageDataDic, DefectImages);
        }
        public static void IniImageDic(ref Dictionary<string, BitmapImage> imageDataDic, ImageContainer[] imageContainer)
        {
            List<BitmapImage> bufferImage = new();
            for (int i = 0; i < imageContainer.Length; i++)
            {
                byte[] buffer = imageContainer[i].Data;
                bufferImage.Add(new BitmapImage());
                if (buffer.Length == 0)
                {
                    continue;
                }
                bufferImage[i].BeginInit();
                bufferImage[i].StreamSource = new MemoryStream(buffer);
                bufferImage[i].EndInit();
                imageDataDic[imageContainer[i].Name] = bufferImage[i];
                if (i > 100) break;
            }
        }
    }
}
