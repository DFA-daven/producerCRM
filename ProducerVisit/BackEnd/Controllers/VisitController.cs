namespace BackEnd.Controllers
{
    using BackEnd.Models;
    using CallForm.Core.Models;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    public class VisitController : Controller
    {
        /// <inheritdoc/>
        private readonly VisitContext _webDatabaseConnection = new VisitContext();

        public ActionResult Index()
        {
            ViewBag.VisitReportCount = _webDatabaseConnection.ProducerVisitReports.Count();
            ViewBag.UserCount = _webDatabaseConnection.UserIdentities.Count();
            ViewBag.UniqueUsers = _webDatabaseConnection.UserIdentities.Distinct().Count();

            string source = string.Empty;
            source = _webDatabaseConnection.Database.Connection.DataSource;
            if (_webDatabaseConnection.Database.Connection.DataSource.Contains(":"))
            {
                // datasource could be tcp:server.name.net,1433
                source = _webDatabaseConnection.Database.Connection.DataSource.Split(':')[1];
            }

            ViewBag.DatabaseSource = source.Split('.')[0]; // just the left-most part of the address
            ViewBag.DatabaseSource = source.Split(',')[0]; // drop the port number, in case the address was only the machine name
            ViewBag.Database = _webDatabaseConnection.Database.Connection.Database;

            // ToDo: add more reports elements here

            return View();
        }

        public ActionResult Summary()
        {
            ViewBag.VisitReportCount = _webDatabaseConnection.ProducerVisitReports.Count();
            ViewBag.UserCount = 99;
            ViewBag.UniqueUsers = 99;
            // ToDo: add more reports elements here

            return View();
        }

        /// <summary>Get the 100 most recent <see cref="StoredProducerVisitReport">ProducerVisitReports</see>/> for a given member number.
        /// </summary>
        /// <param name="id">The 8 digit Member Number.</param>
        /// <returns>A <see cref="ReportListItem"/> object representing the set of records.</returns>
        public ActionResult Recent(string id)
        {
            // FixMe: change this to a .resx value (or an XML entry)
            int quantity = 100;

            var storedProducerVisitReports = _webDatabaseConnection.ProducerVisitReports.Where(visitReport => visitReport.MemberNumber == id)
                .OrderByDescending(visitReport => visitReport.VisitDate)
                .Take(quantity)
                .ToList();

            var reportListItems = storedProducerVisitReports.Select(storedProducerVisitReport =>
            {
                var producerVisitReport = Hydrated(storedProducerVisitReport);
                var userID = _webDatabaseConnection.UserIdentities.FirstOrDefault(uid => uid.DeviceID == storedProducerVisitReport.UserID);
                return new ReportListItem
                {
                    ID = storedProducerVisitReport.ID,
                    UserEmail = (userID ?? new UserIdentity { UserEmail = "Unknown" }).UserEmail,
                    MemberNumber = producerVisitReport.MemberNumber,
                    Local = false,
                    PrimaryReasonCode = producerVisitReport.ReasonCodes[0],
                    VisitDate = producerVisitReport.VisitDate,
                    Uploaded = true
                };
            }).ToList();

            return Json(reportListItems, JsonRequestBehavior.AllowGet);
        }

        public ActionResult All(string id)
        {
            var storedProducerVisitReports = _webDatabaseConnection.ProducerVisitReports.Where(visitReport => visitReport.MemberNumber == id)
                .OrderByDescending(visitReport => visitReport.VisitDate).ToList();

            var reportListItems = storedProducerVisitReports.Select(storedProducerVisitReport =>
            {
                var producerVisitReport = Hydrated(storedProducerVisitReport);
                return new ReportListItem
                {
                    ID = storedProducerVisitReport.ID,
                    UserEmail = _webDatabaseConnection.UserIdentities.First(uid => uid.DeviceID == storedProducerVisitReport.UserID).UserEmail,
                    MemberNumber = producerVisitReport.MemberNumber,
                    Local = false,
                    PrimaryReasonCode = producerVisitReport.ReasonCodes[0],
                    VisitDate = producerVisitReport.VisitDate,
                    Uploaded = true
                };
            }).ToList();

            return Json(reportListItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>Opens the <see cref="_webDatabaseConnection"/>, adds a <see cref="ReasonCode"/>[], and 
        /// returns a <see cref="ProducerVisitReport"/>.
        /// </summary>
        /// <param name="storedProducerVisitReport">A <see cref="StoredProducerVisitReport"/>.</param>
        /// <returns>A <see cref="ProducerVisitReport"/> based on a <see cref="StoredProducerVisitReport"/>.</returns>
        /// <remarks>Opens the <see cref="BackEnd.Models.VisitContext"/> connection, queries the <see cref="VisitXReason"/> table for the given
        /// <see cref="StoredProducerVisitReport"/> ID, matches the VisitXReason.ReasonIDs against the <see cref="ReasonCode"/> table
        /// to get a <see cref="ReasonCode"/>[], and returns the StoredProducerVisitReport.Hydrate(reasonCodes), aka a <see cref="ProducerVisitReport"/>.</remarks>
        private ProducerVisitReport Hydrated(StoredProducerVisitReport storedProducerVisitReport)
        {
            var vxrs = _webDatabaseConnection.VisitXReasons.Where(vxr => vxr.VisitID == storedProducerVisitReport.ID).ToList();
            var ids = vxrs.Select(vxr => vxr.ReasonID).ToList();
            var rcs = _webDatabaseConnection.ReasonCodes.Where(rc => ids.Contains(rc.ID)).ToArray();
            return storedProducerVisitReport.Hydrate(rcs);
        }

        [HttpPost]
        public ActionResult Log(ProducerVisitReport report)
        {
            report.ID = 0;
            var storedProducerVisitReport = new StoredProducerVisitReport(report);
            _webDatabaseConnection.ProducerVisitReports.Add(storedProducerVisitReport);
            _webDatabaseConnection.SaveChanges();
            if (report.ReasonCodes != null)
            {
                foreach (var rc in report.ReasonCodes)
                {
                    _webDatabaseConnection.VisitXReasons.Add(new VisitXReason {ReasonID = rc.ID, VisitID = storedProducerVisitReport.ID});
                }
                _webDatabaseConnection.SaveChanges();
            }
            return Content("Success");
        }

        [HttpPost]
        public ActionResult Identity(UserIdentity report)
        {
            _webDatabaseConnection.UserIdentities.Add(report);
            _webDatabaseConnection.SaveChanges();
            return Content("Success");
        }

        /// <summary>Gets the list of Reason Codes from the web service.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A <see cref="List{T}"/> of the Reason Codes.</returns>
        public ActionResult Reasons(string id)
        {
            List<ReasonCode> reasonCodeList = new List<ReasonCode>(new[]
            {
                    new ReasonCode {Name = "VstCntrllr: initialized", Code = -1},
                });
                    
            // check remote
            if (!_webDatabaseConnection.ReasonCodes.Any())
                {
                reasonCodeList.Add(new ReasonCode { Name = "VstCntrllr: no ReasonCodes on web dB", Code = -1 });
                }
            else
            {
                reasonCodeList = _webDatabaseConnection.ReasonCodes.ToList();
            }

            reasonCodeList.Add(new ReasonCode { Name = "count is " + reasonCodeList.Count() });

            return Json(reasonCodeList, JsonRequestBehavior.AllowGet); 
        }

        /// <summary>Gets a <see cref="ProducerVisitReport"/> based on the 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Report(string id)
        {
            return Json(Hydrated(_webDatabaseConnection.ProducerVisitReports.Find(int.Parse(id))), JsonRequestBehavior.AllowGet);
        }

        /// <summary>Gets the list of Call Types from the web service.
        /// </summary>
        /// <param name="id"></param>
        /// <returns><see cref="List{T}"/> of <see cref="CallType"/>.</returns>
        public ActionResult CallTypes(string id)
        {
            List<CallType> callTypeList = new List<CallType>(new[]
                {
                    new CallType {Name = "VstCntrllr: initialized"},
                });

            if (!_webDatabaseConnection.CallTypes.Any())
            {
                callTypeList.Clear();
                callTypeList.Add(new CallType { Name = "VstCntrllr: no CallTypes on web dB" });
            }
            else
                {
                callTypeList = _webDatabaseConnection.CallTypes.ToList();
            }

            return Json(callTypeList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>Gets the list of Email Recipients from the web service.
        /// </summary>
        /// <param name="id"></param>
        /// <returns><see cref="List{T}"/> of <see cref="EmailRecipient"/>.</returns>
        public ActionResult EmailRecipients(string id)
        {
            List<EmailRecipient> objectList = new List<EmailRecipient>(new[]
                {
                    new EmailRecipient { DisplayName = "VstCntrllr: initialized"},
                });

            if (!_webDatabaseConnection.EmailRecipients.Any())
            {
                objectList.Clear();
                objectList.Add(new EmailRecipient { DisplayName = "VstCntrllr: no EmailRecipients on web dB" });
            }
            else
            {
                objectList = _webDatabaseConnection.EmailRecipients.ToList();
            }

            return Json(objectList, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            _webDatabaseConnection.Dispose();
            base.Dispose(disposing);
        }
    }
}
