using System;
using EyeOfSauron.MyUserControl;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using CoreClass.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Text.RegularExpressions;

namespace EyeOfSauron.ViewModel
{
    public class SampleViewerViewModel : ViewModelBase
    {
        private ColorTool colorTool = new();
        private DateTime dateTime = DateTime.Now;
        private InspImageView inspImageView = new();
        private DefectSelectView defectSelectView = new();
        private PanelListView panelListView = new();
        private string noteString = string.Empty;
        private double loadMissionProgressValue;
        public readonly DispatcherTimer dispatcherTimer = new();
        public int totalPanelCount = 0;
        public int loadedPanelCount = 0;
        public SamplePanelListView samplePanelListView = new();
        public ObservableCollection<SamplePanelListViewModel> sampleCollection = new();
        public SamplePanelListViewModel selectedSamplePanelListViewModel = new();
        public CommandImplementation RefreshMissionCollection { get; }

        public SampleViewerViewModel()
        {
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);
            dispatcherTimer.Tick += new EventHandler(MissionLoadProgress);
            PanelListView.PanelList.SelectionChanged += new SelectionChangedEventHandler(ListView_SelectionChanged);
            PanelListView.PanelListViewDialog.DialogClosing += new DialogClosingEventHandler(PanelListAcceptCancelDialog_OnDialogClosing);
            PanelListView.PanelListBoxClearButton.Click += new RoutedEventHandler(PanelListBoxClearButton_Click);
            samplePanelListView.PanelSampleList.SelectionChanged += new SelectionChangedEventHandler(PanelSampleList_SelectionChanged);

            _ = new DispatcherTimer(
                    TimeSpan.FromMilliseconds(1000),
                    DispatcherPriority.Normal,
                    new EventHandler((o, e) =>
                    {
                        DateTime = DateTime.Now;
                    }), Dispatcher.CurrentDispatcher);
            InspImageView._viewModel.ExtendedUserControl = samplePanelListView;
            RefreshMissionCollection = new(_=>RefreshCollection());
            RefreshCollection();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PanelListView.viewModel.SelectedItem != null)
            {
                InspImageView.LoadOneInspImageView(PanelListView.viewModel.SelectedItem);
            }
        }

        private async void PanelListAcceptCancelDialog_OnDialogClosing(object sender, DialogClosingEventArgs e)
        {
            e.Handled = true;
            if (!Equals(e.Parameter, true))
            {
                PanelListView.InputTextBox.Clear();
                return;
            }
            if (!string.IsNullOrWhiteSpace(PanelListView.InputTextBox.Text))
            {
                //Match every panel ID of input;
                Regex regex = new(@"7[0-9,A-Z][0-9][0-9,A-Z][0-9][1-9,X-Z][0-9,D,E][0-9]{3}[A-C][0-9][A-B][A-B][A-Z][0-2][0-9]");
                string inputText = PanelListView.InputTextBox.Text.ToUpper().Replace(" ", "");
                var panelIdList = regex.Matches(inputText);
                //ProgressBar set
                if (panelIdList.Count > 0)
                {
                    totalPanelCount = panelIdList.Count;
                    dispatcherTimer.Start();
                }
                //Buffer panel list for deduplicating
                List<string> bufferList = new();
                foreach (Match item in panelIdList)
                {
                    string panelId = item.Value;
                    if (!bufferList.Contains(panelId))
                    {
                        bufferList.Add(panelId);
                        loadedPanelCount = bufferList.Count;
                        await Task.Run(() => LoadOnePanel(panelId));
                    }
                }
                if (PanelListView.viewModel.PanelList.Count > 0)
                {
                    PanelListView.viewModel.SelectedItem = PanelListView.viewModel.PanelList[0];
                }
                PanelListView.InputTextBox.Clear();
            }
        }

        /// <summary>
        /// Load all panel mission and add to PanelList
        /// </summary>
        /// <param name="panelId"></param>
        private void LoadOnePanel(string panelId)
        {
            var aetResults = AETresult.Get(panelId);
            if (aetResults != null)
            {
                foreach (var aetResult in aetResults)
                {
                    PanelMission panelMission = new(aetResult);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        PanelListView.viewModel.PanelList.Add(new PanelViewContainer(panelMission));
                    });
                }
            }
        }

        private void PanelListBoxClearButton_Click(object sender, RoutedEventArgs e)
        {
            PanelListView.viewModel.PanelList.Clear();
        }

        private void PanelSampleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (samplePanelListView.viewModel.SelectedItem != null)
            {
                InspImageView.LoadOneInspImageView(samplePanelListView.viewModel.SelectedItem);
            }
        }

        public static List<string> GetCollectionNames()
        {
            var Missions = PanelSample.GetMissionCollection();
            List<string> MissionCollection = new();
            foreach (var item in Missions)
            {
                var mission = item.GetValue("_id").AsString;
                MissionCollection.Add(mission);
            }
            return MissionCollection;
        }

        public void RefreshCollection()
        {
            sampleCollection.Clear();
            var collection = GetCollectionNames();
            foreach (string item in collection)
            {
                sampleCollection.Add(new(item));
            }
        }

        public string NoteString
        {
            get => noteString;
            set => SetProperty(ref noteString, value);
        }

        public PanelListView PanelListView
        {
            get => panelListView;
            set => SetProperty(ref panelListView, value);
        }

        public double LoadMissionProgressValue
        {
            get => loadMissionProgressValue;
            set => SetProperty(ref loadMissionProgressValue, value);
        }

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

        public InspImageView InspImageView
        {
            get => inspImageView;
            set => SetProperty(ref inspImageView, value);
        }

        public DefectSelectView DefectSelectView
        {
            get => defectSelectView;
            set => SetProperty(ref defectSelectView, value);
        }

        public ObservableCollection<SamplePanelListViewModel> SampleCollection
        {
            get => sampleCollection;
            set => SetProperty(ref sampleCollection, value);
        }

        public SamplePanelListViewModel SelectedSamplePanelListViewMode
        {
            get => selectedSamplePanelListViewModel;
            set => SetProperty(ref selectedSamplePanelListViewModel, value);
        }

        private void MissionLoadProgress(object sender, EventArgs e)
        {
            if( LoadMissionProgressValue < 100 )
            {
                var panelLoadPercentComplete = 100.0 / totalPanelCount * loadedPanelCount;
                LoadMissionProgressValue = panelLoadPercentComplete;
            }
            else
            {
                LoadMissionProgressValue = 0;
                dispatcherTimer.Stop();
            }
        }
    }
}
