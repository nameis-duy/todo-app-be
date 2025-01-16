using Application.Interface.Service;

namespace Infrastructure.Implement.Service
{
    public class TimeService : ITimeService
    {
        public DateTime ConvertFromUtcToLocalDateTime(DateTime utcDateTime, string timezoneId = "vi-Vn")
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZone);
        }

        public DateTime GetCurrentUtcDatetime() => DateTime.UtcNow;
    }
}
