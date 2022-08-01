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
using CoreClass.Model;
using CoreClass.Service;
using EyeOfSauron.ViewModel;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System.Windows.Threading;
using CoreClass;
using MaterialDesignThemes.Wpf;

namespace EyeOfSauron.MyUserControl
{
    /// <summary>
    /// Interaction logic for PanelIdInput.xaml
    /// </summary>
    public partial class SamplePanelListView : UserControl
    {
        public readonly SamplePanelListViewModel viewModel;

        public SamplePanelListView()
        {
            InitializeComponent();
            viewModel = new();
            DataContext = viewModel;
            viewModel.PanelList = new();
            viewModel.CollectionName = string.Empty;
            viewModel.SelectedItem = null;
        }

        private void Updata(object sender, RoutedEventArgs e)
        {
            //if (viewModel.SelectedItem != null)
            //{
            //    var filter = Builders<PanelSample>.Filter.Eq("PanelID", "712B260002C2ABE09");
            //    var panelSample = PanelSample.Collection.Find(filter).First();
            //    var id = panelSample.Id;
            //    var update = Builders<PanelSample>.Update.Set(x => x.LastModifyTime, DateTime.Now).Set("MissionCollection.CollectionName", "NoteUpdataTest");
            //    PanelSample.Collection.UpdateOne(filter, update);
            //}
            //else
            //{
            //    DialogHost.Show(new MessageAcceptDialog { Message = { Text = "未选定任何任务集" } }, "CollectionSettingViewDialog");
            //}
        }
    }
}
