namespace BackEnd.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Web;
    using CallForm.Core.Models;

    /// <summary>Class for creating a new <c>VisitContext</c>, inherits from <see cref="DbContext"/>.
    /// </summary>
    public class VisitContext : DbContext
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
        public VisitContext()
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
}