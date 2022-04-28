using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using EyeOfSauron.ViewModel;
using CoreClass.Model;
using System.Collections.Generic;

namespace EyeOfSauron
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class InspWindow : Window
    {
        private int refreshPage = 0;
        
        private Mission mission;

        private readonly MainWindowViewModel _viewModel;
        
        public InspWindow(UserInfoViewModel userInfo, Mission inspMission)
        {
            _viewModel = new MainWindowViewModel(userInfo);
            DataContext = _viewModel;
            mission = inspMission;
            LoadOnInspPanelMission();
            mission.FillPreDownloadMissionQueue();
            InitializeComponent();
        }

        //for test, will be removed later;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            //RefreshInspImage();
            //_viewModel.MissionInfoViewModel.DetailDefectList.SelectedItem.DetailDefectImage = mission.onInspPanelMission.defectImageDataDic.FirstOrDefault().Value;
        }

        public void LoadOnInspPanelMission()
        {
            if (mission.onInspPanelMission != null)
            {
                SetPanelInfo();
                refreshPage = 0;
                RefreshInspImage();
                //temp method
                _viewModel.MissionInfoViewModel.DetailDefectList.SelectedItem.DetailDefectImage = mission.onInspPanelMission.defectImageDataDic.FirstOrDefault().Value;
                
            }
        }
        
        public void RefreshInspImage()
        {
            if ((refreshPage) * 3 < mission.onInspPanelMission.resultImageDataDic.Values.ToArray().Length)
            {
                //SetInspImage(mission.onInspPanelMission.resultImageDataDic.Values.ToArray().Skip((refreshPage) * 3).Take(3).ToArray(), mission.onInspPanelMission.resultImageDataDic.Keys.ToArray().Skip((refreshPage) * 3).Take(3).ToArray());
                SetInspImage(mission.onInspPanelMission.resultImageDataDic, mission.onInspPanelMission.resultImageDataDic);
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
            //_viewModel.MissionInfoViewModel.InspImage.ImageArray = bitmapImages;
            //_viewModel.MissionInfoViewModel.InspImage.ImageNameArray = imageNames;
        }

        public void SetPanelInfo()
        {
            _viewModel.MissionInfoViewModel.PanelId = mission.onInspPanelMission.inspectMission.PanelID;
            SetInspImage(mission.onInspPanelMission.resultImageDataDic, mission.onInspPanelMission.resultImageDataDic);
        }
        
        public void SetInspImage(Dictionary<string, BitmapImage> resultImageDataDic, Dictionary<string, BitmapImage> defectImageDataDic)
        {
            _viewModel.MissionInfoViewModel.InspImage.resultImageDataDic = resultImageDataDic;
            _viewModel.MissionInfoViewModel.InspImage.defectImageDataDic = defectImageDataDic;
            Application.Current.Dispatcher.Invoke(() =>
            {
                _viewModel.MissionInfoViewModel.InspImage.InspImageArray = resultImageDataDic.ToList().ToArray().Take(3).ToArray();
            });
        }

        private void JudgeButtonClick(object sender, RoutedEventArgs e)
        {
            Defect defect = (sender as Button).DataContext as Defect;
            SeverConnector.SendPanelMissionResult(new OperatorJudge(defect, _viewModel.UserInfo.User.Username, _viewModel.UserInfo.User.Account, _viewModel.UserInfo.User.Id, 1), mission.onInspPanelMission.inspectMission);
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
