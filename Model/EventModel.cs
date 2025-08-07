using CommunityToolkit.Mvvm.ComponentModel;
using Syncfusion.Maui.Scheduler;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace OrderApp.Model
{
    public partial class EventModel 
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public string Description { get; set; }
        public string EventType { get; set; }
        public string Notes { get; set; }
        public Brush Background { get; set; }
        public Color TextColor {get ;set;}
        public string BackgroundStr { get; set; }
        public string TextColorStr { get; set; }

        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public ObservableCollection<DateTime> RecurrenceExceptions { get; set; }
    }
}
