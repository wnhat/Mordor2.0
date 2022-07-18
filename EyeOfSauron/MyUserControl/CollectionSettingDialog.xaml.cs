using System;
using System.Collections.Generic;
using System.Windows.Controls;
using CoreClass.Model;
using EyeOfSauron.ViewModel;
using MaterialDesignThemes.Wpf;

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

        private void MenuItem_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //try
            //{
                if (CollectionInfoList.SelectedItem != null)
                {
                    var a = viewModel.SelectPanelMissionCollectionInfo;
                    PushExamMissionDialog pushExamMissionDialog = new(a);
                    DialogHost.Show(pushExamMissionDialog, "CollectionSettingViewDialog");
                }
                else
                {
                    DialogHost.Show(new MessageAcceptDialog { Message = { Text = "未选定任何任务集" } }, "CollectionSettingViewDialog");
                }
            //}catch (Exception ex)
            //{
            //    DialogHost.Show(new MessageAcceptDialog { Message = { Text = ex.Message } }, "CollectionSettingViewDialog");
            //}

        }
        private void PushSettingDialogClosingEventHandler(object sender, DialogClosingEventArgs e)
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
                        string? CollectionName = viewModel.SelectPanelMissionCollectionInfo._id?.CollectionName;
                        List<ExamMissionWIP> ExamMissionWIPs = new();
                        foreach (var item in dialogViewModel.SelectedUsers)
                        {
                            ExamMissionWIPs.Add(new(item, CollectionName));
                        }
                        ExamMissionWIP.DelectMany(CollectionName);
                        if (ExamMissionWIPs.Count > 0) 
                        {
                            ExamMissionWIP.AddMany(ExamMissionWIPs);
                        }
                    }
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
