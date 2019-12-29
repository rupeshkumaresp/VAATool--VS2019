using System;
using System.Collections.Generic;
using System.Linq;
using VAA.DataAccess.Model;
using VAA.Entities.VAAEntity;

namespace VAA.DataAccess.Interfaces
{
    /// <summary>
    /// Route related operations
    /// </summary>
    public interface IRoute
    {
        //route
        IQueryable<Route> GetAllRoutes();
        Route GetRouteById(long routeId);
        long GetRouteId(string departure, string arrival);
        Route GetRouteByFlightNo(string flightNo);
        IQueryable<Route> GetRouteByRegion(int regionId);
        IQueryable<Route> GetRouteByCycle(int cycleId);
        int AddRoute(Route route);
        bool EditRoute(Route route);
        bool DeleteRoute(Route route);
        IQueryable<MenuData> GetMenuForRoute(int routeId);

        //location
        List<tLocation> GetAllLocations();
        List<tLocation> GetArrivalsForDeparture(int departureLocationId);
        List<tLocation> GetDeparturesForArrival(int arrivalLocationId);

        //flight number
        List<tFlightSchedule> GetAllFlightNo();
        List<tFlightSchedule> GetFlightNoByArrival(int arrivalLocationId);
        List<tFlightSchedule> GetFlightNoByDeparture(int departureLocationId);
        List<tFlightSchedule> GetFlightNoByRoute(int departureLocationId, int arrivalLocationId);


        //schedule
        void ClearFlightSchedules();
        void AddSchedule(tFlightSchedule flightSchedule);
        tFlightSchedule GetSchedule(string flightNo, DateTime fromDate, DateTime toDate);
        bool IsDomesticRoute(string departure, string arrival);

        //seatconfiguration
        int UpperClassCapacity(string equipment);
        int PremiumEcoClassCapacity(string equipment);
        int EcoClassCapacity(string equipment);
    }
}