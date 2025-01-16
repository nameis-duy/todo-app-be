namespace Application.Interface.Service
{
    public interface ITimeService
    {
        DateTime GetCurrentUtcDatetime();
        DateTime ConvertFromUtcToLocalDateTime(DateTime utcDateTime, string timezoneId = "vi-Vn");
    }
}
