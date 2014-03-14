using System;

namespace CallForm.Core.Models
{
    /// <summary>An object representing a "ReasonCode" record.
    /// </summary>
    public class ReasonCode
    {
        /// <summary>The internal ID for this object.
        /// </summary>
        public int ID { get; set; }

        /// <summary>The description of the reason.
        /// </summary>
        public string Name { get; set; }

        /// <summary>The value associated with this <see cref="ReasonCode"/>.
        /// </summary>
        public int Code { get; set; }

        /// <summary>The description of this <see cref="ReasonCode"/>.
        /// </summary>
        /// <returns>A <see cref="String"/> of the description.</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
