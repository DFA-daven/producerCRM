using Cirrious.MvvmCross.Plugins.Sqlite;

namespace CallForm.Core.Models
{
    public class VisitXReason
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public int ReasonID { get; set; }
        public int VisitID { get; set; }
    }
}
