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
        /// <summary>Opens a connection to the database defined in Web.Config.
        /// </summary>
        public VisitContext()
            : base("DefaultConnection")
        {
            // review: initializes database (backend) when called from .pubxml
        }

        public DbSet<StoredProducerVisitReport> ProducerVisitReports { get; set; }
        public DbSet<ReasonCode> ReasonCodes { get; set; }
        public DbSet<VisitXReason> VisitXReason { get; set; }
        public DbSet<UserIdentity> UserIdentities { get; set; }
    }
}