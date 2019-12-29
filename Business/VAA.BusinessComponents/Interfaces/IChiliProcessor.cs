using System.Collections.Generic;
using VAA.DataAccess.Model;
using VAA.Entities.VAAEntity;

namespace VAA.BusinessComponents.Interfaces
{
    /// <summary>
    /// Chili processing operations
    /// Resposible for Chili proof, PDF, Tasks
    /// </summary>
    public interface IChiliProcessor
    {
        void Connect(string environment);
        void Connect(string environment, string username, string password);

        #region ForMenu
        string CreateChiliDocumentForMenu(int instanceId, long menuId, int templateId, string docName);
        string CreateChiliDocumentForReOrderMenu(int instanceId, long menuId, string oldChiliDocId, string docName);
        void UpdateAllChiliDocuments(int instanceId, int cycleId);
        void UpdateChiliDocumentVariables(int instanceId, long menuId);

        void UpdateChiliDocumentVariables(int instanceId, long menuId, Dictionary<string, string> values);
        void UpdateChiliDocumentVariables(int instanceId, string chiliDocumentId, string[] templateProcessorClasses, Dictionary<string, string> values);

        void RebuildAllChiliDocuments(int instanceId, int cycleId, int? templateId);
        void RebuildChiliDocument(int instanceId, long menuId, int? templateId);

        void GetLatestPdfGenerationJobId(int instanceId);
        int CreatePdfGenerationJob(int instanceId, long menuId);
        void AddPdfGenerationTaskForMenuTemplate(tMenuTemplates menuTemplate);

        DataAccess.Model.PdfGenerationJob GetPdfGenerationJob(int jobId);
        DataAccess.Model.PdfGenerationTask GetPdfGenerationTask(int jobId, long menuId);
        void UpdateTask(int jobId, long menuId, int templateId, string status, string error);
        void UpdateTask(DataAccess.Model.PdfGenerationTask task);
        #endregion

        #region PackingTicket
        void UpdateChiliDocumentVariablesBoxTickets(int instanceId, long boxTicketId, Dictionary<string, string> values);
        string CreateChiliDocumentForPackingTicket(int instanceId, long boxTicketId, int templateId, string docName);
        tPDFGenerationTasksPackingTicket AddPdfGenerationTaskForBoxTicketTemplate(tBoxTicketTemplate boxTicketTemplate);
        PdfGenerationJobPackingTicket GetPdfGenerationJobPackingTicket(int jobId);
        PdfGenerationTaskPackingTicket GetPdfGenerationTaskPackingTicket(int jobId, long boxTicketId);
        void UpdateTaskPackingTicket(int jobId, long boxTicketId, int templateId, string status, string error);
        void UpdateTaskPackingTicket(PdfGenerationTaskPackingTicket task);
        #endregion

    }
}