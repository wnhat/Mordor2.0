using CoreClass.Model;
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
        private DateTime _date;
        private DateTime _time;
        private ObservableCollection<User> allUsers = new();
        private User? allUsers_Selected;
        private ObservableCollection<User> selectedUsers = new();
        private User? selectedUsers_Selected;

        public PushExamMissionViewModel(MissionCollectionInfo missionCollectionInfo)
        {
            Date = DateTime.Now;
            Time = DateTime.Now;
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
                var b = ExamMissionWIP.GetUserByCollectionName(missionCollectionInfo._id.CollectionName);
                foreach(var item in b)
                {
                    selectedUsers.Add(UserDbClass.GetUser(item.GetValue("UserID").AsObjectId));
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
            else throw new Exception(message: "Error");
        }

        public DateTime Date
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

        public DateTime Time
        {
            get => _time;
            set => SetProperty(ref _time, value);
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
