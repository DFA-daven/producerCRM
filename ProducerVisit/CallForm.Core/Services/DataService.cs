namespace CallForm.Core.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using CallForm.Core.Models;
    using Cirrious.MvvmCross.Plugins.Sqlite;

    /// <summary>Implements the <seealso cref="IDataService"/> interface.
    /// </summary>
    public class DataService : IDataService
    {
        /// <inheritdoc/>
        private readonly IUserIdentityService _userIdentityService;
        
        /// <summary>The connection to the SQLite database.
        /// </summary>
        private readonly ISQLiteConnection _connection;

        /// <summary>Opens the SQLite database; 
        /// if needed create an instance of an SQLite database; 
        /// if needed create tables: StoredProducerVisitReport, VisitXReason, and ReasonCode; 
        /// add 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="userIdentityService"></param>
        public DataService(ISQLiteConnectionFactory factory, IUserIdentityService userIdentityService)
        {
            string address = "one.sql";
            _connection = factory.Create(address);

            // create a table of type StoredProducerVisitReport
            _connection.CreateTable<StoredProducerVisitReport>();

            // create a table of type VisitXReason
            _connection.CreateTable<VisitXReason>();

            // create a table of type ReasonCode
            _connection.CreateTable<ReasonCode>();

            _userIdentityService = userIdentityService;
        }

        /// <summary>Opens the SQLite database, adds <seealso cref="ReasonCode"/>[] to the <seealso cref="StoredProducerVisitReport"/>, and 
        /// returns a <seealso cref="ProducerVisitReport"/>.
        /// </summary>
        /// <param name="spvr">A <seealso cref="StoredProducerVisitReport"/>.</param>
        /// <returns>A <seealso cref="ProducerVisitReport"/> based on a <seealso cref="StoredProducerVisitReport"/>.</returns>
        /// <remarks>Opens the <seealso cref="DataService._connection"/>, queries the <seealso cref="VisitXReason"/> table for the given
        /// <seealso cref="StoredProducerVisitReport"/> ID, matches the VisitXReason.ReasonIDs against the <seealso cref="ReasonCode"/> table
        /// to get a <seealso cref="ReasonCode"/>[], and returns the StoredProducerVisitReport.Hydrate(reasonCodes), aka a <seealso cref="ProducerVisitReport"/>.</remarks>
        private ProducerVisitReport Hydrated(StoredProducerVisitReport spvr)
        {
            List<VisitXReason> vxrs = _connection.Table<VisitXReason>().Where(vxr => vxr.VisitID == spvr.ID).ToList();
            List<int> reasonids = vxrs.Select(vxr => vxr.ReasonID).ToList();
            ReasonCode[] reasonCodes = _connection.Table<ReasonCode>().ToList().Where(rc => reasonids.Contains(rc.ID)).ToArray();
            return spvr.Hydrate(reasonCodes);
        }

        #region Required Definitions
        /// <inheritdoc/>
        public List<ProducerVisitReport> ToUpload()
        {
            var stored = _connection.Table<StoredProducerVisitReport>()
                                .Where(x => x.Uploaded == false)
                                .ToList();
            return stored.Select(Hydrated).ToList();
        }

        /// <inheritdoc/>
        public List<ReportListItem> Recent()
        {
            // fixme: change quantity to a .resx value
            int quantity = 100;

            var spvrs = _connection.Table<StoredProducerVisitReport>()
                .OrderByDescending(pvr => pvr.VisitDate)
                .Take(quantity)
                .ToList();

            return spvrs.Select(spvr =>
                {
                    var pvr = Hydrated(spvr);
                    // review: would a reason code ever not be found? is "other" the right thing to show?
                    return new ReportListItem
                    {
                        ID = spvr.ID,
                        UserEmail = _userIdentityService.IdentityRecorded ? _userIdentityService.GetSavedIdentity().UserEmail : "You",
                        FarmNumber = pvr.FarmNumber,
                        Local = true,
                        PrimaryReasonCode = pvr.ReasonCodes != null && pvr.ReasonCodes.Length > 0 ? pvr.ReasonCodes[0] : new ReasonCode { Name = "Other", Code = -1 },
                        VisitDate = pvr.VisitDate,
                        Uploaded = spvr.Uploaded
                    };
                }).ToList();
        }

        /// <inheritdoc/>
        public List<ReasonCode> GetReasonsForCall()
        {
            var reasons = _connection.Table<ReasonCode>().ToList();
            return reasons;
        }

        /// <inheritdoc/>
        public void UpdateReasons(List<ReasonCode> reasonCodes)
        {
            // drop the existing table
            _connection.DropTable<ReasonCode>();
            _connection.CreateTable<ReasonCode>();
            _connection.InsertAll(reasonCodes); 
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public ProducerVisitReport GetReport(int id)
        {
            var spvr = _connection.Get<StoredProducerVisitReport>(id);
            return Hydrated(spvr);
        }

        /// <inheritdoc/>
        public int Count
        {
            get
            {
                return _connection.Table<StoredProducerVisitReport>().Count();
            }
        }

        /// <inheritdoc/>
        public void ReportUploaded(int id)
        {
            var report = _connection.Get<StoredProducerVisitReport>(id);
            report.Uploaded = true;
            _connection.Update(report);
        }
        #endregion
    }
}
