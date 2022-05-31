using System.Windows.Controls;
using EyeOfSauron.ViewModel;
using CoreClass.Model;
using System.Windows;

namespace EyeOfSauron.MyUserControl
{
    /// <summary>
    /// Interaction logic for JudgeButtons.xaml
    /// </summary>
    public partial class DefectJudgeView : UserControl
    {
        public delegate void ValuePassHandler(object sender, DefectJudgeArgs e);
        
        public event ValuePassHandler? DefectJudgeEvent;

        private readonly DefectJudgeViewModel viewModel;      

        public DefectJudgeView()
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
            DefectJudgeArgs defectJudgeArgs = new(defect);
            DefectJudgeEvent?.Invoke(this, defectJudgeArgs);
        }
    }

    public class DefectJudgeArgs : RoutedEventArgs
    {
        public DefectJudgeArgs(Defect defect)
        {
            Defect = defect;
        }

        public Defect Defect { get; }
    }
}
