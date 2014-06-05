namespace CallForm.Core.Services
{
    using CallForm.Core.Models;
    using CallForm.Core.ViewModels;
    using Cirrious.MvvmCross.Plugins.File;
    using Cirrious.MvvmCross.Plugins.Network.Rest;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;

    /// <summary>Implements the <see cref="ISemiStaticWebDataService"/> interface.
    /// </summary>
    public class SemiStaticWebDataService : ISemiStaticWebDataService
    {
        private readonly IMvxFileStore _fileStore;
        private readonly IMvxJsonRestClient _jsonRestClient;
        private readonly IDataService _dataService;
        //private readonly string _targetURL;

        // hack: fix the _targetURL definitions to match web.*.config
        // temporary config:
        //  - release/production "http://ProducerCRM.DairyDataProcessing.com";
        //  - beta/staging       "http://ProducerCRM.DairyDataProcessing.com";
        //  - alpha/testing      "http://dl-backend-02.azurewebsites.net";
        //  - debug/internal     "http://dl-websvcs-test.dairydata.local:480";

        // final config:
        //  - release/production "http://ProducerCRM.DairyDataProcessing.com";
        //  - beta/staging       "http://dl-backend.azurewebsites.net";
        //  - alpha/testing      "http://dl-backend-02.azurewebsites.net";
        //  - debug/internal     "http://dl-websvcs-test.dairydata.local:480";

        // others/not used:
        //    "http://DL-WebSvcs-03:480";
        //    "http://dl-websvcs-03.dairydata.local:480"; 

        //    "http://dl-webserver-te.dairydata.local:480"; 
        //    "http://dl-WebServer-Te";
        //    "http://dl-WebSvcs-tes2";


        // Note: this value determines where the app will look for web services


        //private static string _targetURL = "http://dl-backend-02.azurewebsites.net";
        private static string _targetURL = "http://dl-websvcs-test.dairydata.local:480";
        //private static string _targetURL = "http://ProducerCRM.DairyDataProcessing.com";

        private static string _dataFolderPathName = "Data";
        private static string _reasonCodeFileName = "ReasonCodes.xml";
        private static string _callTypeFileName = "CallTypes.xml";
        private static string _emailRecipientFileName = "EmailRecipients.xml";

        /// <summary>Provides access to the <paramref name="fileStore"/>, <paramref name="jsonRestClient"/>, and <paramref name="dataService"/>.
        /// </summary>
        /// <param name="fileStore">The target <see cref="Cirrious.MvvmCross.Plugins.File.IMvxFileStore"/></param>
        /// <param name="jsonRestClient">The target <see cref="Cirrious.MvvmCross.Plugins.Network.Rest.IMvxJsonRestClient"/></param>
        /// <param name="dataService">The target <see cref="IDataService"/></param>
        public SemiStaticWebDataService(IMvxFileStore fileStore, IMvxJsonRestClient jsonRestClient, IDataService dataService)
        {
            _fileStore = fileStore;
            _jsonRestClient = jsonRestClient;
            _dataService = dataService;

            // DL-WebServer-Te: server 2003 Enterprise SP2, 1GB
            //_targetURL = "http://dl-webserver-te.dairydata.local:480"; 
            
            // DL-WebSvcs-Test: server 2008 R2 Enterprise SP1, 8GB
                //_targetURL = "http://dl-websvcs-test.dairydata.local"; 

            // DL-WebSvcs-03: server 2008 R2 Enterprise SP1, 8GB
            // INTERNAL
                //_targetURL = "http://DL-WebSvcs-03:480";
            // EXTERNAL
            //_targetURL = "http://ProducerCRM.DairyDataProcessing.com"; 

            // Azure production
                // _targetURL = "http://dl-backend.azurewebsites.net";
            // Azure development
                // _targetURL = "http://dl-backend-02.azurewebsites.net"; 


        }

        #region Required Definitions
        /// <inheritdoc/>
        public List<ReasonCode> GetReasonsForCall()
        {
            // note: see GetCallTypes()
            return _dataService.GetReasonsForCall();
        }

        /// <inheritdoc/>
        public List<string> GetCallTypes()
        {
            _fileStore.EnsureFolderExists(_dataFolderPathName);
            string xml = string.Empty;
            var callTypesFilename = _fileStore.PathCombine(_dataFolderPathName, _callTypeFileName);
            if (_fileStore.Exists(callTypesFilename) && _fileStore.TryReadTextFile(callTypesFilename, out xml))
            {
                return Deserialize<List<string>>(xml);
            }
            else
            {
                // note: USE THIS AS A MODEL FOR THE OTHER PULL-DOWNS. DON'T DELETE THESE COMMENTS UNTIL THE OTHERS ARE RE-FACTORED.
                // note: This file will be missing (locally) on the first run.
                // todo: get this list from the web service.

                // todo: Move the default content to BackEnd. Only create if the database is being initially created (don't overwrite).
                return new List<string>(new[]
                {
                    "SemiStatic",
                    "Phone Call",
                    "Email",
                    "Farm Visit",
                    "Farm Show",
                    "SMS (Text Msg.)",
                    "Other"
                });
            }
        }

        /// <inheritdoc/>
        public List<string> GetEmailNames()
        {
            //// note: see GetCallTypes()
            //return _dataService.GetPvrEmailRecipients();

            _fileStore.EnsureFolderExists(_dataFolderPathName);
            string xml = string.Empty;
            var emailsFilename = _fileStore.PathCombine(_dataFolderPathName, _emailRecipientFileName);
            if (_fileStore.Exists(emailsFilename) && _fileStore.TryReadTextFile(emailsFilename, out xml))
            {
                //return Deserialize<List<string>>(xml);

                // hack: only returning single column data for now
                // note: "Recipients Not Listed" is hard-coded in NewVisit_View
                return new List<string>(new[]
                {
                    "SemiStaticWDS, GetPvrEmailName(): file found",
                    "FieldStaffNotification-Payroll@dairylea.com",
                    "Recipients Not Listed"
                });
            }

            // note: see GetCallTypes()
            else
            {
                // hack: only returning single column data for now
                return new List<string>(new[]
                {
                    "SemiStaticWDS, GetPvrEmailName(): no file",
                });
            }
        }

        /// <inheritdoc/>
        public List<NewEmailRecipient> GetEmailAddressesAndNames()
        {
            // note: see GetCallTypes()
            return _dataService.GetEmailAddressesAndNames();

            //_fileStore.EnsureFolderExists(_dataFolderPathName);
            //string xml = string.Empty;
            //var emailsFilename = _fileStore.PathCombine(_dataFolderPathName, _emailRecipientFileName);
            //if (_fileStore.Exists(emailsFilename) && _fileStore.TryReadTextFile(emailsFilename, out xml))
            //{
            //    //return Deserialize<List<string>>(xml);

            //    // hack: only returning single column data for now
            //    return new List<string>(new[]
            //    {
            //        "SemiStaticWebDataService, GetPvrEmailRecipients(): file found",
            //        "FieldStaffNotification-Payroll@dairylea.com",
            //        "Recipients Not Listed"
            //    });
            //}

            //// note: see GetCallTypes()
            //else
            //{
            //    // hack: only returning single column data for now
            //    return new List<string>(new[]
            //    {
            //        "SemiStaticWebDataService, GetPvrEmailRecipients(): no file",
            //        "FieldStaffNotification-Payroll@dairylea.com",
            //        "Recipients Not Listed"
            //    });
            //}
        }

        /// <inheritdoc/>
        public void Update()
        {
            // note: the "Reasons" part of "/Visit/Reasons/" is handled in BackEnd.Controllers.VisitController.cs
            try
            {
                // FixMe: errors down at this level are not presented to the UI. add an error log?
                // review: how often are these tables going to be changing? do we really need to pull the fresh list every time?
                // request Reason Codes from the web service, and save them on-device
                var request = new MvxRestRequest(_targetURL + "/Visit/Reasons/");
                _jsonRestClient.MakeRequestFor<List<ReasonCode>>(request,
                    response => 
                    {
                       // _dataService.UpdateReasons(response.Result);
                        _fileStore.EnsureFolderExists(_dataFolderPathName);
                        var filename = _fileStore.PathCombine(_dataFolderPathName, _reasonCodeFileName);
                        _fileStore.WriteFile(filename, Serialize(response.Result));
                    },
                    exception => { Error(this, new ErrorEventArgs { Message = exception.Message }); });

                // request Call Types from the web service, and save them on-device
                request = new MvxRestRequest(_targetURL + "/Visit/CallTypes/");
                // FixMe: this table doesn't exist on the web-service, so this is constantly creating an error 
                _jsonRestClient.MakeRequestFor<List<string>>(request,
                    response =>
                    {
                        _fileStore.EnsureFolderExists(_dataFolderPathName);
                        var filename = _fileStore.PathCombine(_dataFolderPathName, _callTypeFileName);
                        _fileStore.WriteFile(filename, Serialize(response.Result));
                    },
                    exception => { Error(this, new ErrorEventArgs { Message = exception.Message }); });

                // request Email Recipients from the web service, and save them on-device
                //request = new MvxRestRequest(_targetURL + "/Visit/pvrEmailRecipients/");
                request = new MvxRestRequest(_targetURL + "/Visit/NewEmailRecipients/");

                // FixMe: this table doesn't exist on the web-service, so this is constantly creating an error 
                _jsonRestClient.MakeRequestFor<List<NewEmailRecipient>>(request,
                    response =>
                    {
                       // _dataService.UpdateRecipients(response.Result);
                        _fileStore.EnsureFolderExists(_dataFolderPathName);
                        var filename = _fileStore.PathCombine(_dataFolderPathName, _emailRecipientFileName);
                        _fileStore.WriteFile(filename, Serialize(response.Result));
                    },
                    exception => { Error(this, new ErrorEventArgs { Message = exception.Message }); });
            }
            catch (Exception exc)
            {
                Error(this, new ErrorEventArgs { Message = exc.Message });
            }
        }
        #endregion

        public event EventHandler<ErrorEventArgs> Error;

        /// <summary>Convert XML to an object of type <paramref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type to apply to the XML.</typeparam>
        /// <param name="xml">An XML ("serialized") string.</param>
        /// <returns>The <paramref name="xml"/> deserialized to an object of type <paramref name="T"/>.</returns>
        public static T Deserialize<T>(string xml)
        {
            var serializer = new XmlSerializer(typeof(T));
            T container;
            using (TextReader stream = new StringReader(xml))
            {
                container = (T)serializer.Deserialize(stream);
            }
            return container;
        }

        /// <summary>Convert an object of type <paramref name="T"/> to XML.
        /// </summary>
        /// <typeparam name="T">The type to apply to <paramref name="obj"/>.</typeparam>
        /// <param name="obj">An object that needs to be serialized.</param>
        /// <returns>An object "serialized" to XML.</returns>
        public static string Serialize<T>(T obj)
        {
            var serializer = new XmlSerializer(typeof(T));
            using (StringWriter stream = new StringWriter())
            {
                serializer.Serialize(stream, obj);
                return stream.ToString();
            }
        }
    }
}
