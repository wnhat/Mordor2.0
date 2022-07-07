using System.Windows.Controls;
using EyeOfSauron.ViewModel;
using CoreClass.Model;
using System.Windows;

namespace EyeOfSauron.MyUserControl
{
    /// <summary>
    /// Interaction logic for JudgeButtons.xaml
    /// </summary>
    public partial class DefectSelectView : UserControl
    {
        private readonly DefectJudgeViewModel viewModel;      

        public DefectSelectView()
        {
            viewModel = new();
            DataContext = viewModel;
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DefectSelectListBox.SelectedItems.Clear();
        }
    }
}
