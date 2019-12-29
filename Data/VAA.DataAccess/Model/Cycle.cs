using System;

namespace VAA.DataAccess.Model
{
    public class Cycle
    {
        public long Id { get; set; }
        public int ? InstanceId { get; set; }
        public string CycleName { get; set; }
        public string ShortName { get; set; }
        public string Year { get; set; }
        public DateTime? CreatedDatetime { get; set; }
        public string RecordTitlePattern { get; set; }
        public string Status { get; set; }
        public bool ? IsLocked { get; set; }
        public bool ? Active { get; set; }
        public bool? Archived { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
