using System;
using System.Collections.Generic;

namespace SportCentrum.Models
{
    public class UserTraining
    {
        public int UserId { get; set; }
        public virtual User User { get; set; }
        public string TrainingId { get; set; }
        public virtual Training Training { get; set; }
    }
}
