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

    /// <remarks>This Class is called from AppDelegate.cs (via FinishedLoading), and in turn calls CallForm.Core.App.</remarks>
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
            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            CommonCore_iOS.DebugMessage(" > return new Core.App().");

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
                CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
                CommonCore_iOS.DebugMessage(" > showing view.");

                if (MasterNavigationController.TopViewController is NewVisit_View)
                {
                    MasterNavigationController.PopViewControllerAnimated(false);
                }
            }

            if (view.Request.ViewModelType == typeof(UserIdentity_View))
            {
                // ToDo: hide status bar (and the 'back' button)
                CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
                CommonCore_iOS.DebugMessage(" > ToDo - hide status bar (and the 'back' button).");
            }

            base.Show(view);
        }
    }
}