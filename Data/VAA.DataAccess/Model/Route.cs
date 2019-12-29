namespace VAA.DataAccess.Model
{
    public class Route
    {
        public long RouteId { get; set; }
        public int DepartureId { get; set; }

        public int DepartureRegionId { get; set; }
        public string DepartureRegionName { get; set; }

        public string DepartureAirportName { get; set; }

        public string DepartureAirportCode { get; set; }
        public int ArrivalId { get; set; }

        public int ArrivalRegionId { get; set; }
        public string ArrivalRegionName { get; set; }

        public string ArrivalAirportName { get; set; }
        public string ArrivalAirportCode { get; set; }

        public string DepartureTime { get; set; }
        public string ArrivialTime { get; set; }

        public string SequenceNumber { get; set; }
        public string FlightNo { get; set; }
        public int SeatConfigurationId { get; set; }

        public int SeatUpperClass { get; set; }
        public int SeatPremiumEconomy { get; set; }
        public int SeatEconomy { get; set; }

    }
}
