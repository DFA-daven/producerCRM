namespace CallForm.Core.Services
{
    using CallForm.Core.Models;
    using CallForm.Core.ViewModels;
    using Cirrious.MvvmCross.Plugins.File;
    using Cirrious.MvvmCross.Plugins.Network.Rest;
    using System;
    using System.Diagnostics;

    /// <summary>Implements the <see cref="IUserIdentityService"/> interface.
    /// </summary>
    public class UserIdentityService : IUserIdentityService
    {
        /// <summary>An instance of the <see cref="IMvxFileStore"/>.
        /// </summary>
        private readonly IMvxFileStore _fileStore;

        /// <summary>Was the User Identity stored?</summary>
        private bool _identityRecorded = false;

        /// <summary>Was the User Identity uploaded?</summary>
        private bool _identityUploaded = false;

        private static string _dataFolderPathName = "Data";
        private static string _userIdentityFileName = "Identity.xml";
        private string _request;


        /// <summary>An instance of the <see cref="IMvxRestClient"/>.
        /// </summary>
        private readonly IMvxRestClient _restClient;

        //private readonly string _targetURL;

        //private static string _targetURL = "http://dl-backend-02.azurewebsites.net";
        //private static string _targetURL = "http://dl-websvcs-test.dairydata.local:480";
        private static string _targetURL = "http://DL-WebSvcs-03:480";
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
                return _identityRecorded;
            }
            set
            {
                _identityRecorded = value;
        }
        }

        /// <inheritdoc/>
        public UserIdentity GetIdentity()
        {
            UserIdentity savedUser = new UserIdentity();
            savedUser.AssetTag = "UserIdentitySvc GetIdentity() 1";

            // ToDo: get identify based on the device ID
            savedUser = GetXmlIdentity();

            return savedUser;
        }

        /// <inheritdoc/>
        public void UpdateIdentity(UserIdentity updatedIdentity)
        {
            SaveIdentityToFile(updatedIdentity);

            SaveIdentityToWebService(updatedIdentity);
        }
        #endregion

        private UserIdentity GetXmlIdentity()
        {
            UserIdentity savedUser = new UserIdentity();
            savedUser.AssetTag = "UserIdentitySvc GetXmlIdentity()";

            _fileStore.EnsureFolderExists(_dataFolderPathName);

            var userIdentityFilename = _fileStore.PathCombine(_dataFolderPathName, _userIdentityFileName);
            string xml = string.Empty;
            if (!_fileStore.Exists(userIdentityFilename))
            {
                CreateIdentity();
            }

            if (_fileStore.TryReadTextFile(userIdentityFilename, out xml))
            {
                savedUser = SemiStaticWebDataService.Deserialize<UserIdentity>(xml);
            }

            return savedUser;
        }

        /// <summary>Attempts to save the <paramref name="identity"/> to an XML file.
        /// </summary>
        /// <param name="identity">The <see cref="UserIdentity"/> to be recorded.</param>
        /// <remarks>Sets the "IdentityRecorded" flag.</remarks>
        private void SaveIdentityToFile(UserIdentity identity)
        {
            //identity.AssetTag = "UserIdentitySvc SaveIdentityToFile() 1";

            try
            {
                _fileStore.EnsureFolderExists(_dataFolderPathName);
                var filename = _fileStore.PathCombine(_dataFolderPathName, _userIdentityFileName);
                _fileStore.WriteFile(filename, SemiStaticWebDataService.Serialize<UserIdentity>(identity));
            }
            catch
            {
                // FixMe: just ignore any errors for now
                throw;
            }
            finally
            {
                IdentityRecorded = !string.IsNullOrWhiteSpace(identity.UserEmail);
            }
        }

        private void SaveIdentityToWebService(UserIdentity identity)
        {
            try
            {
                Request = _targetURL + "/Visit/Identity/";
                var request =
                    new MvxJsonRestRequest<UserIdentity>(Request)
                    {
                        Body = identity
                    };

                // review: is the "IdentityUploaded" flag getting set?
                _restClient.MakeRequest(request, (Action<MvxRestResponse>)ParseResponse, (Action<Exception>)RestException);
            }
            catch
            {
                // FixMe: just ignore any errors for now
                throw;
            }
        }

        private void RestException(Exception exception)
        {
            // TodO; this instance of RestException is for a single call -- it can be customized.
            Debug.WriteLine("Original request: " + Request);
            Debug.WriteLine("Exception message: " + exception.Message);
        }

        /// <summary>Creates an "empty" <see cref="UserIdentity"/>, and 
        /// </summary>
        /// <returns></returns>
        private UserIdentity CreateIdentity()
        {
            UserIdentity newUser = new UserIdentity();
            newUser.AssetTag = "UserIdentitySvc CreateIdentity() 1";
            newUser.DeviceID = newUser.UserEmail = " ";

            SaveIdentityToFile(newUser);

            return newUser;
        }
        
        public string Request
        {
            get { return _request; }
            set
            {
                _request = value;
            }
        }

        private void ParseResponse(MvxRestResponse obj)
        {
        }

        public event EventHandler<ErrorEventArgs> Error;
    }
}
