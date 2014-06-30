namespace CallForm.Core.Models
{
    using Cirrious.MvvmCross.Plugins.Sqlite;
    using System;

    /// <summary>An object representing a "ReasonCode" record.
    /// </summary>
    public class ReasonCode
    {
        /// <summary>The internal ID for this object.
        /// </summary>
        /// <seealso cref="StoredProducerVisitReport.ID"/>
        /// <seealso cref="VisitXReason.ID"/>
        [PrimaryKey, AutoIncrement]
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
        /// <remarks>Including this Override simplifies processing this Class (Model).</remarks>
        public override string ToString()
        {
            return Name;
        }
    }
}
