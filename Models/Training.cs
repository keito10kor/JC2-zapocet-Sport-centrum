using System;
using System.Collections.Generic;

namespace Sport_centrum.Models
{
    public class Training
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public TimeSpan Duration { get; set; }
        public virtual ICollection<TrainingSession> Sessions { get; set; }
    }
}
