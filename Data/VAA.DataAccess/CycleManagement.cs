using System;
using System.Collections.Generic;
using System.Linq;
using VAA.DataAccess.Interfaces;
using VAA.DataAccess.Model;
using VAA.Entities.VAAEntity;

namespace VAA.DataAccess
{
    /// <summary>
    /// Cycle Management class - Handles cycle get, add, edit, delete
    /// </summary>
    public class CycleManagement : ICycle
    {
        private readonly VAAEntities _context = new VAAEntities();

        public List<Cycle> GetCycles()
        {
            try
            {
                var cycleData = (from cycle in _context.sp_GetAll_tCycle() select cycle).ToList();
                return (from data in cycleData
                        orderby data.StartDate descending
                        select new Cycle
                        {
                            Id = data.CycleID,
                            CycleName = data.CycleName,
                            ShortName = data.CycleShortName,
                            Year = data.Year,
                            StartDate = data.StartDate,
                            EndDate = data.EndDate
                        }).ToList();
            }
            catch (Exception ex)
            {
                return new List<Cycle>();
            }
        }


        public List<string> GetCycleWeek(long cycleId)
        {
            List<string> weeks = new List<string>();

            var cycleWeek = (from cw in _context.tCycleWeek where cw.CycleId == cycleId select cw).ToList();

            foreach (var week in cycleWeek)
            {
                weeks.Add(week.WeekNo + " (" + week.StartDate.ToShortDateString() + " - " + week.EndDate.ToShortDateString() + ")");
            }

            return weeks;
        }

        public Cycle GetCycle(long cycleId)
        {
            try
            {
                var cycleData = (from cycle in _context.tCycle where cycle.CycleID == cycleId select cycle).FirstOrDefault();
                if (cycleData != null)
                {
                    return new Cycle()
                    {
                        Id = cycleData.CycleID,
                        CycleName = cycleData.CycleName,
                        ShortName = cycleData.CycleShortName,
                        Year = cycleData.Year,
                        StartDate = cycleData.StartDate,
                        EndDate = cycleData.EndDate
                    };
                }
                else
                {
                    return new Cycle() { Id = 0 };
                }
            }
            catch (Exception ex)
            {
                return new Cycle() { Id = 0 };
            }
        }

        public Cycle CreateNewCycle(Cycle cycle)
        {
            try
            {
                tCycle addNewCycle = new tCycle()
                {
                    InstanceID = 1,
                    Year = cycle.Year,
                    CycleName = cycle.CycleName,
                    CycleShortName = cycle.ShortName,
                    //KeyFieldID = 0,
                    StartDate = cycle.StartDate,
                    EndDate = cycle.EndDate,
                    Active = cycle.Active,
                    Archived = false,
                    IsLocked = false,
                    ModifiedAt = DateTime.Now
                };
                _context.tCycle.Add(addNewCycle);
                _context.SaveChanges();


                //add cycle week
                var startDate = cycle.StartDate;
                var endDate = cycle.EndDate;

                TimeSpan timeSpan = (Convert.ToDateTime(endDate) - Convert.ToDateTime(startDate));

                var totalWeek = Convert.ToInt32(timeSpan.TotalDays / 7);

                for (int i = 1; i <= totalWeek; i++)
                {
                    var cycleWeek = new tCycleWeek();
                    cycleWeek.CycleId = addNewCycle.CycleID;
                    cycleWeek.StartDate = Convert.ToDateTime(startDate).AddDays((i - 1) * 7);
                    cycleWeek.EndDate = cycleWeek.StartDate.AddDays(6);
                    cycleWeek.WeekNo = i;

                    _context.tCycleWeek.Add(cycleWeek);
                    _context.SaveChanges();

                }

                return new Cycle()
                {
                    Id = addNewCycle.CycleID,
                    InstanceId = addNewCycle.InstanceID,
                    CycleName = addNewCycle.CycleName,
                    ShortName = addNewCycle.CycleShortName,
                    Year = addNewCycle.Year,
                    CreatedDatetime = addNewCycle.ModifiedAt,
                    Status = addNewCycle.Active == true ? "Active" : "Inactive",
                    IsLocked = addNewCycle.IsLocked,
                    StartDate = addNewCycle.StartDate,
                    EndDate = addNewCycle.EndDate
                };
            }
            catch (Exception ex)
            {
                return new Cycle() { };
            }
        }

        public bool DeleteCycle(long cycleId)
        {
            try
            {
                var cycleDelete = (from tcycle in _context.tCycle where tcycle.CycleID == cycleId select tcycle).FirstOrDefault();
                if (cycleDelete != null)
                {
                    var cycleWeek = (from cw in _context.tCycleWeek where cw.CycleId == cycleId select cw).ToList();

                    foreach (var week in cycleWeek)
                    {
                        _context.tCycleWeek.Remove(week);
                    }

                    var menusForCycle = (from m in _context.tMenu where m.CycleID == cycleId select m).ToList();

                    foreach (var menu in menusForCycle)
                    {
                        var allHistory = _context.tMenuHistory.Where(h => h.MenuID == menu.ID).ToList();

                        foreach (var history in allHistory)
                        {
                            if (history != null)
                                _context.tMenuHistory.Remove(history);
                        }

                        var menutemplate = _context.tMenuTemplates.Where(t => t.MenuID == menu.ID).FirstOrDefault();
                        if (menutemplate != null)
                            _context.tMenuTemplates.Remove(menutemplate);

                        var allItems = _context.tMenuItem.Where(h => h.MenuID == menu.ID).ToList();

                        foreach (var item in allItems)
                        {
                            if (item != null)
                                _context.tMenuItem.Remove(item);
                        }

                        var allRoutes = _context.tMenuForRoute.Where(h => h.MenuID == menu.ID).ToList();

                        foreach (var route in allRoutes)
                        {
                            if (route != null)
                                _context.tMenuForRoute.Remove(route);
                        }

                        var allLiveOrdersDetails = _context.tLiveOrderDetails.Where(h => h.MenuId == menu.ID).ToList();

                        foreach (var liveOrderdetails in allLiveOrdersDetails)
                        {
                            if (liveOrderdetails != null)
                                _context.tLiveOrderDetails.Remove(liveOrderdetails);
                        }

                        var allLiveOrders = _context.tLiveOrders.Where(h => h.CycleId == cycleId).ToList();

                        foreach (var liveOrder in allLiveOrders)
                        {
                            if (liveOrder != null)
                            {
                                var orders = _context.tOrders.Where(h => h.LiveOrderId == liveOrder.LiveOrderId).ToList();

                                foreach (var o in orders)
                                {
                                    var boxTickets = (from b in _context.tBoxTicketData where b.OrderId == o.OrderID select b).ToList();

                                    foreach (var boxTicket in boxTickets)
                                    {
                                        _context.tBoxTicketData.Remove(boxTicket);
                                    }

                                    _context.tOrders.Remove(o);
                                }
                                _context.tLiveOrders.Remove(liveOrder);
                            }
                        }

                        var allapprovalStage = _context.tMenuApprovalStage.Where(h => h.MenuID == menu.ID).ToList();

                        foreach (var approvalStage in allapprovalStage)
                        {
                            if (approvalStage != null)
                                _context.tMenuApprovalStage.Remove(approvalStage);
                        }

                        var pdfTasks = _context.tPDFGenerationTasks.Where(h => h.MenuID == menu.ID).ToList();

                        foreach (var pdfTask in pdfTasks)
                        {
                            if (pdfTask != null)
                                _context.tPDFGenerationTasks.Remove(pdfTask);
                        }


                        if (menu != null)
                            _context.tMenu.Remove(menu);
                    }

                    _context.tCycle.Remove(cycleDelete);
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool UpdateCycle(Cycle cycle)
        {
            try
            {
                var cycleUpdate = (from tcycle in _context.tCycle where tcycle.CycleID == cycle.Id select tcycle).FirstOrDefault();
                if (cycleUpdate != null)
                {
                    cycleUpdate.Year = cycle.Year;
                    cycleUpdate.CycleName = cycle.CycleName;
                    cycleUpdate.CycleShortName = cycle.ShortName;
                    cycleUpdate.StartDate = cycle.StartDate;
                    cycleUpdate.EndDate = cycle.EndDate;
                    cycleUpdate.ModifiedAt = DateTime.Now;
                    cycleUpdate.Active = cycle.Active;
                    _context.SaveChanges();

                    var cycleWeek = (from cw in _context.tCycleWeek where cw.CycleId == cycle.Id select cw).ToList();

                    foreach (var week in cycleWeek)
                    {
                        _context.tCycleWeek.Remove(week);
                    }

                    _context.SaveChanges();

                    //add cycle week
                    var startDate = cycle.StartDate;
                    var endDate = cycle.EndDate;

                    TimeSpan timeSpan = (Convert.ToDateTime(endDate) - Convert.ToDateTime(startDate));

                    var totalWeek = Convert.ToInt32(timeSpan.TotalDays / 7);

                    for (int i = 1; i <= totalWeek; i++)
                    {
                        var cWeek = new tCycleWeek();
                        cWeek.CycleId = cycle.Id;
                        cWeek.StartDate = Convert.ToDateTime(startDate).AddDays((i - 1) * 7);
                        cWeek.EndDate = cWeek.StartDate.AddDays(6);
                        cWeek.WeekNo = i;

                        _context.tCycleWeek.Add(cWeek);
                        _context.SaveChanges();
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool IsCycleActive(long cycleId)
        {
            var cycle = (from i in _context.tCycle
                         where i.CycleID == cycleId
                         select new Cycle
                         {
                             Id = i.CycleID,
                             InstanceId = i.InstanceID,
                             Year = i.Year,
                             CycleName = i.CycleName,
                             ShortName = i.CycleShortName,
                             StartDate = i.StartDate,
                             EndDate = i.EndDate,
                             Active = i.Active,
                             IsLocked = i.IsLocked,
                             Archived = i.Archived
                         }).FirstOrDefault();

            return cycle != null && Convert.ToBoolean(cycle.Active);
        }

        public bool IsCycleArchived(long cycleId)
        {
            var cycle = (from i in _context.tCycle
                         where i.CycleID == cycleId
                         select new Cycle
                         {
                             Id = i.CycleID,
                             InstanceId = i.InstanceID,
                             Year = i.Year,
                             CycleName = i.CycleName,
                             ShortName = i.CycleShortName,
                             StartDate = i.StartDate,
                             EndDate = i.EndDate,
                             Active = i.Active,
                             IsLocked = i.IsLocked,
                             Archived = i.Archived
                         }).FirstOrDefault();

            return cycle != null && Convert.ToBoolean(cycle.Archived);
        }

        public bool IsCycleLocked(long cycleId)
        {
            var cycle = (from i in _context.tCycle
                         where i.CycleID == cycleId
                         select new Cycle
                         {
                             Id = i.CycleID,
                             InstanceId = i.InstanceID,
                             Year = i.Year,
                             CycleName = i.CycleName,
                             ShortName = i.CycleShortName,
                             StartDate = i.StartDate,
                             EndDate = i.EndDate,
                             Active = i.Active,
                             IsLocked = i.IsLocked,
                             Archived = i.Archived
                         }).FirstOrDefault();

            return cycle != null && Convert.ToBoolean(cycle.IsLocked);
        }
        public List<tCycleWeek> GetWeeksAndDates(long cycleId)
        {
            try
            {
                var cycleweekData = (from data in _context.tCycleWeek
                                     where data.CycleId == cycleId
                                     select new tCycleWeek
                                    {
                                        ID = data.ID,
                                        CycleId = data.CycleId,
                                        WeekNo = data.WeekNo,
                                        StartDate = data.StartDate,
                                        EndDate = data.EndDate
                                    }).ToList();
                return (from data in cycleweekData
                        select new tCycleWeek
                        {
                            ID = data.ID,
                            CycleId = data.CycleId,
                            WeekNo = data.WeekNo,
                            StartDate = data.StartDate,
                            EndDate = data.EndDate
                        }).ToList();
            }
            catch (Exception ex)
            {
                return new List<tCycleWeek>();
            }
        }
        public Cycle GetActiveCycle()
        {
            try
            {
                var cycleData = (from cycle in _context.tCycle where cycle.Active == true select cycle).FirstOrDefault();
                if (cycleData != null)
                {
                    return new Cycle()
                    {
                        Id = cycleData.CycleID,
                        CycleName = cycleData.CycleName,
                        ShortName = cycleData.CycleShortName,
                        Year = cycleData.Year,
                        StartDate = cycleData.StartDate,
                        EndDate = cycleData.EndDate
                    };
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public bool IsCycleHasLiveOrders(long cycleId)
        {
            try
            {
                var cycleData = (from i in _context.tLiveOrders where i.IsConvertedToOrder == false && i.CycleId == cycleId select i).FirstOrDefault();
                if (cycleData != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool UpdateCyclesActiveState(long cycleId)
        {
            try
            {
                var cycleUpdate = (from tcycle in _context.tCycle where tcycle.CycleID == cycleId select tcycle).FirstOrDefault();
                if (cycleUpdate != null)
                {
                    cycleUpdate.Active = false;
                    cycleUpdate.ModifiedAt = DateTime.Now;
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
