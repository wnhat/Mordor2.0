namespace EyeOfSauron.ViewModel
{
    public class InspMissionViewModel : ViewModelBase
    {
        public MissionInfoViewModel MissionInfoViewModel { get; }
        public DefectJudgeViewModel DefectJudge { get; }
        public InspMissionViewModel()
        {
            DefectJudge = new();
            MissionInfoViewModel = new();
        }
    }
}
