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

namespace CallForm.iOS
{
    /// <summary>This references the Core project and lets the runtime know how to instantiate the application.</summary>
    /// <remarks><para>This Class is called from AppDelegate.cs (via FinishedLoading), and in turn calls CallForm.Core.App.</para>
	public class Setup : MvxTouchSetup
	{
        string _nameSpace = "CallForm.iOS.";

		public Setup(MvxApplicationDelegate applicationDelegate, UIWindow window)
            : base(applicationDelegate, window)
		{
		}

	    protected override IMvxTouchViewPresenter CreatePresenter()
        {
            return new MvxProducerVisitTouchViewPresenter(ApplicationDelegate, Window);
	    }

        /// <summary>Creates a new instance of the App.
        /// </summary>
        /// <returns></returns>
	    protected override IMvxApplication CreateApp ()
		{
            Common_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            Common_iOS.DebugMessage("  [Setup][CA] > return new Core.App().");

			return new Core.App();
		}
		
        // broken: using this seems to cause the app to crash
        //protected override IMvxTrace CreateDebugTrace()
        //{
        //    return new DebugTrace();
        //}
	}

    public class MvxProducerVisitTouchViewPresenter : MvxTouchViewPresenter
    {
        string _nameSpace = "CallForm.iOS.";

        public MvxProducerVisitTouchViewPresenter(UIApplicationDelegate applicationDelegate, UIWindow window) : base(applicationDelegate, window)
        {
        }

        /// <summary>Show the view (but if it's <see cref="NewVisit_View"/> turn animation off).
        /// </summary>
        /// <param name="view">The view to be displayed.</param>
        public override void Show(IMvxTouchView view)
        {

            if (view.Request.ViewModelType == typeof(NewVisit_ViewModel))
            {
                Common_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
                Common_iOS.DebugMessage("  [Setup][S] > ViewModelType is NewVisit_ViewModel. ^^^^^^^^^^^^^^^^^^^^^^^^^^^^");

                if (MasterNavigationController.TopViewController is NewVisit_View)
                {
                    Common_iOS.DebugMessage("  [Setup][S] > NewVisit_ViewModel is the TopViewController.");
                    Common_iOS.DebugMessage("  [Setup][S] > about to 'PopViewController' NewVisit_ViewModel.");

                    MasterNavigationController.PopViewControllerAnimated(false);
                    Common_iOS.DebugMessage("  [Setup][S] > NewVisit_ViewModel has been popped. ^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
                }
            }

            if ((view.Request.ViewModelType == typeof(UserIdentity_ViewModel)) || (view.Request.ViewModelType == typeof(UserIdentity_View)))
            {
                // ToDo: hide status bar (and the 'back' button)
                Common_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            if ((view.Request.ViewModelType == typeof(UserIdentity_ViewModel)) || (view.Request.ViewModelType == typeof(UserIdentity_View)))
                Common_iOS.DebugMessage("  [Setup][S] > ToDo - hide status bar (and the 'back' button) on UserIdentity_View. ^^^^^^^^^^^^^^^^^^^^^^^^^^^^");
                //MasterNavigationController.SetNavigationBarHidden(true, false);
            }

            if ((view.Request.ViewModelType == typeof(ViewReports_ViewModel)) || (view.Request.ViewModelType == typeof(ViewReports_View)))
            {
                Common_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
                Common_iOS.DebugMessage("  [Setup][S] > ViewReports_View is the TopViewController. ^^^^^^^^^^^^^^^^^^^^^^^^^^^^");

                //MasterNavigationController.SetNavigationBarHidden(false, false);

                //Common_iOS.DebugMessage("  [Setup][S] > ToDo - hide status bar (and the 'back' button).");
            }

            base.Show(view);
        }
    }
}