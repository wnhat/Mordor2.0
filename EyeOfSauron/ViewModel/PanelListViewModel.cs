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
    public class PanelSampleContainer
    {
        public PanelSampleContainer() { }
        public PanelSampleContainer(string s)
        {
            PanelId = s;
        }
        public string? PanelId
        {
            get ;
            set ;
        }
    }
}
