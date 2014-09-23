using Cirrious.MvvmCross.Plugins.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml.Serialization;

namespace CallForm.Core.Models
{
    /// <summary>Creates an object representing a "UserIdentity" domain object.
    /// </summary>
    /// <remarks>Design goal is to limit this class to only deal with the raw data.</remarks>
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

        /// <summary>The UserEmail of this <see cref="UserIdentity"/>.
        /// </summary>
        /// <returns>A <see cref="String"/> of the description.</returns>
        public override string ToString()
        {
            return UserEmail;
        }
    }
}
