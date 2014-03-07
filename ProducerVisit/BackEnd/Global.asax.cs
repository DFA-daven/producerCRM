using BackEnd.Models;

namespace BackEnd
{
    using System.Data.Entity;
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
            Database.SetInitializer(new CreateDatabaseIfNotExists<VisitContext>());

            // fixme: the issue seems to be that later on, if the empty table(s) exist, the code
            // will not populate them with the default values. It should be possible to move the database
            // seeding here, and never comment out "CreateDatabaseIfNotExists".

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
