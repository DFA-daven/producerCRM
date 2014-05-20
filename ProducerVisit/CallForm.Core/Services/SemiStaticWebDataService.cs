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
        //  - debug/internal     "http://dl-websvcs-test.dairydata.local";

        // final config:
        //  - release/production "http://ProducerCRM.DairyDataProcessing.com";
        //  - beta/staging       "http://dl-backend.azurewebsites.net";
        //  - alpha/testing      "http://dl-backend-02.azurewebsites.net";
        //  - debug/internal     "http://dl-websvcs-test.dairydata.local";

        // others/not used:
        //    "http://dl-webserver-te.dairydata.local:480"; 
        //    "http://DL-WebSvcs-03:480";

        // Note: this value determines where the app will look for web services

#if (RELEASE)
        private static string _targetURL = "http://ProducerCRM.DairyDataProcessing.com"; 
#elif (BETA)
        private static string _targetURL = "http://ProducerCRM.DairyDataProcessing.com"; 
#elif (ALPHA)
        private static string _targetURL = "http://dl-backend-02.azurewebsites.net/";
#else
        private static string _targetURL = "http://dl-websvcs-test";
#endif

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
            _fileStore.EnsureFolderExists("Data");
            string xml = string.Empty;
            var callTypesFilename = _fileStore.PathCombine("Data", "CallTypes.xml");
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
        public List<string> GetPvrEmailRecipients()
        {
            _fileStore.EnsureFolderExists("Data");
            string xml = string.Empty;
            var emailsFilename = _fileStore.PathCombine("Data", "Emails.xml");
            if (_fileStore.Exists(emailsFilename) && _fileStore.TryReadTextFile(emailsFilename, out xml))
            {
                return Deserialize<List<string>>(xml);
            }

            // note: see GetCallTypes()
            else
            {
                return null; // hack
                return new List<string>(new[]
                {
                    "info@agri-maxfinancial.com",
                    "info@agri-servicesagency.com",
                    "communications@dairylea.com",
                    "FieldStaffNotification-DairyOne@DairyOne.com",
                    "FieldStaffNotification-DMS@dairylea.com",
                    "drms@dairylea.com",
                    "FieldStaffNotification-Eagle@dairylea.com",
                    "FieldStaffNotification-HR@dairylea.com",
                    "technicalsupport-brittonfield@dairylea.com",
                    "FieldStaffNotification-Membership@dairylea.com",
                    "FieldStaffNotification-Payroll@dairylea.com",
                    "Recipients Not Listed"
                });
            }
        }

        /// <inheritdoc/>
        public void Update()
        {
            try
            {
                // FixMe: errors down at this level are not presented to the UI. add an error log?
                // review: how often are these tables going to be changing? do we really need to pull the fresh list every time?
                // request Reason Codes from the web service, and save them on-device
                var request = new MvxRestRequest(_targetURL + "/Visit/Reasons/");
                _jsonRestClient.MakeRequestFor<List<ReasonCode>>(request,
                    response => 
                    {
                        _dataService.UpdateReasons(response.Result);
                        //_fileStore.EnsureFolderExists("Data");
                        //var filename = _fileStore.PathCombine("Data", "Reasons.xml");
                        //_fileStore.WriteFile(filename, Serialize(response.Result));
                    },
                    exception => { Error(this, new ErrorEventArgs { Message = exception.Message }); });

                // request Call Types from the web service, and save them on-device
                request = new MvxRestRequest(_targetURL + "/Visit/CallTypes/");
                // FixMe: this table doesn't exist on the web-service, so this is constantly creating an error 
                _jsonRestClient.MakeRequestFor<List<string>>(request,
                    response =>
                    {
                        _fileStore.EnsureFolderExists("Data");
                        var filename = _fileStore.PathCombine("Data", "CallTypes.xml");
                        _fileStore.WriteFile(filename, Serialize(response.Result));
                    },
                    exception => { Error(this, new ErrorEventArgs { Message = exception.Message }); });

                // request Email Recipients from the web service, and save them on-device
                request = new MvxRestRequest(_targetURL + "/Visit/pvrEmailRecipients/");
                // FixMe: this table doesn't exist on the web-service, so this is constantly creating an error 
                _jsonRestClient.MakeRequestFor<List<string>>(request,
                    response =>
                    {
                        _fileStore.EnsureFolderExists("Data");
                        var filename = _fileStore.PathCombine("Data", "Emails.xml");
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
