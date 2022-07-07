﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using CoreClass.Model;

namespace EyeOfSauron.ViewModel
{
    public class SamplePanelListViewModel : ViewModelBase
    {
        private string? collectionName ;
        private ObservableCollection<SamplePanelContainer> panelList = new();
        public string? CollectionName
        {
            get => collectionName;
            set => SetProperty(ref collectionName, value);
        }
        public ObservableCollection<SamplePanelContainer> PanelList
        {
            get => panelList;
            set => SetProperty(ref panelList, value);
        }

        private SamplePanelContainer? selectedItem;
        public SamplePanelContainer? SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }
        public SamplePanelListViewModel()
        {
            GetSamples();
        }
        public SamplePanelListViewModel(string collectionName)
        {
            GetSamples(collectionName);
        }

        public async void GetSamples(string collectionName = "")
        {
            List<PanelSample>? samples;
            if (collectionName == string.Empty)
            {
                samples = await PanelSample.GetSamples();
            }
            else
            {
                samples = await PanelSample.GetSamples(collectionName);
            }
            if (samples != null)
            {
                PanelList.Clear();
                foreach (var item in samples)
                {
                    PanelList.Add(new(item));
                }
                CollectionName = collectionName;
            }
        }
    }

    public class SamplePanelContainer: PanelMission
    {
        public SamplePanelContainer(PanelSample panelSample):base(panelSample.AetResult)
        {
            PanelSample = panelSample;
        }
        public PanelSample PanelSample { get; private set; }
    }
}
