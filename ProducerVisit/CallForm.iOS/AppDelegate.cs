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
    [Register("AppDelegate")]
    public partial class AppDelegate : MvxApplicationDelegate
    {
        UIWindow _window;
        
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            // System.Console.WriteLine("App entered FinishedLaunching.");
            _window = new UIWindow(UIScreen.MainScreen.Bounds);
            bool started = true;

            // initialize the app for single single screen display
            var presenter = new MvxModalSupportTouchViewPresenter(this, _window);
            // fixme: use this after fixing UserIdentityView.cs
            //var setup = new Setup(this, presenter);
            var setup = new Setup(this, _window);
            setup.Initialize();

            // start the app
            //var start = this.GetService<ImvxStartNavigation>();
            var startup = Mvx.Resolve<IMvxAppStart>();
            startup.Start();
            

            _window.MakeKeyAndVisible();

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

            return started;
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

