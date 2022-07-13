using CoreClass.Model;
using EyeOfSauron.MyUserControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeOfSauron.ViewModel
{
    public class CollectionSettingViewModel : ViewModelBase
    {
        private ObservableCollection<PanelMissionCollectionInfo> panelMissionCollectionInfo = new();
        public ObservableCollection<PanelMissionCollectionInfo> PanelMissionCollectionInfo
        {
            get => panelMissionCollectionInfo;
            set => SetProperty(ref panelMissionCollectionInfo, value);
        }
    }

    public class PanelMissionCollectionInfo
    {
        public string MissionCollectionName { get; private set; }
        public int MissionCount { get; private set; }

        public PanelMissionCollectionInfo(string name, int count)
        {
            MissionCollectionName = name;
            MissionCount = count;
        }
    }
}
