using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Linq;
using CoreClass.Model;
using CoreClass;
using System.Windows.Controls;
using System.Windows;
using MaterialDesignThemes.Wpf;

namespace EyeOfSauron.ViewModel
{
    public class MissionInfoViewModel : ViewModelBase
    {
        public MissionInfoViewModel()
        {
            DetailDefectList = new();
            InspImage = new();
            LayoutPresets = new();
        }
        public LayoutPresets LayoutPresets { get;}

        private string panelId = "";
        
        private ProductInfo? productInfo;

        private UserControl extendedUserControl = new();

        private int remainingCount;

        public UserControl ExtendedUserControl
        {
            get => extendedUserControl;
            set => SetProperty(ref extendedUserControl, value);
        }

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

        public int pagePatternCount = 3;

        public bool isVisible = false;

        private BitmapImageContainer? defectMapImage;

        private ObservableCollection<BitmapImageContainer>? inspImages;

        private BitmapImageContainer defaultImage;

        public List<ImageContainer> resultImageDataList = new();

        private readonly PackIconKind[] imageGroupIconArray = 
            {
            PackIconKind.Numeric1BoxOutline, 
            PackIconKind.Numeric2BoxOutline, 
            PackIconKind.Numeric3BoxOutline, 
            PackIconKind.Numeric4BoxOutline, 
            PackIconKind.Numeric5BoxOutline, 
            PackIconKind.Numeric6BoxOutline, 
            PackIconKind.Numeric7BoxOutline,
            PackIconKind.Numeric8BoxOutline,
            PackIconKind.Numeric8BoxOutline,
            PackIconKind.Numeric9BoxOutline,
            PackIconKind.Numeric8BoxOutline,
            PackIconKind.Numeric8BoxOutline,
            PackIconKind.Numeric8BoxOutline,
        };

        public ObservableCollection<PackIconKind> imageGroupIcons = new();
        public PackIconKind? ImageGroup { get; set; }
        
        public List<ImageContainer> ResultImageDataList
        {
            get => resultImageDataList;
            set
            {
                SetProperty(ref resultImageDataList, value);
                int page = (int)Math.Ceiling((double)(resultImageDataList.Count / InspImages.Count));
                ImageGroupIcons.Clear();
                foreach(var item in imageGroupIconArray.Take(page))
                {
                    ImageGroupIcons.Add(item);
                }
            }
        }
        public ObservableCollection<PackIconKind> ImageGroupIcons
        {
            get => imageGroupIcons;
            set => SetProperty(ref imageGroupIcons, value);
        }
        public InspImageViewModel()
        {
            inspImages = new ObservableCollection<BitmapImageContainer>
            {
                new BitmapImageContainer(ImageContainer.GetDefault),
                new BitmapImageContainer(ImageContainer.GetDefault),
                new BitmapImageContainer(ImageContainer.GetDefault)
            };
            defaultImage = new BitmapImageContainer(ImageContainer.GetDefault);
            RefreshImageCommand = new(
                _ => RefreshImageMethod(),
                _ => resultImageDataList != null);
            PreOneImageCommand = new(
                _ =>
                {
                    refreshPage -= pagePatternCount;
                    refreshPage -= pagePatternCount;
                    RefreshImageMethod();
                },
                _ => refreshPage > InspImages.Count);
            NextOneImageCommand =new(
                _ =>
                {
                    RefreshImageMethod();
                },
                _ => (refreshPage + InspImages.Count) <= resultImageDataList.ToArray().Length);
        }

        public bool ImageLableIsVisible
        {
            get => isVisible;
            set => SetProperty(ref isVisible, value);
        }

        public ObservableCollection<BitmapImageContainer> InspImages
        {
            get => inspImages;
            set => SetProperty(ref inspImages, value);
        }

        public BitmapImageContainer DefaultImage
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
        public CommandImplementation PreOneImageCommand { get; }
        public CommandImplementation NextOneImageCommand { get; }

        public void RefreshImageMethod()
        {
            if (resultImageDataList.Count == 0)
            {
                return;
            }
            if ((refreshPage + InspImages.Count) <= resultImageDataList.ToArray().Length)
            {
                for (int i = 0; i < InspImages.Count; i++)
                {
                    InspImages[i] = new BitmapImageContainer(resultImageDataList[refreshPage + i]);
                }
                refreshPage += pagePatternCount;
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
            AetDetailDefects = new ObservableCollection<AetDetailDefect>{ };
            SelectedItem = AetDetailDefects.FirstOrDefault();
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
    }

    public sealed class AetDetailDefect
    {
        public AetDetailDefect(DefectInfo defectInfo, BitmapImage bitmapImage)
        {
            DefectInfo = defectInfo;
            DetailDefectImage = bitmapImage;
        }
        public DefectInfo DefectInfo { get; private set; }
        public BitmapImage DetailDefectImage { get; private set; }
    }

    public class LayoutPresets : ViewModelBase
    {
        private int imageBox1_RowSpan = 1;
        private int imageBox1_Height = 1000;
        private Visibility imageBox2_Visibility = Visibility.Visible;
        private Visibility imageBox3_Visibility = Visibility.Visible;
        private Orientation stackPanel1_Orientation = Orientation.Vertical;
        public int ImageBox1_RowSpan
        {
            get => imageBox1_RowSpan;
            set => SetProperty(ref imageBox1_RowSpan, value>=2?2:1);
        }
        public int ImageBox1_Height
        {
            get => imageBox1_Height;
            set => SetProperty(ref imageBox1_Height, value>1000? value : 1000);
        }
        public Visibility ImageBox2_Visibility
        {
            get => imageBox2_Visibility;
            set => SetProperty(ref imageBox2_Visibility, value == Visibility.Collapsed? Visibility.Collapsed: Visibility.Visible);
        }
        public Visibility ImageBox3_Visibility
        {
            get => imageBox3_Visibility;
            set => SetProperty(ref imageBox3_Visibility, value == Visibility.Collapsed ? Visibility.Collapsed : Visibility.Visible);
        }
        public Orientation StackPanel1_Orientation
        {
            get => stackPanel1_Orientation;
            set => SetProperty(ref stackPanel1_Orientation, value == Orientation.Horizontal ? Orientation.Horizontal : Orientation.Vertical);
        }
        public void SetInspImageLayout(int imageCount)
        {
            switch (imageCount)
            {
                default:
                case 3:
                    imageBox1_RowSpan = 1;
                    ImageBox1_Height = 1000;
                    ImageBox2_Visibility = Visibility.Visible;
                    ImageBox3_Visibility = Visibility.Visible;
                    StackPanel1_Orientation = Orientation.Vertical;
                    break;
                case 2:
                    imageBox1_RowSpan = 1;
                    ImageBox1_Height = 1500;
                    ImageBox2_Visibility = Visibility.Visible;
                    ImageBox3_Visibility = Visibility.Collapsed;
                    StackPanel1_Orientation = Orientation.Horizontal;
                    break;
                case 1:
                    imageBox1_RowSpan = 2;
                    ImageBox1_Height = 1800;
                    ImageBox2_Visibility = Visibility.Collapsed;
                    ImageBox3_Visibility = Visibility.Collapsed;
                    StackPanel1_Orientation = Orientation.Vertical;
                    break;
            }
        }
    }
}
