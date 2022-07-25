﻿using CoreClass.Model;
using EyeOfSauron.MyUserControl;
using MongoDB.Bson.Serialization;
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
        private ObservableCollection<MissionCollectionInfo> panelMissionCollectionInfos = new();
        private MissionCollectionInfo? selectPanelMissionCollectionInfo;
        public ObservableCollection<MissionCollectionInfo> PanelMissionCollectionInfos
        {
            get => panelMissionCollectionInfos;
            set => SetProperty(ref panelMissionCollectionInfos, value);
        }
        public MissionCollectionInfo? SelectPanelMissionCollectionInfo
        {
            get => selectPanelMissionCollectionInfo;
            set => SetProperty(ref selectPanelMissionCollectionInfo, value);
        }
        public CollectionSettingViewModel()
        {
            _ = RefreshCollectView();
        }
        public async Task RefreshCollectView()
        {
            PanelMissionCollectionInfos.Clear();
            var missionCollectionInfos = await PanelSample.GetMissionCountCollection();
            foreach (var item in missionCollectionInfos)
            {
                var missionCollectionInfo = BsonSerializer.Deserialize<MissionCollectionInfo>(item);
                PanelMissionCollectionInfos.Add(missionCollectionInfo);
            }
            if (PanelMissionCollectionInfos != null)
            {
                selectPanelMissionCollectionInfo = PanelMissionCollectionInfos.First();
            }
            else throw new Exception("未找到任何任务集");
        }
    }

    public class MissionCollectionInfo:ViewModelBase
    {
        public MissionCollection _id;//MongoDB 聚合查询结果的属性名必须为”_id“,此类用于反序列化查询结果

        public int count;

        public MissionCollection MissionCollection
        {
            get => _id; 
            set => SetProperty(ref _id, value);
        }
        public int Count 
        {
            get => count;
            set => SetProperty(ref count, value);
        }
        public MissionCollectionInfo()
        {
            _id = new();
        }
    }
    public class NotfoundException : Exception 
    {

    }
}
