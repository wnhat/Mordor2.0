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

        private void JudgeButtonClick(object sender, RoutedEventArgs e)
        {
            Defect defect;
            if ((sender as Button).Content == "S")
            {
                defect = null;
            }
            else if ((sender as Button).Content == "E")
            {
                defect = Defect.OperaterEjudge;
            }
            else
            {
                defect = (sender as Button).DataContext as Defect;
            }
        }
    }
}
