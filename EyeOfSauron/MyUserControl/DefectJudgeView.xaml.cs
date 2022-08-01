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

        public event RoutedEventHandler? DefectJudgedEvent;

        private readonly DefectJudgeViewModel viewModel;      

        public DefectJudgeView()
        {
            viewModel = new();
            DataContext = viewModel;
            InitializeComponent();
        }

        private void JudgeButtonClick(object sender, RoutedEventArgs e)
        {
            Defect? defect;
            Button button = (Button)sender;
            if (button.Content.ToString() == "S")
            {
                defect = null;
            }
            else if (button.Content.ToString() == "E")
            {
                defect = Defect.GetDefectByCode("DE00002");
            }
            else
            {
                Defect? buffer = button.DataContext as Defect;
                defect = Defect.GetDefectByCode(buffer?.DefectCode);
            }
            DefectJudgedEvent?.Invoke(defect, new());
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
