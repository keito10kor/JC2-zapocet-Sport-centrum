using System;
using SportCentrum.Context;
using SportCentrum.Models;
namespace SportCentrum
{
    public static class SimulatedAuth
    {
        private static int? _userId;
        public static int GetUserId(SportCentrumContext context)
        {
            if (_userId == null)
            {
                var users = context.Users.Select(u => u.Id).ToList();
                var rnd = new Random();
                _userId = users[rnd.Next(users.Count)];
            }
            return _userId.Value;
        }
    }
}
