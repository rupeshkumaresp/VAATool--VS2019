using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VAA.DataAccess.Model
{
    public class BoxTicketSortedData
    {
        public long ID { get; set; }
        public string ClassName { get; set; }
        public string ShipTo { get; set; }
        public Nullable<int> ClassId { get; set; }
        public string FlightNo { get; set; }
        public Nullable<System.DateTime> Date { get; set; }
        public string Time { get; set; }
        public string Route { get; set; }
        public Nullable<int> Count { get; set; }
        public string Bound { get; set; }
        public string MenuCode { get; set; }
        public string LoadingFlight { get; set; }
        public Nullable<long> RouteId { get; set; }
        public string BRKMenuCode { get; set; }
        public string TEAMenuCode { get; set; }
    }
}
