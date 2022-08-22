using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using CoreClass.Model;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.Defaults;
using System.Windows.Threading;

namespace EyeOfSauron.ViewModel
{
    public class InformationViewModel : ViewModelBase
    {
        public event EventHandler? CountDownFinishEvent;
        private readonly DispatcherTimer dispatcherTimer = new();
        private int inspCount;
        private TimeSpan tactTimeSpan;
        private TimeSpan avgTactTimeSpan;
        private TimeSpan missionRemainingTime;
        /// <summary>
        /// Time when dispatcherTimer start ticking
        /// </summary>
        private DateTime tickStartTime;
        /// <summary>
        /// Time when one mission start inspecting;
        /// </summary>
        public DateTime tactStartTime;
        public TimeSpan TactTimeSpan 
        { 
            get => tactTimeSpan;
            set => SetProperty(ref tactTimeSpan, value);
        }
        public TimeSpan AvgTactTimeSpan
        {
            get => avgTactTimeSpan;
            set => SetProperty(ref avgTactTimeSpan, value);
        }
        public TimeSpan TotalTactTimeSpan { get; set; }
        public double TactTimeFullPrecision
        {
            get => (double)Math.Round((decimal)(TactTimeSpan.TotalMilliseconds / 1000), 0);
        }

        public int InspCount
        {
            get => inspCount;
            set
            {
                SetProperty(ref inspCount, value);
                if(value > 0)
                {
                    AvgTactTimeSpan = TotalTactTimeSpan / inspCount;
                }
            }
        }
        public TimeSpan MissionTimeLimit { get; set; } = TimeSpan.Zero;
        /// <summary>
        /// Using only in exam mission;
        /// </summary>
        public TimeSpan MissionRemainingTime
        {
            get => missionRemainingTime;
            set => SetProperty(ref missionRemainingTime, value);
        }
        private readonly Random _random = new();
        public ObservableCollection<ISeries> Series { get; set; } = new();
        private LineSeries<double> LineSeries { get; } = new();
        public ObservableCollection<double> SeriesValues { get; set; } = new ObservableCollection<double> { 2, 1, 2, 3, 4, 3, 6, 5, 8, 7, 10, 9 };
        public InformationViewModel()
        {
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);
            AddLineDataCommand = new(
                _ =>
                {
                    LineSeries<double> LineSeries = new() { Values = new ObservableCollection<double> { _random.Next(1, 10), _random.Next(1, 10), _random.Next(1, 10), _random.Next(1, 10), _random.Next(1, 10), _random.Next(1, 10), _random.Next(1, 10), _random.Next(1, 10), _random.Next(1, 10), _random.Next(1, 10) } };
                    Series.Add(LineSeries);
                });
            DelLineDataCommand = new(
                _=>
                {
                    Series.RemoveAt(Series.Count-1);
                },
                _=> Series.Count>0
                );
            LineSeries.Values = SeriesValues;
            Series.Add(LineSeries);
        }
        public CommandImplementation AddLineDataCommand { get; }
        public CommandImplementation DelLineDataCommand { get; }
        public void TactTimeTick(object? sender, EventArgs e)
        {
            TactTimeSpan = DateTime.Now - tactStartTime;
        }
        /// <summary>
        /// MissionRemainingTime countdown;
        /// Will not execute when MissionTimeLimit is zero;
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MissionTimeLimitTick(object? sender, EventArgs e)
        {
            if (MissionTimeLimit == TimeSpan.Zero)
            {
                MissionRemainingTime = TimeSpan.Zero;
                dispatcherTimer.Tick -= MissionTimeLimitTick;
            }
            else
            {
                MissionRemainingTime = tickStartTime + MissionTimeLimit - DateTime.Now;
                if (MissionRemainingTime < TimeSpan.Zero)
                {
                    dispatcherTimer.Tick -= MissionTimeLimitTick;
                    CountDownFinishEvent?.Invoke(this, new());
                }
            }
        }
        public void StartTick()
        {
            tactStartTime = DateTime.Now;
            tickStartTime = DateTime.Now;
            dispatcherTimer.Tick += new EventHandler(TactTimeTick);
            dispatcherTimer.Tick += new EventHandler(MissionTimeLimitTick);
            dispatcherTimer.Start();
        }
        public void TicktStopAndReset()
        {
            dispatcherTimer.Tick -= TactTimeTick;
            dispatcherTimer.Tick -= MissionTimeLimitTick;
            dispatcherTimer.Stop();
            InspCount = 0;
            TactTimeSpan = TimeSpan.Zero;
            TotalTactTimeSpan = TimeSpan.Zero;
            AvgTactTimeSpan = TimeSpan.Zero;
            MissionRemainingTime = TimeSpan.Zero;
        }
    }
}
