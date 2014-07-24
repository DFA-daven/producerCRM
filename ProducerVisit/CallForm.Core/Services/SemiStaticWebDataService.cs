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
    using System.Threading;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    /// <summary>Implements the <see cref="ISemiStaticWebDataService"/> interface.
    /// </summary>
    public class SemiStaticWebDataService : ISemiStaticWebDataService
    {
        private readonly IMvxFileStore _fileStore;
        private readonly IMvxJsonRestClient _jsonRestClient;
        private readonly IDataService _localDatabaseService;
        
        private string _request;
        private string _className = "CallForm.Core.Services.SemiStaticWebDataService";
        private bool _working = false;
        private bool _gettingReasonCodes = false;
        private bool _gettingCallTypes = false;
        private bool _gettingEmailRecipients = false;

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

        public bool NotBusy()
        {
            bool notGetting = (!_gettingCallTypes & !_gettingEmailRecipients & !_gettingReasonCodes);
            return notGetting;
        }

        public bool Busy()
        {
            bool getting = (_gettingCallTypes & _gettingEmailRecipients & _gettingReasonCodes);
            return getting;
        }

        public bool FilesExist()
        {
            bool filesExist = (!CallTypeFileMissing() & !EmailRecipientFileMissing() & !ReasonCodeFileMissing());
            return filesExist;
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
            CommonCore.DebugMessage(_className, "UpdateModels");
            CommonCore.DebugMessage("UpdateModels > starting...");

            // FixMe: switch this to three separate methods, each with an async/await
            string filename = string.Empty;

            int remainder = 0;
            string message = string.Empty;

            int Available = 0;
            int Taken = 1;
            int state = 0;

            int original = Interlocked.CompareExchange(ref state, Taken, Available);

            DateTime start = DateTime.Now;
            DateTime end = new DateTime();
            TimeSpan difference = new TimeSpan();
            double differenceInSeconds = 0;

            try
            {
                CommonCore.DebugMessage("UpdateModels > do/while > trying...");

                CheckFolder(_dataFolderPathName);

                // review: how often are these tables going to be changing? do we really need to pull the fresh list every time?
                // request Reason Codes from the web service, and save them on-device
                Request = _targetURL + "/Visit/Reasons/";
                var request = new MvxRestRequest(Request);
                // (Action<MvxRestResponse>)ParseResponse

                int i = 0;

                do
                {
                    // increment up here, so that if there's an exception the counter will still advance
                    i++;

                    try
                    {
                        //CommonCore.DebugMessage("UpdateModels > ReasonCode > trying...");

                        if (!_gettingReasonCodes & ReasonCodeFileMissing())
                        {
                            CommonCore.DebugMessage("UpdateModels > ReasonCode > getting Reasons...");

                            _gettingReasonCodes = true;
                            Request = _targetURL + "/Visit/Reasons/";

                            CommonCore.DebugMessage("UpdateModels > Request: " + Request);

                            UpdateReasonCodeModel(request);
                        }
                        //CommonCore.DebugMessage("UpdateModels > ReasonCode > done trying.");

                    }
                    catch (Exception e)
                    {
                        CommonCore.DebugMessage("UpdateModels > ReasonCode > " + e.Message);
                    }
                    finally
                    {
                        //CommonCore.DebugMessage("UpdateModels > ReasonCode > finally.");
                    }
                    
                    try
                    {
                        //CommonCore.DebugMessage("UpdateModels > CallType > trying...");

                        if (!_gettingCallTypes & CallTypeFileMissing())
                        {
                            CommonCore.DebugMessage("UpdateModels > CallType > getting CallTypes...");

                            _gettingCallTypes = true;
                            Request = _targetURL + "/Visit/CallTypes/";

                            CommonCore.DebugMessage("UpdateModels > Request: " + Request );

                            UpdateCallTypeModel(request);
                        }
                        //CommonCore.DebugMessage("UpdateModels > CallType > done trying.");

                    }
                    catch (Exception e)
                    {
                        CommonCore.DebugMessage("UpdateModels > CallType > " + e.Message);
                    }
                    finally
                    {
                        //CommonCore.DebugMessage("UpdateModels > CallType > finally.");
                    }

                    try
                    {
                        //CommonCore.DebugMessage("UpdateModels > EmailRecipient > trying...");

                        if (!_gettingEmailRecipients & EmailRecipientFileMissing())
                        {
                            CommonCore.DebugMessage("UpdateModels > EmailRecipient > getting EmailRecipients...");

                            _gettingEmailRecipients = true;
                            Request = _targetURL + "/Visit/EmailRecipients/";
                            
                            CommonCore.DebugMessage("UpdateModels > Request: " + Request);

                            UpdateEmailRecipientModel(request);
                        }
                        //CommonCore.DebugMessage("UpdateModels > EmailRecipient > done trying.");

                    }
                    catch (Exception e)
                    {
                        CommonCore.DebugMessage("UpdateModels > EmailRecipient > " + e.Message);
                    }
                    finally
                    {
                        //CommonCore.DebugMessage("UpdateModels > EmailRecipient > finally.");
                    }

                    difference = DateTime.Now - start;
                    differenceInSeconds = difference.TotalSeconds;
                    remainder = i % 25;

                    if (remainder == 0)
                    {
                        message = "UpdateModels > i = " + i + ", Busy = " + Busy().ToString() + ", FilesExist = " + FilesExist().ToString() + ", Diff = " + differenceInSeconds.ToString();
                        CommonCore.DebugMessage(message);
                    }

                    // !FilesExist() && (differenceInSeconds < 0.1) && (i < 10)
                } while (!FilesExist() & differenceInSeconds < 10);

                CommonCore.DebugMessage("UpdateModels > do/while > done trying.");

            }
            catch (Exception e)
            {
                CommonCore.DebugMessage("UpdateModels > do/while > " + e.Message);
            }
            finally
            {
                CommonCore.DebugMessage("UpdateModels > do/while > finally.");

            }

            CommonCore.DebugMessage("UpdateModels > do/while > completed.");

        }


        #endregion

        #region Model Support
        public void UpdateReasonCodeModel(MvxRestRequest request)
        {
            CommonCore.DebugMessage(_className, "UpdateReasonCodeModel > starting...");

            Request = _targetURL + "/Visit/Reasons/";

            try
            {
                CommonCore.DebugMessage("UpdateReasonCodeModel > ...trying...");

                _jsonRestClient.MakeRequestFor<List<ReasonCode>>(request,
                    (Action<MvxDecodedRestResponse<List<ReasonCode>>>)ReasonCodeRestResponse,
                    (Action<Exception>)RestException);
                CommonCore.DebugMessage("UpdateReasonCodeModel > ...done trying...");                
            }
            catch (Exception e)
            {
                CommonCore.DebugMessage("UpdateReasonCodeModel > " + e.Message);

            }
            finally
            {
                CommonCore.DebugMessage("UpdateReasonCodeModel > ...finally...");

            }

            CommonCore.DebugMessage("UpdateReasonCodeModel > finished.");

        }

        private void ReasonCodeRestResponse(MvxDecodedRestResponse<List<ReasonCode>> response)
        {
            string methodName = "ReasonCodeRestResponse";
            CommonCore.DebugMessage(_className, methodName + " > starting...");

            // Broken: if the response can't be converted (ie it is the word "locked" or something ) don't try the conversion
            // ToDo: might even need to change this so it is accepting an object, so the object can be evaluated to see if it is the correct type before proceeding
            string filename = string.Empty;
            filename = _fileStore.PathCombine(_dataFolderPathName, _reasonCodeFileName);

            try
            {
                //RunOnUiThread(() => { 
                CommonCore.DebugMessage(methodName + " > ...trying UpdateSQLiteReasonCodes()...");
                    _localDatabaseService.UpdateSQLiteReasonCodes(response.Result);
                    CommonCore.DebugMessage(methodName + " > ...trying WriteFile()...");
                    _fileStore.WriteFile(filename, Serialize(response.Result));
                //});

                _gettingReasonCodes = false;
                CommonCore.DebugMessage(methodName + " > ...done trying....");
            }
            catch (Exception e)
            {
                if (e.Message.ToLower().Contains("locked"))
                {
                    _gettingReasonCodes = false;
                }
                CommonCore.DebugMessage(_className, methodName + " > " + e.Message);
            }

            CommonCore.DebugMessage(methodName + " > ...finished.");
        }

        public void UpdateCallTypeModel(MvxRestRequest request)
        {
            CommonCore.DebugMessage(_className, "UpdateCallTypeModel > starting...");

            Request = _targetURL + "/Visit/CallTypes/";

            try
            {
                CommonCore.DebugMessage("UpdateCallTypeModel > ...trying...");

                _jsonRestClient.MakeRequestFor<List<CallType>>(request,
                    (Action<MvxDecodedRestResponse<List<CallType>>>)CallTypeRestResponse,
                    (Action<Exception>)RestException);
                CommonCore.DebugMessage("UpdateCallTypeModel > ...done trying...");
            }
            catch (Exception e)
            {
                CommonCore.DebugMessage("UpdateCallTypeModel > " + e.Message);

            }
            finally
            {
                CommonCore.DebugMessage("UpdateCallTypeModel > ...finally...");

            }

            CommonCore.DebugMessage("UpdateCallTypeModel > finished.");

        }

        private void CallTypeRestResponse(MvxDecodedRestResponse<List<CallType>> response)
        {
            string methodName = "CallTypeRestResponse";
            CommonCore.DebugMessage(_className, methodName + " > starting...");

            // Broken: if the response can't be converted (ie it is the word "locked" or something ) don't try the conversion
            // ToDo: might even need to change this so it is accepting an object, so the object can be evaluated to see if it is the correct type before proceeding
            string filename = string.Empty;
            filename = _fileStore.PathCombine(_dataFolderPathName, _callTypeFileName);

            try
            {
                CommonCore.DebugMessage(methodName + " > ...trying UpdateSQLiteCallTypes()...");
                _localDatabaseService.UpdateSQLiteCallTypes (response.Result);
                CommonCore.DebugMessage(methodName + " > ...trying WriteFile()...");
                _fileStore.WriteFile(filename, Serialize(response.Result));

                _gettingCallTypes = false;
                CommonCore.DebugMessage(methodName + " > ...done trying....");
            }
            catch (Exception e)
            {
                if (e.Message.ToLower().Contains("locked"))
                {
                    _gettingCallTypes = false;
                }
                CommonCore.DebugMessage(_className, methodName + " > " + e.Message);
            }

            CommonCore.DebugMessage(methodName + " > ...finished.");
        }

        public void UpdateEmailRecipientModel(MvxRestRequest request)
        {
            CommonCore.DebugMessage(_className, "UpdateEmailRecipientModel > starting...");

            Request = _targetURL + "/Visit/EmailRecipients/";

            try
            {
                CommonCore.DebugMessage("UpdateEmailRecipientModel > ...trying...");

                _jsonRestClient.MakeRequestFor<List<EmailRecipient>>(request,
                    (Action<MvxDecodedRestResponse<List<EmailRecipient>>>)EmailRecipientRestResponse,
                    (Action<Exception>)RestException);
                CommonCore.DebugMessage("UpdateEmailRecipientModel > ...done trying...");
            }
            catch (Exception e)
            {
                CommonCore.DebugMessage("UpdateEmailRecipientModel > " + e.Message);

            }
            finally
            {
                CommonCore.DebugMessage("UpdateEmailRecipientModel > ...finally...");

            }

            CommonCore.DebugMessage("UpdateEmailRecipientModel > finished.");

        }

        private void EmailRecipientRestResponse(MvxDecodedRestResponse<List<EmailRecipient>> response)
        {
            string methodName = "EmailRecipientRestResponse";

            CommonCore.DebugMessage(_className, methodName + " > starting...");

            // Broken: if the response can't be converted (ie it is the word "locked" or something ) don't try the conversion
            // ToDo: might even need to change this so it is accepting an object, so the object can be evaluated to see if it is the correct type before proceeding
            string filename = string.Empty;
            filename = _fileStore.PathCombine(_dataFolderPathName, _emailRecipientFileName);

            try
            {
                CommonCore.DebugMessage(methodName + " > ...trying UpdateSQLiteEmailRecipients()...");
                _localDatabaseService.UpdateSQLiteEmailRecipients (response.Result);
                CommonCore.DebugMessage(methodName + " > ...trying WriteFile()...");
                _fileStore.WriteFile(filename, Serialize(response.Result));

                _gettingEmailRecipients = false;
                CommonCore.DebugMessage(methodName + " > ...done trying....");
            }
            catch (Exception e)
            {
                if (e.Message.ToLower().Contains("locked"))
                {
                    _gettingEmailRecipients = false;
                }
                CommonCore.DebugMessage(_className, methodName + " > " + e.Message);
            }

            CommonCore.DebugMessage(methodName + " > ...finished.");
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

        private bool XmlFileMissing(string filePath)
        {
            string filename = _fileStore.PathCombine(_dataFolderPathName, filePath);
            //CommonCore.DebugMessage("Checking for " + filename);

            bool fileExists = _fileStore.Exists(filename);
            bool fileMissing = !fileExists;
            //CommonCore.DebugMessage(" > fileMissing = " + fileMissing.ToString());

            return fileMissing;
        }

        public bool CallTypeFileMissing()
        {
            return XmlFileMissing(_callTypeFileName);
        }

        public bool EmailRecipientFileMissing()
        {
            return XmlFileMissing(_emailRecipientFileName);
        }

        public bool ReasonCodeFileMissing()
        {
            return XmlFileMissing(_reasonCodeFileName);
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
