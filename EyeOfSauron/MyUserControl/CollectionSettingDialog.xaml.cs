using System.Windows.Controls;
using EyeOfSauron.ViewModel;

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
            viewModel = new();
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
