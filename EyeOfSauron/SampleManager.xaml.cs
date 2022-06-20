using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EyeOfSauron.ViewModel;
using MaterialDesignThemes.Wpf;
using EyeOfSauron.MyUserControl;
using System.Text.RegularExpressions;
using CoreClass.Model;

namespace EyeOfSauron
{
    /// <summary>
    /// Interaction logic for SampleManager.xaml
    /// </summary>
    public partial class SampleViewWindow : Window
    {
        private readonly SampleViewerViewModel _viewModel;
        
        public SampleViewWindow()
        {
            InitializeComponent();
            _viewModel = new();
            DataContext = _viewModel;
            ResultPanelList.PanelList.SelectionChanged += new SelectionChangedEventHandler(ListView_SelectionChanged);
            ResultPanelList.PanelListViewDialog.DialogClosing += new DialogClosingEventHandler(PanelListAcceptCancelDialog_OnDialogClosing);
            ResultPanelList.PanelListBoxClearButton.Click += new RoutedEventHandler(PanelListBoxClearButton_Click);
            MainSnackbar.MessageQueue?.Enqueue("Welcome to Eye of Sauron");
        }

        private void ColorToolToggleButton_OnClick(object sender, RoutedEventArgs e)
            => ImageView.Focus();

        private void PanelidLableMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string? text = ((Label)sender).Content.ToString();
            Clipboard.SetDataObject(text);
            MainSnackbar.MessageQueue?.Enqueue("复制成功");
        }

        private void PanelListAcceptCancelDialog_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
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
        //public void setSampleView(string panelId)
        //{
        //    string pattern = @"^\d+$";
        //    Regex rg = new(pattern, RegexOptions.Multiline | RegexOptions.Singleline);
        //    _ = rg.Match(ResultPanelList.InputTextBox.Text).Value;
        //    AETresult aETresult = AETresult.Get(panelId);
        //    PanelMission panelMission = new(aETresult);
        //}

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void PanelListBoxClearButton_Click(object sender, RoutedEventArgs e)
        {
            ResultPanelList.viewModel.PanelList.Clear();
            //await DialogHost.Show(new MessageAcceptCancelDialog { Message = { Text = "确认清除"} }, "PanelListViewDialog");
        }

        private void MessageAcceptCancelDialog_OnDialogClosing(object sender, DialogClosingEventArgs eventArgs)
        {
            if (Equals(eventArgs.Parameter, true))
            {
                ResultPanelList.viewModel.PanelList.Clear();
            }
        }
    }
}
