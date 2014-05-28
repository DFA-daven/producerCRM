using CallForm.Core.Models;

namespace CallForm.Core.Services
{
    /// <summary>An interface to the <see cref="UserIdentity"/>.
    /// </summary>
    /// <remarks>Signatures of methods, properties, events and/or indexers</remarks>
    public interface IUserIdentityService
    {
        /// <summary>Indicates if the "Identity.xml" file exists in the "Data" folder of the <see cref="IMvxFileStore"/>.
        /// </summary>
        bool IdentityRecorded { get; }

        /// <summary>Saves <paramref name="identity"/> to the "Identity.xml" file on-device, and to the web service.
        /// </summary>
        /// <param name="identity">A <see cref="UserIdentity"/>.</param>
        void SaveIdentity(UserIdentity identity);

        /// <summary>Get the <see cref="UserIdentity"/> from the XML file on-device.
        /// </summary>
        /// <returns>The on-device copy of <see cref="UserIdentity"/>.</returns>
        /// <returns></returns>
        UserIdentity GetSavedIdentity();
    }
}