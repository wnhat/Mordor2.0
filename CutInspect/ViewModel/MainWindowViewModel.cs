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
using CutInspect.MyUserControl;

namespace CutInspect.ViewModel
{
    public class MainWindowViewModel:ViewModelBase
    {
        private ObservableCollection<BitmapImageContainer> bindItems = new();
        private DateTime dateTime;
        private ColorTool colorTool = new();

        public  ObservableCollection<BitmapImageContainer> BindItems
        {
            get => bindItems;
            set => SetProperty(ref bindItems, value);
        }
        public DateTime DateTime
        {
            get => dateTime;
            set => SetProperty(ref dateTime, value);
        }
        public ColorTool ColorTool
        {
            get => colorTool;
            set => SetProperty(ref colorTool, value);
        }
        public CommandImplementation AddPicCommand { get;}
        public MainWindowViewModel()
        {
            bindItems.Add(new BitmapImageContainer(BitmapImageContainer.GetDefault));
            AddPicCommand = new(_ => bindItems.Add(new BitmapImageContainer(BitmapImageContainer.GetDefault)));
        }
    }
}
