using Cirrious.MvvmCross.Plugins.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml.Serialization;



namespace CallForm.Core.Models
{

    /// <summary>An object representing a visit Call Type.
    /// </summary>
    public class CallType
    {
        /// <summary>The internal ID for this object.
        /// </summary>
        [PrimaryKey, AutoIncrement]      
        public int ID { get; set; }

        /// <summary>Gets/sets the "name" for this instance.
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>The name of this visit Call Type.
        /// </summary>
        /// <returns>A <see cref="String"/> of the description.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
