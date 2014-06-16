namespace CallForm.Core.Services
{
    using System.Collections.Generic;
    using CallForm.Core.Models;

    /// <summary>Interface for infrequently changing data.
    /// </summary>
    /// <remarks>Signatures of methods, properties, events and/or indexers</remarks>
    public interface ISemiStaticWebDataService
    {
        /// <summary>Gets the <see cref="CallType"/>(s) from the <see cref="Cirrious.MvvmCross.Plugins.File.IMvxFileStore"/>.
        /// </summary>
        /// <returns>A <see cref="System.List"/> of type <see cref="CallType"/>.</returns>
        List<string> GetCallTypes();

        /// <summary>Gets the <see cref="EmailRecipient"/>(s) from the <see cref="IMvxFileStore"/>.
        /// </summary>
        /// <returns>A <see cref="List"/> of type <see cref="EmailRecipient"/>.</returns>
        List<EmailRecipient> GetEmailRecipients();

        /// <summary>Gets the <see cref="ReasonCode"/>(s) from the <see cref="IMvxFileStore"/>.
        /// </summary>
        /// <returns>A <see cref="List"/> of type <see cref="ReasonCode"/>.</returns>
        /// <remarks><see cref="ReasonCode"/> are stored in both the SQLite database and XML.</remarks>
        List<ReasonCode> GetReasonCodes();

        /// <summary>Gets the Email Address.
        /// </summary>
        /// <returns>The Email Address for the given Display Name.</returns>
        string GetEmailAddress(string emailDisplayName);

        /// <summary>Gets the Display Name.
        /// </summary>
        /// <returns>The Display Name for the given email address.</returns>
        string GetEmailDisplayName(string emailAddress);

        /// <summary>Gets the <see cref="ReasonCodes"/>, <see cref="CallTypes"/>, and <see cref="EmailRecipients"/> tables from the web service, 
        /// and updates the tables stored in the data service (Reason Codes) and XML/file store (Call Types and Email Recipients).
        /// </summary>
        bool UpdateXml();
    }
}