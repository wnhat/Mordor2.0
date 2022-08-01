using EyeOfSauron.MyUserControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeOfSauron.ViewModel
{
    public class AddToCollectionViewModel:ViewModelBase
    {
        private DefectSelectView defectSelectView = new();
        private ObservableCollection<PanelViewContainer> panelMissions = new();
        private string panelId = string.Empty;
        private string noteString = string.Empty;

        public DefectSelectView DefectSelectView
        {
            get => defectSelectView;
            set => SetProperty(ref defectSelectView, value);
        }
        public ObservableCollection<PanelViewContainer> PanelMissions
        {
            get => panelMissions;
            set => SetProperty(ref panelMissions, value);
        }
        public string PanelId
        {
            get => panelId;
            set => SetProperty(ref panelId, value);
        }
        public string NoteString
        {
            get => noteString;
            set => SetProperty(ref noteString, value);
        }
    }
}
