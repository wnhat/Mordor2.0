using CoreClass.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace CutInspect.Model
{
    public class BitmapImageContainer : ImageContainer
    {
        public BitmapImage BitmapImage { get; set; }
        public DefectInfo DefectInfo { get; set; }
        public BitmapImageContainer(ImageContainer imageContainer, DefectInfo? defectInfo = null)
        {
            DefectInfo = defectInfo ?? new DefectInfo();
            ImageContainer buffer;
            if (imageContainer == null || imageContainer.Data == null || imageContainer.Data.Length == 0)
            {
                buffer = GetDefault;
                Name = imageContainer?.Name;
            }
            else
            {
                buffer = imageContainer;
                Name = imageContainer.Name;
            }
            BitmapImage = new BitmapImage();
            BitmapImage.BeginInit();
            BitmapImage.StreamSource = new MemoryStream(buffer.Data);
            BitmapImage.EndInit();
            BitmapImage.Freeze();
        }
    }
}
