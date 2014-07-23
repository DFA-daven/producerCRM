namespace CallForm.Core.ViewModels
{
    using CallForm.Core.Models;
    using CallForm.Core.Services;
    using Cirrious.CrossCore;
    using Cirrious.CrossCore.Platform;
    using Cirrious.MvvmCross.Plugins.Messenger;
    using Cirrious.MvvmCross.Plugins.PictureChooser;
    using Cirrious.MvvmCross.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Input;

    /// <summary>Class definition of the New Visit ViewModel.
    /// </summary>
    /// <remarks>This is the page for collecting information about a new visit.</remarks>
    public class NewVisit_ViewModel : MvxViewModel
    {
        private readonly ILocationService _locationService;
        private readonly IMvxPictureChooserTask _pictureChooserTask;
        private readonly IDataService _localDatabaseService;
        private readonly IMvxJsonConverter _jsonConverter;

        #region backing fields
        /// <summary>Store for the latitude property.</summary>
        private double _lat;
        /// <summary>Store for the longitude property.</summary>
        private double _lng;
        /// <summary>Store for the selected <c>CallType</c> property.</summary>
        private string _selectedCallType;
        /// <summary>Store for the date property.</summary>
        private DateTime _date;
        /// <summary>Store for the visit duration property.</summary>
        private decimal _duration;
        /// <summary>Store for the (string) visit duration property.</summary>
        private string _durationString;
        /// <summary>Store for the <c>ActualTime</c> property.</summary>
        private DateTime _actualTime;
        /// <summary>Store for the member number property.</summary>
        private string _memberNumber;
        /// <summary>Store for the notes property.</summary>
        private string _notes;
        /// <summary>Store for the selected <c>ReasonCode</c> property.</summary>
        private List<ReasonCode> _selectReasonCodes;
        /// <summary>Store for the list of possible visit <c>CallType</c>s.</summary>
        private List<string> _callTypes;
        /// <summary>Store for the email addresses selected by the user</summary>
        private List<string> _emailAddresses;
        /// <summary>Store for the email display names selected by the user</summary>
        private List<string> _emailDisplayNames;
        /// <summary>Store for the <c>SaveCommand</c> property.</summary>
        private MvxCommand _saveCommand;
        
        // FixMe: re-factor userID to DeviceID
        private string _userID;

        /// <summary>Store for the <c>Height</c> property.</summary>
        private float _height;
        /// <summary>Store for the <c>Width</c> property.</summary>
        private float _width;
        /// <summary>Store for the <c>Editing</c> property.</summary>
        private bool _editing;
        /// <summary>Store for the <c>PictureBytes[]</c> property.</summary>
        private byte[] _pictureBytes;
        /// <summary>Store for the <c>TakePictureCommand</c> property.</summary>
        private MvxCommand _takePictureCommand;

        string _className = "CallForm.Core.ViewModels.NewVisit_ViewModel";
        #endregion

        /// <summary>Creates an instance of <c>NewVisit_ViewModel</c>.
        /// </summary>
        /// <param name="locationService"></param>
        /// <param name="messenger"></param>
        /// <param name="pictureChooserTask"></param>
        /// <param name="localDatabaseService"></param>
        /// <param name="jsonConverter"></param>
        /// <param name="semiStaticWebDataService"></param>
        public NewVisit_ViewModel(
            ILocationService locationService,
            IMvxMessenger messenger,
            IMvxPictureChooserTask pictureChooserTask,
            IDataService localDatabaseService,
            IMvxJsonConverter jsonConverter,
            ISemiStaticWebDataService semiStaticWebDataService)
        {
            ListOfReasonCodes = localDatabaseService.GetSQLiteReasonCodes();
            SelectedReasonCodes = new List<ReasonCode>();

            ListOfCallTypes = semiStaticWebDataService.GetCallTypesAsList();
            SelectedCallType = ListOfCallTypes.Contains("Farm Visit") 
                ? ListOfCallTypes[ListOfCallTypes.IndexOf("Farm Visit")] 
                : ListOfCallTypes.FirstOrDefault();

            ListOfEmailAddresses = semiStaticWebDataService.GetEmailAddressesAsList();
            SelectedEmailAddresses = new List<string>();
            ListOfEmailDisplayNames = semiStaticWebDataService.GetEmailDisplayNamesAsList();
            SelectedEmailDisplayNames = new List<string>();

            Date = DateTime.Now.Date;
            ActualTime = DateTime.Now;

            // review: is the location being obtained twice? And is the second time even if the user answered "don't allow"?
            _locationService = locationService;
            messenger.Subscribe<LocationMessage>(OnLocationMessage);
            GetInitialLocation();

            _pictureChooserTask = pictureChooserTask;
            _localDatabaseService = localDatabaseService;
            _jsonConverter = jsonConverter;

            Editing = true;

            
        }

        public void Init(NewVisitInit data)
        {
            // broken: not sure if this is Working correctly -- seems to crash app
            //Mvx.Trace(MvxTraceLevel.Diagnostic, "Init: Report Data", data.ReportData);
            if (string.IsNullOrEmpty(data.ReportData))
            {
                MemberNumber = data.MemberNumber;
                Editing = true;
                return;
            }
            var report = _jsonConverter.DeserializeObject<ProducerVisitReport>(data.ReportData);
            Editing = false;

            UserID = report.UserID;
            MemberNumber = report.MemberNumber;
            Lat = report.Lat;
            Lng = report.Lng;
            Date = report.VisitDate;
            Duration = report.Duration;
            DurationString = report.Duration.ToString("F2");
            ActualTime = report.EntryDateTime;
            SelectedCallType = report.CallType;
            SelectedReasonCodes = report.ReasonCodes.ToList();
            Notes = report.Notes;
            if (report.EmailRecipients == null)
            {
                SelectedEmailAddresses = new List<string>();
            }
            else
            {
                SelectedEmailAddresses =
                    new List<string>(report.EmailRecipients.Split(new[] {", "}, StringSplitOptions.RemoveEmptyEntries));
            }
            PictureBytes = (byte[]) (report.PictureBytes ?? new byte[0]).Clone();
        }

        //internal List<string> 

        private void GetInitialLocation()
        {
            //System.Console.WriteLine("Attempting to GetInitialLocation");
            CommonCore.DebugMessage(_className, "GetInitialLocation");
            CommonCore.DebugMessage(" > Attempting TryGetLatestLocation()");

            double lat, lng;
            if (_locationService.TryGetLatestLocation(out lat, out lng))
            {
                Lat = lat;
                Lng = lng;
            }
        }

        private void OnLocationMessage(LocationMessage locationMessage)
        {
            Lat = locationMessage.Lat;
            Lng = locationMessage.Lng;
        }

        // FixMe: re-factor userID to DeviceID
        public string UserID
        {
            get { return _userID; }
            set
            {
                _userID = value;
                RaisePropertyChanged(() => UserID);
            }
        }

        public float Height
        {
            get { return _height; }
            set
            {
                _height = value;
                RaisePropertyChanged(() => Height);
            }
        }

        public float Width
        {
            get { return _width; }
            set
            {
                _width = value;
                RaisePropertyChanged(() => Width);
            }
        }

        /// <summary>The latitude value for this visit.
        /// </summary>
        public double Lat
        {
            get { return _lat; }
            set
            {
                _lat = value;
                RaisePropertyChanged(() => Lat);
            }
        }

        /// <summary>The longitude value for this visit.
        /// </summary>
        public double Lng
        {
            get { return _lng; }
            set
            {
                _lng = value;
                RaisePropertyChanged(() => Lng);
            }
        }

        /// <summary>The date assigned to this visit.
        /// </summary>
        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                RaisePropertyChanged(() => Date);
            }
        }

        /// <summary>The (numeric) duration assigned to this visit.
        /// </summary>
        public decimal Duration
        {
            get { return _duration; }
            set
            {
                _duration = value;
                RaisePropertyChanged(() => Duration);
            }
        }

        /// <summary>The (string) duration assigned to this visit.
        /// </summary>
        public string DurationString
        {
            get { return _durationString; }
            set
            {
                _durationString = value;
                RaisePropertyChanged(() => DurationString);
            }
        }

        /// <summary>The time when this visit was created/updated.
        /// </summary>
        public DateTime ActualTime
        {
            get { return _actualTime; }
            set
            {
                _actualTime = value;
                RaisePropertyChanged(() => ActualTime);
            }
        }

        /// <summary>The member number associated with this visit.
        /// </summary>
        public string MemberNumber
        {
            get { return _memberNumber; }
            set
            {
                _memberNumber = value;
                RaisePropertyChanged(() => MemberNumber);
            }
        }

        /// <summary>Any user entered notes associated with this visit.
        /// </summary>
        public string Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                RaisePropertyChanged(() => Notes);
            }
        }

        /// <summary>A set of <see cref="ReasonCode"/>(s) assigned by the user to this Visit.
        /// </summary>
        public List<ReasonCode> SelectedReasonCodes
        {
            get { return _selectReasonCodes; }
            set
            {
                _selectReasonCodes = value;
                RaisePropertyChanged(() => SelectedReasonCodes);
            }
        }

        /// <summary>The list of <see cref="ReasonCode"/>s for the user to select from.
        /// </summary>
        /// <remarks><c>ListOfReasonCodes</c> is handled by <see cref="CallForm.iOS.ViewElements.ReasonCodePickerDialogViewController"/></remarks>
        public readonly List<ReasonCode> ListOfReasonCodes;

        /// <summary>The name of the visit <see cref="CallType"/> selected by the user.
        /// </summary>
        public string SelectedCallType
        {
            get { return _selectedCallType; }
            set
            {
                _selectedCallType = value;
                RaisePropertyChanged(() => SelectedCallType);
            }
        }

        /// <summary>The list of <see cref="CallType"/> names for the user to select from.
        /// </summary>
        /// <remarks><see cref="ListOfCallTypes"/> is handled by <see cref="CallForm.iOS.ViewElements.StringPickerDialog_ViewController"/></remarks>
        public List<string> ListOfCallTypes
        {
            get { return _callTypes; }
            set
            {
                _callTypes = value;
                RaisePropertyChanged(() => ListOfCallTypes);
            }
        }

        #region Save
        /// <summary>The action for Save button.
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

        private void DoSaveCommand()
        {
            // 
            if (MemberNumber == null || MemberNumber.Length != 8)
            {
                Error(this, new ErrorEventArgs {Message = "The Member Number must be eight characters long"});
            }
            else if (SelectedReasonCodes.Count <= 0)
            {
                Error(this, new ErrorEventArgs {Message = "You must select at least one Reason for Call."});
            }
            // FIRST, check if the string can be converted to a decimal
            else if (!decimal.TryParse(DurationString, out _duration))
            {
                Error(this, new ErrorEventArgs { Message = "Invalid Length of Call." });
            }
            // then, if it's <= 0
            else if (Duration <= 0)
            {
                Error(this, new ErrorEventArgs { Message = "You must enter a value for Length of Call." });
            }
            else if (Editing)
            {
                _localDatabaseService.Insert(NewVisitAsProducerVisitReport());
                if (SelectedEmailAddresses == null || SelectedEmailAddresses.Count <= 0)
                {
                    Close(this);
                }
                else
                {
                    SendEmail(this, null);
                }
            }
            else
            {
                //Close(this);
                ShowViewModel<NewVisit_ViewModel>(new NewVisitInit { MemberNumber = MemberNumber });
            }
        }

        private ProducerVisitReport NewVisitAsProducerVisitReport()
        {
            return new ProducerVisitReport
            {
                UserID        = UserID,
                MemberNumber  = MemberNumber,
                Lat           = Lat,
                Lng           = Lng,
                VisitDate     = Date,
                Duration      = Duration,
                EntryDateTime = ActualTime,
                CallType      = SelectedCallType,
                ReasonCodes   = SelectedReasonCodes.ToArray(),
                Notes         = Notes,
                EmailRecipients = string.Join(", ", SelectedEmailAddresses),
                PictureBytes  = (byte[]) (PictureBytes ?? new byte[0]).Clone(),
            };
        }
        #endregion Save

        #region Handle Picture
        public byte[] PictureBytes
        {
            get { return _pictureBytes; }
            set
            {
                _pictureBytes = value;
                RaisePropertyChanged(() => PictureBytes);
            }
        }

        public ICommand TakePictureCommand
        {
            get
            {
                // "??" is the null-coalescing operator. It returns the left-hand operand if the operand is not null; otherwise it returns the right hand operand.
                _takePictureCommand = _takePictureCommand ?? new MvxCommand(DoTakePictureCommand);
                return _takePictureCommand;
            }
        }

        private void DoTakePictureCommand()
        {
            _pictureChooserTask.TakePicture(800, 95, OnPicture, () => { });
        }

        private void OnPicture(Stream stream)
        {
            var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            PictureBytes = memoryStream.ToArray();
        }
        #endregion

        #region Email
        /// <summary>The list of <see cref="EmailRecipient"/> Addresses for the user to select from.
        /// </summary>
        public List<string> SelectedEmailAddresses
        {
            get { return _emailAddresses; }
            set
            {
                _emailAddresses = value;
                RaisePropertyChanged(() => SelectedEmailAddresses);
            }
        }

        /// <summary>The list of <see cref="EmailRecipient"/> DisplayNames for the user to select from.
        /// </summary>
        public List<string> SelectedEmailDisplayNames
        {
            get { return _emailDisplayNames; }
            set
            {
                _emailDisplayNames = value;
                RaisePropertyChanged(() => SelectedEmailDisplayNames);
            }
        }

        /// <summary>Holds the "Address" of potential Email Recipients.
        /// </summary>
        public List<string> ListOfEmailAddresses;

        /// <summary>Holds the "DisplayName" of potential Email Recipients.
        /// </summary>
        public List<string> ListOfEmailDisplayNames;

        internal List<string> AddressToDisplayName (List<string> emailAddresses) 
        {
            List<string> displayNames = emailAddresses;

            return displayNames;
        }

        internal List<string> DisplayNameToAddress (List<string> displayNames) 
        {
            List<string> emailAddresses = displayNames;

            return emailAddresses;
        }

        public event EventHandler SendEmail;
        #endregion

        #region Page Admin
        public bool Editing
        {
            get { return _editing; }
            set
            {
                _editing = value;
                RaisePropertyChanged(() => Editing);
                RaisePropertyChanged(() => SaveButtonText);
                RaisePropertyChanged(() => Title);
            }
        }

        /// <summary>Label for <see cref="SaveCommand"/>.
        /// </summary>
        public string SaveButtonText
        {
            get { return Editing ? "Save" : "New Report for Member"; }
        }

        /// <summary>Value for this View's Title.
        /// </summary>
        public string Title
        {
            get { return Editing ? "New Contact Report" : "Contact Report"; }
        }

        /// <summary>An error event to communicate to the <c>View</c>.
        /// </summary>
        public event EventHandler<ErrorEventArgs> Error;
        #endregion
    }

    public class NewVisitInit
    {
        public string ReportData { get; set; }
        public string MemberNumber { get; set; }
    }
}
