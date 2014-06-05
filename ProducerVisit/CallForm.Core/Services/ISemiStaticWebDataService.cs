using System.Collections.Generic;
using CallForm.Core.Models;

namespace CallForm.Core.Services
{
    /// <summary>Interface for infrequently changing data.
    /// </summary>
    /// <remarks>Signatures of methods, properties, events and/or indexers</remarks>
    public interface ISemiStaticWebDataService
    {
        /// <summary>Gets the <see cref="ReasonCodes"/> from the <see cref="IDataService"/>.
        /// </summary>
        /// <returns>A <see cref="List"/> of type <see cref="ReasonCodes"/>.</returns>
        List<ReasonCode> GetReasonsForCall();

        /// <summary>Gets the <see cref="List"/> of <see cref="CallTypes"/> from the <see cref="IMvxFileStore"/>, or from a built-in list.
        /// </summary>
        /// <returns></returns>
        List<string> GetCallTypes();

        /// <summary>Gets a <see cref="List"/> of <see cref="string"/> from the <see cref="IMvxFileStore"/>, or from a built-in list.
        /// </summary>
        /// <returns></returns>
        List<string> GetEmailNames();

        /// <summary>Gets a <see cref="List"/> of <see cref="NewEmailRecipient"/> from the <see cref="IMvxFileStore"/>, or from a built-in list.
        /// </summary>
        /// <returns></returns>
        List<NewEmailRecipient> GetEmailAddressesAndNames();

        /// <summary>Requests current copies of <see cref="ReasonCodes"/>, <see cref="CallTypes"/>, and <see cref="pvrEmailRecipients"/> from the web service, 
        /// and updates the tables stored in the data service (Reason Codes) and XML/file store (Call Types and Email Recipients).
        /// </summary>
        void Update();
    }
}