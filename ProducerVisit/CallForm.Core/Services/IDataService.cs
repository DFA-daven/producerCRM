using System.Collections.Generic;
using CallForm.Core.Models;

namespace CallForm.Core.Services
{
    public interface IDataService
    {
        /// <summary>
        /// </summary>
        /// <returns>A <seealso cref="List<>"/> of type <seealso cref="ProducerVisitReport"/>.</returns>
        List<ProducerVisitReport> ToUpload();
        
        /// <summary>
        /// </summary>
        /// <returns>A <seealso cref="List<>"/> of type <seealso cref="ReportListItem"/>.</returns>
        List<ReportListItem> Recent();

        /// <summary>
        /// </summary>
        /// <returns>A <seealso cref="List<>"/> of type <seealso cref="ReasonCodes"/>.</returns>
        List<ReasonCode> GetReasonsForCall();

        void UpdateReasons(List<ReasonCode> reasonCodes);
        void Insert(ProducerVisitReport report);

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A <seealso cref="ProducerVisitReport"/>.</returns>
        ProducerVisitReport GetReport(int id);

        int Count { get; }

        void ReportUploaded(int id);
    }
}