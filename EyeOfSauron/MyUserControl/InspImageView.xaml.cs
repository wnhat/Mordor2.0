﻿using System.Linq;
using System.Windows;
using System.Windows.Controls;
using EyeOfSauron.ViewModel;
using CoreClass.Model;
using MongoDB.Driver;
using CoreClass.Service;

namespace EyeOfSauron.MyUserControl
{
    /// <summary>
    /// Interaction logic for InspWindows.xaml
    /// </summary>
    public partial class InspImageView : UserControl
    {
        public readonly MissionInfoViewModel _viewModel;

        public InspImageView()
        {
            _viewModel = new MissionInfoViewModel();
            DataContext = _viewModel;
            InitializeComponent();
        }

        //public void SetMission(Mission inspMission)
        //{
        //    _viewModel.ProductInfo = inspMission.productInfo;
        //    mission = inspMission;
        //    LoadOnInspPanelMission();
        //    mission.FillPreDownloadMissionQueue();
        //}
        
        //public async void LoadOnInspPanelMission()
        //{
        //    if (mission?.onInspPanelMission != null)
        //    {
        //        _viewModel.RemainingCount = await mission.RemainMissionCount();
        //        _viewModel.PanelId = mission.onInspPanelMission.inspectMission.PanelID;
        //        _viewModel.ProductInfo = new ProductInfoService().GetProductInfo(mission.onInspPanelMission.inspectMission.Info).Result;
        //        _viewModel.InspImage.resultImageDataList = mission.onInspPanelMission.resultImageDataList;
        //        _viewModel.InspImage.defectImageDataList = mission.onInspPanelMission.defectImageDataList;
        //        _viewModel.InspImage.DefectMapImage = mission.onInspPanelMission.ContoursImageContainer;
        //        _viewModel.DetailDefectList.AetDetailDefects.Clear();
        //        foreach (var item in mission.onInspPanelMission.bitmapImageContainers)
        //        {
        //            _viewModel.DetailDefectList.AetDetailDefects.Add(new AetDetailDefect(item.Name, item.Name, item.BitmapImage));
        //        }
        //        if (_viewModel.DetailDefectList.AetDetailDefects.Count != 0)
        //        {
        //            _viewModel.DetailDefectList.SelectedItem = _viewModel.DetailDefectList.AetDetailDefects.FirstOrDefault();
        //        }
        //        _viewModel.InspImage.refreshPage = 0;
        //        _viewModel.InspImage.RefreshImageMethod();
        //    }
        //}
        
        //public bool GetNextMission()
        //{
        //    mission.FillPreDownloadMissionQueue();
        //    if (mission.NextMission())
        //    {
        //        LoadOnInspPanelMission();
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}
        
        private void UserControl_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Input.Key.Enter:
                case System.Windows.Input.Key.Space:
                    e.Handled = true;
                    break;
                case System.Windows.Input.Key.LeftCtrl:
                    _viewModel.InspImage.ImageLableIsVisible = true;
                    break;
                default:
                    break;
            }
        }

        private void UserControl_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case System.Windows.Input.Key.LeftCtrl:
                    _viewModel.InspImage.ImageLableIsVisible = false;
                    break;
                default:
                    break;
            }
        }

        public void LoadOneInspImageView(PanelViewContainer panelViewContainer)
        {
            LoadOneInspImageView(panelViewContainer.PanelMission);
        }

        public void LoadOneInspImageView(PanelMission panelMission)
        {
            if (panelMission != null)
            {
                //Notice: AetResult.PanelID is null;
                _viewModel.PanelId = panelMission.AetResult.PanelId;
                _viewModel.InspImage.resultImageDataList = panelMission.resultImageDataList;
                _viewModel.InspImage.DefectMapImage = panelMission.ContoursImageContainer;
                _viewModel.DetailDefectList.AetDetailDefects.Clear();
                foreach (var item in panelMission.defectImageDataList)
                {
                    _viewModel.DetailDefectList.AetDetailDefects.Add(new AetDetailDefect(item.DefectInfo, item.BitmapImage));
                }
                if (_viewModel.DetailDefectList.AetDetailDefects.Count != 0)
                {
                    _viewModel.DetailDefectList.SelectedItem = _viewModel.DetailDefectList.AetDetailDefects.FirstOrDefault();
                }
                _viewModel.InspImage.refreshPage = 0;
                _viewModel.InspImage.RefreshImageMethod();
            }
        }
    }
}