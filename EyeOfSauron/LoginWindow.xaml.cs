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
        public delegate void AccountAuthenticateHandler(object sender, AccountAuthenticateEventArgs e);
        public event AccountAuthenticateHandler? AccountAuthenticateEvent ;

        private readonly UserInfoViewModel _viewModel;
        
        public LogininWindow()
        {
            _viewModel = new UserInfoViewModel();
            DataContext = _viewModel;
        }
        
        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            AuthenticateResult authenticateResult = _viewModel.Authenticate(userNameTextBox.Text, passwordTextBox.Password);
            AccountAuthenticateEventArgs arges = new(authenticateResult, _viewModel);
            AccountAuthenticateEvent(this, arges);
            //switch (authenticateResult)
            //{
            //    case AuthenticateResult.Success:
            //        MainWindow window = new(_viewModel);
            //        Hide();
            //        window.ShowDialog();
            //        Show();
            //        break;
            //    case AuthenticateResult.EmptyInput:
            //        break;
            //    case AuthenticateResult.AccountNotExist:
            //        MessageBox.Show("Account does not exist");
            //        break;
            //    case AuthenticateResult.PasswordError:
            //        MessageBox.Show("Invalid password");
            //        break;
            //    default:
            //        break;
            //} 
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }

    public class AccountAuthenticateEventArgs : EventArgs
    {
        public AccountAuthenticateEventArgs(AuthenticateResult result, UserInfoViewModel userInfoViewModel)
        {
            Result = result;
            UserInfoViewModel = userInfoViewModel;
        }

        public AuthenticateResult Result { get; set; }
        public UserInfoViewModel UserInfoViewModel { get; set; }
    }


}
