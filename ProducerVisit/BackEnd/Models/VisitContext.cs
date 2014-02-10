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
        public VisitContext()
            : base("DefaultConnection")
        {
        }

        public DbSet<StoredProducerVisitReport> ProducerVisitReports { get; set; }
        public DbSet<ReasonCode> ReasonCodes { get; set; }
        public DbSet<VisitXReason> VisitXReason { get; set; }
        public DbSet<UserIdentity> UserIdentities { get; set; }
    }
}