using Application.Interface.Service;
using Domain.Enum.Task;

namespace Infrastructure.Implement.Service
{
    public class EnumService : IEnumService
    {
        public Dictionary<int, string> GetPriorities()
        {
            var priority = new Dictionary<int, string>();
            foreach (Priority value in Enum.GetValues(typeof(Priority)))
            {
                priority.Add((int)value, value.ToString());
            }

            return priority;
        }

        public Dictionary<int, string> GetStatus()
        {
            var status = new Dictionary<int, string>();
            foreach (Status value in Enum.GetValues(typeof(Status)))
            {
                status.Add((int)value, value.ToString());
            }

            return status;
        }
    }
}
