namespace CallForm.iOS.Screens
{
    using System.Drawing;

    using MonoTouch.UIKit;
    using MonoTouch.Foundation;

    // Review: not used. Stub left-over from initial MvvmCross example.
    [Register("UniversalView")]
    public class UniversalView : UIView
    {
        public UniversalView()
        {
            Initialize();
        }

        public UniversalView(RectangleF bounds) : base(bounds)
        {
            Initialize();
        }

        void Initialize()
        {
            BackgroundColor = UIColor.Red;
        }
    }

    // Review: not used. Stub left-over from initial MvvmCross example.
    [Register("ConnectionScreen")]
    public class ConnectionScreen : UIViewController
    {
        public ConnectionScreen()
        {
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            View = new UniversalView();

            base.ViewDidLoad();

            // Perform any additional setup after loading the view
        }
    }
}