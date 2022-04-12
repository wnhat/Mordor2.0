using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using EyeOfSauron.ViewModel;
using MongoDB.Driver;
using CoreClass;
using CoreClass.Model;
using System.IO;

namespace EyeOfSauron
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class InspWindow : Window
    {
        private Mission mission;
        private  MainWindowViewModel _viewModel;
        public InspWindow(UserInfoViewModel userInfo)
        {
            _viewModel = new MainWindowViewModel(userInfo);
            DataContext = _viewModel;
            InitializeComponent();
        }
        public void SetMission(Mission m)
        {
            mission = m;
        }
        public void SetImage()
        {
            _viewModel._inspImage.imageArray = mission.onInspPanelMission.resultImageDataDic.Values.ToArray().Skip(0).Take(3).ToArray();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SetImage();
        }
    }
}
