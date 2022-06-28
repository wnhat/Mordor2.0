using System;
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
        private ObservableCollection<SamplePanelContainer> panelList = new();
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

        public static async void Get()
        {
            var Missions = await PanelSample.GetMissionCollection();
            List<string> MissionCollection = new();
            foreach (var item in Missions)
            {
                // Convert the first BsonElement in the item to ProductInfo;
                var mission = item.GetValue("collection").AsString;
                MissionCollection.Add(mission);
            }
        }

        public async void GetSamples()
        {
            var samples = await PanelSample.GetSamples();
            if(samples != null)
            {
                PanelList.Clear();
                foreach (var item in samples)
                {
                    PanelList.Add(new(item));
                }
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
