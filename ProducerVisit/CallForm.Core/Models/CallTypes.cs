namespace CallForm.Core.Models
{
    using System;
    using System.Net;

    /// <summary>An object representing an "EmailRecipient" record.
    /// </summary>
    public class CallTypes
    {
        /// <summary>The internal ID for this object.
        /// </summary>
        public int ID { get; set; }

        /// <summary>Gets/sets the "type" for this instance.
        /// </summary>
        public string CallType
        {
            get;
            set;
        }

        /// <summary>The DisplayName of this <see cref="NewEmailRecipient"/>.
        /// </summary>
        /// <returns>A <see cref="String"/> of the description.</returns>
        public override string ToString()
        {
            return CallType;
        }
    }
}
