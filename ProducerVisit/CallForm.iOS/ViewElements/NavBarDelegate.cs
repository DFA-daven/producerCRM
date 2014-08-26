namespace CallForm.iOS.ViewElements
{
    using MonoTouch.Foundation;
    using MonoTouch.UIKit;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Text;

    class NavBarDelegate : UINavigationBarDelegate
    {
        public override UIBarPosition GetPositionForBar(IUIBarPositioning barPositioning)
        {
            return UIBarPosition.TopAttached;
        }
    }
}