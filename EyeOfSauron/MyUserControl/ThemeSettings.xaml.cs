using EyeOfSauron.ViewModel;

namespace EyeOfSauron.MyUserControl
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
