using System.Collections.ObjectModel;
using CoreClass;
using CoreClass.Model;

namespace EyeOfSauron.ViewModel
{
    public class DefectJudgeViewModel : ViewModelBase
    {
        private ObservableCollection<Defect> defectJudgeList = new();
        public DefectJudgeViewModel()
        {
            //convert Parameter.CodeNameList to ObservableCollection<Defect>
            foreach (var defect in Parameter.CodeNameList)
            {
                DefectJudgeList.Add(defect);
            }
        }
        public ObservableCollection<Defect> DefectJudgeList
        {
            get => defectJudgeList;
            set => SetProperty(ref defectJudgeList, value);
        }
    }
}
