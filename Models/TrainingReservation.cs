using System;
using System.Collections.Generic;

namespace SportCentrum.Models
{
    public class TrainingReservation
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public string TrainingSessionId { get; set; }
        public virtual TrainingSession TrainingSession { get; set; }
        public DateTime ReservationTime { get; set; }

    }
}
