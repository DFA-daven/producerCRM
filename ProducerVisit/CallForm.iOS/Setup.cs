using CallForm.Core.ViewModels;
using CallForm.iOS.Views;
using Cirrious.MvvmCross.Touch.Views;
using Cirrious.MvvmCross.Touch.Views.Presenters;
using MonoTouch.UIKit;
using Cirrious.CrossCore.Platform;
using Cirrious.MvvmCross.ViewModels;
using Cirrious.MvvmCross.Touch.Platform;

namespace CallForm.iOS
{
	public class Setup : MvxTouchSetup
	{

		public Setup(MvxApplicationDelegate applicationDelegate, UIWindow window)
            : base(applicationDelegate, window)
		{
		}

	    protected override IMvxTouchViewPresenter CreatePresenter()
        {
            return new MvxProducerVisitTouchViewPresenter(ApplicationDelegate, Window);
	    }

	    protected override IMvxApplication CreateApp ()
		{
			return new Core.App();
		}
		
        protected override IMvxTrace CreateDebugTrace()
        {
            return new DebugTrace();
        }
	}

    public class MvxProducerVisitTouchViewPresenter : MvxTouchViewPresenter
    {
        public MvxProducerVisitTouchViewPresenter(UIApplicationDelegate applicationDelegate, UIWindow window) : base(applicationDelegate, window)
        {
        }

        public override void Show(IMvxTouchView view)
        {

            if (view.Request.ViewModelType == typeof(NewVisitViewModel))
            {
                if (MasterNavigationController.TopViewController is NewVisitView)
                {
                    MasterNavigationController.PopViewControllerAnimated(false);
                }
            }
            base.Show(view);
        }
    }
}