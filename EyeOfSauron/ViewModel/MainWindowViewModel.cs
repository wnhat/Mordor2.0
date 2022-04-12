using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoreClass.Model;
using System.Windows.Media.Imaging;

namespace EyeOfSauron.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        public UserInfoViewModel _userInfo { get; }
        public DefectListViewModel _defectList { get; }
        public InspImageViewModel _inspImage { get; }
        public DefectJudgeViewModel _defectJudge { get; }
        public MainWindowViewModel(UserInfoViewModel userInfo)
        {
            _userInfo = userInfo;
            _defectList = new DefectListViewModel();
            _inspImage = new InspImageViewModel();
            _defectJudge = new DefectJudgeViewModel();
        }
    }
}
