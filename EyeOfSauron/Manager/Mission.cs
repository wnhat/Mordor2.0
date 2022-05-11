﻿using System;
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

        static readonly DICSRemainInspectMissionService dicsRemainInspectMissionService = new();

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
                    //while (PreDownloadedPanelMissionQueue.Count <= 3)
                    while (PreDownloadedPanelMissionQueue.Count <= Parameter.PreLoadQuantity)
                    {
                        if (!PreLoadOneMission())
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
                if (inspectMission == null)
                {
                    return false;
                }
                try
                {
                    PanelInspectHistory history = PanelInspectHistory.Get(inspectMission.PanelID);
                    //_ = PreJudge(ref inspectMission, ref history);
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

        public async Task<int> RemainMissionCount()
        {
            return PreDownloadedPanelMissionQueue.Count + await GetRemainingQuantityOfTheProduct();
        }

        private async Task<int> GetRemainingQuantityOfTheProduct()
        {
            // get the remaining mission quantity to set viewmodel
            var remainMissionCount = await dicsRemainInspectMissionService.GetRemainMissionCount(productInfo.Id);
            return remainMissionCount.GetValue("count").ToInt32();
        }

        public static bool PreJudge(ref InspectMission mission, ref PanelInspectHistory aetResult)
        {
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
        public List<ImageContainer> resultImageDataList = new();

        public List<ImageContainer> defectImageDataList = new();

        public InspectMission inspectMission;

        public AETresult aetResult;

        public PanelMission(InspectMission mission)
        {
            inspectMission = mission;
            aetResult = AETresult.Get(inspectMission.HistoryID);
            IniResultImageDataList(aetResult.ResultImages);
            IniDefectImageDataList(aetResult.DefectImages);
        }

        public PanelMission(InspectMission mission, AETresult result)
        {
            inspectMission = mission;
            aetResult = result;
            IniResultImageDataList(aetResult.ResultImages);
            IniDefectImageDataList(aetResult.DefectImages);
        }

        public void IniResultImageDataList(ImageContainer[] ResultImages)
        {
            List<string> imageNmaeIndex = new();
            
            if (ResultImages != null)
            {
                foreach (ImageContainer image in ResultImages)
                {
                    imageNmaeIndex.Add(image.Name);
                }
            }
            foreach (string aviImageName in Parameter.AviImageNameList)
            {
                if (imageNmaeIndex.Contains(aviImageName))
                {
                    resultImageDataList.Add(ResultImages[imageNmaeIndex.IndexOf(aviImageName)]);
                }
                else
                {
                    resultImageDataList.Add(new ImageContainer(aviImageName));
                }
            }

            foreach (string sviImageName in Parameter.SviImageNameList)
            {
                if (imageNmaeIndex.Contains(sviImageName))
                {
                    resultImageDataList.Add(ResultImages[imageNmaeIndex.IndexOf(sviImageName)]);
                }
                else
                {
                    resultImageDataList.Add(new ImageContainer(sviImageName));
                }
            }
        }

        public void IniDefectImageDataList(ImageContainer[] DefectImages)
        {
            if (DefectImages == null)
            {
                return;
            }
            else
            {
                for (int i = 0; i < DefectImages.Length; i++)
                {
                    defectImageDataList.Add(DefectImages[i]);
                    if (i > 50)
                    {
                        break;
                    }
                }
            }
        }
    }
    
    public class BitmapImageContainer : ImageContainer
    {
        public BitmapImage? BitmapImage { get; set; }
        //initialize with ImageContainer constructor
        public BitmapImageContainer(ImageContainer imageContainer) : base(imageContainer.Name, imageContainer.Data)
        {
            if (imageContainer == null || imageContainer.Data == null || imageContainer.Data.Length == 0)
            {
                return;
            }
            BitmapImage = new BitmapImage();
            BitmapImage.BeginInit();
            BitmapImage.StreamSource = new MemoryStream(imageContainer.Data);
            BitmapImage.EndInit();
            BitmapImage.Freeze();
        }
    }
}
