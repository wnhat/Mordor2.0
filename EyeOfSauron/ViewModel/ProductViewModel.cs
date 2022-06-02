using System.Collections.Generic;
using System.Collections.ObjectModel;
using CoreClass.Model;

namespace EyeOfSauron.ViewModel
{

    public class ProductViewModel : ViewModelBase
    {
        public UserInfoViewModel userInfoViewModel = new();

        private ObservableCollection<ProductCardViewModel> productCardViewModels = new();

        private ProductCardViewModel? selectedProductCardViewModel;

        public ProductViewModel(UserInfoViewModel userInfo)
        {
            this.userInfoViewModel = userInfo;
        }
        
        public ProductViewModel() { }
        
        public UserInfoViewModel UserInfoViewModel
        {
            get => this.userInfoViewModel;
            set
            {
                this.userInfoViewModel = value;
                this.OnPropertyChanged();
            }
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
