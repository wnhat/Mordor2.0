using System.Collections.ObjectModel;
using CoreClass;
using CoreClass.Model;
using System.Windows.Controls;
using System;

namespace EyeOfSauron.ViewModel
{
    public class DefectJudgeViewModel : ViewModelBase
    {
        private ObservableCollection<Defect> defectJudgeList = new();
        public DefectJudgeViewModel()
        {
            //convert Parameter.CodeNameList to ObservableCollection<Defect>
            try
            {
                foreach (var defect in Parameter.CodeNameList)
                {
                    DefectJudgeList.Add(defect);
                }
            }
            catch(TypeInitializationException e)
            {
                throw e;
            }
            catch (TimeoutException e)
            {
                throw e;
            }
        }
        public ObservableCollection<Defect> DefectJudgeList
        {
            get => defectJudgeList;
            set => SetProperty(ref defectJudgeList, value);
        }
    }
}
