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
            ProductSelectView = new();
            InspImageView = new();
            MainContent = ProductSelectView;
            OnShowView = ViewName.ProductSelectView;      
        }
        
        public MainWindowViewModel()
        {
            ColorToolView = new DemoItem("Color Tool", typeof(ColorTool));
            DefectJudgeView = new();
            MainContent = new();
            ColorTool = new();
            OnShowView = ViewName.Null;
            DefectJudgeView.DefectJudgeEvent += new DefectJudgeView.ValuePassHandler(DefectJudge);
            StartInspCommand = new CommandImplementation(StartInsp);
            EndInspCommand = new CommandImplementation(_ => EndInsp());
        }

        private UserInfoViewModel? userInfo;
        private DemoItem? colorToolView;
        private ProductSelectView? productSelectView;
        private InspImageView? inspImageView;
        private DefectJudgeView? defectJudgeView;
        private UserControl? mainContent;
        private ColorTool? colorTool;
        
        private ViewName onShowView;
        public CommandImplementation StartInspCommand { get; }
        public CommandImplementation EndInspCommand { get; }

        public ViewName OnShowView
        {
            get => onShowView;
            set => SetProperty(ref onShowView, value);
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

        public DemoItem? ColorToolView
        {
            get => colorToolView;
            set => SetProperty(ref colorToolView, value);
        }

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
            if(o is ProductInfo)
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
        
        private void EndInsp()
        {
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