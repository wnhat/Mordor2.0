using System;
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
            try
            {
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
            }catch (Exception ex)
            {
                DialogHost.Show(new MessageAcceptDialog { Message = { Text = ex.Message } }, "CollectionSettingViewDialog");
            }

        }
        private void PushSettingDialogClosingEventHandler(object sender, DialogClosingEventArgs e)
        {
            if (e.Handled)
            {
                e.Cancel();
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
