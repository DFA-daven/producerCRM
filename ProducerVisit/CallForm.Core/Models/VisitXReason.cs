namespace CallForm.Core.Models
{
    using Cirrious.MvvmCross.Plugins.Sqlite;

    /// <summary>An object cross-referencing a <see cref="StoredProducerVisitReport"/> with a <see cref="ReasonCode"/>.
    /// </summary>
    /// <remarks>This is a cross-reference linking a specific visit to a reason code(s).</remarks>
    public class VisitXReason
    {
        /// <summary>The internal ID for this object.
        /// </summary>
        /// <remarks>[VisitXReasons].[ID]</remarks>
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        /// <summary>A [ReasonCodes].[ID].
        /// </summary>
        /// <remarks>[VisitXReasons].[ReasonID] == [ReasonCodes].[ID]</remarks>
        public int ReasonID { get; set; }

        /// <summary>A [StoredProducerVisitReports].[ID].
        /// </summary>
        /// <remarks>[VisitXReasons].[VisitID] == [StoredProducerVisitReports].[ID]</remarks>
        public int VisitID { get; set; }
    }
}
