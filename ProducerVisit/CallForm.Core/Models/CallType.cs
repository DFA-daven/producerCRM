namespace CallForm.Core.Models
{
    using Cirrious.MvvmCross.Plugins.Sqlite;
    using System;

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
        /// <remarks>Including this Override simplifies processing this Class (Model).</remarks>
        public override string ToString()
        {
            return Name;
        }
    }
}

