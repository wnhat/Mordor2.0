using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using CoreClass;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using CoreClass.Model;

namespace EyeOfSauron.ViewModel
{
    
    public class ProductViewModel : ViewModelBase
    {
        public readonly UserInfoViewModel _userInfo;
        private ObservableCollection<ProductCardViewModel> _productCardViewModels = new();
        private KeyValuePair<ProductInfo, int> _selectProductInfo;
        private List<KeyValuePair<ProductInfo, int>> _productInfos = new();
        private bool[] product = new bool[10];
        public ProductViewModel(UserInfoViewModel userInfo)
        {
            _userInfo = userInfo;
            KeyValuePair<ProductInfo, int> productInfo = InspectMission.GetWaittingMissionOverView();
            for (int i = 0; i < Product.Length; i++)
            {
                if (i < ProductInfos.Count)
                {
                    Product[i] = true;
                }
                else
                {
                    Product[i] = false;
                }
            }
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
        public bool[] Product
        {
            get => product;
            set => SetProperty(ref product, value);
        }
        public ObservableCollection<ProductCardViewModel> ProductCardViewModels
        {
            get => _productCardViewModels;
            set => SetProperty(ref _productCardViewModels, value);
        }
    }
}
