namespace CallForm.Core.Services
{
    using CallForm.Core.Models;
    using Cirrious.MvvmCross.Plugins.Sqlite;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>Implements the <see cref="IDataService"/> interface.
    /// </summary>
    public class DataService : IDataService
    {
        /// <inheritdoc/>
        private readonly IUserIdentityService _userIdentityService;
        
        /// <summary>The connection to the SQLite database.
        /// </summary>
        private readonly ISQLiteConnection _localConnection;

        /// <summary>Opens the SQLite database; 
        /// if needed create an instance of an SQLite database; 
        /// if needed create tables: StoredProducerVisitReport, VisitXReason, and ReasonCode; 
        /// add 
        /// </summary>
        /// <param name="factory">The <see cref="ISQLiteConnectionFactory"/>.</param>
        /// <param name="userIdentityService">The <see cref="IUserIdentityService"/>.</param>
        public DataService(ISQLiteConnectionFactory factory, IUserIdentityService userIdentityService)
        {
            string address = "local.sql";
            _localConnection = factory.Create(address);

            // create a table of type StoredProducerVisitReport
            _localConnection.CreateTable<StoredProducerVisitReport>();

            // create a table of type VisitXReason
            _localConnection.CreateTable<VisitXReason>();

            // create a table of type ReasonCode
            _localConnection.CreateTable<ReasonCode>();

            // create a table of type NewEmailRecipient
            _localConnection.CreateTable<NewEmailRecipient>();
        }

        /// <summary>Opens the SQLite database, adds <see cref="ReasonCode[]"/> to the <see cref="Models.StoredProducerVisitReport"/>, and 
        /// returns a <see cref="ProducerVisitReport"/>.
        /// </summary>
        /// <param name="storedProducerVisitReport">A <see cref="StoredProducerVisitReport"/>.</param>
        /// <returns>A <see cref="ProducerVisitReport"/> based on a <see cref="StoredProducerVisitReport"/>.</returns>
        /// <remarks>Opens the <see cref="DataService._localConnection"/>, queries the <see cref="VisitXReason"/> table for the given
        /// <see cref="StoredProducerVisitReport"/> ID, matches the VisitXReason.ReasonIDs against the <see cref="ReasonCode"/> table
        /// to get a <see cref="ReasonCode"/>[], and returns the StoredProducerVisitReport.Hydrate(reasonCodes), aka a <see cref="ProducerVisitReport"/>.</remarks>
        private ProducerVisitReport Hydrated(StoredProducerVisitReport storedProducerVisitReport)
        {
            List<VisitXReason> vxrs = _localConnection.Table<VisitXReason>().Where(vxr => vxr.VisitID == storedProducerVisitReport.ID).ToList();
            List<int> reasonids = vxrs.Select(vxr => vxr.ReasonID).ToList();
            ReasonCode[] reasonCodes = _localConnection.Table<ReasonCode>().ToList().Where(rc => reasonids.Contains(rc.ID)).ToArray();
            return storedProducerVisitReport.Hydrate(reasonCodes);
        }

        #region Required Definitions
        /// <inheritdoc/>
        public List<ProducerVisitReport> ToUpload()
        {
            var stored = _localConnection.Table<StoredProducerVisitReport>()
                                .Where(x => x.Uploaded == false)
                                .ToList();
            return stored.Select(Hydrated).ToList();
        }

        /// <inheritdoc/>
        public List<ReportListItem> Recent()
        {
            // FixMe: change quantity to a .resx value (or an XML entry)
            int quantity = 100;

            var storedProducerVisitReports = _localConnection.Table<StoredProducerVisitReport>()
                .OrderByDescending(producerVisitReport => producerVisitReport.VisitDate)
                .Take(quantity)
                .ToList();

            return storedProducerVisitReports.Select(storedProducerVisitReport =>
                {
                    var producerVisitReport = Hydrated(storedProducerVisitReport);
                    // review: would a reason code ever not be found? is "other" the right thing to show?
                    return new ReportListItem
                    {
                        ID = storedProducerVisitReport.ID,
                        UserEmail = _userIdentityService.IdentityRecorded ? _userIdentityService.GetIdentity().UserEmail : "You",
                        MemberNumber = producerVisitReport.MemberNumber,
                        Local = true,
                        PrimaryReasonCode = producerVisitReport.ReasonCodes != null && producerVisitReport.ReasonCodes.Length > 0 ? producerVisitReport.ReasonCodes[0] : new ReasonCode { Name = "Other", Code = -1 },
                        VisitDate = producerVisitReport.VisitDate,
                        Uploaded = storedProducerVisitReport.Uploaded
                    };
                }).ToList();
        }

        /// <inheritdoc/>
        public void Insert(ProducerVisitReport report)
        {
            var storedProducerVisitReport = new StoredProducerVisitReport(report);
            _localConnection.Insert(storedProducerVisitReport);
            foreach (var reasonCode in report.ReasonCodes)
            {
                var vxr = new VisitXReason
                {
                    ReasonID = reasonCode.ID,
                    VisitID = storedProducerVisitReport.ID,
                };
                _localConnection.Insert(vxr);
            }
        }

        /// <inheritdoc/>
        public ProducerVisitReport GetReport(int id)
        {
            var storedProducerVisitReport = _localConnection.Get<StoredProducerVisitReport>(id);
            return Hydrated(storedProducerVisitReport);
        }

        /// <inheritdoc/>
        public int Count
        {
            get
            {
                return _localConnection.Table<StoredProducerVisitReport>().Count();
            }
        }

        /// <inheritdoc/>
        public void ReportUploaded(int id)
        {
            var report = _localConnection.Get<StoredProducerVisitReport>(id);
            report.Uploaded = true;
            _localConnection.Update(report);
        }

        /// <inheritdoc/>
        public List<ReasonCode> GetReasonsForCall()
        {
            var reasons = _localConnection.Table<ReasonCode>().ToList();
            return reasons;
        }

        /// <inheritdoc/>
        public void UpdateReasons(List<ReasonCode> reasonCodes)
        {
            // drop the existing table

            // BROKEN: DropTable? Seriously?
            _localConnection.DropTable<ReasonCode>();
            _localConnection.CreateTable<ReasonCode>();
            _localConnection.InsertAll(reasonCodes);
        }

        /// <inheritdoc/>
        public List<NewEmailRecipient> GetEmailAddressesAndNames()
        {
            var recipients = _localConnection.Table<NewEmailRecipient>().ToList();
            return recipients;
        }

        /// <inheritdoc/>
        public void UpdateRecipients(List<NewEmailRecipient> recipients)
        {
            // drop the existing table

            // BROKEN: DropTable? Seriously?
            _localConnection.DropTable<NewEmailRecipient>();
            _localConnection.CreateTable<NewEmailRecipient>();
            _localConnection.InsertAll(recipients);
        }


        #endregion
    }
}
