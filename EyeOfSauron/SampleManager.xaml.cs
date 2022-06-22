using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EyeOfSauron.ViewModel;
using MaterialDesignThemes.Wpf;
using EyeOfSauron.MyUserControl;
using System.Text.RegularExpressions;
using CoreClass.Model;
using CoreClass.Service;
using System.Linq;
using System.Windows.Threading;
using System;
using System.Threading.Tasks;

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
            ResultPanelList.PanelListViewDialog.DialogClosing += new DialogClosingEventHandler(PanelListAcceptCancelDialog_OnDialogClosing);
            ResultPanelList.PanelListBoxClearButton.Click += new RoutedEventHandler(PanelListBoxClearButton_Click);
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
                    ResultPanelList.viewModel.selectedItem = ResultPanelList.viewModel.PanelList[0];
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
                        ResultPanelList.viewModel.PanelList.Add(new PanelSampleContainer(panelMission));
                    });
                }
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ResultPanelList.viewModel.selectedItem != null)
            {
                _viewModel.InspImageView._viewModel.PanelId = ResultPanelList.viewModel.selectedItem.PanelMission.AetResult.PanelId;
                _viewModel.InspImageView._viewModel.InspImage.resultImageDataList = ResultPanelList.viewModel.selectedItem.PanelMission.resultImageDataList;
                _viewModel.InspImageView._viewModel.InspImage.defectImageDataList = ResultPanelList.viewModel.selectedItem.PanelMission.defectImageDataList;
                _viewModel.InspImageView._viewModel.InspImage.DefectMapImage = ResultPanelList.viewModel.selectedItem.PanelMission.ContoursImageContainer;
                _viewModel.InspImageView._viewModel.DetailDefectList.AetDetailDefects.Clear();
                foreach (var item in ResultPanelList.viewModel.selectedItem.PanelMission.bitmapImageContainers)
                {
                    _viewModel.InspImageView._viewModel.DetailDefectList.AetDetailDefects.Add(new AetDetailDefect(item.Name, item.Name, item.BitmapImage));
                }
                if (_viewModel.InspImageView._viewModel.DetailDefectList.AetDetailDefects.Count != 0)
                {
                    _viewModel.InspImageView._viewModel.DetailDefectList.SelectedItem = _viewModel.InspImageView._viewModel.DetailDefectList.AetDetailDefects.FirstOrDefault();
                }
                _viewModel.InspImageView._viewModel.InspImage.refreshPage = 0;
                _viewModel.InspImageView._viewModel.InspImage.RefreshImageMethod();
            }
        }

        private void PanelListBoxClearButton_Click(object sender, RoutedEventArgs e)
        {
            ResultPanelList.viewModel.PanelList.Clear();
        }

        private void MessageAcceptCancelDialog_OnDialogClosing(object sender, DialogClosingEventArgs e)
        {
            if (Equals(e.Parameter, true))
            {
                ResultPanelList.viewModel.PanelList.Clear();
            }
        }

    }
}
