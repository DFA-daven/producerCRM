using System.Collections.Generic;
using CallForm.Core.Models;

namespace CallForm.Core.Services
{
    public interface IDataService
    {
        /// <summary />
        /// <returns>A <seealso cref="List<ProducerVisitReport>"/>.</returns>
        List<ProducerVisitReport> ToUpload();
        
        /// <summary />
        /// <returns>A <seealso cref="List<ReportListItem>"/>.</returns>
        List<ReportListItem> Recent();

        /// <summary>Replace the "ReasonCodes" table in the database.
        /// </summary>
        /// <returns>A <seealso cref="List<ReasonCode>"/>.</returns>
        List<ReasonCode> GetReasonsForCall();

        /// <summary>Replace the "ReasonCodes" table in the database.
        /// </summary>
        /// <param name="reasonCodes"></param>
        void UpdateReasons(List<ReasonCode> reasonCodes);

        /// <summary>Adds a <seealso cref="ProducerVisitReport"/> to the database.
        /// </summary>
        /// <param name="report">A <seealso cref="ProducerVisitReport"/>.</param>
        void Insert(ProducerVisitReport report);

        /// <summary />
        /// <param name="id"></param>
        /// <returns>A <seealso cref="ProducerVisitReport"/>.</returns>
        ProducerVisitReport GetReport(int id);

        int Count { get; }

        void ReportUploaded(int id);
    }
}