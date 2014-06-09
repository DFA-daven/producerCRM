namespace CallForm.Core.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using CallForm.Core.Models;
    using CallForm.Core.Services;
    using Cirrious.CrossCore.Platform;
    using Cirrious.MvvmCross.Plugins.Network.Rest;
    using Cirrious.MvvmCross.ViewModels;

    public class ViewReports_ViewModel : MvxViewModel
    {
        private readonly IMvxJsonRestClient _jsonRestClient;
        private readonly IMvxJsonConverter _jsonConverter;
        private readonly IDataService _dataService;
        private readonly IMvxRestClient _restClient;
        private readonly IUserIdentityService _userIdentityService;
        //private readonly string _targetURL;

        //private static string _targetURL = "http://dl-backend-02.azurewebsites.net";
        private static string _targetURL = "http://dl-websvcs-test.dairydata.local:480";
        //private static string _targetURL = "http://ProducerCRM.DairyDataProcessing.com";


        private MvxCommand _newVisitCommand;
        private string _filter;
        private List<ReportListItem> _reports;
        private MvxCommand _getReportsCommand;
        private bool _loading;
        private ReportListItem _selectedReport;
        private MvxCommand _viewReportCommand;

        /// <summary>Creates an instance of <see cref="ViewReports_ViewModel"/>.
        /// </summary>
        /// <param name="jsonRestClient"></param>
        /// <param name="jsonConverter"></param>
        /// <param name="dataService"></param>
        /// <param name="restClient"></param>
        /// <param name="webDataService"></param>
        /// <param name="userIdentityService"></param>
        public ViewReports_ViewModel(
            IMvxJsonRestClient jsonRestClient,
            IMvxJsonConverter jsonConverter,
            IDataService dataService,
            IMvxRestClient restClient,
            ISemiStaticWebDataService webDataService,
            IUserIdentityService userIdentityService)
        {
            //try
            //{
                _jsonRestClient = jsonRestClient;
                _jsonConverter = jsonConverter;
                _dataService = dataService;
                _restClient = restClient;

                _userIdentityService = userIdentityService;
                userIdentityService.GetIdentity();

                // broken: this requires an existing UserIdentity -- but on the first start there is no UserIdentity...
                // Reports = _dataService.Recent();
                Loading = false;

                // Hack: commenting out the Update() seems to prevent the Airplane Mode error
                // broken: is it just too early to be updating???
                
                webDataService.Update();
            //}
            //catch (Exception exc)
            //{
            //    // FixMe: add proper error handling
            //    Error(this, new ErrorEventArgs { Message = exc.Message });
            //}
        }

        public event EventHandler<ErrorEventArgs> Error;

        public void UploadReports()
        {
            foreach (var producerVisitReport in _dataService.ToUpload().ToList())
            {
                // error: break this code.
                var request =
                    new MvxJsonRestRequest<ProducerVisitReport>(_targetURL + "/Visit/Log/")
                    {
                        Body = producerVisitReport,
                        Tag = producerVisitReport.ID.ToString()
                    };
                // note: example of handling the response/error with a call to a method.
                // make the request: if OK, pass the response to ParseResponse; else it's an error
                //_restClient.MakeRequest(request, (Action<MvxRestResponse>)ParseResponse, exception => { Error(this, new ErrorEventArgs { Message = exception.Message }); });
                _restClient.MakeRequest(request, (Action<MvxRestResponse>)ParseResponse, exception => {  });
            }
        }

        /// <summary>Runs before this view Overlays <see cref="UserIdentityViewModel"/> if no identity exists.
        /// </summary>
        /// <remarks>This model is the RegisterAppStart<> of App.cs.</remarks>
        public override void Start()
        {
            // note: CallForm.Core starts here!
            base.Start();

            // broken: commenting this out reveals there is another problem (in addition to UserIdentity)
            // review: does this always require a call to the Connection? (even if local data exists?)
            if (!_userIdentityService.IdentityRecorded)
            {
                // broken: is IdentityRecorded causing the failure?

                // Note: open the User Identity page (to capture the missing information)
                // ShowViewModel<UserIdentity_ViewModel>();
            }
        }

        private void ParseResponse(MvxRestResponse response)
        {
            _dataService.ReportUploaded(int.Parse(response.Tag));
        }

        public ICommand NewVisitCommand
        {
            get
            {
                _newVisitCommand = _newVisitCommand ?? new MvxCommand(DoNewVisitCommand);
                return _newVisitCommand;
            }
        }

        /// <summary>Open the <see cref="NewVisit_ViewModel"/> view.
        /// </summary>
        protected void DoNewVisitCommand()
        {
            ShowViewModel<NewVisit_ViewModel>(new NewVisitInit {MemberNumber = string.Empty});
        }

        public string Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                RaisePropertyChanged(() => Filter);
            }
        }

        public List<ReportListItem> Reports
        {
            get { return _reports; }
            set
            {
                _reports = value;
                RaisePropertyChanged(() => Reports);
            }
        }

        /// <summary>Gets the Reports
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
            int memberNumberFilter = 0;

            if (string.IsNullOrEmpty(Filter))       // is there something to search for?
            {
                Reports = _dataService.Recent();
                Loading = false;
            }
            else if (Int32.TryParse(Filter, out memberNumberFilter)) // is it a number?
            {
                if (Filter.Length != 8)
                {
                    Error(this, new ErrorEventArgs { Message = "Member Number must be eight characters"});
                }
                else
                {
                    Loading = true;
                    var request = new MvxRestRequest(_targetURL + "/Visit/Recent/" + Filter);
                    // note: example of handling the response/error in-line
                    _jsonRestClient.MakeRequestFor<List<ReportListItem>>(request,
                        response =>
                        {
                            Reports = response.Result;
                            Loading = false;
                        },
                        exception =>
                        {
                            Loading = false;
                            Error(this, new ErrorEventArgs {Message = exception.Message});
                        });
                }
            }
            else
            {
                // todo: add new service to check for member name
            }
        }


        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                RaisePropertyChanged(() => Loading);
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
                    ShowViewModel<NewVisit_ViewModel>(new NewVisitInit { ReportData = _jsonConverter.SerializeObject(_dataService.GetReport(_selectedReport.ID)) });
                }
                else
                {
                    Loading = true;
                    var request = new MvxRestRequest(_targetURL + "/Visit/Report/" + SelectedReport.ID);
                    _jsonRestClient.MakeRequestFor<ProducerVisitReport>(request,
                        response =>
                        {
                            ShowViewModel<NewVisit_ViewModel>(new NewVisitInit { ReportData = _jsonConverter.SerializeObject(response.Result) });
                            Loading = false;
                        },
                        exception =>
                        {
                            Loading = false;
                            Error(this, new ErrorEventArgs { Message = exception.Message });
                        });
                }
            }
        }
    }

    /// <summary>An instance of an error event.
    /// </summary>
    public class ErrorEventArgs : EventArgs
    {
        /// <summary>The message to display on the error pop-up.
        /// </summary>
        public string Message { get; set; }
    }
}
