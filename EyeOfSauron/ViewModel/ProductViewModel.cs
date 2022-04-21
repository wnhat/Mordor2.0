using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using CoreClass;
using System.Collections.ObjectModel;
using CoreClass.Model;

namespace EyeOfSauron.ViewModel
{
    
    public class ProductViewModel : ViewModelBase
    {
        public readonly UserInfoViewModel _userInfo;
        private ObservableCollection<ProductCardViewModel> _productCardViewModels = new();
        private ProductCardViewModel _selectedProductCardViewModel;
        private KeyValuePair<ProductInfo, int> _selectProductInfo;
        public ProductViewModel(UserInfoViewModel userInfo)
        {
            _userInfo = userInfo;
            
            //TODO: 期望返回List<KeyValuePair<ProductInfo, int>>对象
            KeyValuePair<ProductInfo, int> productInfo = InspectMission.GetWaittingMissionOverView();
            //selectProductInfo = _productInfos[0];
            //TODO 
        }

        public ProductCardViewModel SelectedProductCardViewModel
        {
            get => _selectedProductCardViewModel;
            set => SetProperty(ref _selectedProductCardViewModel, value);
        }
        public KeyValuePair<ProductInfo, int> SelectProductInfo
        {
            get => _selectProductInfo;
            set => SetProperty(ref _selectProductInfo, value);
        }
  
        public ObservableCollection<ProductCardViewModel> ProductInfos
        {
            get => _productCardViewModels;
            set => SetProperty(ref _productCardViewModels, value);
        }
    }
}
