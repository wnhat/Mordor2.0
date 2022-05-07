using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Controls;
using EyeOfSauron.ViewModel;
using CoreClass.Model;
using System.Collections.Generic;
using System;

namespace EyeOfSauron
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class InspWindow : Window
    {
        private readonly Mission mission;

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
            mission.NextMission();
            LoadOnInspPanelMission();
        }

        public void LoadOnInspPanelMission()
        {
            if (mission.onInspPanelMission != null)
            {
                SetPanelInfo();
                _viewModel.MissionInfoViewModel.InspImage.refreshPage = 0;
                _viewModel.MissionInfoViewModel.InspImage.RefreshImageMethod();
            }
        }

        public void SetPanelInfo()
        {
            _viewModel.MissionInfoViewModel.PanelId = mission.onInspPanelMission.inspectMission.PanelID;
            if (mission.onInspPanelMission.resultImageDataList != null)
            {
                _viewModel.MissionInfoViewModel.InspImage.resultImageDataList = mission.onInspPanelMission.resultImageDataList;
            }
            else
            {
                _viewModel.MissionInfoViewModel.InspImage.resultImageDataList.Clear();
            }
            _viewModel.MissionInfoViewModel.InspImage.defectImageDataList = mission.onInspPanelMission.defectImageDataList;
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

        private new void KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
                switch (e.Key)
            {
                case System.Windows.Input.Key.Tab:
                    _viewModel.MissionInfoViewModel.InspImage.RefreshImageMethod();
                    break;
                default:
                    break;
            }
        }
    }
}
