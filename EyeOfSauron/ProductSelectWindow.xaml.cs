using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MongoDB.Driver;
using EyeOfSauron.ViewModel;
using CoreClass;
using CoreClass.Model;

namespace EyeOfSauron
{
    /// <summary>
    /// Interaction logic for ProductSelectWindow.xaml
    /// </summary>
    public partial class ProductSelectWindow : Window
    {
        public readonly ProductViewModel _viewModel;
        int count = 100;
        public ProductSelectWindow(UserInfoViewModel userInfo)
        {
            _viewModel = new ProductViewModel(userInfo);
            DataContext = _viewModel;
            GetMissions();
            InitializeComponent();
        }

        private void GetMissions()
        {
            var collection = DBconnector.DICSDB.GetCollection<ProductInfo>("ProductInfo");
            var filter = Builders<ProductInfo>.Filter.Empty;
            List<ProductInfo> productInfos = collection.Find(filter).ToList();
            foreach (var productInfo in productInfos)
            {
                count += 200;
                _viewModel.ProductInfos.Add(new ProductCardViewModel(new(productInfo, count)));
            }
            //for test, will be removed later
            _viewModel.SelectedProductCardViewModel = _viewModel.ProductInfos[0];
        }

        private void ProductSelectBuuttonClick(object sender, RoutedEventArgs e)
        {
            SetSelectProductInfo(sender, e);
            Hide();
            Mission mission = new(_viewModel.SelectedProductCardViewModel.ProductInfo.Key);
            InspWindow inspWindow = new(_viewModel._userInfo);
            inspWindow.SetMission(mission);
            inspWindow.ShowDialog();
            Show();
        }

        //for test, will be removed later;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var collection = DBconnector.DICSDB.GetCollection<ProductInfo>("ProductInfo");
            var filter = Builders<ProductInfo>.Filter.Empty;
            List<ProductInfo> productInfos = new();
            productInfos = collection.Find(filter).ToList();
            count += 100;
            //_viewModel.SelectProductInfo = new KeyValuePair<ProductInfo, int>(productInfos.ToArray()[1], count);
            _viewModel.ProductInfos.Add(new ProductCardViewModel(new(productInfos.ToArray()[1], count)));
        }

        private void SetSelectProductInfo(object sender, RoutedEventArgs e)
        {
            ProductCardViewModel viewModel = (sender as Button).DataContext as ProductCardViewModel;
            _viewModel.SelectProductInfo = viewModel.ProductInfo;
        }
    }
}
