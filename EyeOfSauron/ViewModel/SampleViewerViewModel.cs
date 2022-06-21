using System;
using EyeOfSauron.MyUserControl;
using System.Windows.Threading;

namespace EyeOfSauron.ViewModel
{
    class SampleViewerViewModel:ViewModelBase
    {
        private ColorTool colorTool = new();
        private DateTime dateTime = DateTime.Now;
        private InspImageView inspImageView = new();
        private double loadMissionProgressValue;
        private readonly DispatcherTimer dispatcherTimer = new();

        public SampleViewerViewModel()
        {
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);
            dispatcherTimer.Tick += new EventHandler(MissionLoadProgress);
            _ = new DispatcherTimer(
                    TimeSpan.FromMilliseconds(1000),
                    DispatcherPriority.Normal,
                    new EventHandler((o, e) =>
                    {
                        DateTime = DateTime.Now;
                    }), Dispatcher.CurrentDispatcher);
        }

        public double LoadMissionProgressValue 
        {
            get => loadMissionProgressValue;
            set => SetProperty(ref loadMissionProgressValue, value);
        }
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

        private void MissionLoadProgress(object sender, EventArgs e)
        {

        }
    }
}
