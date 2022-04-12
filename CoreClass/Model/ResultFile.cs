using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreClass.DICSEnum;
using CoreClass.Element;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace CoreClass.Model
{
    public class AETresult
    {
        [BsonIgnore]
        public static IMongoCollection<AETresult> AETresultCollection = DBconnector.DICSDB.GetCollection<AETresult>("AETresult");

        [BsonId]
        public ObjectId Id { get; set; }
        public PanelInspectHistory history;
        public string PanelId;
        public ImageContainer[] ResultImages;
        public ImageContainer[] DefectImages;

        // 对于检查结果文件中的 765H210002A9AAT05.txt 进行解析,填充下列字段；
        public string defectCollection;
        public string AVIRecipeName;
        public string SVIRecipeName;

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
                        // 添加Defect缩略图；
                        if (dir.FileContainerArray != null && dir != null)
                        {
                            foreach (var Defectimagefile in dir.FileContainerArray)
                            {
                                if (Defectimagefile.Name.Contains(".jpg"))
                                {
                                    defectimages.Add(new ImageContainer(Defectimagefile.Name, Defectimagefile.Data));
                                }
                            }
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
                        // 添加Defect缩略图；
                        if (dir != null)
                        {
                            foreach (var Defectimagefile in dir.FileContainerArray)
                            {
                                if (Defectimagefile.Name.Contains(".jpg"))
                                {
                                    defectimages.Add(new ImageContainer(Defectimagefile.Name, Defectimagefile.Data));
                                }
                            }
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
    }

    public class ImageContainer
    {
        public string Name;
        public byte[] Data;
        public ImageContainer(string name, byte[] data)
        {
            Name = name;
            Data = data;
        }
    }
}
