using System;
using System.Windows;
using CoreClass.Model;
using MaterialDesignThemes.Wpf;
using EyeOfSauron.MyUserControl;
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

        public LogininWindow()
        {
            _viewModel = new UserInfoViewModel();
            DataContext = _viewModel;
            InitializeComponent();
        }
        
        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            var authenticateResult = _viewModel.Authenticate(userNameTextBox.Text, passwordTextBox.Password);
            switch (authenticateResult)
            {
                case AuthenticateResult.Success:
                    AccountAuthenticateEventArgs arges = new(_viewModel.User);
                    AccountAuthenticateEvent?.Invoke(this, arges);
                    break;
                case AuthenticateResult.EmptyInput:
                    break;
                case AuthenticateResult.AccountNotExist:
                    DialogHost.Show(new MessageAcceptDialog { Message = { Text = "用户不存在" } }, "LoginWindowDialogHost");
                    break;
                case AuthenticateResult.PasswordError:
                    DialogHost.Show(new MessageAcceptDialog { Message = { Text = "密码错误" } }, "LoginWindowDialogHost");
                    break;
                default:
                    break;
            }
        }
    }

    public class AccountAuthenticateEventArgs : EventArgs
    {
        public User User { get; set; }

        public AccountAuthenticateEventArgs(User user)
        {
            this.User = user;
        }
    }
}
