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
using System;

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
            MainSnackbar.MessageQueue?.Enqueue("Welcome to Eye of Sauron");
            AddPanelMissionToCollectionDialog.DialogClosing += new DialogClosingEventHandler(AddPanelMissionToCollection_OnDialogClosing);
        }

        private void ColorToolToggleButton_OnClick(object sender, RoutedEventArgs e)
            => ImageView.Focus();

        private void PanelidLableMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string? text = ((Button)sender).Content.ToString();
            Clipboard.SetDataObject(text);
            MainSnackbar.MessageQueue?.Enqueue("复制成功");
        }

        private void AddPanelSampleButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.PanelListView.viewModel.SelectedItem != null && MissionCollectionComboBox.Text != string.Empty)
            {
                PanelMission selectPanelMission = _viewModel.PanelListView.viewModel.SelectedItem.PanelMission;
                PanelSample.AddOnePanelSample(new(selectPanelMission.AetResult, MissionCollectionComboBox.Text, "", MissionType.Sample, selectPanelMission.ProductInfo));
                _viewModel.samplePanelListView.viewModel.GetSamples(MissionCollectionComboBox.Text);
            }
        }

        private void MissionCollectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if(_viewModel.SelectedSamplePanelListViewMode != null)
                {
                    _viewModel.samplePanelListView.viewModel.PanelList = _viewModel.SelectedSamplePanelListViewMode.PanelList;
                    _viewModel.samplePanelListView.viewModel.CollectionName = _viewModel.SelectedSamplePanelListViewMode.CollectionName;
                    _viewModel.samplePanelListView.viewModel.SelectedItem = _viewModel.SelectedSamplePanelListViewMode.SelectedItem;
                }
            }
            catch (NullReferenceException)
            {
            }
        }

        private void AddPanelMissionToCollection_OnDialogClosing(object sender, DialogClosingEventArgs e)
        {
            if (!Equals(e.Parameter, true))
            {
                return ;
            }
            else
            {
                var defects = _viewModel.DefectSelectView.DefectSelectListBox.SelectedItems;
                Defect[] defectArray = new Defect[defects.Count];
                for(int i = 0; i < defects.Count; i++)
                {
                    defectArray[i] = (Defect)defects[i];
                }
                if (_viewModel.PanelListView.viewModel.SelectedItem != null && MissionCollectionComboBox.Text != string.Empty)
                {
                    PanelMission selectPanelMission = _viewModel.PanelListView.viewModel.SelectedItem.PanelMission;
                    PanelSample.AddOnePanelSample(new(selectPanelMission.AetResult, MissionCollectionComboBox.Text, _viewModel.NoteString,  MissionType.Sample, selectPanelMission.ProductInfo));
                    _viewModel.samplePanelListView.viewModel.GetSamples(MissionCollectionComboBox.Text);
                    _viewModel.NoteString = string.Empty;
                }
            }
        }
    }
}
