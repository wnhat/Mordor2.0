using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace EyeOfSauron.ViewModel
{
    public class PanelListViewModel : ViewModelBase
    {
        private ObservableCollection<PanelSampleContainer> panelList = new();
        public ObservableCollection<PanelSampleContainer> PanelList
        {
            get => panelList;
            set => SetProperty(ref panelList, value);
        }

        private PanelSampleContainer? selectedItem;
        public PanelSampleContainer? SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }
    }
    public class PanelSampleContainer
    {
        public PanelSampleContainer(PanelMission panelMission)
        {
            PanelId = panelMission.AetResult.PanelId;
            //UTC+8:00
            InspDate = panelMission.AetResult.history.InspDate.AddHours(8);
            PanelMission = panelMission;
        }            
        public string PanelId { get; private set; }
        public DateTime InspDate { get; private set; }
        public PanelMission PanelMission { get; private set; }
    }
}
