using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportCentrum.Context;
using SportCentrum.Models;
using SportCentrum.ViewModels;
using System.Collections.Generic;
using System.Linq;

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
            /*IQueryable<Training> query = _context.Trainings;
            if(type == "individual")
            {
                query = query.Where(t => t.DurationWithCoach != null || t.DurationWithoutCoach != null);
            }
            if(type == "group")
            {
                query = query.Where(t => t.Duration != null);
            }*/

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
                return NotFound();
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
            var isGroup = coachId == null;
            if (isGroup)
            {
                var sessions = _context.Sessions.Include(s => s.Coach).Include(s => s.Training).Where(s => s.Training.Id == training.Id && s.IsGroup == isGroup);
                var groupedDays = sessions.GroupBy(s => s.Start.Date).Select(g => new DayShedule
                {
                    Date = g.Key,
                    TimeSlots = g.Select(ts => new TimeSlot
                    {
                        Time = ts.Start.TimeOfDay,
                        CoachName = isGroup ? $"{ts.Coach.Name} {ts.Coach.Surname}" : null,
                        Capacity = isGroup ? ts.Capacity : null
                    }).ToList()
                })
                .OrderBy(d => d.Date).ToList();

                var model = new SheduleViewModel
                {
                    TrainingType = "group",
                    Days = groupedDays,
                    CoachId = null
                };

                return View(model);
            }
            else
            {
                var duration = coachId > 0 ? training.DurationWithCoach : training.DurationWithoutCoach;
                if(duration == null)
                {
                    return BadRequest("Duration not defined");
                }

                int startHour = 8;
                int endHour = 20;
                var now = DateTime.UtcNow;
                var days = new List<DayShedule>();

                List<TrainingSession> busySessions = new();
                if (coachId > 0)
                {
                    busySessions = _context.Sessions.Where(s => s.CoachId == coachId).ToList();
                }
                var sessionsByDayOfWeek = busySessions.GroupBy(s => s.Start.DayOfWeek).ToDictionary(
                    g => g.Key,
                    g => g.ToList());

                for(int i = 0; i < 7; i++)
                {
                    var date = now.AddDays(i);
                    var dayOfWeek = date.DayOfWeek;
                    sessionsByDayOfWeek.TryGetValue(dayOfWeek, out var sessionsForDay);
                    var busySlots = sessionsForDay?.Select(s => new { Start = s.Start.TimeOfDay, End = s.End.TimeOfDay }).ToList();
                    var timeSlots = new List<TimeSlot>();
                    for(var time = TimeSpan.FromHours(startHour); (time + duration) <= TimeSpan.FromHours(endHour); time += duration.Value)
                    {
                        bool isBusy = false;
                        if(busySlots != null)
                        {
                            foreach(var busy in busySlots)
                            {
                                if(time < busy.End && (time + duration) > busy.Start)
                                {
                                    isBusy = true;
                                    break;
                                }
                            }
                        }
                        if(!isBusy)
                        {
                            timeSlots.Add(new TimeSlot
                            {
                                Time = time,
                                CoachName = null,
                                Capacity = 0
                            });
                        }
                    }
                    if(timeSlots.Any())
                    {
                        days.Add(new DayShedule
                        {
                            Date = date,
                            TimeSlots = timeSlots
                        });
                    }
                }
                var model = new SheduleViewModel
                {
                    TrainingType = "individual",
                    Days = days,
                    CoachId = coachId
                };
                return View(model);
            }
        }
    }
}
