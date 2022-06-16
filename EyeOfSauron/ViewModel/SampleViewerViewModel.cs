using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using EyeOfSauron.MyUserControl;
using CoreClass.Model;
using System.Windows;
using MaterialDesignThemes.Wpf;

namespace EyeOfSauron.ViewModel
{
    class SampleViewerViewModel:ViewModelBase
    {
        private ColorTool colorTool = new();
        private DateTime dateTime = DateTime.Now;
        private InspImageView inspImageView = new();
        public DateTime DateTime
        {
            get => dateTime;
            set => SetProperty(ref dateTime, value);
        }
        public ColorTool ColorTool
        {
            get => colorTool;
            set => SetProperty(ref colorTool, value);
        }
        public InspImageView InspImageView
        {
            get => inspImageView;
            set => SetProperty(ref inspImageView, value);
        }
    }
}
