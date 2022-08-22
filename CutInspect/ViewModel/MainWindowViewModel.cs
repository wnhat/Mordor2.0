using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using CoreClass.Model;
using CutInspect.Model;

namespace CutInspect.ViewModel
{
    public class MainWindowViewModel:ViewModelBase
    {
        private ObservableCollection<BitmapImageContainer> bindItems = new();

        public  ObservableCollection<BitmapImageContainer> BindItems
        {
            get => bindItems;
            set => SetProperty(ref bindItems, value);
        }
        public CommandImplementation AddPicCommand { get;}
        public MainWindowViewModel()
        {
            bindItems.Add(new BitmapImageContainer(BitmapImageContainer.GetDefault));
            AddPicCommand = new(_ => bindItems.Add(new BitmapImageContainer(BitmapImageContainer.GetDefault)));
        }
    }
}
