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
        private bool _updatingReasonCodes = false;
        private bool _updatingCallTypes = false;
        private bool _updatingEmailRecipients = false;

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

        //public string Request
        //{
        //    get { return _request; }
        //    set
        //    {
        //        _request = value;
        //    }
        //}

        //public bool NotGetting 
        //{
        //    get 
        //    {
        //        bool notGetting = (!GettingCallTypes && !GettingEmailRecipients && !GettingReasonCodes);
        //        return notGetting;
        //    }
        //}

        // Review: once this is set to true, does it switch back?

        public bool BusyGetting 
        {
            get 
            {
                bool getting = (GettingCallTypes || GettingEmailRecipients || GettingReasonCodes);
                return getting;
            }
        }

        private bool GettingCallTypes
        {
            get { return _gettingCallTypes; }
            set { _gettingCallTypes = value; }
        }

        private bool GettingEmailRecipients
        {
            get { return _gettingEmailRecipients; }
            set { _gettingEmailRecipients = value; }
        }

        private bool GettingReasonCodes
        {
            get { return _gettingReasonCodes; }
            set { _gettingReasonCodes = value; }
        }

        public bool FilesExist()
        {
            bool filesExist = (!CallTypeFileMissing & !EmailRecipientFileMissing & !ReasonCodeFileMissing);
            return filesExist;
        }

        public bool BusyUpdating
        {
            get
            {
                bool updating = (UpdatingCallTypes || UpdatingEmailRecipients || UpdatingReasonCodes);
                return updating;
            }
        }

        private bool UpdatingCallTypes
        {
            get { return _updatingCallTypes; }
            set { _updatingCallTypes = value; }
        }

        private bool UpdatingEmailRecipients
        {
            get { return _updatingEmailRecipients; }
            set { _updatingEmailRecipients = value; }
        }

        private bool UpdatingReasonCodes
        {
            get { return _updatingReasonCodes; }
            set { _updatingReasonCodes = value; }
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
            CommonCore.DebugMessage("  core[sswds][um] > UpdateModels > starting...");

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
                CommonCore.DebugMessage("  core[sswds][um] > UpdateModels > do/while > trying...");

                CheckFolder(_dataFolderPathName);

                // review: how often are these tables going to be changing? do we really need to pull the fresh list every time?
                // request Reason Codes from the web service, and save them on-device
                
                int i = 0;

                do
                {
                    // increment up here, so that if there's an exception the counter will still advance
                    i++;

                    try
                    {
                        //CommonCore.DebugMessage("  core[sswds][um] > UpdateModels > ReasonCode > trying...");

                        if (!GettingReasonCodes & ReasonCodeFileMissing)
                        {
                            CommonCore.DebugMessage("  core[sswds][um] > UpdateModels > ReasonCode > getting Reasons...");

                            GettingReasonCodes = true;
                            string requestURL = _targetURL + "/Visit/Reasons/";
                            var request = new MvxRestRequest(requestURL);

                            CommonCore.DebugMessage("  core[sswds][um] > UpdateModels > Request: " + requestURL);

                            UpdateReasonCodeModel(request);
                        }
                        //CommonCore.DebugMessage("  core[sswds][um] > UpdateModels > ReasonCode > done trying.");

                    }
                    catch (Exception e)
                    {
                        CommonCore.DebugMessage("  core[sswds][um] > UpdateModels > ReasonCode > " + e.Message);
                    }
                    
                    try
                    {
                        //CommonCore.DebugMessage("  core[sswds][um] > UpdateModels > CallType > trying...");

                        if (!GettingCallTypes & CallTypeFileMissing)
                        {
                            CommonCore.DebugMessage("  core[sswds][um] > UpdateModels > CallType > getting CallTypes...");

                            GettingCallTypes = true;
                            string requestURL = _targetURL + "/Visit/CallTypes/";
                            var request = new MvxRestRequest(requestURL);

                            CommonCore.DebugMessage("  core[sswds][um] > UpdateModels > Request: " + requestURL);

                            UpdateCallTypeModel(request);
                        }
                        //CommonCore.DebugMessage("  core[sswds][um] > UpdateModels > CallType > done trying.");

                    }
                    catch (Exception e)
                    {
                        CommonCore.DebugMessage("  core[sswds][um] > UpdateModels > CallType > " + e.Message);
                    }

                    try
                    {
                        //CommonCore.DebugMessage("  core[sswds][um] > UpdateModels > EmailRecipient > trying...");

                        if (!GettingEmailRecipients & EmailRecipientFileMissing)
                        {
                            CommonCore.DebugMessage("  core[sswds][um] > UpdateModels > EmailRecipient > getting EmailRecipients...");

                            GettingEmailRecipients = true;
                            string requestURL = _targetURL + "/Visit/EmailRecipients/";
                            var request = new MvxRestRequest(requestURL);

                            CommonCore.DebugMessage("  core[sswds][um] > UpdateModels > Request: " + requestURL);

                            UpdateEmailRecipientModel(request);
                        }
                        //CommonCore.DebugMessage(  core[sswds][um] > "UpdateModels > EmailRecipient > done trying.");

                    }
                    catch (Exception e)
                    {
                        CommonCore.DebugMessage("  core[sswds][um] > UpdateModels > EmailRecipient > " + e.Message);
                    }

                    difference = DateTime.Now - start;
                    differenceInSeconds = difference.TotalSeconds;
                    remainder = i % 500;

                    if (remainder == 0)
                    {
                        message = "  core[sswds][um] > UpdateModels > i = " + i + ", Getting = " + BusyGetting.ToString().Substring(0, 1) + ", Updating = " + BusyUpdating.ToString().Substring(0, 1) + ", Files = ";
                        message += (!CallTypeFileMissing).ToString().Substring(0, 1) + " " + (!EmailRecipientFileMissing).ToString().Substring(0, 1) + " " + (!ReasonCodeFileMissing).ToString().Substring(0, 1);
                        message += ", Time = " + differenceInSeconds.ToString();
                        CommonCore.DebugMessage(message);
                    }

                    // !FilesExist() && (differenceInSeconds < 0.1) && (i < 10)
                } while (!FilesExist() & differenceInSeconds < 10);

                CommonCore.DebugMessage("  core[sswds][um] > UpdateModels > do/while > done trying.");

            }
            catch (Exception e)
            {
                CommonCore.DebugMessage("  core[sswds][um] > UpdateModels > do/while > " + e.Message);
            }
            finally
            {
                CommonCore.DebugMessage("  core[sswds][um] > UpdateModels > do/while > finally.");

            }

            CommonCore.DebugMessage("  core[sswds][um] > UpdateModels > do/while > completed.");

        }


        #endregion

        #region Model Support
        public void UpdateReasonCodeModel(MvxRestRequest request)
        {
            string methodName = _className + " > UpdateReasonCodeModel";
            CommonCore.DebugMessage(methodName + " > starting...");

            try
            {
                CommonCore.DebugMessage(methodName + " > ...trying...");

                _jsonRestClient.MakeRequestFor<List<ReasonCode>>(request,
                    (Action<MvxDecodedRestResponse<List<ReasonCode>>>)ReasonCodeRestResponse,
                    (Action<Exception>)ReasonCodeRestException);
                CommonCore.DebugMessage(methodName + " > ...done trying...");                
            }
            catch (Exception e)
            {
                // can this code ever (reasonably) be reached?
                CommonCore.DebugMessage(methodName + " > " + e.Message);
                GettingReasonCodes = false;
            }

            CommonCore.DebugMessage(methodName + " > finished.");
        }

        /// <summary>Handle the REST Response containing <c>ReasonCode</c>.
        /// </summary>
        /// <param name="response">The REST response</param>
        /// <remarks>This code doesn't run until a few seconds after the Request is
        /// generated by <c>UpdateReasonCodeModel()</c>.</remarks>
        private void ReasonCodeRestResponse(MvxDecodedRestResponse<List<ReasonCode>> response)
        {
            string methodName = _className + " > ReasonCodeRestResponse";
            CommonCore.DebugMessage(methodName + " > starting...");

            string filename = string.Empty;
            filename = _fileStore.PathCombine(_dataFolderPathName, _reasonCodeFileName);

            int? rowsUpdated = null;

            while (!rowsUpdated.HasValue)
            {
                if (!BusyUpdating)
                {
                    UpdatingReasonCodes = true;

                    try
                    {
                        CommonCore.DebugMessage(methodName + " > ...trying UpdateSQLite[table_name]()...");
                        rowsUpdated = _localDatabaseService.UpdateSQLiteReasonCodes(response.Result);

                        CommonCore.DebugMessage(methodName + " > ...done UpdateSQLite[table_name]()....");

                        try
                        {
                            CommonCore.DebugMessage(methodName + " > ...trying WriteFile(" + filename + ")...");
                            _fileStore.WriteFile(filename, Serialize(response.Result));

                            CommonCore.DebugMessage(methodName + " > ...done WriteFile(" + filename + ")....");
                            UpdatingReasonCodes = GettingReasonCodes = false;    // success! 
                        }
                        catch (Exception e)
                        {
                            CommonCore.DebugMessage(methodName + " > generated an Exception trying to WriteFile(" + filename + ")");
                            CommonCore.DebugMessage(methodName + " > " + e.Message);
                            UpdatingReasonCodes = GettingReasonCodes = false;    // Failed to write file. We'll try both again by resetting the flag.
                        }
                    }
                    catch (Exception e)
                    {
                        CommonCore.DebugMessage(methodName + " > received a REST 'response' (not an 'exception'), but the content indicates an error occurred.");
                        CommonCore.DebugMessage(methodName + " > " + e.Message);

                        // ToDo: handle each bad response separately. Ex: if (e.Message.ToLower().Contains("locked"))
                        UpdatingReasonCodes = GettingReasonCodes = false;    // Failed to update local SQLite dB. We'll try both again by resetting the flag.
                    }
                }
            }
            
            CommonCore.DebugMessage(methodName + " > ...finished.");
        }

        public void UpdateCallTypeModel(MvxRestRequest request)
        {
            string methodName = _className + " > UpdateCallTypeModel";
            CommonCore.DebugMessage(methodName + " > starting...");

            try
            {
                CommonCore.DebugMessage(methodName + " > ...trying...");

                _jsonRestClient.MakeRequestFor<List<CallType>>(request,
                    (Action<MvxDecodedRestResponse<List<CallType>>>)CallTypeRestResponse,
                    (Action<Exception>)CallTypeRestException);
                CommonCore.DebugMessage(methodName + " > ...done trying...");
            }
            catch (Exception e)
            {
                CommonCore.DebugMessage(methodName + " > " + e.Message);
                GettingCallTypes = false;
            }

            CommonCore.DebugMessage(methodName + " > finished.");
        }

        private void CallTypeRestResponse(MvxDecodedRestResponse<List<CallType>> response)
        {
            string methodName = _className + " > CallTypeRestResponse";
            CommonCore.DebugMessage(methodName + " > starting...");

            // Broken: if the response can't be converted (ie it is the word "locked" or something ) don't try the conversion
            // ToDo: might even need to change this so it is accepting an object, so the object can be evaluated to see if it is the correct type before proceeding
            string filename = string.Empty;
            filename = _fileStore.PathCombine(_dataFolderPathName, _callTypeFileName);

            int? rowsUpdated = null;

            while (!rowsUpdated.HasValue)
            {
                if (!BusyUpdating)
                {
                    UpdatingCallTypes = true;

                    try
                    {
                        CommonCore.DebugMessage(methodName + " > ...trying UpdateSQLite[table_name]()...");
                        rowsUpdated = _localDatabaseService.UpdateSQLiteCallTypes(response.Result);

                        CommonCore.DebugMessage(methodName + " > ...done UpdateSQLite[table_name]()....");

                        try
                        {
                            CommonCore.DebugMessage(methodName + " > ...trying WriteFile(" + filename + ")...");
                            _fileStore.WriteFile(filename, Serialize(response.Result));

                            CommonCore.DebugMessage(methodName + " > ...done WriteFile(" + filename + ")....");
                            UpdatingCallTypes = GettingCallTypes = false;    // success! 
                        }
                        catch (Exception e)
                        {
                            CommonCore.DebugMessage(methodName + " > generated an Exception trying to WriteFile(" + filename + ")");
                            CommonCore.DebugMessage(methodName + " > " + e.Message);
                            UpdatingCallTypes = GettingCallTypes = false;    // Failed to write file. We'll try both again by resetting the flag.
                        }
                    }
                    catch (Exception e)
                    {
                        CommonCore.DebugMessage(methodName + " > received a REST 'response' (not an 'exception'), but the content indicates an error occurred.");
                        CommonCore.DebugMessage(methodName + " > " + e.Message);

                        // ToDo: handle each bad response separately. Ex: if (e.Message.ToLower().Contains("locked"))
                        UpdatingCallTypes = GettingCallTypes = false;    // Failed to update local SQLite dB. We'll try both again by resetting the flag.
                    }
                }
            }
            
            CommonCore.DebugMessage(methodName + " > ...finished.");
        }

        public void UpdateEmailRecipientModel(MvxRestRequest request)
        {
            string methodName = _className + " > UpdateEmailRecipientModel";
            CommonCore.DebugMessage(methodName + " > starting...");

            try
            {
                CommonCore.DebugMessage(methodName + " > ...trying...");

                _jsonRestClient.MakeRequestFor<List<EmailRecipient>>(request,
                    (Action<MvxDecodedRestResponse<List<EmailRecipient>>>)EmailRecipientRestResponse,
                    (Action<Exception>)EmailRecipientsRestException);
                CommonCore.DebugMessage(methodName + " > ...done trying...");
            }
            catch (Exception e)
            {
                CommonCore.DebugMessage(methodName + " > " + e.Message);
                GettingEmailRecipients = false;
            }

            CommonCore.DebugMessage(methodName + " > finished.");
        }

        private void EmailRecipientRestResponse(MvxDecodedRestResponse<List<EmailRecipient>> response)
        {
            string methodName = _className + " > EmailRecipientRestResponse";

            CommonCore.DebugMessage(methodName + " > starting...");

            // Broken: if the response can't be converted (ie it is the word "locked" or something ) don't try the conversion
            // ToDo: might even need to change this so it is accepting an object, so the object can be evaluated to see if it is the correct type before proceeding
            string filename = string.Empty;
            filename = _fileStore.PathCombine(_dataFolderPathName, _emailRecipientFileName);

            int? rowsUpdated = null;

            while (!rowsUpdated.HasValue)
            {
                if (!BusyUpdating)
                {
                    UpdatingEmailRecipients = true;

                    try
                    {
                        CommonCore.DebugMessage(methodName + " > ...trying UpdateSQLite[table_name]()...");
                        rowsUpdated = _localDatabaseService.UpdateSQLiteEmailRecipients(response.Result);

                        CommonCore.DebugMessage(methodName + " > ...done UpdateSQLite[table_name]()....");

                        try
                        {
                            CommonCore.DebugMessage(methodName + " > ...trying WriteFile(" + filename + ")...");
                            _fileStore.WriteFile(filename, Serialize(response.Result));

                            CommonCore.DebugMessage(methodName + " > ...done WriteFile(" + filename + ")....");
                            UpdatingEmailRecipients = GettingEmailRecipients = false;    // success! 
                        }
                        catch (Exception e)
                        {
                            CommonCore.DebugMessage(methodName + " > generated an Exception trying to WriteFile(" + filename + ")");
                            CommonCore.DebugMessage(methodName + " > " + e.Message);
                            UpdatingEmailRecipients = GettingEmailRecipients = false;    // Failed to write file. We'll try both again by resetting the flag.
                        }
                    }
                    catch (Exception e)
                    {
                        CommonCore.DebugMessage(methodName + " > received a REST 'response' (not an 'exception'), but the content indicates an error occurred.");
                        CommonCore.DebugMessage(methodName + " > " + e.Message);

                        // ToDo: handle each bad response separately. Ex: if (e.Message.ToLower().Contains("locked"))
                        UpdatingEmailRecipients = GettingEmailRecipients = false;    // Failed to update local SQLite dB. We'll try both again by resetting the flag.
                    }
                }
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
            //CommonCore.DebugMessage("  core[sswds][xfm] > Checking for " + filename);

            bool fileExists = _fileStore.Exists(filename);
            bool fileMissing = !fileExists;
            //CommonCore.DebugMessage("  core[sswds][xfm] > fileMissing = " + fileMissing.ToString());

            return fileMissing;
        }

        private bool CallTypeFileMissing
        {
            get { return XmlFileMissing(_callTypeFileName); }
        }

        private bool EmailRecipientFileMissing
        {
            get { return XmlFileMissing(_emailRecipientFileName); }
        }

        private bool ReasonCodeFileMissing
        {
            get { return XmlFileMissing(_reasonCodeFileName); }
        }

        // remove - unused
        private void ParseResponse(MvxRestResponse response)
        {
            _localDatabaseService.ReportUploaded(int.Parse(response.Tag));
        }

        private void CallTypeRestException(Exception exception)
        {
            UpdatingCallTypes = GettingCallTypes = false;    // start over...
            CommonCore.DebugMessage(_className + " > CallTypeRestException");
            RestExceptionMessage(exception);
        }

        private void EmailRecipientsRestException(Exception exception)
        {
            UpdatingEmailRecipients = GettingEmailRecipients = false;    // start over...
            CommonCore.DebugMessage(_className + " > EmailRecipientsRestException");
            RestExceptionMessage(exception);
        }

        private void ReasonCodeRestException(Exception exception)
        {
            UpdatingReasonCodes = GettingReasonCodes = false;    // start over...
            CommonCore.DebugMessage(_className + " > ReasonCodeRestException");
            RestExceptionMessage(exception);
        }

        private void RestExceptionMessage(Exception exception)
        {
            CommonCore.DebugMessage(_className, "RestExceptionMessage");
            CommonCore.DebugMessage(_className + " > Exception message: " + exception.Message);
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
