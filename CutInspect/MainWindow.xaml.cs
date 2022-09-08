using CutInspect.Model;
using CutInspect.ViewModel;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CutInspect
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainWindowViewModel _viewModel = new();
        private bool rect_MoveEnableFlag = false;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _viewModel;
        }

        private void ColorToolToggleButton_OnClick(object sender, RoutedEventArgs e) => MainContent.Focus();

        private void PanelidLableMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            string? text = ((Button)sender).Content.ToString();
            Clipboard.SetDataObject(text);
            MainSnackbar.MessageQueue?.Enqueue(string.Format("复制成功：{0}", text));
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.ShowSelectedPanelMission();
        }

        private void FinishedListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.ShowFinishedPanelMission();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MoveRect_MouseMove(object sender, MouseEventArgs e)
        {
            if (rect_MoveEnableFlag)
            {
                //获取鼠标指针到MoveRect的偏移量
                double deltaV = e.GetPosition(MoveRect).Y - MoveRect.Height / 2;
                double deltaH = e.GetPosition(MoveRect).X - MoveRect.Width / 2; ;
                //计算MoveRect新位置
                double newTop = deltaV + Canvas.GetTop(MoveRect);
                double newLeft = deltaH + Canvas.GetLeft(MoveRect);
                //左右边界处理，限制MoveRect 移动区域在SmallBox内
                newLeft = newLeft <= 0 ? 0 : (newLeft >= (SmallBox.Width - MoveRect.Width) ? (SmallBox.Width - MoveRect.Width) : newLeft);
                //上下边界处理，限制MoveRect 移动区域在SmallBox内
                newTop = newTop <= 0 ? 0 : (newTop >= (SmallBox.Height - MoveRect.Height) ? (SmallBox.Height - MoveRect.Height) : newTop);
                //设置MoveRect位置
                Canvas.SetTop(MoveRect, newTop);
                Canvas.SetLeft(MoveRect, newLeft);
                SetBigImgPos();
            }
        }

        private void SetBigImgPos()
        {
            //获取右侧BigImg与MoveRect的比例
            double n = BigBox.Width / MoveRect.Width;

            //获取MoveRect在左侧SmallImg中的位置
            double left = Canvas.GetLeft(MoveRect);
            double top = Canvas.GetTop(MoveRect);

            //计算和设置BigImg在右侧Canvas中的位置
            Canvas.SetLeft(BigImg, -left * n);
            Canvas.SetTop(BigImg, -top * n);
        }

        /// <summary>
        /// 控制MoveRect在鼠标进入SmallImg区域后显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SmallBox_MouseEnter(object sender, MouseEventArgs e)
        {
            MoveRect.Visibility = Visibility.Visible;
        }

        /// <summary>
        /// 控制MoveRect在鼠标移出SmallImg区域后隐藏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SmallBox_MouseLeave(object sender, MouseEventArgs e)
        {
            MoveRect.Visibility = Visibility.Hidden;
        }

        private void SmallBox_MouseDown(object sender, MouseButtonEventArgs e)
        {
            rect_MoveEnableFlag = true;
            MoveRect_MouseMove(sender, e);
        }

        private void SmallBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            rect_MoveEnableFlag = false;
        }

        private void EqpMissionList_SelectChanged(object sender, SelectionChangedEventArgs e)
        {
            _viewModel.ShowFirstPanelMission();
        }

        private void SmallBox_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if(e.Delta > 0)
            {
                _viewModel.MoveRectWidth = _viewModel.MoveRectWidth < 100 ? _viewModel.MoveRectWidth += 10 : 100;
            }
            else if(e.Delta < 0)
            {
                _viewModel.MoveRectWidth = _viewModel.MoveRectWidth>20 ? _viewModel.MoveRectWidth-=10 : 20;
            }
            rect_MoveEnableFlag = true;
            MoveRect_MouseMove(sender, e);
        }
    }
}
