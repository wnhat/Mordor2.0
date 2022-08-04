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
        private double tactTime;
        private double avgTactTime;
        private int inspCount;
        private readonly DispatcherTimer dispatcherTimer = new();
        public DateTime tactStartTime = DateTime.Now;
        public TimeSpan TactTimeSpan { get; set; }
        public TimeSpan TotalTactTimeSpan { get; set; }
        public double TactTime
        {
            get => (double)Math.Round((decimal)tactTime, 0);
            set => SetProperty(ref tactTime, value); 

        }
        public double TactTimeFullPrecision
        {
            get => tactTime;
        }
        public double AverageTactTime
        {
            get => avgTactTime;
            set => SetProperty(ref avgTactTime, value);
        }
        public int InspCount
        {
            get => inspCount;
            set
            {
                SetProperty(ref inspCount, value);
                AverageTactTime = (double)Math.Round((decimal)(TotalTactTimeSpan.TotalMilliseconds / inspCount / 1000), 2);
            }
        }
        private readonly Random _random = new();
        public ObservableCollection<ISeries> Series { get; set; } = new();
        private LineSeries<double> LineSeries { get; } = new();
        public ObservableCollection<double> SeriesValues { get; set; } = new ObservableCollection<double> { 2, 1, 2, 3, 4, 3, 6, 5, 8, 7, 10, 9 };

        public InformationViewModel()
        {
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(100);
            dispatcherTimer.Tick += new EventHandler(TactTimeTick);
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
            TactTime = TactTimeSpan.TotalMilliseconds / 1000;
        }
        public void StartTick()
        {
            tactStartTime = DateTime.Now;
            dispatcherTimer.Start();
        }
        public void ResetTact()
        {
            TactTime = 0;
            InspCount = 0;
            AverageTactTime = 0;
            TotalTactTimeSpan = TimeSpan.Zero;
        }
    }
}
