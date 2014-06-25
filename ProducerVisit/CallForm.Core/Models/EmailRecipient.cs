namespace CallForm.Core.Models
{
    using Cirrious.MvvmCross.Plugins.Sqlite;
    using System;

    /// <summary>An object representing an "EmailRecipient" record.
    /// </summary>
    public class EmailRecipient
    {
        /// <summary>The internal ID for this object.
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }

        /// <summary>Gets/sets the e-mail address for this instance.
        /// </summary>
        public string Address { get; set; }

        /// <summary>Gets/sets the e-mail address for this instance.
        /// </summary>
        public string DisplayName 
        { 
            get; 
            set; 
        }

        /// <summary>The DisplayName of this <see cref="EmailRecipient"/>.
        /// </summary>
        /// <returns>A <see cref="String"/> of the description.</returns>
        /// <remarks>Including this Override simplifies processing this Class (Model).</remarks>
        public override string ToString()
        {
            return DisplayName;
        }
    }
}
