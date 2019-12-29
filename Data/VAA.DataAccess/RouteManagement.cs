using System;
using System.Collections.Generic;
using System.Linq;
using VAA.DataAccess.Interfaces;
using VAA.DataAccess.Model;
using VAA.Entities.VAAEntity;

namespace VAA.DataAccess
{
    /// <summary>
    /// Route management class - Handles Route get, add, edit, delete of menu various route related entities
    /// </summary>
    public class RouteManagement : IRoute
    {
        readonly VAAEntities _context = new VAAEntities();

        public IQueryable<Route> GetAllRoutes()
        {
            throw new NotImplementedException();
        }

        public Route GetRouteById(long routeId)
        {
            var route = _context.tRouteDetails.Where(r => r.RouteID == routeId).FirstOrDefault();

            var arrivalLocation = _context.tLocation.Where(l => l.LocationID == route.ArrivalID).FirstOrDefault();
            var departureLocation = _context.tLocation.Where(l => l.LocationID == route.DepartureID).FirstOrDefault();
            var region = _context.tRegion.Where(r => r.ID == route.RegionID).FirstOrDefault();

            var routeModel = new Route();
            routeModel.RouteId = route.RouteID;
            routeModel.ArrivalId = route.ArrivalID;
            routeModel.DepartureId = route.DepartureID;
            routeModel.ArrivalAirportCode = arrivalLocation.AirportCode;
            routeModel.DepartureAirportCode = departureLocation.AirportCode;

            return routeModel;
        }

        public long GetRouteId(string departure, string arrival)
        {
            var departureId =
                (from l in _context.tLocation where l.AirportCode == departure select l.LocationID).FirstOrDefault();

            var arrivalId =
               (from l in _context.tLocation where l.AirportCode == arrival select l.LocationID).FirstOrDefault();

            if (departureId == 0)
            {

                var loc = new tLocation
                {
                    AirportCode = departure,
                    AirportName = departure
                };


                _context.tLocation.Add(loc);
                _context.SaveChanges();
                departureId = loc.LocationID;
            }

            if (arrivalId == 0)
            {
                var loc = new tLocation
                {
                    AirportCode = arrival,
                    AirportName = arrival
                };

                _context.tLocation.Add(loc);
                _context.SaveChanges();
                arrivalId = loc.LocationID;

            }

            var routeId =
                (from r in _context.tRouteDetails
                 where r.DepartureID == departureId && r.ArrivalID == arrivalId
                 select r.RouteID).FirstOrDefault();

            var notAssignedRegionId = (from r in _context.tRegion where r.RegionName == "NA" select r.ID).FirstOrDefault();

            if (routeId == 0)
            {
                var route = new tRouteDetails
                {
                    ArrivalID = arrivalId,
                    DepartureID = departureId,
                    RegionID = notAssignedRegionId
                };

                _context.tRouteDetails.Add(route);
                _context.SaveChanges();

                routeId = route.RouteID;
            }

            return routeId;
        }

        public Route GetRouteByFlightNo(string flightNo)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Route> GetRouteByRegion(int regionId)
        {
            throw new NotImplementedException();
        }



        public IQueryable<Route> GetRouteByCycle(int cycleId)
        {
            throw new NotImplementedException();
        }

        public int AddRoute(Route route)
        {
            throw new NotImplementedException();
        }

        public bool EditRoute(Route route)
        {
            throw new NotImplementedException();
        }

        public bool DeleteRoute(Route route)
        {
            throw new NotImplementedException();
        }

        public IQueryable<MenuData> GetMenuForRoute(int routeId)
        {
            throw new NotImplementedException();
        }

        public List<tLocation> GetAllLocations()
        {
            return _context.tLocation.OrderBy(x => x.AirportCode).ToList();
        }

        public List<tLocation> GetArrivalsForDeparture(int departureLocationId)
        {
            var routes = _context.tRouteDetails.Where(t => t.DepartureID == departureLocationId).ToList();

            return routes.Select(route => _context.tLocation.FirstOrDefault(l => l.LocationID == route.ArrivalID)).Where(location => location != null).OrderBy(x => x.AirportCode).ToList();

        }

        public List<tLocation> GetDeparturesForArrival(int arrivalLocationId)
        {
            var routes = _context.tRouteDetails.Where(t => t.ArrivalID == arrivalLocationId).ToList();

            return routes.Select(route => _context.tLocation.FirstOrDefault(l => l.LocationID == route.DepartureID)).Where(location => location != null).OrderBy(x => x.AirportCode).ToList();

        }

        public List<tFlightSchedule> GetAllFlightNo()
        {
            var flights = (from f in _context.tFlightSchedule
                           where f.FlightNo != "IGNORE"  
                           orderby f.FlightNo.Substring(2,5)
                           select new
                           {
                               flightno = f.FlightNo
                           }).Distinct().ToList();

            return (from x in flights                    
                    select new tFlightSchedule
                    {
                        FlightNo = x.flightno
                    }).OrderBy(y => y.FlightNo).ToList();
        }

        public List<tFlightSchedule> GetFlightNoByArrival(int arrivalLocationId)
        {
            var flights = (from f in _context.tFlightSchedule
                           join r in _context.tRouteDetails on f.RouteID equals r.RouteID
                           where r.ArrivalID == arrivalLocationId && f.FlightNo != "IGNORE"
                           orderby f.FlightNo.Substring(2, 5)
                           select new
                           {
                               flightno = f.FlightNo
                           }).Distinct().ToList();

            return (from x in flights
                    select new tFlightSchedule
                    {
                        FlightNo = x.flightno
                    }).OrderBy(y => y.FlightNo).ToList();
        }

        public List<tFlightSchedule> GetFlightNoByDeparture(int departureLocationId)
        {
            var flights = (from f in _context.tFlightSchedule
                           join r in _context.tRouteDetails on f.RouteID equals r.RouteID
                           where r.DepartureID == departureLocationId && f.FlightNo != "IGNORE"
                           orderby f.FlightNo.Substring(2, 5)
                           select new
                           {
                               flightno = f.FlightNo
                           }).Distinct().ToList();

            return (from x in flights
                    select new tFlightSchedule
                    {
                        FlightNo = x.flightno
                    }).OrderBy(y => y.FlightNo).ToList();
        }

        public List<tFlightSchedule> GetFlightNoByRoute(int departureLocationId, int arrivalLocationId)
        {
            var flights = (from f in _context.tFlightSchedule
                           join r in _context.tRouteDetails on f.RouteID equals r.RouteID
                           where r.DepartureID == departureLocationId && r.ArrivalID == arrivalLocationId && f.FlightNo != "IGNORE"
                           orderby f.FlightNo.Substring(2, 5)
                           select new
                           {
                               flightno = f.FlightNo
                           }).Distinct().ToList();

            return (from x in flights
                    select new tFlightSchedule
                    {
                        FlightNo = x.flightno
                    }).OrderBy(y => y.FlightNo).ToList();
        }

        public void ClearFlightSchedules()
        {
            var schedules = (from fs in _context.tFlightSchedule select fs.ID).ToList();

            for (int i = 0; i < schedules.Count; i++)
            {
                var id = schedules[i];
                var fSchedule =
                    (from fs in _context.tFlightSchedule where fs.ID == id select fs).FirstOrDefault();

                _context.tFlightSchedule.Remove(fSchedule);

                _context.SaveChanges();

            }
        }

        public void AddSchedule(tFlightSchedule flightSchedule)
        {

            //var existingSchedule = (from fs in _context.tFlightSchedule where fs.FlightNo == flightSchedule.FlightNo && fs.RouteID == flightSchedule.RouteID && fs.Bound == flightSchedule.Bound && fs.EquipmentType == flightSchedule.EquipmentType select fs).FirstOrDefault();

            var existingSchedule = new tFlightSchedule();

            //if (existingSchedule != null)
            //{

            existingSchedule.FlightNo = flightSchedule.FlightNo;
            existingSchedule.ArrivalTime = flightSchedule.ArrivalTime;
            existingSchedule.DepartureTime = flightSchedule.DepartureTime;
            existingSchedule.RouteID = flightSchedule.RouteID;
            existingSchedule.Effective = flightSchedule.Effective;
            existingSchedule.Discontinued = flightSchedule.Discontinued;
            existingSchedule.Bound = flightSchedule.Bound;
            existingSchedule.EquipmentType = flightSchedule.EquipmentType;
            existingSchedule.Monday = flightSchedule.Monday;
            existingSchedule.Tuesday = flightSchedule.Tuesday;
            existingSchedule.Wednesday = flightSchedule.Wednesday;
            existingSchedule.Thursday = flightSchedule.Thursday;
            existingSchedule.Friday = flightSchedule.Friday;
            existingSchedule.Saturday = flightSchedule.Saturday;
            existingSchedule.Sunday = flightSchedule.Sunday;
            //_context.SaveChanges();
            //}
            //else
            //{
            _context.tFlightSchedule.Add(flightSchedule);
            _context.SaveChanges();
            //}
        }


        public List<tFlightSchedule> GetFullFlightSchdules()
        {
            return _context.tFlightSchedule.ToList();

        }
        public tFlightSchedule GetSchedule(string flightNo, DateTime fromDate, DateTime toDate)
        {
            return (from fs in _context.tFlightSchedule where fs.FlightNo == flightNo && fs.Effective <= fromDate && fs.Discontinued >= toDate select fs).FirstOrDefault();
        }

        public tFlightSchedule GetScheduleMonday(string flightNo, DateTime date)
        {
            return (from fs in _context.tFlightSchedule where fs.FlightNo == flightNo && fs.Effective <= date && fs.Discontinued >= date && fs.Monday == true select fs).FirstOrDefault();
        }

        public tFlightSchedule GetScheduleTuesday(string flightNo, DateTime date)
        {
            return (from fs in _context.tFlightSchedule where fs.FlightNo == flightNo && fs.Effective <= date && fs.Discontinued >= date && fs.Tuesday == true select fs).FirstOrDefault();
        }
        public tFlightSchedule GetScheduleWednesday(string flightNo, DateTime date)
        {
            return (from fs in _context.tFlightSchedule where fs.FlightNo == flightNo && fs.Effective <= date && fs.Discontinued >= date && fs.Wednesday == true select fs).FirstOrDefault();
        }
        public tFlightSchedule GetScheduleThursday(string flightNo, DateTime date)
        {
            return (from fs in _context.tFlightSchedule where fs.FlightNo == flightNo && fs.Effective <= date && fs.Discontinued >= date && fs.Thursday == true select fs).FirstOrDefault();
        }

        public tFlightSchedule GetScheduleFriday(string flightNo, DateTime date)
        {
            return (from fs in _context.tFlightSchedule where fs.FlightNo == flightNo && fs.Effective <= date && fs.Discontinued >= date && fs.Friday == true select fs).FirstOrDefault();
        }

        public tFlightSchedule GetScheduleSaturday(string flightNo, DateTime date)
        {
            return (from fs in _context.tFlightSchedule where fs.FlightNo == flightNo && fs.Effective <= date && fs.Discontinued >= date && fs.Saturday == true select fs).FirstOrDefault();
        }

        public tFlightSchedule GetScheduleSunday(string flightNo, DateTime date)
        {
            return (from fs in _context.tFlightSchedule where fs.FlightNo == flightNo && fs.Effective <= date && fs.Discontinued >= date && fs.Sunday == true select fs).FirstOrDefault();
        }

        public bool IsDomesticRoute(string departure, string arrival)
        {
            var dometicRoute = new[]
            {
                "LGW",
                "GLA",
                "LHR",
                "EDI",
                "ABZ"
            };


            if (dometicRoute.Contains(departure) && dometicRoute.Contains(arrival))
                return true;

            return false;
        }

        public int UpperClassCapacity(string equipment)
        {
            var seatConfiguration = (from s in _context.tSeatConfiguration where s.Equipment == equipment select s).FirstOrDefault();

            if (seatConfiguration != null)
                return Convert.ToInt32(seatConfiguration.UC);

            return 0;

        }

        public int PremiumEcoClassCapacity(string equipment)
        {
            var seatConfiguration = (from s in _context.tSeatConfiguration where s.Equipment == equipment select s).FirstOrDefault();

            if (seatConfiguration != null)
                return Convert.ToInt32(seatConfiguration.PE);

            return 0;
        }
        public int EcoClassCapacity(string equipment)
        {
            var seatConfiguration = (from s in _context.tSeatConfiguration where s.Equipment == equipment select s).FirstOrDefault();

            if (seatConfiguration != null)
                return Convert.ToInt32(seatConfiguration.Eco);

            return 0;

        }

        public bool CheckEquipmentType(string equipmentCode)
        {
            var seatConfiguration = _context.tSeatConfiguration.Where(s => s.Equipment == equipmentCode).FirstOrDefault();

            if (seatConfiguration == null)
                return false;
            else
                return true;
        }
    }
}
