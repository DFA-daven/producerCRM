using Cirrious.MvvmCross.Plugins.Sqlite;

namespace CallForm.Core.Models
{
    /// <summary>Creates an instance of <seealso cref="UserIdentity"/>.
    /// </summary>
    public class UserIdentity
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        public string DeviceID { get; set; }
        public string UserEmail { get; set; }
        public string AssetTag { get; set; }
    }
}
