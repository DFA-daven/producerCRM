using CallForm.Core.Models;

namespace CallForm.Core.Services
{
    /// <summary>An interface to the <seealso cref="UserIdentity"/>.
    /// </summary>
    public interface IUserIdentityService
    {
        bool IdentityRecorded { get; }
        void SaveIdentity(UserIdentity identity);

        UserIdentity GetSavedIdentity();
    }
}