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
        User user;
        BitmapImage[] imageArray = new BitmapImage[3];
        public MainWindowViewModel()
        {
        }
    }
}
