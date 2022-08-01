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
using System.Net;

namespace EyeOfSauron.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public delegate void ValuePassHandler(object sender, RoutedEventArgs e);
        public event ValuePassHandler? LoginRequestEvent;
        public event RoutedEventHandler? MissionFinishedEvent; 
        public MainWindowViewModel(UserInfoViewModel userInfoViewModel):this()
        {
            UserInfo = userInfoViewModel;
        }
        public MainWindowViewModel()
        {
            IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress[] addr = ipEntry.AddressList;
            Defect.RefreshDefectList();
            foreach (IPAddress address in addr)
            {
                var a = address;
                var b = a.ToString();
                Eqp = DicsEqp.GetByIp(b);
                if (Eqp != null)
                {
                    break;
                }
            }
            MainContent = ProductSelectView;
            DefectJudgeView.DefectJudgedEvent += new RoutedEventHandler(DefectJudge);
            StartInspCommand = new CommandImplementation(StartInsp);
            EndInspCommand = new CommandImplementation(_ => EndInsp(), _=> productSelectView._viewModel.ControlTabSelectedIndex != ControlTableItem.ExamMission);
            _ = new DispatcherTimer(
                    TimeSpan.FromMilliseconds(1000),
                    DispatcherPriority.Normal,
                    new EventHandler((o, e) =>
                    {
                        DateTime = DateTime.Now;
                    }), 
                    Dispatcher.CurrentDispatcher);
            inspImageView._viewModel.ExtendedUserControl = informationView;
        }
        public DicsEqp? Eqp = null;
        private Mission? mission;
        public PanelMission? onInspPanelMission;
        private UserInfoViewModel userInfo = new();
        private ProductSelectView productSelectView = new ();
        private InspImageView inspImageView = new ();
        private InformationView informationView = new();
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
                if (o is ControlTableItem controlTableItem)
                {
                    switch (controlTableItem)
                    {
                        case ControlTableItem.ProductMission:
                            SetInspView();
                            ProductInfo? productInfo = productSelectView._viewModel.SelectedProductCardViewModel?.ProductInfo.Key;
                            if (productInfo != null)
                            {
                                try
                                {
                                    productInfo.InspPatternCount = 3;//This value should be set when the productInfo initializing
                                    SetInspImageLayout(productInfo.InspPatternCount);
                                    mission = new(productInfo);
                                    LoadOnInspPanelMission();
                                    informationView.StartTick();
                                    mission.FillPreDownloadMissionQueue();
                                }
                                catch (MissionEmptyException ex)
                                {
                                    await DialogHost.Show(new MessageAcceptDialog { Message = { Text = ex.Message } }, "MainWindowDialog");
                                    SetProductSelectView();
                                }
                            }
                            else
                            {
                                await DialogHost.Show(new MessageAcceptDialog { Message = { Text = "未选择有效型号" } }, "MainWindowDialog");
                            }
                            break;
                        case ControlTableItem.ExamMission:
                            ExamMissionCollection? examMission = productSelectView._viewModel.SelectedExamMissionCardViewModel;
                            if (examMission != null)
                            {
                                var dialogResult =  await DialogHost.Show(new MessageAcceptCancelDialog { Message = { Text = "即将开始考试，考试过程中断将无法继续" } }, "MainWindowDialog");
                                if(dialogResult is bool result)
                                {
                                    if (result)
                                    {
                                        List<ExamMissionResult> ExamMissionResultList = new();
                                        var sampleIds = PanelSample.GetSampleIds(examMission.MissionCollectionName);
                                        foreach (var id in sampleIds)
                                        {
                                            ExamMissionResultList.Add(new(examMission, id.GetValue("_id").AsObjectId));
                                        }
                                        await ExamMissionResult.AddMany(ExamMissionResultList);
                                        SetInspView();
                                        try
                                        {
                                            SetInspImageLayout(3);
                                            mission = new(examMission, ControlTableItem.ExamMission);
                                            informationView.StartTick();
                                            LoadOnInspPanelMission();
                                            mission.FillPreDownloadMissionQueue();
                                        }
                                        catch (MissionEmptyException ex)
                                        {
                                            await DialogHost.Show(new MessageAcceptDialog { Message = { Text = ex.Message } }, "MainWindowDialog");
                                            ExamMissionCollection.FinishOne(examMission.Id);
                                            SetProductSelectView();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                await DialogHost.Show(new MessageAcceptDialog { Message = { Text = "未选择有效考试" } }, "MainWindowDialog");
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
                LoginRequestEvent?.Invoke(this, new RoutedEventArgs());
            }
        }

        private void SetInspImageLayout(int patternCount )
        {
            patternCount = patternCount > 3 & patternCount <1 ? 3 : patternCount;
            switch (patternCount)
            {
                case 1:
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
                case 3:
                    InspImageView._viewModel.LayoutPresets.SetInspImageLayout(patternCount);
                    InspImageView._viewModel.InspImage.pagePatternCount = patternCount;
                    InspImageView._viewModel.InspImage.InspImages.Clear();
                    for (int i = 0; i < patternCount; i++)
                    {
                        InspImageView._viewModel.InspImage.InspImages.Add(new BitmapImageContainer(ImageContainer.GetDefault));
                    }
                    Grid.SetRowSpan(inspImageView.imageGrid1, 1);
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
                //InspImageView._viewModel.ProductInfo = new ProductInfoService().GetProductInfo(mission.onInspPanelMission.inspectMission.Info).Result;
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

        private async void DefectJudge(object sender,RoutedEventArgs e)
        {
            Defect defect = (Defect)sender;
            bool IsServerConnected;
            var tactTime = informationView.viewModel.TactTimeFullPrecision;
            informationView.viewModel.TactTime = 0;
            informationView.viewModel.TotalTactTimeSpan += informationView.viewModel.TactTimeSpan;
            informationView.viewModel.InspCount += 1;
            informationView.viewModel.tactStartTime = DateTime.Now;
            switch (mission.MissionType)
            {
                default:
                case ControlTableItem.ProductMission:
                    IsServerConnected = SeverConnector.SendPanelMissionResult(new OperatorJudge(defect, UserInfo.User.Username, UserInfo.User.Account, UserInfo.User.Id, 1), mission.onInspPanelMission.inspectMission);
                    InspectMissionResult.InsertOne(new(mission.onInspPanelMission.inspectMission.ID, UserInfo.User.Id,Eqp,defect,tactTime));
                    IsServerConnected = true;
                    break;
                case ControlTableItem.ExamMission:
                    mission.onInspPanelMission.examMission?.SetResult(defect, Eqp , tactTime);
                    List<KeyValuePair<string, object>> properties = new();
                    properties.Add(new KeyValuePair<string, object>("ResultDefect", defect));
                    properties.Add(new KeyValuePair<string, object>("IsChecked", true));
                    properties.Add(new KeyValuePair<string, object>("Eqp", Eqp));
                    properties.Add(new KeyValuePair<string, object>("TactTime", tactTime));
                    properties.Add(new KeyValuePair<string, object>("IsCorrect", mission.onInspPanelMission.examMission.IsCorrect));
                    ExamMissionResult.UpdateProperties(mission.onInspPanelMission.examMission.Id, properties);
                    IsServerConnected = true;
                    break;
            }
            //Server offline;
            if (IsServerConnected)
            {
                if (!GetNextMission())
                {
                    await DialogHost.Show(new MessageAcceptDialog { Message = { Text = "There is no mission left" } }, "MainWindowDialog");
                    switch (mission.MissionType)
                    {
                        default: break;
                        case ControlTableItem.ExamMission:
                            ExamMissionCollection.FinishOne(mission.ExamMissionWIP.Id);
                            sender = mission.ExamMissionWIP.Id;
                            break;
                    }
                    MissionFinishedEvent?.Invoke(sender, new());
                }
            }
            else
            {
                await DialogHost.Show(new ProgressMessageDialog(), "MainWindowDialog");
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