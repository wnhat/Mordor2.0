using System;
using System.Collections.Generic;
using System.Windows.Controls;
using CoreClass.Model;
using EyeOfSauron.ViewModel;
using MaterialDesignThemes.Wpf;
using MongoDB.Driver;

namespace EyeOfSauron.MyUserControl
{
    /// <summary>
    /// Interaction logic for SampleMessageDialog.xaml
    /// </summary>
    public partial class CollectionSettingDialog : UserControl
    {
        public readonly CollectionSettingViewModel viewModel;
        public CollectionSettingDialog()
        {
            try
            {
                viewModel = new();
            }
            catch (Exception)
            {
                throw;
            }
            DataContext = viewModel;
            InitializeComponent();
            CollectionSettingViewDialog.DialogClosing += new DialogClosingEventHandler(PushSettingDialogClosingEventHandler);
        }

        private async void ExamPush(object sender, System.Windows.RoutedEventArgs e)
        {
            if (viewModel.SelectPanelMissionCollectionInfo != null)
            {
                PushExamMissionDialog pushExamMissionDialog = new(viewModel.SelectPanelMissionCollectionInfo);
                await DialogHost.Show(pushExamMissionDialog, "CollectionSettingViewDialog");
            }
            else
            {
                await DialogHost.Show(new MessageAcceptDialog { Message = { Text = "未选定任何任务集" } }, "CollectionSettingViewDialog");
            }
        }
        private async void PushSettingDialogClosingEventHandler(object sender, DialogClosingEventArgs e)
        {
            if (!Equals(e.Parameter, true))
            {
                return;
            }
            else
            {
                var eventSource = ((DialogHost)sender).DialogContent;
                if (eventSource is PushExamMissionDialog Dialog)
                {
                    e.Handled = true;
                    var dialogViewModel = Dialog.viewModel;
                    if (viewModel.SelectPanelMissionCollectionInfo != null)
                    {
                        string? CollectionName = viewModel.SelectPanelMissionCollectionInfo.MissionCollection.CollectionName;
                        await PanelSample.UpdateProperty("MissionCollection.CollectionName", CollectionName, "MissionCollection.MissionType", MissionType.ExamMission);
                        if (dialogViewModel.SelectedUsers.Count > 0)
                        {
                            foreach (var item in dialogViewModel.SelectedUsers)
                            {
                                ExamMissionCollection.UpdateOrAdd(new ExamMissionCollection(item, CollectionName));
                            }
                        }
                        else
                        {
                            await PanelSample.UpdateProperty("MissionCollection.CollectionName", CollectionName, "MissionCollection.MissionType", MissionType.Sample);
                        }
                    }
                    await viewModel.RefreshCollectView();
                }
            }
        }
        public void MessageDialogOpenedEventHandler(object sender, DialogOpenedEventArgs e)
        {

        }

        public void MessageDialogClosingEventHandler(object sender, DialogClosingEventArgs e)
        {

        }
    }
}
