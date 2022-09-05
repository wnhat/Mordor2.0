using CutInspect.Model;
using System.Windows.Controls;

namespace CutInspect.MyUserControl
{
    /// <summary>
    /// Interaction logic for SampleMessageDialog.xaml
    /// </summary>
    public partial class MessageAcceptDialog : UserControl
    {
        public MessageAcceptDialog()
        {
            InitializeComponent();
            AppLogClass.Logger.Information(":MessageShow:{0}",Message.Text);
        }
    }
}
