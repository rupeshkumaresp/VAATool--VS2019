using System.Collections.Generic;
using VAA.DataAccess.Model;
using VAA.Entities.VAAEntity;

namespace VAA.DataAccess.Interfaces
{
    /// <summary>
    /// Menu related operations
    /// </summary>
    public interface IMenu
    {
        List<MenuData> GetAllMenu();
        MenuData GetMenuById(long menuId);
        bool DeleteMenu(string menuCode);
        MenuData GetMenuByMenuCode(string menuCode);
        List<MenuData> GetMenuByCycle(long cycleId);
        List<MenuData> GetMenuByMenuType(int menuTypeId);
        List<MenuData> GetMenuByCycleAndMenuType(long cycleId, int menuTypeId);
        List<MenuData> GetMenuByRoute(long routeId);
        List<MenuData> GetMenuByCycleAndRoute(long cycleId, long routeId);
        List<MenuData> GetMenuByClass(int classId);
        List<MenuData> GetMenuByCycleAndClass(long cycleId, int classId);
        List<MenuData> GetMenuByCycleClassAndMenutype(long cycleId, int classId, int menuTypeId);
        List<MenuData> GetMenuByCycleClassMenutypeAndUserid(int userid, long cycleId, int classId, int menuTypeId);
        List<MenuData> GetRoutesByMenu(long menuId);
        List<MenuData> GetMenuByLanguage(int languageId);
        List<MenuData> GetMenuByCycleAndLanguage(long cycleId, int languageId);
        List<MenuData> GetMenuByDeparture(int departureId);
        List<MenuData> GetMenuByCycleAndDeparture(long cycleId, int departureId);
        List<MenuData> GetMenuByCycleDepartureAndClass(long cycleId, int departureId, int classId);
        List<MenuData> GetMenuByArrival(int arrivalId);
        List<MenuData> GetMenuByCycleAndArrival(long cycleId, int arrivalId);
        List<MenuData> GetMenuByCycleArrivalAndClass(long cycleId, int arrivalId, int classId);


        long AddMenu(string menuName, string menuCode, int menuTypeID, int createdBy, long cycleId, int languageId);
        void AddRouteForMenu(long menuId, long routeId, string flightNo);

        int GetMenuCurrentApprovalStatus(long menuId);

        string GetMenuPdfFileName(long menuId);
        string GetMenuName(long menuId);
        string GetMenuTitle(long menuId);
        string GetMenuCode(long menuId);
        bool IsMenuActive(long menuId);
        int GetMenuQuantity(long menuId);

        //Menu Items
        List<BaseItem> GetAllMenuItems(long menuId);
        bool AddMenuItem(long menuId, string baseItemCode);
        bool DeleteMenuItem(long menuId, MenuItem menuItem);

        //approvals
        List<tApprovalStatuses> GetMenuApprovalStatus(long menuId);
        List<MenuData> GetAllStatuses();
        int GetMenuNextApproverId(long menuId);
        bool UpdateStatus(MenuData menu, int UserId);

        List<tMenuHistory> GetMenuHistory(long menuId);
        void UpdateMenuHistory(long menuId, int userId, string action);

        //Class
        int GetMenuClass(int menuTypeId);
        List<tClass> GetAllClass();
        List<tMenuType> GetMenuTypeByClass(int classId);
        tMenuType GetMenuTypeById(int menuTypeId);

        string GetClassShortName(int classId);
        string GetMenuTypeName(int menuTypeId);

        tMenuTemplates GetMenuTemplate(long menuId);
        bool CreateMenuTemplate(long menuId, int templateId);
        bool UpdateMenuTemplate(long menuId, int templateId, string chilidoc);
        bool UpdateMenuTemplate(long menuId, int templateId);

        tTemplates GetTemplate(int menuTypeId, int languageId);
        tTemplates GetTemplate(int templateId);

        string GetLanguage(int languageId);

        void UpdateMenuNameBasedOnRoute(long menuId);
        long GetMenuIdByMenuCode(string menuCode);

        List<tMenuItemCategory> GetAllMenuItemCategory();
        tMenuItemCategory GetMenuItemCategory(string categoryCode, int language);

        tBoxTicketTemplate GetBoxTicketTemplate(long BoxTicketId);
        void UpdateMenuChangeNotification(string fromUser, string toUser, string menuCode, string menuName, string message);
    }
}

