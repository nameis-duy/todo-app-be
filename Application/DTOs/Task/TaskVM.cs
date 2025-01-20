namespace Application.DTOs.Task
{
    public class TaskVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAtUtc { get; }
        public DateTime ModifiedAtUtc { get; }
        public DateTime ExpiredAtUtc { get; set; }
        public string Priority { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        //
        public int CreatedBy { get; set; }
    }
}
