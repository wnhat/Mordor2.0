using DICS_WebApi.Models;
using WebApi.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Service
{
    public interface IDefectCodeService
    {
        Task<List<DefectCode>> GetAllDefectCodes();
        Task<DefectCode> CreateDefectCode(DefectCode param);
        Task UpdateDefectCode(DefectCode param);
        Task DeleteDefectCode(string code);
        Task DeleteDefectCodeMany(string[] code_list);
    }

    public class DefectCodeService : IDefectCodeService
    {
        private readonly IMongoCollection<DefectCode> _defectcodes;

        public DefectCodeService(IUserDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _defectcodes = database.GetCollection<DefectCode>(settings.UsersCollectionName.DefectCode);
        }

        public Task<DefectCode> CreateDefectCode(DefectCode param)
        {
            return Task.Run(() =>
            {
                var filter = Builders<DefectCode>.Filter.Eq("Code", param.Code);
                if (_defectcodes.Find(filter).FirstOrDefault() != null)
                {
                    throw new ApplicationException("Defect \"" + param.Code + "\" is already existed");
                }

                _defectcodes.InsertOne(param);

                return param;
            });
        }

        public Task DeleteDefectCode(string code)
        {
            return Task.Run(() => { return _defectcodes.DeleteOne(defectCode => defectCode.Code == code); });
        }

        public Task DeleteDefectCodeMany(string[] code_list)
        {
            return Task.Run(() =>
            {
                // 全选删除
                if (code_list.Length == 0)
                {
                    return _defectcodes.DeleteMany(Builders<DefectCode>.Filter.Empty);
                }
                // 多选删除
                else
                {
                    return _defectcodes.DeleteMany(defectCode => code_list.Contains(defectCode.Code));
                }
            });
        }

        public Task<List<DefectCode>> GetAllDefectCodes()
        {
            return Task.Run(() => { return _defectcodes.Find(defectCode => true).ToList(); });
        }

        public Task UpdateDefectCode(DefectCode param)
        {
            return Task.Run(() =>
            {
                var filter = Builders<DefectCode>.Filter.Eq("Id", param.Id);
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
