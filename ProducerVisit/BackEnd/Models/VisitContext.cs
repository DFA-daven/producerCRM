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
        // review: switching the configuration is handled by Web.Release.config -- probably don't need these preprocessor directives
#if (RELEASE)
        private static string buildConfiguration = "DefaultConnection";
#else
        private static string buildConfiguration = "DefaultConnection";
#endif

        /// <summary>Opens a connection to a database using the definition in Web.Config.
        /// </summary>
        public VisitContext()
            : base(buildConfiguration)
        {
            // review: initializes database (backend) when called from .pubxml
        }

        /// <summary>The collection of <seealso cref="StoredProducerVisitReport"/>.
        /// </summary>
        public DbSet<StoredProducerVisitReport> ProducerVisitReports 
        {
            // review: is this correct - isn't it returning *Stored*ProducerVisitReports?
            get; 
            set; 
        }

        /// <summary>The collection of <seealso cref="ReasonCode"/>.
        /// </summary>
        public DbSet<ReasonCode> ReasonCodes { get; set; }

        /// <summary>The collection of <seealso cref="VisitXReason"/>.
        /// </summary>
        public DbSet<VisitXReason> VisitXReason { get; set; }

        /// <summary>The collection of <seealso cref="UserIdentities"/>.
        /// </summary>
        public DbSet<UserIdentity> UserIdentities { get; set; }
    }
}