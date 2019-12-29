using System;
using System.Collections.Generic;
using System.IO;
namespace VAA.BusinessComponents.Interfaces
{
    /// <summary>
    /// Menu processor operations
    /// </summary>
    public interface IMenuProcessor
    {
        List<string> ValidateServicePlanMenu(Stream stream);

        void ImportMenu(long cycleId, int classId, int menuTypeId, Stream stream, int userId);
        void UpdateMenuNames(List<long> menuIdCollection);
        void UpdateMenuHistory(List<long> menuIdCollection, int userId, string action);
        void CreateMenuTemplate(List<long> menuIdCollection);

        void CreateChiliVariableCollectionAndApply(List<long> menuIdCollection);
        void CreateChiliVariableCollectionAndApplyForMenuId(long menuId);
        void CreateChiliDocumentForMenuId(long menuId);
        void RebuildChiliDocumentForMenu(long menuId);
        void CreateChiliDocumentForReOrderMenu(List<long> menuIdCollection);

        void GeneratePdf(long cycleId, int classId, int menuTypeId, long routeId);
        void GeneratePdfForOrder(long cycleId, int classId, long orderId);
        void DownloadPdf(long cycleId, int classId, int menuTypeId, long routeId);

        void CalculateQuantity(long liveOrderId, DateTime fromDate, DateTime toDate);
      
    }
}