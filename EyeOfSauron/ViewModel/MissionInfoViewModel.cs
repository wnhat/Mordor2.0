using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Linq;
using CoreClass.Model;

namespace EyeOfSauron.ViewModel
{
    public class MissionInfoViewModel : ViewModelBase
    {
        public MissionInfoViewModel()
        {
            DetailDefectList = new();
            InspImage = new();
        }
        
        private string panelId = "";
        
        private ProductInfo? productInfo;

        private int remainingCount;

        public string PanelId
        {
            get => panelId;
            set => SetProperty(ref panelId, value);
        }

        public int RemainingCount
        {
            get => remainingCount;
            set => SetProperty(ref remainingCount, value);
        }

        public ProductInfo ProductInfo
        {
            get => productInfo;
            set => SetProperty(ref productInfo, value);
        }

        public DefectListViewModel DetailDefectList { get; }

        public InspImageViewModel InspImage { get; }
    }

    public class InspImageViewModel : ViewModelBase
    {
        
        
        public int refreshPage = 0;

        public bool isVisible = false;

        private BitmapImage defaultImage = new();

        private BitmapImageContainer? defectMapImage;

        static readonly Uri _defaultImageUri = new(@"D:\DICS Software\DefaultSample\AVI\Orign\DefaultSample\00_DUST_CAM00.bmp", UriKind.Absolute);

        private ObservableCollection<BitmapImageContainer>? inspImages;

        private ImageContainer[] inspImageArray1 = new ImageContainer[3];

        public List<ImageContainer> resultImageDataList = new();

        public List<ImageContainer> defectImageDataList = new();

        public InspImageViewModel()
        {
            defaultImage.BeginInit();
            defaultImage.UriSource = _defaultImageUri;
            defaultImage.EndInit();
            defaultImage.Freeze();
            inspImages = new ObservableCollection<BitmapImageContainer>
            {
                new BitmapImageContainer(new ImageContainer()),
                new BitmapImageContainer(new ImageContainer()),
                new BitmapImageContainer(new ImageContainer())
            };
            RefreshImageCommand = new(
                _ => RefreshImageMethod(),
                _ => resultImageDataList != null);
        }

        public bool ImageLableIsVisible
        {
            get => isVisible;
            set => SetProperty(ref isVisible, value);
        }

        public ImageContainer[] InspImageArray1
        {
            get => inspImageArray1;
            set => SetProperty(ref inspImageArray1, value);
        }

        public ObservableCollection<BitmapImageContainer> InspImages
        {
            get => inspImages;
            set => SetProperty(ref inspImages, value);
        }

        public BitmapImage DefaultImage
        {
            get => defaultImage;
            set => SetProperty(ref defaultImage, value);
        }

        public BitmapImageContainer DefectMapImage
        {
            get => defectMapImage;
            set => SetProperty(ref defectMapImage, value);
        }

        public CommandImplementation RefreshImageCommand { get; }
        
        public void RefreshImageMethod()
        {
            if (resultImageDataList.Count == 0)
            {
                return;
            }
            if (refreshPage < resultImageDataList.ToArray().Length)
            {
                for (int i = 0; i < InspImages.Count; i++)
                {
                    InspImages[i] = new BitmapImageContainer(resultImageDataList[refreshPage + i]);
                }
                refreshPage += 3;
            }
            else
            {
                refreshPage = 0;
                RefreshImageMethod();
            }
        }
    }

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
        private string defectNo = "";

        private string name = "";

        private BitmapImage detailDefectImage = new();

        public AetDetailDefect(string name, string defectNo)
        {
            Name = name;
            DefectNo = defectNo;
        }

        public AetDetailDefect(string name, string defectNo, BitmapImage bitmapImage)
        {
            Name = name;
            DefectNo = defectNo;
            DetailDefectImage = bitmapImage;
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
