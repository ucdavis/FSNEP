namespace FSNEP.Core.Abstractions
{
    public class ReportResult
    {
        public ReportResult(byte[] reportContent, string contentType)
        {
            ReportContent = reportContent;
            ContentType = contentType;
        }

        public byte[] ReportContent { get; set; }
        public string ContentType { get; set; }
    }
}