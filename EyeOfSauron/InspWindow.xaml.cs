using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using EyeOfSauron.ViewModel;
using CoreClass.Model;

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
            LoadOnInspPanelMission();
            InitializeComponent();
        }

        //for test, will be removed later;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            RefreshInspImage();
            _viewModel._defectList.DetailDefectImage = mission.onInspPanelMission.defectImageDataDic.FirstOrDefault().Value;
        }

        public void LoadOnInspPanelMission()
        {
            if (mission.onInspPanelMission != null)
            {
                refreshPage = 0;
                RefreshInspImage();
                //temp method
                _viewModel._defectList.DetailDefectImage = mission.onInspPanelMission.defectImageDataDic.FirstOrDefault().Value;
            }
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

        private void JudgeButtonClick(object sender, RoutedEventArgs e)
        {
            Defect defect = (sender as Button).DataContext as Defect;
            SeverConnector.SendPanelMissionResult(new OperatorJudge(defect, _viewModel._userInfo.User.Username, _viewModel._userInfo.User.Account, _viewModel._userInfo.User.Id, 1), mission.onInspPanelMission.inspectMission);
            //Should be called after OPJudge action, temporarily call here for test;
            mission.FillPreDownloadMissionQueue();
            if (!mission.NextMission())
            {
                MessageBox.Show("There is no mission left.");
                this.Close();
            }
            else
            {
                LoadOnInspPanelMission();
            }
        }
    }
}
