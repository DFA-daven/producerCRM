namespace CallForm.Core.Models
{
    using System;

    /// <summary>Creates an object representing the top-level information for a visit.
    /// </summary>
    /// <remarks>This object is used to populate the table views for the UI.</remarks>
    public class ReportListItem
    {
        /// <summary>The internal ID for this object.
        /// </summary>
        public int ID { get; set; }

        /// <summary>The email address of the user who created this entry.
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>The 8 digit member number.
        /// </summary>
        public string MemberNumber { get; set; }

        /// <summary>The date of the visit/contact with the member.
        /// </summary>
        public DateTime VisitDate { get; set; }

        // FixMe: this is getting producerVisitReport.ReasonCodes[0], which will be the first match in [VisitXReasons].[ReasonID]. This is not necessarily the "primary" reason.
        /// <summary>The primary reason for the visit.
        /// </summary>
        public ReasonCode PrimaryReasonCode { get; set; }

        /// <summary>Indicates if this object has been uploaded.
        /// </summary>
        public bool Uploaded { get; set; }

        /// <summary>Indicates if this record is local.
        /// </summary>
        public bool Local { get; set; }
    }
}