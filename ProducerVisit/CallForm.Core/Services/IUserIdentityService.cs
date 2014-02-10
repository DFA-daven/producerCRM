using CallForm.Core.Models;

namespace CallForm.Core.Services
{
    public interface IUserIdentityService
    {
        bool IdentityRecorded { get; }
        void SaveIdentity(UserIdentity identity);
        UserIdentity GetSavedIdentity();
    }
}