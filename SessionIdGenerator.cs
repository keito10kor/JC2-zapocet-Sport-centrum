using SportCentrum.Models;
using System.Data;
using System.Diagnostics.Metrics;

namespace SportCentrum
{
    public class SessionIdGenerator
    {
        public static string Generate(string trainingId, DayOfWeek day, TimeSpan time)
        {
            string[] dayShortNames = { "MO", "TU", "WE", "TH", "FR", "SA", "SU" };
            string dayShort = dayShortNames[(int)day];
            string timeStr = time.ToString(@"hhmm");
            string baseId = $"{trainingId}{dayShort}{timeStr}";
            string uniquePart = Guid.NewGuid().ToString("N").Substring(0, 5);
            return $"{baseId}_{uniquePart}";
        }
    }
}
