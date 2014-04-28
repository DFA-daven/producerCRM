using BackEnd.Models;

namespace BackEnd
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;
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
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            // broken: this line must be commented out on the initial Publish/Deploy. can this be automated?
            // Doing so enables the tables (schema) to be established in the database.
            // After the first run, the uncommented line allows everything to work.
            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<VisitContext>());

            // The following implementations are provided: 
            // DropCreateDatabaseIfModelChanges<TContext>, DropCreateDatabaseAlways<TContext>, CreateDatabaseIfNotExists<TContext>.
            // this seems safer, but still must be commented out on first run:
            // Database.SetInitializer(new CreateDatabaseIfNotExists<VisitContext>());

            Database.SetInitializer<VisitContext>(new SiteDBInitialize());
            using (var myContext = new VisitContext())
            {
                var x = myContext.Database.Exists(); // hack
            }

            // FixMe: the issue seems to be that later on, if the empty table(s) exist, the code
            // will not populate them with the default values. It should be possible to move the database
            // seeding here, and never comment out "CreateDatabaseIfNotExists".

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        public class SiteDBInitialize : CreateDatabaseIfNotExists<VisitContext>
        {
            protected override void Seed(VisitContext context)
            {
                context.NewEmailRecipients.Add(new CallForm.Core.Models.NewEmailRecipient { Address = "global.asax.cs CreateDatabaseIfNotExists", });

                context.SaveChanges();
            }
        }

        //public class SiteDBInitialize : DropCreateDatabaseAlways<VisitContext> 
        //{
        //    protected override void Seed(VisitContext context)
        //    {
        //        context.NewEmailRecipients.Add(new CallForm.Core.Models.NewEmailRecipient { Address = "global.asax.cs DropCreateDatabaseAlways", });

        //        context.SaveChanges();
        //    }
        //}

        //public class SiteDBInitialize : DropCreateDatabaseIfModelChanges<VisitContext>
        //{
        //    protected override void Seed(VisitContext context)
        //    {
        //        context.NewEmailRecipients.Add(new CallForm.Core.Models.NewEmailRecipient { Address = "global.asax.cs DropCreateDatabaseIfModelChanges", });
        //        // context.NewEmailRecipients.Add(new CallForm.Core.Models.NewEmailRecipient { Address = "info@agri-maxfinancial.com", DisplayName = "info@agri-maxfinancial.com" });
        //        //context.NewEmailRecipients.Add(new CallForm.Core.Models.NewEmailRecipient {Address = "info@agri-servicesagency.com", DisplayName =  "info@agri-servicesagency.com"});
        //        //context.NewEmailRecipients.Add(new CallForm.Core.Models.NewEmailRecipient {Address = "communications@dairylea.com", DisplayName = "Member Communications" });
        //        //context.NewEmailRecipients.Add(new CallForm.Core.Models.NewEmailRecipient {Address = "FieldStaffNotification-DairyOne@DairyOne.com", DisplayName = "FieldStaffNotification-DairyOne@DairyOne.com" });
        //        //context.NewEmailRecipients.Add(new CallForm.Core.Models.NewEmailRecipient {Address = "FieldStaffNotification-DMS@dairylea.com", DisplayName = "FieldStaffNotification-DMS@dairylea.com" });
        //        //context.NewEmailRecipients.Add(new CallForm.Core.Models.NewEmailRecipient {Address = "drms@dairylea.com", DisplayName = "DRMS" });
        //        //context.NewEmailRecipients.Add(new CallForm.Core.Models.NewEmailRecipient {Address = "FieldStaffNotification-Eagle@dairylea.com", DisplayName = "FieldStaffNotification-Eagle@dairylea.com" });
        //        //context.NewEmailRecipients.Add(new CallForm.Core.Models.NewEmailRecipient {Address = "FieldStaffNotification-HR@dairylea.com", DisplayName = "FieldStaffNotification-HR@dairylea.com" });
        //        //context.NewEmailRecipients.Add(new CallForm.Core.Models.NewEmailRecipient {Address = "TechnicalSupport-brittonfield@dairylea.com", DisplayName = "Technical Support" });
        //        //context.NewEmailRecipients.Add(new CallForm.Core.Models.NewEmailRecipient {Address = "FieldStaffNotification-Membership@dairylea.com", DisplayName = "FieldStaffNotification-Membership@dairylea.com" });
        //        //context.NewEmailRecipients.Add(new CallForm.Core.Models.NewEmailRecipient {Address = "FieldStaffNotification-Payroll@dairylea.com", DisplayName = "FieldStaffNotification-Payroll@dairylea.com" });

        //        context.SaveChanges();
        //    }
        //}
    }
}
