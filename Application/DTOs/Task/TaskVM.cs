namespace Application.DTOs.Task
{
#pragma warning disable CS8618
    public class TaskVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset ModifiedAt { get; set; }
        public DateTimeOffset ExpiredAt { get; set; }
        public string Priority { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string ImageUrl { get; set; }
        public bool IsCompleted { get; set; }
        //
        public int CreatedBy { get; set; }
    }
}
