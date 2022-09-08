
using System;

namespace CutInspect.Model
{
    public class InspectItem
    {
        public string? Id { get; set; }
        public string?  ImageName { get; set; }
        public string?  ProductId { get; set; }
        public string?  LayerId { get; set; }
        public string?  EquipmentId{ get; set; }
        public string?  Direction{ get; set; }
        public int?  Status{ get; set; }
        public string? DefectBoxs{ get; set; }
        public string?  Note{ get; set; }
        public DateTime?  StartTime{ get; set; }
        public DateTime? EndTime{ get; set; }
        public string?  ImagePath{ get; set; }
        public string?  FtpPath{ get; set; }
        public string?  ExtendOne{ get; set; }
        public string?  ExtendTwo{ get; set; }
        public string?  ExtendThree{ get; set; }
        public string?  CreateBy{ get; set; }
        public DateTime? CreateDate{ get; set; }
        public string? UpdateBy{ get; set; }
        public DateTime? UpdateDate{ get; set; }
    }
}
