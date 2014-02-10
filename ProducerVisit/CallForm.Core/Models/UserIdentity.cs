using Cirrious.MvvmCross.Plugins.Sqlite;

namespace CallForm.Core.Models
{
    public class UserIdentity
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string DeviceID { get; set; }
        public string UserEmail { get; set; }
        public string AssetTag { get; set; }
    }
}
