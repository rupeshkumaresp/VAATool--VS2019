using System;

namespace VAA.DataAccess.Model
{
    public class Schedules
    {
        public int ID { get; set; }
        public string Subject { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string UserID { get; set; }
        public string RecurrenceRule { get; set; }
        public int? RecurrenceParentID { get; set; }
        public string Annotations { get; set; }
        public string Description { get; set; }
        public string Remainder { get; set; }
        public bool? Completed { get; set; }
        public int? ColorID { get; set; } 
    }
}
