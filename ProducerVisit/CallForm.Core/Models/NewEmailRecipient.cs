namespace CallForm.Core.Models
{
    using System;
    using System.Net;

    /// <summary>An object representing an "EmailRecipient" record.
    /// </summary>
    public class EmailRecipient
    {
        /// <summary>The internal ID for this object.
        /// </summary>
        public int ID { get; set; }

        /// <summary>Gets/sets the e-mail address for this instance.
        /// </summary>
        public string Address { get; set; }

        /// <summary>Gets/sets the displayed name for this instance.
        /// </summary>
        public string DisplayName
        {
            get;
            set;
        }

        /// <summary>The DisplayName of this <see cref="EmailRecipient"/>.
        /// </summary>
        /// <returns>A <see cref="String"/> of the description.</returns>
        public override string ToString()
        {
            return DisplayName;
        }
    }
}
