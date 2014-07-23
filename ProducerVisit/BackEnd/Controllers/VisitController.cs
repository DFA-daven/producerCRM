namespace BackEnd.Controllers
{
    using BackEnd.Models;
    using CallForm.Core.Models;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Common;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    /// <summary>Creates a new <c>VisitController</c> for managing requests to the  web service. Inherits from <see cref="Controller"/>.
    /// </summary>
    public class VisitController : Controller
    {
        /// <summary>The web service connection.
        /// </summary>
        private readonly VisitContext _webProducerCrmDatabaseConnection = new VisitContext();

        //private readonly EntityContext _webMemberDatabaseConnection = new EntityContext();

        /// <summary>Composes the Index (default) page.
        /// </summary>
        /// <returns>A <see cref="Controller.View()"/> (page).</returns>
        public ActionResult Index()
        {
            ViewBag.VisitReportCount = _webProducerCrmDatabaseConnection.ProducerVisitReports.Count();
            ViewBag.UserCount = _webProducerCrmDatabaseConnection.UserIdentities.Count();
            ViewBag.UniqueUsers = _webProducerCrmDatabaseConnection.UserIdentities.Distinct().Count();
            //ViewBag.EntityNumber = _webMemberDatabaseConnection.Database.SqlQuery<string>("mySpName {0}, {1}, {2}", new object[] { param1, param2, param3 });
            //ViewBag.EntityNumber = _webMemberDatabaseConnection.Database.SqlQuery<En>("SELECT TABLE_NAME FROM information_schema.tables WHERE TABLE_NAME LIKE '%{0}%'", new object[] { "Call" });

            string source = string.Empty;
            source = _webProducerCrmDatabaseConnection.Database.Connection.DataSource;
            if (_webProducerCrmDatabaseConnection.Database.Connection.DataSource.Contains(":"))
            {
                // DataSource could be TCP:server.name.net,1433
                source = _webProducerCrmDatabaseConnection.Database.Connection.DataSource.Split(':')[1];
            }

            ViewBag.DatabaseSource = source.Split('.')[0]; // just the left-most part of the address
            ViewBag.DatabaseSource = source.Split(',')[0]; // drop the port number, in case the address was only the machine name
            ViewBag.Database = _webProducerCrmDatabaseConnection.Database.Connection.Database;

            string connectionString = GetConnectionStringByProvider("System.Data.SqlClient");
            DbConnection connection = CreateDbConnection("System.Data.SqlClient", connectionString);
            
            //DbCommand command =  { CommandType = System.Data.CommandType.StoredProcedure, CommandText = "myStoredProcedure", Parameters = new object[] { "22222222" } };

            // ToDo: add more reports elements here


            return View();
        }

        /// <summary>Composes the Summary (default) page.
        /// </summary>
        /// <returns>A <see cref="Controller.View()"/> (page).</returns>
        public ActionResult Summary()
        {
            ViewBag.VisitReportCount = _webProducerCrmDatabaseConnection.ProducerVisitReports.Count();
            ViewBag.UserCount = 99;
            ViewBag.UniqueUsers = 99;
            // ToDo: add more reports elements here

            return View();
        }

        /// <summary>Get the 100 most recent <see cref="StoredProducerVisitReport">ProducerVisitReports</see> for 
        /// a given member number, AND FILTER for just this user.
        /// </summary>
        /// <param name="id">The 8 digit Member Number.</param>
        /// <returns>A <see cref="ReportListItem"/> object representing the set of records.</returns>
        public ActionResult Recent(string id)
        {
            // FixMe: change this to a .resx value (or an XML entry)
            int quantity = 100;

            var storedProducerVisitReports = _webProducerCrmDatabaseConnection.ProducerVisitReports
                .Where(visitReport => visitReport.MemberNumber.Contains(id))
                .OrderByDescending(visitReport => visitReport.VisitDate)
                .Take(quantity)
                .ToList();

            var reportListItems = storedProducerVisitReports.Select(storedProducerVisitReport =>
            {
                var producerVisitReport = Hydrated(storedProducerVisitReport);
                // FixMe: change this to filter using the user's email address instead of userID.
                var userID = _webProducerCrmDatabaseConnection.UserIdentities
                    .FirstOrDefault(uid => uid.DeviceID == storedProducerVisitReport.UserID);
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

        /// <summary>Get all <see cref="StoredProducerVisitReport">ProducerVisitReports</see> for a given member number.
        /// </summary>
        /// <param name="id">The 8 digit Member Number.</param>
        /// <returns>A <see cref="ReportListItem"/> object representing the set of records.</returns>
        public ActionResult All(string id)
        {
            var storedProducerVisitReports = _webProducerCrmDatabaseConnection.ProducerVisitReports.Where(visitReport => visitReport.MemberNumber == id)
                .OrderByDescending(visitReport => visitReport.VisitDate)
                .ToList();

            var reportListItems = storedProducerVisitReports.Select(storedProducerVisitReport =>
            {
                var producerVisitReport = Hydrated(storedProducerVisitReport);
                return new ReportListItem
                {
                    ID = storedProducerVisitReport.ID,
                    UserEmail = _webProducerCrmDatabaseConnection.UserIdentities.First(uid => uid.DeviceID == storedProducerVisitReport.UserID).UserEmail,
                    MemberNumber = producerVisitReport.MemberNumber,
                    Local = false,
                    PrimaryReasonCode = producerVisitReport.ReasonCodes[0],
                    VisitDate = producerVisitReport.VisitDate,
                    Uploaded = true
                };
            }).ToList();

            return Json(reportListItems, JsonRequestBehavior.AllowGet);
        }

        /// <summary>Opens the <see cref="_webProducerCrmDatabaseConnection"/>, adds a <see cref="ReasonCode"/>[], and 
        /// returns a <see cref="ProducerVisitReport"/>.
        /// </summary>
        /// <param name="storedProducerVisitReport">A <see cref="StoredProducerVisitReport"/>.</param>
        /// <returns>A <see cref="ProducerVisitReport"/> based on a <see cref="StoredProducerVisitReport"/>.</returns>
        /// <remarks>Opens the <see cref="BackEnd.Models.VisitContext"/> connection, queries the <see cref="VisitXReason"/> table for the given
        /// <see cref="StoredProducerVisitReport"/> ID, matches the VisitXReason.ReasonIDs against the <see cref="ReasonCode"/> table
        /// to get a <see cref="ReasonCode"/>[], and returns the StoredProducerVisitReport.Hydrate(reasonCodes), aka a <see cref="ProducerVisitReport"/>.</remarks>
        private ProducerVisitReport Hydrated(StoredProducerVisitReport storedProducerVisitReport)
        {
            var vxrs = _webProducerCrmDatabaseConnection.VisitXReasons.Where(vxr => vxr.VisitID == storedProducerVisitReport.ID).ToList();
            var ids = vxrs.Select(vxr => vxr.ReasonID).ToList();
            var rcs = _webProducerCrmDatabaseConnection.ReasonCodes.Where(rc => ids.Contains(rc.ID)).ToArray();
            return storedProducerVisitReport.Hydrate(rcs);
        }

        /// <summary>Creates a new <see cref="StoredProducerVisitReport"/> on the web service.
        /// </summary>
        /// <param name="report">A <see cref="ProducerVisitReport">visit report</see>.</param>
        /// <returns>A "Success" message.</returns>
        /// <remarks>The <c>ToUpLoad()</c> flag is set in <c>ReportUploaded()</c>, which is 
        /// passed the new <see cref="StoredProducerVisitReport"/> ID by <c>ParseResponse</c>.</remarks>
        [HttpPost]
        public ActionResult Log(ProducerVisitReport report)
        {
            report.ID = 0;
            var storedProducerVisitReport = new StoredProducerVisitReport(report);
            _webProducerCrmDatabaseConnection.ProducerVisitReports.Add(storedProducerVisitReport);
            _webProducerCrmDatabaseConnection.SaveChanges();
            if (report.ReasonCodes != null)
            {
                foreach (var rc in report.ReasonCodes)
                {
                    _webProducerCrmDatabaseConnection.VisitXReasons.Add(new VisitXReason {ReasonID = rc.ID, VisitID = storedProducerVisitReport.ID});
                }
                _webProducerCrmDatabaseConnection.SaveChanges();
            }
            return Content("Success");
        }

        [HttpPost]
        public ActionResult Identity(UserIdentity report)
        {
            _webProducerCrmDatabaseConnection.UserIdentities.Add(report);
            _webProducerCrmDatabaseConnection.SaveChanges();
            return Content("Success");
        }

        /// <summary>Gets the list of Reason Codes from the web service.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>A List&lt;<see cref="ReasonCode"/>>.</returns>
        public ActionResult Reasons(string id)
        {
            List<ReasonCode> reasonCodeList = new List<ReasonCode>(new[]
            {
                    new ReasonCode {Name = "VstCntrllr: initialized", Code = -1},
                });
                    
            // check remote
            if (!_webProducerCrmDatabaseConnection.ReasonCodes.Any())
                {
                reasonCodeList.Add(new ReasonCode { Name = "VstCntrllr: no ReasonCodes on web dB", Code = -1 });
                }
            else
            {
                reasonCodeList = _webProducerCrmDatabaseConnection.ReasonCodes.ToList();
            }

            //reasonCodeList.Add(new ReasonCode { Name = "count is " + reasonCodeList.Count() });

            return Json(reasonCodeList, JsonRequestBehavior.AllowGet); 
        }

        /// <summary>Gets a <see cref="ProducerVisitReport"/> based on the 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Report(string id)
        {
            return Json(Hydrated(_webProducerCrmDatabaseConnection.ProducerVisitReports.Find(int.Parse(id))), JsonRequestBehavior.AllowGet);
        }

        /// <summary>Gets the list of Call Types from the web service.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List&lt;<see cref="CallType"/>>.</returns>
        public ActionResult CallTypes(string id)
        {
            List<CallType> callTypeList = new List<CallType>(new[]
                {
                    new CallType {Name = "VstCntrllr: initialized"},
                });

            if (!_webProducerCrmDatabaseConnection.CallTypes.Any())
            {
                callTypeList.Clear();
                callTypeList.Add(new CallType { Name = "VstCntrllr: no CallTypes on web dB" });
            }
            else
                {
                callTypeList = _webProducerCrmDatabaseConnection.CallTypes.ToList();
            }

            return Json(callTypeList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>Gets the list of Email Recipients from the web service.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>List&lt;<see cref="EmailRecipient"/>>.</returns>
        public ActionResult EmailRecipients(string id)
        {
            List<EmailRecipient> objectList = new List<EmailRecipient>(new[]
                {
                    new EmailRecipient { DisplayName = "VstCntrllr: initialized"},
                });

            if (!_webProducerCrmDatabaseConnection.EmailRecipients.Any())
            {
                objectList.Clear();
                objectList.Add(new EmailRecipient { DisplayName = "VstCntrllr: no EmailRecipients on web dB" });
            }
            else
            {
                objectList = _webProducerCrmDatabaseConnection.EmailRecipients.ToList();
            }

            return Json(objectList, JsonRequestBehavior.AllowGet);
        }

        static DbConnection CreateDbConnection(string providerName, string connectionString)
        {
            DbConnection connection = null;

            if (connectionString != null)
            {
                try
                {
                    DbProviderFactory factory = DbProviderFactories.GetFactory(providerName);

                    connection = factory.CreateConnection();
                    connection.ConnectionString = connectionString;
                }
                catch (Exception ex)
                {
                    if (connection != null)
                    {
                        connection = null;
                    }

                    //Console.Writeline(ex.Message);
                }
            }

            return connection;
        }

        static string GetConnectionStringByProvider(string providerName)
        {
            string returnValue = null;

            ConnectionStringSettingsCollection settings = ConfigurationManager.ConnectionStrings;

            if (settings != null)
            {
                foreach (ConnectionStringSettings cs in settings)
                {
                    if (cs.ProviderName == providerName)
                    {
                        returnValue = cs.ConnectionString;
                        break;
                    }
                }
            }

            return returnValue;
        }

        static void ExecuteDbCommand(DbConnection connection)
        {
            if (connection != null)
            {
                using (connection)
                {
                    try
                    {
                        connection.Open();
                        DbCommand command = connection.CreateCommand();
                        command.CommandText = "Select * ";
                        int rows = command.ExecuteNonQuery();

                    }
                    catch (DbException exDb)
                    {

                    }
                    catch(Exception ex)
                    {

                    }
                }
            }
            else
            {
                // connection was null
            }
        }

        protected override void Dispose(bool disposing)
        {
            _webProducerCrmDatabaseConnection.Dispose();
            //_webMemberDatabaseConnection.Dispose();
            base.Dispose(disposing);
        }
    }
}
