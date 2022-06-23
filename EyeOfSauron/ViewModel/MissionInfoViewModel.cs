﻿using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Linq;
using CoreClass.Model;
using CoreClass;

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

        private BitmapImageContainer? defectMapImage;

        private ObservableCollection<BitmapImageContainer>? inspImages;

        public List<ImageContainer> resultImageDataList = new();

        public InspImageViewModel()
        {
            inspImages = new ObservableCollection<BitmapImageContainer>
            {
                new BitmapImageContainer(ImageContainer.GetDefult),
                new BitmapImageContainer(ImageContainer.GetDefult),
                new BitmapImageContainer(ImageContainer.GetDefult)
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

        public ObservableCollection<BitmapImageContainer> InspImages
        {
            get => inspImages;
            set => SetProperty(ref inspImages, value);
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

    public sealed class AetDetailDefect : ViewModelBase
    {
        private string defectNo = "";

        private string name = "";

        private BitmapImage detailDefectImage = new();

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
