namespace BackEnd.Models
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Web;
    using CallForm.Core.Models;

    public class EntityContext : DbContext
    {
        /// <summary>The (BackEnd) database connection information.
        /// </summary>
        private static string entityConfiguration = "EntityConnection";

        /// <summary>Opens a connection to a database using the definition in Web.Config.
        /// </summary>
        public EntityContext()
            : base(entityConfiguration)
        {
            // Note: entityConfiguration must be defined in Web.*.config
        }

        public DbSet<CallType> EntityCallTypes { get; set;  }
    }
}