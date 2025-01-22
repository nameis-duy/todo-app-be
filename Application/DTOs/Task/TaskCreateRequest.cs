﻿using Domain.Enum.Task;

namespace Application.DTOs.Task
{
#pragma warning disable CS8618
    public class TaskCreateRequest
    {
        public string Title { get; set; }
        public DateTime ExpiredAt { get; set; }
        public Priority Priority { get; set; }
        public string Description { get; set; }
    }
}
