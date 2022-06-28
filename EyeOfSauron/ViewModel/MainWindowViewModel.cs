using System.Windows.Controls;
using EyeOfSauron.MyUserControl;
using CoreClass.Model;
using System;
using System.Windows;
using MaterialDesignThemes.Wpf;
using System.Linq;
using CoreClass.Service;
using System.Windows.Threading;

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
                if (o is ProductInfo && o != null)
                {
                    SetInspView();
                    ProductInfo productInfo = o as ProductInfo;
                    try
                    {
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
            }
            else
            {
                await DialogHost.Show(new MessageAcceptDialog { Message = { Text = "请登录后操作" } }, "MainWindowDialog");
                LoginRequestEvent.Invoke(this, new RoutedEventArgs());
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