using System.Windows.Controls;
using EyeOfSauron.ViewModel;
using MaterialDesignThemes.Wpf;

namespace EyeOfSauron.MyUserControl
{
    /// <summary>
    /// Interaction logic for SampleMessageDialog.xaml
    /// </summary>
    public partial class PushExamMissionDialog : UserControl
    {
        public readonly PushExamMissionViewModel viewModel;
        public PushExamMissionDialog(MissionCollectionInfo missionCollectionInfo)
        {
            try
            {
                viewModel = new(missionCollectionInfo);
                DataContext = viewModel;
                InitializeComponent();
            }
            catch
            {
                throw;
            }
        }
    }
}
