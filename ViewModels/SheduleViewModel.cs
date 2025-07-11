namespace SportCentrum.ViewModels
{
    public class SheduleViewModel
    {
        public string TrainingType { get; set; }
        public int? CoachId { get; set; }
        public List<DayShedule> Days { get; set; }
    }

    public class DayShedule
    {
        public DateTime Date { get; set; }
        public List<TimeSlot> TimeSlots { get; set; }
    }

    public class TimeSlot 
    {
        public TimeSpan Time { get; set; }
        public string? CoachName { get; set; }
        public int? Capacity { get; set; }
        public string? SessionId { get; set; }
    }
}
