namespace CallForm.Core.ViewModels
{
    using CallForm.Core.Models;
    using CallForm.Core.Services;
    using Cirrious.CrossCore.Platform;
    using Cirrious.MvvmCross.Plugins.Network.Rest;
    using Cirrious.MvvmCross.ViewModels;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    //using System.Reflection;
    using System.Windows.Input;

    /// <summary>Class definition of the "View Reports" domain object.
    /// </summary>
    /// <remarks>Design goal is to limit this class to only deal with the raw data.</remarks>
    public class ViewReports_ViewModel : MvxViewModel
    {
        #region Properties
        private readonly IMvxJsonRestClient _jsonRestClient;
        private readonly IMvxJsonConverter _jsonConverter;
        private readonly IDataService _localSQLiteDataService;
        private readonly IMvxRestClient _restClient;
        private readonly IUserIdentityService _userIdentityService;
        private readonly ISemiStaticWebDataService _semiStaticWebDataService;

        //private readonly string _targetURL;
        private string _request;
		string _namespace = "CallForm.Core.ViewModels.";
		string _class = "ViewReports_ViewModel.";
		string _method = "TBD";

        //private static string _targetURL = "http://dl-backend-02.azurewebsites.net";
        //private static string _targetURL = "http://dl-websvcs-test.dairydata.local:480";
        private static string _targetURL = "http://dl-websvcs-03:480";
        //private static string _targetURL = "http://ProducerCRM.DairyDataProcessing.com";

        private MvxCommand _newVisitCommand;
        private string _filter;
        private List<ReportListItem> _reports;
        private MvxCommand _getReportsCommand;

        private ReportListItem _selectedReport;
        private MvxCommand _viewReportCommand;

        private bool _loading;
        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                RaisePropertyChanged(() => Loading);

                // Review: this is probably the best location for this -- too difficult to track changes when it's located in the web service 
                CommonCore.SetNetworkActivityIndicatorVisible(value);
            }
        }

        private bool _iOSVersionOK;
        public bool IOSVersionOK
        {
            get { return _iOSVersionOK; }
            set { _iOSVersionOK = value; }
        }

        ///// <summary>Store for the <c>RowHeight</c> property.</summary>
        //private float _rowHeight;
        //public float RowHeight
        //{
        //    get { return _rowHeight; }
        //    set
        //    {
        //        _rowHeight = value;
        //        RaisePropertyChanged(() => RowHeight);
        //    }
        //}

        
        #endregion

        /// <summary>Runs before this view Overlays <see cref="UserIdentity_ViewModel"/> if no identity exists.
        /// </summary>
        /// <remarks>This model is the <see cref="App.Initialize">RegisterAppStart</see> of <see cref="App"/>.</remarks>
        public override void Start()
        {
            _method = "Start";

            CommonCore.DebugMessage(_method, " > starting up.");

            Loading = true;

            // note: CallForm.Core starts here!
            // note: this creates a new instance of ViewReports_ViewModel.
            base.Start();

            //if (!IOSVersionOK)
            //{
            //    CommonCore.DebugMessage("##################### Halt app. ####################################################");
            //}

            if (string.IsNullOrWhiteSpace(_userIdentityService.GetIdentity().UserEmail) == true)
            {
                // open the UserIdentity_View (to capture the missing information)
                CommonCore.DebugMessage(_method, "  [vr_vm][start] > requesting to show <UserIdentity_ViewModel> ");

                ShowViewModel<UserIdentity_ViewModel>();
            }
            else
            {
				CommonCore.DebugMessage(_method, "  [vr_vm][start] > decided NOT to ShowViewModel<UserIdentity_ViewModel>() ");
            }

            //Loading = false;
        }


        /// <summary>Holds the REST request string.
        /// </summary>
        /// <remarks>This is mostly for debugging -- so if the request fails there's an easy way to see the contents.</remarks>
        public string Request
        {
            get { return _request; }
            set
            {
                _request = value;
            }
        }

        /// <summary>Creates an instance of <see cref="ViewReports_ViewModel"/>.
        /// </summary>
        /// <param name="jsonRestClient"></param>
        /// <param name="jsonConverter"></param>
        /// <param name="localDataService"></param>
        /// <param name="restClient"></param>
        /// <param name="semiStaticWebDataService"></param>
        /// <param name="userIdentityService"></param>
        public ViewReports_ViewModel(
            IMvxJsonRestClient jsonRestClient,
            IMvxJsonConverter jsonConverter,
            IDataService localDataService,
            IMvxRestClient restClient,
            ISemiStaticWebDataService semiStaticWebDataService,
            IUserIdentityService userIdentityService)
        {
            try
            {
                _jsonRestClient = jsonRestClient;
                _jsonConverter = jsonConverter;
                _localSQLiteDataService = localDataService;
                _restClient = restClient;
                _userIdentityService = userIdentityService;
                _semiStaticWebDataService = semiStaticWebDataService;

                // Review: Recent() must only query if UserIdentity is valid (that is, no query on first run). async/await?
                // Note: on first-run, there will be no UserIdentity. This method will ask try to get the user's email address, which will result in an empty UserIdentity being created.
                //Reports = localDataService.Recent();

                // Note: on first run GetIdentity() must instantiate a UserIdentity.
                userIdentityService.GetIdentity();

                //Loading = false;

                // Hack: update this to the current back-end target
                // _targetURL = "http://dl-websvcs-test.dairydata.local:480";
                _targetURL = "http://DL-WebSvcs-03:480";

                Loading = true;
                semiStaticWebDataService.UpdateModels();

                bool check = semiStaticWebDataService.FilesExist();
                
                Loading = false;
            }
            catch (Exception exc)
            {
				CommonCore.DebugMessage(_class, exc.Message);

                // FixMe: add proper error handling
                Error(this, new ErrorEventArgs { Message = exc.Message });
            }
        }



        #region Get report
        /// <summary>The value the user would like to search for.
        /// </summary>
        public string Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                RaisePropertyChanged(() => Filter);
            }
        }
        
        /// <summary>Gets the Reports Command.
        /// </summary>
        public ICommand GetReportsCommand
        {
            get
            {
                _getReportsCommand = _getReportsCommand ?? new MvxCommand(DoGetReportsCommand);
                return _getReportsCommand;
            }
        }

        /// <summary>Executes the query that retrieves the Reports (or if blank, the most recent result).
        /// </summary>
        private void DoGetReportsCommand()
        {
            Loading = true;

            int memberNumberFilter = 0;

            if (string.IsNullOrEmpty(Filter))       // is there something to search for?
            {
                Reports = _localSQLiteDataService.Recent();     // ...so this is a 'refresh'.
            }
            else if (Int32.TryParse(Filter, out memberNumberFilter)) // is it a number?
            {
                if (Filter.Length != 8)
                {
                    Loading = false;
                    Error(this, new ErrorEventArgs { Message = "Member Number must be eight characters"});
                }
                else
                {
                    Request = _targetURL + "/Visit/Recent/" + Filter;
                    var request = new MvxRestRequest(Request);
                    // note: example of handling the response/error in-line
                    _jsonRestClient.MakeRequestFor<List<ReportListItem>>(request,
                        response =>
                        {
                            Reports = response.Result;
                        },
                        (Action<Exception>)RestException);
                }
            }
            else
            {
                // todo: add new service to check for member name
                // we've got a string to use as a filter, but it's not a member number
                // make a call to the "other" web service, and request a short list (10?) of members
                // open an new (sub)view with the member's info
                // if a row (cell) is selected, query it
            }

            Loading = false;
        }
        #endregion

        #region New Visit
        public ICommand NewVisitCommand
        {
            get
            {
                //CommonCore.DebugMessage("  core[vr_vm][nvc] > Assigning _newVisitCommand... ");

                // Review: does this property need to be handled safely?
                _newVisitCommand = _newVisitCommand ?? new MvxCommand(DoNewVisitCommand);

                //CommonCore.DebugMessage("  core[vr_vm][nvc] > ..._newVisitCommand assigned: >" + _newVisitCommand.ToString() + "< ");

                return _newVisitCommand;
            }
        }

        /// <summary>Open the <see cref="NewVisit_ViewModel"/> view.
        /// </summary>
        protected void DoNewVisitCommand()
        {
            //CommonCore.DebugMessage("  core[vr_vm][dnvc] > About to ShowViewModel<NewVisit_ViewModel>()... ");
            ShowViewModel<NewVisit_ViewModel>(new NewVisitInit { MemberNumber = string.Empty });
        }

        /// <summary>Value for this View's Title.
        /// </summary>
        public string Title
        {
            get 
            {
                // Note: setting this to "Home" instructs the "Back" button on subordinate pages to display the word "Back". 
                return "Home";
            }
        }

        /// <summary>Uploads each <see cref="ProducerVisitReport">visit report</see> that is flagged 
        /// <see cref="DataService.ToUpLoad()">ToUpLoad()</see> in the on-device
        /// database to the web service.
        /// </summary>
        /// <remarks>This is called each time the <c>ViewReport</c>'s <c>ViewDidAppear()</c> event occurs.</remarks>
        public void UploadReports()
        {
            foreach (var producerVisitReport in _localSQLiteDataService.ToUpload().ToList())
            {
                Request = _targetURL + "/Visit/Log/";
                var request =
                    new MvxJsonRestRequest<ProducerVisitReport>(Request)
                    {
                        Body = producerVisitReport,
                        Tag = producerVisitReport.ID.ToString()
                    };
                // note: example of handling the response/error with a call to a method.
                // make the request: if OK, pass the response to ParseResponse; else it's an error
                //_restClient.MakeRequest(request, (Action<MvxRestResponse>)ParseResponse, exception => { Error(this, new ErrorEventArgs { Message = exception.Message }); });
                _restClient.MakeRequest(request, (Action<MvxRestResponse>)ParseResponse, (Action<Exception>)RestException);
            }
        }
        #endregion

        #region Report List
        /// <summary>The "visit reports" are listed in the <c>TableView</c>.
        /// </summary>
        public List<ReportListItem> Reports
        {
            get { return _reports; }
            set
            {
                _reports = value;
                RaisePropertyChanged(() => Reports);
            }
        }

        public ReportListItem SelectedReport
        {
            get { return _selectedReport; }
            set
            {
                _selectedReport = value;
                RaisePropertyChanged(() => SelectedReport);
            }
        }

        public ICommand ViewReportCommand
        {
            get
            {
                _viewReportCommand = _viewReportCommand ?? new MvxCommand(DoViewReportCommand);
                return _viewReportCommand;
            }
        }

        private void DoViewReportCommand()
        {
            if (_selectedReport != null)
            {
                if (SelectedReport.Local)
                {
                    ShowViewModel<NewVisit_ViewModel>(new NewVisitInit { ReportData = _jsonConverter.SerializeObject(_localSQLiteDataService.GetHydratedReport(_selectedReport.ID)) });
                }
                else
                {
                    Loading = true;
                    Request = _targetURL + "/Visit/Report/" + SelectedReport.ID;
                    var request = new MvxRestRequest(Request);
                    _jsonRestClient.MakeRequestFor<ProducerVisitReport>(request,
                        response =>
                        {
                            ShowViewModel<NewVisit_ViewModel>(new NewVisitInit { ReportData = _jsonConverter.SerializeObject(response.Result) });
                        },
                        (Action<Exception>)RestException);
                    Loading = false;
                }
            }
        }
        #endregion

        /// <summary>Parse the response from the web service for a <see cref="StoredProducerVisitReport"/> ID, 
        /// and mark the visit report as uploaded.
        /// </summary>
        /// <param name="response">The REST response from the web service.</param>
        private void ParseResponse(MvxRestResponse response)
        {
            _localSQLiteDataService.ReportUploaded(int.Parse(response.Tag));
        }

        private void RestException(Exception exception)
        {
			CommonCore.DebugMessage(_class, "RestExceptionMessage");
            CommonCore.DebugMessage("  core[vr_vm][re] > Original request: " + Request);
            CommonCore.DebugMessage("  core[vr_vm][re] > Exception message: " + exception.Message);
        }

        public event EventHandler<ErrorEventArgs> Error;
    }

    /// <summary>An error event to communicate to the <c>View</c>.
    /// </summary>
    public class ErrorEventArgs : EventArgs
    {
        /// <summary>The message to display on the error pop-up.
        /// </summary>
        public string Message { get; set; }
    }
}
