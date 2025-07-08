using System;
using System.Collections.Generic;

namespace SportCentrum.Models
{
    public class Training
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public TimeSpan? Duration { get; set; }
        public TimeSpan? DurationWithoutCoach {  get; set; }
        public TimeSpan? DurationWithCoach { get; set; }
        public virtual ICollection<TrainingSession> Sessions { get; set; }
        public virtual ICollection<UserTraining> UserTrainings { get; set; }
        public virtual ICollection<CoachTraining> CoachTrainings { get; set; }
    }
}
