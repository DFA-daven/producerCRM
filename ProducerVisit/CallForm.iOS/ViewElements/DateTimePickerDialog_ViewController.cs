// CallForm.iOS\ViewElements\DateTimePickerDialog_ViewController.cs

namespace CallForm.iOS.ViewElements
{
    using CallForm.iOS.Views;
    using MonoTouch.Foundation;
    using MonoTouch.UIKit;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public partial class DateTimePickerDialog_ViewController : UIViewController
    {
        private Action<DateTime> _setValue = obj => { };
        private UIDatePicker _picker;

        public DateTimePickerDialog_ViewController(Action<DateTime> setValue, DateTime initialValue, UIDatePickerMode mode, NewVisit_TableViewSource source)
        {
            // ToDo: select time-zone from user preferences
            _picker = new UIDatePicker
            {
                Date = initialValue,
                Mode = mode,
                TimeZone = NSTimeZone.FromAbbreviation("GMT"),
            };

            //_picker.BackgroundColor = UIColor.Gray;
            //_picker.Alpha = 0.75f;
            _picker.AutoresizingMask = UIViewAutoresizing.FlexibleBottomMargin | UIViewAutoresizing.FlexibleRightMargin;

            // ToDo: replace AddSubview() with Add()
            View.AddSubview(_picker);
            View.SizeToFit();

            var doneButton = new UIButton(UIButtonType.System);
            doneButton.SetTitle("Done", UIControlState.Normal);
            doneButton.TouchUpInside += (sender, args) => { source.DismissPopover(); };
            doneButton.Frame = new RectangleF(0, _picker.Frame.Height, _picker.Frame.Width, 50);

            // ToDo: replace AddSubview() with Add()
            View.AddSubview(doneButton);

            View.BackgroundColor = UIColor.White;

            _setValue += setValue;
        }

        /// <summary>Specify that this View should *not* be displayed beneath the
        /// Status Bar (or the Navigation Bar, if present).
        /// </summary>
        public override UIRectEdge EdgesForExtendedLayout
        {
            get
            {
                return UIRectEdge.None;
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            _setValue(_picker.Date);
        }

        public override SizeF PreferredContentSize
        {
            get
            {
                SizeF size = _picker.Frame.Size;
                // Hack: hide doneButton
                //size.Height += 50;
                return size;
            }
            set { base.PreferredContentSize = value; }
        }
    }
}
