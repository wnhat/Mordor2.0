using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MongoDB.Driver;
using EyeOfSauron.ViewModel;
using CoreClass.Service;
using System.Windows.Threading;

namespace EyeOfSauron.MyUserControl
{
    /// <summary>
    /// Interaction logic for ProductSelectWindow.xaml
    /// </summary>
    public partial class ProductSelectView : UserControl
    {
        private readonly DispatcherTimer dispatcherTimer = new();

        DateTime progressStartTime = DateTime.Now;

        public readonly ProductViewModel _viewModel;

        static readonly DICSRemainInspectMissionService RemainService = new();

        public ProductSelectView()
        {
            _viewModel = new();
            DataContext = _viewModel;
            GetMissions();
            InitializeComponent();
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);
            dispatcherTimer.Tick += new EventHandler(RefreshProgressValueUpdate);
        }

        /// <summary>
        /// Get all missions from database to set viewmodel;
        /// </summary>
        public async void GetMissions()
        {
            // Get the remaining mission quantity to set viewmodel;
            _viewModel.ProductInfos.Clear();
            //Issue: Thread will be waiting here if DICSDB no connection;
            try
            {
                var remainMissionCount = await RemainService.GetRemainMissionCount();
                foreach (var item in remainMissionCount)
                {
                    // Convert the first BsonElement in the item to ProductInfo;
                    var productObjectId = item.GetValue("_id").AsObjectId;
                    var productInfo = new ProductInfoService().GetProductInfo(productObjectId).Result;
                    int count = item.GetValue("count").ToInt32();
                    _viewModel.ProductInfos.Add(new ProductCardViewModel(new(productInfo, count)));
                }
                if (_viewModel.ProductInfos.Count > 0)
                {
                    _viewModel.SelectedProductCardViewModel = _viewModel.ProductInfos.First();
                }
            }

            catch(TimeoutException e)
            {
                MessageBox.Show(e.Message);
            }
            catch (TypeInitializationException)
            {
                MessageBox.Show("DICSDB no connection");
            }
        }

        private void ProductSelectBuuttonClick(object sender, RoutedEventArgs e)
        {
            ProductCardViewModel viewModel = ((Button)sender).DataContext as ProductCardViewModel;
            _viewModel.SelectedProductCardViewModel = viewModel;
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.IsMissionFreshAllowable = false;
            dispatcherTimer.Start();
            progressStartTime = DateTime.Now;
            GetMissions();
            MainWindow.Snackbar.MessageQueue?.Enqueue("Mission Refresh Successfully");
        }

        private void RefreshProgressValueUpdate(object sender, EventArgs e)
        {
            if (!_viewModel.IsMissionFreshAllowable)
            {
                var totalDuration = progressStartTime.AddSeconds(3).Ticks - progressStartTime.Ticks;
                var currentDuration = DateTime.Now.Ticks - progressStartTime.Ticks;
                var autoCountdownPercentComplete = 100.0 / totalDuration * currentDuration;
                _viewModel.RefreshButtonProgressValue = autoCountdownPercentComplete;
            }
            if (_viewModel.RefreshButtonProgressValue > 100)
            {
                _viewModel.RefreshButtonProgressValue = 0;
                _viewModel.IsMissionFreshAllowable = true;
                dispatcherTimer.Stop();
            }
        }
    }
}
