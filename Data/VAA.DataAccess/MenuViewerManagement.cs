using System;
using System.Collections.Generic;
using System.Linq;
using VAA.DataAccess.Interfaces;
using VAA.DataAccess.Model;
using VAA.Entities.VAAEntity;

namespace VAA.DataAccess
{
    /// <summary>
    /// MenuViewer management class - Handles all the grid functionalities in the menuviewer/cabincrew site
    /// </summary>
    public class MenuViewerManagement : IMenuViewer
    {
        readonly VAAEntities _context = new VAAEntities();

        public List<MenuData> GetListForMenuViewer(long cycle, int menuclass, int menutype, int departure, int arrival, string flightno)
        {
            try
            {
                if (flightno != null)
                {
                    var data = (from m in _context.tMenu
                                join cmtm in _context.tClassMenuTypeMap on m.MenuTypeID equals cmtm.MenuTypeID
                                join mfr in _context.tMenuForRoute on m.ID equals mfr.MenuID
                                join rd in _context.tRouteDetails on mfr.RouteID equals rd.RouteID
                                where (m.CycleID == cycle || cycle == 0) && (cmtm.FlightClassID == menuclass || menuclass == 0) && (m.MenuTypeID == menutype || menutype == 0) && (rd.DepartureID == departure || departure == 0) && (rd.ArrivalID == arrival || arrival == 0) && m.MenuName.Contains(flightno) && mfr.Flights.Contains(flightno)
                                select new
                                {
                                    FlightNo = mfr.Flights,
                                    CycleName = (from cy in _context.tCycle
                                                 where cy.CycleID == m.CycleID
                                                 select cy.CycleName).FirstOrDefault(),
                                    ClassName = (from cl in _context.tClass
                                                 join cmtmp in _context.tClassMenuTypeMap on cl.ID equals cmtmp.FlightClassID
                                                 where cmtmp.MenuTypeID == m.MenuTypeID
                                                 select cl.FlightClass).FirstOrDefault(),
                                    MenuTypeName = (from mt in _context.tMenuType
                                                    where mt.ID == m.MenuTypeID
                                                    select mt.DisplayName).FirstOrDefault(),
                                    FromRoute = (from l in _context.tLocation
                                                 where l.LocationID == rd.DepartureID
                                                 select l.AirportCode).FirstOrDefault(),
                                    ToRoute = (from l in _context.tLocation
                                               where l.LocationID == rd.ArrivalID
                                               select l.AirportCode).FirstOrDefault(),
                                    MenuId = m.ID,
                                    MenuCode = m.MenuCode
                                }).ToList();
                    return (from x in data
                            select new MenuData
                            {
                                FlightNo = x.FlightNo,
                                CycleName = x.CycleName,
                                ClassName = x.ClassName,
                                MenuTypeName = x.MenuTypeName,
                                Route = x.FromRoute + "-" + x.ToRoute,
                                Id = x.MenuId,
                                MenuCode = x.MenuCode
                            }).ToList();
                }
                else
                {
                    var data = (from m in _context.tMenu
                                            join cmtm in _context.tClassMenuTypeMap on m.MenuTypeID equals cmtm.MenuTypeID
                                            join mfr in _context.tMenuForRoute on m.ID equals mfr.MenuID
                                            join rd in _context.tRouteDetails on mfr.RouteID equals rd.RouteID
                                            where (m.CycleID == cycle || cycle == 0) && (cmtm.FlightClassID == menuclass || menuclass == 0) && (m.MenuTypeID == menutype || menutype == 0) && (rd.DepartureID == departure || departure == 0) && (rd.ArrivalID == arrival || arrival == 0) 
                                            select new
                                            {
                                                FlightNo = mfr.Flights,
                                                CycleName = (from cy in _context.tCycle
                                                             where cy.CycleID == m.CycleID
                                                             select cy.CycleName).FirstOrDefault(),
                                                ClassName = (from cl in _context.tClass
                                                             join cmtmp in _context.tClassMenuTypeMap on cl.ID equals cmtmp.FlightClassID
                                                             where cmtmp.MenuTypeID == m.MenuTypeID
                                                             select cl.FlightClass).FirstOrDefault(),
                                                MenuTypeName = (from mt in _context.tMenuType
                                                                where mt.ID == m.MenuTypeID
                                                                select mt.DisplayName).FirstOrDefault(),
                                                FromRoute = (from l in _context.tLocation
                                                             where l.LocationID == rd.DepartureID
                                                             select l.AirportCode).FirstOrDefault(),
                                                ToRoute = (from l in _context.tLocation
                                                           where l.LocationID == rd.ArrivalID
                                                           select l.AirportCode).FirstOrDefault(),
                                                MenuId = m.ID,
                                                MenuCode = m.MenuCode
                                            }).ToList();
                                return (from x in data
                                        select new MenuData
                                        {
                                            FlightNo = x.FlightNo,
                                            CycleName = x.CycleName,
                                            ClassName = x.ClassName,
                                            MenuTypeName = x.MenuTypeName,
                                            Route = x.FromRoute + "-" + x.ToRoute,
                                            Id = x.MenuId,
                                            MenuCode = x.MenuCode
                                        }).ToList();
                            
                }
                return new List<MenuData>();
            }
            catch (Exception e)
            {
                //write to Elma
               // ErrorSignal.FromCurrentContext().Raise(e);

                return new List<MenuData>();
            }
        }

    }
}
