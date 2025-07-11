using SportCentrum.Context;

namespace Sport_centrum
{
    public static class DbInitializer
    {
        public static void ClearOldReservations(SportCentrumContext context)
        {
            var oldReservations = context.Reservations.ToList();

            if (oldReservations.Count == 0)
            {
                return;
            }
            foreach(var res in oldReservations)
            {
                var session = context.Sessions.FirstOrDefault(s => s.Id == res.TrainingSessionId);
                if(session != null)
                {
                    if(!session.IsGroup && session.CoachId != null)
                    {
                        context.Sessions.Remove(session);
                        context.SaveChanges();
                    }
                    else
                    {
                        session.Capacity += 1;
                        context.Sessions.Update(session);
                        context.SaveChanges();
                    }
                }
            }
            context.Reservations.RemoveRange(oldReservations);
            context.SaveChanges();
        }
    }
}
