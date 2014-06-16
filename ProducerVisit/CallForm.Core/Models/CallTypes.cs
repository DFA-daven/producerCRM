namespace CallForm.Core.Models
{
    using System;
    using System.Net;

    /// <summary>An object representing a visit Call Type.
    /// </summary>
    public class CallType
    {
        /// <summary>The internal ID for this object.
        /// </summary>
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
