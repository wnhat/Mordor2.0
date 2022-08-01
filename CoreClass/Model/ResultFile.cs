﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreClass.DICSEnum;
using CoreClass.Model;
using CoreClass.Element;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.Drawing;

namespace CoreClass.Model
{
    [BsonIgnoreExtraElements]
    public class AETresult
    {
        [BsonIgnore]
        public static IMongoCollection<AETresult> AETresultCollection = DBconnector.DICSDB.GetCollection<AETresult>("AETresult");

        [BsonId]
        public ObjectId Id { get; set; }
        public PanelInspectHistory history { get; set; }
        public string PanelId;
        public ImageContainer[] ResultImages;
        public ImageContainer[] DefectImages;

        // 对于检查结果文件中的 765H210002A9AAT05.txt 进行解析,填充下列字段；
        public List<DefectInfo> DefectCollection = new List<DefectInfo>();
        public string AVIRecipeName;
        public string SVIRecipeName;
        public string LotID;
        public string PanelID;//Notice: May be null
        public string AviJudge;
        public string SviJudge;
        public string RecipeName;
        public Coordinate AviRoiStart;
        public Coordinate AviRoiEnd;
        public Coordinate SviRoiStart;
        public Coordinate SviRoiEnd;
        public string AviContours;
        public string SviContours;

        // 当没有相关文件的情况下，这些值有可能会是空或null或default，在使用该值前应自行校验；
        public int EqId;
        public string AviIp;
        [BsonRepresentation(BsonType.String)]
        public Disk AviDisk;
        public string SviIp;
        [BsonRepresentation(BsonType.String)]
        public Disk SviDisk;

        public string EqName
        {
            get
            {
                if (EqId > 9)
                {
                    return "7CTCT" + EqId.ToString();
                }
                else
                {
                    return "7CTCT0" + EqId.ToString();
                }
            }
        }
        public string AviOriginImagePath
        {
            get
            {   // \\172.16.180.83\NetworkDrive\F_Drive\Defect Info\Origin
                string returnstring = "\\\\" + AviIp + "\\NetworkDrive\\" + AviDisk + "\\Defect Info\\Origin\\" + PanelId;
                return returnstring;
            }
        }
        public string AviResultPath
        {
            get
            {   // \\172.16.180.83\NetworkDrive\F_Drive\Defect Info\Result
                string returnstring = "\\\\" + AviIp + "\\NetworkDrive\\" + AviDisk + "\\Defect Info\\Result\\" + PanelId;
                return returnstring;
            }
        }
        public string SviOriginImagePath
        {
            get
            {   // \\172.16.180.83\NetworkDrive\F_Drive\Defect Info\Origin
                string returnstring = "\\\\" + SviIp + "\\NetworkDrive\\" + SviDisk + "\\Defect Info\\Origin\\" + PanelId;
                return returnstring;
            }
        }
        public string SviResultPath
        {
            get
            {   // \\172.16.180.83\NetworkDrive\F_Drive\Defect Info\Result
                string returnstring = "\\\\" + SviIp + "\\NetworkDrive\\" + SviDisk + "\\Defect Info\\Result\\" + PanelId;
                return returnstring;
            }
        }
        public string[] ResultImagesName 
        { 
            get
            {
                var names = from item in ResultImages
                                 select item.Name;
                return names.ToArray();
            } 
        }
        public AETresult(PanelInspectHistory his,PanelPathContainer[] panelPaths)
        {
            this.history = his;
            PanelId = his.PanelId;
            EqId = his.EqpID;

            List<ImageContainer> resultImages = new List<ImageContainer>();
            List<ImageContainer> defectimages = new List<ImageContainer>();

            foreach (var path in panelPaths)
            {
                var dirTime = new DirectoryInfo(path.OriginImagePath).CreationTime;
                if (dirTime - his.InspDate < TimeSpan.FromMinutes(30))
                {
                    // 添加DICS用图
                    DirContainer dir = new DirContainer(path.ResultPath);
                    
                    // 添加Defect缩略图；
                    if (dir.FileContainerArray != null && dir != null)
                    {
                        foreach (var Defectimagefile in dir.FileContainerArray)
                        {
                            if (Defectimagefile.Name.Contains(".jpg"))
                            {
                                defectimages.Add(new ImageContainer(Defectimagefile.Name, Defectimagefile.Data));
                            }
                            // 765H210002A9AAT05.txt
                            if (Defectimagefile.Name == his.PanelId + ".txt")
                            {
                                string[] lines = new String(Encoding.UTF8.GetChars(Defectimagefile.Data)).Split('\n');
                                foreach (var line in lines)
                                {
                                    string[] field = line.Split(',');
                                    if (line.StartsWith("DATA,HEADER_DATA"))
                                    {
                                        // TODO: 记录检查工站TT信息；
                                    }
                                    if (line.StartsWith("DATA,PANELDATA") && field.Length >= (int)ResultIndexPanelData.ROI_START_Y)
                                    {
                                        this.AVIRecipeName = field[(int)ResultIndexPanelData.RecipeName];
                                        this.LotID = field[(int)ResultIndexPanelData.LotID];
                                        
                                        this.RecipeName = field[(int)ResultIndexPanelData.RecipeName];
                                        if (path.PcName == Pcinfo.AVI)
                                        {
                                            this.AviRoiStart = new Coordinate(Convert.ToInt32(field[(int)ResultIndexPanelData.ROI_START_X]), Convert.ToInt32(field[(int)ResultIndexPanelData.ROI_START_Y]));
                                            this.AviRoiEnd = new Coordinate(Convert.ToInt32(field[(int)ResultIndexPanelData.ROI_END_X]), Convert.ToInt32(field[(int)ResultIndexPanelData.ROI_END_Y]));
                                            this.AviJudge = field[(int)ResultIndexPanelData.Judge];
                                        }
                                        else
                                        {
                                            this.SviJudge = field[(int)ResultIndexPanelData.Judge];
                                            this.SviRoiStart = new Coordinate(Convert.ToInt32(field[(int)ResultIndexPanelData.ROI_START_X]), Convert.ToInt32(field[(int)ResultIndexPanelData.ROI_START_Y]));
                                            this.SviRoiEnd = new Coordinate(Convert.ToInt32(field[(int)ResultIndexPanelData.ROI_END_X]), Convert.ToInt32(field[(int)ResultIndexPanelData.ROI_END_Y]));
                                        }
                                    }
                                    if (line.StartsWith("DATA,DEFECT") && field.Length >= (int)ResultIndexDefectData.PS_FLAG)
                                    {
                                        DefectInfo newdefect = new DefectInfo
                                        {
                                            PatternName = field[(int)ResultIndexDefectData.IMG_NAME],
                                            DefectName = field[(int)ResultIndexDefectData.DEF_TYPE],
                                            DefectCode = field[(int)ResultIndexDefectData.DEFECT_CODE],
                                            PixelStart = new Coordinate(Convert.ToInt32(field[(int)ResultIndexDefectData.PIXEL_START_X]), Convert.ToInt32(field[(int)ResultIndexDefectData.PIXEL_START_Y])),
                                            PixelEnd = new Coordinate(Convert.ToInt32(field[(int)ResultIndexDefectData.PIXEL_END_X]), Convert.ToInt32(field[(int)ResultIndexDefectData.PIXEL_END_Y])),
                                            CoordStart = new Coordinate(Convert.ToDecimal(field[(int)ResultIndexDefectData.COORD_START_X]), Convert.ToDecimal(field[(int)ResultIndexDefectData.COORD_START_Y])),
                                            CoordEnd = new Coordinate(Convert.ToDecimal(field[(int)ResultIndexDefectData.COORD_END_X]), Convert.ToDecimal(field[(int)ResultIndexDefectData.COORD_END_Y])),
                                            PsFlag = field[(int)ResultIndexDefectData.PS_FLAG],
                                            Size = Convert.ToInt32(field[(int)ResultIndexDefectData.DEF_SIZE]),
                                            DefectImageFileName = field[(int)ResultIndexDefectData.DEF_IMG_FILE],
                                            DRAW_RECT = field[(int)ResultIndexDefectData.DRAW_RECT],
                                            CameraNumber = Int32.Parse(field[(int)ResultIndexDefectData.CAM_NO]),
                                        };
                                        this.DefectCollection.Add(newdefect);
                                    }
                                }
                            }
                        }
                    }
                    
                    if (path.PcName == Pcinfo.AVI)
                    {
                        this.AviIp = path.PcIp;
                        this.AviDisk = path.diskName;

                        // 添加DICS用图；
                        var imagedir = dir.GetDirContainer("MNImg");
                        if (imagedir != null)
                        {
                            foreach (var DICSimage in imagedir.FileContainerArray)
                            {
                                if (DICSimage.Name.Contains(".jpg"))
                                {
                                    resultImages.Add(new ImageContainer(DICSimage.Name, DICSimage.Data));
                                }
                            }
                        }
                        // 添加contours；
                        var contours = dir.GetFileContainer("Contours.Merge");
                        if (contours != null)
                        {
                            this.AviContours = new string(Encoding.UTF8.GetChars(contours.Data));
                        }
                    }
                    else if (path.PcName == Pcinfo.SVI)
                    {
                        this.SviIp = path.PcIp;
                        this.SviDisk = path.diskName;
                        // 添加DICS用图；
                        var imagedir = dir.GetDirContainer("SVI_ColorSpace");
                        if (imagedir != null)
                        {
                            foreach (var DICSimage in imagedir.FileContainerArray)
                            {
                                if (DICSimage.Name.Contains(".jpg"))
                                {
                                    resultImages.Add(new ImageContainer(DICSimage.Name, DICSimage.Data));
                                }
                            }
                        }
                        // 添加contours；
                        var contours = dir.GetFileContainer("Contours.Merge");
                        if (contours != null)
                        {
                            this.SviContours = new String(Encoding.UTF8.GetChars(contours.Data));
                        }
                    }
                }
            }

            if (resultImages.Count != 0)
            {
                this.ResultImages = resultImages.ToArray();
            }
            if (defectimages.Count != 0)
            {
                this.DefectImages = defectimages.ToArray();
            }
        }
        public static AETresult Get(ObjectId id)
        {
            var getresult = AETresultCollection.Find(x => x.history.ID == id).FirstOrDefault();
            return getresult;
        }

        public static List<AETresult> Get(string panelId)
        {
            var getresults = AETresultCollection.Find(x => x.PanelId == panelId);
            return getresults.ToList();
        }
    }

    public class ImageContainer
    {
        public string Name { get; set; }
        public byte[] Data { get; set; }

        [JsonConstructor]
        public ImageContainer() { }

        public ImageContainer(string name)
        {
            Name = name;
        }

        public ImageContainer(string name, byte[] data)
        {
            Name = name;
            Data = data;
        }

        public static ImageContainer GetDefault
        {
            get
            {
                int cellX = 1500;
                int cellY = 600;
                Bitmap defectMap = new(cellX, cellY);
                Graphics graphics = Graphics.FromImage(defectMap);
                SolidBrush solidBrush = new SolidBrush(Color.FromArgb(182, 226, 255));
                graphics.FillRectangle(solidBrush, new Rectangle(new Point(0, 0), new Size(cellX, cellY)));
                MemoryStream buffer = new();
                defectMap.Save(buffer, System.Drawing.Imaging.ImageFormat.Jpeg);
                ImageContainer imageContainer = new("DefaultImage", buffer.ToArray());
                return imageContainer;
            }
        }
    }
    public class DefectInfo
    {
        public string PatternName { get; set; }
        public string DefectName { get; set; }
        public string DefectCode { get; set; }
        public Coordinate PixelStart { get; set; }
        public Coordinate PixelEnd { get; set; }
        public Coordinate CoordStart { get; set; }
        public Coordinate CoordEnd { get; set; }
        public int Size { get; set; }
        public string DefectImageFileName { get; set; }
        public string DRAW_RECT { get; set; }
        public int CameraNumber { get; set; }
        public string PsFlag { get; set; }
    }
}
