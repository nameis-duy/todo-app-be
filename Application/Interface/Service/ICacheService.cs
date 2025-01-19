namespace Application.Interface.Service
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T data, int minutesValid);
    }
}
