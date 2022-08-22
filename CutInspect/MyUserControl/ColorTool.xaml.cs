using CutInspect.ViewModel;
using System.Windows.Controls;

namespace CutInspect.MyUserControl
{
    public partial class ColorTool : UserControl
    {
        public ColorTool()
        {
            DataContext = new ColorToolViewModel();
            InitializeComponent();
        }
    }
}
