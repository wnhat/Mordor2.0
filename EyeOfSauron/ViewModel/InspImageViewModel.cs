using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace EyeOfSauron.ViewModel
{
    public class InspImageViewModel
    {
        public BitmapImage image = new BitmapImage();
        public InspImageViewModel()
        {
            image.BeginInit();
            image.UriSource = new Uri(@"D:\DICS Software\DefaultSample\AVI\Orign\DefaultSample\00_DUST_CAM00.bmp", UriKind.Absolute);
            image.EndInit();
        }
        
    }
}
