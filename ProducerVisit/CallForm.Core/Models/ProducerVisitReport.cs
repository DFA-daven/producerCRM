using System;

namespace CallForm.Core.Models
{
    public class ProducerVisitReport
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public string FarmNumber { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public DateTime VisitDate { get; set; }
        public decimal Duration { get; set; }
        public DateTime EntryDateTime { get; set; }
        public string CallType { get; set; }
        public ReasonCode[] ReasonCodes { get; set; }
        public string Notes { get; set; }
        public string EmailRecipients { get; set; }
        public byte[] PictureBytes { get; set; }
    }
}
