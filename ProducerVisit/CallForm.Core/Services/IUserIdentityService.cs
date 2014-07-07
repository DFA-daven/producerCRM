using CallForm.Core.Models;

namespace CallForm.Core.Services
{
    /// <summary>An interface to the <see cref="UserIdentity"/>.
    /// </summary>
    /// <remarks>Signatures of methods, properties, events and/or indexers</remarks>
    public interface IUserIdentityService
    {
        /// <summary>Indicates if the XML file exists in the <see cref="Cirrious.MvvmCross.Plugins.File.IMvxFileStore"/>.
        /// </summary>
        /// <value>Was the User Identity recorded?</value>
        bool IdentityRecorded { get; set; }

        /// <summary>Updates <paramref name="identity"/> to the XML file on-device, and to the web service.
        /// </summary>
        /// <param name="identity">A <see cref="UserIdentity"/>.</param>
        void UpdateIdentity(UserIdentity identity);

        /// <summary>Get the <see cref="UserIdentity"/>.
        /// </summary>
        /// <returns>The on-device copy of <see cref="UserIdentity"/>.</returns>
        /// <returns></returns>
        UserIdentity GetIdentity();
    }
}