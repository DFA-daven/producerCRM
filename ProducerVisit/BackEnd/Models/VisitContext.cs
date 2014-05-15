using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using CallForm.Core.Models;

namespace BackEnd.Models
{
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
        private static string buildConfiguration = "SolutionConfigurationConnection";

        /// <summary>Opens a connection to a database using the definition in Web.Config.
        /// </summary>
        /// <remarks>The specific web.*.config file is selected when the BackEnd project is published.</remarks>
        public VisitContext()
            : base(buildConfiguration)
        {
            // review: initializes database (BackEnd) when called from .pubxml
        }

        /// <summary>The collection of <see cref="StoredProducerVisitReport"/>.
        /// </summary>
        public DbSet<StoredProducerVisitReport> ProducerVisitReports 
        {
            // review: is this correct - isn't it returning *Stored*ProducerVisitReports?
            get; 
            set; 
        }

        /// <summary>The collection of <see cref="ReasonCode"/>.
        /// </summary>
        public DbSet<ReasonCode> ReasonCodes { get; set; }

        /// <summary>The collection of <see cref="VisitXReason"/>.
        /// </summary>
        public DbSet<VisitXReason> VisitXReason { get; set; }

        /// <summary>The collection of <see cref="UserIdentities"/>.
        /// </summary>
        public DbSet<UserIdentity> UserIdentities { get; set; }

        /// <summary>The collection of <see cref="NewEmailRecipient"/>.
        /// </summary>
        public DbSet<NewEmailRecipient> NewEmailRecipients { get; set; }
    }
}