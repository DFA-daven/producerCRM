using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using CallForm.Core.Models;
using CallForm.Core.Services;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.Plugins.Network.Rest;
using Cirrious.MvvmCross.ViewModels;

namespace CallForm.Core.ViewModels
{
    public class ViewFarmReportsViewModel : MvxViewModel
    {
        private readonly IMvxJsonRestClient _jsonRestClient;
        private readonly IMvxJsonConverter _jsonConverter;
        private readonly IDataService _dataService;
        private readonly IMvxRestClient _restClient;
        private readonly IUserIdentityService _userIdentityService;

        public ViewFarmReportsViewModel(
            IMvxJsonRestClient jsonRestClient,
            IMvxJsonConverter jsonConverter,
            IDataService dataService,
            IMvxRestClient restClient,
            ISemiStaticWebDataService webDataService,
            IUserIdentityService userIdentityService)
        {
            _jsonRestClient = jsonRestClient;
            _jsonConverter = jsonConverter;
            _dataService = dataService;
            _restClient = restClient;
            _userIdentityService = userIdentityService;
            Reports = _dataService.Recent();
            Loading = false;
            webDataService.Update();
        }

        public event EventHandler<ErrorEventArgs> Error;

        public void UploadReports()
        {
            foreach (var producerVisitReport in _dataService.ToUpload().ToList())
            {
                var request =
                    new MvxJsonRestRequest<ProducerVisitReport>("http://dairyleademo.azurewebsites.net/Visit/Log/")
                    {
                        Body = producerVisitReport,
                        Tag = producerVisitReport.ID.ToString()
                    };
                _restClient.MakeRequest(request, (Action<MvxRestResponse>) ParseResponse, exception => Error(this, new ErrorEventArgs {Message = exception.Message}));
            }
        }

        public override void Start()
        {
            base.Start();
            if (!_userIdentityService.IdentityRecorded)
            {
                ShowViewModel<UserIdentityViewModel>();
            }
        }

        private void ParseResponse(MvxRestResponse response)
        {
            _dataService.ReportUploaded(int.Parse(response.Tag));
        }

        private MvxCommand _newVisitCommand;

        public ICommand NewVisitCommand
        {
            get
            {
                _newVisitCommand = _newVisitCommand ?? new MvxCommand(DoNewVisitCommand);
                return _newVisitCommand;
            }
        }

        protected void DoNewVisitCommand()
        {
            ShowViewModel<NewVisitViewModel>(new NewVisitInit {FarmNumber = ""});
        }

        private string _filter;

        public string Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                RaisePropertyChanged(() => Filter);
            }
        }

        private List<ReportListItem> _reports;

        public List<ReportListItem> Reports
        {
            get { return _reports; }
            set
            {
                _reports = value;
                RaisePropertyChanged(() => Reports);
            }
        }

        private MvxCommand _getReportsCommand;

        public ICommand GetReportsCommand
        {
            get
            {
                _getReportsCommand = _getReportsCommand ?? new MvxCommand(DoGetReportsCommand);
                return _getReportsCommand;
            }
        }

        private void DoGetReportsCommand()
        {
            if (string.IsNullOrEmpty(Filter))
            {
                Reports = _dataService.Recent();
                Loading = false;
            }
            else if (Filter.Length != 8)
            {
                Error(this, new ErrorEventArgs { Message = "Farm Number must be eight characters"});
            }
            else
            {
                Loading = true;
                var request = new MvxRestRequest("http://dairyleademo.azurewebsites.net/Visit/Recent/" + Filter);
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

        private bool _loading;

        public bool Loading
        {
            get { return _loading; }
            set
            {
                _loading = value;
                RaisePropertyChanged(() => Loading);
            }
        }

        private ReportListItem _selectedReport;

        public ReportListItem SelectedReport
        {
            get { return _selectedReport; }
            set
            {
                _selectedReport = value;
                RaisePropertyChanged(() => SelectedReport);
            }
        }

        private MvxCommand _viewReportCommand;

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
                    ShowViewModel<NewVisitViewModel>(new NewVisitInit { ReportData = _jsonConverter.SerializeObject(_dataService.GetReport(_selectedReport.ID)) });
                }
                else
                {
                    Loading = true;
                    var request = new MvxRestRequest("http://dairyleademo.azurewebsites.net/Visit/Report/" + SelectedReport.ID);
                    _jsonRestClient.MakeRequestFor<ProducerVisitReport>(request,
                        response =>
                        {
                            ShowViewModel<NewVisitViewModel>(new NewVisitInit { ReportData = _jsonConverter.SerializeObject(response.Result) });
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

    public class ErrorEventArgs : EventArgs
    {
        public string Message { get; set; }
    }
}
