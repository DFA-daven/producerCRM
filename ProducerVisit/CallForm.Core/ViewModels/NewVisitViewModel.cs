using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Input;
using System.Xml.Serialization;
using CallForm.Core.Models;
using CallForm.Core.Services;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.Plugins.File;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.MvvmCross.Plugins.PictureChooser;
using Cirrious.MvvmCross.ViewModels;

namespace CallForm.Core.ViewModels
{
    public class NewVisitViewModel 
		: MvxViewModel
    {
        private readonly ILocationService _locationService;
        private readonly IMvxPictureChooserTask _pictureChooserTask;
        private readonly IDataService _dataService;
        private readonly IMvxJsonConverter _jsonConverter;

        public NewVisitViewModel(
            ILocationService locationService,
            IMvxMessenger messenger,
            IMvxPictureChooserTask pictureChooserTask,
            IDataService dataService,
            IMvxJsonConverter jsonConverter,
            ISemiStaticWebDataService webDataService)
        {
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
            CallType = CallType;
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

        private string _userID;

        public string UserID
        {
            get { return _userID; }
            set
            {
                _userID = value;
                RaisePropertyChanged(() => UserID);
            }
        }

        private double _lat;

        public double Lat
        {
            get { return _lat; }
            set
            {
                _lat = value;
                RaisePropertyChanged(() => Lat);
            }
        }

        private double _lng;

        public double Lng
        {
            get { return _lng; }
            set
            {
                _lng = value;
                RaisePropertyChanged(() => Lng);
            }
        }

        private string _callType;

        public string CallType
        {
            get { return _callType; }
            set
            {
                _callType = value;
                RaisePropertyChanged(() => CallType);
            }
        }

        private List<string> _callTypes;

        public List<string> CallTypes
        {
            get { return _callTypes; }
            set
            {
                _callTypes = value;
                RaisePropertyChanged(() => CallTypes);
            }
        }

        private DateTime _date;

        public DateTime Date
        {
            get { return _date; }
            set
            {
                _date = value;
                RaisePropertyChanged(() => Date);
            }
        }

        private decimal _duration;

        public decimal Duration
        {
            get { return _duration; }
            set
            {
                _duration = value;
                RaisePropertyChanged(() => Duration);
            }
        }

        private string _durationString;

        public string DurationString
        {
            get { return _durationString; }
            set
            {
                _durationString = value;
                RaisePropertyChanged(() => DurationString);
            }
        }

        private DateTime _actualTime;

        public DateTime ActualTime
        {
            get { return _actualTime; }
            set
            {
                _actualTime = value;
                RaisePropertyChanged(() => ActualTime);
            }
        }

        private string _farmNumber;

        public string FarmNumber
        {
            get { return _farmNumber; }
            set
            {
                _farmNumber = value;
                RaisePropertyChanged(() => FarmNumber);
            }
        }

        private string _notes;

        public string Notes
        {
            get { return _notes; }
            set
            {
                _notes = value;
                RaisePropertyChanged(() => Notes);
            }
        }

        private List<ReasonCode> _reasonCodes;

        public List<ReasonCode> ReasonCodes
        {
            get { return _reasonCodes; }
            set
            {
                _reasonCodes = value;
                RaisePropertyChanged(() => ReasonCodes);
            }
        }

        public readonly List<ReasonCode> BuiltInReasonCodes;

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
            if (FarmNumber == null || FarmNumber.Length != 8)
            {
                Error(this, new ErrorEventArgs {Message = "The farm number must be eight characters long"});
            }
            else if (ReasonCodes.Count <= 0)
            {
                Error(this, new ErrorEventArgs {Message = "You must select a reason for the contact"});
            }
            else if (!decimal.TryParse(DurationString, out _duration))
            {
                Error(this, new ErrorEventArgs { Message = "Invalid Duration" });
            }
            else if (Duration <= 0)
            {
                Error(this, new ErrorEventArgs { Message = "You must enter a call duration" });
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

        private bool _editing;

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
            get { return Editing ? "Save" : "New Report for Producer"; }
        }

        public string Title
        {
            get { return Editing ? "New Contact Report" : "Contact Report"; }
        }

        private byte[] _pictureBytes;

        public byte[] PictureBytes
        {
            get { return _pictureBytes; }
            set
            {
                _pictureBytes = value;
                RaisePropertyChanged(() => PictureBytes);
            }
        }

        private MvxCommand _takePictureCommand;

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

        private List<string> _emailRecipients;

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

        public event EventHandler<ErrorEventArgs> Error;

        public event EventHandler SendEmail;
    }

    public class NewVisitInit
    {
        public string ReportData { get; set; }
        public string FarmNumber { get; set; }
    }
}
