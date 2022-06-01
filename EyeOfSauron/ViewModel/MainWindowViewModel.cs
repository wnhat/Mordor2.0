using System.Windows.Controls;
using EyeOfSauron.MyUserControl;
using CoreClass.Model;
using System;
using System.Windows;

namespace EyeOfSauron.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        
        public MainWindowViewModel(UserInfoViewModel userInfoViewModel):this()
        {
            UserInfo = userInfoViewModel;
        }
        
        public MainWindowViewModel()
        {
            //ColorToolView = new DemoItem("Color Tool", typeof(ColorTool));
            MainContent = ProductSelectView;
            DefectJudgeView.DefectJudgeEvent += new DefectJudgeView.ValuePassHandler(DefectJudge);
            StartInspCommand = new CommandImplementation(StartInsp);
            EndInspCommand = new CommandImplementation(_ => EndInsp());
        }

        private UserInfoViewModel userInfo = new();
        //private DemoItem? colorToolView;
        private ProductSelectView productSelectView = new ();
        private InspImageView? inspImageView = new ();
        private DefectJudgeView defectJudgeView = new();
        private UserControl mainContent = new();
        private ColorTool? colorTool = new();
        private DateTime dateTime = DateTime.Now;
        private ViewName onShowView = ViewName.ProductSelectView;
        public CommandImplementation StartInspCommand { get; }
        
        public CommandImplementation EndInspCommand { get; }

        public ViewName OnShowView
        {
            get => onShowView;
            set => SetProperty(ref onShowView, value);
        }

        public DateTime DateTime
        {
            get => dateTime;
            set => SetProperty(ref dateTime, value);
        }

        public DefectJudgeView DefectJudgeView
        {
            get => defectJudgeView;
            set => SetProperty(ref defectJudgeView, value);
        }

        public ColorTool? ColorTool
        {
            get => colorTool;
            set => SetProperty(ref colorTool, value);
        }

        public UserControl MainContent
        {
            get => mainContent;
            set => SetProperty(ref mainContent, value);
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

        //public DemoItem? ColorToolView
        //{
        //    get => colorToolView;
        //    set => SetProperty(ref colorToolView, value);
        //}

        public void SetInspView()
        {
            MainContent = InspImageView;
            OnShowView = ViewName.InspImageView;
        }

        public void SetProductSelectView()
        {
            MainContent = ProductSelectView;
            OnShowView = ViewName.ProductSelectView;
        }

        private void StartInsp(object o)
        {
            if (UserInfo.UserExist)
            {
                if (o is ProductInfo && o != null)
                {
                    SetInspView();
                    var productInfo = o as ProductInfo;
                    try
                    {
                        Mission mission = new(productInfo);
                        InspImageView.SetMission(mission);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                        ProductSelectView.GetMissions();
                        SetProductSelectView();
                    }
                }
            }
            else
            {
                MessageBox.Show("Please login first!");
            }
            
        }
        
        private void EndInsp()
        {
            UserInfo.Logout();
            ProductSelectView.GetMissions();
            SetProductSelectView();
        }

        private void DefectJudge(object sender, DefectJudgeArgs e)
        {
            Defect defect = e.Defect;
            InspImageView.DefectJudge(defect, UserInfo.User);
        }
    }
    public enum ViewName
    {
        Null,
        InspImageView,
        ProductSelectView
    }
}