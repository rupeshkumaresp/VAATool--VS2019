using System;
using System.Collections.Generic;

namespace VAA.DataAccess.Model
{
    public class PdfGenerationJobPackingTicket
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public IEnumerable<PdfGenerationTaskPackingTicket> Tasks { get; set; }
        public int InstanceId { get; set; }
    }
}