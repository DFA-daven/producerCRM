using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BackEnd.Models;
using CallForm.Core.Models;

namespace BackEnd.Controllers
{
    public class VisitController : Controller
    {
        /// <inheritdoc/>
        private readonly VisitContext _db = new VisitContext();

        public ActionResult Index()
        {
            ViewBag.VisitReportCount = _db.ProducerVisitReports.Count();
            ViewBag.UserCount = _db.UserIdentities.Count();
            ViewBag.UniqueUsers = _db.UserIdentities.Distinct().Count();

            string source = string.Empty;
            source = _db.Database.Connection.DataSource;
            if (_db.Database.Connection.DataSource.Contains(":"))
            {
                // datasource could be tcp:server.name.net,1433
                source = _db.Database.Connection.DataSource.Split(':')[1];
            }

            ViewBag.DatabaseSource = source.Split('.')[0]; // just the left-most part of the address
            ViewBag.DatabaseSource = source.Split(',')[0]; // drop the port number, in case the address was only the machine name
            ViewBag.Database = _db.Database.Connection.Database;

            // ToDo: add more reports elements here

            return View();
        }

        public ActionResult Summary()
        {
            ViewBag.VisitReportCount = _db.ProducerVisitReports.Count();
            ViewBag.UserCount = 99;
            ViewBag.UniqueUsers = 99;
            // ToDo: add more reports elements here

            return View();
        }

        /// <summary>Get the 100 most recent <see cref="ProducerVisitReports"/> for a given member number.
        /// </summary>
        /// <param name="id">The 8 digit Member Number.</param>
        /// <returns>A <see cref="ReportListItem"/> object representing the set of records.</returns>
        /// <seealso cref="DataService.Recent()"/>
        public ActionResult Recent(string id)
        {
            // FixMe: change this to a .resx value (or an XML entry)
            int quantity = 100;

            var storedProducerVisitReports = _db.ProducerVisitReports.Where(visitReport => visitReport.MemberNumber == id)
                .OrderByDescending(visitReport => visitReport.VisitDate)
                .Take(quantity)
                .ToList();

            var reportListItems = storedProducerVisitReports.Select(storedProducerVisitReport =>
            {
                var producerVisitReport = Hydrated(storedProducerVisitReport);
                var userID = _db.UserIdentities.FirstOrDefault(uid => uid.DeviceID == storedProducerVisitReport.UserID);
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
            var storedProducerVisitReports = _db.ProducerVisitReports.Where(visitReport => visitReport.MemberNumber == id)
                .OrderByDescending(visitReport => visitReport.VisitDate).ToList();

            var reportListItems = storedProducerVisitReports.Select(storedProducerVisitReport =>
            {
                var producerVisitReport = Hydrated(storedProducerVisitReport);
                return new ReportListItem
                {
                    ID = storedProducerVisitReport.ID,
                    UserEmail = _db.UserIdentities.First(uid => uid.DeviceID == storedProducerVisitReport.UserID).UserEmail,
                    MemberNumber = producerVisitReport.MemberNumber,
                    Local = false,
                    PrimaryReasonCode = producerVisitReport.ReasonCodes[0],
                    VisitDate = producerVisitReport.VisitDate,
                    Uploaded = true
                };
            }).ToList();

            return Json(reportListItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>Opens the <see cref="_db"/>, adds a <see cref="ReasonCode"/>[], and 
        /// returns a <see cref="ProducerVisitReport"/>.
        /// </summary>
        /// <param name="storedProducerVisitReport">A <see cref="StoredProducerVisitReport"/>.</param>
        /// <returns>A <see cref="ProducerVisitReport"/> based on a <see cref="StoredProducerVisitReport"/>.</returns>
        /// <remarks>Opens the <see cref="BackEnd.Models.VisitContext"/> connection, queries the <see cref="VisitXReason"/> table for the given
        /// <see cref="StoredProducerVisitReport"/> ID, matches the VisitXReason.ReasonIDs against the <see cref="ReasonCode"/> table
        /// to get a <see cref="ReasonCode"/>[], and returns the StoredProducerVisitReport.Hydrate(reasonCodes), aka a <see cref="ProducerVisitReport"/>.</remarks>
        private ProducerVisitReport Hydrated(StoredProducerVisitReport storedProducerVisitReport)
        {
            var vxrs = _db.VisitXReason.Where(vxr => vxr.VisitID == storedProducerVisitReport.ID).ToList();
            var ids = vxrs.Select(vxr => vxr.ReasonID).ToList();
            var rcs = _db.ReasonCodes.Where(rc => ids.Contains(rc.ID)).ToArray();
            return storedProducerVisitReport.Hydrate(rcs);
        }

        [HttpPost]
        public ActionResult Log(ProducerVisitReport report)
        {
            report.ID = 0;
            var storedProducerVisitReport = new StoredProducerVisitReport(report);
            _db.ProducerVisitReports.Add(storedProducerVisitReport);
            _db.SaveChanges();
            if (report.ReasonCodes != null)
            {
                foreach (var rc in report.ReasonCodes)
                {
                    _db.VisitXReason.Add(new VisitXReason {ReasonID = rc.ID, VisitID = storedProducerVisitReport.ID});
                }
                _db.SaveChanges();
            }
            return Content("Success");
        }

        [HttpPost]
        public ActionResult Identity(UserIdentity report)
        {
            _db.UserIdentities.Add(report);
            _db.SaveChanges();
            return Content("Success");
        }

        /// <summary>Gets the list of Reason Codes. Creates a new list if needed.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A <see cref="List<ReasonCode>"/> of the Reason Codes.</returns>
        /// <remarks>Checks to see if the web service has a ReasonCodes table. If not, creates one.</remarks>
        public ActionResult Reasons(string id)
        {
            // review: this seems like a better approach to retrieving the data. Use this for the other lists below?
            // ToDo: these should probably be in an .resx
            // note: establishes the reason code list
            if (!_db.ReasonCodes.Any())
            {
                var list = new List<ReasonCode>(new[]
                {
                    new ReasonCode {Name = "VisitController Regulatory: Calibrations", Code = 11},
                    new ReasonCode {Name = "Regulatory: Inspection", Code = 12},
                    new ReasonCode {Name = "Regulatory: Quality: Antibiotic", Code = 15},
                    new ReasonCode {Name = "Regulatory: Quality: Cryo", Code = 16},
                    new ReasonCode {Name = "Regulatory: Quality: Hi-count", Code = 17},
                    new ReasonCode {Name = "Regulatory: Samples", Code = 13},
                    
                    new ReasonCode {Name = "Membership: Cancellation", Code = 59},
                    new ReasonCode {Name = "Membership: Relationship Call", Code = 55},
                    new ReasonCode {Name = "Membership: Solicitation", Code = 56},
                    
                    new ReasonCode {Name = "Gold Standard III", Code = 43},
                    
                    new ReasonCode {Name = "Farm Services: Agri-Max", Code = 31},
                    new ReasonCode {Name = "Farm Services: ASA: Insurance", Code = 32},
                    new ReasonCode {Name = "Farm Services: Dairy One", Code = 33},
                    new ReasonCode {Name = "Farm Services: Eagle: Farm Supplies", Code = 34},
                    new ReasonCode {Name = "Farm Services: Empire Livestock", Code = 35},
                    new ReasonCode {Name = "Farm Services: Risk Management", Code = 36},
                    
                    new ReasonCode {Name = "Other", Code = -1},
                });
                foreach (var reasonCode in list)
                {
                    _db.ReasonCodes.Add(reasonCode);
                }
                _db.SaveChanges();
            }
            return Json(_db.ReasonCodes.ToList(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>Gets a <see cref="ProducerVisitReport"/> based on the 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Report(string id)
        {
            return Json(Hydrated(_db.ProducerVisitReports.Find(int.Parse(id))), JsonRequestBehavior.AllowGet);
        }

        /// <summary>Creates the list of Call Types.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List<string> of Call Types.</returns>
        public ActionResult CallTypes(string id)
        {
            // review: this static list is being used to keep the web service from querying the database. This is better than
            // hard-coding the values into the app, but still requires a re-publish in order to make changes. Apply same
            // change to vcEmailRecipients.
            return Json(new List<string>(new[]
                {
                    "VisitController",
                    "Phone Call",
                    "Email",
                    "Farm Visit",
                    "Farm Show",
                    "SMS (Text Msg.)",
                    "Other"
                }), JsonRequestBehavior.AllowGet);
        }

        /// <summary>Gets the Email Recipients.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List<string> of email recipients.</returns>
        public ActionResult pvrEmailRecipients(string id)
        {
            if (!_db.NewEmailRecipients.Any())
            {
                var list = new List<NewEmailRecipient>(new[]
                {
                    new NewEmailRecipient {Address = "visitcontroller.cs", DisplayName = "provided by method" },
                    //new NewEmailRecipient {Address = "info@agri-maxfinancial.com",                     DisplayName = "info@agri-maxfinancial.com"},
                    //new NewEmailRecipient {Address = "info@agri-servicesagency.com",                   DisplayName = "info@agri-servicesagency.com"},
                    //new NewEmailRecipient {Address = "communications@dairylea.com",                    DisplayName = "Member Communications"},
                    //new NewEmailRecipient {Address = "FieldStaffNotification-DairyOne@DairyOne.com",   DisplayName = "FieldStaffNotification-DairyOne@DairyOne.com"},
                    //new NewEmailRecipient {Address = "FieldStaffNotification-DMS@dairylea.com",        DisplayName = "FieldStaffNotification-DMS@dairylea.com"},
                    //new NewEmailRecipient {Address = "drms@dairylea.com",                              DisplayName = "DRMS"},
                    //new NewEmailRecipient {Address = "FieldStaffNotification-Eagle@dairylea.com",      DisplayName = "FieldStaffNotification-Eagle@dairylea.com"},
                    //new NewEmailRecipient {Address = "FieldStaffNotification-HR@dairylea.com",         DisplayName = "FieldStaffNotification-HR@dairylea.com"},
                    //new NewEmailRecipient {Address = "TechnicalSupport-brittonfield@dairylea.com",     DisplayName = "Technical Support"},
                    //new NewEmailRecipient {Address = "FieldStaffNotification-Membership@dairylea.com", DisplayName = "FieldStaffNotification-Membership@dairylea.com"},
                    //new NewEmailRecipient {Address = "FieldStaffNotification-Payroll@dairylea.com",    DisplayName = "FieldStaffNotification-Payroll@dairylea.com"},
                });
                foreach (var emailRecipient in list)
                {
                    _db.NewEmailRecipients.Add(emailRecipient);
                }
                _db.SaveChanges();
            }
            return Json(_db.NewEmailRecipients.ToList(), JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
