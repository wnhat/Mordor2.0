using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MaterialDesignThemes.Wpf;
using System.Text.RegularExpressions;
using CoreClass.Model;
using System.Linq;
using System.Threading.Tasks;
using EyeOfSauron.ViewModel;

namespace EyeOfSauron
{
    /// <summary>
    /// Interaction logic for SampleManager.xaml
    /// </summary>
    public partial class SampleViewWindow : Window
    {
        private readonly SampleViewerViewModel _viewModel;
        public SampleViewWindow()
        {
            InitializeComponent();
            _viewModel = new();
            DataContext = _viewModel;
            ResultPanelList.PanelList.SelectionChanged += new SelectionChangedEventHandler(ListView_SelectionChanged);
            ResultPanelList.PanelList.MouseUp += new MouseButtonEventHandler(ListView_MouseUp);
            ResultPanelList.PanelListViewDialog.DialogClosing += new DialogClosingEventHandler(PanelListAcceptCancelDialog_OnDialogClosing);
            ResultPanelList.PanelListBoxClearButton.Click += new RoutedEventHandler(PanelListBoxClearButton_Click);
            _viewModel.samplePanelListView.PanelSampleList.SelectionChanged += new SelectionChangedEventHandler(PanelSampleList_SelectionChanged);
            MainSnackbar.MessageQueue?.Enqueue("Welcome to Eye of Sauron");
        }

        private void ColorToolToggleButton_OnClick(object sender, RoutedEventArgs e)
            => ImageView.Focus();

        private void PanelidLableMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string? text = ((Button)sender).Content.ToString();
            Clipboard.SetDataObject(text);
            MainSnackbar.MessageQueue?.Enqueue("复制成功");
        }

        private async void PanelListAcceptCancelDialog_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, true))
            {
                ResultPanelList.InputTextBox.Clear();
                return;
            }
            if (!string.IsNullOrWhiteSpace(ResultPanelList.InputTextBox.Text))
            {
                //Match every panel ID of input;
                Regex regex = new(@"7[0-9,A-Z][0-9][0-9,A-Z][0-9][1-9,X-Z][0-9,D,E][0-9]{3}[A-C][0-9][A-B][A-B][A-Z][0-2][0-9]");
                string inputText = ResultPanelList.InputTextBox.Text.ToUpper().Replace(" ", "");
                var panelIdList = regex.Matches(inputText);
                //ProgressBar set
                if (panelIdList.Count > 0)
                {
                    _viewModel.totalPanelCount = panelIdList.Count;
                    _viewModel.dispatcherTimer.Start();
                }
                //Buffer panel list for deduplicating
                List<string> bufferList = new();
                foreach (Match item in panelIdList)
                {
                    string panelId = item.Value;
                    if (!bufferList.Contains(panelId))
                    {
                        bufferList.Add(panelId);
                        _viewModel.loadedPanelCount = bufferList.Count;
                        await Task.Run(() => LoadOnePanel(panelId));
                    }
                }
                if (ResultPanelList.viewModel.PanelList.Count > 0)
                {
                    ResultPanelList.viewModel.SelectedItem = ResultPanelList.viewModel.PanelList[0];
                }
                ResultPanelList.InputTextBox.Clear();
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
                        ResultPanelList.viewModel.PanelList.Add(new PanelViewContainer(panelMission));
                    });
                }
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ResultPanelList.viewModel.SelectedItem != null)
            {
                _viewModel.InspImageView.LoadOneInspImageView(ResultPanelList.viewModel.SelectedItem);
            }
        }

        private void ListView_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void PanelListBoxClearButton_Click(object sender, RoutedEventArgs e)
        {
            ResultPanelList.viewModel.PanelList.Clear();
        }

        private void AddPanelSampleButton_Click(object sender, RoutedEventArgs e)
        {
            if (ResultPanelList.viewModel.SelectedItem != null)
            {
                PanelSample.AddOnePanelSample(new(ResultPanelList.viewModel.SelectedItem.PanelMission.AetResult, MissionCollectionComboBox.Text, "", MissionType.Sample));
                _viewModel.samplePanelListView.viewModel.GetSamples();
            }
        }

        private void PanelSampleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel.samplePanelListView.viewModel.SelectedItem != null)
            {
                _viewModel.InspImageView.LoadOneInspImageView(_viewModel.samplePanelListView.viewModel.SelectedItem);
            }
        }
    }
}
