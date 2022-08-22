﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using CoreClass.Model;
using MaterialDesignThemes.Wpf;
using EyeOfSauron.MyUserControl;

namespace EyeOfSauron.ViewModel
{
    public class SamplePanelListViewModel : ViewModelBase
    {
        private string collectionName = string.Empty;
        private ObservableCollection<SamplePanelContainer> panelList = new();

        public string CollectionName
        {
            get => collectionName;
            set => SetProperty(ref collectionName, value);
        }
        public ObservableCollection<SamplePanelContainer> PanelList
        {
            get => panelList;
            set => SetProperty(ref panelList, value);
        }
        public CommandImplementation ItemDeleteCommand
        {
            get;
        }
        public CommandImplementation ItemUpdateCommand
        {
            get;
        }

        private SamplePanelContainer? selectedItem;
        public SamplePanelContainer? SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }
        public SamplePanelListViewModel(string collectionName = "")
        {
            GetSamples(collectionName);
            ItemDeleteCommand = new(_ => { ItemDelete(); }, _ => SelectedItem != null) ;
            ItemUpdateCommand = new(_ => { ItemUpdate(); }, _ => SelectedItem != null);
        }

        public async void GetSamples(string collectionName)
        {
            List<PanelSample>? samples;
            if (collectionName == string.Empty)
            {
                //samples = await PanelSample.GetSamples();
                samples = new List<PanelSample>();
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
        private void ItemDelete()
        {
            //if (ItemDeleteCommand_CanExec())
            //{
                PanelSample.DeleteInfo(SelectedItem.PanelSample.Id);
                PanelList.Remove(SelectedItem);
            //}
        }

        private bool ItemDeleteCommand_CanExec()
        {
            //ISSUE
            var diaResult = DialogHost.Show(new MessageAcceptCancelDialog { Message = { Text = string.Format("确认删除样本：{0}", SelectedItem.PanelSample.PanelID) } }, "SamplePanelListDialog").Result;
            return (bool)(diaResult is bool? diaResult:false);
        }

        private void ItemUpdate()
        {
            DialogHost.Show(new MessageAcceptCancelDialog(), "SamplePanelListDialog");
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
