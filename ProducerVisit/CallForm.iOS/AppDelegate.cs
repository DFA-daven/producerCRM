using Cirrious.CrossCore;
using Cirrious.MvvmCross.Touch.Platform;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using Cirrious.MvvmCross.ViewModels;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.Diagnostics;
//using CallForm.Core.ViewModels;


namespace CallForm.iOS
{
    /// <summary>Creates an AppDelegate for CallForm.iOS.</summary>
    /// <remarks>
    /// <para>This is the heart of the project.</para>
    /// <para>This Class is called from Main.cs, and in turn calls Setup.cs.</para>
    /// <para>The <see cref="AppDelegate"/> type inherits from <see cref="UIApplicationDelegate"/> 
    /// (via <see cref="MvxApplicationDelegate"/>), which provides application life-cycle events 
    /// such as FinishedLaunching and WillTerminate.</para>
    /// </remarks>
    [Register("AppDelegate")]
    public partial class AppDelegate : MvxApplicationDelegate
    {
        #region class-level declarations
        UIWindow _window;
        UINavigationController _navController;
        UINavigationBar _navigationBar;
        #endregion

        /// <summary>Defines actions to occur after FinishedLaunching.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="options"></param>
        /// <returns>True</returns>
        /// <remarks>
        /// <para>The critical piece is the call to <see cref="Setup"/>.</para>
        /// 
        /// <para>This method is invoked when the application has loaded and is ready to run. In this
        /// method you should instantiate the window, load the UI into it and then make the window
        /// visible.</para>
        /// 
        /// <para>You have 17 seconds to return from this method, or iOS will terminate your application.</para>
        /// </remarks>
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            string message = "AddDelegate.FinishedLaunching(): starting method.";
            System.Console.WriteLine(message);
            Debug.WriteLine(message);

            _window = new UIWindow(UIScreen.MainScreen.Bounds);     // required

            // FixMe: use this after fixing UserIdentityView.cs
            // initialize the app for single screen display
            //var presenter = new MvxModalSupportTouchViewPresenter(this, _window);
            //// Note: this is a call to the custom Setup.cs
            //var setup = new Setup(this, presenter);
            //setup.Initialize();


            // Note: this is a call to the custom Setup.cs
            var setup = new Setup(this, _window);                   // required
            setup.Initialize();                                     // required


            // start the app
            //var start = this.GetService<ImvxStartNavigation>();

            // launch the App via the IMvxAppStart interface -- CallForm.Core.ViewModels.ViewReports_ViewModel.Start
            var startup = Mvx.Resolve<IMvxAppStart>();              // required
            startup.Start();                                        // required

            #region experimental
            // Note: The default presenter uses a UINavigationController for the RootController 
            // on the window; so we can manipulate the navigation bar globally in 
            // AppDelegate by grabbing it off the window and casting:
            _navController = (UINavigationController)_window.RootViewController;
            _navigationBar = _navController.NavigationBar;
            _navigationBar.BackgroundColor = UIColor.Red;
            //navigationBar.BarTintColor = UIColor.Black;
            //navigationBar.TintColor = UIColor.White;

            //navigationBar.Hidden = true;
            //navController.NavigationBarHidden = true;

            app.SetStatusBarStyle(UIStatusBarStyle.LightContent, true);
            //app.SetStatusBarHidden(true, false);        // this makes the 'Back" button on NewVisit disappear
            #endregion

            _window.MakeKeyAndVisible();                            // required

            //if (!Reachability.IsHostReachable("dl-backend-02.azurewebsites.net"))
            //{
            //    started = false;
            //    Mvx.Error("Host not reachable.");
            //    //Error(this, new ErrorEventArgs { Message = "Host not reachable." });
            //}
            //else
            //{
            //    started = true;
            //    Mvx.Error("Host is reachable.");
            //}

            message = "AddDelegate.FinishedLaunching(): finished method.";
            System.Console.WriteLine(message);
            Debug.WriteLine(message);

            bool started = true;
            return started;                                         // required
        }

        public override void OnActivated(UIApplication application)
        {
            string message = "AddDelegate.OnActivated(): App is active.";
            System.Console.WriteLine(message);
            Debug.WriteLine(message);
        }

        public override void WillEnterForeground(UIApplication application)
        {
            string message = "AddDelegate.WillEnterForeground(): App will enter foreground.";
            System.Console.WriteLine(message);
            Debug.WriteLine(message);
        }

        public override void OnResignActivation(UIApplication application)
        {
            string message = "AddDelegate.OnResignActivation(): App moving to inactive state.";
            System.Console.WriteLine(message);
            Debug.WriteLine(message);
        }

        public override void DidEnterBackground(UIApplication application)
        {
            string message = "AddDelegate.DidEnterBackground(): App entering background state.";
            System.Console.WriteLine(message);
            Debug.WriteLine(message);
        }

        public override void ReceivedLocalNotification (UIApplication application, UILocalNotification notification)
        {
            string message = "AddDelegate.ReceivedLocalNotification(): " + notification.Description;
            System.Console.WriteLine(message);
            Debug.WriteLine(message);
        }

        // not guaranteed that this will run
        public override void WillTerminate(UIApplication application)
        {
            string message = "AddDelegate.WillTerminate(): App is terminating.";
            System.Console.WriteLine(message);
            Debug.WriteLine(message);
        }

        //public event EventHandler<ErrorEventArgs> Error;
    }
}

