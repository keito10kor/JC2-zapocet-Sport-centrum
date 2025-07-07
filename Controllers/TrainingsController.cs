using Microsoft.AspNetCore.Mvc;
using SportCentrum.Context;
using SportCentrum.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sport_centrum.Controllers
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
    }
}
