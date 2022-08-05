using CoreClass.Model;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeOfSauron.ViewModel
{
    public class PushExamMissionViewModel : ViewModelBase
    {
        private ObservableCollection<User> allUsers = new();
        private User? allUsers_Selected;
        private ObservableCollection<User> selectedUsers = new();
        private User? selectedUsers_Selected;
        private TimeSpan timeLimitPerPanel = TimeSpan.FromSeconds(10);
        private TimeSpan timeLimitPerMission;
        private readonly int missionCount;

        public PushExamMissionViewModel(MissionCollectionInfo missionCollectionInfo)
        {
            AddUserToSelected = new CommandImplementation(
                _ => 
                {
                    selectedUsers.Add(AllUsers_Selected);
                    allUsers.Remove(AllUsers_Selected);
                    if(AllUsers.Count > 0)
                    {
                        AllUsers_Selected = AllUsers.First();
                    }
                },
                _ => allUsers_Selected != null);
            RemoveUserFromSelected = new CommandImplementation(
                _ => 
                {
                    allUsers.Add(SelectedUsers_Selected);
                    selectedUsers.Remove(SelectedUsers_Selected);
                    if(SelectedUsers.Count > 0)
                    {
                        SelectedUsers_Selected = SelectedUsers?.First();
                    }
                    
                }, 
                _ => selectedUsers_Selected != null);
            if (missionCollectionInfo != null)
            {
                missionCount = missionCollectionInfo.Count;
                var userIds = ExamMissionCollection.GetUserByCollectionName(missionCollectionInfo.MissionCollection.CollectionName);
                foreach(var item in userIds)
                {
                    var userId = item.GetValue("_id").AsObjectId;
                    selectedUsers.Add(UserDbClass.GetUser(userId));
                }
                var userList = UserDbClass.GetAllUsers();
                foreach (var user in userList)
                {
                    if (!selectedUsers.Contains(user))
                    {
                        allUsers.Add(user);
                    }
                }
            }
            else throw new Exception(message: "missionCollectionInfo is null");
        }
        public TimeSpan TimeLimitPerPanel
        {
            get => timeLimitPerPanel;
            set
            {
                value = value > TimeSpan.FromSeconds(100)? TimeSpan.FromSeconds(100):value;
                SetProperty(ref timeLimitPerPanel, value);
                TimeLimitPerMission = TimeSpan.FromSeconds((timeLimitPerPanel * missionCount).TotalSeconds);
            }
        }
        public TimeSpan TimeLimitPerMission
        {
            get => timeLimitPerMission;
            set => SetProperty(ref timeLimitPerMission, value);
        }
        public ObservableCollection<User> AllUsers
        {
            get => allUsers;
            set => SetProperty(ref allUsers, value);
        }
        public User? AllUsers_Selected
        {
            get => allUsers_Selected;
            set => SetProperty(ref allUsers_Selected, value);
        }
        public ObservableCollection <User> SelectedUsers
        {
            get => selectedUsers;
            set => SetProperty(ref selectedUsers, value);
        }
        public User? SelectedUsers_Selected
        {
            get => selectedUsers_Selected;
            set => SetProperty(ref selectedUsers_Selected, value);
        }
        public CommandImplementation AddUserToSelected
        {
            get;
        }
        public CommandImplementation RemoveUserFromSelected
        {
            get;
        }
    }
}
