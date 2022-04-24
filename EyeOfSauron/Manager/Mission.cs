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
    public class Mission
    {
        //TODO: 区分任务类型
        private Queue<PanelMission> PreDownloadedPanelMissionQueue = new();
        
        public PanelMission onInspPanelMission;
        
        private readonly ProductInfo productInfo;
        
        readonly object Predownloadlock = new();
       
        public Mission(ProductInfo Info)
        {
            productInfo = Info;
            if (PreLoadOneMission())
            {
                onInspPanelMission = PreDownloadedPanelMissionQueue.Dequeue();
            }
            else
            {
                throw new Exception("Mission Empty");
            }
        }
        
        public void FillPreDownloadMissionQueue()
        {
            lock (Predownloadlock)
            {
                Task.Run(() =>
                {
                    while (PreDownloadedPanelMissionQueue.Count <= 3)
                        //while (PreDownloadedPanelMissionQueue.Count <= Parameter.PreLoadQuantity)
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
            try
            {
                InspectMission inspectMission = InspectMission.GetMission(productInfo);
                if(inspectMission == null)
                {
                    return false;
                }
                try
                {
                    PanelInspectHistory history = PanelInspectHistory.Get(inspectMission.PanelID);
                    _ = PreJudge(ref inspectMission, ref history);
                    AETresult aetResult = AETresult.Get(inspectMission.HistoryID);
                    PanelMission panelMission = new(inspectMission, aetResult);
                    PreDownloadedPanelMissionQueue.Enqueue(panelMission);
                    return true;
                }
                catch (NullReferenceException)
                {
                    SeverConnector.SendPanelMissionResult(new OperatorJudge(new Defect("异显", "DE00010"), User.AutoJudgeUser.Username, User.AutoJudgeUser.Account, User.AutoJudgeUser.Id, 1), inspectMission);
                    return PreLoadOneMission();
                }
            }
            catch (NullReferenceException)
            {
                return false;
            }
        }
        
        public bool NextMission()
        {
            if (PreDownloadedPanelMissionQueue.Count == 0)
            {
                return false;
            }
            else
            {
                onInspPanelMission = PreDownloadedPanelMissionQueue.Dequeue();
                return true;
            }
        }

        public static bool PreJudge(ref InspectMission mission, ref PanelInspectHistory aetResult)
        {
            //need prejudge logic;
            Defect defect = new("defactName", "defectCode");
            if (true)
            {
                return false;
            }
            else
            {
                SeverConnector.SendPanelMissionResult(new OperatorJudge(defect, User.AutoJudgeUser.Username, User.AutoJudgeUser.Account, User.AutoJudgeUser.Id, 1), mission);
                return true;
            }
        }
        
        public static void SendOpJudgeResult(Defect defect, User user, int score, InspectMission inspectMission)
        {
            SeverConnector.SendPanelMissionResult(new OperatorJudge(defect, user.Username, user.Account, user.Id, score), inspectMission);
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
            if (DefectImages == null)
            {
                return;
            }
            else
            {
                IniImageDic(ref defectImageDataDic, DefectImages);

            }
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
