namespace Application.Interface.Service
{
    public interface IEnumService
    {
        Dictionary<int, string> GetPriorities();
        Dictionary<int, string> GetStatus();
    }
}
