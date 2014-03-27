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
            ViewBag.VRCount = _db.ProducerVisitReports.Count();
            ViewBag.UserCount = _db.UserIdentities.Count();
            ViewBag.UniqueUsers = _db.UserIdentities.Distinct().Count();
            // ToDo: add more reports elements here

            return View();
        }

        public ActionResult Summary()
        {
            ViewBag.VRCount = _db.ProducerVisitReports.Count();
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

            var spvrs = _db.ProducerVisitReports.Where(vr => vr.FarmNumber == id)
                .OrderByDescending(vr => vr.VisitDate)
                .Take(quantity)
                .ToList();

            var rlis = spvrs.Select(spvr =>
            {
                var pvr = Hydrated(spvr);
                var userID = _db.UserIdentities.FirstOrDefault(uid => uid.DeviceID == spvr.UserID);
                return new ReportListItem
                {
                    ID = spvr.ID,
                    UserEmail = (userID ?? new UserIdentity { UserEmail = "Unknown" }).UserEmail,
                    FarmNumber = pvr.FarmNumber,
                    Local = false,
                    PrimaryReasonCode = pvr.ReasonCodes[0],
                    VisitDate = pvr.VisitDate,
                    Uploaded = true
                };
            }).ToList();

            return Json(rlis, JsonRequestBehavior.AllowGet);
        }

        public ActionResult All(string id)
        {
            var spvrs = _db.ProducerVisitReports.Where(vr => vr.FarmNumber == id)
                .OrderByDescending(vr => vr.VisitDate).ToList();

            var rlis = spvrs.Select(spvr =>
            {
                var pvr = Hydrated(spvr);
                return new ReportListItem
                {
                    ID = spvr.ID,
                    UserEmail = _db.UserIdentities.First(uid => uid.DeviceID == spvr.UserID).UserEmail,
                    FarmNumber = pvr.FarmNumber,
                    Local = false,
                    PrimaryReasonCode = pvr.ReasonCodes[0],
                    VisitDate = pvr.VisitDate,
                    Uploaded = true
                };
            }).ToList();

            return Json(rlis, JsonRequestBehavior.AllowGet);
        }

        /// <summary>Opens the <see cref="_db"/>, adds a <see cref="ReasonCode"/>[], and 
        /// returns a <see cref="ProducerVisitReport"/>.
        /// </summary>
        /// <param name="spvr">A <see cref="StoredProducerVisitReport"/>.</param>
        /// <returns>A <see cref="ProducerVisitReport"/> based on a <see cref="StoredProducerVisitReport"/>.</returns>
        /// <remarks>Opens the <see cref="BackEnd.Models.VisitContext"/> connection, queries the <see cref="VisitXReason"/> table for the given
        /// <see cref="StoredProducerVisitReport"/> ID, matches the VisitXReason.ReasonIDs against the <see cref="ReasonCode"/> table
        /// to get a <see cref="ReasonCode"/>[], and returns the StoredProducerVisitReport.Hydrate(reasonCodes), aka a <see cref="ProducerVisitReport"/>.</remarks>
        private ProducerVisitReport Hydrated(StoredProducerVisitReport spvr)
        {
            var vxrs = _db.VisitXReason.Where(vxr => vxr.VisitID == spvr.ID).ToList();
            var ids = vxrs.Select(vxr => vxr.ReasonID).ToList();
            var rcs = _db.ReasonCodes.Where(rc => ids.Contains(rc.ID)).ToArray();
            return spvr.Hydrate(rcs);
        }

        [HttpPost]
        public ActionResult Log(ProducerVisitReport report)
        {
            report.ID = 0;
            var spvr = new StoredProducerVisitReport(report);
            _db.ProducerVisitReports.Add(spvr);
            _db.SaveChanges();
            if (report.ReasonCodes != null)
            {
                foreach (var rc in report.ReasonCodes)
                {
                    _db.VisitXReason.Add(new VisitXReason {ReasonID = rc.ID, VisitID = spvr.ID});
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
                    new ReasonCode {Name = "Regulatory - Inspection", Code = 51},
                    new ReasonCode {Name = "Regulatory - Hi-Count / Antibiotic", Code = 11},
                    new ReasonCode {Name = "Regulatory - Calibrations", Code = 12},
                    new ReasonCode {Name = "Regulatory - Samples", Code = 15},
                    new ReasonCode {Name = "Membership - Relationship Call", Code = 55},
                    new ReasonCode {Name = "Membership - Solicitation", Code = 56},
                    new ReasonCode {Name = "Membership - Cancellation", Code = 50},
                    new ReasonCode {Name = "Gold Standard III", Code = 40},
                    new ReasonCode {Name = "Farm Services - Agri-Max", Code = 31},
                    new ReasonCode {Name = "Farm Services - ASA - Insurance", Code = 32},
                    new ReasonCode {Name = "Farm Services - Dairy One", Code = 33},
                    new ReasonCode {Name = "Farm Services - Eagle - Farm Supplies", Code = 34},
                    new ReasonCode {Name = "Farm Services - Empire Livestock", Code = 35},
                    new ReasonCode {Name = "Farm Services - Risk Management", Code = 36},
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
            // change to EmailRecipients.
            return Json(new List<string>(new[]
                {
                    "Farm Visit",
                    "Phone Call",
                    "Email",
                    "Farm Show",
                    "Other"
                }), JsonRequestBehavior.AllowGet);
        }

        /// <summary>Creates the list of Email Recipients.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List<string> of email recipients.</returns>
        public ActionResult EmailRecipients(string id)
        {
            // undone: can't this be pulled from the XML file?
            return Json(new List<string>(new[]
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
            }), JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
