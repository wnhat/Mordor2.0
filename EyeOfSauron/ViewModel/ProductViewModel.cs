using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using CoreClass;
using CoreClass.Model;

namespace EyeOfSauron.ViewModel
{
    
    public class ProductViewModel : ViewModelBase
    {
        public readonly UserInfoViewModel _userInfo;
        public KeyValuePair<ProductInfo, int> _selectProductInfo;
        private List<KeyValuePair<ProductInfo, int>> _productInfos;
        public ProductViewModel(UserInfoViewModel userInfo)
        {
            _userInfo = userInfo;
            KeyValuePair<ProductInfo, int> productInfos = InspectMission.GetWaittingMissionOverView();
            //TODO: 应当返回List<KeyValuePair<ProductInfo, int>>
            //selectProductInfo = _productInfos[0];
            //TODO 
        }
        public List<KeyValuePair<ProductInfo, int>> productInfos
        {
            get { return _productInfos; }  
            set { SetProperty(ref _productInfos, value); }
        }public KeyValuePair<ProductInfo, int> selectProductInfo
        {
            get { return _selectProductInfo; }
            set { SetProperty(ref _selectProductInfo, value); }
        }

    }
}
