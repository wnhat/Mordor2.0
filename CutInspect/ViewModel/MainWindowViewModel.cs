using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using CoreClass.Model;
using CutInspect.Model;
using CutInspect.MyUserControl;

namespace CutInspect.ViewModel
{
    public class MainWindowViewModel:ViewModelBase
    {
        private DateTime dateTime;
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
            JudgeCommand = new(PanelMissionJudge,_=>true);
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
            var startTime = DateTimePicker.StartTime;
            var endTime = DateTimePicker.EndTime;
            try
            {
                var allMissions = ServerConnector.GetInfo(startTime, endTime);
                var missionGroup = ServerConnector.GetGroupedData(allMissions);
                EqpMissionViewModels.Clear();
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
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void ShowSelectedPanelMission()
        {
            BitmapImage = selectedEqpMission?.SelectPanelMission?.PanelImage;
        }
        public void ShowFinishedPanelMission()
        {
            BitmapImage = SelectPanelMission?.PanelImage;
        }
        public void ShowFirstPanelMission()
        {
            if (SelectedEqpMission?.PanelMissionOBCollection?.Count >= 1)
            {
                SelectedEqpMission.SelectPanelMission = SelectedEqpMission?.PanelMissionOBCollection[0];
                ShowSelectedPanelMission();
            }
        }
        public void PanelMissionJudge(object o)
        {
            if(o is bool result)
            {
                var id = SelectedEqpMission?.SelectPanelMission?.PanelInfo?.Id;
                if (id != null)
                {
                    try
                    {
                        ServerConnector.SendResult(id, result == true ? 1 : 0);
                        if (SelectedEqpMission.FinishPanelMission(out PanelMission? panelMission))
                        {
                            if(panelMission != null)
                            {
                                panelMission.Status = result == true ? 1 : 0;
                                AddToFinishedCollection(panelMission);
                            }
                            ShowFirstPanelMission();
                        };
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                    
                }
            }
        }
        public void AddToFinishedCollection(PanelMission panelMission)
        {
            if(FinishedPanelMIssion.Count > 20)
            {
                FinishedPanelMIssion.Remove(FinishedPanelMIssion[0]);
            }
            FinishedPanelMIssion.Add(panelMission);
        }
    }
}
