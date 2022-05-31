using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MongoDB.Driver;
using EyeOfSauron.ViewModel;
using CoreClass;
using CoreClass.Model;
using CoreClass.Service;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;

namespace EyeOfSauron.MyUserControl
{
    /// <summary>
    /// Interaction logic for ProductSelectWindow.xaml
    /// </summary>
    public partial class ProductSelectView : UserControl
    {
        public readonly ProductViewModel _viewModel;

        static readonly DICSRemainInspectMissionService RemainService = new();

        public ProductSelectView()
        {
            _viewModel = new();
            DataContext = _viewModel;
            GetMissions();
            InitializeComponent();
        }

        /// <summary>
        /// Get all missions to set viewmodel from database
        /// </summary>
        public async void GetMissions()
        {
            // get the remain mission count to set viewmodel
            _viewModel.ProductInfos.Clear();
            var remainMissionCount = await RemainService.GetRemainMissionCount();
            foreach (var item in remainMissionCount)
            {
                // convert the first BsonElement in the item to ProductInfo;
                var buffer = item.GetValue("_id").AsObjectId;
                var productInfo = new ProductInfoService().GetProductInfo(buffer).Result;
                int count = item.GetValue("count").ToInt32();
                _viewModel.ProductInfos.Add(new ProductCardViewModel(new(productInfo, count)));
            }
            if (_viewModel.ProductInfos.Count > 0)
            {
                _viewModel.SelectedProductCardViewModel = _viewModel.ProductInfos.First();
            }
        }

        private void ProductSelectBuuttonClick(object sender, RoutedEventArgs e)
        {
            SetSelectProductInfo(sender, e);
        }

        //for test, will be removed later;
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            GetMissions();
        }

        private void SetSelectProductInfo(object sender, RoutedEventArgs e)
        {
            ProductCardViewModel viewModel = ((Button)sender).DataContext as ProductCardViewModel;
            _viewModel.SelectedProductCardViewModel = viewModel;
        }
    }
}
