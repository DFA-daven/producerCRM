using System.Collections.Generic;
using CallForm.Core.Models;

namespace CallForm.Core.Services
{
    public interface IDataService
    {
        List<ProducerVisitReport> ToUpload();
        List<ReportListItem> Recent();
        List<ReasonCode> GetReasonsForCall();
        void UpdateReasons(List<ReasonCode> reasonCodes);
        void Insert(ProducerVisitReport report);
        ProducerVisitReport GetReport(int id);
        int Count { get; }
        void ReportUploaded(int id);
    }
}