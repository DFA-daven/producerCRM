using System;
using System.Collections.Generic;
using CallForm.Core.Models;
using Cirrious.MvvmCross.Plugins.File;
using Cirrious.MvvmCross.Plugins.Network.Rest;

namespace CallForm.Core.Services
{
    public class UserIdentityService : IUserIdentityService
    {
        /// <summary>An instance of the <see cref="IMvxFileStore"/>.
        /// </summary>
        private readonly IMvxFileStore _fileStore;

        /// <summary>An instance of the <see cref="IMvxRestClient"/>.
        /// </summary>
        private readonly IMvxRestClient _restClient;

        /// <summary>Provides access to the <paramref name="IMvxFileStore"/> and <paramref name="IMvxRestClient"/>.
        /// </summary>
        /// <param name="fileStore">The target <see cref="IMvxFileStore"/></param>
        /// <param name="restClient">The target <see cref="IMvxRestClient"/></param>
        public UserIdentityService(IMvxFileStore fileStore, IMvxRestClient restClient)
        {
            _fileStore = fileStore;
            _restClient = restClient;
        }

        /// <summary>Indicates if the "Identity.xml" file exists in the "Data" folder of the <see cref="IMvxFileStore"/>.
        /// </summary>
        public bool IdentityRecorded
        {
            get
            {
                _fileStore.EnsureFolderExists("Data");
                var filename = _fileStore.PathCombine("Data", "Identity.xml");
                return _fileStore.Exists(filename);
            }
        }

        /// <summary>Saves <paramref name="identity"/> if the "Identity.xml" file exists in the "Data" folder of the <see cref="IMvxFileStore"/>.
        /// </summary>
        /// <param name="identity">A <seealso cref="UserIdentity"/>.</param>
        public void SaveIdentity(UserIdentity identity)
        {
            // TODO: update this to the current backend target
            var request =
                new MvxJsonRestRequest<UserIdentity>("http://dl-backend-02.azurewebsites.net/Visit/Identity/")
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
