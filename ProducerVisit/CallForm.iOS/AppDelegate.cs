using Cirrious.CrossCore;
using Cirrious.MvvmCross.Touch.Platform;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using Cirrious.MvvmCross.ViewModels;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
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
        UIWindow _window;

        /// <summary>Defines actions to occur after FinishedLaunching.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="options"></param>
        /// <returns>True</returns>
        /// <remarks>The critical piece is the call to <see cref="Setup"/>.</remarks>
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            _window = new UIWindow(UIScreen.MainScreen.Bounds);     // required

            // FixMe: use this after fixing UserIdentityView.cs
            // initialize the app for single screen display
            //var presenter = new MvxModalSupportTouchViewPresenter(this, _window);
            //// Note: this is a call to the custom Setup.cs
            //var setup = new Setup(this, presenter);
            //setup.Initialize();


            // Note: this is a call to the custom Setup.cs
            var setup = new Setup(this, _window);                   // required
            setup.Initialize();


            // start the app
            //var start = this.GetService<ImvxStartNavigation>();

            // launch the App via the IMvxAppStart interface -- CallForm.Core.ViewModels.ViewReports_ViewModel.Start
            var startup = Mvx.Resolve<IMvxAppStart>();              // required
            startup.Start();                                        // required
            

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

            bool started = true;
            return started;                                         // required
        }

        public override void OnActivated(UIApplication application)
        {
            System.Console.WriteLine("OnActivated called, App is active.");
        }

        public override void WillEnterForeground(UIApplication application)
        {
            System.Console.WriteLine("App will enter foreground");
        }

        public override void OnResignActivation(UIApplication application)
        {
            System.Console.WriteLine("OnResignActivation called, App moving to inactive state.");
        }

        public override void DidEnterBackground(UIApplication application)
        {
            System.Console.WriteLine("App entering background state.");
        }

        public override void ReceivedLocalNotification (UIApplication application, UILocalNotification notification)
        {
            System.Console.WriteLine("ReceivedLocalNotification: " + notification.Description); 
        }

        // not guaranteed that this will run
        public override void WillTerminate(UIApplication application)
        {
            System.Console.WriteLine("App is terminating.");
        }

        //public event EventHandler<ErrorEventArgs> Error;
    }
}

