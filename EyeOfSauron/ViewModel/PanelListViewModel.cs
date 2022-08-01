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
        private ObservableCollection<PanelViewContainer> panelList = new();
        private PanelViewContainer? selectedItem;
        public PanelListViewModel()
        {
            OpenPathCommand = new CommandImplementation(OpenPath);
        }

        public CommandImplementation OpenPathCommand { get; }
        public ObservableCollection<PanelViewContainer> PanelList
        {
            get => panelList;
            set => SetProperty(ref panelList, value);
        }
        public PanelViewContainer? SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }
        public void OpenPath(object o)
        {
            if (SelectedItem != null)
            {
                string path = (string)o;
                System.Diagnostics.Process.Start("explorer.exe", path);
            }
        }
    }
    public class PanelViewContainer
    {
        public PanelViewContainer(PanelMission panelMission)
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
