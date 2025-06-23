using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SportCentrum.DtoModels
{
    [XmlRoot("Data")]
    public class DataDto
    {
        public required UsersDto Users { get; set; }
        public required CoachesDto Coaches { get; set; }
        public required TrainingsDto Trainings { get; set; }
        public required TrainingSessionsDto TrainingSessions { get; set; }
    }

    public class UsersDto
    {
        [XmlElement("User")]
        public required List<UsersDto> Users { get; set; }
    }

    public class CoachesDto
    {
        [XmlElement("Coach")]
        public required List<CoachesDto> Coaches { get; set; }
    }

    public class TrainingsDto
    {
        [XmlElement("Training")]
        public required List<TrainingsDto> Trainings { get; set; }
    }

    public class TrainingSessionsDto
    {
        public required IndividualSessionsDto Individual { get; set; }
        public required GroupSessionsDto Group { get; set; }
    }

    public class IndividualSessionsDto
    {
        [XmlElement("Session")]
        public required List<IndividualSessionDto> Sessions { get; set; }
    }

    public class GroupSessionsDto
    {
        [XmlArray("SwimmimgSessions")]
        [XmlArrayItem("Session")]
        public required List<GroupSessionDto> SwimmingSessions { get; set; }

        [XmlArray("TennisSessions")]
        [XmlArrayItem("Session")]
        public required List<GroupSessionDto> TennisSessions { get; set; }

        [XmlArray("VolleyballSessions")]
        [XmlArrayItem("Session")]
        public required List<GroupSessionDto> VolletballSessions { get; set; }

        [XmlArray("BadmintinSessions")]
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
        public int Id { get; set; }
        public required string Name { get; set; }
        public string DurationWithoutCoach { get; set; }
        public required string DurationWithCoach { get; set; }
        public required string Duration { get; set; }
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

    public class IndividualSessionDto
    {
        public required string TrainingId { get; set; }
        public required string TrainingType { get; set; }
        public required string StartTime { get; set; }
        public int Capacity { get; set; }
        [XmlArray("DaysOfWeek")]
        [XmlArrayItem("Day")]
        public required List<string> DaysOfWeek { get; set; }
    }

    public class GroupSessionDto
    {
        public required string Id { get; set; }
        public int CoachId { get; set; }
        public required string TrainingId { get; set; }
        public required string TrainingType { get; set; }
        public required string DayofWeek { get; set; }
        public required string Start { get; set; }
        public int Capacity { get; set; }
    }
}
