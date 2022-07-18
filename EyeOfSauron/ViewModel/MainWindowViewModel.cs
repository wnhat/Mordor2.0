using System.Windows.Controls;
using EyeOfSauron.MyUserControl;
using CoreClass.Model;
using System;
using System.Windows;
using MaterialDesignThemes.Wpf;
using System.Linq;
using CoreClass.Service;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace EyeOfSauron.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public delegate void ValuePassHandler(object sender, RoutedEventArgs e);
        public event ValuePassHandler? LoginRequestEvent;
        public MainWindowViewModel(UserInfoViewModel userInfoViewModel):this()
        {
            UserInfo = userInfoViewModel;
        }
        
        public MainWindowViewModel()
        {
            MainContent = ProductSelectView;
            DefectJudgeView.DefectJudgeEvent += new DefectJudgeView.ValuePassHandler(DefectJudge);
            StartInspCommand = new CommandImplementation(StartInsp);
            EndInspCommand = new CommandImplementation(_ => EndInsp());
            _ = new DispatcherTimer(
                    TimeSpan.FromMilliseconds(1000),
                    DispatcherPriority.Normal,
                    new EventHandler((o, e) =>
                    {
                        DateTime = DateTime.Now;
                    }), Dispatcher.CurrentDispatcher);
        }
        private Mission? mission;
        public PanelMission? onInspPanelMission;
        private UserInfoViewModel userInfo = new();
        private ProductSelectView productSelectView = new ();
        private InspImageView inspImageView = new ();
        private DefectJudgeView defectJudgeView = new();
        private UserControl mainContent = new();
        private ColorTool colorTool = new();
        private DateTime dateTime = DateTime.Now;
        private ViewName onShowView = ViewName.ProductSelectView;
        public CommandImplementation StartInspCommand { get; }
        public CommandImplementation EndInspCommand { get; }

        public ViewName OnShowView
        {
            get => onShowView;
            set => SetProperty(ref onShowView, value);
        }

        public DateTime DateTime
        {
            get => dateTime;
            set => SetProperty(ref dateTime, value);
        }

        public DefectJudgeView DefectJudgeView
        {
            get => defectJudgeView;
            set => SetProperty(ref defectJudgeView, value);
        }

        public ColorTool? ColorTool
        {
            get => colorTool;
            set => SetProperty(ref colorTool, value);
        }

        public UserControl MainContent
        {
            get => mainContent;
            set => SetProperty(ref mainContent, value);
        }

        public ProductSelectView ProductSelectView
        {
            get => productSelectView;
            set => SetProperty(ref productSelectView, value);
        }

        public InspImageView InspImageView
        {
            get => inspImageView;
            set => SetProperty(ref inspImageView, value);
        }

        public UserInfoViewModel UserInfo
        {
            get => userInfo;
            set => SetProperty(ref userInfo, value);
        }

        //public DemoItem? ColorToolView
        //{
        //    get => colorToolView;
        //    set => SetProperty(ref colorToolView, value);
        //}

        public void SetInspView()
        {
            MainContent = InspImageView;
            OnShowView = ViewName.InspImageView;
        }

        public void SetProductSelectView()
        {
            ProductSelectView.GetMissions();
            MainContent = ProductSelectView;
            OnShowView = ViewName.ProductSelectView;
        }

        private async void StartInsp(object o)
        {
            if (UserInfo.UserExist)
            {
                if(o is ControlTableItem)
                {
                    ControlTableItem controlTableItem = (ControlTableItem)o;
                    switch (controlTableItem)
                    {
                        case ControlTableItem.ProductMission:
                            SetInspView();
                            ProductInfo? productInfo = productSelectView._viewModel.SelectedProductCardViewModel?.ProductInfo.Key;
                            if (productInfo != null)
                            {
                                try
                                {
                                    productInfo.InspPatternCount = 2;//This value should be set when the productInfo initializing
                                    SetInspImageLayout(productInfo.InspPatternCount);
                                    mission = new(productInfo);
                                    LoadOnInspPanelMission();
                                    mission.FillPreDownloadMissionQueue();
                                }
                                catch (MissionEmptyException ex)
                                {
                                    await DialogHost.Show(new MessageAcceptDialog { Message = { Text = ex.Message } }, "MainWindowDialog");
                                    SetProductSelectView();
                                }
                            }
                            break;
                        case ControlTableItem.ExamMission:
                            ExamMissionWIP? missionWIP = productSelectView._viewModel.SelectedExamMissionCardViewModel;
                            if(missionWIP != null)
                            {
                                List<ExamMissionResult> ExamMissionResultList = new ();
                                var a = PanelSample.GetSampleIds(missionWIP.MissionCollectionName).Result;
                                foreach(var id in a)
                                {
                                    ExamMissionResultList.Add(new(missionWIP, id.GetValue("_id").AsObjectId));
                                }
                                ExamMissionResult.AddMany(ExamMissionResultList);

                                SetInspView();
                                try
                                {
                                    SetInspImageLayout(3);
                                    mission = new(missionWIP, ControlTableItem.ExamMission);
                                    LoadOnInspPanelMission();
                                    mission.FillPreDownloadMissionQueue();
                                }
                                catch (MissionEmptyException ex)
                                {
                                    await DialogHost.Show(new MessageAcceptDialog { Message = { Text = ex.Message } }, "MainWindowDialog");
                                    SetProductSelectView();
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                await DialogHost.Show(new MessageAcceptDialog { Message = { Text = "请登录后操作" } }, "MainWindowDialog");
                LoginRequestEvent.Invoke(this, new RoutedEventArgs());
            }
        }

        private void SetInspImageLayout(int patternCount )
        {
            switch (patternCount)
            {
                case 1:
                case 3:
                    InspImageView._viewModel.LayoutPresets.SetInspImageLayout(patternCount);
                    InspImageView._viewModel.InspImage.pagePatternCount = patternCount;
                    InspImageView._viewModel.InspImage.InspImages.Clear();
                    for (int i = 0; i < patternCount; i++)
                    {
                        InspImageView._viewModel.InspImage.InspImages.Add(new BitmapImageContainer(ImageContainer.GetDefault));
                    }
                    Grid.SetRowSpan(inspImageView.imageGrid1, 2);
                    Grid.SetColumn(inspImageView.imageGrid4, 1);
                    Grid.SetColumnSpan(inspImageView.imageGrid4, 1);
                    break;
                case 2:
                    InspImageView._viewModel.LayoutPresets.SetInspImageLayout(patternCount);
                    InspImageView._viewModel.InspImage.pagePatternCount = patternCount;
                    InspImageView._viewModel.InspImage.InspImages.Clear();
                    for (int i = 0; i < patternCount; i++)
                    {
                        InspImageView._viewModel.InspImage.InspImages.Add(new BitmapImageContainer(ImageContainer.GetDefault));
                    }
                    Grid.SetRowSpan(inspImageView.imageGrid1, 1);
                    Grid.SetColumn(inspImageView.imageGrid4, 0);
                    Grid.SetColumnSpan(inspImageView.imageGrid4, 2);
                    break;
                default:
                    InspImageView._viewModel.LayoutPresets.SetInspImageLayout(3);
                    InspImageView._viewModel.InspImage.pagePatternCount = 3;
                    InspImageView._viewModel.InspImage.InspImages.Clear();
                    for (int i = 0; i < 3; i++)
                    {
                        InspImageView._viewModel.InspImage.InspImages.Add(new BitmapImageContainer(ImageContainer.GetDefault));
                    }
                    Grid.SetRowSpan(inspImageView.imageGrid1, 2);
                    Grid.SetColumn(inspImageView.imageGrid4, 1);
                    Grid.SetColumnSpan(inspImageView.imageGrid4, 1);
                    break;
            }
        }

        /// <summary>
        /// Load one PanelMission to set InspImageViewModel and refresh the view;
        /// </summary>
        public async void LoadOnInspPanelMission()
        {
            if (mission?.onInspPanelMission != null)
            {
                InspImageView._viewModel.RemainingCount = await mission.RemainMissionCount();
                InspImageView._viewModel.ProductInfo = new ProductInfoService().GetProductInfo(mission.onInspPanelMission.inspectMission.Info).Result;
                InspImageView.LoadOneInspImageView(mission.onInspPanelMission);
            }
        }

        /// <summary>
        /// Get next mission after a mission is finished;
        /// </summary>
        /// <returns> True if the mission have next PanelMission, false if the mission is empty;</returns>
        public bool GetNextMission()
        {
            if (mission == null) return false;
            mission.FillPreDownloadMissionQueue();
            if (mission.NextMission())
            {
                LoadOnInspPanelMission();
                return true;
            }
            else
            {
                return false;
            }
        }

        private async void EndInsp()
        {
            var result = await DialogHost.Show(new MessageAcceptCancelDialog { Message = { Text = "退出当前检查任务" } }, "MainWindowDialog");
            if (result is bool value)
            {
                if (value)
                {
                    SetProductSelectView();
                }
            }
        }

        private void DefectJudge(object sender, DefectJudgeArgs e)
        {
            Defect defect = e.Defect;
            bool IsServerConnected = SeverConnector.SendPanelMissionResult(new OperatorJudge(defect, UserInfo.User.Username, UserInfo.User.Account, UserInfo.User.Id, 1), mission.onInspPanelMission.inspectMission);
            //Server offline;
            if (!IsServerConnected)
            {
                if (!GetNextMission())
                {
                    DialogHost.Show(new MessageAcceptDialog { Message = { Text = "There is no mission left" } }, "MainWindowDialog");
                }
            }
            else
            {
                DialogHost.Show(new ProgressMessageDialog(), "MainWindowDialog");
            }
        }
    }
    public enum ViewName
    {
        Null,
        InspImageView,
        ProductSelectView
    }
}