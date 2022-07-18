﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using CoreClass.Model;

namespace EyeOfSauron.ViewModel
{

    public class ProductViewModel : ViewModelBase
    {
        private ObservableCollection<ProductCardViewModel> productCardViewModels = new();

        private ProductCardViewModel? selectedProductCardViewModel;

        private ObservableCollection<ExamMissionWIP> examMissionCardViewModels = new();

        private ExamMissionWIP? selectedExamMissionCardViewModel;

        private int controlTabSelectedIndex = (int)ControlTableItem.ProductMission;

        private double refreshButtonProgressValue;

        private bool isMissionFreshAllowable = true;

        public ProductViewModel() { }

        public double RefreshButtonProgressValue
        {
            get => refreshButtonProgressValue;
            set => SetProperty(ref refreshButtonProgressValue, value);
        }

        public bool IsMissionFreshAllowable
        {
            get => isMissionFreshAllowable;
            set => SetProperty(ref isMissionFreshAllowable, value);
        }

        public ProductCardViewModel? SelectedProductCardViewModel
        {
            get => selectedProductCardViewModel;
            set => SetProperty(ref selectedProductCardViewModel, value);
        }
  
        public ObservableCollection<ProductCardViewModel> ProductInfos
        {
            get => productCardViewModels;
            set => SetProperty(ref productCardViewModels, value);
        }

        public ExamMissionWIP? SelectedExamMissionCardViewModel
        {
            get => selectedExamMissionCardViewModel;
            set => SetProperty(ref selectedExamMissionCardViewModel, value);
        }
        public ObservableCollection<ExamMissionWIP> ExamMissionCardViewModels
        {
            get => examMissionCardViewModels;
            set => SetProperty(ref examMissionCardViewModels, value);
        }
        public int ControlTabSelectedIndex
        {
            get => controlTabSelectedIndex;
            set => SetProperty(ref controlTabSelectedIndex, value);
        }
    }

    public class ProductCardViewModel : ViewModelBase
    {
        private KeyValuePair<ProductInfo, int> productInfo;

        public ProductCardViewModel(KeyValuePair<ProductInfo, int> productInfo)
        {
            ProductInfo = productInfo;
        }

        public KeyValuePair<ProductInfo, int> ProductInfo
        {
            get => productInfo;
            set => SetProperty(ref productInfo, value);
        }
    }
    public enum ControlTableItem
    {
        ProductMission = 0,
        ExamMission = 1,
    }
}
