﻿namespace CallForm.Core.Services
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
        /// <returns>A List&lt;<see cref="CallType"/>>.</returns>
        List<string> GetCallTypesAsList();

        /// <summary>Gets the "DisplayName" of the <see cref="EmailRecipient"/>(s) from the <see cref="Cirrious.MvvmCross.Plugins.File.IMvxFileStore"/>.
        /// </summary>
        /// <returns>A List&lt;<see cref="EmailRecipient"/>> "DisplayName".</returns>
        List<string> GetEmailDisplayNamesAsList();

        /// <summary>Gets the "Address" of the <see cref="EmailRecipient"/>(s) from the <see cref="Cirrious.MvvmCross.Plugins.File.IMvxFileStore"/>.
        /// </summary>
        /// <returns>A List&lt;<see cref="EmailRecipient"/>> "Address".</returns>
        List<string> GetEmailAddressesAsList();

        /// <summary>Gets the <see cref="ReasonCode"/>(s) from the <see cref="Cirrious.MvvmCross.Plugins.File.IMvxFileStore"/>.
        /// </summary>
        /// <returns>A List&lt;<see cref="ReasonCode"/>>.</returns>
        /// <remarks><see cref="ReasonCode"/> are stored in both the SQLite database and XML.</remarks>
        List<ReasonCode> GetReasonCodes();

        ///// <summary>Gets the Email Address.
        ///// </summary>
        ///// <returns>The Email Address for the given Display Name.</returns>
        //string GetEmailAddress(string emailDisplayName);

        ///// <summary>Gets the Display Name.
        ///// </summary>
        ///// <returns>The Display Name for the given email address.</returns>
        //string GetEmailDisplayName(string emailAddress);

        /// <summary>Gets the object (model) tables from the web service, and updates the SQLite an/or XML file as required.
        /// </summary>
        void UpdateModels();

        bool FilesExist();
    }
}