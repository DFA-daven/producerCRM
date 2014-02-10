using System;

namespace CallForm.Core.Models
{
    public class ReportListItem
    {
        public int ID { get; set; }
        public string UserEmail { get; set; }
        public string FarmNumber { get; set; }
        public DateTime VisitDate { get; set; }
        public ReasonCode PrimaryReasonCode { get; set; }
        public bool Uploaded { get; set; }
        public bool Local { get; set; }
    }
}