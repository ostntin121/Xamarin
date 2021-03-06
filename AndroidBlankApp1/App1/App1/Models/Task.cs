﻿using SQLite;

namespace App1.Models
{
    [Table("Tasks")]
    public class Task: IHasId<int>
    {
        public string Name { get; set; }
        public int PlanId { get; set; }

        public string ImagePath { get; set; }

        public string AudioPath { get; set; }
        public bool IsCompleted { get; set; }
    }
}