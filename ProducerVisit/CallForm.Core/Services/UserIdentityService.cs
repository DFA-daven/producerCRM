using System;
using CallForm.Core.Models;
using Cirrious.MvvmCross.Plugins.File;
using Cirrious.MvvmCross.Plugins.Network.Rest;
using CallForm.Core.ViewModels;

namespace CallForm.Core.Services
{
    /// <summary>Implements the <see cref="IUserIdentityService"/> interface.
    /// </summary>
    public class UserIdentityService : IUserIdentityService
    {
        /// <summary>An instance of the <see cref="IMvxFileStore"/>.
        /// </summary>
        private readonly IMvxFileStore _fileStore;

        /// <summary>An instance of the <see cref="IMvxRestClient"/>.
        /// </summary>
        private readonly IMvxRestClient _restClient;

        //private readonly string _targetURL;

        //private static string _targetURL = "http://dl-backend-02.azurewebsites.net";
        private static string _targetURL = "http://dl-websvcs-test.dairydata.local:480";
        //private static string _targetURL = "http://ProducerCRM.DairyDataProcessing.com";


        /// <summary>Provides access to the <paramref name="fileStore"/> and <paramref name="restClient"/>.
        /// </summary>
        /// <param name="fileStore">The target <see cref="IMvxFileStore"/></param>
        /// <param name="restClient">The target <see cref="IMvxRestClient"/></param>
        public UserIdentityService(IMvxFileStore fileStore, IMvxRestClient restClient)
        {
            // FixMe: this seems to be the first method that requires a data connection
            _fileStore = fileStore;
            _restClient = restClient;
        }

        #region Required Definitions
        /// <inheritdoc/>
        public bool IdentityRecorded
        {
            get
            {
                bool foundIdentityFile = false;

                try
                {
                    // FixMe: this should be using Documents .. Library as the location 
                    // note: the current top level directory location may be breaking app signature, and risks not being copied btwn app versions.

                    // create folder if not found
                    _fileStore.EnsureFolderExists("Data"); 

                    var filename = _fileStore.PathCombine("Data", "Identity.xml");
                    foundIdentityFile = _fileStore.Exists(filename);
                }
                catch (Exception)
                {
                    // FixMe: Fail silently
                }

                return foundIdentityFile;
            }
        }

        /// <inheritdoc/>
        public void SaveIdentity(UserIdentity identity)
        {
            SaveIdentityToFile(identity);

            SaveIdentityToWebService(identity);
        }

        private void SaveIdentityToFile(UserIdentity identity)
        {
            try
            {
                if (!IdentityRecorded)
                {
                    _fileStore.EnsureFolderExists("Data");
                    var filename = _fileStore.PathCombine("Data", "Identity.xml");
                    _fileStore.WriteFile(filename, SemiStaticWebDataService.Serialize(identity));
                }
            }
            catch
            {
                // FixMe: just ignore any errors for now
                throw;
            }
        }

        private void SaveIdentityToWebService(UserIdentity identity)
        {
            try
            {
                var request =
                    new MvxJsonRestRequest<UserIdentity>(_targetURL + "/Visit/Identity/")
                    {
                        Body = identity
                    };

                // review: add error handling here
                _restClient.MakeRequest(request, (Action<MvxRestResponse>)ParseResponse, exception => { });
            }
            catch
            {
                // FixMe: just ignore any errors for now
                throw;
            }
        }
        
        /// <inheritdoc/>
        public UserIdentity GetSavedIdentity()
        {
            UserIdentity savedUser = new UserIdentity();
            savedUser.AssetTag = string.Empty;
            savedUser.UserEmail = "unknown";
            savedUser.DeviceID = "unknown";

            var filename = _fileStore.PathCombine("Data", "Identity.xml");
            string xml = string.Empty;
            if (_fileStore.Exists(filename)) 
            {
                if (_fileStore.TryReadTextFile(filename, out xml))
                {
                    savedUser = SemiStaticWebDataService.Deserialize<UserIdentity>(xml);
                }
            }
            else
            {
                SaveIdentity(savedUser);
            }

            return savedUser;
        }
        #endregion

        private void ParseResponse(MvxRestResponse obj)
        {
        }

        public event EventHandler<ErrorEventArgs> Error;
    }
}
