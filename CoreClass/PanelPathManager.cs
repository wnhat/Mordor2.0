using CoreClass.DICSEnum;
using System.Collections.Generic;
using System.Linq;

namespace CoreClass
{
    public class PanelPathManager
    {
        Dictionary<string, List<PanelPathContainer>> theContainer = new Dictionary<string, List<PanelPathContainer>>();
        private readonly object ContainerLock = new object();
        public Dictionary<string, List<PanelPathContainer>> PathDict
        {
            get
            {
                return theContainer;
            }
        }
        public void AddPanelPath(PanelPathContainer thispanel)
        {
            lock (ContainerLock)
            {
                if (theContainer.ContainsKey(thispanel.PanelId))
                {
                    theContainer[thispanel.PanelId].Add(thispanel);
                }
                else
                {
                    theContainer.Add(thispanel.PanelId, new List<PanelPathContainer> { thispanel });
                }
            }
        }

        public void PanelPathAdd(List<PanelPathContainer> panelList)
        {
            foreach (var panel in panelList)
            {
                AddPanelPath(panel);
            }
        }

        public List<PanelPathContainer> PanelPathGet(string panelId)
        {
            if (theContainer.ContainsKey(panelId))
            {
                return theContainer[panelId];
            }
            else
            {
                return null;
            }
        }
        public PanelPathContainer PanelPathGet(string panelId, Pcinfo pcsection)
        {
            // TODO: 当同时存在多个相同ID产品时的情况；
            PanelPathContainer pathContainer = null;
            if (theContainer.ContainsKey(panelId))
            {
                List<PanelPathContainer> containerList = theContainer[panelId];
                foreach (var containerItem in containerList)
                {
                    if (containerItem.PcName == pcsection)
                    {
                        pathContainer = containerItem;
                    }
                }
            }
            return pathContainer;
        }

        public void Clear()
        {
            theContainer.Clear();
        }

        public bool Contains(string panelId)
        {
            return theContainer.ContainsKey(panelId);
        }

        public string[] GetKeys()
        {
            return theContainer.Keys.ToArray();
        }

        public void AddRange(PanelPathManager newManager)
        {
            foreach (var item in newManager.GetKeys())
            {
                this.PanelPathAdd(newManager.PanelPathGet(item));
            }

        }
    }
}