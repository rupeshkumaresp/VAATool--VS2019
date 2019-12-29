using System;
using System.Collections.Generic;
using System.IO;
using VAA.DataAccess.Model;
namespace VAA.BusinessComponents.Interfaces
{
    /// <summary>
    /// PackingTicket operations
    /// </summary>
    public interface IPackingTicket
    {
        void CalculateBoxTicketData(long orderId);
        Dictionary<string, int> CalculatePrintRun(long orderId);
        List<BoxTicketSortedData> CreateBoxTicketProofs(long orderId);
        void CreateBoxTicketPDfs(long orderId, List<BoxTicketSortedData> sortedBoxTicketData);
        void CreateBoxTicketProofsAndTicketPDF(long orderId);
        void GenerateSinglePackingTicketPDFs(int jobID, long boxTicketID);
    }
}