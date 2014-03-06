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
using System.Windows.Input;

namespace CallForm.Core.ViewModels
{
    public class NewVisitViewModel 
		: MvxViewModel
    {
        private readonly ILocationService _locationService;
        private readonly IMvxPictureChooserTask _pictureChooserTask;
        private readonly IDataService _dataService;
        private readonly IMvxJsonConverter _jsonConverter;

        // backing fields
        private double _lat;
        private double _lng;
        private string _callType;
        private DateTime _date;
        private decimal _duration;
        private string _durationString;
        private DateTime _actualTime;
        private string _farmNumber;
        private string _notes;
        private List<ReasonCode> _reasonCodes;
        private MvxCommand _saveCommand;

        // fixme: refactor userID to DeviceID
        private string _userID;
        private List<string> _callTypes;
        private bool _editing;
        private byte[] _pictureBytes;
        private MvxCommand _takePictureCommand;
        private List<string> _emailRecipients;

        public NewVisitViewModel(
            ILocationService locationService,
            IMvxMessenger messenger,
            IMvxPictureChooserTask pictureChooserTask,
            IDataService dataService,
            IMvxJsonConverter jsonConverter,
            ISemiStaticWebDataService webDataService)
        {
            // fixme: refactor "BuiltInReasonCodes" to indicate the source is webDataService.GetReasonsForCall()
            BuiltInReasonCodes = webDataService.GetReasonsForCall();
            ReasonCodes = new List<ReasonCode>();

            CallTypes = webDataService.GetCallTypes();
            CallType = CallTypes.First();

            BuiltinEmailRecipients = webDataService.GetEmailRecipients();

            Date = DateTime.Now.Date;
            ActualTime = DateTime.Now;

            _locationService = locationService;
            messenger.Subscribe<LocationMessage>(OnLocationMessage);
            GetInitialLocation();

            _pictureChooserTask = pictureChooserTask;
            _dataService = dataService;
            _jsonConverter = jsonConverter;

            Editing = true;

            _emailRecipients = new List<string>();
        }

        public void Init(NewVisitInit data)
        {
            // broken: not sure if this is working correctly -- seems to crash app
            Mvx.Trace(MvxTraceLevel.Diagnostic, "Init: Report Data", data.ReportData);
            if (string.IsNullOrEmpty(data.ReportData))
            {
                FarmNumber = data.FarmNumber;
                Editing = true;
                return;
            }
            var report = _jsonConverter.DeserializeObject<ProducerVisitReport>(data.ReportData);
            Editing = false;

            UserID = report.UserID;
            FarmNumber = report.FarmNumber;
            Lat = report.Lat;
            Lng = report.Lng;
            Date = report.VisitDate;
            Duration = report.Duration;
            DurationString = report.Duration.ToString("F2");
            ActualTime = report.EntryDateTime;
            CallType = report.CallType;
            ReasonCodes = report.ReasonCodes.ToList();
            Notes = report.Notes;
            if (report.EmailRecipients == null)
            {
                EmailRecipients = new List<string>();
            }
            else
            {
                EmailRecipients =
                    new List<string>(report.EmailRecipients.Split(new[] {", "}, StringSplitOptions.RemoveEmptyEntries));
            }
            PictureBytes = (byte[]) (report.PictureBytes ?? new byte[0]).Clone();
        }

        private void GetInitialLocation()
        {
            //System.Console.WriteLine("Attempting to GetInitialLocation");
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

        // fixme: refactor userID to DeviceID
        public string UserID
        {
            get { return _userID; }
            set
            {
                _userID = value;
                RaisePropertyChanged(() => UserID);
            }
        }

        public double Lat
        {
            get { return _lat; }
            set
            {
                _lat = value;
                RaisePropertyChanged(() => Lat);
            }
        }

        public double Lng
        {
            get { return _lng; }
            set
            {
                _lng = value;
                RaisePropertyChanged(() => Lng);
            }
        }

        public string CallType
        {
            get { return _callType; }
            set
            {
                _callType = value;
                RaisePropertyChanged(() => CallType);
            }
        }

        public List<string> CallTypes
        {
            get { return _callTypes; }
            set
            {
                _callTypes = value;
                RaisePropertyChanged(() => CallTypes);
            }
        }

        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                RaisePropertyChanged(() => Date);
            }
        }

        public decimal Duration
        {
            get { return _duration; }
            set
            {
                _duration = value;
                RaisePropertyChanged(() => Duration);
            }
        }

        public string DurationString
        {
            get { return _durationString; }
            set
            {
                _durationString = value;
                RaisePropertyChanged(() => DurationString);
            }
        }

        public DateTime ActualTime
        {
            get { return _actualTime; }
            set
            {
                _actualTime = value;
                RaisePropertyChanged(() => ActualTime);
            }
        }

        public string FarmNumber
        {
            get { return _farmNumber; }
            set
            {
                _farmNumber = value;
                RaisePropertyChanged(() => FarmNumber);
            }
        }

        public string Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                RaisePropertyChanged(() => Notes);
            }
        }

        public List<ReasonCode> ReasonCodes
        {
            get { return _reasonCodes; }
            set
            {
                _reasonCodes = value;
                RaisePropertyChanged(() => ReasonCodes);
            }
        }

        // review: what do BuiltInReasonCodes do?
        public readonly List<ReasonCode> BuiltInReasonCodes;

        #region Save
        // review: is this the "Save" buton?
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
            // 
            if (FarmNumber == null || FarmNumber.Length != 8)
            {
                Error(this, new ErrorEventArgs {Message = "The Member Number must be eight characters long"});
            }
            else if (ReasonCodes.Count <= 0)
            {
                Error(this, new ErrorEventArgs {Message = "You must select at least one Reason for Call."});
            }
            else if (Duration <= 0)
            {
                Error(this, new ErrorEventArgs { Message = "You must enter a value for Length of Call." });
            }
            else if (!decimal.TryParse(DurationString, out _duration))
            {
                Error(this, new ErrorEventArgs { Message = "Invalid Length of Call." });
            }
            
            else if (Editing)
            {
                _dataService.Insert(ToProducerVisitReport());
                if (EmailRecipients == null || EmailRecipients.Count <= 0)
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
                ShowViewModel<NewVisitViewModel>(new NewVisitInit { FarmNumber = FarmNumber });
            }
        }

        private ProducerVisitReport ToProducerVisitReport()
        {
            return new ProducerVisitReport
            {
                UserID = UserID,
                FarmNumber = FarmNumber,
                Lat = Lat,
                Lng = Lng,
                VisitDate = Date,
                Duration = Duration,
                EntryDateTime = ActualTime,
                CallType = CallType,
                ReasonCodes = ReasonCodes.ToArray(),
                Notes = Notes,
                EmailRecipients = string.Join(", ", EmailRecipients),
                PictureBytes = (byte[]) (PictureBytes ?? new byte[0]).Clone(),
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
        public List<string> EmailRecipients
        {
            get { return _emailRecipients; }
            set
            {
                _emailRecipients = value;
                RaisePropertyChanged(() => EmailRecipients);
            }
        }

        public List<string> BuiltinEmailRecipients;

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

        public string SaveButtonText
        {
            get { return Editing ? "Save" : "New Report for Member"; }
        }

        public string Title
        {
            get { return Editing ? "New Contact Report" : "Contact Report"; }
        }

        public event EventHandler<ErrorEventArgs> Error;
        #endregion
    }

    public class NewVisitInit
    {
        public string ReportData { get; set; }
        public string FarmNumber { get; set; }
    }
}
