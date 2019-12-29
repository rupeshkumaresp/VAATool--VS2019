using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using VAA.CommonComponents;
using VAA.DataAccess.Interfaces;
using VAA.DataAccess.Model;
using VAA.Entities.VAAEntity;

namespace VAA.DataAccess
{
    /// <summary>
    /// Order management class - Handles Order, Live Order get, add, edit, delete of menu various Order related entities
    /// </summary>
    public class OrderManagement : IOrder
    {
        private readonly VAAEntities _context = new VAAEntities();
        readonly AccountManagement _accountManagement = new AccountManagement();
        readonly MenuManagement _menuManagement = new MenuManagement();

        public OrderManagement()
        {
        }


        public tLiveOrders GetLiveOrder()
        {
            var liveorder =
                (from lo in _context.tLiveOrders where lo.IsConvertedToOrder == false select lo).FirstOrDefault();

            return liveorder;
        }

        public tOrders GetOrderById(long orderId)
        {
            return _context.tOrders.Where(o => o.OrderID == orderId).FirstOrDefault();
        }

        public string GetOrderFriendlyNameFromLiveOrderId(long liveOrderId)
        {
            var order = _context.tOrders.Where(o => o.LiveOrderId == liveOrderId).FirstOrDefault();
            if (order != null)
                return order.OrderFriendlyName;
            else return "";

        }


        public long GetLiveOrderCycleId()
        {
            var liveorder =
                (from lo in _context.tLiveOrders where lo.IsConvertedToOrder == false select lo).FirstOrDefault();

            if (liveorder != null)
                return Convert.ToInt64(liveorder.CycleId);
            else
                return 0;
        }

        public long GetCycleIdOfLiveOrder(long liveOrderId)
        {
            var liveorder =
                (from lo in _context.tLiveOrders where lo.LiveOrderId == liveOrderId select lo).FirstOrDefault();

            if (liveorder != null)
                return Convert.ToInt64(liveorder.CycleId);
            else
                return 0;
        }

        public long GetLiveOrderIdFromOrderId(long orderId)
        {
            var liveOrderId = (from o in _context.tOrders where o.OrderID == orderId select o.LiveOrderId).FirstOrDefault();

            if (liveOrderId != null)
                return Convert.ToInt64(liveOrderId);

            return 0;

        }

        public string GetLotNoFromOrderId(long orderId)
        {
            var liveOrderId = (from o in _context.tOrders where o.OrderID == orderId select o.LiveOrderId).FirstOrDefault();

            if (liveOrderId != null)
            {
                var liveOrder = _context.tLiveOrders.Where(l => l.LiveOrderId == liveOrderId).FirstOrDefault();

                if (liveOrder != null)
                    return liveOrder.LotNo;
            }

            return " ";

        }



        public List<MenuData> GetMenuStatusAndApprovers(long cycleid, int classid, int menutypeid, int userid)
        {
            var usertype = _accountManagement.GetUserTypeByUserid(userid);
            if (usertype == "ESP")
            {
                List<MenuData> menuItemCollection = new List<MenuData>();

                var userdata = (from m in _context.tMenu
                                join mfr in _context.tMenuForRoute on m.ID equals mfr.MenuID
                                join rd in _context.tRouteDetails on mfr.RouteID equals rd.RouteID
                                join app in _context.tApprovers on rd.DepartureID equals app.OriginLocationID
                                where m.CycleID == cycleid && app.ClassID == classid && m.MenuTypeID == menutypeid
                                select m).Distinct().ToList();
                foreach (var menu in userdata)
                {
                    var menuItemdata = (from menuitem in _context.sp_GetMenuAndApprovers(menu.ID, classid) select menuitem).FirstOrDefault();
                    MenuData menudata = new MenuData()
                    {
                        Id = menuItemdata.MenuId,
                        MenuName = menuItemdata.MenuName,
                        MenuCode = menuItemdata.MenuCode,
                        IsMovedToLiveOrder = menuItemdata.IsMovedToLiveOrder,
                        LanguageId = menuItemdata.LanguangeID,
                        LanguageName = menuItemdata.Language,
                        ApprovalStatusId = menuItemdata.ApprovalStatusID,
                        ApprovalStatusName = menuItemdata.ApprovalStatus,
                        VirginApproverId = menuItemdata.VirginAppUserId,
                        VirginAppFirstName = menuItemdata.VirginAppFirstName,
                        VirginAppLastName = menuItemdata.VirginAppLastName,
                        CatererApproverId = menuItemdata.CatererAppUserId,
                        CatererAppFirstName = menuItemdata.CatererAppFirstName,
                        CatererAppLastName = menuItemdata.CatererAppLastName,
                        TranslatorApproverId = menuItemdata.TranslatorAppUserId,
                        TranslatorAppFirstName = menuItemdata.TranslatorAppFirstName,
                        TranslatorAppLastName = menuItemdata.TranslatorAppLastName
                    };
                    menuItemCollection.Add(menudata);
                }
                return menuItemCollection;
            }
            else
            {
                List<MenuData> menuItemCollection = new List<MenuData>();

                var userdata = (from m in _context.tMenu
                                join mfr in _context.tMenuForRoute on m.ID equals mfr.MenuID
                                join rd in _context.tRouteDetails on mfr.RouteID equals rd.RouteID
                                join app in _context.tApprovers on rd.DepartureID equals app.OriginLocationID
                                where app.VirginApproverID == userid && m.CycleID == cycleid && app.ClassID == classid && m.MenuTypeID == menutypeid
                                select m).Distinct().ToList();
                foreach (var menu in userdata)
                {
                    var menuItemdata = (from menuitem in _context.sp_GetMenuAndApprovers(menu.ID, classid) select menuitem).FirstOrDefault();
                    MenuData menudata = new MenuData()
                    {
                        Id = menuItemdata.MenuId,
                        MenuName = menuItemdata.MenuName,
                        MenuCode = menuItemdata.MenuCode,
                        IsMovedToLiveOrder = menuItemdata.IsMovedToLiveOrder,
                        LanguageId = menuItemdata.LanguangeID,
                        LanguageName = menuItemdata.Language,
                        ApprovalStatusId = menuItemdata.ApprovalStatusID,
                        ApprovalStatusName = menuItemdata.ApprovalStatus,
                        VirginApproverId = menuItemdata.VirginAppUserId,
                        VirginAppFirstName = menuItemdata.VirginAppFirstName,
                        VirginAppLastName = menuItemdata.VirginAppLastName,
                        CatererApproverId = menuItemdata.CatererAppUserId,
                        CatererAppFirstName = menuItemdata.CatererAppFirstName,
                        CatererAppLastName = menuItemdata.CatererAppLastName,
                        TranslatorApproverId = menuItemdata.TranslatorAppUserId,
                        TranslatorAppFirstName = menuItemdata.TranslatorAppFirstName,
                        TranslatorAppLastName = menuItemdata.TranslatorAppLastName
                    };
                    menuItemCollection.Add(menudata);
                }
                return menuItemCollection;
            }
        }

        public List<MenuData> GetMenuByCycleClassAndMenutype(long cycleId, int classId, int menuTypeId)
        {
            try
            {
                return (from m in _context.tMenu
                        join cmtm in _context.tClassMenuTypeMap on m.MenuTypeID equals cmtm.MenuTypeID
                        join aps in _context.tApprovalStatuses on m.CurrentApprovalStatusID equals aps.ApprovalStatusID
                        where m.CycleID == cycleId && cmtm.FlightClassID == classId && m.MenuTypeID == menuTypeId
                        select new MenuData
                        {
                            Id = m.ID,
                            MenuName = m.MenuName,
                            MenuCode = m.MenuCode,
                            LanguageId = m.LanguageID,
                            ApprovalStatusId = m.CurrentApprovalStatusID,
                            ApprovalStatusName = aps.Status,
                            IsMovedToLiveOrder = m.IsMovedToLiveOrder
                        }).ToList();
            }
            catch (Exception ex)
            {
                return new List<MenuData>();
            }
        }

        public List<MenuData> GetAllApprovalStatus()
        {
            return (from ap in _context.tApprovalStatuses
                    select new
                    MenuData
                    {
                        ApprovalStatusId = ap.ApprovalStatusID,
                        ApprovalStatusName = ap.Status,
                    }).ToList();
        }

        public List<tUsers> GetApproversByMenuIdAndClass(int menuId, int classid)
        {
            var approverData = (from u in _context.tUsers
                                join app in _context.tApprovers on u.UserID equals app.VirginApproverID
                                join rd in _context.tRouteDetails on app.OriginLocationID equals rd.DepartureID
                                join mfr in _context.tMenuForRoute on rd.RouteID equals mfr.RouteID
                                join m in _context.tMenu on mfr.MenuID equals m.ID
                                where m.ID == menuId && app.ClassID == classid
                                select new
                                {
                                    id = u.UserID,
                                    firstName = u.FirstName,
                                    lastName = u.LastName
                                }).ToList();
            return (from x in approverData
                    select new tUsers
                    {
                        UserID = x.id,
                        FirstName = x.firstName,
                        LastName = x.lastName
                    }).ToList();
        }

        public User GetApproverByMenuIdAndClass(int menuId, int classid)
        {
            return (from u in _context.tUsers
                    join app in _context.tApprovers on u.UserID equals app.VirginApproverID
                    join rd in _context.tRouteDetails on app.OriginLocationID equals rd.DepartureID
                    join mfr in _context.tMenuForRoute on rd.RouteID equals mfr.RouteID
                    join m in _context.tMenu on mfr.MenuID equals m.ID
                    where m.ID == menuId && app.ClassID == classid
                    select new User
                    {
                        Id = u.UserID,
                        FirstName = u.FirstName,
                        LastName = u.LastName
                    }).FirstOrDefault();
        }

        public List<ApprovedMenu> GetAllApprovedMenu()
        {

            var LiveOrder = (from LO in _context.tLiveOrders where LO.IsConvertedToOrder == false select LO).FirstOrDefault();
            if (LiveOrder != null)
            {
                var dataToBind = (from LO in _context.tLiveOrders
                                  join LOD in _context.tLiveOrderDetails on LO.LiveOrderId equals LOD.LiveOrderId
                                  join M in _context.tMenu on LOD.MenuId equals M.ID
                                  join RD in _context.tRouteDetails on LOD.RouteId equals RD.RouteID
                                  where LO.LiveOrderId == LiveOrder.LiveOrderId
                                  select new
                                  {
                                      LiveOrderId = LOD.ID,
                                      MenuCode = M.MenuCode,
                                      FromRoute = (from L in _context.tLocation where L.LocationID == RD.DepartureID select L.AirportCode).FirstOrDefault(),
                                      ToRoute = (from L in _context.tLocation where L.LocationID == RD.ArrivalID select L.AirportCode).FirstOrDefault(),
                                      ClassName = (from CMTM in _context.tClassMenuTypeMap
                                                   join C in _context.tClass on CMTM.FlightClassID equals C.ID
                                                   where CMTM.MenuTypeID == M.MenuTypeID
                                                   select C.FlightClass).FirstOrDefault(),
                                      MenuTypeName = (from MT in _context.tMenuType where MT.ID == M.MenuTypeID select MT.DisplayName).FirstOrDefault(),
                                      FlightType = LOD.FlightNo,
                                      Quantity = LOD.Quantity
                                  }).ToList();
                return (from x in dataToBind
                        select new ApprovedMenu
                        {
                            LiveOrderId = x.LiveOrderId,
                            MenuCode = x.MenuCode,
                            Route = x.FromRoute + " - " + x.ToRoute,
                            ClassName = x.ClassName,
                            MenuType = x.MenuTypeName,
                            FlightNo = x.FlightType,
                            Quantity = Convert.ToString(x.Quantity)
                        }).ToList();
            }
            else
            {
                return new List<ApprovedMenu>();
            }
        }

        public List<tLiveOrderDetails> GetLiveOrderDetails(Int64 liveOrderId)
        {
            return (from LOD in _context.tLiveOrderDetails where LOD.LiveOrderId == liveOrderId select LOD).ToList();
        }

        public List<tLiveOrderDetails> GetRecentOrderDetails()
        {
            var latestLiveOrder = _context.tLiveOrders.OrderByDescending(u => u.LiveOrderId).FirstOrDefault();

            var latestLiveOrderId = latestLiveOrder.LiveOrderId;

            return GetLiveOrderDetails(latestLiveOrderId);
        }


        public bool IsMenuPresentInRecentOrderDetails(long menuId, List<tLiveOrderDetails> recentOrderDetails)
        {
            var menu = _menuManagement.GetMenuById(menuId);

            if (menu.MenuTypeId == 4 || menu.MenuTypeId == 5 || menu.MenuTypeId == 6 || menu.MenuTypeId == 7)
                return true;

            bool menuFound = false;

            foreach (var details in recentOrderDetails)
            {
                if (details != null && details.MenuId == menuId)
                {
                    menuFound = true;
                    break;
                }
            }

            return menuFound;
        }

        public void UpdateQuantity(Int64 LiveOrderDetailsId, int Quantity)
        {
            var liveOrderData = (from LOD in _context.tLiveOrderDetails where LOD.ID == LiveOrderDetailsId select LOD).FirstOrDefault();
            if (liveOrderData != null)
            {
                liveOrderData.Quantity = Quantity;
                _context.SaveChanges();
            }
        }

        public List<CurrentPreviousOrder> GetAllOrders()
        {
            return (from O in _context.tOrders
                    join OS in _context.tOrderStatus on O.OrderStatusId equals OS.StatusID
                    join LO in _context.tLiveOrders on O.LiveOrderId equals LO.LiveOrderId
                    join C in _context.tCycle on LO.CycleId equals C.CycleID
                    select new CurrentPreviousOrder
                    {
                        OrderRowId = O.OrderID,
                        OrderId = O.OrderID,
                        OrderDate = O.OrderDate,
                        LOTNumber = LO.LotNo,
                        Cycle = C.CycleName,
                        OrderStatusId = O.OrderStatusId,
                        OrderStatus = OS.Status,
                        FriendlyName = O.OrderFriendlyName,
                    }).ToList();
        }

        public List<CurrentPreviousOrder> GetAllCurrentOrders()
        {
            return (from O in _context.tOrders
                    join OS in _context.tOrderStatus on O.OrderStatusId equals OS.StatusID
                    join LO in _context.tLiveOrders on O.LiveOrderId equals LO.LiveOrderId
                    join C in _context.tCycle on LO.CycleId equals C.CycleID
                    where O.IsDelivered == false
                    select new CurrentPreviousOrder
                    {
                        OrderRowId = O.OrderID,
                        OrderId = O.OrderID,
                        OrderDate = O.OrderDate,
                        LOTNumber = LO.LotNo,
                        Cycle = C.CycleName,
                        OrderStatusId = O.OrderStatusId,
                        OrderStatus = OS.Status,
                        FriendlyName = O.OrderFriendlyName,
                    }).ToList();
        }

        public List<ApprovedMenu> GetOrderDetailsbyOrderId(long orderId)
        {
            var dataToBind = (from O in _context.tOrders
                              join LO in _context.tLiveOrders on O.LiveOrderId equals LO.LiveOrderId
                              join LOD in _context.tLiveOrderDetails on LO.LiveOrderId equals LOD.LiveOrderId
                              join M in _context.tMenu on LOD.MenuId equals M.ID
                              join RD in _context.tRouteDetails on LOD.RouteId equals RD.RouteID
                              where O.OrderID == orderId
                              select new
                              {
                                  LiveOrderId = O.LiveOrderId,
                                  MenuCode = M.MenuCode,
                                  MenuId = M.ID,
                                  FromRoute = (from L in _context.tLocation where L.LocationID == RD.DepartureID select L.AirportCode).FirstOrDefault(),
                                  ToRoute = (from L in _context.tLocation where L.LocationID == RD.ArrivalID select L.AirportCode).FirstOrDefault(),
                                  ClassName = (from CMTM in _context.tClassMenuTypeMap
                                               join C in _context.tClass on CMTM.FlightClassID equals C.ID
                                               where CMTM.MenuTypeID == M.MenuTypeID
                                               select C.FlightClass).FirstOrDefault(),
                                  MenuTypeName = (from MT in _context.tMenuType where MT.ID == M.MenuTypeID select MT.DisplayName).FirstOrDefault(),
                                  FlightType = LOD.FlightNo,
                                  Quantity = LOD.Quantity,
                                  LanguageName = (from L in _context.tMenuLanguage where L.ID == M.LanguageID select L.LanguageCode).FirstOrDefault()
                              }).ToList();
            return (from x in dataToBind
                    select new ApprovedMenu
                    {
                        LiveOrderId = x.LiveOrderId,
                        MenuId = x.MenuId,
                        MenuCode = x.MenuCode,
                        Route = x.FromRoute + " - " + x.ToRoute,
                        ClassName = x.ClassName,
                        MenuType = x.MenuTypeName,
                        FlightNo = x.FlightType,
                        Quantity = Convert.ToString(x.Quantity),
                        LanguageName = x.LanguageName
                    }).ToList();

        }

        public List<ApprovedMenu> GetMenuDetailsbyOrderId(int orderId)
        {
            var dataToBind = (from O in _context.tOrders
                              join LO in _context.tLiveOrders on O.LiveOrderId equals LO.LiveOrderId
                              join LOD in _context.tLiveOrderDetails on LO.LiveOrderId equals LOD.LiveOrderId
                              join M in _context.tMenu on LOD.MenuId equals M.ID
                              join RD in _context.tRouteDetails on LOD.RouteId equals RD.RouteID
                              where O.OrderID == orderId
                              select new
                              {
                                  LiveOrderId = O.LiveOrderId,
                                  MenuId = M.ID,
                                  MenuCode = M.MenuCode,
                                  ClassName = (from CMTM in _context.tClassMenuTypeMap
                                               join C in _context.tClass on CMTM.FlightClassID equals C.ID
                                               where CMTM.MenuTypeID == M.MenuTypeID
                                               select C.FlightClass).FirstOrDefault(),
                                  MenuTypeName = (from MT in _context.tMenuType where MT.ID == M.MenuTypeID select MT.DisplayName).FirstOrDefault()
                              }).Distinct().ToList();
            return (from x in dataToBind
                    select new ApprovedMenu
                    {
                        LiveOrderId = x.LiveOrderId,
                        MenuId = x.MenuId,
                        MenuCode = x.MenuCode,
                        ClassName = x.ClassName,
                        MenuType = x.MenuTypeName
                    }).ToList();

        }

        public List<CurrentPreviousOrder> GetAllPreviousOrders()
        {
            return (from O in _context.tOrders
                    join OS in _context.tOrderStatus on O.OrderStatusId equals OS.StatusID
                    join LO in _context.tLiveOrders on O.LiveOrderId equals LO.LiveOrderId
                    join C in _context.tCycle on LO.CycleId equals C.CycleID
                    where O.IsDelivered == true
                    select new CurrentPreviousOrder
                    {
                        OrderRowId = O.OrderID,
                        OrderId = O.OrderID,
                        OrderDate = O.OrderDate,
                        LOTNumber = LO.LotNo,
                        Cycle = C.CycleName,
                        OrderStatusId = O.OrderStatusId,
                        OrderStatus = OS.Status,
                        CompletedDate = O.CompletedDate,
                        FriendlyName = O.OrderFriendlyName,
                    }).ToList();
        }

        public List<OrderStatusList> GetAllOrderStatus()
        {
            return (from OS in _context.tOrderStatus
                    select new OrderStatusList
                    {
                        StatusId = OS.StatusID,
                        Status = OS.Status
                    }).ToList();
        }

        public void UpdateOrderStatus(Int64 OrderRowId, int StatusId)
        {
            var orderData = (from O in _context.tOrders where O.OrderID == OrderRowId select O).FirstOrDefault();
            if (orderData != null)
            {
                orderData.OrderStatusId = StatusId;

                if (StatusId == 4)
                {
                    orderData.IsDelivered = true;
                    orderData.CompletedDate = DateTime.Now;
                }
                _context.SaveChanges();
            }
        }

        public void CreateOrderNow(DateTime startDate, DateTime endDate)
        {
            var liveorder = (from LO in _context.tLiveOrders where LO.IsConvertedToOrder == false select LO).FirstOrDefault();
            if (liveorder != null)
            {
                liveorder.IsConvertedToOrder = true;
                _context.SaveChanges();
                tOrders addNewOrder = new tOrders()
                {
                    LiveOrderId = liveorder.LiveOrderId,
                    OrderStatusId = 1,
                    OrderDate = DateTime.Now,
                    OrderCycleStartDate = startDate,
                    OrderCycleEndDate = endDate,
                    IsDelivered = false,
                };
                _context.tOrders.Add(addNewOrder);
                _context.SaveChanges();
            }
        }

        public void CreateOrderNow(DateTime startDate, DateTime endDate, string orderFriendlyName)
        {
            var liveorder = (from LO in _context.tLiveOrders where LO.IsConvertedToOrder == false select LO).FirstOrDefault();
            if (liveorder != null)
            {
                liveorder.IsConvertedToOrder = true;
                _context.SaveChanges();
                tOrders addNewOrder = new tOrders()
                {
                    LiveOrderId = liveorder.LiveOrderId,
                    OrderStatusId = 1,
                    OrderDate = DateTime.Now,
                    OrderCycleStartDate = startDate,
                    OrderCycleEndDate = endDate,
                    IsDelivered = false,
                    OrderFriendlyName = orderFriendlyName,
                };
                _context.tOrders.Add(addNewOrder);
                _context.SaveChanges();
            }
        }

        public void CreateLiveOrderNow(long menuId)
        {

            var menumovetolive = (from m in _context.tMenu where m.ID == menuId select m).FirstOrDefault();
            if (menumovetolive != null)
            {
                menumovetolive.IsMovedToLiveOrder = true;
                _context.SaveChanges();
            }

            var menu = _menuManagement.GetMenuById(menuId);

            var liveorder = (from LO in _context.tLiveOrders where LO.IsConvertedToOrder == false select LO).FirstOrDefault();

            if (liveorder == null)
            {
                liveorder = new tLiveOrders();
                liveorder.CycleId = menu.CycleId;
                liveorder.IsConvertedToOrder = false;
                //TOTO: create a logic to create LOT
                liveorder.LotNo = Helper.Get3Digits() + "L" + Helper.Get3Digits();
                _context.tLiveOrders.Add(liveorder);
                _context.SaveChanges();
            }



            //get all the route for this menu reading from tMenuForRoute table
            var routes = (from r in _context.tMenuForRoute where r.MenuID == menuId select r).ToList();

            //loop in this above collection and add all the entries as liveorderdetails
            foreach (var route in routes)
            {
                var flights = route.Flights.Split(new char[] { ',' });
                for (int i = 0; i < flights.Length; i++)
                {
                    var flightNo = flights[i].Trim();
                    if (!string.IsNullOrEmpty(flightNo))
                    {
                        var loDetails = (from lo in _context.tLiveOrderDetails
                                         where lo.MenuId == menuId && lo.LiveOrderId == liveorder.LiveOrderId
                                             && lo.RouteId == route.RouteID && lo.FlightNo == flightNo
                                         select lo).FirstOrDefault();

                        if (loDetails == null)
                        {
                            var liveOrderDetails = new tLiveOrderDetails();
                            liveOrderDetails.LiveOrderId = liveorder.LiveOrderId;
                            liveOrderDetails.MenuId = menuId;
                            liveOrderDetails.RouteId = route.RouteID;
                            liveOrderDetails.FlightNo = flightNo;
                            _context.tLiveOrderDetails.Add(liveOrderDetails);
                        }
                    }
                }
            }
            _context.SaveChanges();


        }

        public void RemoveMenuFromLiveOrder(long menuId)
        {
            var menu = _menuManagement.GetMenuById(menuId);

            var liveorder = (from LO in _context.tLiveOrders where LO.IsConvertedToOrder == false select LO).FirstOrDefault();

            if (liveorder != null)
            {
                //get all the route for this menu reading from tMenuForRoute table
                var routes = (from r in _context.tMenuForRoute where r.MenuID == menuId select r).ToList();

                //loop in this above collection and add all the entries as liveorderdetails
                foreach (var route in routes)
                {
                    var flights = route.Flights.Split(new char[] { ',' });
                    for (int i = 0; i < flights.Length; i++)
                    {
                        var flightNo = flights[i].Trim();

                        var liveOrderDetails = (from lod in _context.tLiveOrderDetails
                                                where lod.LiveOrderId == liveorder.LiveOrderId
                                                && lod.MenuId == menuId
                                                && lod.RouteId == route.RouteID
                                                && lod.FlightNo == flightNo
                                                select lod).FirstOrDefault();

                        if (liveOrderDetails != null)
                            _context.tLiveOrderDetails.Remove(liveOrderDetails);
                    }
                }
            }

            _context.SaveChanges();

            var menumovetolive = (from m in _context.tMenu where m.ID == menuId select m).FirstOrDefault();
            if (menumovetolive != null)
            {
                menumovetolive.IsMovedToLiveOrder = false;
                _context.SaveChanges();
            }


            var liveOrderDetailExists = (from lod in _context.tLiveOrderDetails
                                         where lod.LiveOrderId == liveorder.LiveOrderId
                                         select lod).FirstOrDefault();

            if (liveOrderDetailExists == null && liveorder != null)
            {
                _context.tLiveOrders.Remove(liveorder);

                _context.SaveChanges();
            }

        }

        public List<ApprovedMenu> GetAllApprovedMenuOnly()
        {
            var LiveOrder = (from LO in _context.tLiveOrders where LO.IsConvertedToOrder == false select LO).FirstOrDefault();
            if (LiveOrder != null)
            {
                var dataToBind = (from LOD in _context.tLiveOrderDetails
                                  join M in _context.tMenu on LOD.MenuId equals M.ID
                                  where LOD.LiveOrderId == LiveOrder.LiveOrderId
                                  select new
                                  {
                                      MenuId = M.ID,
                                      MenuCode = M.MenuCode,
                                      ClassName = (from CMTM in _context.tClassMenuTypeMap
                                                   join C in _context.tClass on CMTM.FlightClassID equals C.ID
                                                   where CMTM.MenuTypeID == M.MenuTypeID
                                                   select C.FlightClass).FirstOrDefault(),
                                      LanguageName = (from tml in _context.tMenuLanguage
                                                      join tm in _context.tMenu on tml.ID equals tm.LanguageID
                                                      where tm.ID == LOD.MenuId
                                                      select tml.LanguageCode).FirstOrDefault(),
                                      MenuTypeName = (from MT in _context.tMenuType where MT.ID == M.MenuTypeID select MT.DisplayName).FirstOrDefault(),
                                      Quantity = (from lo in _context.tLiveOrderDetails where lo.MenuId == M.ID && lo.LiveOrderId == LiveOrder.LiveOrderId select lo.Quantity).Sum()
                                  }).Distinct().ToList();
                return (from x in dataToBind
                        select new ApprovedMenu
                        {
                            MenuId = x.MenuId,
                            MenuCode = x.MenuCode,
                            ClassName = x.ClassName,
                            MenuType = x.MenuTypeName,
                            Quantity = Convert.ToString(x.Quantity),
                            LanguageName = x.LanguageName
                        }).ToList();
            }
            else
            {
                return new List<ApprovedMenu>();
            }
        }

        public List<ApprovedMenu> GetAllApprovedMenuDetails(Int64 MenuId)
        {
            var LiveOrder = (from LO in _context.tLiveOrders where LO.IsConvertedToOrder == false select LO).FirstOrDefault();
            if (LiveOrder != null)
            {
                var dataToBind = (from LO in _context.tLiveOrders
                                  join LOD in _context.tLiveOrderDetails on LO.LiveOrderId equals LOD.LiveOrderId
                                  join RD in _context.tRouteDetails on LOD.RouteId equals RD.RouteID
                                  where LO.LiveOrderId == LiveOrder.LiveOrderId
                                  && LOD.MenuId == MenuId
                                  select new
                                  {
                                      LiveOrderId = LOD.ID,
                                      MenuId = LOD.MenuId,
                                      FromRoute = (from L in _context.tLocation where L.LocationID == RD.DepartureID select L.AirportCode).FirstOrDefault(),
                                      ToRoute = (from L in _context.tLocation where L.LocationID == RD.ArrivalID select L.AirportCode).FirstOrDefault(),
                                      FlightType = LOD.FlightNo,
                                      Quantity = LOD.Quantity
                                  }).ToList();
                return (from x in dataToBind
                        select new ApprovedMenu
                        {
                            LiveOrderId = x.LiveOrderId,
                            MenuId = Convert.ToInt64(x.MenuId),
                            Route = x.FromRoute + " - " + x.ToRoute,
                            FlightNo = x.FlightType,
                            Quantity = Convert.ToString(x.Quantity)
                        }).ToList();
            }
            else
            {
                return new List<ApprovedMenu>();
            }
        }

        public tLiveOrders CreateReorderFromLiveOrder(Int64 orderId)
        {
            var liveOrderId = (from o in _context.tOrders where o.OrderID == orderId select o.LiveOrderId).FirstOrDefault();

            if (liveOrderId != null)
            {
                //getdata
                var getLiveOrderData = (from lo in _context.tLiveOrders where lo.LiveOrderId == liveOrderId select lo).FirstOrDefault();

                long newLiveorderId = 0;
                if (getLiveOrderData != null)
                {
                    //create new liveorder
                    var liveorder = new tLiveOrders();
                    liveorder.CycleId = getLiveOrderData.CycleId;
                    liveorder.IsConvertedToOrder = false;
                    liveorder.LotNo = getLiveOrderData.LotNo;
                    _context.tLiveOrders.Add(liveorder);
                    _context.SaveChanges();

                    newLiveorderId = liveorder.LiveOrderId;
                }


                var oldliveorderdata = (from lod in _context.tLiveOrderDetails where lod.LiveOrderId == liveOrderId select lod).ToList();
                //create new live orderDetails
                foreach (var data in oldliveorderdata)
                {
                    var liveorderdetails = new tLiveOrderDetails();
                    liveorderdetails.LiveOrderId = newLiveorderId;
                    liveorderdetails.MenuId = data.MenuId;
                    liveorderdetails.RouteId = data.RouteId;
                    liveorderdetails.FlightNo = data.FlightNo;
                    liveorderdetails.Quantity = null;
                    _context.tLiveOrderDetails.Add(liveorderdetails);
                    _context.SaveChanges();

                    var menumovetolive = (from m in _context.tMenu where m.ID == data.MenuId select m).FirstOrDefault();
                    if (menumovetolive != null)
                    {
                        menumovetolive.IsMovedToLiveOrder = true;
                    }
                    _context.SaveChanges();
                }


                var newliveorderdata = (from newlo in _context.tLiveOrders
                                        where newlo.IsConvertedToOrder == false && newlo.LiveOrderId == newLiveorderId
                                        select newlo).FirstOrDefault();
                return newliveorderdata;
            }
            else
            {
                return null;
            }

        }
        public void UpdateReOrderCount(List<string> menuIds)
        {
            if (menuIds != null)
            {
                foreach (var menus in menuIds)
                {
                    long menuid = Convert.ToInt64(menus);
                    var menu = (from m in _context.tMenu where m.ID == menuid select m).FirstOrDefault();
                    // var updatemenu = new tMenu();
                    if (menu.ReorderCount != null)
                    {
                        menu.ReorderCount = menu.ReorderCount + 1;
                    }
                    else
                    {
                        menu.ReorderCount = 1;
                    }
                    _context.SaveChanges();
                }
            }
        }

        public void CreateReOrderNow(Int64 liveorderid, DateTime fromDate, DateTime toDate, string OrderFriendlyName)
        {
            var liveorder = (from LO in _context.tLiveOrders where LO.LiveOrderId == liveorderid && LO.IsConvertedToOrder == false select LO).FirstOrDefault();
            if (liveorder != null)
            {
                liveorder.IsConvertedToOrder = true;
                _context.SaveChanges();
                tOrders addNewOrder = new tOrders()
                {
                    LiveOrderId = liveorder.LiveOrderId,
                    OrderStatusId = 1,
                    OrderDate = DateTime.Now,
                    OrderCycleStartDate = fromDate,
                    OrderCycleEndDate = toDate,
                    IsDelivered = false,
                    OrderFriendlyName = OrderFriendlyName,
                };
                _context.tOrders.Add(addNewOrder);
                _context.SaveChanges();
            }
        }
        public tLiveOrders CreateLiveOrderForReOrder(long menuId)
        {
            var menu = _menuManagement.GetMenuById(menuId);

            var liveorder = (from LO in _context.tLiveOrders where LO.IsConvertedToOrder == false select LO).FirstOrDefault();

            long newLiveorderId = 0;
            if (liveorder == null)
            {
                liveorder = new tLiveOrders();
                liveorder.CycleId = menu.CycleId;
                liveorder.IsConvertedToOrder = false;
                //TOTO: create a logic to create LOT
                liveorder.LotNo = Helper.Get3Digits() + "L" + Helper.Get3Digits();
                _context.tLiveOrders.Add(liveorder);
                _context.SaveChanges();

                newLiveorderId = liveorder.LiveOrderId;
            }
            else
            {
                newLiveorderId = liveorder.LiveOrderId;
            }

            //get all the route for this menu reading from tMenuForRoute table
            var routes = (from r in _context.tMenuForRoute where r.MenuID == menuId select r).ToList();

            //loop in this above collection and add all the entries as liveorderdetails
            foreach (var route in routes)
            {
                var flights = route.Flights.Split(new char[] { ',' });
                for (int i = 0; i < flights.Length; i++)
                {
                    var flightNo = flights[i].Trim();
                    if (!string.IsNullOrEmpty(flightNo))
                    {
                        var liveOrderDetails = new tLiveOrderDetails();
                        liveOrderDetails.LiveOrderId = liveorder.LiveOrderId;
                        liveOrderDetails.MenuId = menuId;
                        liveOrderDetails.RouteId = route.RouteID;
                        liveOrderDetails.FlightNo = flights[i];
                        //liveOrderDetails.Quantity = 0;
                        _context.tLiveOrderDetails.Add(liveOrderDetails);
                    }
                }
            }
            _context.SaveChanges();

            var menumovetolive = (from m in _context.tMenu where m.ID == menuId select m).FirstOrDefault();
            if (menumovetolive != null)
            {
                menumovetolive.IsMovedToLiveOrder = true;
                _context.SaveChanges();

            }
            var newliveorderdata = (from newlo in _context.tLiveOrders
                                    where newlo.IsConvertedToOrder == false && newlo.LiveOrderId == newLiveorderId
                                    select newlo).FirstOrDefault();
            return newliveorderdata;
        }

        public long CreateReorderMenuFromMenuid(long menuId, int userid)
        {
            var originalMenu = _menuManagement.GetMenuById(menuId);
            var currCycleId = (from c in _context.tCycle where c.Active == true select c.CycleID).FirstOrDefault();

            if (originalMenu != null)
            {
                //getdata
                var getOldMenuData = (from om in _context.tMenu where om.ID == menuId select om).FirstOrDefault();

                long newMenuId = 0;
                if (getOldMenuData != null)
                {
                    //create new menu
                    var ifDuplicate = '_';
                    var newmenu = new tMenu();
                    newmenu.MenuName = getOldMenuData.MenuName;
                    if (getOldMenuData.MenuCode.Contains(ifDuplicate))
                    {
                        string oldMenucode = getOldMenuData.MenuCode;
                        string str1 = oldMenucode.Substring(0, 8);
                        int str2 = Convert.ToInt16(oldMenucode.Substring(8));
                        //check if there is a higher version of the menu already exist  
                        for (var i = 1; i < 50; i++)
                        {
                            string str3 = Convert.ToString(str2 + i);
                            string checkmenucode = str1 + str3;
                            var checkmenu = _menuManagement.GetMenuIdByMenuCode(checkmenucode);
                            if (checkmenu == 0)
                            {
                                newmenu.MenuCode = checkmenucode;
                                break;
                            }
                        }

                    }
                    else
                    {
                        string oldMenucode = getOldMenuData.MenuCode;

                        string str1 = oldMenucode + "_";
                        for (var i = 1; i < 50; i++)
                        {
                            string str2 = Convert.ToString(i);
                            string checkmenucode = str1 + str2;
                            var checkmenu = _menuManagement.GetMenuIdByMenuCode(checkmenucode);
                            if (checkmenu == 0)
                            {
                                newmenu.MenuCode = checkmenucode;
                                break;
                            }
                        }
                    }
                    newmenu.MenuTypeID = getOldMenuData.MenuTypeID;
                    newmenu.IsDeleted = false;
                    newmenu.CreatedAt = DateTime.Now;
                    newmenu.CreatedBy = userid;
                    newmenu.LanguageID = getOldMenuData.LanguageID;
                    newmenu.CycleID = currCycleId;
                    newmenu.CurrentApprovalStatusID = 1;
                    newmenu.IsMovedToLiveOrder = false;
                    newmenu.ReorderCount = null;
                    _context.tMenu.Add(newmenu);
                    _context.SaveChanges();

                    newMenuId = newmenu.ID;
                }
                //get and create new Menuforroutes
                var oldMenuforRoutesData = (from mfr in _context.tMenuForRoute where mfr.MenuID == menuId select mfr).ToList();

                foreach (var mfrdata in oldMenuforRoutesData)
                {
                    var newmfr = new tMenuForRoute();
                    newmfr.MenuID = newMenuId;
                    newmfr.RouteID = mfrdata.RouteID;
                    newmfr.Flights = mfrdata.Flights;
                    _context.tMenuForRoute.Add(newmfr);
                    _context.SaveChanges();
                }

                //get and create new MenuItem
                var oldMenuItemData = (from mi in _context.tMenuItem where mi.MenuID == menuId select mi).ToList();

                foreach (var midata in oldMenuItemData)
                {
                    var newmi = new tMenuItem();
                    newmi.MenuID = newMenuId;
                    newmi.BaseItemCode = midata.BaseItemCode;
                    _context.tMenuItem.Add(newmi);
                    _context.SaveChanges();
                }

                //get and create new MenuTemplate
                var oldMenuTemplate = (from mt in _context.tMenuTemplates where mt.MenuID == menuId select mt).FirstOrDefault();

                var newMenuTemp = new tMenuTemplates();
                newMenuTemp.MenuID = newMenuId;
                newMenuTemp.TemplateID = oldMenuTemplate.TemplateID;
                newMenuTemp.ChiliDocumentID = oldMenuTemplate.ChiliDocumentID;
                _context.tMenuTemplates.Add(newMenuTemp);
                _context.SaveChanges();

                return newMenuId;
            }
            else
            {
                return 0;
            }

        }

        public void DeleteBoxTicketData(long orderId)
        {
            try
            {
                var boxTickets = (from b in _context.tBoxTicketData where b.OrderId == orderId select b).ToList();

                foreach (var boxTicket in boxTickets)
                {
                    var template = GetBoxTicketTemplate(boxTicket.ID);

                    if (template != null)
                        _context.tBoxTicketTemplate.Remove(template);

                }
                _context.SaveChanges();


                foreach (var boxTicket in boxTickets)
                {
                    var task = _context.tPDFGenerationTasksPackingTicket.Where(t => t.BoxTicketID == boxTicket.ID).FirstOrDefault();
                    if (task != null)
                        _context.tPDFGenerationTasksPackingTicket.Remove(task);

                }
                _context.SaveChanges();


                foreach (var boxTicket in boxTickets)
                {
                    _context.tBoxTicketData.Remove(boxTicket);
                }

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(ex);
            }

        }


        public void AddBoxTicketData(tBoxTicketData boxTicketData)
        {

            DateTime dt;
            bool res = DateTime.TryParse(boxTicketData.Time, out dt);
            if (res)
                boxTicketData.Time = dt.ToString("HH:mm:ss");

            _context.tBoxTicketData.Add(boxTicketData);
            _context.SaveChanges();
        }


        public List<tBoxTicketData> GetBoxTicketData(long orderId)
        {
            return (_context.tBoxTicketData.Where(o => o.OrderId == orderId).OrderBy(o => o.Date).ToList());
        }


        public List<string> GetLcoalPDFFilesForBoxTickets()
        {
            return _context.tPDFGenerationTasksPackingTicket.Where(t => t.ID >= 319841).Select(t => t.LocalPDFFile).ToList();
        }



        public tBoxTicketTemplate GetBoxTicketTemplate(long boxTicketId)
        {
            var boxTicketTemplate = _context.tBoxTicketTemplate.Where(t => t.BoxTicketID == boxTicketId).FirstOrDefault();

            if (boxTicketTemplate != null)
                return boxTicketTemplate;

            return null;

        }

        public tBoxTicketTemplate CreateBoxTicketTemplate(long boxTicketId, string bound)
        {
            var boxTicketTemplate = new tBoxTicketTemplate();
            boxTicketTemplate.BoxTicketID = boxTicketId;

            if (bound.ToLower() == "inbound")
                boxTicketTemplate.TemplateID = 1;
            else
                boxTicketTemplate.TemplateID = 2;

            _context.tBoxTicketTemplate.Add(boxTicketTemplate);
            _context.SaveChanges();

            return boxTicketTemplate;
        }

        public void UpdateBoxTemplate(long boxTicketId, int TemplateID, string chilidoc)
        {
            var boxTempalte = (from t in _context.tBoxTicketTemplate where t.TemplateID == TemplateID && t.BoxTicketID == boxTicketId select t).FirstOrDefault();

            if (boxTempalte != null)
            {
                boxTempalte.ChiliDocumentID = chilidoc;

                _context.SaveChanges();
            }
        }

        public void DeleteBoxTicketProof(long orderId)
        {
            var boxTickets = (from b in _context.tBoxTicketData where b.OrderId == orderId select b).ToList();

            foreach (var boxTicket in boxTickets)
            {
                var template = GetBoxTicketTemplate(boxTicket.ID);

                if (template != null)
                    _context.tBoxTicketTemplate.Remove(template);

            }
            _context.SaveChanges();
        }

        public bool IsBoxTicketDataGenerated(long orderId)
        {
            var boxTickets = _context.tBoxTicketData.Where(b => b.OrderId == orderId).ToList();

            if (boxTickets != null && boxTickets.Count > 0)
                return true;

            return false;

        }
    }
}
