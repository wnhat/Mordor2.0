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

        private ProductCardViewModel? _selectedProductCardViewModel;

        private KeyValuePair<ProductInfo, int> _selectProductInfo;

        public ProductViewModel(UserInfoViewModel userInfo)
        {
            _userInfo = userInfo;
        }

        public ProductCardViewModel SelectedProductCardViewModel
        {
#pragma warning disable CS8603 // Possible null reference return.
            get => _selectedProductCardViewModel;
#pragma warning restore CS8603 // Possible null reference return.
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
