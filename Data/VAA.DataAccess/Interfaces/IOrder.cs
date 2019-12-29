using System;
using System.Collections.Generic;
using VAA.DataAccess.Model;
using VAA.Entities.VAAEntity;

namespace VAA.DataAccess.Interfaces
{
    /// <summary>
    /// Order related operations
    /// </summary>
    public interface IOrder
    {
        tLiveOrders GetLiveOrder();
        tOrders GetOrderById(long orderId);
        long GetLiveOrderCycleId();
        long GetCycleIdOfLiveOrder(long liveOrderId);
        long GetLiveOrderIdFromOrderId(long orderId);
        string GetLotNoFromOrderId(long orderId);
        List<MenuData> GetAllApprovalStatus();      
        List<MenuData> GetMenuStatusAndApprovers(long cycleid, int classid, int menutypeid, int userid);

        List<ApprovedMenu> GetAllApprovedMenu();
        void UpdateQuantity(Int64 LiveOrderDetailsId, int Quantity);

        List<CurrentPreviousOrder> GetAllCurrentOrders();
        List<CurrentPreviousOrder> GetAllPreviousOrders();
        List<ApprovedMenu> GetOrderDetailsbyOrderId(long orderId);
        List<ApprovedMenu> GetMenuDetailsbyOrderId(int orderId);
        List<OrderStatusList> GetAllOrderStatus();
        void UpdateOrderStatus(Int64 OrderRowId, int StatusId);
        tLiveOrders CreateReorderFromLiveOrder(Int64 orderId);      
        List<tLiveOrderDetails> GetLiveOrderDetails(Int64 liveOrderId);
        List<tLiveOrderDetails> GetRecentOrderDetails();
        void CreateOrderNow(DateTime startDate, DateTime endDate);
        void CreateOrderNow(DateTime startDate, DateTime endDate, string orderFriendlyName);
        void CreateLiveOrderNow(long menuId);
        void RemoveMenuFromLiveOrder(long menuId);
        void CreateReOrderNow(Int64 liveorderid, DateTime fromDate, DateTime toDate, string OrderFriendlyName);
        long CreateReorderMenuFromMenuid(long menuId, int userid);

        void DeleteBoxTicketData(long orderId);
        void AddBoxTicketData(tBoxTicketData boxTicketData);
        List<tBoxTicketData> GetBoxTicketData(long orderId);
        tBoxTicketTemplate GetBoxTicketTemplate(long boxTicketId);
        tBoxTicketTemplate CreateBoxTicketTemplate(long boxTicketId, string bound);
        void UpdateBoxTemplate(long boxTicketId, int TemplateID, string chilidoc);
        tLiveOrders CreateLiveOrderForReOrder(long menuId);
        void UpdateReOrderCount(List<string> menuIds);
    }
}