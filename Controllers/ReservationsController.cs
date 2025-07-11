using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportCentrum.Context;
using SportCentrum.Models;
using SportCentrum.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace SportCentrum.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly SportCentrumContext _context;

        public ReservationsController(SportCentrumContext context)
        {
            _context = context;
        }

        public IActionResult MyReservations()
        {
            var userId = SimulatedAuth.GetUserId(_context);

            var reservations = _context.Reservations.Include(r => r.TrainingSession).ThenInclude(ts => ts.Training).Include(r => r.TrainingSession.Coach)
                .Where(r => r.UserId == userId).OrderBy(r => r.TrainingSession.Start).ToList();

            return View(reservations);
        }

        public IActionResult Create(string sport, int? coachId, DateTime date, TimeSpan time, string sessionId)
        {
            int userId = SimulatedAuth.GetUserId(_context);
            var training = _context.Trainings.FirstOrDefault(t => t.Name == sport);
            if (training == null)
            {
                return NotFound();
            }
            var start = date.Date + time;
            if (coachId > 0)
            {
                var duration = training.DurationWithCoach;
                if (duration == null)
                {
                    return BadRequest("Training duration is not set");
                }
                var end = start + duration.Value;
                var day = date.DayOfWeek;

                var newSession = new TrainingSession
                {
                    Id = SessionIdGenerator.Generate(training.Id, day, time),
                    TrainingId = training.Id,
                    CoachId = coachId,
                    Start = DateTime.SpecifyKind(start, DateTimeKind.Utc),
                    End = DateTime.SpecifyKind(end, DateTimeKind.Utc),
                    DayOfWeek = day.ToString(),
                    IsGroup = false,
                    Capacity = 1

                };
                _context.Sessions.Add(newSession);
                _context.SaveChanges();
                sessionId = newSession.Id;
            }
        
            var session = _context.Sessions.FirstOrDefault(s => s.Id == sessionId);
            if (session == null)
            {
                return NotFound("Session is not found");
            }
            bool alreadyReserved = _context.Reservations.Any(r => r.UserId == userId && r.TrainingSessionId == sessionId);
            if (alreadyReserved)
            {
                TempData["Error"] = "You have already reserved this session";
                return RedirectToAction("MyReservations");
            }
            var reservation = new TrainingReservation
            {
                UserId = userId,
                TrainingSessionId = sessionId,
                ReservationTime = DateTime.UtcNow
            };
            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            session.Capacity -= 1;
            _context.Sessions.Update(session);
            _context.SaveChanges();
           
            TempData["Success"] = "Session successfully reserved!";
            return RedirectToAction("MyReservations");
        }

        public IActionResult Cancel(string sessionId)
        {
            var userId = SimulatedAuth.GetUserId(_context);
            var reservation = _context.Reservations.FirstOrDefault(r => r.TrainingSessionId == sessionId && r.UserId == userId);
            if(reservation == null)
            {
                return NotFound();
            }
            _context.Reservations.Remove(reservation);
            _context.SaveChanges();

            var session = _context.Sessions.FirstOrDefault(s => s.Id == sessionId);
            if(session != null) 
            {
                if (!session.IsGroup && session.CoachId != null)
                {
                    _context.Sessions.Remove(session);
                    _context.SaveChanges();
                }
                else
                {
                    session.Capacity += 1;
                    _context.Sessions.Update(session);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("MyReservations");
        }
    }
}
