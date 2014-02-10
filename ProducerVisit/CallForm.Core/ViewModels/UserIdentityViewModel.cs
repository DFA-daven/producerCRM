using System;
using System.Windows.Input;
using CallForm.Core.Models;
using CallForm.Core.Services;
using Cirrious.MvvmCross.ViewModels;

namespace CallForm.Core.ViewModels
{
    public class UserIdentityViewModel : MvxViewModel
    {
        private readonly IUserIdentityService _userIdentityService;

        public UserIdentityViewModel(IUserIdentityService userIdentityService)
        {
            _userIdentityService = userIdentityService;
        }

        private string _deviceID;

        public string DeviceID
        {
            get { return _deviceID; }
            set
            {
                _deviceID = value;
                RaisePropertyChanged(() => DeviceID);
            }
        }

        private string _userEmail;

        public string UserEmail
        {
            get { return _userEmail; }
            set
            {
                _userEmail = value;
                RaisePropertyChanged(() => UserEmail);
            }
        }

        private string _assetTag;

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

        private MvxCommand _saveCommand;

        public ICommand SaveCommand
        {
            get
            {
                _saveCommand = _saveCommand ?? new MvxCommand(DoSaveCommand);
                return _saveCommand;
            }
        }

        private void DoSaveCommand()
        {
            if (string.IsNullOrEmpty(UserEmail))
            {
                Error(this, new ErrorEventArgs {Message = "You must enter your email address"});
            }
            else
            {
                UserIdentity id = new UserIdentity
                {
                    DeviceID = DeviceID,
                    UserEmail = UserEmail,
                    AssetTag = AssetTag ?? string.Empty
                };

                _userIdentityService.SaveIdentity(id);

                Close(this);
            }
        }
    }
}
