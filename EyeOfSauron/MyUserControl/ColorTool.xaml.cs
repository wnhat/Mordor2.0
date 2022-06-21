using EyeOfSauron.ViewModel;
using System.Windows.Controls;

namespace EyeOfSauron.MyUserControl
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
