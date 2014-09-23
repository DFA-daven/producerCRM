namespace CallForm.Core.ViewModels
{
    using CallForm.Core.Models;
    using CallForm.Core.Services;
    using Cirrious.MvvmCross.ViewModels;
    using System;
    using System.Windows.Input;

    /// <summary>Class definition of the "User Identity" domain object.
    /// </summary>
    /// <remarks>Design goal is to limit this class to only deal with the raw data.</remarks>
    public class UserIdentity_ViewModel : MvxViewModel
    {
        #region backing fields
        private readonly IUserIdentityService _userIdentityService;
        private string _deviceID;
        private string _userEmail;
        private string _assetTag;
        #endregion

        /// <summary>Creates an instance of <see cref="UserIdentity_ViewModel"/>.
        /// </summary>
        /// <param name="userIdentityService"></param>
        public UserIdentity_ViewModel(IUserIdentityService userIdentityService)
        {
            _userIdentityService = userIdentityService;
        }

        /// <summary>The unique Id of the iOS device.
        /// </summary>
        public string DeviceID
        {
            get { return _deviceID; }
            set
            {
                _deviceID = value;
                RaisePropertyChanged(() => DeviceID);
            }
        }

        /// <summary>The user's email address.
        /// </summary>
        public string UserEmail
        {
            get { return _userEmail; }
            set
            {
                _userEmail = value;
                RaisePropertyChanged(() => UserEmail);
            }
        }

        /// <summary>The "Asset Tag" assigned to the iOS device.
        /// </summary>
        public string AssetTag
        {
            get { return _assetTag; }
            set
            {
                _assetTag = value;
                RaisePropertyChanged(() => AssetTag);
            }
        }

        public event EventHandler<ErrorEventArgs> Error;

        #region Save
        private MvxCommand _saveCommand;

        /// <summary>The action for the "OK" / Save button.
        /// </summary>
        public ICommand SaveCommand
        {
            get
            {
                // "??" is the null-coalescing operator. It returns the left-hand operand if the operand is not null; otherwise it returns the right hand operand.
                _saveCommand = _saveCommand ?? new MvxCommand(DoSaveCommand);
                return _saveCommand;
            }
        }

        /// <summary>This is the action of the "OK" button at the bottom of the UserIdentityView page.
        /// </summary>
        private void DoSaveCommand()
        {
            if (string.IsNullOrEmpty(UserEmail))
            {
                Error(this, new ErrorEventArgs { Message = "You must enter your email address" });
            }
            // ToDo: add check to validate email address
            else
            {
                try
                {
                    UserIdentity id = new UserIdentity
                    {
                        DeviceID = DeviceID,
                        UserEmail = UserEmail,
                        AssetTag = AssetTag ?? string.Empty
                    };

                    _userIdentityService.UpdateIdentity(id);

                    Close(this);
                }
                catch (Exception exc)
                {
                    Error(this, new ErrorEventArgs { Message = exc.Message + exc.InnerException });

                }
            }
        }
        #endregion
    }
}
