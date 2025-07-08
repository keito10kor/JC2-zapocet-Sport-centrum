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
            IQueryable<Training> query = _context.Trainings;
            if(type == "individual")
            {
                query = query.Where(t => t.DurationWithCoach != null || t.DurationWithoutCoach != null);
            }
            if(type == "group")
            {
                query = query.Where(t => t.Duration != null);
            }

            var sportNames = query.Select(t => t.Name).Distinct().ToList();
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

        public IActionResult SelectDate(string sport, int? coachId = null)
        {
            var training = _context.Trainings.FirstOrDefault(t => t.Name == sport);
            if(training == null)
            {
                return NotFound();
            }
            var isGroup = coachId == null;
            var sessions = _context.Sessions.Include(s => s.Coach).Include(s => s.Training).Where(s => s.Training.Id == training.Id && s.IsGroup == isGroup);
            if(!isGroup)
            {
                sessions = sessions.Where(s => s.CoachId == (coachId > 0 ? coachId : null));
            }
            var sessionsList = sessions.ToList();
            var groupedDays = sessionsList.GroupBy(s => s.Start.Date).Select(g => new DayShedule
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
                TrainingType = isGroup ? "group" : "individual",
                Days = groupedDays
            };

            return View(model);
        }
    }
}
