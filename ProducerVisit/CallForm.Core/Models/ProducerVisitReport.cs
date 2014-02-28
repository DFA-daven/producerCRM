namespace CallForm.Core.Models
{
    using System;

    /// <summary>Creates an instance of a <seealso cref="ProducerVisitReport"/>.
    /// </summary>
    public class ProducerVisitReport
    {
        /// <summary>The ID associated with this visit. Used by this database.
        /// </summary>
        public int ID { get; set; }

        // fixme: change userID to DeviceID

        /// <summary>The user ID associated with this visit. Used by this database.
        /// </summary>
        public string UserID { get; set; }

        /// <summary>The 8 digit farm number.
        /// </summary>
        public string FarmNumber { get; set; }

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

        /// <summary>The timestamp of this Visit Report.
        /// </summary>
        public DateTime EntryDateTime { get; set; }

        /// <summary>The Call Type for this visit.
        /// </summary>
        public string CallType { get; set; }

        /// <summary>Text notes associated with this visit.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>The email recipients notified about this visit.
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
