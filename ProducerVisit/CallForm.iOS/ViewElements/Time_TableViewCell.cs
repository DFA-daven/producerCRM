using System;
using System.Drawing;
using MonoTouch.UIKit;

namespace CallForm.iOS.ViewElements
{
    class Time_TableViewCell : UITableViewCell
    {
        private readonly UIButton _setToNow;

        public Time_TableViewCell(string cellID, bool editing, string buttonText, Action onClick) : base(UITableViewCellStyle.Value1, cellID)
        {
            _setToNow = new UIButton(UIButtonType.System);
            _setToNow.SetTitle(buttonText, UIControlState.Normal);
            _setToNow.TouchUpInside += (sender, args) => { onClick(); };
            if (editing)
            {
                ContentView.Add(_setToNow);
            }
        }

        #region overrides
        #pragma warning disable 1591
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            _setToNow.Frame = new RectangleF(ContentView.Bounds.Width / 2 - 75, 5, 150, 60);
        }
        #pragma warning restore 1591
        #endregion overrides
    }
}
