
using System;

namespace CutInspect
{
    public class InspectItem
    {
        public string? id { get; set; }
        public string?  imageName { get; set; }
        public string?  productId { get; set; }
        public string?  layerId { get; set; }
        public string?  equipmentId{ get; set; }
        public string?  direction{ get; set; }
        public int  status{ get; set; }
        public string? defectBoxs{ get; set; }
        public string?  note{ get; set; }
        public DateTime?  startTime{ get; set; }
        public DateTime? endTime{ get; set; }
        public string?  imagePath{ get; set; }
        public string?  ftpPath{ get; set; }
        public string?  extendOne{ get; set; }
        public string?  extendTwo{ get; set; }
        public string?  extendThree{ get; set; }
        public string?  createBy{ get; set; }
        public DateTime? createDate{ get; set; }
        public string? updateBy{ get; set; }
        public DateTime? updateDate{ get; set; }
    }
}
