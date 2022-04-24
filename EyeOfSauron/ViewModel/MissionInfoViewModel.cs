using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EyeOfSauron;

namespace EyeOfSauron.ViewModel
{
    public class MissionInfoViewModel : ViewModelBase
    {
        private string panelId;
        public DefectListViewModel DefectList { get; }
        public InspImageViewModel InspImage { get; }

        public MissionInfoViewModel()
        {
            DefectList = new();
            InspImage = new();
        }
        public string PanelId
        {
            get => panelId;
            set => SetProperty(ref panelId, value);
        }
    }
}
