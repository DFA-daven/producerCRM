namespace CallForm.Core.Models
{
    using System;

    /// <summary>Creates an object representing a report of a visit to a member.
    /// </summary>
    /// <remarks>A ProducerVisitReport object is equivalent to a <see cref="StoredProducerVisitReport"/> with the associated
    /// <see cref="ReasonCode"/> list. It may represent a visit that exists on the web service, or a new visit.</remarks>
    public class ProducerVisitReport
    {
        /// <summary>The internal ID for this object.
        /// </summary>
        public int ID { get; set; }

        // FixMe: change userID to DeviceID

        /// <summary>The device ID associated with this visit.
        /// </summary>
        public string UserID { get; set; }

        /// <summary>The 8 digit member number.
        /// </summary>
        public string MemberNumber { get; set; }

        /// <summary>Decimal latitude.
        /// </summary>
        public double Lat { get; set; }

        /// <summary>Decimal longitude.
        /// </summary>
        public double Lng { get; set; }

        /// <summary>The date the visit took place.
        /// </summary>
        public DateTime VisitDate { get; set; }

        /// <summary>The length of the visit.
        /// </summary>
        public decimal Duration { get; set; }

        /// <summary>The time-stamp of this Visit Report.
        /// </summary>
        public DateTime EntryDateTime { get; set; }

        /// <summary>The <see cref="CallType"/> for this visit.
        /// </summary>
        public string CallType { get; set; }

        /// <summary>Text notes associated with this visit.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>The email recipient(s) notified about this visit.
        /// </summary>
        public string EmailRecipients { get; set; }

        /// <summary>A picture associated with this visit.
        /// </summary>
        public byte[] PictureBytes { get; set; }

        /// <summary>The Reason Code(s) for this visit.
        /// </summary>
        public ReasonCode[] ReasonCodes { get; set; }
    }
}
