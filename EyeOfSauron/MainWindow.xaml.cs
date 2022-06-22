using System;
using System.Diagnostics;
using EyeOfSauron.ViewModel;
using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Controls;
using EyeOfSauron.MyUserControl;

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
                loginWindow.AccountAuthenticateEvent += new LogininWindow.ValuePassHandler(AccountAuthenticate);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void AccountAuthenticate(object sender, AccountAuthenticateEventArgs arges)
        {
            _viewModel.UserInfo.User = arges.User;
            loginWindow.Close();
            MainSnackbar.MessageQueue?.Enqueue(string.Format("Welcome to Eye Of Sauron, {0}", _viewModel.UserInfo.User.Username));
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
            => MainScrollViewer.Focus();
        
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
                loginWindow.AccountAuthenticateEvent += new LogininWindow.ValuePassHandler(AccountAuthenticate);
                loginWindow.ShowDialog();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                case Key.Space:
                    e.Handled = true;
                    break;
                default:
                    break;
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
                        _viewModel.SetProductSelectView();
                        MainSnackbar.MessageQueue?.Enqueue("退出登录成功");
                    }
                }
            }
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (Window w in Application.Current.Windows)
            {
                if (w != this)
                    w.Close();
            }
        }

        private void UserChangeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.UserInfo.UserExist)
            {
                loginWindow = new();
                loginWindow.AccountAuthenticateEvent += new LogininWindow.ValuePassHandler(AccountAuthenticate);
                loginWindow.ShowDialog();
            }
        }
    }
}
