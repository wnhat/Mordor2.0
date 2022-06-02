using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using CoreClass;
using CoreClass.Model;
using System.IO;
using System.Drawing;
using CoreClass.Service;
using Rectangle = System.Drawing.Rectangle;

namespace EyeOfSauron
{
    public class Mission
    {
        //TODO: 区分任务类型
        private readonly Queue<PanelMission> PreDownloadedPanelMissionQueue = new();

        public PanelMission onInspPanelMission;

        public readonly ProductInfo productInfo;

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
                    while (PreDownloadedPanelMissionQueue.Count <= 2)
                    //while (PreDownloadedPanelMissionQueue.Count <= Parameter.PreLoadQuantity)
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
                    //PanelInspectHistory history = PanelInspectHistory.Get(inspectMission.PanelID);
                    //_ = PreJudge(ref inspectMission, ref history);  //Prejudge is unnecessary;
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
            var remainMissionCount = await DICSRemainInspectMissionService.GetRemainMissionCount(productInfo.Id);
            if (remainMissionCount != null)
            {
                return remainMissionCount.GetValue("count").ToInt32();
            }
            return 0;
        }
    }

    public class PanelMission
    {
        public List<ImageContainer> resultImageDataList = new();

        public List<ImageContainer> defectImageDataList = new();

        public List<BitmapImageContainer> bitmapImageContainers = new();

        public BitmapImageContainer ContoursImageContainer = new(new ImageContainer());

        public InspectMission inspectMission;

        public AETresult aetResult;

        public PanelMission(InspectMission mission)
        {
            inspectMission = mission;
            aetResult = AETresult.Get(inspectMission.HistoryID);
            IniResultImageDataList(aetResult.ResultImages);
            IniDefectImageDataList(aetResult.DefectImages);
            InitialContoursImage();
        }

        public PanelMission(InspectMission mission, AETresult result) : this(mission)
        {
            aetResult = result;
        }

        private void InitialContoursImage()
        {
            DetailDefectContours defect = new(aetResult.AviContours, aetResult.SviContours);
            ContoursImageContainer = new(defect.GetImageContainer());
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
                    bitmapImageContainers.Add(new BitmapImageContainer(DefectImages[i]));
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
