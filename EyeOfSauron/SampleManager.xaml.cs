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
using EyeOfSauron.MyUserControl;
using System.Collections.ObjectModel;

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
            //AddPanelMissionToCollectionDialog.DialogClosing += new DialogClosingEventHandler(_viewModel.AddToCollection_OnDialogClosing);
            InspViewDialogHost.DialogClosing += new DialogClosingEventHandler(InspViewDialog_OnDialogClosing);
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
                PanelSample.AddOnePanelSample(new(selectPanelMission.AetResult, new(MissionCollectionComboBox.Text), "", MissionType.Sample, selectPanelMission.ProductInfo));
                _viewModel.samplePanelListView.viewModel.GetSamples(MissionCollectionComboBox.Text);
            }
        }

        private void MissionCollectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (_viewModel.SelectedSamplePanelListViewMode != null)
                {
                    _viewModel.samplePanelListView.viewModel.PanelList = _viewModel.SelectedSamplePanelListViewMode.PanelList;
                    _viewModel.samplePanelListView.viewModel.CollectionName = _viewModel.SelectedSamplePanelListViewMode.CollectionName;
                    _viewModel.samplePanelListView.viewModel.SelectedItem = _viewModel.SelectedSamplePanelListViewMode.SelectedItem;
                }
            }
            catch (NullReferenceException)
            {
                //由于输入任务集名称造成的异常，不进行处理；
            }
        }

        private void CollectionSetting_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CollectionSettingDialog collectionSettingDialog = new();
                DialogHost.Show(collectionSettingDialog, "InspViewDialogHost");
            }
            catch (Exception ex)
            {
                DialogHost.Show(new MessageAcceptDialog{ Message = {Text = ex.Message} }, "InspViewDialogHost");
            }
        }

        private void AddCollectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.PanelListView.PanelList.SelectedItem != null )
            {
                var selectedItems = _viewModel.PanelListView.PanelList.SelectedItems;
                ObservableCollection<PanelViewContainer> panelViewContainers = new();
                foreach (var panelViewContainer in selectedItems)
                {
                    panelViewContainers.Add((PanelViewContainer)panelViewContainer);
                }
                AddToCollectionDialog addToCollectionDialog = new();
                addToCollectionDialog.viewModel.PanelMissions = panelViewContainers;
                DialogHost.Show(addToCollectionDialog, "InspViewDialogHost");
            }
        }

        private void InspViewDialog_OnDialogClosing(object sender, DialogClosingEventArgs e)
        {
            if (!Equals(e.Parameter, true))
            {
                return;
            }
            else
            {
                var eventSource = ((DialogHost)sender).DialogContent;
                if (eventSource is AddToCollectionDialog Dialog)
                {
                    var dialogViewModel = Dialog.viewModel;
                    List<Defect>? defects = new();
                    if (dialogViewModel.DefectSelectView.DefectSelectListBox.SelectedItem != null)
                    {
                        foreach (var defect in dialogViewModel.DefectSelectView.DefectSelectListBox.SelectedItems)
                        {
                            defects.Add((Defect)defect);
                        }
                    }
                    if (_viewModel.PanelListView.viewModel.SelectedItem != null && _viewModel.AddCollectionDialog_ComboxText != string.Empty)
                    {
                        foreach(var item in dialogViewModel.PanelMissions)
                        {
                            PanelMission selectPanelMission = item.PanelMission;
                            PanelSample.AddOnePanelSample(new(selectPanelMission.AetResult, new(_viewModel.AddCollectionDialog_ComboxText), dialogViewModel.NoteString, MissionType.Sample, selectPanelMission.ProductInfo, defects));
                        }
                        _viewModel.samplePanelListView.viewModel.GetSamples(_viewModel.AddCollectionDialog_ComboxText);
                        _viewModel.NoteString = string.Empty;
                    }
                }
                else if(eventSource is CollectionSettingDialog Dialog2)
                {
                    var dialogViewModel = Dialog2.viewModel;
                }
            }
        }
    }
}
