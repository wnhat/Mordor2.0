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
        public ProductInfo productInfo { get; set; }
        public ProductViewModel(UserInfoViewModel userInfo)
        {
            productInfo = new ProductInfo();
            _userInfo = userInfo;
        }
    }
}
