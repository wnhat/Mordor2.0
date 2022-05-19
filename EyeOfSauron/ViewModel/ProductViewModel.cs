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

        private ObservableCollection<ProductCardViewModel> productCardViewModels = new();

        private ProductCardViewModel? selectedProductCardViewModel;

        public ProductViewModel(UserInfoViewModel userInfo)
        {
            _userInfo = userInfo;
        }

        public ProductCardViewModel SelectedProductCardViewModel
        {
            get => selectedProductCardViewModel;
            set => SetProperty(ref selectedProductCardViewModel, value);
        }
  
        public ObservableCollection<ProductCardViewModel> ProductInfos
        {
            get => productCardViewModels;
            set => SetProperty(ref productCardViewModels, value);
        }
    }

    public class ProductCardViewModel : ViewModelBase
    {
        private KeyValuePair<ProductInfo, int> productInfo;

        public ProductCardViewModel(KeyValuePair<ProductInfo, int> productInfo)
        {
            ProductInfo = productInfo;
        }

        public KeyValuePair<ProductInfo, int> ProductInfo
        {
            get => productInfo;
            set => SetProperty(ref productInfo, value);
        }
    }
}
