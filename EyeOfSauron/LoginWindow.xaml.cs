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

namespace EyeOfSauron
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LogininWindow : Window
    {
        public LogininWindow()
        {
        }
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                //userManager.Authenticate(userNameTextBox.Text, passwordTextBox.Password);
                Window window = new ProductSelectWindow();
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
