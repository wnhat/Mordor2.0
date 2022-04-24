using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;

namespace EyeOfSauron.ViewModel
{
    //This class shoudl be rewrite;
    public sealed class DefectListViewModel : ViewModelBase
    {
        private AetDetailDefect? _selectedItem;
        private DefectList? _defectList;
        private BitmapImage detailDefectImage = new BitmapImage();

        public DefectListViewModel()
        {
            DefectList = new DefectList();
            SelectedItemChangedCommand = new CommandImplementation(
                _ =>
                {
                    var aetDetailDefect = SelectedItem as AetDetailDefect;
                    if (aetDetailDefect != null)
                    {

                    }
                    else
                    {

                    }
                },
                _ => SelectedItem != null);
        }
        public BitmapImage DetailDefectImage
        {
            get => detailDefectImage;
            set => SetProperty(ref detailDefectImage, value);
        }
        public DefectList DefectList
        {
            get => _defectList;
            set => SetProperty(ref _defectList, value);
        }
        public AetDetailDefect? SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }
        public CommandImplementation SelectedItemChangedCommand { get; }

    }

    public sealed class AetDetailDefect : ViewModelBase
    {
        private string? _name;
        public AetDetailDefect(string name)
        {
            Name = name;
        }
        public string Name {
            get => _name;
            set => SetProperty(ref _name, value);
        }
    }


    public sealed class DefectList : ViewModelBase
    {
        private ObservableCollection<AetDetailDefect>? _aetDetailDefects;
        public ObservableCollection<AetDetailDefect> AetDetailDefects {
            get => _aetDetailDefects;
            set => SetProperty(ref _aetDetailDefects, value);
        }
        public DefectList()
        {
            AetDetailDefects = new ObservableCollection<AetDetailDefect>
            {
                new AetDetailDefect("InnerDefect1"),
                new AetDetailDefect("InnerDefect2"),
            };
        }
    }
}
