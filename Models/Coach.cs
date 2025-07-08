using System;
using System.Collections.Generic;

namespace SportCentrum.Models
{
    public class Coach
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Specialty { get; set; }
        public virtual ICollection<TrainingSession> Sessions { get; set; }
        public virtual ICollection<CoachTraining> CoachTrainings { get; set; }
        
    }
}
