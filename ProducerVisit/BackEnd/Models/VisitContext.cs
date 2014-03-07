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

        /// <summary>Opens a connection to the database defined in Web.Config.
        /// </summary>
        public VisitContext()
            : base(buildConfiguration)
        {
            // review: initializes database (backend) when called from .pubxml
        }

        public DbSet<StoredProducerVisitReport> ProducerVisitReports { get; set; }
        public DbSet<ReasonCode> ReasonCodes { get; set; }
        public DbSet<VisitXReason> VisitXReason { get; set; }
        public DbSet<UserIdentity> UserIdentities { get; set; }
    }
}