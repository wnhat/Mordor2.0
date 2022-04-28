using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Linq;

namespace EyeOfSauron.ViewModel
{
    public class MissionInfoViewModel : ViewModelBase
    {
        private string panelId = "";

        public DefectListViewModel DetailDefectList { get; }

        public InspImageViewModel InspImage { get; }

        public MissionInfoViewModel()
        {
            DetailDefectList = new();
            InspImage = new();
        }

        public string PanelId
        {
            get => panelId;
            set => SetProperty(ref panelId, value);
        }
    }

    public class InspImageViewModel : ViewModelBase
    {
        private int refreshPage = 0;

        private BitmapImage _defaultImage = new();

        static readonly Uri _defaultImageUri = new(@"D:\DICS Software\DefaultSample\AVI\Orign\DefaultSample\00_DUST_CAM00.bmp", UriKind.Absolute);

        private KeyValuePair<string, BitmapImage>[] inspImageArray = new KeyValuePair<string, BitmapImage>[3];

        public Dictionary<string, BitmapImage> resultImageDataDic = new();

        public Dictionary<string, BitmapImage> defectImageDataDic = new();

        public InspImageViewModel()
        {
            _defaultImage.BeginInit();
            _defaultImage.UriSource = _defaultImageUri;
            _defaultImage.EndInit();
            //for (int i = 0; i < 3; i++)
            //{
            //    imageArray[i] = _defaultImage;
            //}
            RefreshImageCommand = new CommandImplementation(
                _ => RefreshImageMethod(),
                _ => resultImageDataDic != null); 
        }

        public BitmapImage DefaultImage
        {
            get => _defaultImage;
            set => SetProperty(ref _defaultImage, value);
        }

        public KeyValuePair<string, BitmapImage>[] InspImageArray
        {
            get => inspImageArray;
            set => SetProperty(ref inspImageArray, value);
        }


        //public BitmapImage[] ImageArray
        //{
        //    get => imageArray;
        //    set => SetProperty(ref imageArray, value);
        //}

        //public string[] ImageNameArray
        //{
        //    get => imageNameArray;
        //    set => SetProperty(ref imageNameArray, value);
        //}



        public CommandImplementation RefreshImageCommand { get; }
        
        private void RefreshImageMethod()
        {
            if ((refreshPage) * 3 < resultImageDataDic.Values.ToArray().Length)
            {
                inspImageArray = resultImageDataDic.ToArray().Skip((refreshPage) * 3).Take(3).ToArray();
                refreshPage++;
            }
            else
            {
                refreshPage = 0;
                RefreshImageMethod();
                
            }
        }
    }

    //This class shoudl be rewrite;
    public sealed class DefectListViewModel : ViewModelBase
    {
        private AetDetailDefect? selectedItem;

        private ObservableCollection<AetDetailDefect>? aetDetailDefects;

        public DefectListViewModel()
        {
            AetDetailDefects = new ObservableCollection<AetDetailDefect>
            {
                new AetDetailDefect("InnerDefect1","0001"),
                new AetDetailDefect("InnerDefect2","0002"),
            };
            
            SelectedItem = AetDetailDefects.FirstOrDefault();

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

        public AetDetailDefect? SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }

        public ObservableCollection<AetDetailDefect> AetDetailDefects
        {
            get => aetDetailDefects;
            set => SetProperty(ref aetDetailDefects, value);
        }

        public CommandImplementation SelectedItemChangedCommand { get; }

    }

    public sealed class AetDetailDefect : ViewModelBase
    {
        private string defectNo;

        private string name;

        private BitmapImage detailDefectImage = new();

        public AetDetailDefect(string name, string defectNo)
        {
            Name = name;
            DefectNo = defectNo;
        }

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public string DefectNo
        {
            get => defectNo;
            set => SetProperty(ref defectNo, value);
        }

        public BitmapImage DetailDefectImage
        {
            get => detailDefectImage;
            set => SetProperty(ref detailDefectImage, value);
        }
    }
}
