namespace EyeOfSauron.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public UserInfoViewModel UserInfo { get; }
        public MissionInfoViewModel MissionInfoViewModel { get; }
        public DefectJudgeViewModel DefectJudge { get; }
        public MainWindowViewModel(UserInfoViewModel userInfo)
        {
            UserInfo = userInfo;
            DefectJudge = new();
            MissionInfoViewModel = new();
        }
    }
}
