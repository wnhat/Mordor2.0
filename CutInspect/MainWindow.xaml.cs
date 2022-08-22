using CutInspect.Model;
using CutInspect.ViewModel;
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
        private readonly Timer? UpdateScrollTimer = null;//消失状态计时器
        private readonly Storyboard storyboard = new ();
        private readonly MainWindowViewModel _viewModel = new();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = _viewModel;
            _viewModel.BindItems.CollectionChanged += ListData_CollectionChanged;
            UpdateScrollTimer = new Timer(UpdateScrollTimerCallBack, null, 1000, Timeout.Infinite);
        }

        /// <summary>
        /// 数据集改变时加入动画
        /// </summary>
        private void ListData_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    this.listPic.UpdateLayout();
                    var listItem = this.listPic.ItemContainerGenerator.ContainerFromIndex(0) as ListViewItem;
                    var animation = new DoubleAnimation() { Duration = TimeSpan.FromMilliseconds(500), From = listItem.ActualWidth };
                    Storyboard.SetTarget(animation, listItem);
                    Storyboard.SetTargetProperty(animation, new PropertyPath("RenderTransform.X"));
                    storyboard.Children.Add(animation);
                    storyboard.Begin();
                }
                else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    storyboard.Children.RemoveAt(0);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// 定时器回调
        /// </summary>
        private void UpdateScrollTimerCallBack(object? sender)
        {
            this.Dispatcher.Invoke(() =>
            {
                if (_viewModel.BindItems.Count > 2)
                    _viewModel.BindItems.Remove(_viewModel.BindItems[1]);
                _viewModel.BindItems.Insert(0, new BitmapImageContainer(BitmapImageContainer.GetDefault));
            });
            if (UpdateScrollTimer != null)
                UpdateScrollTimer.Change(1000, Timeout.Infinite);
        }
    }
}
