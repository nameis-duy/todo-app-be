using Application.Constant;
using Application.Interface.Service;

namespace Infrastructure.Implement.Service
{
    public class TimeService : ITimeService
    {
        public DateTime ConvertFromUtcToLocalDateTime(DateTime utcDateTime, string timezoneId = ConfigConstant.VIETNAM_TIME_ZONE)
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
            return TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, timeZone);
        }

        public DateTime GetCurrentLocalDateTime() => DateTime.Now;

        public DateTime GetCurrentUtcDatetime() => DateTime.UtcNow;
    }
}
