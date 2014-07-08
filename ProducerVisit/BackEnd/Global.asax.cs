using BackEnd.Models;

namespace BackEnd
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
    using System.Linq;
    using System.Text;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;

    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public string EntityType { get; set;}

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            // Note: this is where the "line that must be commented out" used to be. That action is no longer necessary.
            Database.SetInitializer<VisitContext>(new SeededSiteDBInitialize());
            using (var myContext = new VisitContext())
            {
                var x = myContext.Database.Exists(); // hack
            }

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        /// <summary>Create a new instance of type <see cref="VisitContext"/>
        /// </summary>
        /// <remarks>
        /// <para>On the first connection to the server, if the specified database does not exist it will be created.</para>
        /// 
        /// <para>Visual Studio does not do anything with the database during the deployment process. However, when 
        /// the deployed application tries to access the database for the first time after deployment, Code First 
        /// automatically creates the database or updates the database schema to the latest version. If the 
        /// application implements a Migrations Seed method, the method runs after the database is created or the schema is updated.</para>
        /// 
        /// <para>The Seed method has been overloaded so that it supplies default data.</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// Database.SetInitializer&lt;VisitContext>(new SeededSiteDBInitialize());
        /// using (var myContext = new VisitContext())
        /// {
        ///     var x = myContext.Database.Exists(); // hack
        /// }
        /// </code>
        /// </example>
        public class SiteDBInitialize : CreateDatabaseIfNotExists<VisitContext>
        {
            // Note: the Seed (override) method below is used to initially populate the database.
            // Note: if the tables on the database have no rows, delete the database and publish BackEnd. Seed (override) will populate the tables.
        }

        /// <summary>Dangerous. If the database model has changed, Drop/Create a new instance of type <VisitContext>. 
        /// </summary>
        /// <remarks>On the first connection to the server, if the specified database exists and the database model is 
        /// different, the existing instance of the database will be dropped and a new instance (with the new model)
        /// will be created.</remarks>
        //public class SiteDBInitialize : DropCreateDatabaseIfModelChanges<VisitContext>
        //{
        //    protected override void Seed(VisitContext context)
        //    {
        //        context.NewEmailRecipients.Add(new CallForm.Core.Models.NewEmailRecipient { Address = "global.asax.cs DropCreateDatabaseIfModelChanges", });
        //        context.SaveChanges();
        //    }
        //}

        ///// <summary>Very dangerous. ALWAYS Drop/Create a new instance of type <VisitContext>. 
        ///// </summary>
        //public class SiteDBInitialize : DropCreateDatabaseAlways<VisitContext>
        //{
        //    // use the Seed method below to initially populate the database
        //    //protected override void Seed(VisitContext context)
        //    //{
        //    //    context.NewEmailRecipients.Add(new CallForm.Core.Models.NewEmailRecipient { Address = "global.asax.cs DropCreateDatabaseAlways", });
        //    //    context.SaveChanges();
        //    //}
        //}

        /// <summary>Inherited object that "wraps" SiteDBInitialize so that this one Seed method can service all three initialize classes
        /// (by simply commenting out the two inactive classes).
        /// </summary>
        public class SeededSiteDBInitialize : SiteDBInitialize
        {
            /// <summary>Runs after the database is created or the schema is updated.
            /// </summary>
            /// <param name="context">The VisitContext model from BackEnd.</param>
            protected override void Seed(VisitContext context)
            {
                // NewEmailRecipients
                //context.EmailRecipients.Add(new CallForm.Core.Models.EmailRecipient { Address = "loaded via global", DisplayName = "loaded via global" });
                context.EmailRecipients.Add(new CallForm.Core.Models.EmailRecipient { Address = "info@agri-maxfinancial.com", 
                                                                                      DisplayName = "AgriMax" });
                context.EmailRecipients.Add(new CallForm.Core.Models.EmailRecipient { Address = "info@agri-servicesagency.com", 
                                                                                      DisplayName = "ASA" });
                context.EmailRecipients.Add(new CallForm.Core.Models.EmailRecipient { Address = "communications@dairylea.com",
                                                                                      DisplayName = "Communications" });
                context.EmailRecipients.Add(new CallForm.Core.Models.EmailRecipient { Address = "FieldStaffNotification-DairyOne@DairyOne.com",
                                                                                      DisplayName = "DairyOne" });
                context.EmailRecipients.Add(new CallForm.Core.Models.EmailRecipient { Address = "FieldStaffNotification-DMS@dairylea.com", 
                                                                                      DisplayName = "DMS" });
                context.EmailRecipients.Add(new CallForm.Core.Models.EmailRecipient { Address = "drms@dairylea.com", 
                                                                                      DisplayName = "DRMS" });
                context.EmailRecipients.Add(new CallForm.Core.Models.EmailRecipient { Address = "FieldStaffNotification-Eagle@dairylea.com", 
                                                                                      DisplayName = "Eagle" });
                context.EmailRecipients.Add(new CallForm.Core.Models.EmailRecipient { Address = "FieldStaffNotification-HR@dairylea.com", 
                                                                                      DisplayName = "Human Resources" });
                context.EmailRecipients.Add(new CallForm.Core.Models.EmailRecipient { Address = "TechnicalSupport-brittonfield@dairylea.com", 
                                                                                      DisplayName = "Technical Support" });
                context.EmailRecipients.Add(new CallForm.Core.Models.EmailRecipient { Address = "FieldStaffNotification-Membership@dairylea.com", 
                                                                                      DisplayName = "Membership" });
                context.EmailRecipients.Add(new CallForm.Core.Models.EmailRecipient { Address = "FieldStaffNotification-Payroll@dairylea.com", 
                                                                                      DisplayName = "Producer Payments" });

                // CallTypes
                //context.CallTypes.Add(new CallForm.Core.Models.CallType { Name = "Global" });
                context.CallTypes.Add(new CallForm.Core.Models.CallType { Name = "Phone Call" });
                context.CallTypes.Add(new CallForm.Core.Models.CallType { Name = "Email" });
                context.CallTypes.Add(new CallForm.Core.Models.CallType { Name = "Farm Visit" });
                context.CallTypes.Add(new CallForm.Core.Models.CallType { Name = "Farm Show" });
                context.CallTypes.Add(new CallForm.Core.Models.CallType { Name = "SMS (Text Msg.)" });
                context.CallTypes.Add(new CallForm.Core.Models.CallType { Name = "Other" });

                // ReasonCodes
                context.ReasonCodes.Add(new CallForm.Core.Models.ReasonCode { Name = "Regulatory: Calibrations", Code = 11 });
                context.ReasonCodes.Add(new CallForm.Core.Models.ReasonCode { Name = "Regulatory: Inspection", Code = 12 });
                context.ReasonCodes.Add(new CallForm.Core.Models.ReasonCode { Name = "Regulatory: Quality: Antibiotic", Code = 15 });
                context.ReasonCodes.Add(new CallForm.Core.Models.ReasonCode { Name = "Regulatory: Quality: Cryo", Code = 16 });
                context.ReasonCodes.Add(new CallForm.Core.Models.ReasonCode { Name = "Regulatory: Quality: Hi-count", Code = 17 });
                context.ReasonCodes.Add(new CallForm.Core.Models.ReasonCode { Name = "Regulatory: Samples", Code = 13 });

                context.ReasonCodes.Add(new CallForm.Core.Models.ReasonCode { Name = "Membership: Cancellation", Code = 59 });
                context.ReasonCodes.Add(new CallForm.Core.Models.ReasonCode { Name = "Membership: Relationship Call", Code = 55 });
                context.ReasonCodes.Add(new CallForm.Core.Models.ReasonCode { Name = "Membership: Solicitation", Code = 56 });

                context.ReasonCodes.Add(new CallForm.Core.Models.ReasonCode { Name = "Gold Standard III", Code = 43 });

                context.ReasonCodes.Add(new CallForm.Core.Models.ReasonCode { Name = "Farm Services: Agri-Max", Code = 31 });
                context.ReasonCodes.Add(new CallForm.Core.Models.ReasonCode { Name = "Farm Services: ASA: Insurance", Code = 32 });
                context.ReasonCodes.Add(new CallForm.Core.Models.ReasonCode { Name = "Farm Services: Dairy One", Code = 33 });
                context.ReasonCodes.Add(new CallForm.Core.Models.ReasonCode { Name = "Farm Services: Eagle: Farm Supplies", Code = 34 });
                context.ReasonCodes.Add(new CallForm.Core.Models.ReasonCode { Name = "Farm Services: Empire Livestock", Code = 35 });
                context.ReasonCodes.Add(new CallForm.Core.Models.ReasonCode { Name = "Farm Services: Risk Management", Code = 36 });

                // write changes
                context.SaveChanges();
            }
        }
    }
}
