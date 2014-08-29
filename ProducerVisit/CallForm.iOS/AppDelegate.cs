using Cirrious.CrossCore;
using Cirrious.MvvmCross.Touch.Platform;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using Cirrious.MvvmCross.ViewModels;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.Diagnostics;
using System.Reflection;
//using CallForm.Core.ViewModels;


namespace CallForm.iOS
{
    /// <summary>Creates an AppDelegate for CallForm.iOS.</summary>
    /// <remarks>
    /// <para>This is the heart of the project.</para>
    /// <para>This Class is called from <c>CallForm.iOS.Main.cs</c>, and in turn calls <c>CallForm.iOS.Setup.cs</c>.</para>
    /// <para>The <see cref="AppDelegate"/> type inherits from <see cref="UIApplicationDelegate"/> 
    /// (via <see cref="MvxApplicationDelegate"/>), which provides application life-cycle events 
    /// such as FinishedLaunching and WillTerminate.</para>
    /// </remarks>
    [Register("AppDelegate")]
    public partial class AppDelegate : MvxApplicationDelegate
    {
        #region class-level declarations
        UIWindow _window;

        // Note: The Navigation Controller is a UI-less View Controller responsible for
        // managing a stack of View Controllers and provides tools for navigation, such 
        // as a navigation bar with a back button.
        UINavigationController _navController;
        UINavigationBar _navigationBar;
        string _nameSpace = "CallForm.iOS.";

        public override UIWindow Window
        {
            get
            {
                return base.Window;
            }
            set
            {
                base.Window = value;
            }
        }
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
            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            CommonCore_iOS.DebugMessage("  [AppD][fl] > Assembly info:" + System.Reflection.Assembly.GetExecutingAssembly().GetName().ToString());
            CommonCore_iOS.DebugMessage("  [AppD][fl] > starting method...");

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
            //_navigationBar.BackgroundColor = UIColor.Green;
            //navigationBar.BarTintColor = UIColor.Black;
            //navigationBar.TintColor = UIColor.White;

            //navigationBar.Hidden = true;
            //navController.NavigationBarHidden = true;

            // Review: why won't this work? 
            //_navigationBar.Delegate = new NavBarDelegate();

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

            // experimental:
            UIApplication.SharedApplication.ApplicationSupportsShakeToEdit = true;

            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            CommonCore_iOS.DebugMessage("  [AppD][fl] > ...finished method.");

            bool started = true;
            return started;                                         // required
        }

        public override void OnActivated(UIApplication application)
        {
            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            CommonCore_iOS.DebugMessage("  [AppD][oa] > App is active.");
        }

        public override void WillEnterForeground(UIApplication application)
        {
            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            CommonCore_iOS.DebugMessage("  [AppD][wef] > App will enter foreground.");
        }

        public override void OnResignActivation(UIApplication application)
        {
            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            CommonCore_iOS.DebugMessage("  [AppD][ora] > App moving to inactive state.");
        }

        public override void DidEnterBackground(UIApplication application)
        {
            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            CommonCore_iOS.DebugMessage("  [AppD][deb] > App entering background state.");
        }

        public override void ReceivedLocalNotification (UIApplication application, UILocalNotification notification)
        {
            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            CommonCore_iOS.DebugMessage("  [AppD][rln] > " + notification.Description);
        }

        // not guaranteed that this will run
        public override void WillTerminate(UIApplication application)
        {
            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            CommonCore_iOS.DebugMessage("  [AppD][wt] > App is terminating.");
        }

        //public event EventHandler<ErrorEventArgs> Error;
    }

    class NavBarDelegate : UINavigationBarDelegate
    {
        public override UIBarPosition GetPositionForBar(IUIBarPositioning barPositioning)
        {
            return UIBarPosition.TopAttached;
        }
    }
}

