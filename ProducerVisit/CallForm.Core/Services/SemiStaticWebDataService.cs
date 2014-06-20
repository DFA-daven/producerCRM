namespace CallForm.Core.Services
{
    using CallForm.Core.Models;
    using CallForm.Core.ViewModels;
    using Cirrious.MvvmCross.Plugins.File;
    using Cirrious.MvvmCross.Plugins.Network.Rest;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;

    /// <summary>Implements the <see cref="ISemiStaticWebDataService"/> interface.
    /// </summary>
    public class SemiStaticWebDataService : ISemiStaticWebDataService
    {
        private readonly IMvxFileStore _fileStore;
        private readonly IMvxJsonRestClient _jsonRestClient;
        private readonly IDataService _localSQLiteDataService;
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
        private static string _callTypeFileName = "CallTypes.xml";
        private static string _emailRecipientFileName = "EmailRecipients.xml";
        private static string _reasonCodeFileName = "ReasonCodes.xml";

        /// <summary>Provides access to the <paramref name="fileStore"/>, <paramref name="jsonRestClient"/>, and <paramref name="localSQLiteDataService"/>.
        /// </summary>
        /// <param name="fileStore">The target <see cref="Cirrious.MvvmCross.Plugins.File.IMvxFileStore"/></param>
        /// <param name="jsonRestClient">The target <see cref="Cirrious.MvvmCross.Plugins.Network.Rest.IMvxJsonRestClient"/></param>
        /// <param name="localSQLiteDataService">The target <see cref="IDataService"/></param>
        public SemiStaticWebDataService(IMvxFileStore fileStore, IMvxJsonRestClient jsonRestClient, IDataService localSQLiteDataService)
        {
            _fileStore = fileStore;
            _jsonRestClient = jsonRestClient;
            _localSQLiteDataService = localSQLiteDataService;

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
        public List<string> GetCallTypesAsList()
        {
            // note: USE THIS METHOD AS A MODEL FOR THE OTHER PULL-DOWNS. DON'T DELETE THESE COMMENTS UNTIL THE OTHERS ARE RE-FACTORED.
            // note: The XML file will be missing (locally) on the first run.

            List<string> objectList = new List<string>(new[]
                {
                    "initialized"
                });
            //List<CallType> objectList = new List<CallType>(new[]
            //    {
            //        new CallType { Name= "initialized" }
            //    });

            objectList = _localSQLiteDataService.GetSQLiteCallTypes().Select(i => i.ToString()).ToList();

            string xml = string.Empty;
            var targetFilename = _fileStore.PathCombine(_dataFolderPathName, _callTypeFileName);

            CheckFolder(_dataFolderPathName);

            //if (!_fileStore.Exists(targetFilename))
            //{
            //    //objectList.Clear();
            //    //objectList.Add( "file doesn't exist."  );
            //    UpdateXmlCallTypes();
            //}

            //if (_fileStore.TryReadTextFile(targetFilename, out xml))
            //{
            //    objectList = Deserialize<List<string>>(xml);
            //}
            //else
            //{
            //    objectList.Clear();
            //    objectList.Add("Error reading file.");
            //}

            // double-check that we got some result
            int objectCount = objectList.Count();
            objectList.Add("count is " + objectCount);

            return objectList;
        }

        /// <inheritdoc/>
        public List<string> GetEmailRecipientsAsList()
        {
            List<string> objectList = _localSQLiteDataService.GetSQLiteEmailRecipients().Select(i => i.ToString()).ToList();

            // double-check that we got some result
            int objectCount = objectList.Count();
            objectList.Add("count is " + objectCount);

            return objectList;
        }

        /// <inheritdoc/>
        public List<ReasonCode> GetReasonCodes()
        {
            List<ReasonCode> objectList = _localSQLiteDataService.GetSQLiteReasonCodes();

            // double-check that we got some result
            int objectCount = objectList.Count();
            objectList.Add(new ReasonCode{ Name =  "count is " + objectCount});

            return objectList;
        }

        /// <inheritdoc/>
        public string GetEmailAddress(string emailName)
        {
            string result = "matching email name";
            return result;
        }

        /// <inheritdoc/>
        public string GetEmailDisplayName(string emailAddress)
        {
            string result = "matching email address";
            return result;
        }

        /// <inheritdoc/>
        public void UpdateModels()
        {
            //try
            //{
                CheckFolder(_dataFolderPathName);

                // FixMe: errors down at this level are not presented to the UI. add an error log?
                // review: how often are these tables going to be changing? do we really need to pull the fresh list every time?

                // UpdateEmailRecipients();
                
                UpdateCallTypes();
                //UpdateReasonCodes();
                //UpdateEmailRecipients();

                //success = true;
            //}
            //catch (Exception exc)
            //{
            //    //Error(this, new ErrorEventArgs { Message = exc.Message });
            //}
                // Note: methods in ISemiStaticWebDataService must return an object -- so this always returns true
                //return true;
        }
        #endregion

        private void UpdateCallTypes()
        {
            //UpdateSQLiteCallTypes();  // broken:
            UpdateXmlCallTypes();
        }

        private void UpdateEmailRecipients()
        {
            UpdateSQLiteEmailrecipients();
            //UpdateXmlEmailRecipients();
        }

        private void UpdateReasonCodes()
        {
            UpdateSQLiteReasonCodes();
            //UpdateXmlReasonCodes();
        }

        /// <summary>Request CallTypes from the web service, and save them on-device.
        /// </summary>
        /// <remarks>The "CallTypes" part of "/Visit/CallTypes/" is handled in BackEnd.Controllers.VisitController.cs.</remarks>
        private void UpdateXmlCallTypes()
        {
            // Hack: this is actually returning a List<CallType>
            var request = new MvxRestRequest(_targetURL + "/Visit/CallTypes/");

            _jsonRestClient.MakeRequestFor<List<string>>(request,
                response =>
                {
                    _fileStore.EnsureFolderExists(_dataFolderPathName);
                    var filename = _fileStore.PathCombine(_dataFolderPathName, _callTypeFileName);
                    _fileStore.WriteFile(filename, Serialize(response.Result));
                },
                exception => { });
        }

        /// <summary>Request Email Recipients from the web service, and save them on-device.
        /// </summary>
        /// <remarks>The "NewEmailRecipients" part of "/Visit/NewEmailRecipients/" is handled in BackEnd.Controllers.VisitController.cs.</remarks>
        private void UpdateXmlEmailRecipients()
        {
            // Hack: this is actually returning a List<EmailRecipient>
            var request = new MvxRestRequest(_targetURL + "/Visit/EmailRecipients/");

            _jsonRestClient.MakeRequestFor<List<string>>(request,
                response =>
                {
                    _fileStore.EnsureFolderExists(_dataFolderPathName);
                    var filename = _fileStore.PathCombine(_dataFolderPathName, _emailRecipientFileName);
                    _fileStore.WriteFile(filename, Serialize(response.Result));
                },
                exception => { });
        }

        /// <summary>Request (visit) Reasons from the web service, and save them on-device.
        /// </summary>
        /// <remarks>The "Reasons" part of "/Visit/Reasons/" is handled in BackEnd.Controllers.VisitController.cs</remarks>
        private void UpdateXmlReasonCodes()
        {
            var request = new MvxRestRequest(_targetURL + "/Visit/Reasons/");

            _jsonRestClient.MakeRequestFor<List<ReasonCode>>(request,
                response =>
                {
                    var filename = _fileStore.PathCombine(_dataFolderPathName, _reasonCodeFileName);
                    _fileStore.DeleteFile(filename);
                    _fileStore.WriteFile(filename, Serialize(response.Result));
                },
                exception => { });
        }

        private void UpdateSQLiteCallTypes()
        {
            var request = new MvxRestRequest(_targetURL + "/Visit/CallTypes/");

            _jsonRestClient.MakeRequestFor<List<CallType>>(request,
                response => { _localSQLiteDataService.UpdateSQLiteCallTypes(response.Result); },
                exception => { });
        }

        private void UpdateSQLiteEmailrecipients()
        {
            var request = new MvxRestRequest(_targetURL + "/Visit/EmailRecipients/");

            _jsonRestClient.MakeRequestFor<List<EmailRecipient>>(request,
                response => { _localSQLiteDataService.UpdateSQLiteEmailRecipients(response.Result);  },
                exception => { });
        }

        private void UpdateSQLiteReasonCodes()
        {
            var request = new MvxRestRequest(_targetURL + "/Visit/Reasons/");

            _jsonRestClient.MakeRequestFor<List<ReasonCode>>(request,
                response => { _localSQLiteDataService.UpdateSQLiteReasonCodes(response.Result); },
                exception => { });
        }


        ///// <inheritdoc/>
        //public List<NewEmailRecipient> GetEmailAddressesAndNames()
        //{
        //    NewEmailRecipient email1 = new NewEmailRecipient();
        //    NewEmailRecipient email2 = new NewEmailRecipient();
        //    email1.Address = "add1@dl";
        //    email1.DisplayName = "name one";
        //    email2.Address = "add2@dl";
        //    email2.DisplayName = "name two";

        //    // note: see GetCallTypes()
        //    List<NewEmailRecipient> emailRecipients = new List<NewEmailRecipient>(new[]
        //        {
        //            email1,
        //        });

        //    string xml = string.Empty;
        //    var callTypesFilename = _fileStore.PathCombine(_dataFolderPathName, _callTypeFileName);

        //    _fileStore.EnsureFolderExists(_dataFolderPathName);

        //    if (_fileStore.TryReadTextFile(callTypesFilename, out xml))
        //    {
        //        emailRecipients = Deserialize<List<NewEmailRecipient>>(xml);
        //        // List<int> reasonids = vxrs.Select(vxr => vxr.ReasonID).ToList();
        //        List<string> test = Serialize<List<NewEmailRecipient>>(xml).sel
        //    }
        //    else
        //    {
        //        // note: USE THIS AS A MODEL FOR THE OTHER PULL-DOWNS. DON'T DELETE THESE COMMENTS UNTIL THE OTHERS ARE RE-FACTORED.
        //        // note: This file will be missing (locally) on the first run.
        //        // todo: get this list from the web service.

        //        // todo: Move the default content to BackEnd. Only create if the database is being initially created (don't overwrite).
        //        emailRecipients = new List<NewEmailRecipient>(new[]
        //        {
        //            email2,
        //        });
        //    }

        //    return emailRecipients;
        //}

        /// <inheritdoc/>
        public List<string> GetEmailDisplayNames()
        {
            EmailRecipient email1 = new EmailRecipient();
            EmailRecipient email2 = new EmailRecipient();
            email1.Address = "add1@dl";
            email1.DisplayName = "name one";
            email2.Address = "add2@dl";
            email2.DisplayName = "name two";

            // note: see GetCallTypes()
            List<string> emailDisplayNames = new List<string>(new[]
                {
                    email1.ToString(),
                });

            string xml = string.Empty;
            var filename = _fileStore.PathCombine(_dataFolderPathName, _emailRecipientFileName);

            _fileStore.EnsureFolderExists(_dataFolderPathName);

            if (_fileStore.TryReadTextFile(filename, out xml))
            {
                List<EmailRecipient> emailRecipients = Deserialize<List<EmailRecipient>>(xml);
                //emailDisplayNames = List<string>
            }
            else
            {
                // note: USE THIS AS A MODEL FOR THE OTHER PULL-DOWNS. DON'T DELETE THESE COMMENTS UNTIL THE OTHERS ARE RE-FACTORED.
                // note: This file will be missing (locally) on the first run.
                // todo: get this list from the web service.

                // todo: Move the default content to BackEnd. Only create if the database is being initially created (don't overwrite).
                emailDisplayNames = new List<string>(new[]
                {
                    email2.ToString(),
                });
            }

            return emailDisplayNames;
        }

        private void CheckFolder(string folderPath)
        {
            _fileStore.EnsureFolderExists(folderPath);
        }

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
