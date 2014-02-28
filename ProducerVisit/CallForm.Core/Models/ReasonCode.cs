using System;

namespace CallForm.Core.Models
{
    /// <summary>Creates an instance of a visit <seealso cref="ReasonCode"/>.
    /// </summary>
    public class ReasonCode
    {
        /// <summary>The internal ID for this <seealso cref="ReasonCode"/>.
        /// </summary>
        public int ID { get; set; }

        /// <summary>The description of the reason for the visit.
        /// </summary>
        public string Name { get; set; }

        /// <summary>The value associated with this <seealso cref="ReasonCode"/>.
        /// </summary>
        public int Code { get; set; }

        /// <summary>The description of this <seealso cref="ReasonCode"/> as a <seealso cref="String"/>.
        /// </summary>
        /// <returns>The description of this <seealso cref="ReasonCode"/>.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
