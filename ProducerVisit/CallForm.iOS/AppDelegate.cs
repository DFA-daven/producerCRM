using Cirrious.CrossCore;
using Cirrious.MvvmCross.Touch.Platform;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using Cirrious.MvvmCross.ViewModels;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CallForm.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : MvxApplicationDelegate
    {
        UIWindow _window;
        
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
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

            if (!Reachability.IsHostReachable("dl-backend-02.azurewebsites.net"))
            {
                started = false;
            }
            else
            {
                started = true;
            }

            return started;
        }
    }
}

