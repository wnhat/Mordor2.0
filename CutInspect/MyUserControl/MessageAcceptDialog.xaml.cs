using CutInspect.Model;
using CutInspect.ViewModel;
using System.Windows.Controls;

namespace CutInspect.MyUserControl
{
    /// <summary>
    /// Interaction logic for SampleMessageDialog.xaml
    /// </summary>
    public partial class MessageAcceptDialog : UserControl
    {
        private MessageDialogViewModel _viewModel;
        public MessageAcceptDialog(string message)
        {
            _viewModel = new (message);
            DataContext = _viewModel;
            InitializeComponent();
            AppLogClass.Logger.Information(":MessageShow:{0}", _viewModel.Message);
        }
    }
}
