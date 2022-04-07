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
        public BitmapImage[] imageArray = new BitmapImage[3];
        public BitmapImage defaultImage = new BitmapImage();
        Uri defaultImageUri = new Uri(@"D:\DICS Software\DefaultSample\AVI\Orign\DefaultSample\00_DUST_CAM00.bmp", UriKind.Absolute);
        public InspImageViewModel()
        {
            defaultImage.BeginInit();
            defaultImage.UriSource = defaultImageUri;
            defaultImage.EndInit();
            for (int i = 0; i < 3; i++)
            {
                imageArray[i] = defaultImage;
            }
        }
        
    }
}
