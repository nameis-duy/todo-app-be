﻿using Domain.Enum.Task;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entity
{
#pragma warning disable CS8618
    [Table("Task")]
    public class Tasks
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTimeOffset CreatedAt { get; }
        public DateTimeOffset ModifiedAt { get; }
        public DateTimeOffset ExpiredAt { get; set; }
        public Priority Priority { get; set; }
        public string Description { get; set; }
        public Status Status { get; set; }
        public string ImageUrl { get; set; }
        public bool IsRemoved { get; set; }
        //
        public int CreatedBy { get; set; }
        public Account Account { get; set; }
    }
}
