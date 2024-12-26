using System.Globalization;
using System.Text.RegularExpressions;

namespace DateKeywordParser
{
    public static class DateKeywordParser
    {
        public static string Parse(string template, string? dateFormat = null, IFormatProvider? formatProvider = null)
        {
            try
            {
                if (string.IsNullOrEmpty(template))
                    throw new ArgumentNullException(nameof(template));

                formatProvider ??= CultureInfo.CurrentCulture;
                var regexMatch = GetRegexMatch(template);
                var periodKeyword = regexMatch.Groups[1].Value;
                var datePeriod = ResolveDateKeyword(periodKeyword, formatProvider);
                var replacePattern = @"<([a-zA-Z0-9+\-]+)>";
                var formattedDatePeriod = datePeriod.ToString(dateFormat, formatProvider);
                var parsedTemplate = Regex.Replace(template, replacePattern, formattedDatePeriod);
                return parsedTemplate;
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(ex.Message);
            }
            catch (FormatException ex)
            {
                throw new FormatException(ex.Message);
            }
            catch (Exception)
            {
                throw new Exception("Invalid expression");
            }
        }

        private static Match GetRegexMatch(string template)
        {
            string pattern = @"^(?:.*_)?<([a-zA-Z0-9+\-]+)>(?:_.*|\.\w+|)$";
            var match = Regex.Match(template, pattern);
            if (!match.Success)
            {
                throw new ArgumentException($"Invalid expression");
            }
            return match;
        }

        private static DateTime ResolveDateKeyword(string keyword, IFormatProvider formatProvider)
        {
            var pattern = @"^(NOW|YESTERDAY|TODAY|TOMORROW|NEXTWEEK|LASTWEEK)(([+-]\d+)(d|h|m|s))?(([+-]\d+)(d|h|m|s))?(([+-]\d+)(d|h|m|s))?(([+-]\d+)(d|h|m|s))?$";
            var match = Regex.Match(keyword, pattern, RegexOptions.IgnoreCase);

            var now = DateTime.Now;

            if (match.Success)
            {
                string firstUnit = match.Groups[4].Value.ToUpperInvariant();
                string secondUnit = match.Groups[7].Value.ToUpperInvariant();
                string thirdUnit = match.Groups[10].Value.ToUpperInvariant();
                string fourthUnit = match.Groups[13].Value.ToUpperInvariant();

                string[] units = [firstUnit, secondUnit, thirdUnit, fourthUnit];
                var duplicateUnits = units.Where(x => !string.IsNullOrEmpty(x))
                    .GroupBy(u => u).Where(g => g.Count() > 1).Select(g => g.Key);
                if (duplicateUnits.Any())
                {
                    throw new FormatException($"Duplicate unit(s): {string.Join(", ", duplicateUnits)}");
                }

                DateTime adjustedDate = match.Groups[1].Value switch
                {
                    "TODAY" => now.Date,
                    "TOMORROW" => now.Date.AddDays(1),
                    "YESTERDAY" => now.Date.AddDays(-1),
                    "NOW" => now,
                    "NEXTWEEK" => GetStartOfWeek(now.AddDays(7), formatProvider),
                    "LASTWEEK" => GetStartOfWeek(now.AddDays(-7), formatProvider),
                    _ => now.Date
                };

                if (!string.IsNullOrEmpty(match.Groups[2].Value))
                {
                    int offset = int.Parse(match.Groups[3].Value);
                    adjustedDate = AddDateOffset(adjustedDate, firstUnit, offset);
                }

                if (!string.IsNullOrEmpty(match.Groups[5].Value))
                {
                    int offset = int.Parse(match.Groups[6].Value);
                    adjustedDate = AddDateOffset(adjustedDate, secondUnit, offset);
                }

                if (!string.IsNullOrEmpty(match.Groups[8].Value))
                {
                    int offset = int.Parse(match.Groups[9].Value);
                    adjustedDate = AddDateOffset(adjustedDate, thirdUnit, offset);
                }

                if (!string.IsNullOrEmpty(match.Groups[11].Value))
                {
                    int offset = int.Parse(match.Groups[12].Value);
                    adjustedDate = AddDateOffset(adjustedDate, fourthUnit, offset);
                }

                return adjustedDate;
            }
            else
            {
                throw new ArgumentException($"Invalid expression");
            }
        }

        private static DateTime GetStartOfWeek(DateTime date, IFormatProvider formatProvider)
        {
            var culture = (CultureInfo)formatProvider;
            DayOfWeek firstDayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;
            int diff = date.DayOfWeek - firstDayOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return date.AddDays(-1 * diff).Date;
        }

        private static DateTime AddDateOffset(DateTime date, string unit, int offset)
        {
            switch (unit)
            {
                case "D":
                    return date.AddDays(offset);
                case "H":
                    return date.AddHours(offset);
                case "M":
                    return date.AddMinutes(offset);
                case "S":
                    return date.AddSeconds(offset);
                default:
                    throw new FormatException($"Invalid unit: {unit}");
            }
        }
    }
}
