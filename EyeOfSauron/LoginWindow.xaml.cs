using System;
using System.Windows;
using CoreClass.Model;
using MaterialDesignThemes.Wpf;
using EyeOfSauron.MyUserControl;
using EyeOfSauron.ViewModel;
using System.Net;

namespace EyeOfSauron
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LogininWindow : Window
    {
        public event RoutedEventHandler? UserAuthenticateEvent;
        private readonly UserInfoViewModel _viewModel;
        private readonly MessageAcceptDialog messageAcceptDialog = new();

        public LogininWindow()
        {
            _viewModel = new UserInfoViewModel();
            DataContext = _viewModel;
            InitializeComponent();
            LoginWindowDialogHost.DialogClosing += new DialogClosingEventHandler(DialogClosing);
            
        }
        
        private void LoginButtonClick(object sender, RoutedEventArgs e)
        {
            var authenticateResult = _viewModel.Authenticate(userNameTextBox.Text, passwordTextBox.Password);
            switch (authenticateResult)
            {
                case AuthenticateResult.Success:
                    UserAuthenticateEvent?.Invoke(_viewModel.User, new());
                    break;
                case AuthenticateResult.EmptyInput:
                    break;
                case AuthenticateResult.AccountNotExist:
                    messageAcceptDialog.Message.Text = "用户不存在";
                    DialogHost.Show(messageAcceptDialog, "LoginWindowDialogHost");
                    break;
                case AuthenticateResult.PasswordError:
                    messageAcceptDialog.Message.Text = "密码错误";
                    DialogHost.Show(messageAcceptDialog, "LoginWindowDialogHost");
                    break;
                default:
                    break;
            }
        }

        private void DialogClosing(object sender, DialogClosingEventArgs e)
        {
            var a = (DialogHost)sender;
            var b = a.DialogContent;
            if(b is MessageAcceptDialog)
            {
                if (!Equals(e.Parameter, true))
                {
                    e.Cancel();
                }
            }
            else
            {
                e.Cancel();
            }
        }
    }
}
