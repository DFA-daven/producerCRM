namespace CallForm.Core
{
    using Cirrious.CrossCore.IoC;
    using Cirrious.MvvmCross.ViewModels;
    using System.Diagnostics;
    using System.Reflection;

    /// <summary>Creates an instance of the App. This is the "Core" Project.
    /// </summary>
    /// <remarks><para>This Class is called from CallForm.iOS.Setup.</para>
    /// <para>This initializes the services and defines the ViewModel that will start on launch.</para></remarks>
    public class App : MvxApplication
    {
        /// <summary>Defines the App Start point as ViewReports_ViewModel.
        /// </summary>
        public override void Initialize()
        {
            CommonCore.DebugMessage("CallForm.Core.App", "Initialize");
            CommonCore.DebugMessage("  [App][App] > RegisterAppStart<ViewModels.ViewReports_ViewModel>()");

            CreatableTypes()
                .EndingWith("Service")
                .AsInterfaces()
                .RegisterAsLazySingleton();
				
            RegisterAppStart<ViewModels.ViewReports_ViewModel>();
        }
    }

    /// <summary>Commonly used methods.
    /// </summary>
    /// <remarks>This class may be (partially) duplicated in other Projects.</remarks>
    public class CommonCore
    {
        /// <summary>Take the given information and write it to the iOS app's err.log.
        /// </summary>
        /// <param name="message">The message to write to the log.</param>
        private static void DebugMessage(string message)
        {
            Debug.WriteLine(message);
        }

        /// <summary>Take the given information and write it to the iOS app's err.log.
        /// </summary>
        /// <param name="message">The message to write to the log.</param>
        /// <param name="writeToConsole">If <c>True</c>, also write <paramref name="message"/> to the <c>System.Console</c>.</param>
        public static void DebugMessage(string message, bool writeToConsole = false)
        {
            if (writeToConsole)
            {
                // Review: is the assembly reference correct? System.Console doesn't work here.
                //System.Console.WriteLine(message);
            }

            DebugMessage(message);
        }

        /// <summary>Take the given information and write it to the iOS app's err.log.
        /// </summary>
        /// <param name="declaringName">The name of the file that threw the error</param>
        /// <param name="methodName">The name of the method that threw the error.
        /// This is useful for things like the Master page.</param>
        /// <param name="writeToConsole">If <c>True</c>, also write the message to the <c>System.Console</c>.</param>
        public static void DebugMessage(string declaringName, string methodName, bool writeToConsole = false)
        {
            string message = "Class: " + declaringName + ", Method: " + methodName + "()";
            DebugMessage(message, writeToConsole);
        }

        /// <summary>Take the given information and write it to the iOS app's err.log.
        /// </summary>
        /// <param name="declaringName">The name of the file that threw the error</param>
        /// <param name="methodName">The name of the method that threw the error</param>
        /// <param name="parentName">The URL of the page that threw the exception. 
        /// This is useful for things like the Master page.</param>
        /// <param name="writeToConsole">If <c>True</c>, also write the message to the <c>System.Console</c>.</param>
        public static void DebugMessage(string declaringName, string methodName, string parentName, bool writeToConsole = false)
        {
            string message = "Class: " + declaringName + ", Method: " + methodName + "(), Parent: " + parentName;
            DebugMessage(message, writeToConsole);
        }

        // Review: will this even work if set to static? 
        // review: should this be in the individual ViewModels?
        private static int _numberOfCallsToSetVisible = 0;
        public static int NumberOfCallsToSetVisible
        {
            get { return _numberOfCallsToSetVisible; }
            set { _numberOfCallsToSetVisible = value; }
        }

        // Undone: finish implementing in the ViewMethods.
        public static void SetNetworkActivityIndicatorVisible(bool setVisible)
        {
            if (setVisible)
            {
                NumberOfCallsToSetVisible++;
            }
            else
            {
                NumberOfCallsToSetVisible--;
            }

            if (NumberOfCallsToSetVisible < 0)
            {
                NumberOfCallsToSetVisible = 0;
                DebugMessage("  [CommonCore][snaiv] > SetNetworkActivityIndicatorVisible() was asked to hide more often than shown.");
            }

            // Note: is this the right place to send an instruction to the View 
            //UIApplication.SharedApplication.NetworkActivityIndicatorVisible = (NumberOfCallsToSetVisible > 0);
        }
    }
}