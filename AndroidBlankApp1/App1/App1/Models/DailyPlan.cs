using System;
using SQLite;

namespace App1.Models
{
    [Table("DailyPlans")]
    public class DailyPlan: IHasId<int>
    {
        public string Name { get; set; }
        
        public int WeekId { get; set; }
        public DateTime Date { get; set; }
        public DayOfWeek Day { get; set; }

        public bool IsExpired { get; set; }
        public bool IsScratch { get; set; }

        public DailyPlan(): base()
        {
            IsScratch = true;
            IsExpired = false;
        }
    }
}