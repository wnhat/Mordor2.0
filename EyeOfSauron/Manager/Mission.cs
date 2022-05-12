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
using System.Drawing;
using CoreClass.Service;
using Rectangle = System.Drawing.Rectangle;

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

        public List<BitmapImageContainer> bitmapImageContainers = new();

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

    

    class ContoursFile
    {
        int cellX = 1500;

        int cellY = 600;

        List<string> lineData1 = new List<string>();

        List<string> lineData2 = new List<string>();

        List<DefectContours> defectContoursList = new List<DefectContours>();

        public Bitmap defectMap = null;

        public ContoursFile(string filePath1, string filePath2)
        {
            if (!System.IO.File.Exists(filePath1))
            {
                if (!System.IO.File.Exists(filePath2))
                {
                }
                else
                {
                }
            }
            lineData1 = ReadFile(filePath1);
            lineData2 = ReadFile(filePath2);
            GetDefectContours(lineData1);
            GetDefectContours(lineData2);
            defectMap = DrawDefectMap(defectContoursList, cellX, cellY);
        }

        public ContoursFile(string filePath)
        {
            lineData1 = ReadFile(filePath);
            GetDefectContours(lineData1);
            defectMap = DrawDefectMap(defectContoursList, cellX, cellY);
        }

        private List<string> ReadFile(string filePath)
        {
            List<string> lineData = new List<string>();
            if (!System.IO.File.Exists(filePath))
            {
                return lineData;
            }
            using (FileStream fsRead = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fsRead, Encoding.Default))
                {
                    while (!sr.EndOfStream)
                    {
                        string lineInfo = sr.ReadLine().Trim();
                        lineInfo = lineInfo.Replace(" ", "");
                        lineData.Add(lineInfo);//添加每行数据到列表
                    }
                }
            }
            return lineData;
        }

        private void GetDefectContours(List<string> L)
        {
            double scaleX = 1;
            double scaleY = 1;
            List<Point> points = new List<Point>();
            DefectContours defectContours;
            for (int i = 0; i < L.Count; i++)
            {
                string s = L[i];
                if (s.StartsWith("Cell_X"))//记录cell尺寸
                {
                    string[] cellSizeInfo = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    scaleX = Convert.ToDouble(cellSizeInfo[0].Substring(7)) / (double)cellX;
                    scaleY = Convert.ToDouble(cellSizeInfo[1].Substring(7)) / (double)cellY;
                }
                else if (s.StartsWith("No="))//
                {
                    string[] defectInfo = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    int defectNum = Convert.ToInt32(defectInfo[0].Split(new char[] { '=' })[1]);
                    int pattern = Convert.ToInt32(defectInfo[1].Split(new char[] { '=' })[1]);
                    int camera = Convert.ToInt32(defectInfo[2].Split(new char[] { '=' })[1]);
                    int defect = Convert.ToInt32(defectInfo[3].Split(new char[] { '=' })[1]);
                    defectContours = new DefectContours(points.ToArray());
                    defectContoursList.Add(defectContours);
                    points.Clear();
                }
                else
                {
                    string[] pointLocation = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    int pointX = Convert.ToInt32(pointLocation[0]);
                    if (pointX > 10)
                    {
                        pointX = (int)((double)pointX / scaleX);
                    }
                    else
                    {
                        pointX += 5;
                    }
                    int pointY = Convert.ToInt32(pointLocation[1]);
                    if (pointY > 10)
                    {
                        pointY = (int)((double)pointY / scaleY);
                    }
                    else
                    {
                        pointY += 5;
                    }
                    points.Add(new Point(pointX, pointY));
                    if (i == L.Count - 1)
                    {
                        defectContours = new DefectContours(points.ToArray());
                        defectContoursList.Add(defectContours);
                    }
                }
            }
        }

        private Bitmap DrawDefectMap(List<DefectContours> defectContoursList, int length, int width)
        {
            List<Point[]> pointList = new List<Point[]>();

            foreach (DefectContours defectContours in defectContoursList)
            {
                pointList.Add(defectContours.points);
            }

            return DrawDefectMap(pointList, length, width); ;
        }

        private Bitmap DrawDefectMap(List<Point[]> pointList, int length, int width)
        {
            int cellX = length;//Cell X 尺寸

            int cellY = width;//Cell Y 尺寸

            Graphics g; //创建画图对象

            SolidBrush solidBrush = new SolidBrush(Color.FromArgb(69, 99, 73));

            defectMap = new Bitmap(cellX, cellY);//指明画布大小

            g = Graphics.FromImage(defectMap);//指明画图对象
            
            Rectangle r = new Rectangle(new Point(0, 0), new Size(cellX, cellY));

            g.FillRectangle(solidBrush, r);//添加背景颜色

            Pen defectDrawPen = new Pen(Color.White);

            defectDrawPen.Width = (float)1.6;

            foreach (Point[] p in pointList)
            {
                if (p.Length > 2)
                {
                    g.DrawPolygon(defectDrawPen, p);
                }
            }

            return defectMap;
        }
    }

    class DefectContours
    {
        int defectNum = 0;
        int pattern = 0;
        int camera = 0;
        int defect = 0;
        public Point[] points;
        public DefectContours(Point[] pointArray)
        {
            points = pointArray;
        }
    }
}
