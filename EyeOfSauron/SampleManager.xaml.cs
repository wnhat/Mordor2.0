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
using System.Windows.Shapes;
using System.Diagnostics;
using EyeOfSauron.ViewModel;
using MaterialDesignThemes.Wpf;
using System.Windows.Controls.Primitives;
using EyeOfSauron.MyUserControl;

namespace EyeOfSauron
{
    /// <summary>
    /// Interaction logic for SampleManager.xaml
    /// </summary>
    public partial class SampleManager : Window
    {
        private readonly SampleManagerViewModel _viewModel;
        
        public SampleManager()
        {
            InitializeComponent();
            _viewModel = new();
            DataContext = _viewModel;
            ResultPanelList.PanelList.SelectionChanged += new SelectionChangedEventHandler(ListView_SelectionChanged);
            ResultPanelList.PanelListViewDialog.DialogClosing += new DialogClosingEventHandler(AcceptCancelDialog_OnDialogClosing);
            MainSnackbar.MessageQueue?.Enqueue("Welcome Login to Eye of Sauron");
        }

        private void ColorToolToggleButton_OnClick(object sender, RoutedEventArgs e)
            => ImageView.Focus();

        private void PanelidLableMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string? text = ((Label)sender).Content.ToString();
            Clipboard.SetDataObject(text);
            MainSnackbar.MessageQueue?.Enqueue("Copy Successfully");
        }

        private void AcceptCancelDialog_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (!Equals(eventArgs.Parameter, true))
            {
                ResultPanelList.InputTextBox.Clear();
                return;
            }
            if (!string.IsNullOrWhiteSpace(ResultPanelList.InputTextBox.Text))
            {
                List<string> lines = new();
                int lineCount = ResultPanelList.InputTextBox.LineCount;
                for (int line = 0; line < lineCount; line++)
                {
                    lines.Add(ResultPanelList.InputTextBox.GetLineText(line).Trim());
                }
                ResultPanelList.InputTextBox.Clear();
                foreach (string item in lines)
                {
                    ResultPanelList.viewModel.PanelList.Add(new PanelSampleContainer(item));
                }
            }
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
