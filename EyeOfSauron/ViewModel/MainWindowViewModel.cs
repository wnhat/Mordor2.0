using System.Windows.Controls;
using EyeOfSauron.MyUserControl;

namespace EyeOfSauron.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel(UserInfoViewModel userInfoViewModel)
        {
            UserInfo = userInfoViewModel;
            ColorToolView = new DemoItem("Color Tool", typeof(ColorTool));
            ProductSelectWindow = new();
            UserControlContent = ProductSelectWindow;
        }
        private UserInfoViewModel? userInfo;
        private DemoItem? colorToolView;
        private MyUserControl.ProductSelectWindow? productSelectWindow;
        private MyUserControl.InspWindow? inspWindow;
        private UserControl? userControlContent;

        public UserControl UserControlContent
        {
            get => userControlContent;
            set => SetProperty(ref userControlContent, value);
        }

        public MyUserControl.ProductSelectWindow ProductSelectWindow
        {
            get => productSelectWindow;
            set => SetProperty(ref productSelectWindow, value);
        }

        public MyUserControl.InspWindow InspWindow
        {
            get => inspWindow;
            set => SetProperty(ref inspWindow, value);
        }

        public UserInfoViewModel UserInfo
        {
            get => userInfo;
            set => SetProperty(ref userInfo, value);
        }

        public DemoItem? ColorToolView
        {
            get => colorToolView;
            set => SetProperty(ref colorToolView, value);
        }

        public void IniInspWindow()
        {
            InspWindow = new MyUserControl.InspWindow();
            UserControlContent = InspWindow;
        }
    }
}