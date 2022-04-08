﻿using System;
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
        public ProductSelectWindow(UserInfoViewModel userInfo)
        {
            _viewModel = new ProductViewModel(userInfo);
            GetMissions();
            InitializeComponent();
        }

        private void GetMissions()
        {
            var colcetion = DBconnector.DICSDB.GetCollection<ProductInfo>("ProductInfo");
            var filter = Builders<ProductInfo>.Filter.Eq("Name", "D2");
            _viewModel.productInfo = colcetion.Find(filter).FirstOrDefault();
            missionManager = new MissionManager();  
        }
        private void WindowClose(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            TextBox1.Text = TextBox1.Text.Equals("1")?"2":"1";
        }

        private void WindowMinSize(object sender, RoutedEventArgs e)
        { 
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            InspWindow inspWindow = new InspWindow(_viewModel._userInfo);
            inspWindow.ShowDialog();
            Show();
        }
    }
}
