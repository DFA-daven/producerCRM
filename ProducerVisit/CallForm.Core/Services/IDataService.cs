using System.Collections.Generic;
using CallForm.Core.Models;

namespace CallForm.Core.Services
{
    /// <summary>Interface for visit reports stored in the SQLite database.
    /// </summary>
    /// <remarks>Signatures of methods, properties, events and/or indexers</remarks>
    public interface IDataService
    {
        /// <summary>Opens the SQLite database, gets rows from "StoredProducerVisitReport"
        /// where "Uploaded" is false, and returns them as <see cref="List<ProducerVisitReport>"/>.
        /// </summary>
        /// <returns>A <see cref="List<ProducerVisitReport>"/> where "Uploaded" is false.
        List<ProducerVisitReport> ToUpload();

        /// <summary>Opens the SQLite database, gets the most recent <see cref="StoredProducerVisitReport"/>s.
        /// </summary>
        /// <returns>A <see cref="List<ReportListItem>"/> sorted in descending order by VisitDate.</returns>
        /// <remarks>See <see cref="VisitController.Recent()"/>.</remarks>
        List<ReportListItem> Recent();

        /// <summary>Given a <see cref="ProducerVisitReport"/>, adds a 
        /// <see cref="StoredProducerVisitReport"/> (and <see cref="VisitXReason"/>(s)) to the SQLite database.
        /// </summary>
        /// <param name="report">A new <see cref="ProducerVisitReport"/>.</param>
        void Insert(ProducerVisitReport report);

        /// <summary>Opens the SQLite database, gets a <see cref="StoredProducerVisitReport"/> (and <see cref="ReasonCode"/>), 
        /// and 
        /// </summary>
        /// <param name="id">The internal ID number of a <see cref="StoredProducerVisitReport"/>.</param>
        /// <returns>A <see cref="ProducerVisitReport"/>.</returns>
        ProducerVisitReport GetReport(int id);

        /// <summary>The number of <see cref="StoredProducerVisitReport"/> records in the SQLite database.
        /// </summary>
        int Count { get; }

        /// <summary>Marks the "uploaded" flag for a given <see cref="StoredProducerVisitReport"/> in the SQLite database.
        /// </summary>
        /// <param name="id">The internal ID number of <see cref="StoredProducerVisitReport"/>.</param>
        void ReportUploaded(int id);

        /// <summary>Opens the SQLite database, gets the <see cref="ReasonCodes"/>.
        /// </summary>
        /// <returns>A <see cref="List<>"/> of <see cref="ReasonCodes"/>.</returns>
        List<ReasonCode> GetReasonsForCall();

        /// <summary>Opens the SQLite database, replaces the "ReasonCodes" table with <paramref name="reasonCodes"/>.
        /// </summary>
        /// <param name="reasonCodes">A <see cref="List"/> of new <see cref="ReasonCodes"/>.</param>
        void UpdateReasons(List<ReasonCode> reasonCodes);
    }
}