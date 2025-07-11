using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.Mapping;
using SportCentrum.Context;
using SportCentrum.Models;
using SportCentrum.ViewModels;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using static System.Reflection.Metadata.BlobBuilder;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace SportСentrum.Controllers
{
    public class TrainingsController : Controller
    {
        private readonly SportCentrumContext _context;

        public TrainingsController(SportCentrumContext context)
        {
            _context = context;
        }

        public IActionResult SelectType()
        {
            return View();
        }

        public IActionResult SelectSport(string type)
        {
            ViewBag.Type = type;
            bool isGroup = type == "group";

            var sportNames = _context.Sessions.Where(s => s.IsGroup == isGroup).Include(s => s.Training).Select(s => s.Training.Name).Distinct().ToList();
            return View(sportNames);
        }

        public IActionResult SelectCoach(string type, string sport)
        {
            ViewBag.Type = type;
            ViewBag.Sport = sport;
            var training = _context.Trainings.FirstOrDefault(t => t.Name == sport);
            if(training == null)
            {
                return BadRequest("Training is not found");
            }
            var coaches = _context.CoachTrainings.Where(ct => ct.TrainingId == training.Id).Select(ct => ct.Coach).Distinct().ToList();
            return View(coaches);
        }

        public IActionResult SelectDate(string sport, int? coachId)
        {
            Console.WriteLine($"Sport: {sport}");
            var training = _context.Trainings.FirstOrDefault(t => t.Name == sport);
            if(training == null)
            {
                return NotFound();
            }
            ViewBag.Sport = sport;
            var isGroup = coachId == null;
            var now = DateTime.UtcNow;
            var days = new List<DayShedule>();
            if (isGroup)
            {
                var sessions = _context.Sessions.Include(s => s.Coach).Include(s => s.Training).Where(s => s.Training.Id == training.Id && s.IsGroup == isGroup);
                days = sessions.GroupBy(s => s.Start.Date).Select(g => new DayShedule
                {
                    Date = g.Key,
                    TimeSlots = g.OrderBy(ts => ts.Start.TimeOfDay).Select(ts => new TimeSlot
                    {
                        Time = ts.Start.TimeOfDay,
                        CoachName = isGroup ? $"{ts.Coach.Name} {ts.Coach.Surname}" : null,
                        Capacity = isGroup ? ts.Capacity : null,
                        SessionId = ts.Id
                    }).ToList()
                })
                .OrderBy(d => d.Date).ToList();
            }
            else if (coachId > 0)
            {
                var duration = training.DurationWithCoach;
                if(duration == null)
                {
                    return BadRequest("Training duration is not set");
                }
                var busy = _context.Sessions.Where(s => s.CoachId == coachId).ToList();

                for (int i = 0; i < 7; i++)
                {
                    var date = now.Date.AddDays(i);
                    var slots = new List<TimeSlot>();

                    var busySlots = busy.Where(s => s.Start.Date == date).Select(s => ( Start: s.Start.TimeOfDay, End: s.End.TimeOfDay)).ToList();
                    for (var time = TimeSpan.FromHours(8); time + duration <= TimeSpan.FromHours(20); time += duration.Value)
                    {
                        bool conflict = busySlots.Any(b => time < b.End && (time + duration) > b.Start);
                        if (!conflict)
                        {
                            slots.Add(new TimeSlot { Time = time });
                        }
                    }
                    if (slots.Any())
                    {
                        days.Add(new DayShedule { Date = date, TimeSlots = slots });
                    }
                }
            }
            else
            {
                var duration = training.DurationWithoutCoach;
                if (duration == null)
                {
                    return BadRequest("Training duration is not set");
                }
                var sessions = _context.Sessions.Where(s => s.TrainingId == training.Id && !s.IsGroup && s.CoachId == null).ToList();
                for(int i = 0; i < 7; i++)
                {
                    var date = now.AddDays(i);
                    var slots = new List<TimeSlot>();
                    var day = date.DayOfWeek;
                    slots = sessions.Where(s => s.DayOfWeek == day.ToString()).OrderBy(s => s.Start.TimeOfDay).Select(s => new TimeSlot { Time = s.Start.TimeOfDay, Capacity = s.Capacity, SessionId = s. Id }).ToList();
                    if (slots.Any())
                    {
                        days.Add(new DayShedule { Date = date, TimeSlots = slots });
                    }
                }
            }

            return View(new SheduleViewModel
            {
                TrainingType = isGroup ? "group" : "individual",
                Days = days,
                CoachId = coachId
            });
        }
    }
}
