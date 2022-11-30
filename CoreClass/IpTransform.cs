using CoreClass.DICSEnum;
using CoreClass.Model;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreClass
{
    public static class IpTransform
    {
        public static IMongoCollection<PC> IP = DBconnector.DICSDB.GetCollection<PC>("IP");
        public static List<PC> PcListAll;
        static IpTransform()
        {
            PcListAll = IP.Find(e => true).ToList();
        }
        public static List<PC> Name2IP(List<int> eq_id_list, Pcinfo[] pc_name_list, Side[] pc_side_list)
        {
            return PcListAll.Where(x => { return x.IsPcInType(eq_id_list, pc_name_list, pc_side_list); }).ToList();
        }
        public static List<PC> Name2IP(List<int> eq_id_list, Pcinfo[] pc_name_list)
        {
            Side[] pc_side_list = (Side[])Enum.GetValues(typeof(Side));
            return Name2IP(eq_id_list, pc_name_list, pc_side_list);
        }
        public static List<PC> Name2IP(List<int> eq_id_list)
        {
            Side[] pc_side_list = (Side[])Enum.GetValues(typeof(Side));
            Pcinfo[] pc_name_list = (Pcinfo[])Enum.GetValues(typeof(Pcinfo));
            return Name2IP(eq_id_list, pc_name_list, pc_side_list);
        }
        public static List<PC> Name2IP(Pcinfo[] pc_name_list)
        {
            // GROUP BY PC name;
            Side[] pc_side_list = (Side[])Enum.GetValues(typeof(Side));
            List<int> eq_id_list = Enumerable.Range(1, 33).ToList();
            return Name2IP(eq_id_list, pc_name_list, pc_side_list);
        }
        public static PC GetPC(string pcip)
        {
            return PcListAll.Where((x) => (x.PcIp == pcip)).FirstOrDefault() ;
        }
    }

}
