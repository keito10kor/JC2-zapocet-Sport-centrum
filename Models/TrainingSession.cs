using System;
using System.Collections.Generic;

namespace SportCentrum.Models
{

    public enum TrainingType
    {
        Individual,
        Group
    }
    public class TrainingSession
    {
        public string Id { get; set; }
        public string TrainingId { get; set; }
        public virtual Training Training { get; set; }
        public int? CoachId { get; set; }
        public virtual Coach? Coach { get; set; }
        public TrainingType TrainingType { get; set; }
        public string? DayOfWeek { get; set; }
        public string? DaysOfWeek { get; set; }
        public DateTime Start {  get; set; }
        public DateTime End { get; set; }
        public int Capacity { get; set; }
        public virtual ICollection<TrainingReservation> Reservations { get; set; }

    }
}
