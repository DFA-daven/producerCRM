using System.Collections.Generic;
using System.Linq;
using CallForm.Core.Models;
using Cirrious.MvvmCross.Plugins.Sqlite;

namespace CallForm.Core.Services
{
    public class DataService : IDataService
    {
        private readonly IUserIdentityService _userIdentityService;
        private readonly ISQLiteConnection _connection;

        public DataService(ISQLiteConnectionFactory factory,
            IUserIdentityService userIdentityService)
        {
            _userIdentityService = userIdentityService;
            _connection = factory.Create("one.sql");
            _connection.CreateTable<StoredProducerVisitReport>();
            _connection.CreateTable<VisitXReason>();
            _connection.CreateTable<ReasonCode>();
        }

        public List<ProducerVisitReport> ToUpload()
        {
            var stored = _connection.Table<StoredProducerVisitReport>()
                              .Where(x => x.Uploaded == false)
                              .ToList();
            return stored.Select(Hydrated).ToList();
        }

        private ProducerVisitReport Hydrated(StoredProducerVisitReport spvr)
        {
            List<VisitXReason> vxrs = _connection.Table<VisitXReason>().Where(vxr => vxr.VisitID == spvr.ID).ToList();
            List<int> reasonids = vxrs.Select(vxr => vxr.ReasonID).ToList();
            ReasonCode[] reasonCodes = _connection.Table<ReasonCode>().ToList().Where(rc => reasonids.Contains(rc.ID)).ToArray();
            return spvr.Hydrate(reasonCodes);
        }

        public List<ReportListItem> Recent()
        {
            var spvrs = _connection.Table<StoredProducerVisitReport>()
                .OrderByDescending(pvr => pvr.ID)
                .Take(20)
                .ToList();
            return spvrs.Select(spvr =>
                {
                    var pvr = Hydrated(spvr);
                    return new ReportListItem
                    {
                        ID = spvr.ID,
                        UserEmail = _userIdentityService.IdentityRecorded ? _userIdentityService.GetSavedIdentity().UserEmail : "You",
                        FarmNumber = pvr.FarmNumber,
                        Local = true,
                        PrimaryReasonCode = pvr.ReasonCodes != null && pvr.ReasonCodes.Length > 0 ? pvr.ReasonCodes[0] : new ReasonCode {Name = "No Reason"},
                        VisitDate = pvr.VisitDate,
                        Uploaded = spvr.Uploaded
                    };
                }).ToList();
        }

        public List<ReasonCode> GetReasonsForCall()
        {
            return _connection.Table<ReasonCode>().ToList();
        }

        public void UpdateReasons(List<ReasonCode> reasonCodes)
        {
            _connection.DropTable<ReasonCode>();
            _connection.CreateTable<ReasonCode>();
            _connection.InsertAll(reasonCodes);
        }

        public void Insert(ProducerVisitReport report)
        {
            var spvr = new StoredProducerVisitReport(report);
            _connection.Insert(spvr);
            foreach (var reasonCode in report.ReasonCodes)
            {
                var vxr = new VisitXReason
                {
                    ReasonID = reasonCode.ID,
                    VisitID = spvr.ID,
                };
                _connection.Insert(vxr);
            }
        }

        public ProducerVisitReport GetReport(int id)
        {
            var spvr = _connection.Get<StoredProducerVisitReport>(id);
            return Hydrated(spvr);
        }

        public int Count
        {
            get
            {
                return _connection.Table<StoredProducerVisitReport>().Count();
            }
        }

        public void ReportUploaded(int id)
        {
            var report = _connection.Get<StoredProducerVisitReport>(id);
            report.Uploaded = true;
            _connection.Update(report);
        }
    }
}
