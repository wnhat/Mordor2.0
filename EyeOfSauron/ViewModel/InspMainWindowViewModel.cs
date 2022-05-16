namespace EyeOfSauron.ViewModel
{
    public class InspMainWindowViewModel : ViewModelBase
    {
        public UserInfoViewModel UserInfo { get; }
        public MissionInfoViewModel MissionInfoViewModel { get; }
        public DefectJudgeViewModel DefectJudge { get; }
        public InspMainWindowViewModel(UserInfoViewModel userInfo)
        {
            UserInfo = userInfo;
            DefectJudge = new();
            MissionInfoViewModel = new();
        }
    }
}
