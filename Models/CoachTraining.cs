namespace SportCentrum.Models
{
    public class CoachTraining
    {
        public int CoachId { get; set; }
        public virtual Coach Coach { get; set; }
        public string TrainingId { get; set; }
        public virtual Training Training { get; set; }
    }
}
