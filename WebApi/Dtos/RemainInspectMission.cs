using CoreClass.Model;
using System.Collections;
using System.Collections.Generic;

namespace WebApi.Dtos
{
    public class RemainInspectMission
    {
        public ProductInfo productInfo { get; set; }
        public int remainInspectCount { get; set; }
    }
}
