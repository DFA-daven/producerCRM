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
        /// where "Uploaded" is false, and returns them as <seealso cref="List<ProducerVisitReport>"/>.
        /// </summary>
        /// <returns>A <seealso cref="List<ProducerVisitReport>"/> where "Uploaded" is false.
        List<ProducerVisitReport> ToUpload();

        /// <summary>Opens the SQLite database, gets the most recent <seealso cref="StoredProducerVisitReport"/>s.
        /// </summary>
        /// <returns>A <seealso cref="List<ReportListItem>"/> sorted in descending order by VisitDate.</returns>
        /// <remarks>See <seealso cref="VisitController.Recent()"/>.</remarks>
        List<ReportListItem> Recent();

        /// <summary>Opens the SQLite database, gets the <seealso cref="ReasonCodes"/>.
        /// </summary>
        /// <returns>A <seealso cref="List<>"/> of <seealso cref="ReasonCodes"/>.</returns>
        List<ReasonCode> GetReasonsForCall();

        /// <summary>Opens the SQLite database, replaces the "ReasonCodes" table with <paramref name="reasonCodes"/>.
        /// </summary>
        /// <param name="reasonCodes">A <seealso cref="List<>"/> of new <seealso cref="ReasonCodes"/>.</param>
        void UpdateReasons(List<ReasonCode> reasonCodes);

        /// <summary>Given a <seealso cref="ProducerVisitReport"/>, adds a 
        /// <seealso cref="StoredProducerVisitReport"/> (and <seealso cref="VisitXReason"/>(s)) to the SQLite database.
        /// </summary>
        /// <param name="report">A new <seealso cref="ProducerVisitReport"/>.</param>
        void Insert(ProducerVisitReport report);

        /// <summary>Opens the SQLite database, gets a <seealso cref="StoredProducerVisitReport"/> (and <seealso cref="ReasonCodes"/>), 
        /// and 
        /// </summary>
        /// <param name="id">The internal ID number of a <seealso cref="StoredProducerVisitReport"/>.</param>
        /// <returns>A <seealso cref="ProducerVisitReport"/>.</returns>
        ProducerVisitReport GetReport(int id);

        /// <summary>The number of <seealso cref="StoredProducerVisitReport"/> records in the SQLite database.
        /// </summary>
        int Count { get; }

        /// <summary>Marks the "uploaded" flag for a given <seealso cref="StoredProducerVisitReport"/> in the SQLite database.
        /// </summary>
        /// <param name="id">The internal ID number of <seealso cref="StoredProducerVisitReport"/>.</param>
        void ReportUploaded(int id);
    }
}