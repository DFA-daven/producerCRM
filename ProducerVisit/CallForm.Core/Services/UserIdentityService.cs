using System;
using System.Collections.Generic;
using CallForm.Core.Models;
using Cirrious.MvvmCross.Plugins.File;
using Cirrious.MvvmCross.Plugins.Network.Rest;
//using CallForm.Core.ViewModels;

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

        private readonly string _targetURL;

        /// <summary>Provides access to the <paramref name="fileStore"/> and <paramref name="restClient"/>.
        /// </summary>
        /// <param name="fileStore">The target <see cref="IMvxFileStore"/></param>
        /// <param name="restClient">The target <see cref="IMvxRestClient"/></param>
        public UserIdentityService(IMvxFileStore fileStore, IMvxRestClient restClient)
        {
            // fixme: this seems to be the first method that requires a data connection
            _fileStore = fileStore;
            _restClient = restClient;

            // Hack: update this to the current backend target
            _targetURL = "http://dl-backend-02.azurewebsites.net";
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

        /// <summary>Saves <paramref name="identity"/> in the web service, and in the "Identity.xml" file on device.
        /// </summary>
        /// <param name="identity">A <seealso cref="UserIdentity"/>.</param>
        public void SaveIdentity(UserIdentity identity)
        {
            var request =
                new MvxJsonRestRequest<UserIdentity>(_targetURL + "/Visit/Identity/")
                {
                    Body = identity
                };

            // review: add error handling here
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

        //public event EventHandler<ErrorEventArgs> Error;

    }
}
