using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace EyeOfSauron.ViewModel
{
    public class InspImageViewModel : ViewModelBase
    {
        private BitmapImage[] _imageArray = new BitmapImage[3];
        private string[] _imageNameArray = new string[3];
        private BitmapImage _defaultImage = new();
        static Uri defaultImageUri = new(@"D:\DICS Software\DefaultSample\AVI\Orign\DefaultSample\00_DUST_CAM00.bmp", UriKind.Absolute);
        public InspImageViewModel()
        {
            _defaultImage.BeginInit();
            _defaultImage.UriSource = defaultImageUri;
            _defaultImage.EndInit();
            for (int i = 0; i < 3; i++)
            {
                _imageArray[i] = _defaultImage;
            }
        }
        public BitmapImage defaultImage
        {
            get => _defaultImage;
            set => SetProperty(ref _defaultImage, value);
        }
        public BitmapImage[] imageArray
        {
            get => _imageArray;
            set => SetProperty(ref _imageArray, value);
        }
        public string[] imageNameArray
        {
            get => _imageNameArray;
            set => SetProperty(ref _imageNameArray, value);
        }
    }
}
