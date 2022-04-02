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

namespace EyeOfSauron
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class InspWindow : Window
    {
        int flag = 1;
        Mission mission;
        private readonly TreesViewModel _viewModel;
        
        public InspWindow()
        {
            _viewModel = new TreesViewModel();
            DataContext = _viewModel;
            InitializeComponent();
        }
        public void SetMission(Mission m)
        {
            mission = m;
        }
        public void SetImage()
        {
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.UriSource = new Uri(@"D:\DICS Software\DefaultSample\AVI\Orign\DefaultSample\00_DUST_CAM00.bmp", UriKind.Absolute);
            image.EndInit();
            ImageBox1.Source = image;
            ImageBox2.Source = image;
            ImageBox3.Source = image;
            //WpfAnimatedGif.ImageBehavior.SetAnimatedSource(ImageBox2, image);
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //ImageViewBox.RenderSize = new System.Windows.Size(3440, 2440);
            //ImageViewBox.RenderSize = flag == 1 ? new System.Windows.Size(1740, 1240) : new System.Windows.Size(3440, 2440);
            flag = flag == 1 ? 0 : 1;
            _viewModel.Defects.Add(new("Defect3","Saction3"));
            SetImage();
        }
    }
}
