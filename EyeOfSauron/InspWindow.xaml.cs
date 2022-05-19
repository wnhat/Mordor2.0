﻿using System.Linq;
using System.Windows;
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
        private readonly Mission mission;

        private readonly InspMainWindowViewModel _viewModel;
        
        public InspWindow(UserInfoViewModel userInfo, Mission inspMission)
        {
            _viewModel = new InspMainWindowViewModel(userInfo);
            _viewModel.MissionInfoViewModel.ProductInfo = inspMission.productInfo;
            DataContext = _viewModel;
            mission = inspMission;
            LoadOnInspPanelMission();
            mission.FillPreDownloadMissionQueue();
            InitializeComponent();
        }

        //for test, will be removed later;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //mission.NextMission();
            //LoadOnInspPanelMission();
        }

        public async void LoadOnInspPanelMission()
        {
            if (mission.onInspPanelMission != null)
            {
                _viewModel.MissionInfoViewModel.RemainingCount = await mission.RemainMissionCount();
                _viewModel.MissionInfoViewModel.PanelId = mission.onInspPanelMission.inspectMission.PanelID;
                _viewModel.MissionInfoViewModel.ProductInfo = mission.onInspPanelMission.inspectMission.Info;
                _viewModel.MissionInfoViewModel.InspImage.resultImageDataList = mission.onInspPanelMission.resultImageDataList;
                _viewModel.MissionInfoViewModel.InspImage.defectImageDataList = mission.onInspPanelMission.defectImageDataList;
                _viewModel.MissionInfoViewModel.DetailDefectList.AetDetailDefects.Clear();
                foreach (var item in mission.onInspPanelMission.bitmapImageContainers)
                {
                    _viewModel.MissionInfoViewModel.DetailDefectList.AetDetailDefects.Add(new AetDetailDefect(item.Name, item.Name, item.BitmapImage));
                }
                if (_viewModel.MissionInfoViewModel.DetailDefectList.AetDetailDefects.Count != 0)
                {
                    _viewModel.MissionInfoViewModel.DetailDefectList.SelectedItem = _viewModel.MissionInfoViewModel.DetailDefectList.AetDetailDefects.FirstOrDefault();
                }
                _viewModel.MissionInfoViewModel.InspImage.refreshPage = 0;
                _viewModel.MissionInfoViewModel.InspImage.RefreshImageMethod();
            }
        }

        private void JudgeButtonClick(object sender, RoutedEventArgs e)
        {
            Defect defect;
            if ((sender as Button).Content == "S")
            {
                defect = null;
            }
            else if((sender as Button).Content == "E")
            {
                defect = Defect.OperaterEjudge;
            }
            else
            {
                defect = (sender as Button).DataContext as Defect;
            }
            SeverConnector.SendPanelMissionResult(new OperatorJudge(defect, _viewModel.UserInfo.User.Username, _viewModel.UserInfo.User.Account, _viewModel.UserInfo.User.Id, 1), mission.onInspPanelMission.inspectMission);
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
                case System.Windows.Input.Key.Enter:
                case System.Windows.Input.Key.Space:
                    e.Handled = true;
                    break;
                case System.Windows.Input.Key.Tab:
                    _viewModel.MissionInfoViewModel.InspImage.RefreshImageMethod();
                    break;
                case System.Windows.Input.Key.LeftCtrl:
                    _viewModel.MissionInfoViewModel.InspImage.IsVisible = true;
                    break;
                default:
                    break;
            }
        }

        private new void KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Input.Key.LeftCtrl:
                    _viewModel.MissionInfoViewModel.InspImage.IsVisible = false;
                    break;
                default:
                    break;
            }
        }

        private void PanelIDLableDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string text = (sender as Label).Content.ToString();
            Clipboard.SetDataObject(text);
            e.Handled = true;
        }
    }
}
