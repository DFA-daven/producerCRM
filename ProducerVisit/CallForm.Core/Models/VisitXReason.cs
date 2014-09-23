namespace CallForm.Core.Models
{
    using Cirrious.MvvmCross.Plugins.Sqlite;

    /// <summary>An object cross-referencing a <see cref="StoredProducerVisitReport"/> with a <see cref="ReasonCode"/>.
    /// </summary>
    /// <remarks>This is a cross-reference linking a specific visit to a reason code(s). 
    /// Design goal is to limit this class to only deal with the raw data.</remarks>
    public class VisitXReason
    {
        /// <summary>The internal ID for this object.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        /// <summary>A <see cref="ReasonCode.ID">ReasonCode.ID</see>.
        /// </summary>
        /// <remarks><see cref="VisitXReason.ReasonID">VisitXReason.ReasonID</see> == <see cref="ReasonCode.ID">ReasonCode.ID</see></remarks>
        public int ReasonID { get; set; }

        /// <summary>A <see cref="StoredProducerVisitReport.ID">StoredProducerVisitReport.ID</see>.
        /// </summary>
        /// <remarks><see cref="VisitXReason.VisitID">VisitXReason.VisitID</see> == <see cref="StoredProducerVisitReport.ID">StoredProducerVisitReport.ID</see></remarks>
        public int VisitID { get; set; }
    }
}
