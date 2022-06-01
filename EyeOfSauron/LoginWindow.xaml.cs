using System;
using System.Windows;
using EyeOfSauron.ViewModel;
using CoreClass.Model;


namespace EyeOfSauron
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LogininWindow : Window
    {
        public delegate void ValuePassHandler(object sender, AccountAuthenticateEventArgs e);
        public event ValuePassHandler? AccountAuthenticateEvent ;
        private readonly UserInfoViewModel _viewModel;

        public LogininWindow()
        {
            _viewModel = new UserInfoViewModel();
            DataContext = _viewModel;
            InitializeComponent();
        }
        
        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            AuthenticateResult authenticateResult = _viewModel.Authenticate(userNameTextBox.Text, passwordTextBox.Password);
            AccountAuthenticateEventArgs arges = new(authenticateResult, _viewModel.User);
            AccountAuthenticateEvent?.Invoke(this, arges);
        }
    }

    public class AccountAuthenticateEventArgs : EventArgs
    {
        public AuthenticateResult Result { get; set; }
        public User User { get; set; }

        public AccountAuthenticateEventArgs(AuthenticateResult result, User user)
        {
            Result = result;
            this.User = user;
        }
    }
}
