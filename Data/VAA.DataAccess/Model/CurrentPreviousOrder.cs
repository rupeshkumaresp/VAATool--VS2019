using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAA.DataAccess.Model
{
    public class CurrentPreviousOrder
    {
        public Int64 OrderRowId { get; set; }
        public Int64 OrderId { get; set; }
        public DateTime? OrderDate { get; set; }
        public string LOTNumber { get; set; }
        public string Cycle { get; set; }
        public int? OrderStatusId { get; set; }
        public string OrderStatus { get; set; }
        public string FriendlyName { get; set; }
        public DateTime? CompletedDate { get; set; }
    }

    public class OrderStatusList
    {
        public int StatusId { get; set; }
        public string Status { get; set; }
    }

}
