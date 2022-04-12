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

namespace EyeOfSauron
{
    public class MissionManager
    {

    }
    public class Mission
    {
        private Queue<PanelMission> PreDownloadedPanelMissionQueue = new Queue<PanelMission>();
        public PanelMission onInspPanelMission;
        private ProductInfo productInfo;
        readonly object Predownloadlock = new object();
        public Mission(ProductInfo Info)
        {
            productInfo = Info;
            //FillPreDownloadMissionQueue();
            if (PreLoadOneMission())
            {

            }
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
            PreDownloadedPanelMissionQueue.Enqueue(new PanelMission(productInfo));
            //TODO: null check
            return true; 
        }
    }
    public class PanelMission
    {
        public Dictionary<string, BitmapImage> resultImageDataDic = new Dictionary<string, BitmapImage>();
        public Dictionary<string, BitmapImage> defectImageDataDic = new Dictionary<string, BitmapImage>();
        public InspectMission inspectMission;
        public AETresult aetResult;
        public PanelMission(ProductInfo info)
        {
            inspectMission = InspectMission.GetMission(info);
            aetResult = AETresult.Get(inspectMission.ResultContainerId);
            iniResultImageDic(aetResult.ResultImages);
            iniDefectImageArDic(aetResult.DefectImages);
        }
        public void iniResultImageDic(ImageContainer[] ResultImages)
        {
            Dictionary<string, BitmapImage> imageDataDic = new Dictionary<string, BitmapImage>();
            iniImageDic(ref imageDataDic, ResultImages);
            BitmapImage? bufferImage;
            foreach (string aviImageName in Parameter.AviImageNameList)
            {
                imageDataDic.TryGetValue(aviImageName, out bufferImage);
                resultImageDataDic.Add(aviImageName, bufferImage);
            }
        }
        public void iniDefectImageArDic(ImageContainer[] DefectImages)
        {
            iniImageDic(ref defectImageDataDic, DefectImages);
        }
        public void iniImageDic(ref Dictionary<string, BitmapImage> imageDataDic, ImageContainer[] imageContainer)
        {
            BitmapImage bufferImage = new BitmapImage();
            for (int i = 0; i < imageContainer.Length; i++)
            {
                byte[] buffer = imageContainer[i].Data;
                bufferImage.BeginInit();
                bufferImage.StreamSource = new MemoryStream(buffer);
                bufferImage.EndInit();
                imageDataDic[imageContainer[i].Name] = bufferImage;
            }
        }
    }
}
