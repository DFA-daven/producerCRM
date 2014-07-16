namespace CallForm.Core
{
    using Cirrious.CrossCore.IoC;
    using System.Diagnostics;

    /// <summary>Creates an instance of the App. This is the "Core" Project.
    /// </summary>
    /// <remarks>This Class is called from CallForm.iOS.Setup.</remarks>
    public class App : Cirrious.MvvmCross.ViewModels.MvxApplication
    {
        /// <summary>Defines the App Start point as ViewReports_ViewModel.
        /// </summary>
        public override void Initialize()
        {
            string message = "App.Initialize(): RegisterAppStart<ViewModels.ViewReports_ViewModel>().";
            //System.Console.WriteLine(message);
            Debug.WriteLine(message);

            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();
				
            RegisterAppStart<ViewModels.ViewReports_ViewModel>();
        }
    }
}