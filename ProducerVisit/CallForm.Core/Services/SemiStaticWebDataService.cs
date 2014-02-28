namespace CallForm.Core.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Serialization;
    using CallForm.Core.Models;
    using Cirrious.MvvmCross.Plugins.File;
    using Cirrious.MvvmCross.Plugins.Network.Rest;

    public class SemiStaticWebDataService : ISemiStaticWebDataService
    {
        private readonly IMvxFileStore _fileStore;
        private readonly IMvxJsonRestClient _jsonRestClient;
        private readonly IDataService _dataService;

        public SemiStaticWebDataService(IMvxFileStore fileStore, IMvxJsonRestClient jsonRestClient, IDataService dataService)
        {
            _fileStore = fileStore;
            _jsonRestClient = jsonRestClient;
            _dataService = dataService;
        }

        public List<ReasonCode> GetReasonsForCall()
        {
            return _dataService.GetReasonsForCall();
        }

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
                // fixme: 
                return new List<string>(new[]
                {
                    "Farm Visit",
                    "Phone Call",
                    "Email",
                    "Farm Show",
                    "Other"
                });
            }
        }

        public List<string> GetEmailRecipients()
        {
            _fileStore.EnsureFolderExists("Data");
            string xml = string.Empty;
            var emailsFilename = _fileStore.PathCombine("Data", "Emails.xml");
            if (_fileStore.Exists(emailsFilename) && _fileStore.TryReadTextFile(emailsFilename, out xml))
            {
                return Deserialize<List<string>>(xml);
            }
            else
            {
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

        public void Update()
        {
            // TODO: update this to the current backend target
            var request = new MvxRestRequest("http://dl-backend.azurewebsites.net/Visit/Reasons/");
            _jsonRestClient.MakeRequestFor<List<ReasonCode>>(request,
                response => _dataService.UpdateReasons(response.Result),
                exception => { });
            // TODO: update this to the current backend target
            //request = new MvxRestRequest("http://dl-webserver-te.dairydata.local:480/Visit/CallTypes/");
            request = new MvxRestRequest("http://dl-backend.azurewebsites.net/Visit/CallTypes/");
            _jsonRestClient.MakeRequestFor<List<string>>(request,
                response =>
                {
                    _fileStore.EnsureFolderExists("Data");
                    var filename = _fileStore.PathCombine("Data", "CallTypes.xml");
                    _fileStore.WriteFile(filename, Serialize(response.Result));
                },
                exception => { });
            // TODO: update this to the current backend target
            request = new MvxRestRequest("http://dl-backend.azurewebsites.net/Visit/EmailRecipients/");
            _jsonRestClient.MakeRequestFor<List<string>>(request,
                response =>
                {
                    _fileStore.EnsureFolderExists("Data");
                    var filename = _fileStore.PathCombine("Data", "Emails.xml");
                    _fileStore.WriteFile(filename, Serialize(response.Result));
                },
                exception => { });
        }

        public static T Deserialize<T>(string s)
        {
            var serializer = new XmlSerializer(typeof(T));
            T container;
            using (TextReader stream = new StringReader(s))
            {
                container = (T)serializer.Deserialize(stream);
            }
            return container;
        }

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
