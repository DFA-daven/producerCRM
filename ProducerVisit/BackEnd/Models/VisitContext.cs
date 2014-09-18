namespace BackEnd.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Web;
    using CallForm.Core.Models;
    using System.Web.Services;

    /// <summary>Class for creating a new <c>ProducerCrmVisitContext</c>, inherits from <see cref="DbContext"/>.
    /// </summary>
    public class ProducerCrmVisitContext : DbContext
    {
        // Note: Web.*.config will automatically assign the database connection for the web service 
        // based on the currently selected Solution Configuration. To change the database, you must 
        // select the target Solution Configuration, rebuild the solution, Publish BackEnd, and deploy the mobile app.

        /// <summary>The (BackEnd) database connection information.
        /// </summary>
        /// <remarks>Web.*.config will automatically assign the database connection for the web service 
        /// based on the currently selected Solution Configuration. To change the database, you must 
        /// select the target Solution Configuration, rebuild the solution, Publish BackEnd, 
        /// and deploy the mobile app.</remarks>
        //private static string buildConfiguration = "SolutionConfigurationConnection";
        private static string buildConfiguration = "DefaultConnection";

        /// <summary>Opens a connection to a database using the definition in Web.Config.
        /// </summary>
        /// <remarks>The specific web.*.config file is selected when the BackEnd project is published.</remarks>
        public ProducerCrmVisitContext()
            : base(buildConfiguration)
        {
            // review: initializes database (BackEnd) when called from .pubxml

            // Note: buildConfiguration 1) must be defined in Web.*.config; 2) the catalog defined in the 
            // connection string will be created when BackEnd is published; 3) you must verify that
            // deployed app is actually using the same database.
        }

        /// <summary>The collection of <see cref="CallTypes"/>.
        /// </summary>
        public DbSet<CallType> CallTypes { get; set; }

        /// <summary>The collection of <see cref="EmailRecipient"/>.
        /// </summary>
        public DbSet<EmailRecipient> EmailRecipients { get; set; }

        /// <summary>The collection of <see cref="ReasonCode"/>.
        /// </summary>
        public DbSet<ReasonCode> ReasonCodes { get; set; }

        /// <summary>The collection of <see cref="StoredProducerVisitReport"/>.
        /// </summary>
        /// <remarks>These records do not contain <see cref="ReasonCodes"/></remarks>
        public DbSet<StoredProducerVisitReport> StoredProducerVisitReports
        {
            get;
            set;
        }

        /// <summary>The collection of <see cref="UserIdentities"/>.
        /// </summary>
        public DbSet<UserIdentity> UserIdentities { get; set; }

        /// <summary>The collection of <see cref="VisitXReasons"/>.
        /// </summary>
        public DbSet<VisitXReason> VisitXReasons { get; set; }
    }

    /// <summary>Class for creating a new <c>EnterpriseContext</c>, inherits from <see cref="DbContext"/>.
    /// </summary>
    public class EnterpriseContext : DbContext
    {
        // Note: Web.*.config will automatically assign the database connection for the web service 
        // based on the currently selected Solution Configuration. To change the database, you must 
        // select the target Solution Configuration, rebuild the solution, Publish BackEnd, and deploy the mobile app.

        /// <summary>The (BackEnd) database connection information.
        /// </summary>
        /// <remarks>Web.*.config will automatically assign the database connection for the web service 
        /// based on the currently selected Solution Configuration. To change the database, you must 
        /// select the target Solution Configuration, rebuild the solution, Publish BackEnd, 
        /// and deploy the mobile app.</remarks>
        //private static string buildConfiguration = "SolutionConfigurationConnection";
        private static string buildConfiguration = "DefaultConnection";
        //private static string buildConfiguration = "EnterpriseConnection";

        /// <summary>Opens a connection to a database using the definition in Web.Config.
        /// </summary>
        /// <remarks>The specific web.*.config file is selected when the BackEnd project is published.</remarks>
        public EnterpriseContext()
            : base(buildConfiguration)
        {
            // review: initializes database (BackEnd) when called from .pubxml

            // Note: buildConfiguration 1) must be defined in Web.*.config; 2) the catalog defined in the 
            // connection string will be created when BackEnd is published; 3) you must verify that
            // deployed app is actually using the same database.
        }

        /// <summary>The collection of <see cref="CallTypes"/>.
        /// </summary>
        public DbSet<CallType> CallTypes { get; set; }

        /// <summary>The collection of <see cref="EmailRecipient"/>.
        /// </summary>
        public DbSet<EmailRecipient> EmailRecipients { get; set; }

        /// <summary>The collection of <see cref="ReasonCode"/>.
        /// </summary>
        public DbSet<ReasonCode> ReasonCodes { get; set; }

        /// <summary>The collection of <see cref="StoredProducerVisitReport"/>.
        /// </summary>
        /// <remarks>These records do not contain <see cref="ReasonCodes"/></remarks>
        public DbSet<StoredProducerVisitReport> StoredProducerVisitReports
        {
            get;
            set;
        }

        /// <summary>The collection of <see cref="UserIdentities"/>.
        /// </summary>
        public DbSet<UserIdentity> UserIdentities { get; set; }

        /// <summary>The collection of <see cref="VisitXReasons"/>.
        /// </summary>
        public DbSet<VisitXReason> VisitXReasons { get; set; }

        
    }

    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class Service1 : System.Web.Services.WebService
    {
        [WebMethod]
        public string GetDataLINQ()
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
                catch (Exception ex)
                {

                }
            }

            try
            {
                EnterpriseContext dc = new EnterpriseContext();
                //var command = dc.usp_dequeueTestProject();
                var command = dc.CallTypes;
                string value = command.Select(c => c.Command).FirstOrDefault();
                return value;
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}