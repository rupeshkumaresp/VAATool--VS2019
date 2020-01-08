using Elmah;
using System;
using System.Collections.Generic;
using System.Linq;
using VAA.DataAccess.Interfaces;
using VAA.DataAccess.Model;
using VAA.Entities.VAAEntity;

namespace VAA.DataAccess
{
    /// <summary>
    /// Menu management class - Handles Menu get, add, edit, delete of menu various menu related entities
    /// </summary>
    public class MenuManagement : IMenu
    {
        private readonly VAAEntities _context = new VAAEntities();

        public MenuManagement()
        {
        }



        public List<MenuData> GetAllMenu()
        {
            var menus = (from i in _context.tMenu
                         select new MenuData
                         {
                             Id = i.ID,
                             MenuName = i.MenuName,
                             MenuCode = i.MenuCode,
                             IsDeleted = i.IsDeleted
                         }).ToList();
            return menus;
        }

        public MenuData GetMenuById(long menuId)
        {
            return (from m in _context.tMenu
                    join a in _context.tApprovalStatuses
                    on m.CurrentApprovalStatusID equals a.ApprovalStatusID
                    where m.ID == menuId
                    select new MenuData
                    {
                        Id = m.ID,
                        MenuName = m.MenuName,
                        MenuCode = m.MenuCode,
                        ApprovalStatusName = a.Status,
                        MenuTypeId = m.MenuTypeID,
                        LanguageId = m.LanguageID,
                        CycleId = m.CycleID,
                    }).FirstOrDefault();

        }

        public bool DeleteMenu(string menuCode)
        {
            bool deleted = true;

            try
            {
                var menu = GetMenuByMenuCode(menuCode);

                var allHistory = _context.tMenuHistory.Where(h => h.MenuID == menu.Id).ToList();

                foreach (var history in allHistory)
                {
                    if (history != null)
                        _context.tMenuHistory.Remove(history);
                }

                var menutemplate = _context.tMenuTemplates.Where(t => t.MenuID == menu.Id).FirstOrDefault();
                if (menutemplate != null)
                    _context.tMenuTemplates.Remove(menutemplate);

                var allItems = _context.tMenuItem.Where(h => h.MenuID == menu.Id).ToList();

                foreach (var item in allItems)
                {
                    if (item != null)
                        _context.tMenuItem.Remove(item);
                }

                var allRoutes = _context.tMenuForRoute.Where(h => h.MenuID == menu.Id).ToList();

                foreach (var route in allRoutes)
                {
                    if (route != null)
                        _context.tMenuForRoute.Remove(route);
                }

                var allLiveOrdersDetails = _context.tLiveOrderDetails.Where(h => h.MenuId == menu.Id).ToList();

                foreach (var liveOrderdetails in allLiveOrdersDetails)
                {
                    if (liveOrderdetails != null)
                        _context.tLiveOrderDetails.Remove(liveOrderdetails);
                }

                var allapprovalStage = _context.tMenuApprovalStage.Where(h => h.MenuID == menu.Id).ToList();

                foreach (var approvalStage in allapprovalStage)
                {
                    if (approvalStage != null)
                        _context.tMenuApprovalStage.Remove(approvalStage);
                }

                var pdfTasks = _context.tPDFGenerationTasks.Where(h => h.MenuID == menu.Id).ToList();

                foreach (var pdfTask in pdfTasks)
                {
                    if (pdfTask != null)
                        _context.tPDFGenerationTasks.Remove(pdfTask);
                }

                var tmenu = _context.tMenu.Where(m => m.ID == menu.Id).FirstOrDefault();

                if (tmenu != null)
                    _context.tMenu.Remove(tmenu);

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                deleted = false;
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            return deleted;
        }

        public MenuData GetMenuByMenuCode(string menuCode)
        {
            return (from m in _context.tMenu
                    join a in _context.tApprovalStatuses
                    on m.CurrentApprovalStatusID equals a.ApprovalStatusID
                    where m.MenuCode == menuCode
                    select new MenuData
                    {
                        Id = m.ID,
                        MenuName = m.MenuName,
                        MenuCode = m.MenuCode,
                        ApprovalStatusName = a.Status,
                        MenuTypeId = m.MenuTypeID,
                        LanguageId = m.LanguageID,
                        CycleId = m.CycleID,
                    }).FirstOrDefault();

        }
        public List<MenuData> GetAllStatuses()
        {
            return (from s in _context.tApprovalStatuses
                    select new MenuData
                    {
                        ApprovalStatusId = s.ApprovalStatusID,
                        ApprovalStatusName = s.Status
                    }).ToList();
        }
        public bool UpdateStatus(MenuData menu, int UserId)
        {
            try
            {
                var statusUpdate = (from m in _context.tMenu where m.ID == menu.Id select m).FirstOrDefault();
                if (statusUpdate != null)
                {
                    //get current status, if status is same then do not update the status
                    if (statusUpdate.CurrentApprovalStatusID == menu.ApprovalStatusId)
                        return true;

                    statusUpdate.CurrentApprovalStatusID = menu.ApprovalStatusId;
                    _context.SaveChanges();

                    var status = (from s in _context.tApprovalStatuses where s.ApprovalStatusID == menu.ApprovalStatusId select s).FirstOrDefault();

                    var menuHistory = new tMenuHistory();
                    menuHistory.ActionByUserID = UserId;
                    menuHistory.MenuID = menu.Id;
                    menuHistory.ModifiedAt = DateTime.Now;
                    menuHistory.ActionTaken = "Status Changed To: " + status.Status;

                    _context.tMenuHistory.Add(menuHistory);
                    _context.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(e);
                return false;
            }
        }
        public List<MenuData> GetMenuByCycle(long cycleId)
        {
            var menus = (from i in _context.tMenu
                         where i.CycleID == cycleId
                         select new MenuData
                         {
                             Id = i.ID,
                             MenuName = i.MenuName,
                             MenuCode = i.MenuCode,
                             IsDeleted = i.IsDeleted
                         }).ToList();
            return menus;
        }

        public List<MenuData> GetMenuByMenuType(int menuTypeId)
        {
            throw new NotImplementedException();
        }

        public List<MenuData> GetMenuByCycleAndMenuType(long cycleId, int menuTypeId)
        {
            var menus = (from i in _context.tMenu
                         where i.CycleID == cycleId && i.MenuTypeID == menuTypeId
                         select new MenuData
                         {
                             Id = i.ID,
                             MenuName = i.MenuName,
                             MenuCode = i.MenuCode
                         }).ToList();
            return menus;
        }


        public int GetMenuClass(int menuTypeId)
        {
            var menuClass = (from c in _context.tClassMenuTypeMap where c.MenuTypeID == menuTypeId select c).FirstOrDefault();

            if (menuClass != null)
                return menuClass.FlightClassID;

            return 0;

        }
        public List<MenuData> GetMenuByRoute(long routeId)
        {
            throw new NotImplementedException();
        }

        public List<MenuData> GetMenuByCycleAndRoute(long cycleId, long routeId)
        {
            throw new NotImplementedException();
        }

        public List<MenuData> GetMenuByClass(int classId)
        {
            throw new NotImplementedException();
        }

        public List<MenuData> GetMenuByCycleAndClass(long cycleId, int classId)
        {
            throw new NotImplementedException();
        }

        public List<MenuData> GetMenuByCycleClassAndMenutype(long cycleId, int classId, int menuTypeId)
        {
            try
            {
                return (from m in _context.tMenu
                        join cmtm in _context.tClassMenuTypeMap on m.MenuTypeID equals cmtm.MenuTypeID
                        join lang in _context.tMenuLanguage on m.LanguageID equals lang.ID
                        where m.CycleID == cycleId && cmtm.FlightClassID == classId && m.MenuTypeID == menuTypeId
                        select new MenuData
                        {
                            Id = m.ID,
                            MenuName = m.MenuName,
                            MenuCode = m.MenuCode,
                            LanguageId = m.LanguageID,
                            LanguageName = lang.LanguageCode
                        }).ToList();
            }
            catch (Exception e)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(e);

                return new List<MenuData>();
            }
        }
        public List<MenuData> GetMenuByCycleClassMenutypeAndUserid(int userId, long cycleId, int classId, int menuTypeId)
        {
            try
            {
                return (from m in _context.sp_GetMenuByUserId(userId, cycleId, classId, menuTypeId)
                        select new MenuData
                        {
                            Id = m.ID,
                            MenuName = m.MenuName,
                            MenuCode = m.MenuCode,
                            LanguageId = m.LanguageID,
                            LanguageName = m.LanguageCode
                        }).Distinct().ToList();
            }
            catch (Exception e)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(e);
                return new List<MenuData>();
            }
        }

        public List<MenuData> GetRoutesByMenu(long menuId)
        {
            try
            {
                return (from mfr in _context.tMenuForRoute
                        join rd in _context.tRouteDetails on mfr.RouteID equals rd.RouteID
                        join r in _context.tRegion on rd.RegionID equals r.ID
                        join dl in _context.tLocation on rd.DepartureID equals dl.LocationID
                        join al in _context.tLocation on rd.ArrivalID equals al.LocationID
                        where mfr.MenuID == menuId
                        select new MenuData
                        {
                            RouteId = mfr.RouteID,
                            RegionName = r.RegionName,
                            DepartureAirportName = dl.AirportName,
                            DepartureAirportCode = dl.AirportCode,
                            ArrivalAirportName = al.AirportName,
                            ArrivalAirportCode = al.AirportCode,
                            FlightNo = mfr.Flights
                        }).ToList();

            }
            catch (Exception e)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(e);
                return new List<MenuData>();
            }
        }

        public List<MenuData> GetMenuByCycleRouteAndClass(long cycleId, long routeId, int classId)
        {
            throw new NotImplementedException();
        }

        public List<MenuData> GetMenuByLanguage(int languageId)
        {
            throw new NotImplementedException();
        }

        public List<MenuData> GetMenuByCycleAndLanguage(long cycleId, int languageId)
        {
            throw new NotImplementedException();
        }

        public List<MenuData> GetMenuByDeparture(int departureId)
        {
            throw new NotImplementedException();
        }

        public List<MenuData> GetMenuByCycleAndDeparture(long cycleId, int departureId)
        {
            throw new NotImplementedException();
        }

        public List<MenuData> GetMenuByCycleDepartureAndClass(long cycleId, int departureId, int classId)
        {
            throw new NotImplementedException();
        }

        public List<MenuData> GetMenuByArrival(int arrivalId)
        {
            throw new NotImplementedException();
        }

        public List<MenuData> GetMenuByCycleAndArrival(long cycleId, int arrivalId)
        {
            throw new NotImplementedException();
        }

        public List<MenuData> GetMenuByCycleArrivalAndClass(long cycleId, int arrivalId, int classId)
        {
            throw new NotImplementedException();
        }

        public int GetMenuCurrentApprovalStatus(long menuId)
        {
            throw new NotImplementedException();
        }

        public string GetMenuPdfFileName(long menuId)
        {
            throw new NotImplementedException();
        }

        public string GetMenuName(long menuId)
        {
            throw new NotImplementedException();
        }

        public string GetMenuTitle(long menuId)
        {
            throw new NotImplementedException();
        }

        public string GetMenuCode(long menuId)
        {
            throw new NotImplementedException();
        }

        public bool IsMenuActive(long menuId)
        {
            throw new NotImplementedException();
        }

        public int GetMenuQuantity(long menuId)
        {
            throw new NotImplementedException();
        }

        public List<BaseItem> GetAllMenuItems(long menuId)
        {
            try
            {
                List<BaseItem> baseItemCollection = new List<BaseItem>();

                var menu = GetMenuById(menuId);

                var menuItemCollection = (from mi in _context.tMenuItem where mi.MenuID == menuId select mi).ToList();

                foreach (var menuItem in menuItemCollection)
                {
                    var baseItemData = (from baseItem in _context.tBaseItems where baseItem.BaseItemCode == menuItem.BaseItemCode select baseItem).FirstOrDefault();

                    if (baseItemData != null)
                    {
                        var categoryId = baseItemData.CategoryID;

                        var category =
                            (from c in _context.tMenuItemCategory where c.ID == categoryId select c).FirstOrDefault();

                        if (category != null)
                        {
                            BaseItem baseItem = new BaseItem()
                            {
                                BaseItemId = baseItemData.ID,
                                BaseItemCode = baseItemData.BaseItemCode,
                                ClassId = baseItemData.ClassID,
                                MenuTypeId = baseItemData.MenuTypeID,
                                CategoryName = category.CategoryName,
                                CategoryCode = category.CategoryCode,
                                BaseItemTitle = baseItemData.BaseItemTitle,
                                BaseItemTitleDescription = baseItemData.BaseItemTitleDescription,
                                BaseItemDescription = baseItemData.BaseItemDescription,
                                BaseItemSubDescription = baseItemData.BaseItemSubDescription,
                                BaseItemAttributes = baseItemData.BaseItemAttributes
                            };

                            baseItemCollection.Add(baseItem);
                        }
                    }
                }
                return baseItemCollection;
            }
            catch (Exception e)
            {
                //write to Elma
                ErrorSignal.FromCurrentContext().Raise(e);
                return null;
            }
        }

        public bool AddMenuItem(long menuId, string baseItemCode)
        {
            var item = new tMenuItem();
            item.BaseItemCode = baseItemCode;
            item.MenuID = menuId;
            //item.Sequence = sequence;

            _context.tMenuItem.Add(item);
            _context.SaveChanges();

            return true;
        }

        public bool DeleteMenuItem(long menuId, MenuItem menuItem)
        {
            throw new NotImplementedException();
        }

        public List<tApprovalStatuses> GetMenuApprovalStatus(long menuId)
        {
            throw new NotImplementedException();
        }

        public int GetMenuNextApproverId(long menuId)
        {
            throw new NotImplementedException();
        }

        public List<tMenuHistory> GetMenuHistory(long menuId)
        {
            return _context.tMenuHistory.Where(h => h.MenuID == menuId).ToList();
        }

        public List<tClass> GetAllClass()
        {
            return _context.tClass.ToList();
        }

        public List<tMenuType> GetMenuTypeByClass(int classId)
        {
            List<tMenuType> menuTypeList = new List<tMenuType>();

            //var menuTypeMap = _context.tClassMenuTypeMap.Where(mt => mt.FlightClassID == classId).ToList();

            var menuTypeMap =
                (from mtp in _context.tClassMenuTypeMap where mtp.FlightClassID == classId select mtp).ToList();


            foreach (var mtm in menuTypeMap)
            {
                var menuType = _context.tMenuType.FirstOrDefault(mt => mt.ID == mtm.MenuTypeID);

                if (menuType != null)
                    menuTypeList.Add(menuType);
            }

            return menuTypeList;
        }

        public List<tMenuItemCategory> GetAllMenuItemCategory()
        {
            return _context.tMenuItemCategory.ToList();
        }


        public tMenuItemCategory GetMenuItemCategory(string categoryCode, int language)
        {
            var menuItemCategory = (from mc in _context.tMenuItemCategory where mc.CategoryCode == categoryCode && mc.LanguageId == language select mc).FirstOrDefault();

            if (menuItemCategory != null)
            {
                return menuItemCategory;
            }

            return null;

        }



        public long AddMenu(string menuName, string menuCode, int menuTypeID, int createdBy, long cycleId, int languageId)
        {
            tMenu menu = new tMenu();

            menu.MenuName = menuName;
            menu.MenuCode = menuCode;
            menu.MenuTypeID = menuTypeID;
            menu.CreatedBy = createdBy;
            menu.CycleID = cycleId;
            menu.LanguageID = languageId;
            menu.CreatedAt = DateTime.Now;
            menu.IsDeleted = false;
            menu.CurrentApprovalStatusID = 1;


            _context.tMenu.Add(menu);
            _context.SaveChanges();

            return menu.ID;

        }




        public void RemoveRouteForMenu(long menuId, long routeId, string flightNo)
        {
            var routeForMenu = (from rm in _context.tMenuForRoute where rm.MenuID == menuId && rm.RouteID == routeId select rm).FirstOrDefault();

            if (routeForMenu != null)
            {
                if (!routeForMenu.Flights.Contains(","))
                {
                    _context.tMenuForRoute.Remove(routeForMenu);
                    _context.SaveChanges();

                }
                else
                {
                    var flightsPart = routeForMenu.Flights.Split(new char[] { ',' });

                    var revisedflights = "";
                    for (int icount = 0; icount < flightsPart.Length; icount++)
                    {
                        if (flightsPart[icount] != flightNo)
                            revisedflights += flightsPart[icount] + ",";
                    }

                    if (revisedflights.EndsWith(","))
                        revisedflights = revisedflights.Substring(0, revisedflights.Length - 1);


                    routeForMenu.Flights = revisedflights;

                    _context.SaveChanges();
                }
            }
        }



        public void AddRouteForMenu(long menuId, long routeId, string flightNo)
        {
            var routeForMenu = (from rm in _context.tMenuForRoute where rm.MenuID == menuId && rm.RouteID == routeId select rm).FirstOrDefault();

            if (routeForMenu == null)
            {
                routeForMenu = new tMenuForRoute();

                routeForMenu.MenuID = menuId;
                routeForMenu.RouteID = routeId;
                routeForMenu.Flights = flightNo;
                _context.tMenuForRoute.Add(routeForMenu);
                _context.SaveChanges();
            }
            else
            {
                routeForMenu.Flights = routeForMenu.Flights + "," + flightNo;
                _context.SaveChanges();
            }
        }


        public string GetClassShortName(int classId)
        {

            var flightClass = (from c in _context.tClass where c.ID == classId select c).FirstOrDefault();

            if (flightClass != null)
            {
                return flightClass.ShortName;
            }

            return "";
        }


        public string GetMenuTypeName(int menuTypeId)
        {
            var menuType = (from mt in _context.tMenuType where mt.ID == menuTypeId select mt).FirstOrDefault();

            if (menuType != null)
            {
                return menuType.DisplayName;
            }

            return "";
        }

        public tMenuTemplates GetMenuTemplate(long menuId)
        {
            var menuTemplate = _context.tMenuTemplates.Where(mt => mt.MenuID == menuId).FirstOrDefault();

            if (menuTemplate != null)
                return menuTemplate;

            return null;

        }

        public bool CreateMenuTemplate(long menuId, int templateId)
        {
            var menutemplate = _context.tMenuTemplates.Where(mt => mt.MenuID == menuId).FirstOrDefault();

            if (menutemplate == null)
            {
                menutemplate = new tMenuTemplates();
                menutemplate.MenuID = menuId;
                menutemplate.TemplateID = templateId;

                _context.tMenuTemplates.Add(menutemplate);
                _context.SaveChanges();
            }
            return true;
        }

        public bool UpdateMenuTemplate(long menuId, int templateId, string chilidoc)
        {
            var menutemplate = _context.tMenuTemplates.Where(mt => mt.MenuID == menuId).FirstOrDefault();

            if (menutemplate != null)
            {
                menutemplate.ChiliDocumentID = chilidoc;
                menutemplate.TemplateID = templateId;
                _context.SaveChanges();
            }
            else
            {
                menutemplate = new tMenuTemplates();
                menutemplate.MenuID = menuId;
                menutemplate.TemplateID = templateId;
                menutemplate.ChiliDocumentID = chilidoc;

                _context.tMenuTemplates.Add(menutemplate);
                _context.SaveChanges();
            }
            return true;
        }

        public bool UpdateMenuTemplate(long menuId, int templateId)
        {
            var menutemplate = _context.tMenuTemplates.Where(mt => mt.MenuID == menuId).FirstOrDefault();

            if (menutemplate != null)
            {
                menutemplate.TemplateID = templateId;
                _context.SaveChanges();
            }
            else
            {
                menutemplate = new tMenuTemplates();
                menutemplate.MenuID = menuId;
                menutemplate.TemplateID = templateId;

                _context.tMenuTemplates.Add(menutemplate);
                _context.SaveChanges();
            }
            return true;
        }


        public tTemplates GetTemplate(int menuTypeId, int languageId)
        {
            var template = (from t in _context.tTemplates where t.MenuTypeID == menuTypeId && t.LanguageId == languageId select t).FirstOrDefault();

            if (template != null)
                return template;
            return null;
        }


        public tTemplates GetTemplate(int templateId)
        {
            var template = (from t in _context.tTemplates where t.TemplateID == templateId select t).FirstOrDefault();

            if (template != null)
                return template;
            return null;
        }


        public string GetLanguage(int languageId)
        {
            var language = _context.tMenuLanguage.Where(l => l.ID == languageId).FirstOrDefault();

            if (language != null)
                return language.LanguageCode;

            return "";
        }

        public void UpdateMenuNameBasedOnRoute(long menuId)
        {
            var routes = (from r in _context.tMenuForRoute where r.MenuID == menuId select r).ToList();

            string flightdetails = "";

            foreach (var route in routes)
            {
                var flight = route.Flights;

                if (route.Flights.Contains(","))
                    flight = flight.Replace(",", "/");

                flightdetails += "/" + flight;
            }

            var menu = (from m in _context.tMenu where m.ID == menuId select m).FirstOrDefault();

            menu.MenuName += flightdetails;

            _context.SaveChanges();
        }

        public void UpdateMenuHistory(long menuId, int userId, string action)
        {
            tMenuHistory menuHistory = new tMenuHistory();
            menuHistory.ActionByUserID = userId;
            menuHistory.ActionTaken = action;
            menuHistory.ModifiedAt = System.DateTime.Now;
            menuHistory.MenuID = menuId;
            _context.tMenuHistory.Add(menuHistory);
            _context.SaveChanges();
        }
        public tMenuType GetMenuTypeById(int menuTypeId)
        {
            var menuType = _context.tMenuType.FirstOrDefault(mt => mt.ID == menuTypeId);

            return menuType;
        }
        public long GetMenuIdByMenuCode(string menuCode)
        {
            var menuid = (from m in _context.tMenu where m.MenuCode == menuCode select m.ID).FirstOrDefault();
            if (menuid != null)
            {
                return menuid;
            }
            else
            {
                return 0;
            }
        }

        public tBoxTicketTemplate GetBoxTicketTemplate(long BoxTicketId)
        {
            return _context.tBoxTicketTemplate.Where(t => t.BoxTicketID == BoxTicketId).FirstOrDefault();
        }

        public void UpdateMenuChangeNotification(string fromUser, string toUser, string menuCode, string menuName, string message)
        {
            tNotification notification = new tNotification();

            notification.FromUser = fromUser;
            notification.MenuCode = menuCode;
            notification.MenuName = menuName;
            notification.NotificationMessage = message;
            notification.SentToUser = toUser;
            notification.CreatedAt = System.DateTime.Now;

            _context.tNotification.Add(notification);

            _context.SaveChanges();

        }


        public int GetTemplateIdByChiliDocumentId(string chiliDocumentId)
        {
            var template = _context.tTemplates.Where(t => t.ChiliDocumentID == chiliDocumentId).FirstOrDefault();

            if (template != null)
                return template.TemplateID;

            return 0;
        }

        public int GetLanguageIdByChiliDocumentId(string chiliDocumentId)
        {
            var template = _context.tTemplates.Where(t => t.ChiliDocumentID == chiliDocumentId).FirstOrDefault();

            if (template != null)
                return Convert.ToInt32(template.LanguageId);

            return 0;
        }

        public void UpdateMenuLanguage(long menuId, int languageId)
        {
            var menu = _context.tMenu.Where(m => m.ID == menuId).FirstOrDefault();

            if (menu != null)
            {
                menu.LanguageID = languageId;
                _context.SaveChanges();
            }
        }

        public string GetBreakfastMenuCodeFromMainMenuCode(string menuCode, long? routeId, string FlightNo, long liveOrderId)
        {
            // TODO: Implement this method
            var mainMenu = GetMenuByMenuCode(menuCode);

            var cycleId = mainMenu.CycleId;
            var liveOrderDetails = (from lod in _context.tLiveOrderDetails where lod.RouteId == routeId && lod.FlightNo == FlightNo && lod.LiveOrderId == liveOrderId select lod).ToList();

            foreach (var lo in liveOrderDetails)
            {
                var menuId = lo.MenuId;

                var menuObj = GetMenuById(Convert.ToInt64(menuId));

                if (menuObj.MenuTypeId == 3 && menuObj.CycleId == cycleId)
                {
                    return menuObj.MenuCode;
                }


            }
            return "";


        }

        public string GetTeaMenuCodeFromMainMenuCode(string menuCode, long? routeId, string FlightNo, long liveOrderId)
        {
            // TODO: Implement this method
            var mainMenu = GetMenuByMenuCode(menuCode);

            var cycleId = mainMenu.CycleId;
            var liveOrderDetails = (from lod in _context.tLiveOrderDetails where lod.RouteId == routeId && lod.FlightNo == FlightNo && lod.LiveOrderId == liveOrderId select lod).ToList();

            foreach (var lo in liveOrderDetails)
            {
                var menuId = lo.MenuId;

                var menuObj = GetMenuById(Convert.ToInt64(menuId));

                if (menuObj.MenuTypeId == 2 && menuObj.CycleId == cycleId)
                {
                    return menuObj.MenuCode;
                }


            }
            return "";
        }

        public string GetWineCardMenuCodeFromMainMenuCode(string menuCode, long? routeId, string FlightNo)
        {
            // TODO: Implement this method
            var mainMenu = GetMenuByMenuCode(menuCode);

            var cycleId = mainMenu.CycleId;
            var liveOrderDetails = (from lod in _context.tLiveOrderDetails where lod.RouteId == routeId && lod.FlightNo == FlightNo select lod).ToList();

            foreach (var lo in liveOrderDetails)
            {
                var menuId = lo.MenuId;

                var menuObj = GetMenuById(Convert.ToInt64(menuId));

                if (menuObj.MenuTypeId == 4 && menuObj.CycleId == cycleId)
                {
                    return menuObj.MenuCode;
                }


            }
            return "";
        }

        public void updateTeaBoxTicketData(long ticketid, string teaCode)
        {
            var ticket = _context.tBoxTicketData.Where(t => t.ID == ticketid).FirstOrDefault();
            ticket.TEAMenuCode = teaCode;
            _context.SaveChanges();
        }

        public void updateBRKBoxTicketData(long ticketid, string BRKCode)
        {
            var ticket = _context.tBoxTicketData.Where(t => t.ID == ticketid).FirstOrDefault();
            ticket.BRKMenuCode = BRKCode;
            _context.SaveChanges();
        }

        public void UpdateMenuName(long menuId, string newMenuName)
        {

            var menu = _context.tMenu.Where(m => m.ID == menuId).FirstOrDefault();

            menu.MenuName = newMenuName;

            _context.SaveChanges();

        }

        public void UpdateMenuCode(long menuId, string newMenuCode)
        {

            var menu = _context.tMenu.Where(m => m.ID == menuId).FirstOrDefault();

            menu.MenuCode = newMenuCode;

            _context.SaveChanges();

        }


        public Dictionary<long, long> CloneMenuCodesToNewCycle(string fromCycle, string menuCodes, string toCycle)
        {
            Dictionary<long, long> NewClonedMenus = new Dictionary<long, long>();

            var cycleFrom = _context.tCycle.Where(c => c.CycleName == fromCycle).FirstOrDefault();
            var cycleTo = _context.tCycle.Where(c => c.CycleName == toCycle).FirstOrDefault();

            var fromCycleID = cycleFrom.CycleID;
            var toCycleID = cycleTo.CycleID;


            List<tMenu> oldMenus = new List<tMenu>();

            var oldMenuArray = menuCodes.Split(new char[] { ',' });

            foreach (var om in oldMenuArray)
            {
                var menu = _context.tMenu.Where(m => m.MenuCode == om).FirstOrDefault();

                oldMenus.Add(menu);
            }
            //clone menu now

            CloneMenus(fromCycle, NewClonedMenus, cycleFrom, cycleTo, toCycleID, oldMenus);


            return NewClonedMenus;
        }

        public Dictionary<long, long> CloneMenusToNewCycle(string fromCycle, string weekNo, string toCycle)
        {
            Dictionary<long, long> NewClonedMenus = new Dictionary<long, long>();

            var cycleFrom = _context.tCycle.Where(c => c.CycleName == fromCycle).FirstOrDefault();
            var cycleTo = _context.tCycle.Where(c => c.CycleName == toCycle).FirstOrDefault();

            var fromCycleID = cycleFrom.CycleID;
            var toCycleID = cycleTo.CycleID;

            int week = Convert.ToInt32(weekNo);


            //clone Upper class main menu

            var ucMainMenus = _context.tMenu.Where(m => m.CycleID == fromCycleID && m.MenuTypeID == 1 && m.MenuCode.Contains("_" + week.ToString())).ToList();

            //clone menu now

            CloneMenus(fromCycle, NewClonedMenus, cycleFrom, cycleTo, toCycleID, ucMainMenus);

            //clone upper class brk cards
            var ucBRKMenus = _context.tMenu.Where(m => m.CycleID == fromCycleID && m.MenuTypeID == 3 && m.MenuCode.Contains("_" + week.ToString())).ToList();
            CloneMenus(fromCycle, NewClonedMenus, cycleFrom, cycleTo, toCycleID, ucBRKMenus);


            //clone upper class food guide
            var ucFGMenus = _context.tMenu.Where(m => m.CycleID == fromCycleID && m.MenuTypeID == 5).ToList();
            CloneMenus(fromCycle, NewClonedMenus, cycleFrom, cycleTo, toCycleID, ucFGMenus);


            //clone PE main menu
            var peMainMenus = _context.tMenu.Where(m => m.CycleID == fromCycleID && m.MenuTypeID == 10 && m.MenuCode.Contains("_" + week.ToString())).ToList();
            CloneMenus(fromCycle, NewClonedMenus, cycleFrom, cycleTo, toCycleID, peMainMenus);


            //clone ECO main menu
            var ecoMainMenus = _context.tMenu.Where(m => m.CycleID == fromCycleID && m.MenuTypeID == 13 && m.MenuCode.Contains("_" + week.ToString())).ToList();
            CloneMenus(fromCycle, NewClonedMenus, cycleFrom, cycleTo, toCycleID, ecoMainMenus);



            return NewClonedMenus;
        }

        private void CloneMenus(string fromCycle, Dictionary<long, long> NewClonedMenus, tCycle cycleFrom, tCycle cycleTo, long toCycleID, List<tMenu> oldMenus)
        {
            foreach (var oldMenu in oldMenus)
            {
                if (oldMenu == null)
                    continue;

                //create menu
                var oldMenuId = oldMenu.ID;

                tMenu newMenu = new tMenu();

                var menuName = oldMenu.MenuName.Replace(cycleFrom.CycleShortName, cycleTo.CycleShortName);
                newMenu.MenuName = menuName;
                //TODO : assign new menu code
                //newMenu.MenuCode = oldMenu.MenuCode;
                newMenu.MenuTypeID = oldMenu.MenuTypeID;
                newMenu.IsDeleted = oldMenu.IsDeleted;
                newMenu.CreatedAt = System.DateTime.Now;
                newMenu.CreatedBy = 2;
                newMenu.LanguageID = oldMenu.LanguageID;
                //assign new cycle id

                newMenu.CycleID = toCycleID;
                newMenu.CurrentApprovalStatusID = 1;


                _context.tMenu.Add(newMenu);
                _context.SaveChanges();


                var newMenuId = newMenu.ID;


                //create menu for route
                var menuForRoutes = _context.tMenuForRoute.Where(mr => mr.MenuID == oldMenuId).ToList();

                foreach (var menuForRoute in menuForRoutes)
                {
                    tMenuForRoute newMenuForRoute = new tMenuForRoute();
                    newMenuForRoute.MenuID = newMenuId;
                    newMenuForRoute.RouteID = menuForRoute.RouteID;
                    newMenuForRoute.Flights = menuForRoute.Flights;

                    _context.tMenuForRoute.Add(newMenuForRoute);
                    _context.SaveChanges();

                }


                //write to menu history

                tMenuHistory history = new tMenuHistory();

                history.MenuID = newMenuId;
                history.ActionTaken = "Menu created by cloning from menu - " + oldMenu.MenuCode + "  cycle -" + fromCycle;
                history.ActionByUserID = 2;
                history.ModifiedAt = System.DateTime.Now;

                _context.tMenuHistory.Add(history);
                _context.SaveChanges();

                //clone menu chili doc tempalte and assign

                var oldMenuTemplate = _context.tMenuTemplates.Where(t => t.MenuID == oldMenuId).FirstOrDefault();

                tMenuTemplates newMenuTemplate = new tMenuTemplates();

                newMenuTemplate.MenuID = newMenuId;

                newMenuTemplate.ChiliDocumentID = "";
                newMenuTemplate.TemplateID = oldMenuTemplate.TemplateID;

                _context.tMenuTemplates.Add(newMenuTemplate);
                _context.SaveChanges();


                NewClonedMenus.Add(newMenuId, oldMenuId);
            }
        }
    }
}
