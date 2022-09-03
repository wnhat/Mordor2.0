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
            _ = new DispatcherTimer(
                    TimeSpan.FromMilliseconds(1000),
                    DispatcherPriority.Normal,
                    new EventHandler((o, e) =>
                    {
                        DateTime = DateTime.Now;
                    }),
                    Dispatcher.CurrentDispatcher);
        }

        private void GetMission()
        {
            EqpMissionViewModels.Clear();
            Task.Run(() =>
            {
                var startTime = DateTimePicker.StartTime;
                var endTime = DateTimePicker.EndTime;
                try
                {
                    var allMissions = ServerConnector.GetInfo(startTime, endTime);
                    var missionGroup = ServerConnector.GetGroupedData(allMissions);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        foreach (var mission in missionGroup)
                        {
                            EqpMissionViewModel eqpMission = new(mission);
                            eqpMission.FillMissionViewCollection();
                            EqpMissionViewModels.Add(eqpMission);
                            //TODO:排序
                        }
                        if (EqpMissionViewModels.Count >= 1)
                        {
                            SelectedEqpMission = EqpMissionViewModels[0];
                        }
                    });
                }
                catch (Exception)
                {
                    throw;
                }
            });
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
                    var id = SelectPanelMission?.PanelInfo?.Id;
                    if (id != null)
                    {
                        if (SelectPanelMission != null && SelectedEqpMission != null)
                        {
                            PanelMission panelMission = SelectPanelMission;
                            Task.Run(() =>
                            {
                                try
                                {
                                    ServerConnector.SendResult(id, result == true ? 1 : 0);
                                }
                                catch (Exception)
                                {
                                    throw;
                                }
                            });
                            var isRemoved = SelectedEqpMission.RemoveOneFromOBCollection(ref panelMission);
                            if (panelMission != null)
                            {
                                panelMission.PanelInfo.Status = result == true ? 1 : 0;
                                panelMission.PanelInfo.UpdateDate = DateTime.Now;
                                AddToFinishedCollection(panelMission);
                            }
                            if (isRemoved)
                            {
                                ShowFirstPanelMission();
                            }
                        }
                    }
                }
            }
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
