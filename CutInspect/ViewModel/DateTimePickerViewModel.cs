using CutInspect.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CutInspect.ViewModel
{
    public class DateTimePickerViewModel:ViewModelBase
    {
        private DateTime _date;
        private DateTime _time;
        private DateTime startTime;
        private DateTime endTime;
        private WorkType workType;
        public DateTime PickedDate
        {
            get => _date;
            set
            {
                SetProperty(ref _date, value);
                StartTime = PickedDate.Hour > 6 && PickedDate.Hour <= 18 ? PickedDate.Date + TimeSpan.FromHours(6) : (PickedDate.Hour <= 6 ? PickedDate.Date - TimeSpan.FromHours(6) : PickedDate.Date + TimeSpan.FromHours(18));
            }
        }
        public DateTime Time
        {
            get => _time;
            set => SetProperty(ref _time, value);
        }
        public DateTime StartTime
        {
            get => startTime;
            set
            {
                SetProperty(ref startTime, value);
                EndTime = startTime + TimeSpan.FromHours(12);
            }
        }
        public DateTime EndTime
        {
            get => endTime;
            private set => SetProperty(ref endTime, value);
        }
        public WorkType WorkType
        {
            get => workType;
            set
            {
                SetProperty(ref workType, value);
                switch (value)
                {
                    case WorkType.DAY:
                        StartTime = PickedDate.Date + TimeSpan.FromHours(6);
                        break;
                    case WorkType.NIGHT:
                        StartTime = PickedDate.Date + TimeSpan.FromHours(18);
                        break;
                    default:
                        break;
                }
            }
        }
        public CommandImplementation StepBackOneDayCommand { get; }
        public CommandImplementation StepOneDayCommand { get; }
        public CommandImplementation NowDateCommand { get; }
        public DateTimePickerViewModel()
        {
            PickedDate = DateTime.Now;
            StartTime = PickedDate.Hour > 6 && PickedDate.Hour <= 18 ? PickedDate.Date + TimeSpan.FromHours(6) : (PickedDate.Hour <= 6  ? PickedDate.Date - TimeSpan.FromHours(6):PickedDate.Date + TimeSpan.FromHours(18));
            StepBackOneDayCommand = new(_ => PickedDate -= TimeSpan.FromDays(1));
            StepOneDayCommand = new(_ => PickedDate += TimeSpan.FromDays(1),_=> PickedDate.Day<DateTime.Now.Day);
            NowDateCommand = new(_ => PickedDate = DateTime.Now);
        }
        public void SetStartTime(object? sender,EventArgs args)
        {
            StartTime = startTime;
        }
    }

}
