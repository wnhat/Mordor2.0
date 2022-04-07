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
        public UserInfoViewModel _userInfo;
        public DefectListViewModel _defectList;
        public InspImageViewModel _inspImage;
        public DefectJudgeViewModel _defectJudge = new DefectJudgeViewModel();
        public MainWindowViewModel(UserInfoViewModel userInfo)
        {
            _userInfo = userInfo;
            _defectList = new DefectListViewModel();
            _inspImage = new InspImageViewModel();
        }
    }
}
