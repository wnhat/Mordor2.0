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
using System.Windows.Media.Imaging;

namespace EyeOfSauron
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class InspWindow : Window
    {
        Mission mission;
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
            var colcetion = DBconnector.DICSDB.GetCollection<AETresult>("AETresult");
            var filter = Builders<AETresult>.Filter.Eq("ModelId", "606_AUTO_ET_L5");
            AETresult resultFile;
            resultFile = colcetion.Find(filter).FirstOrDefault();
            byte[] buffer = resultFile.ResultImages[0].Data;
            //Image image1 = Image.FromStream(ms);
            BitmapImage defaultImage = new BitmapImage();
            defaultImage.BeginInit();
            defaultImage.StreamSource = new MemoryStream(buffer);
            defaultImage.EndInit();
            _viewModel._inspImage.imageArray[0] = defaultImage;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            SetImage();
        }
    }
}
