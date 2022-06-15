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
        private readonly MainWindowViewModel _viewModel;

        public LogininWindow loginWindow = new();
        
        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new();
            DataContext = _viewModel;
            loginWindow.AccountAuthenticateEvent += new LogininWindow.ValuePassHandler(AccountAuthenticate);
        }

        private void AccountAuthenticate(object sender, AccountAuthenticateEventArgs arges)
        {
            _viewModel.UserInfo.User = arges.User;
            loginWindow.Close();
            MainSnackbar.MessageQueue?.Enqueue("Welcome Login to Eye of Sauron");
        }

        private async void MenuPopupButton_OnClick(object sender, RoutedEventArgs e)
        {
            var sampleMessageDialog = new SampleMessageDialog
            {
                Message = { Text = ((ButtonBase)sender).Content.ToString() }
            };

            await DialogHost.Show(sampleMessageDialog, "RootDialog");
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
            MainSnackbar.MessageQueue?.Enqueue("Copy Successfully");
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

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (MessageBox.Show("确认退出", "退出", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Environment.Exit(0);
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
