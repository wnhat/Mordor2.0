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
        public delegate void ValuePassHandler(object sender, AccountAuthenticateEventArgs e);
        public event ValuePassHandler? AccountAuthenticateEvent ;

        private readonly UserInfoViewModel _viewModel;
        public bool IsClosed { get; set; }

        public LogininWindow()
        {
            _viewModel = new UserInfoViewModel();
            DataContext = _viewModel;
            InitializeComponent();
            IsClosed = false;
        }
        
        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            AuthenticateResult authenticateResult = _viewModel.Authenticate(userNameTextBox.Text, passwordTextBox.Password);
            AccountAuthenticateEventArgs arges = new(authenticateResult, _viewModel);
            AccountAuthenticateEvent?.Invoke(this, arges);
        }

        private void LoginWindow_Closed(object sender, EventArgs e)
        {
            IsClosed = true;
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
