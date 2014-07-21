namespace CallForm.Core.Services
{
    using CallForm.Core.Models;
    using CallForm.Core.ViewModels;
    using Cirrious.MvvmCross.Plugins.File;
    using Cirrious.MvvmCross.Plugins.Network.Rest;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Reflection;
    using System.Xml.Serialization;

    /// <summary>Implements the <see cref="ISemiStaticWebDataService"/> interface.
    /// </summary>
    public class SemiStaticWebDataService : ISemiStaticWebDataService
    {
        private readonly IMvxFileStore _fileStore;
        private readonly IMvxJsonRestClient _jsonRestClient;
        private readonly IDataService _localDatabaseService;
        
        private string _request;
        string _className = "CallForm.Core.Services.SemiStaticWebDataService";

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
        //private static string _targetURL = "http://dl-websvcs-test.dairydata.local:480";
        private static string _targetURL = "http://dl-websvcs-03:480";
        //private static string _targetURL = "http://ProducerCRM.DairyDataProcessing.com";

        private static string _dataFolderPathName = "Data";
        private static string _callTypeFileName = "CallTypes.xml";
        private static string _emailRecipientFileName = "EmailRecipients.xml";
        private static string _reasonCodeFileName = "ReasonCodes.xml";

        public string Request
        {
            get { return _request; }
            set
            {
                _request = value;
            }
        }

        /// <summary>Provides access to the <paramref name="fileStore"/>, <paramref name="jsonRestClient"/>, and <paramref name="localSQLiteDataService"/>.
        /// </summary>
        /// <param name="fileStore">The target <see cref="Cirrious.MvvmCross.Plugins.File.IMvxFileStore"/></param>
        /// <param name="jsonRestClient">The target <see cref="Cirrious.MvvmCross.Plugins.Network.Rest.IMvxJsonRestClient"/></param>
        /// <param name="localSQLiteDataService">The target <see cref="IDataService"/></param>
        public SemiStaticWebDataService(IMvxFileStore fileStore, IMvxJsonRestClient jsonRestClient, IDataService localSQLiteDataService)
        {
            _fileStore = fileStore;
            _jsonRestClient = jsonRestClient;
            _localDatabaseService = localSQLiteDataService;
        }

        #region Required Definitions
        /// <inheritdoc/>
        public List<ReasonCode> GetReasonCodes()
        {
            return _localDatabaseService.GetSQLiteReasonCodes();
        }

        /// <inheritdoc/>
        public List<string> GetCallTypesAsList()
        {
            List<CallType> objectList = _localDatabaseService.GetSQLiteCallTypes(); 
            List<string> stringList = objectList.Select(i => i.ToString()).ToList();

            return stringList;
        }

        //public List<string> GetCallTypesAsList()
        //{
        //    List<string> stringList = new List<string>(new[] { "initialized" });

        //    CheckFolder(_dataFolderPathName);

        //    string xml = string.Empty;
        //    string targetFilename = _fileStore.PathCombine(_dataFolderPathName, _callTypeFileName);

        //    if (!_fileStore.Exists(targetFilename))
        //    {
        //        stringList.Clear();
        //        stringList.Add("file doesn't exist.");
        //        //UpdateXmlCallTypes();
        //    }

        //    if (_fileStore.TryReadTextFile(targetFilename, out xml))
        //    {
        //        List<CallType> objectList = Deserialize<List<CallType>>(xml);
        //        stringList = objectList.Select(i => i.ToString()).ToList();
        //    }
        //    else
        //    {
        //        stringList.Clear();
        //        stringList.Add("Error reading file.");
        //    }

        //    // double-check that we got some result
        //    int objectCount = stringList.Count();
        //    stringList.Add("count is " + objectCount);

        //    return stringList;
        //}

        /// <inheritdoc/>
        public List<string> GetEmailDisplayNamesAsList()
        {
            List<EmailRecipient> objectList = _localDatabaseService.GetSQLiteEmailRecipients();
            List<string> stringList = objectList.Select(i => i.DisplayName).ToList();

            return stringList;
        }

        //public List<string> GetEmailDisplayNamesAsList()
        //{
        //    List<string> stringList = new List<string>(new[] { "initialized" } );
                    
        //    CheckFolder(_dataFolderPathName);

        //    string xml = string.Empty;
        //    string targetFilename = _fileStore.PathCombine(_dataFolderPathName, _reasonCodeFileName);

        //    if (!_fileStore.Exists(targetFilename))
        //    {
        //        stringList.Clear();
        //        stringList.Add("file doesn't exist.");
        //    }

        //    if (_fileStore.TryReadTextFile(targetFilename, out xml))
        //    {
        //        List<EmailRecipient> objectList = Deserialize<List<EmailRecipient>>(xml);
        //        stringList = objectList.Select(i => i.ToString()).ToList();
        //    }
        //    else
        //    {
        //        stringList.Clear();
        //        stringList.Add("Error reading file.");
        //    }

        //    // double-check that we got some result
        //    int objectCount = stringList.Count();
        //    stringList.Add("count is " + objectCount);

        //    return stringList;
        //}

        ///// <inheritdoc/>
        //public List<string> GetEmailAddressesAsList()
        //{
        //    List<EmailRecipient> objectList = _localDatabaseService.GetSQLiteEmailAddresses();
        //    List<string> stringList = objectList.Select(i => i.ToString()).ToList();

        //    return stringList;
        //}

        /// <inheritdoc/>
        public List<string> GetEmailAddressesAsList()
        {
            List<EmailRecipient> objectList = _localDatabaseService.GetSQLiteEmailRecipients();
            List<string> stringList = objectList.Select(i => i.Address).ToList();

            return stringList;
        }

        /// <inheritdoc/>
//        [DebuggerStepThrough]
        public void UpdateModels()
        {
            // FixMe: switch this to three separate methods, each with an async/await
            string filename = string.Empty;

            try
            {
                CheckFolder(_dataFolderPathName);

                // review: how often are these tables going to be changing? do we really need to pull the fresh list every time?
                // request Reason Codes from the web service, and save them on-device
                Request = _targetURL + "/Visit/Reasons/";
                var request = new MvxRestRequest(Request);
                // (Action<MvxRestResponse>)ParseResponse

                int i = 0;

                do
                {
                    if (!ReasonCodeFileExists())
                    {
                        UpdateReasonCodeModel(request);

                        //_jsonRestClient.MakeRequestFor<List<ReasonCode>>(request,
                        //    response =>
                        //    {
                        //        _localDatabaseService.UpdateSQLiteReasonCodes(response.Result);

                        //        filename = _fileStore.PathCombine(_dataFolderPathName, _reasonCodeFileName);
                        //        _fileStore.WriteFile(filename, Serialize(response.Result));
                        //    },
                        //    (Action<Exception>)RestException);
                    }

                    if (!CallTypeFileExists())
                    {
                        // request Call Types from the web service
                        Request = _targetURL + "/Visit/CallTypes/";
                        request = new MvxRestRequest(Request);
                        _jsonRestClient.MakeRequestFor<List<CallType>>(request,
                            response =>
                            {
                                _localDatabaseService.UpdateSQLiteCallTypes(response.Result);

                                filename = _fileStore.PathCombine(_dataFolderPathName, _callTypeFileName);
                                _fileStore.WriteFile(filename, Serialize(response.Result));
                            },
                            (Action<Exception>)RestException);
                    }

                    if (!EmailRecipientFileExists())
                    {
                        // request Email Recipients from the web service
                        Request = _targetURL + "/Visit/EmailRecipients/";
                        request = new MvxRestRequest(Request);
                        _jsonRestClient.MakeRequestFor<List<EmailRecipient>>(request,
                            response =>
                            {
                                _localDatabaseService.UpdateSQLiteEmailRecipients(response.Result);

                                filename = _fileStore.PathCombine(_dataFolderPathName, _emailRecipientFileName);
                                _fileStore.WriteFile(filename, Serialize(response.Result));
                            },
                            (Action<Exception>)RestException);
                    }

                    i++;
                } while (i < 5);
            }
            catch (Exception e)
            {
                CommonCore.DebugMessage(_className, "UpdateModels");
                CommonCore.DebugMessage(" > " + e.Message);
            }
        }


        #endregion

        #region Model Support
        public void UpdateReasonCodeModel(MvxRestRequest request)
        {
            Request = _targetURL + "/Visit/Reasons/";

            try
            {
                _jsonRestClient.MakeRequestFor<List<ReasonCode>>(request,
                    (Action<MvxDecodedRestResponse<List<ReasonCode>>>)ReasonCodeRestResponse,
                    (Action<Exception>)RestException);
            }
            catch (Exception e)
            {
                CommonCore.DebugMessage(_className, "UpdateReasonCodeModel");
                CommonCore.DebugMessage(" > " + e.Message);

            }
        }

        private void ReasonCodeRestResponse(MvxDecodedRestResponse<List<ReasonCode>> response)
        {
            string filename = string.Empty;
            filename = _fileStore.PathCombine(_dataFolderPathName, _reasonCodeFileName);

            try
            {
                //RunOnUiThread(() => { 
                    _localDatabaseService.UpdateSQLiteReasonCodes(response.Result);
                    _fileStore.WriteFile(filename, Serialize(response.Result));
                //});
            }
            catch (Exception e)
            {
                CommonCore.DebugMessage(_className, "ReasonCodeRestResponse");
                CommonCore.DebugMessage(" > " + e.Message);
            }
        }

        //// Note: requires 'using System.Uri'
        //[DebuggerStepThrough]
        //public static async Task<string> SendAndReceiveJsonRequest(MvxRestRequest request)
        //{
        //    string responseStr = null;
        //    //string uri = "uri-to-send-the-data-to";
        //    string uri = request.ToString();

        //    // Create a json string with a single key/value pair.
        //    var json = new JObject(new JProperty("Reasons", "NA")).ToString();

        //    using (var httpClient = new HttpClient())
        //    {
        //        //create the HTTP request content
        //        HttpContent content = new StringContent(json);

        //        try
        //        {
        //            // Send the json to the server using POST
        //            Task<HttpResponseMessage> getResponse = httpClient.PostAsync(uri, content);

        //            // Wait for the response and read it to a string var
        //            HttpResponseMessage response = await getResponse;
        //            responseStr = await response.Content.ReadAsStringAsync();
        //        }
        //        catch (Exception e)
        //        {
        //            Debug.WriteLine("Error communicating with the server: " + e.Message);
        //        }
        //    }
        //    return responseStr;
        //}

        private void CheckFolder(string folderPath)
        {
            _fileStore.EnsureFolderExists(folderPath);
        }

        private bool XmlFileExists(string filePath)
        {
            string filename = _fileStore.PathCombine(_dataFolderPathName, filePath);
            return _fileStore.Exists(filename);
        }

        public bool CallTypeFileExists()
        {
            return XmlFileExists(_callTypeFileName);
        }

        public bool EmailRecipientFileExists()
        {
            return XmlFileExists(_emailRecipientFileName);
        }

        public bool ReasonCodeFileExists()
        {
            return XmlFileExists(_reasonCodeFileName);
        }

        // remove - unused
        private void ParseResponse(MvxRestResponse response)
        {
            _localDatabaseService.ReportUploaded(int.Parse(response.Tag));
        }

        private void RestException(Exception exception)
        {
            CommonCore.DebugMessage(_className, "RestException");
            CommonCore.DebugMessage(" > Original request: " + Request);
            CommonCore.DebugMessage(" > Exception message: " + exception.Message);
        }
        #endregion


        public event EventHandler<ErrorEventArgs> Error;

        /// <summary>Convert XML to an object.
        /// </summary>
        /// <typeparam name="T">The type to apply to the XML.</typeparam>
        /// <param name="xml">An XML ("serialized") string.</param>
        /// <returns>The <paramref name="xml"/> deserialized to an object.</returns>
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

        /// <summary>Convert an object to XML.
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
