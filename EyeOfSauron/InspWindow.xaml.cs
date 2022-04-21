using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using EyeOfSauron.ViewModel;

namespace EyeOfSauron
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class InspWindow : Window
    {
        private int refreshPage = 0;
        private Mission mission;
        private MainWindowViewModel _viewModel;
        public InspWindow(UserInfoViewModel userInfo, Mission inspMission)
        {
            _viewModel = new MainWindowViewModel(userInfo);
            DataContext = _viewModel;
            mission = inspMission;
            InitializeComponent();
        }

        public void SetMission(Mission m)
        {
            mission = m;
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RefreshInspImage();
            _viewModel._defectList.DetailDefectImage = mission.onInspPanelMission.defectImageDataDic.FirstOrDefault().Value;
        }

        public void RefreshInspImage()
        {
            if ((refreshPage) * 3 < mission.onInspPanelMission.resultImageDataDic.Values.ToArray().Length)
            {
                //_viewModel._inspImage.imageArray = mission.onInspPanelMission.resultImageDataDic.Values.ToArray().Skip((refreshPage) * 3).Take(3).ToArray();
                //_viewModel._inspImage.imageNameArray = mission.onInspPanelMission.resultImageDataDic.Keys.ToArray().Skip((refreshPage) * 3).Take(3).ToArray();
                SetInspImage(mission.onInspPanelMission.resultImageDataDic.Values.ToArray().Skip((refreshPage) * 3).Take(3).ToArray(), mission.onInspPanelMission.resultImageDataDic.Keys.ToArray().Skip((refreshPage) * 3).Take(3).ToArray());
                refreshPage++;
            }
            else
            {
                refreshPage = 0;
                RefreshInspImage();
            }
        }

        public void SetInspImage(BitmapImage[] bitmapImages, string[] imageNames)
        {
            _viewModel._inspImage.imageArray = bitmapImages;
            _viewModel._inspImage.imageNameArray = imageNames;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
