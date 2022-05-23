﻿using System;
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
    public partial class ProductSelectWindow : UserControl
    {
        int count = 0;

        public readonly ProductViewModel _viewModel;

        static readonly DICSRemainInspectMissionService _RemainService = new();

        public ProductSelectWindow(UserInfoViewModel userInfo)
        {
            _viewModel = new ProductViewModel(userInfo);
            DataContext = _viewModel;
            GetMissions();
            InitializeComponent();
        }

        private async void GetMissions()
        {
            // get the remain mission count to set viewmodel
            _viewModel.ProductInfos.Clear();
            var remainMissionCount = await _RemainService.GetRemainMissionCount();
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
            //Hide();
            try
            {
                Mission mission = new(_viewModel.SelectedProductCardViewModel.ProductInfo.Key);
                //InspWindow inspWindow = new(_viewModel._userInfo, mission);
                ///inspWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                GetMissions();
                //ShowDialog();
            }
        }

        //for test, will be removed later;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var collection = DBconnector.DICSDB.GetCollection<ProductInfo>("ProductInfo");
            var filter = Builders<ProductInfo>.Filter.Empty;
            List<ProductInfo> productInfos = collection.Find(filter).ToList();
            count += 100;
            //_viewModel.SelectProductInfo = new KeyValuePair<ProductInfo, int>(productInfos.ToArray()[1], count);
            _viewModel.ProductInfos.Add(new ProductCardViewModel(new(productInfos.ToArray()[1], count)));
            Window window = new MainWindow(new UserInfoViewModel());
            window.ShowDialog();
        }

        private void SetSelectProductInfo(object sender, RoutedEventArgs e)
        {
            ProductCardViewModel viewModel = ((Button)sender).DataContext as ProductCardViewModel;
            _viewModel.SelectedProductCardViewModel = viewModel;
        }
    }
}