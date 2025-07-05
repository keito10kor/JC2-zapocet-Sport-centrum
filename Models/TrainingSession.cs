using System;
using System.Collections.Generic;

namespace SportCentrum.Models
{
    public class TrainingSession
    {
        public string Id { get; set; }
        public string TrainingId { get; set; }
        public virtual Training Training { get; set; }
        public int? CoachId { get; set; }
        public virtual Coach? Coach { get; set; }
        public bool IsGroup { get; set; }
        public string DayOfWeek { get; set; }
        public DateTime Start {  get; set; }
        public DateTime End { get; set; }
        public int Capacity { get; set; }
        public virtual ICollection<TrainingReservation> Reservations { get; set; }
    }
}
