namespace VAA.DataAccess.Model
{
    public class PdfGenerationTask
    {
        public int PdfGenerationJobId { get; set; }
        public long MenuId { get; set; }
        public int TemplateId { get; set; }
        public int CategoryId { get; set; }
        public string Status { get; set; }
        public int MergeQuantity { get; set; }
        public string ChiliDocumentId { get; set; }
        public string ChiliTaskId { get; set; }
        public string ChiliPdfurl { get; set; }
        public string ChiliError { get; set; }
        public string LocalPdfFile { get; set; }
    }
}