﻿using Domain.Enum.Task;

namespace Application.DTOs.Task
{
#pragma warning disable CS8618
    public class TaskUpdateRequest : BaseTaskUpdateRequest
    {
        public string Title { get; set; }
        public DateTime ExpiredAtUtc { get; set; }
        public Priority Priority { get; set; }
        public string Description { get; set; }
        public Status Status { get; set; }
    }
}
