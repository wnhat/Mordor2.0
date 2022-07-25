using System;
using System.Diagnostics;
using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Controls;
using EyeOfSauron.ViewModel;
using EyeOfSauron.MyUserControl;
using CoreClass.Model;
using MongoDB.Bson;
using System.Net;
using System.Collections.Generic;

namespace EyeOfSauron
{
    public partial class MainWindow : Window
    {
        public static Snackbar Snackbar = new();

        private readonly MainWindowViewModel? _viewModel;
        
        private LogininWindow loginWindow = new();

        public MainWindow()
        {
            InitializeComponent();
            try
            {
                _viewModel = new();
                _viewModel.LoginRequestEvent += LoginButton_Click;
                DataContext = _viewModel;
                Snackbar = MainSnackbar;
                loginWindow.UserAuthenticateEvent += new RoutedEventHandler(UserAuthenticate);
                _viewModel.ProductSelectView.ExamMissionRefreshButton.Click += new RoutedEventHandler(ExamMissionRefresh);
                _viewModel.DefectJudgeView.DefectJudgedEvent += new RoutedEventHandler(DefectJudge);
                _viewModel.MissionFinishedEvent += new RoutedEventHandler(FinishMission);
                _viewModel.UserInfo.LogoutEvent += new RoutedEventHandler(UserLogout);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void UserAuthenticate(object sender,RoutedEventArgs e)
        {
            if(_viewModel.Eqp != null)
            {
                _viewModel.UserInfo.User = (User)sender;
                loginWindow.Close();
                _viewModel.ProductSelectView._viewModel.ExamMissionCardViewModels.Clear();
                if (_viewModel.UserInfo.UserExist)
                {
                    var examMissionWIPs = ExamMissionCollection.GetByUser(_viewModel.UserInfo.User.Id);
                    _viewModel.ProductSelectView.SetExamMissions(examMissionWIPs);
                    if (_viewModel.ProductSelectView._viewModel.ExamMissionCardViewModels.Count > 0)
                    {
                        DialogHost.Show(new MessageAcceptDialog { Message = { Text = "有待完成的考试任务" } }, "MainWindowDialog");
                        _viewModel.ProductSelectView._viewModel.ControlTabSelectedIndex = ControlTableItem.ExamMission;
                    }
                }
                MainSnackbar.MessageQueue?.Enqueue(string.Format("Welcome to Eye Of Sauron, {0}", _viewModel.UserInfo.User.Username));
            }
            else
            {
                DialogHost.Show(new MessageAcceptDialog { Message = { Text = "此设备未注册，请联系DICS管理员" } }, "MainWindowDialog");
            }
        }

        private void OnCopy(object sender, ExecutedRoutedEventArgs e)
        {
            if (e.Parameter is string stringValue)
            {
                try
                {
                    Clipboard.SetDataObject(stringValue);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                }
            }
        }

        private void ColorToolToggleButton_OnClick(object sender, RoutedEventArgs e) 
            => MainContent.Focus();
        
        private static void ModifyTheme(bool isDarkTheme)
        {
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();

            theme.SetBaseTheme(isDarkTheme ? Theme.Dark : Theme.Light);
            paletteHelper.SetTheme(theme);
        }

        private void PanelidLableMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string? text = ((Button)sender).Content.ToString();
            Clipboard.SetDataObject(text);
            MainSnackbar.MessageQueue?.Enqueue("复制成功");
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (!_viewModel.UserInfo.UserExist)
            {
                loginWindow = new();
                loginWindow.UserAuthenticateEvent += new RoutedEventHandler(UserAuthenticate);
                loginWindow.ShowDialog();
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Alt && e.SystemKey == Key.F4)
            {
                e.Handled = true;
            }
            else if (e.SystemKey == Key.Enter || e.SystemKey == Key.Space)
            {
                e.Handled= true;
            }
            else
            {
                base.OnKeyDown(e);
            }
        }

        private async void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.UserInfo.UserExist)
            {
                var result = await DialogHost.Show(new MessageAcceptCancelDialog { Message = { Text = "退出登录，将退出当前任务"} }, "MainWindowDialog");
                if(result is bool value)
                {
                    if (value)
                    {
                        _viewModel.UserInfo.Logout();
                    }
                }
            }
        }

        private void UserLogout(object sender, RoutedEventArgs e)
        {
            ExamMissionRefresh(sender, e);
            _viewModel.SetProductSelectView();
            MainSnackbar.MessageQueue?.Enqueue("退出登录成功");
        }

        private async void SampleViewButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.UserInfo.UserExist)
            {
                SampleViewWindow sampleViewWindow = new();
                sampleViewWindow.ShowDialog();
            }
            else
            {
                await DialogHost.Show(new MessageAcceptDialog { Message = { Text = "请登录后操作" } }, "MainWindowDialog");
                LoginButton_Click(sender, e);
            } 
        }

        private void DefectJudge(object sender, RoutedEventArgs e)
        {
            this.MainContent.Focus();
        }

        private void ExamMissionRefresh(object sender, RoutedEventArgs e)
        {
            _viewModel.ProductSelectView._viewModel.ExamMissionCardViewModels.Clear();
            if (_viewModel.UserInfo.UserExist)
            {
                var examMissioncCollection = ExamMissionCollection.GetByUser(_viewModel.UserInfo.User.Id);
                _viewModel.ProductSelectView.SetExamMissions(examMissioncCollection);
                Snackbar.MessageQueue?.Enqueue("ExamMission Refresh Successfully");
            }
            else
            {
                DialogHost.Show(new MessageAcceptDialog { Message = { Text = "请登录后操作" } }, "MainWindowDialog");
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (Window w in Application.Current.Windows)
            {
                if (w != this)
                    w.Close();
            }
        }

        private async void UserChangeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.UserInfo.UserExist)
            {
                var result = await DialogHost.Show(new MessageAcceptCancelDialog { Message = { Text = "将退出当前用户，重新登录" } }, "MainWindowDialog");
                if (result is bool value)
                {
                    if (value)
                    {
                        loginWindow = new();
                        loginWindow.UserAuthenticateEvent += new RoutedEventHandler(UserAuthenticate);
                        loginWindow.ShowDialog();
                    }
                }
                
            }
        }

        private void FinishMission(object sender, RoutedEventArgs e)
        {
            if(sender is ObjectId collectionId)
            {
                var result = ExamMissionResult.GetAccuracyValue(collectionId);
                int correctCount = 0;
                int incorrectCount = 0;
                foreach(var item in result)
                {
                    var a = item.GetValue("_id").AsBoolean;
                    if (a)
                    {
                        correctCount = item.GetValue("count").AsInt32;
                    }
                    else
                    {
                        incorrectCount = item.GetValue("count").AsInt32;
                    }
                }
                int totalCount = correctCount + incorrectCount;
                double examScore = 100 * correctCount/totalCount;
                DialogHost.Show(new MessageAcceptCancelDialog { Message = { Text = String.Format("正确率为：{0}%", examScore.ToString()) } }, "MainWindowDialog");
            }
            ExamMissionRefresh(this, e);
            _viewModel?.SetProductSelectView();
        }

        private void SetEqpDB()
        {
            //TODO:注册设备IP
            List<DicsEqp> dicsEqps = new();
            string Ip1 = "172";
            string Ip2 = "16";
            string Ip3 = "160";
            string Ip4 = "1";
            string num = "";
            for (int j = 1; j <= 25; j++)
            {
                if (j < 10)
                {
                    num = string.Format("0{0}", j);
                }
                else { num = j.ToString(); }
                Ip4 = j.ToString();
                DicsEqp dicsEqp = new(string.Format("{0}.{1}.{2}.{3}", Ip1, Ip2, Ip3, Ip4), string.Format("DICS{0}",num));
                dicsEqps.Add(dicsEqp);
            }
            DicsEqp.AddMany(dicsEqps);
        }
    }
}
