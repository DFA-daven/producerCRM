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
        private readonly ISQLiteConnection _localSQLiteConnection;

        /// <summary>Opens the SQLite database; 
        /// if needed create an instance of an SQLite database; 
        /// if needed create object tables.
        /// </summary>
        /// <param name="factory">The <see cref="ISQLiteConnectionFactory"/>.</param>
        /// <param name="userIdentityService">The <see cref="IUserIdentityService"/>.</param>
        public DataService(ISQLiteConnectionFactory factory, IUserIdentityService userIdentityService)
        {
            string databaseFilename = "local.sql";
            _localSQLiteConnection = factory.Create(databaseFilename);

            // these tables are used for recording new visits, and visits retrieved from the web service
            _localSQLiteConnection.CreateTable<StoredProducerVisitReport>();
            _localSQLiteConnection.CreateTable<ReasonCode>();
            _localSQLiteConnection.CreateTable<VisitXReason>();

            // Review: does it make more sense to have these in an XML file?
            // these tables are used for populating the pull-downs on a report
            _localSQLiteConnection.CreateTable<CallType>();
            _localSQLiteConnection.CreateTable<EmailRecipient>();

            // initialize the UserIdentity -- local XML, and record in cloud
            _userIdentityService = userIdentityService;
        }

        /// <summary>Opens the SQLite database, adds <see cref="ReasonCode[]"/> to the <see cref="Models.StoredProducerVisitReport"/>, and 
        /// returns a <see cref="ProducerVisitReport"/>.
        /// </summary>
        /// <param name="storedProducerVisitReport">A <see cref="StoredProducerVisitReport"/>.</param>
        /// <returns>A <see cref="ProducerVisitReport"/> based on a <see cref="StoredProducerVisitReport"/>.</returns>
        /// <remarks>Opens the <see cref="DataService._localSQLiteConnection"/>, queries the <see cref="VisitXReason"/> table for the given
        /// <see cref="StoredProducerVisitReport"/> ID, matches the VisitXReason.ReasonIDs against the <see cref="ReasonCode"/> table
        /// to get a <see cref="ReasonCode"/>[], and returns the StoredProducerVisitReport.Hydrate(reasonCodes), aka a <see cref="ProducerVisitReport"/>.</remarks>
        private ProducerVisitReport Hydrated(StoredProducerVisitReport storedProducerVisitReport)
        {
            List<VisitXReason> vxrs = _localSQLiteConnection.Table<VisitXReason>().Where(vxr => vxr.VisitID == storedProducerVisitReport.ID).ToList();
            List<int> reasonids = vxrs.Select(vxr => vxr.ReasonID).ToList();
            ReasonCode[] reasonCodes = _localSQLiteConnection.Table<ReasonCode>().ToList().Where(rc => reasonids.Contains(rc.ID)).ToArray();
            return storedProducerVisitReport.Hydrate(reasonCodes);
        }

        #region Required Definitions
        /// <inheritdoc/>
        public List<ProducerVisitReport> ToUpload()
        {
            var stored = _localSQLiteConnection.Table<StoredProducerVisitReport>()
                                .Where(x => x.Uploaded == false)
                                .ToList();
            return stored.Select(Hydrated).ToList();
        }

        /// <inheritdoc/>
        public List<ReportListItem> Recent()
        {
            // FixMe: change quantity to a .resx value (or an XML entry)
            int quantity = 100;
            quantity = 20;

            var storedProducerVisitReports = _localSQLiteConnection.Table<StoredProducerVisitReport>()
                .OrderByDescending(producerVisitReport => producerVisitReport.VisitDate)
                .Take(quantity)
                .ToList();

            //string currentUserEmail = _userIdentityService.GetIdentity().UserEmail;

            //if (string.IsNullOrWhiteSpace(currentUserEmail)) 
            //{
            //    currentUserEmail = "unknown";
            //}
            

            return storedProducerVisitReports.Select(storedProducerVisitReport =>
                {
                    var producerVisitReport = Hydrated(storedProducerVisitReport);
                    // review: would a reason code ever not be found? is "other" the right thing to show?

                    //UserEmail = _userIdentityService.IdentityRecorded ? "identityTrue" : "identityFalse",
                    //UserEmail = _userIdentityService.IdentityRecorded ? _userIdentityService.GetIdentity().UserEmail : "You",
                    //UserEmail = currentUserEmail,


                    return new ReportListItem
                    {
                        ID = storedProducerVisitReport.ID,
                        UserEmail = _userIdentityService.IdentityRecorded ? _userIdentityService.GetIdentity().UserEmail : "You",
                        MemberNumber = producerVisitReport.MemberNumber,
                        Local = true,
                        PrimaryReasonCode = producerVisitReport.ReasonCodes != null && producerVisitReport.ReasonCodes.Length > 0 ? producerVisitReport.ReasonCodes[0] : new ReasonCode { Name = "Other" },
                        VisitDate = producerVisitReport.VisitDate,
                        Uploaded = storedProducerVisitReport.Uploaded
                    };
                }).ToList();
        }

        /// <inheritdoc/>
        public void Insert(ProducerVisitReport report)
        {
            var storedProducerVisitReport = new StoredProducerVisitReport(report);
            _localSQLiteConnection.Insert(storedProducerVisitReport);
            foreach (var reasonCode in report.ReasonCodes)
            {
                var vxr = new VisitXReason
                {
                    ReasonID = reasonCode.ID,
                    VisitID = storedProducerVisitReport.ID,
                };
                _localSQLiteConnection.Insert(vxr);
            }
        }

        /// <inheritdoc/>
        public ProducerVisitReport GetHydratedReport(int id)
        {
            var storedProducerVisitReport = _localSQLiteConnection.Get<StoredProducerVisitReport>(id);
            return Hydrated(storedProducerVisitReport);
        }

        /// <inheritdoc/>
        public int Count
        {
            get
            {
                return _localSQLiteConnection.Table<StoredProducerVisitReport>().Count();
            }
        }

        /// <inheritdoc/>
        public void ReportUploaded(int id)
        {
            var report = _localSQLiteConnection.Get<StoredProducerVisitReport>(id);
            report.Uploaded = true;
            _localSQLiteConnection.Update(report);
        }


        /// <inheritdoc/>
        public List<ReasonCode> GetSQLiteReasonCodes()
        {
            var reasons = _localSQLiteConnection.Table<ReasonCode>().ToList();
            return reasons;
        }

        /// <inheritdoc/>
        public void UpdateSQLiteReasonCodes(List<ReasonCode> reasonCodes)
        {
            // drop the existing table
            _localSQLiteConnection.DropTable<ReasonCode>();
            _localSQLiteConnection.CreateTable<ReasonCode>();
            _localSQLiteConnection.InsertAll(reasonCodes); 
        }





        #endregion
    }
}
