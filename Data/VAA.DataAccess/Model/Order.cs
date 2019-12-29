using System;

namespace VAA.DataAccess.Model
{
    public class Order
    {
        public long Id { get; set; }
        public long MenuId { get; set; }
        public int OrderId { get; set; }
        public int PrintJobId { get; set; }

        public string Route { get; set; }
        public long RouteId { get; set; }
        public string ClassName { get; set; }
        public int ClassId { get; set; }
        public string MenuTypeName { get; set; }
        public int MenuTypeId { get; set; }
        public string CycleName { get; set; }
        public long CycleId { get; set; }
        public string MenuCode { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsDelivered { get; set; }
        public string InterlinkStatus { get; set; }
        public DateTime CompletedDate { get; set; }

        //Interlink delivery status
        public string CustomerRef { get; set; }
        public string Consignment { get; set; }
        public string Address { get; set; }
        public string PostCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

    }
}

