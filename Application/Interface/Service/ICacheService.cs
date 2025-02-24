namespace Application.Interface.Service
{
    public interface ICacheService
    {
        T? Get<T>(string key);
        void Set<T>(string key, T data, int minutesValid);
    }
}
