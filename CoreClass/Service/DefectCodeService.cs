using CoreClass;
using CoreClass.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreClass.Service
{
    public interface IDefectCodeService
    {
        Task<List<Defect>> GetAllDefectCodes();
        Task<Defect> CreateDefectCode(Defect param);
        Task UpdateDefectCode(Defect param);
        Task DeleteDefectCode(string code);
        Task DeleteDefectCodeMany(string[] code_list);
    }

    public class DefectCodeService : IDefectCodeService
    {
        private static readonly IMongoCollection<Defect> _defectcodes = DBconnector.DICSDB.GetCollection<Defect>("DefectCode");

        public Task<Defect> CreateDefectCode(Defect param)
        {
            return Task.Run(() =>
            {
                var filter = Builders<Defect>.Filter.Eq("DefectCode", param.DefectCode);
                if (_defectcodes.Find(filter).FirstOrDefault() != null)
                {
                    throw new ApplicationException("Defect \"" + param.DefectCode + "\" is already existed");
                }

                _defectcodes.InsertOne(param);

                return param;
            });
        }

        public Task DeleteDefectCode(string code)
        {
            return Task.Run(() => { return _defectcodes.DeleteOne(defectCode => defectCode.DefectCode == code); });
        }

        public Task DeleteDefectCodeMany(string[] code_list)
        {
            return Task.Run(() =>
            {
                // 全选删除
                if (code_list.Length == 0)
                {
                    return _defectcodes.DeleteMany(Builders<Defect>.Filter.Empty);
                }
                // 多选删除
                else
                {
                    return _defectcodes.DeleteMany(defectCode => code_list.Contains(defectCode.DefectCode));
                }
            });
        }

        public Task<List<Defect>> GetAllDefectCodes()
        {
            return Task.Run(() => { return _defectcodes.Find(x => true).ToList(); });
        }

        public Task UpdateDefectCode(Defect param)
        {
            return Task.Run(() =>
            {
                var filter = Builders<Defect>.Filter.Eq("Id", param.Id);
                var defectCode = _defectcodes.Find(filter).FirstOrDefault();

                if (defectCode == null)
                {
                    throw new ApplicationException("Defect Code not found!");
                }

                _defectcodes.ReplaceOne(find => find.Id == param.Id, param);
                
            });
        }
    }
}
