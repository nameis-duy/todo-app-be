using Application.Constant;

namespace Application.Interface.Service
{
    public interface ITimeService
    {
        DateTime GetCurrentUtcDatetime();
        DateTime GetCurrentLocalDateTime();
        DateTime ConvertFromUtcToLocalDateTime(DateTime utcDateTime, string timezoneId = ConfigConstant.VIETNAM_TIME_ZONE);
    }
}
