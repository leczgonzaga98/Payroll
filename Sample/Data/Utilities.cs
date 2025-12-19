using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Sample.Models;
using System.Globalization;

namespace Sample.Data
{
    public class Utilities
    {
        public static string Generate(string employeeName, DateTime dateOfBirth)
        {
            // 1️⃣ First 3 letters of name (uppercase)
            string namePart = new string(
                employeeName
                    .Replace(" ", "")
                    .Take(3)
                    .ToArray()
            ).ToUpper();

            // Pad if name is shorter than 3 letters
            namePart = namePart.PadRight(3, 'X');

            // 2️⃣ Random 5-digit number (00000–99999)
            var random = new Random();
            string randomPart = random.Next(0, 100000).ToString("D5");

            // 3️⃣ DOB (ddMMMyyyy)
            string dobPart = dateOfBirth.ToString("ddMMMyyyy", CultureInfo.InvariantCulture);

            return $"{namePart}{randomPart}{dobPart}";
        }

        public static DateTime ConvertDateTime(string dateTime)
        {
            return DateTime.ParseExact(
                dateTime,
                "dd/MM/yyyy",              
                CultureInfo.InvariantCulture
            );
        }

        public static List<DateTime> GetFullWeekFromBirthday(DateTime birthday)
        {
            // 1️⃣ Map birthday to year 2011
            DateTime baseDate = new DateTime(2011, birthday.Month, birthday.Day);

            // 2️⃣ Find Monday of that week
            int diff = baseDate.DayOfWeek == DayOfWeek.Sunday
                ? -6
                : DayOfWeek.Monday - baseDate.DayOfWeek;

            DateTime monday = baseDate.AddDays(diff);

            // 3️⃣ Return all 7 days (Mon–Sun)
            var week = new List<DateTime>();

            for (int i = 0; i < 7; i++)
            {
                week.Add(monday.AddDays(i));
            }

            return week;
        }

        private static DayOfWeek[] GetPatternDays(string pattern)
        {
            return pattern.ToUpper() switch
            {
                "MWF" => new[] { DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday },
                "TTHS" => new[] { DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Saturday },
                _ => throw new ArgumentException("Invalid pattern")
            };
        }

        public static PatternResult GetPatternWeek(DateTime birthday, string pattern)
        {
            DateTime baseDate = new DateTime(2011, birthday.Month, birthday.Day);

            // Monday of the week
            int diffToMonday = baseDate.DayOfWeek == DayOfWeek.Sunday
                ? -6
                : DayOfWeek.Monday - baseDate.DayOfWeek;
            DateTime weekStart = baseDate.AddDays(diffToMonday);

            // Prevent start date going to previous month
            if (weekStart.Month < baseDate.Month)
                weekStart = new DateTime(baseDate.Year, baseDate.Month, 1);

            var patternDays = GetPatternDays(pattern);

            // Find first pattern day
            DateTime firstPatternDate = weekStart;
            while (!patternDays.Contains(firstPatternDate.DayOfWeek))
                firstPatternDate = firstPatternDate.AddDays(1);

            // Collect pattern days until Friday of the last week
            List<DateTime> patternDates = new List<DateTime>();
            DateTime current = firstPatternDate;

            // Limit: we never go more than 14 days (2 weeks) to avoid infinite loop
            int maxDays = 14;
            int daysChecked = 0;

            while (patternDates.Count < patternDays.Length && daysChecked < maxDays)
            {
                if (patternDays.Contains(current.DayOfWeek))
                    patternDates.Add(current);

                current = current.AddDays(1);
                daysChecked++;
            }

            // Ensure end date is Friday after last pattern day
            DateTime lastPatternDate = patternDates.Last();
            int daysToFriday = DayOfWeek.Friday - lastPatternDate.DayOfWeek;
            if (daysToFriday < 0) daysToFriday += 7;

            DateTime endDate = lastPatternDate.AddDays(daysToFriday);

            return new PatternResult
            {
                StartDate = patternDates.First(),
                EndDate = endDate,
                DaysInPattern = CountDaysInPattern(patternDates.First(), endDate, pattern).DaysInPattern
            };
        }


        public static (int DaysInPattern, List<DateTime> PatternDates) CountDaysInPattern(DateTime startDate, DateTime endDate, string pattern)
        {
            if (endDate < startDate)
                throw new ArgumentException("EndDate must be after StartDate");

            var patternDays = GetPatternDays(pattern);
            List<DateTime> matchedDates = new List<DateTime>();

            DateTime current = startDate;
            while (current <= endDate)
            {
                if (patternDays.Contains(current.DayOfWeek))
                    matchedDates.Add(current);

                current = current.AddDays(1);
            }

            return (matchedDates.Count, matchedDates);
        }
    }
}
