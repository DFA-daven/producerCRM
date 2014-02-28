namespace CallForm.Core.Models
{
    using System;
    using Cirrious.MvvmCross.Plugins.Sqlite;

    /// <summary>Creates an instance of a <seealso cref="StoredProducerVisitReport"/>.
    /// </summary>
    public class StoredProducerVisitReport
    {
        /// <summary>The ID associated with this visit. Used by this database.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

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

        /// <summary>The timestamp of this Producer Visit Report.
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

        /// <summary>Has this <seealso cref="StoredProducerVisitReport"/> been uploaded?
        /// </summary>
        public bool Uploaded { get; set; }

        /// <summary>The class constructor.
        /// </summary>
        public StoredProducerVisitReport()
        {}

        /// <summary>Creates a <seealso cref="StoredProducerVisitReport"/> based on a <seealso cref="ProducerVisitReport"/>.
        /// </summary>
        /// <param name="pvr">The producer visit report.</param>
        public StoredProducerVisitReport(ProducerVisitReport pvr)
        {
            ID = pvr.ID;
            UserID = pvr.UserID;
            FarmNumber = pvr.FarmNumber;
            Lat = pvr.Lat;
            Lng = pvr.Lng;
            VisitDate = pvr.VisitDate;
            Duration = pvr.Duration;
            EntryDateTime = pvr.EntryDateTime;
            CallType = pvr.CallType;
            Notes = pvr.Notes;
            EmailRecipients = pvr.EmailRecipients;
            PictureBytes = pvr.PictureBytes;
            Uploaded = false;
        }

        /// <summary>Appends <seealso cref="ReasonCode"/>(s) to a <seealso cref="ProducerVisitReport"/>.
        /// </summary>
        /// <param name="reasonCodes"></param>
        /// <returns></returns>
        public ProducerVisitReport Hydrate(ReasonCode[] reasonCodes)
        {
            return new ProducerVisitReport
            {
                ID = ID,
                UserID = UserID,
                FarmNumber = FarmNumber,
                Lat = Lat,
                Lng = Lng,
                VisitDate = VisitDate,
                Duration = Duration,
                EntryDateTime = EntryDateTime,
                CallType = CallType,
                Notes = Notes,
                EmailRecipients = EmailRecipients,
                PictureBytes = PictureBytes,
                ReasonCodes = reasonCodes
            };
        }
    }
}