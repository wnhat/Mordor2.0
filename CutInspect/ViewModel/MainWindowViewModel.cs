using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CoreClass.Model;
using CutInspect.Model;
using CutInspect.MyUserControl;
using MaterialDesignThemes.Wpf;

namespace CutInspect.ViewModel
{
    public class MainWindowViewModel:ViewModelBase
    {
        private object finishLock = new();
        private DateTime dateTime;
        private int moveRectWidth = 100;
        private ColorTool colorTool = new();
        private DateTimePickerViewModel dateTimePicker = new();
        private ObservableCollection<EqpMissionViewModel> eqpMissionViewModels = new();
        private ObservableCollection<PanelMission> finishedPanelMIssion = new();
        private PanelMission? selectPanelMission;
        private EqpMissionViewModel? selectedEqpMission;
        private BitmapImage? bitmapImage;
        public DateTime DateTime
        {
            get => dateTime;
            set => SetProperty(ref dateTime, value);
        }
        public int MoveRectWidth
        {
            get => moveRectWidth;
            set => SetProperty(ref moveRectWidth, value);
        }
        public ColorTool ColorTool
        {
            get => colorTool;
            set => SetProperty(ref colorTool, value);
        }
        public DateTimePickerViewModel DateTimePicker
        {
            get => dateTimePicker;
            set => SetProperty(ref dateTimePicker, value);
        }
        public CommandImplementation GetMissionCommand { get;}
        public CommandImplementation ShowFirstPanelMissionCommand { get; }
        public CommandImplementation JudgeCommand { get; }
        public CommandImplementation CopyCommand { get; }
        public ObservableCollection<EqpMissionViewModel> EqpMissionViewModels
        {
            get => eqpMissionViewModels;
            set => SetProperty(ref eqpMissionViewModels, value);
        }
        public ObservableCollection<PanelMission> FinishedPanelMIssion
        {
            get => finishedPanelMIssion;
            set => SetProperty(ref finishedPanelMIssion, value);
        }
        public PanelMission? SelectPanelMission
        {
            get => selectPanelMission;
            set => SetProperty(ref selectPanelMission, value);
        }
        public EqpMissionViewModel? SelectedEqpMission
        {
            get => selectedEqpMission;
            set => SetProperty(ref selectedEqpMission, value);
        }
        public BitmapImage? BitmapImage
        {
            get => bitmapImage;
            set => SetProperty(ref bitmapImage, value);
        }
        public MainWindowViewModel()
        {
            GetMissionCommand = new(_ => GetMission());//TODO：canexec方法；
            ShowFirstPanelMissionCommand = new(_=> ShowFirstPanelMission());
            JudgeCommand = new(PanelMissionJudge,_=> SelectPanelMission!=null);
            CopyCommand = new(CopyToClipboard);
            _ = new DispatcherTimer(
                    TimeSpan.FromMilliseconds(1000),
                    DispatcherPriority.Normal,
                    new EventHandler((o, e) =>
                    {
                        DateTime = DateTime.Now;
                    }),
                    Dispatcher.CurrentDispatcher);
        }

        private async void GetMission()
        {
            EqpMissionViewModels.Clear();
            DialogHost.Show(new ProgressMessageDialog(), "MainWindowDialog");
            await Task.Run(() =>
            {
                var startTime = DateTimePicker.StartTime;
                var endTime = DateTimePicker.EndTime;
                try
                {
                    var allMissions = ServerConnector.GetInfo(startTime, endTime);
                    AppLogClass.Logger.Information(":AllMissions 获取成功");
                    var missionGroup = ServerConnector.GetGroupedData(allMissions);
                    List<EqpMissionViewModel> missions = new();
                    foreach (var mission in missionGroup)
                    {

                        EqpMissionViewModel eqpMission = new(mission);
                        eqpMission.FillMissionViewCollection();
                        missions.Add(eqpMission);
                    }
                    if (missions.Count >= 1)
                    {
                        missions.Sort();
                        EqpMissionViewModels = new(missions);
                        SelectedEqpMission = EqpMissionViewModels[0];
                    }
                }
                catch (Exception ex)
                {
                    Application.Current.Dispatcher.Invoke(async () =>
                    {
                        await DialogHost.Show(new MessageAcceptDialog(string.Format("{0}", ex.Message)), "MainWindowDialog");
                    });
                    AppLogClass.Logger.Error(":获取任务时发生异常，异常信息：{0}", ex.Message);
                }
            });
            DialogHost.Close("MainWindowDialog");
            DialogHost.Show(new MessageAcceptDialog("刷新完成"), "MainWindowDialog");
        }
        public void ShowSelectedPanelMission()
        {
            BitmapImage = SelectPanelMission?.PanelImage;
        }
        public void ShowFinishedPanelMission()
        {
            BitmapImage = SelectPanelMission?.PanelImage;
        }
        public void ShowFirstPanelMission()
        {
            if (SelectedEqpMission?.PanelMissionOBCollection?.Count >= 1)
            {
                SelectPanelMission = SelectedEqpMission?.PanelMissionOBCollection[0];
                ShowSelectedPanelMission();
            }
        }
        public void PanelMissionJudge(object o)
        {
            if(o is bool result)
            {
                lock (finishLock)
                {
                    var id = SelectPanelMission?.Id;
                    if (id != null)
                    {
                        if (SelectPanelMission != null && SelectedEqpMission != null)
                        {
                            PanelMission panelMission = SelectPanelMission;
                            try
                            {
                                ServerConnector.SendResult(id, result == true ? 1 : 0);
                            }
                            catch (Exception ex)
                            {
                                DialogHost.Show(new MessageAcceptDialog("发送检查结果时发生异常，请联系管理员"), "MainWindowDialog");
                                AppLogClass.Logger.Error(ex.Message);
                                return;
                            }
                            var isRemoved = SelectedEqpMission.RemoveOneFromOBCollection(ref panelMission);
                            panelMission.Status = result == true ? 1 : 0;
                            panelMission.UpdateDate = DateTime.Now;
                            AddToFinishedCollection(panelMission);
                            if (isRemoved)
                            {
                                ShowFirstPanelMission();
                            }
                        }
                    }
                }
            }
        }

        public void CopyToClipboard(object o)
        {
            Clipboard.SetDataObject(o.ToString());
        }
        public void AddToFinishedCollection(PanelMission panelMission)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (FinishedPanelMIssion.Count > 20)
                {
                    FinishedPanelMIssion.Remove(FinishedPanelMIssion[0]);
                }
                FinishedPanelMIssion.Add(panelMission);
            });
        }
    }
}
