using System;
using System.Diagnostics;
using EyeOfSauron.ViewModel;
using MaterialDesignThemes.Wpf;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using CoreClass.Model;
using System.Windows.Controls;
using EyeOfSauron.MyUserControl;
using EyeOfSauron.ViewModel;

namespace EyeOfSauron
{
    public partial class MainWindow : Window
    {
        public static Snackbar Snackbar = new();        
        
        private MainWindowViewModel viewModel;

        public LogininWindow loginWindow = new();
        public MainWindow()
        {
            InitializeComponent();
            viewModel = new();
            DataContext = viewModel;
            Task.Factory.StartNew(() => Thread.Sleep(1000)).ContinueWith(t =>
            {
                //note you can use the message queue from any thread, but just for the demo here we 
                //need to get the message queue from the snackbar, so need to be on the dispatcher
                MainSnackbar.MessageQueue?.Enqueue("Welcome Login to Eye of Sauron");
            }, TaskScheduler.FromCurrentSynchronizationContext());
            //var paletteHelper = new PaletteHelper();
            //var theme = paletteHelper.GetTheme();
            //DarkModeToggleButton.IsChecked = theme.GetBaseTheme() == BaseTheme.Dark;
            //if (paletteHelper.GetThemeManager() is { } themeManager)
            //{
            //    themeManager.ThemeChanged += (_, e)
            //        => DarkModeToggleButton.IsChecked = e.NewTheme?.GetBaseTheme() == BaseTheme.Dark;
            //}
            Snackbar = MainSnackbar;
            loginWindow.AccountAuthenticateEvent += new LogininWindow.ValuePassHandler(AccountAuthenticate);
        }

        private void AccountAuthenticate(object sender, AccountAuthenticateEventArgs arges)
        {
            switch (arges.Result)
            {
                case AuthenticateResult.Success:
                    viewModel.UserInfo.User = arges.User;
                    loginWindow.Close();
                    break;
                case AuthenticateResult.EmptyInput:
                    break;
                case AuthenticateResult.AccountNotExist:
                    MessageBox.Show("Account does not exist");
                    break;
                case AuthenticateResult.PasswordError:
                    MessageBox.Show("Invalid password");
                    break;
                default:
                    break;
            }
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

        //private void MenuDarkModeButton_Click(object sender, RoutedEventArgs e) 
        //    => ModifyTheme(DarkModeToggleButton.IsChecked == true);

        private static void ModifyTheme(bool isDarkTheme)
        {
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();

            theme.SetBaseTheme(isDarkTheme ? Theme.Dark : Theme.Light);
            paletteHelper.SetTheme(theme);
        }

        private void PanelidLableMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string text = (sender as Label).Content.ToString();
            Clipboard.SetDataObject(text);
            e.Handled = true;
        }

        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            if (!viewModel.UserInfo.UserExist)
            {
                loginWindow = new();
                loginWindow.AccountAuthenticateEvent += new LogininWindow.ValuePassHandler(AccountAuthenticate);
                loginWindow.ShowDialog();
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

        private void DefectJudge(object sender, DefectJudgeArgs e)
        {
            Defect defect = e.Defect;
            viewModel.InspImageView.DefectJudge(defect, viewModel.UserInfo.User);
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
    }
}
