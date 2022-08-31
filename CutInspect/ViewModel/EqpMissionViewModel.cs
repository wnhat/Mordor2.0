using CutInspect.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CutInspect.ViewModel
{
    public class EqpMissionViewModel:ViewModelBase,IComparable<EqpMissionViewModel>
    {
        private string? eqpName = "";
        private int totalCount = 0;
        private int checkedMissionCount = 0;
        //public event EventHandler? PanelMissionFinishedEvent;
        private readonly Queue<InspectItem>? missionQueue = new();
        private ObservableCollection<InspectItem> panelItemOBcollection = new();
        public ObservableCollection<PanelMission>? panelMissionOBCollection = new();
        private InspectItem? selectMission;
        private PanelMission? selectPanelMission;
        public string? EqpName
        {
            get => eqpName;
            set => SetProperty(ref eqpName, value);
        }
        public int TotalCount
        {
            get => totalCount;
            set => SetProperty(ref totalCount, value);
        }
        public int CheckedMissionCount
        {
            get => checkedMissionCount;
            set => SetProperty(ref checkedMissionCount, value);
        }
        public int RemainingMissionCount
        {
            get => TotalCount - CheckedMissionCount;
        }

        public ObservableCollection<PanelMission> PanelMissionOBCollection
        {
            get => panelMissionOBCollection;
            set => SetProperty(ref panelMissionOBCollection, value);
        }
        public ObservableCollection<InspectItem> PanelItemOBcollection
        {
            get => panelItemOBcollection;
            set => SetProperty(ref panelItemOBcollection, value);
        }
        public InspectItem? SelectMission
        {
            get => selectMission;
            set => SetProperty(ref selectMission, value);
        }
        public PanelMission? SelectPanelMission
        {
            get => selectPanelMission;
            set => SetProperty(ref selectPanelMission, value);
        }
        public EqpMissionViewModel(GroupData groupData)
        {
            EqpName = groupData.EqpName;
            TotalCount = groupData.TotalItems;
            CheckedMissionCount = groupData.FinishedItem;
            foreach (var itme in groupData.InspectItems)
            {
                if (itme.Status == null)
                {
                    PanelItemOBcollection.Add(itme);
                    missionQueue?.Enqueue(itme);
                }
            }
        }
        public void FillMissionViewCollection()
        {
            Task.Run(() =>
            {
                while (PanelMissionOBCollection?.Count <= 20)
                {
                    if (missionQueue!=null && missionQueue.TryDequeue(out InspectItem? inspectItem))
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            PanelMission a = new(inspectItem);
                            PanelMissionOBCollection?.Add(new(inspectItem));
                        });
                    }
                    else
                    {
                        break;
                    }
                }
            });
        }

        public bool FinishPanelMission(out PanelMission? panelMission)
        {
            panelMission = null;
            if (SelectPanelMission!=null)
            {
                panelMission = SelectPanelMission;
                if (PanelMissionOBCollection.Remove(SelectPanelMission))
                {
                    FillMissionViewCollection();
                    CheckedMissionCount = CheckedMissionCount >= 0 ? CheckedMissionCount-- : 0;
                    //PanelMissionFinishedEvent?.Invoke(SelectPanelMission, new());
                    return true;
                }
            }
            return false;
        }

        public int CompareTo(EqpMissionViewModel? other)
        {
            if (other != null)
            {
                var a = Convert.ToInt32(this.EqpName?[^2..]);
                var b = Convert.ToInt32(other.EqpName?[^2..]);
                return a > b ? 1 : -1;
            }
            else
            {
                return 0;
            }
        }
    }

    public class PanelMission: InspectItem
    {
        public BitmapImage? PanelImage { get; private set; }
        public InspectItem? PanelInfo { get; private set; }
        public PanelMission()
        {

        }
        public PanelMission(InspectItem Item)
        {
            if (Item.Id != null)
            {
                PanelInfo = Item;
                try
                {
                    var imageStream = ServerConnector.GetImage(Item.Id);
                    BitmapImage bitmapImage = new();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = imageStream;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                    PanelImage = bitmapImage;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
