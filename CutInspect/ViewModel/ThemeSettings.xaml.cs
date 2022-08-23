using CutInspect.ViewModel;

namespace CutInspect.MyUserControl
{
    public partial class ThemeSettings
    {
        public ThemeSettings()
        {
            DataContext = new ThemeSettingsViewModel();
            InitializeComponent();
        }
    }
}
