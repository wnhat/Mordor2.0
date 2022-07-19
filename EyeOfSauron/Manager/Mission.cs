using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using CoreClass;
using CoreClass.Model;
using System.IO;
using CoreClass.Service;
using EyeOfSauron.ViewModel;
using MongoDB.Bson;

namespace EyeOfSauron
{
    public class Mission
    {
        //TODO: 区分任务类型
        private readonly Queue<PanelMission> PreDownloadedPanelMissionQueue = new();

        public PanelMission onInspPanelMission;

        public readonly ProductInfo? productInfo;

        public ExamMissionWIP? ExamMissionWIP;

        public ControlTableItem MissionType;

        readonly object Predownloadlock = new();

        public Mission(object o, ControlTableItem MissionType = ControlTableItem.ProductMission)
        {
            this.MissionType = MissionType;
            switch (MissionType)
            {
                default:
                case ControlTableItem.ProductMission:
                    if(o is ProductInfo info)
                    {
                        productInfo = info;
                    }
                    break;
                case ControlTableItem.ExamMission:
                    if(o is ExamMissionWIP exam)
                    {
                        ExamMissionWIP = exam;
                    }
                    break;
                
            }
            if (PreLoadOneMission())
            {
                onInspPanelMission = PreDownloadedPanelMissionQueue.Dequeue();
            }
            else
            {
                throw new MissionEmptyException("Mission Empty");
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

        /// <summary>
        /// Enqueue one available PanelMission to PreDownloadedPanelMissionQueue Recursively;
        /// </summary>
        /// <returns></returns>
        public bool PreLoadOneMission()
        {
            switch (MissionType)
            {
                case ControlTableItem.ProductMission:
                    InspectMission? inspectMission = InspectMission.GetMission(productInfo);
                    if (inspectMission == null)
                    {
                        return false;
                    }
                    AETresult? aetResult = AETresult.Get(inspectMission.HistoryID);
                    if (aetResult == null)
                    {
                        SeverConnector.SendPanelMissionResult(new OperatorJudge(new Defect("异显", "DE00010"), User.AutoJudgeUser.Username, User.AutoJudgeUser.Account, User.AutoJudgeUser.Id, 1), inspectMission);
                        return PreLoadOneMission();
                    }
                    else
                    {
                        PanelMission panelMission = new(aetResult, inspectMission,null);
                        PreDownloadedPanelMissionQueue.Enqueue(panelMission);
                        return true;
                    }
                case ControlTableItem.ExamMission:
                    ExamMissionResult? ExamMission = ExamMissionResult.GetOneAndUpdate(ExamMissionWIP.UserID, ExamMissionWIP.MissionCollectionName);
                    if (ExamMission == null)
                    {
                        return true;
                    }
                    AETresult? examMissionAetResult = PanelSample.GetSample(ExamMission.PanelSampleId).AetResult;
                    if (examMissionAetResult == null)
                    {
                        //SeverConnector.SendPanelMissionResult(new OperatorJudge(new Defect("异显", "DE00010"), User.AutoJudgeUser.Username, User.AutoJudgeUser.Account, User.AutoJudgeUser.Id, 1), inspectMission);
                        return PreLoadOneMission();
                    }
                    else
                    {
                        PanelMission panelMission = new(examMissionAetResult, null, ExamMission);
                        PreDownloadedPanelMissionQueue.Enqueue(panelMission);
                        return true;
                    }
                default:
                    return false;
            }

        }

        /// <summary>
        /// Dequeue one PanelMission to onInspPanelMission of PreDownloadedPanelMissionQueue if where are remaining missions
        /// </summary>
        /// <returns>True if PreDownloadedPanelMissionQueue success; False if PreDownloadedPanelMissionQueue is empty;</returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Sum of preLoaded_mission quantity and remaining quantity of the product In DICSDB;</returns>
        public async Task<int> RemainMissionCount()
        {
            return PreDownloadedPanelMissionQueue.Count + await GetRemainingQuantityOfTheProduct();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Remaining quantity of the product In DICSDB;</returns>
        private async Task<int> GetRemainingQuantityOfTheProduct()
        {
            BsonDocument remainMissionCount = MissionType switch
            {
                ControlTableItem.ExamMission => await ExamMissionResult.GetRemainMissionCount(ExamMissionWIP.UserID, ExamMissionWIP.MissionCollectionName),
                _ => await DICSRemainInspectMissionService.GetRemainMissionCount(productInfo.Id),
            };
            if (remainMissionCount != null)
            {
                return remainMissionCount.GetValue("count").ToInt32();
            }
            return 0;
        }
    }

    /// <summary>
    ///  Initialize with InspectMission and get ResultImage,DefectImage and ContoursImage by AETresult;
    /// </summary>
    public class PanelMission
    {
        public List<ImageContainer> resultImageDataList = new();

        public List<BitmapImageContainer> defectImageDataList = new();

        public BitmapImageContainer ContoursImageContainer = new(new ImageContainer());

        public InspectMission? inspectMission;

        public ExamMissionResult? examMission;

        public AETresult AetResult { get; set; }

        public ProductInfo ProductInfo { get; set; }
        
        public PanelMission(AETresult result, InspectMission? mission = null, ExamMissionResult? examMissionResult = null)
        {
            inspectMission = mission;
            examMission = examMissionResult;
            AetResult = result;
            ProductInfo = ProductInfo.GetProductInfo(result.PanelId);
            IniResultImageDataList(AetResult.ResultImages);
            IniDefectImageDataList(AetResult.DefectImages, AetResult.DefectCollection);
            InitialContoursImage();
        }

        private void InitialContoursImage()
        {
            DetailDefectContours defect = new(AetResult.AviContours, AetResult.SviContours);
            ContoursImageContainer = new(defect.GetImageContainer());
        }

        public void IniResultImageDataList(ImageContainer[] resultImages)
        {
            List<string> imageNmaeIndex = new();
            
            if (resultImages != null)
            {
                foreach (ImageContainer image in resultImages)
                {
                    imageNmaeIndex.Add(image.Name);
                }
            }
            foreach (string aviImageName in Parameter.AviImageNameList)
            {
                if (imageNmaeIndex.Contains(aviImageName))
                {
                    resultImageDataList.Add(resultImages[imageNmaeIndex.IndexOf(aviImageName)]);
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
                    resultImageDataList.Add(resultImages[imageNmaeIndex.IndexOf(sviImageName)]);
                }
                else
                {
                    resultImageDataList.Add(new ImageContainer(sviImageName));
                }
            }
        }
        
        public void IniDefectImageDataList(ImageContainer[] defectImages, List<DefectInfo> defectInfos)
        {
            if (defectImages == null)
            {
                return;
            }
            else if (defectInfos.Count == 0)
            {
                for (int i = 0; i < defectImages.Length; i++)
                {
                    defectImageDataList.Add(new BitmapImageContainer(defectImages[i]));
                    if (i > 50)
                    {
                        break;
                    }
                }
            }
            else
            {
                List<string> defectImageNmaeIndex = new();
                List<string> defectInfoImageNmaeIndex = new();
                foreach (ImageContainer defectImage in defectImages)
                {
                    defectImageNmaeIndex.Add(defectImage.Name);
                }
                foreach (DefectInfo defectInfo in defectInfos)
                {
                    defectInfoImageNmaeIndex.Add(defectInfo.DefectImageFileName);
                }
                foreach (string ImageName in defectInfoImageNmaeIndex)
                {
                    if (defectImageNmaeIndex.Contains(ImageName))
                    {
                        defectImageDataList.Add(new BitmapImageContainer(defectImages[defectImageNmaeIndex.IndexOf(ImageName)], defectInfos[defectInfoImageNmaeIndex.IndexOf(ImageName)]));
                    }
                    else
                    {
                        defectImageDataList.Add(new BitmapImageContainer(ImageContainer.GetDefault, defectInfos[defectInfoImageNmaeIndex.IndexOf(ImageName)]));
                    }
                }
            }
        }
    }

    /// <summary>
    /// Initialize BitmapImage with ImageContainer to show on UI;
    /// </summary>
    public class BitmapImageContainer : ImageContainer
    {
        public BitmapImage BitmapImage { get; set; }
        public DefectInfo DefectInfo { get; set; }
        public BitmapImageContainer(ImageContainer imageContainer, DefectInfo? defectInfo = null)
        {
            DefectInfo = defectInfo ?? new DefectInfo();
            ImageContainer buffer;
            if (imageContainer == null || imageContainer.Data == null || imageContainer.Data.Length == 0)
            {
                buffer = GetDefault;
                Name = imageContainer?.Name;
            }
            else
            {
                buffer = imageContainer;
                Name = imageContainer.Name;
            }
            BitmapImage = new BitmapImage();
            BitmapImage.BeginInit();
            BitmapImage.StreamSource = new MemoryStream(buffer.Data);
            BitmapImage.EndInit();
            BitmapImage.Freeze();
        }
    }
    public class MissionEmptyException : Exception
    {
        public MissionEmptyException(string? message) : base(message)
        {
        }
    }
}
