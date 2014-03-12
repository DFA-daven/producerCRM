using System.Collections.Generic;
using CallForm.Core.Models;

namespace CallForm.Core.Services
{
    /// <summary>Interface for infrequently changing data.
    /// </summary>
    /// <remarks>Signatures of methods, properties, events and/or indexers</remarks>
    public interface ISemiStaticWebDataService
    {
        /// <summary>Gets the <seealso cref="ReasonCodes"/> from the <seealso cref="IDataService"/>.
        /// </summary>
        /// <returns>A <seealso cref="List<>"/> of type <seealso cref="ReasonCodes"/>.</returns>
        List<ReasonCode> GetReasonsForCall();

        /// <summary>Gets the <seealso cref="List<String>"/> from the <seealso cref="IMvxFileStore"/>, or from a built-in list.
        /// </summary>
        /// <returns></returns>
        List<string> GetCallTypes();

        /// <summary>Gets a <seealso cref="List<String>"/> of "Email Recipients" from the <seealso cref="IMvxFileStore"/>, or from a built-in list.
        /// </summary>
        /// <returns></returns>
        List<string> GetEmailRecipients();

        /// <summary>Requests current copies of Reason Codes, Call Types, and Email Recipients from the web service, 
        /// and updates the tables stored in the data service (Reason Codes) and XML/file store (Call Types and Email Recipients).
        /// </summary>
        void Update();
    }
}