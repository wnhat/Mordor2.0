using System;
using System.Windows;
using EyeOfSauron.ViewModel;


namespace EyeOfSauron
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LogininWindow : Window
    {
        private readonly UserInfoViewModel _viewModel;
        
        public LogininWindow()
        {
            _viewModel = new UserInfoViewModel();
            DataContext = _viewModel;
        }
        
        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
            _viewModel.Authenticate(userNameTextBox.Text, passwordTextBox.Password);
            MainWindow window = new(_viewModel);
            Hide();
            window.ShowDialog();
            Show();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
