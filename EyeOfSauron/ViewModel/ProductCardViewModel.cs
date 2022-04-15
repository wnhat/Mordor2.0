using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreClass.Model;

namespace EyeOfSauron.ViewModel
{
    public class ProductCardViewModel : ViewModelBase
    {
        private KeyValuePair<ProductInfo, int> _productInfo;
        public ProductCardViewModel(KeyValuePair<ProductInfo, int> productInfo)
        {
            ProductInfo = productInfo;
        }
        public KeyValuePair<ProductInfo, int> ProductInfo
        {
            get => ProductInfo;
            set => SetProperty(ref _productInfo, value);
            
        }
    }
}
