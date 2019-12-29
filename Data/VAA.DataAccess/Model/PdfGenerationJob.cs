using System;
using System.Collections.Generic;

namespace VAA.DataAccess.Model
{
    public class PdfGenerationJob
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public IEnumerable<PdfGenerationTask> Tasks { get; set; }
        public int InstanceId { get; set; }
    }
}