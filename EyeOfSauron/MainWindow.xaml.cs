using System;
using System.Diagnostics;
using EyeOfSauron.ViewModel;
using MaterialDesignThemes.Wpf;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Controls;
using EyeOfSauron.MyUserControl;
using EyeOfSauron.ViewModel;

namespace EyeOfSauron
{
    public partial class MainWindow : Window
    {
        public static Snackbar Snackbar = new();
        private MainWindowViewModel? viewModel;
        private readonly LogininWindow logininWindow = new();
        public MainWindow()
        {


            InitializeComponent();

            Task.Factory.StartNew(() => Thread.Sleep(1000)).ContinueWith(t =>
            {
                //note you can use the message queue from any thread, but just for the demo here we 
                //need to get the message queue from the snackbar, so need to be on the dispatcher
                MainSnackbar.MessageQueue?.Enqueue("Welcome Login to Eye of Sauron");
            }, TaskScheduler.FromCurrentSynchronizationContext());

            var paletteHelper = new PaletteHelper();

            var theme = paletteHelper.GetTheme();

            DarkModeToggleButton.IsChecked = theme.GetBaseTheme() == BaseTheme.Dark;

            if (paletteHelper.GetThemeManager() is { } themeManager)
            {
                themeManager.ThemeChanged += (_, e)
                    => DarkModeToggleButton.IsChecked = e.NewTheme?.GetBaseTheme() == BaseTheme.Dark;
            }

            Snackbar = MainSnackbar;
        }



        private void AccountAuthenticate(object sender, AccountAuthenticateEventArgs arges)
        {
            switch (arges.Result)
            {
                case AuthenticateResult.Success:
                    viewModel = new(arges.UserInfoViewModel);
                    DataContext = viewModel;
                    logininWindow.Hide();
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

        private void MenuToggleButton_OnClick(object sender, RoutedEventArgs e) 
            => MainScrollViewer.Focus();

        private void MenuDarkModeButton_Click(object sender, RoutedEventArgs e) 
            => ModifyTheme(DarkModeToggleButton.IsChecked == true);

        private static void ModifyTheme(bool isDarkTheme)
        {
            var paletteHelper = new PaletteHelper();
            var theme = paletteHelper.GetTheme();

            theme.SetBaseTheme(isDarkTheme ? Theme.Dark : Theme.Light);
            paletteHelper.SetTheme(theme);
        }

        private void StartInspButtunClick(object sender, RoutedEventArgs e)
        {
            viewModel.SetInspView();
            var productInfo = viewModel.ProductSelectView._viewModel.SelectedProductCardViewModel.ProductInfo.Key;
            Mission mission = new(productInfo);
            viewModel.InspImageView.SetMission(mission);
        }

        private void PanelidLableMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string text = (sender as Label).Content.ToString();
            Clipboard.SetDataObject(text);
            e.Handled = true;
        }

        private void EndInspButtonClick(object sender, RoutedEventArgs e)
        {
            viewModel.ProductSelectView.GetMissions();
            viewModel.SetProductSelectView();
        }

        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            logininWindow.AccountAuthenticateEvent += new LogininWindow.AccountAuthenticateHandler(AccountAuthenticate);
            logininWindow.ShowDialog();
        }
    }
}
