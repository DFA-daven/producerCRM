using System;
using System.Collections.Generic;
using CallForm.Core.Models;
using Cirrious.MvvmCross.Plugins.File;
using Cirrious.MvvmCross.Plugins.Network.Rest;

namespace CallForm.Core.Services
{
    public class UserIdentityService : IUserIdentityService
    {
        private readonly IMvxFileStore _fileStore;
        private readonly IMvxRestClient _restClient;

        public UserIdentityService(IMvxFileStore fileStore, IMvxRestClient restClient)
        {
            _fileStore = fileStore;
            _restClient = restClient;
        }

        public bool IdentityRecorded
        {
            get
            {
                _fileStore.EnsureFolderExists("Data");
                var filename = _fileStore.PathCombine("Data", "Identity.xml");
                return _fileStore.Exists(filename);
            }
        }

        public void SaveIdentity(UserIdentity identity)
        {
            // TODO: update this to the current backend target
            var request =
                new MvxJsonRestRequest<UserIdentity>("http://dl-webserver-te.dairydata.local:480/Visit/Identity/")
                {
                    Body = identity
                };
            _restClient.MakeRequest(request, (Action<MvxRestResponse>) ParseResponse, exception => { });
            _fileStore.EnsureFolderExists("Data");
            var filename = _fileStore.PathCombine("Data", "Identity.xml");
            _fileStore.WriteFile(filename, SemiStaticWebDataService.Serialize(identity));
        }

        public UserIdentity GetSavedIdentity()
        {
            var filename = _fileStore.PathCombine("Data", "Identity.xml");
            string xml;
            if (_fileStore.Exists(filename) && _fileStore.TryReadTextFile(filename, out xml))
            {
                return SemiStaticWebDataService.Deserialize<UserIdentity>(xml);
            }
            return null;
        }

        private void ParseResponse(MvxRestResponse obj)
        {
        }
    }
}
