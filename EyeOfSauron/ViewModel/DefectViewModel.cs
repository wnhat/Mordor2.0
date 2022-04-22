using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace EyeOfSauron.ViewModel
{
    //This class shoudl be rewrite;
    public sealed class DefectListViewModel : ViewModelBase
    {
        private object? _selectedItem;
        private BitmapImage detailDefectImage = new BitmapImage();

        public DefectList defectList { get; }

        public object? SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }

        public DefectListViewModel()
        {
            defectList = new DefectList();
        }
        public BitmapImage DetailDefectImage
        {
            get => detailDefectImage;
            set => SetProperty(ref detailDefectImage, value);
        }
    }
    public sealed class AetDetailDefect
    {
        public AetDetailDefect(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
    public sealed class DefectList : ViewModelBase
    {
        public ObservableCollection<AetDetailDefect> list { get; }
        public DefectList()
        {
            list = new ObservableCollection<AetDetailDefect>
            {
                new AetDetailDefect("InnerDefect1"),
                new AetDetailDefect("InnerDefect2"),
            };
        }
    }
}
