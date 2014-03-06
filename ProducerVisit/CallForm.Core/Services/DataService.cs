﻿namespace CallForm.Core.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using CallForm.Core.Models;
    using Cirrious.MvvmCross.Plugins.Sqlite;

    /// <summary>Creates an object representing 
    /// </summary>
    public class DataService : IDataService
    {
        private readonly IUserIdentityService _userIdentityService;
        
        /// <summary>Creates a connection to an SQLite database.
        /// </summary>
        private readonly ISQLiteConnection _connection;

        public DataService(ISQLiteConnectionFactory factory, IUserIdentityService userIdentityService)
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

        /// <summary>Get the 100 most recent <seealso cref="StoredProducerVisitReport"/>s.
        /// </summary>
        /// <returns>A <seealso cref="List<>"/> of <seealso cref="ReportListItem"/>s.</returns>
        /// <remarks>See <seealso cref="VisitController.Recent()"/>.</remarks>
        public List<ReportListItem> Recent()
        {
            // fixme: change this to a .resx value
            int quantity = 100;

            var spvrs = _connection.Table<StoredProducerVisitReport>()
                .OrderByDescending(pvr => pvr.ID)
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

        /// <summary>Gets the list of Reason Codes.
        /// </summary>
        /// <returns></returns>
        public List<ReasonCode> GetReasonsForCall()
        {
            return _connection.Table<ReasonCode>().ToList();
        }

        /// <summary>Drop the "ReasonCodes" table and replace with <paramref name="reasonCodes"/>.
        /// </summary>
        /// <param name="reasonCodes"></param>
        public void UpdateReasons(List<ReasonCode> reasonCodes)
        {
            // review: is this method ever called?
            _connection.DropTable<ReasonCode>();
            _connection.CreateTable<ReasonCode>();
            _connection.InsertAll(reasonCodes); 
        }

        /// <summary>Given a <seealso cref="ProducerVisitReport"/> (and <seealso cref="ReasonCodes"/>), adds a 
        /// <seealso cref="StoredProducerVisitReport"/> (and <seealso cref="VisitXReason"/>(s)) to the <seealso cref="ISQLiteConnection"/>.
        /// </summary>
        /// <param name="report">A new <seealso cref="ProducerVisitReport"/>.</param>
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

        /// <summary>Given a 
        /// </summary>
        /// <param name="id">An 8-digit member number.</param>
        /// <returns></returns>
        public ProducerVisitReport GetReport(int id)
        {
            var spvr = _connection.Get<StoredProducerVisitReport>(id);
            return Hydrated(spvr);
        }

        /// <summary>The number of records in the <seealso cref="StoredProducerVisitReport"/> table.
        /// </summary>
        public int Count
        {
            get
            {
                return _connection.Table<StoredProducerVisitReport>().Count();
            }
        }

        /// <summary>Marks the "uploaded" flag for a given <seealso cref="StoredProducerVisitReport"/>.
        /// </summary>
        /// <param name="id">The ID number of <seealso cref="StoredProducerVisitReport"/>.</param>
        public void ReportUploaded(int id)
        {
            var report = _connection.Get<StoredProducerVisitReport>(id);
            report.Uploaded = true;
            _connection.Update(report);
        }
    }
}
