using System;
using Cirrious.MvvmCross.Plugins.Sqlite;

namespace CallForm.Core.Models
{
    public class StoredProducerVisitReport
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string UserID { get; set; }
        public string FarmNumber { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public DateTime VisitDate { get; set; }
        public decimal Duration { get; set; }
        public DateTime EntryDateTime { get; set; }
        public string CallType { get; set; }
        public string Notes { get; set; }
        public string EmailRecipients { get; set; }
        public byte[] PictureBytes { get; set; }
        public bool Uploaded { get; set; }

        public StoredProducerVisitReport()
        {}

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