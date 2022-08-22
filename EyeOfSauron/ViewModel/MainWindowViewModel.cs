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
                Eqp = DicsEqp.GetByIp(address.ToString());
                if (Eqp != null)
                {
                    break;
                }
            }
            MainContent = ProductSelectView;
            DefectJudgeView.DefectJudgedEvent += new RoutedEventHandler(DefectJudge);
            Information.CountDownFinishEvent += new EventHandler(ExamCountDownFinish);
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
        }
        public DicsEqp? Eqp = null;
        private Mission? mission;
        //public PanelMission? onInspPanelMission;
        private UserInfoViewModel userInfo = new();
        private ProductSelectView productSelectView = new ();
        private InspImageView inspImageView = new ();
        private DefectJudgeView defectJudgeView = new();
        private UserControl mainContent = new();
        private ColorTool colorTool = new();
        private DateTime dateTime = DateTime.Now;
        private ViewName onShowView = ViewName.ProductSelectView;
        public InformationViewModel Information { get; } = new();
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
                            ProductInfo? productInfo = productSelectView._viewModel.SelectedProductCardViewModel?.ProductInfo.Key;
                            if (productInfo != null)
                            {
                                //切换至检查界面
                                SetInspView();
                                try
                                {
                                    productInfo.InspPatternCount = PatternCount.Three;//This value should be set when the productInfo initializing
                                    SetInspImageLayout((int)productInfo.InspPatternCount);
                                    mission = new(productInfo);
                                    LoadOnInspPanelMission();
                                    Information.StartTick();
                                    mission.FillPreDownloadMissionQueue();
                                }
                                catch (MissionEmptyException ex)
                                {
                                    await DialogHost.Show(new MessageAcceptDialog { Message = { Text = ex.Message } }, "MainWindowDialog");
                                    Information.TicktStopAndReset();
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
                                        //添加检查任务至数据库
                                        List<ExamMissionResult> ExamMissionResultList = new();
                                        var sampleIds = PanelSample.GetSampleIds(examMission.MissionCollectionName);
                                        foreach (var id in sampleIds)
                                        {
                                            ExamMissionResultList.Add(new(examMission, id.GetValue("_id").AsObjectId));
                                        }
                                        await ExamMissionResult.AddMany(ExamMissionResultList);
                                        //切换至检查界面
                                        SetInspView();
                                        try
                                        {
                                            SetInspImageLayout(3);
                                            mission = new(examMission, ControlTableItem.ExamMission);
                                            //设置考试任务时间限制
                                            Information.MissionTimeLimit = examMission.MissionTimeLimit;
                                            LoadOnInspPanelMission();
                                            
                                            Information.StartTick();
                                            mission.FillPreDownloadMissionQueue();
                                        }
                                        catch (MissionEmptyException ex)
                                        {
                                            await DialogHost.Show(new MessageAcceptDialog { Message = { Text = ex.Message } }, "MainWindowDialog");
                                            ExamMissionCollection.FinishOneMission(examMission.Id);
                                            Information.TicktStopAndReset();
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
                    Information.TicktStopAndReset();
                    SetProductSelectView();
                }
            }
        }

        private async void DefectJudge(object sender,RoutedEventArgs e)
        {
            Defect defect = (Defect)sender;
            bool IsServerConnected;
            var tactTime = Information.TactTimeFullPrecision;
            Information.TotalTactTimeSpan += Information.TactTimeSpan;
            Information.TactTimeSpan = TimeSpan.Zero;
            Information.InspCount += 1;
            Information.tactStartTime = DateTime.Now;
            switch (mission.MissionType)
            {
                default:
                case ControlTableItem.ProductMission:
                    IsServerConnected = SeverConnector.SendPanelMissionResult(new OperatorJudge(defect, UserInfo.User.Username, UserInfo.User.Account, UserInfo.User.Id, 1), mission.onInspPanelMission.inspectMission);
                    InspectMissionResult.InsertOne(new(mission.onInspPanelMission.inspectMission.ID, UserInfo.User.Id,Eqp,defect,tactTime));
                    break;
                case ControlTableItem.ExamMission:
                    mission.onInspPanelMission.examMission?.SetResult(defect, Eqp , tactTime); 
                    JudgeOnInspExamMission();
                    IsServerConnected = true;
                    break;
            }
            //Server offline;
            if (IsServerConnected)
            {
                if (!GetNextMission())
                {
                    object eventSender = new();
                    await DialogHost.Show(new MessageAcceptDialog { Message = { Text = "There is no mission left" } }, "MainWindowDialog");
                    Information.TicktStopAndReset();
                    switch (mission.MissionType)
                    {
                        default: break;
                        case ControlTableItem.ExamMission:
                            ExamMissionCollection.FinishOneMission(mission.ExamMissionWIP.Id);
                            eventSender = mission.ExamMissionWIP.Id;
                            break;
                    }
                    MissionFinishedEvent?.Invoke(eventSender, new());
                }
            }
            else
            {
                await DialogHost.Show(new ProgressMessageDialog(), "MainWindowDialog");
            }
        }
        
        private async void ExamCountDownFinish(object? sender, EventArgs e)
        {
            //Finish remaining exam missions;
            mission.onInspPanelMission.examMission?.SetResult(Defect.GetDefectByName("OperaterEjudge"), Eqp, 0);
            JudgeOnInspExamMission();
            while (mission.NextMission())
            {
                mission.onInspPanelMission.examMission?.SetResult(Defect.GetDefectByName("OperaterEjudge"), Eqp, 0);
                JudgeOnInspExamMission();
            }
            ExamMissionCollection.FinishOneMission(mission.ExamMissionWIP.Id);
            Information.TicktStopAndReset();
            await DialogHost.Show(new MessageAcceptDialog { Message = { Text = "考试时间结束" } }, "MainWindowDialog");
            MissionFinishedEvent?.Invoke(mission.ExamMissionWIP.Id, new());
        }
        private void JudgeOnInspExamMission()
        {
            var a = mission.onInspPanelMission.examMission;
            if (a != null)
            {
                List<KeyValuePair<string, object>> properties = new();
                properties.Add(new KeyValuePair<string, object>("ResultDefect", a.ResultDefect));
                properties.Add(new KeyValuePair<string, object>("IsChecked", true));
                properties.Add(new KeyValuePair<string, object>("Eqp", a.Eqp));
                properties.Add(new KeyValuePair<string, object>("TactTime", a.TactTime));
                properties.Add(new KeyValuePair<string, object>("IsCorrect", a.IsCorrect));
                ExamMissionResult.UpdateProperties(mission.onInspPanelMission.examMission.Id, properties);
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