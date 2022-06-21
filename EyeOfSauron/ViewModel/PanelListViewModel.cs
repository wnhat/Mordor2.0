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
        public PanelSampleContainer selectedItem = new();
        public PanelSampleContainer SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }
    }
    public class PanelSampleContainer : ViewModelBase
    {
        private PanelMission? panelMission;
        private string? panelId;
        private DateTime? inspDate;
        public PanelSampleContainer()
        {

        }
        public PanelSampleContainer(string panelId)
        {
            PanelId = panelId; 
        }
        public PanelSampleContainer(PanelMission panelMission)
        {
            PanelId = panelMission.AetResult.PanelId;
            //UTC+8:00
            InspDate = panelMission.AetResult.history.InspDate.AddHours(8);
            PanelMission = panelMission;
        }            
        public string PanelId
        {
            get => panelId;
            set => SetProperty(ref panelId, value);
        }
        public DateTime? InspDate
        {
            get => inspDate;
            set => SetProperty(ref inspDate, value);
        }
        public PanelMission PanelMission
        {
            get => panelMission;
            set => SetProperty(ref panelMission, value);
        }
    }
}
