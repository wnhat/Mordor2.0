using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                _viewModel.Authenticate(userNameTextBox.Text, passwordTextBox.Password);
                Window window = new ProductSelectWindow(_viewModel);
                Hide();
                window.ShowDialog();
                Show();
            }
            catch(Exception exception)
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
