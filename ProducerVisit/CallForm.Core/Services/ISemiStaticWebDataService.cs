using System.Collections.Generic;
using CallForm.Core.Models;

namespace CallForm.Core.Services
{
    public interface ISemiStaticWebDataService
    {
        List<ReasonCode> GetReasonsForCall();
        List<string> GetCallTypes();
        List<string> GetEmailRecipients();
        void Update();
    }
}