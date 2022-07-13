using System.Windows.Controls;
using EyeOfSauron.ViewModel;

namespace EyeOfSauron.MyUserControl
{
    /// <summary>
    /// Interaction logic for SampleMessageDialog.xaml
    /// </summary>
    public partial class AddToCollectionDialog : UserControl
    {
        public readonly AddToCollectionViewModel viewModel;
        public AddToCollectionDialog()
        {
            viewModel = new();
            DataContext = viewModel;
            InitializeComponent();
        }
    }
}
