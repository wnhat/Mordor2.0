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
using MaterialDesignThemes.Wpf;

namespace EyeOfSauron.MyUserControl
{
    /// <summary>
    /// Interaction logic for PanelIdInput.xaml
    /// </summary>
    public partial class PanelListView : UserControl
    {
        public readonly PanelListViewModel viewModel;

        public PanelListView()
        {
            InitializeComponent();
            viewModel = new();
            DataContext = viewModel;
        }

        private void InputTextBoxClear(object sender, RoutedEventArgs e)
        {
            InputTextBox.Clear();
        }
    }
}
