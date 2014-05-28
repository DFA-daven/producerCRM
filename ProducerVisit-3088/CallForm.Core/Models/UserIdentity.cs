using Cirrious.MvvmCross.Plugins.Sqlite;

namespace CallForm.Core.Models
{
    /// <summary>Creates an object representing a "UserIdentity" record.
    /// </summary>
    public class UserIdentity
    {
        /// <summary>The internal ID for this object.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        /// <summary>The ID of the device (hardware).
        /// </summary>
        public string DeviceID { get; set; }

        /// <summary>The user's email address.
        /// </summary>
        public string UserEmail { get; set; }

        /// <summary>Text entered by the user to identify this device.
        /// </summary>
        public string AssetTag { get; set; }
    }
}
