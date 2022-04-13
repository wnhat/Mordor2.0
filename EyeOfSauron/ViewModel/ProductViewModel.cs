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
        private KeyValuePair<ProductInfo, int> _selectProductInfo;
        private List<KeyValuePair<ProductInfo, int>> _productInfos = new();
        public ProductViewModel(UserInfoViewModel userInfo)
        {
            _userInfo = userInfo;
            KeyValuePair<ProductInfo, int> productInfo = InspectMission.GetWaittingMissionOverView();
            //TODO: 应当返回List<KeyValuePair<ProductInfo, int>>
            //selectProductInfo = _productInfos[0];
            //TODO 
        }
        public List<KeyValuePair<ProductInfo, int>> ProductInfos
        {
            get { return _productInfos; }  
            set { SetProperty(ref _productInfos, value); }
        }
        public KeyValuePair<ProductInfo, int> SelectProductInfo
        {
            get => _selectProductInfo;
            set => SetProperty(ref _selectProductInfo, value);
        }
    }
}
