using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SportCentrum.DtoModels
{
    public interface ISessionDto
    {
        string Id { get; }
        string TrainingId { get; }
        string StartTime { get; }
        int Capacity { get; }
        int? CoachId {  get; }
        bool IsGroup { get; }
        IEnumerable<string> Days { get; }

    }

    [XmlRoot("Data")]
    public class DataDto
    {
        public required UsersDto Users { get; set; }
        public required CoachesDto Coaches { get; set; }
        public required TrainingsDto Trainings { get; set; }
        public required AllSessionsDto TrainingSessions { get; set; }
        public required CoachTrainingsDto CoachTrainings { get; set; }
    }

    public class UsersDto
    {
        [XmlElement("User")]
        public required List<UserDto> Users { get; set; }
    }

    public class CoachesDto
    {
        [XmlElement("Coach")]
        public required List<CoachDto> Coaches { get; set; }
    }

    public class TrainingsDto
    {
        [XmlElement("Training")]
        public required List<TrainingDto> Trainings { get; set; }
    }

    public class AllSessionsDto
    {
        [XmlElement("Individual")]
        public required IndividualSessionsDto Individual { get; set; }

        [XmlElement("Group")]
        public required GroupSessionsDto Group { get; set; }

    }

    public class IndividualSessionsDto
    {
        [XmlElement("Session")]
        public required List<IndividualSessionDto> Sessions { get; set; }
    }

    public class GroupSessionsDto
    {
        [XmlArray("SwimmingSessions")]
        [XmlArrayItem("Session")]
        public required List<GroupSessionDto> SwimmingSessions { get; set; }

        [XmlArray("TennisSessions")]
        [XmlArrayItem("Session")]
        public required List<GroupSessionDto> TennisSessions { get; set; }

        [XmlArray("VolleyballSessions")]
        [XmlArrayItem("Session")]
        public required List<GroupSessionDto> VolleyballSessions { get; set; }

        [XmlArray("BadmintonSessions")]
        [XmlArrayItem("Session")]
        public required List<GroupSessionDto> BadmintonSessions { get; set; }

        [XmlArray("BasketballSessions")]
        [XmlArrayItem("Session")]
        public required List<GroupSessionDto> BasketballSessions { get; set; }

        [XmlArray("FloorballSessions")]
        [XmlArrayItem("Session")]
        public required List<GroupSessionDto> FloorballSessions { get; set; }

        [XmlArray("YogaSessions")]
        [XmlArrayItem("Session")]
        public required List<GroupSessionDto> YogaSessions { get; set; }

        [XmlArray("AquaAerobicsSessions")]
        [XmlArrayItem("Session")]
        public required List<GroupSessionDto> AquaAerobicsSessions { get; set; }

        [XmlArray("PilatesSessions")]
        [XmlArrayItem("Session")]
        public required List<GroupSessionDto> PilatesSessions { get; set; }
    }

    public class CoachTrainingsDto
    {
        [XmlElement("CoachTraining")]
        public required List<CoachTrainingDto> CoachTrainings { get; set; }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string Email { get; set; }
        public required string Gender { get; set; }
    }

    public class TrainingDto
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public string? DurationWithoutCoach { get; set; }
        public string? DurationWithCoach { get; set; }
        public string? Duration { get; set; }
    }

    public class CoachDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string Email { get; set; }
        public required string Gender { get; set; }
        public required string Specialty { get; set; }
    }

    public class IndividualSessionDto : ISessionDto
    {
        public required string Id { get; set; }
        public required string TrainingId { get; set; }
        public required string TrainingType { get; set; }
        public required string StartTime { get; set; }
        public int Capacity { get; set; }
        public int? CoachId { get; set; } = null;
        public bool IsGroup { get; set; } = false;
        [XmlArray("DaysOfWeek")]
        [XmlArrayItem("Day")]
        public List<string> DaysOfWeek { get; set; }
        public IEnumerable<string> Days => DaysOfWeek;
    }

    public class GroupSessionDto : ISessionDto
    {
        public required string Id { get; set; }
        public int? CoachId { get; set; }
        public required string TrainingId { get; set; }
        public required string TrainingType { get; set; }
        public required string DayOfWeek { get; set; }
        [XmlElement("Start")]
        public required string StartTime { get; set; }
        public int Capacity { get; set; }
        public bool IsGroup { get; set; } = true;
        public IEnumerable<string> Days => new List<string> { DayOfWeek };

    }

    public class CoachTrainingDto
    {
        public int CoachId { get; set; }
        public string TrainingId { get; set; }
    }
}
