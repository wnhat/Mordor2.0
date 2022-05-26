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
            //ColorToolView = new ();
            ProductSelectView = new();
            InspImageView = new();
            UserControlContent = ProductSelectView;
        }
        
        private UserInfoViewModel? userInfo;
        private DemoItem? colorToolView;
        private ProductSelectView? productSelectView;
        private InspImageView? inspImageView;
        private UserControl? userControlContent;
        private ViewName onShowView;

        public ViewName OnShowView
        {
            get => onShowView;
            set => SetProperty(ref onShowView, value);
        }

        public UserControl UserControlContent
        {
            get => userControlContent;
            set => SetProperty(ref userControlContent, value);
        }

        public ProductSelectView ProductSelectView
        {
            get => productSelectView;
            set => SetProperty(ref productSelectView, value);
        }

        public InspImageView InspImageView
        {
            get => inspImageView;
            set => SetProperty(ref inspImageView, value);
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

        public void SetInspView()
        {
            UserControlContent = InspImageView;
            OnShowView = ViewName.InspImageView;
        }

        public void SetProductSelectView()
        {
            UserControlContent = ProductSelectView;
            OnShowView = ViewName.ProductSelectView;
        }
    }
    public enum ViewName
    {
        InspImageView,
        ProductSelectView
    }
}