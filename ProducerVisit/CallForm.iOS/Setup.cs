using CallForm.Core.ViewModels;
using CallForm.iOS.Views;
using Cirrious.MvvmCross.Touch.Views;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using MonoTouch.UIKit;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Touch.Platform;
using System.Diagnostics;
using System.Reflection;
using System;

namespace CallForm.iOS
{
    /// <summary>This references the Core project and lets the runtime know how to instantiate the application.</summary>
    /// <remarks><para>This Class is called from AppDelegate.cs (via FinishedLoading), and in turn calls CallForm.Core.App.</para>
	public class Setup : MvxTouchSetup
	{
		string _namespace = "CallForm.iOS.";
		string _class = "Setup.";
		string _method = "TBD";

		public Setup(MvxApplicationDelegate applicationDelegate, UIWindow window)
            : base(applicationDelegate, window)
		{
		}

	    protected override IMvxTouchViewPresenter CreatePresenter()
        {
            return new MvxProducerVisit_TouchViewPresenter(ApplicationDelegate, Window);
	    }

        /// <summary>Creates a new instance of the App.
        /// </summary>
        /// <returns></returns>
	    protected override IMvxApplication CreateApp ()
		{
			_method = "CreateApp";
	  // Common_iOS.DebugMessage(_namespace + _class, _method);
            Console.WriteLine("  [Setup][CA] > return new Core.App(). < SETUP SETUP SETUP SETUP SETUP SETUP");
			return new Core.App();
		}
		
        // broken: using this seems to cause the app to crash
        //protected override IMvxTrace CreateDebugTrace()
        //{
        //    return new DebugTrace();
        //}
	}

    public class MvxProducerVisit_TouchViewPresenter : MvxTouchViewPresenter
    {
        string _nameSpace = "CallForm.iOS.";
		string _class = "MvxProducerVisit_TouchViewPresenter.";
		string _method = "TBD";

        public MvxProducerVisit_TouchViewPresenter(UIApplicationDelegate applicationDelegate, UIWindow window) : base(applicationDelegate, window)
        {
			_method = "BASE";
	  // Common_iOS.DebugMessage(_nameSpace + _class, _method);
            Console.WriteLine("  [Setup][mpv_tvp] > New instance of MvxProducerVisit_TouchViewPresenter created. < SETUP SETUP SETUP SETUP SETUP SETUP");
        }

        /// <summary>Show the view (but if it's <see cref="NewVisit_View"/> turn animation off).
        /// </summary>
        /// <param name="view">The view to be displayed.</param>
        public override void Show(IMvxTouchView view)
        {
			_method = "Show";
	  // Common_iOS.DebugMessage(_nameSpace + _class, _method);

            if (view.Request.ViewModelType == typeof(NewVisit_ViewModel))
            {
                Console.WriteLine("  [Setup][mpv_tvp][S] > the View requested NewVisit_ViewModel. < SETUP SETUP SETUP SETUP SETUP SETUP");

                if (MasterNavigationController.TopViewController is NewVisit_View)
                {
                    Console.WriteLine("  [Setup][mpv_tvp][S] > NewVisit_ViewModel is the TopViewController. < SETUP SETUP SETUP SETUP SETUP SETUP");
                    Console.WriteLine("  [Setup][mpv_tvp][S] > about to 'PopViewController' NewVisit_ViewModel. < SETUP SETUP SETUP SETUP SETUP SETUP");

                    MasterNavigationController.PopViewControllerAnimated(false);
                    Console.WriteLine("  [Setup][mpv_tvp][S] > NewVisit_ViewModel has been popped. < SETUP SETUP SETUP SETUP SETUP SETUP");
                }
            }
            else if (view.Request.ViewModelType == typeof(UserIdentity_ViewModel))
            {
                Console.WriteLine("  [Setup][mpv_tvp][S] > the View requested UserIdentity_ViewModel. < SETUP SETUP SETUP SETUP SETUP SETUP");
            }
            else if ((view.Request.ViewModelType == typeof(ViewReports_ViewModel)))
            {
                Console.WriteLine("  [Setup][mpv_tvp][S] > the View requested ViewReports_ViewModel. < SETUP SETUP SETUP SETUP SETUP SETUP");
            }
            else
            {
                Console.WriteLine("  [Setup][mpv_tvp][S] > the View requested an 'unknown' (new?) ViewModel type. < SETUP SETUP SETUP SETUP SETUP SETUP");
            }

            base.Show(view);
        }
    }
}