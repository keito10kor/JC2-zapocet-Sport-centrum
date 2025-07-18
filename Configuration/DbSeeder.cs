﻿using System;
using System.Linq;
using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using SportCentrum.Models;
using SportCentrum.DtoModels;
using SportCentrum.Context;
using System.Xml;
using System.Reflection.Metadata.Ecma335;

namespace SportCentrum.Configuration
{
    public class DbSeeder
    {
        private static TimeSpan? SafeConvertationToTimeSpan(string? duration)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(duration))
                {
                    return null;
                }
                return XmlConvert.ToTimeSpan(duration);
            }
            catch (Exception e) 
            { 
                Console.WriteLine(e);
                return null;
            }
        }

        private static DateTime GetNextWeekday(DateTime start, DayOfWeek day)
        {
            return start.AddDays(((int)day - (int)start.DayOfWeek + 7) % 7);
        }

        private static List<TrainingSession> GetAllSessions(AllSessionsDto allSessions, List<Training> trainingsList)
        {
            Console.WriteLine($"Group null? {allSessions.Group == null}");
            Console.WriteLine($"Tennis count: {allSessions.Group?.TennisSessions?.Count}");
            Console.WriteLine($"Individual null? {allSessions.Individual == null}");
            Console.WriteLine($"Individual count: {allSessions.Individual?.Sessions?.Count}");
            var sessions = new List<ISessionDto>();
            sessions.AddRange(allSessions.Group!.TennisSessions);
            sessions.AddRange(allSessions.Group.PilatesSessions);
            sessions.AddRange(allSessions.Group.BadmintonSessions);
            sessions.AddRange(allSessions.Group.YogaSessions);
            sessions.AddRange(allSessions.Group.AquaAerobicsSessions);
            sessions.AddRange(allSessions.Group.BasketballSessions);
            sessions.AddRange(allSessions.Group.FloorballSessions);
            sessions.AddRange(allSessions.Group.VolleyballSessions);
            sessions.AddRange(allSessions.Group.SwimmingSessions);
            sessions.AddRange(allSessions.Individual!.Sessions);

            var result = new List<TrainingSession>();
            foreach (var session in sessions)
            {
                if(!TimeSpan.TryParse(session.StartTime, out var startTime))
                {
                    continue;
                }

                var training = trainingsList.FirstOrDefault(t => t.Id == session.TrainingId);
                if(training == null)
                {
                    continue;
                }
                var duration = training.Duration ?? training.DurationWithoutCoach ?? TimeSpan.Zero;
                foreach (var dayStr in session.Days)
                {
                    Console.WriteLine($"Creating individual session: {session.TrainingId} at {session.StartTime} on {dayStr}");
                    if (!Enum.TryParse<DayOfWeek>(dayStr, true, out var dayOfWeek))
                    {
                        continue;
                    }
                    var startLocal = GetNextWeekday(DateTime.Today, dayOfWeek).Add(startTime);
                    var start = DateTime.SpecifyKind(startLocal, DateTimeKind.Utc);
                    var isGroup = session.IsGroup;
                    if(!isGroup && session.CoachId == null)
                    {
                        bool exists = result.Any(s => !s.IsGroup && s.CoachId == null && s.TrainingId == session.TrainingId && s.DayOfWeek == dayOfWeek.ToString() && s.Start.TimeOfDay == start.TimeOfDay);
                        if(exists)
                        {
                            continue;
                        }
                    }
                    string id = isGroup ? session.Id : SessionIdGenerator.Generate(session.TrainingId, dayOfWeek, startTime);


                    result.Add(new TrainingSession
                    {
                        Id = id,
                        TrainingId = session.TrainingId,
                        CoachId = session.CoachId,
                        DayOfWeek = dayOfWeek.ToString(),
                        Start = start,
                        End = start.Add(duration),
                        Capacity = session.Capacity,
                        IsGroup = session.IsGroup
                    });
                }
            }
            Console.WriteLine(result.Count);
            return result;
        }
        public static void Seed(SportCentrumContext context, string xmlPath)
        {
            DataDto data;
            try
            {
                data = XmlDataLoader.LoadFromFile(xmlPath);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in loading xml file: {xmlPath}");
                Console.WriteLine($"Exception: {e.Message}");
                Console.WriteLine($"StackTrace: {e.StackTrace}");
                return;
            }
            
            var trainingEntities = new List<Training>();

            if (!context.Users.Any())
            {
                foreach (var user in data.Users.Users)
                {
                    var client = new User
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Surname = user.Surname,
                        Email = user.Email,
                        Gender = user.Gender
                    };
                    context.Users.Add(client);
                    context.SaveChanges();
                }
            }

            if (!context.Coaches.Any())
            {
                foreach (var coach in data.Coaches.Coaches)
                {
                    var trainer = new Coach
                    {
                        Id = coach.Id,
                        Name = coach.Name,
                        Surname = coach.Surname,
                        Email = coach.Email,
                        Gender = coach.Gender,
                        Specialty = coach.Specialty
                    };
                    context.Coaches.Add(trainer);
                    context.SaveChanges();
                }
            }
                    
            foreach (var training in data.Trainings.Trainings)
            {
                var existing = context.Trainings.FirstOrDefault(t => t.Id == training.Id);
                if (existing == null)
                {
                    var workout = new Training
                    {
                        Id = training.Id,
                        Name = training.Name,
                        DurationWithCoach = SafeConvertationToTimeSpan(training.DurationWithCoach),
                        DurationWithoutCoach = SafeConvertationToTimeSpan(training.DurationWithoutCoach),
                        Duration = SafeConvertationToTimeSpan(training.Duration)
                    };
                    trainingEntities.Add(workout);
                    context.Trainings.Add(workout);
                }
                else
                {
                    trainingEntities.Add(existing);
                }
            }
            context.SaveChanges();
                    
            if(!context.CoachTrainings.Any())
            {
                foreach (var coachTraining in data.CoachTrainings.CoachTrainings)
                {
                    var trainerTraining = new CoachTraining
                    {
                        CoachId = coachTraining.CoachId,
                        TrainingId = coachTraining.TrainingId
                    };
                    context.CoachTrainings.Add(trainerTraining);
                    context.SaveChanges();
                }
            }

            var sessionsList = GetAllSessions(data.TrainingSessions, trainingEntities);
            var existingCoachIds = context.Coaches.Select(c => c.Id).ToList();
            var filteredSessions = sessionsList.Where(s => s.CoachId == null || existingCoachIds.Contains((int)s.CoachId));
            foreach(var session in filteredSessions)
            {
                if(!context.Sessions.Any())
                {
                    context.Sessions.Add(session);
                }
            }
            context.SaveChanges();
        }
    }
}
