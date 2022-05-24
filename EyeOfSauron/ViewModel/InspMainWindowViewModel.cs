namespace EyeOfSauron.ViewModel
{
    public class InspMainWindowViewModel : ViewModelBase
    {
        public MissionInfoViewModel MissionInfoViewModel { get; }
        public DefectJudgeViewModel DefectJudge { get; }
        public InspMainWindowViewModel()
        {
            DefectJudge = new();
            MissionInfoViewModel = new();
        }
    }
}
