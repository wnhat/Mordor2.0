using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using CoreClass.Model;

namespace EyeOfSauron.ViewModel
{
    public class InformationViewModel : ViewModelBase
    {
        private double tactTime;
        private double avgTactTime;
        private int inspCount;
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
        public void TactTimeTick(object? sender, EventArgs e)
        {
            TactTimeSpan = DateTime.Now - tactStartTime;
            TactTime = TactTimeSpan.TotalMilliseconds / 1000;
        }
    }
}
