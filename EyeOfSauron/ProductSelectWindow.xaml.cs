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
        MissionManager missionManager;
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

            List<ProductInfo> productInfos;
            List<KeyValuePair<ProductInfo, int>> keyValuePairs = new();
            productInfos = collection.Find(filter).ToList();
            foreach (var productInfo in productInfos)
            {
                count += 100;
                keyValuePairs.Add(new(productInfo, count));
                _viewModel.ProductCardViewModels.Add(new ProductCardViewModel(new(productInfo, count)));
            }
            _viewModel.ProductInfos = keyValuePairs;
            _viewModel.SelectProductInfo = keyValuePairs.First();
        }
        private void WindowClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            TextBox1.Text = TextBox1.Text.Equals("1")?"2":"1";
        }
        private void ProductSelectBuuttonClick(object sender, RoutedEventArgs e)
        {
            Hide();
            Mission mission = new(_viewModel.SelectProductInfo.Key);
            InspWindow inspWindow = new(_viewModel._userInfo);
            inspWindow.SetMission(mission);
            inspWindow.ShowDialog();
            Show();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var collection = DBconnector.DICSDB.GetCollection<ProductInfo>("ProductInfo");
            var filter = Builders<ProductInfo>.Filter.Empty;
            List<ProductInfo> productInfos = new();
            productInfos = collection.Find(filter).ToList();
            count += 100;
            _viewModel.SelectProductInfo = new KeyValuePair<ProductInfo, int>(productInfos.ToArray()[1], count);
        }
    }
}
