using System;
using System.Drawing;
using CallForm.iOS.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CallForm.iOS.ViewElements
{
    public class DateTimePickerDialogViewController : UIViewController
    {
        private Action<DateTime> _setValue = obj => { };
        private readonly UIDatePicker _picker;

        public DateTimePickerDialogViewController(Action<DateTime> setValue, DateTime initialValue, UIDatePickerMode mode, NewVisitTableViewSource source)
        {
            _picker = new UIDatePicker
            {
                Date = initialValue,
                Mode = mode,
                TimeZone = NSTimeZone.FromAbbreviation("GMT")
            };
            View.AddSubview(_picker);

            var doneButton = new UIButton(UIButtonType.System);
            doneButton.SetTitle("Done", UIControlState.Normal);
            doneButton.TouchUpInside += (sender, args) => source.DismissPopover();
            doneButton.Frame = new RectangleF(0, _picker.Frame.Height, _picker.Frame.Width, 50);

            View.AddSubview(doneButton);

            View.BackgroundColor = UIColor.White;

            _setValue += setValue;
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            _setValue(_picker.Date);
        }

        public override SizeF ContentSizeForViewInPopover
        {
            get
            {
                SizeF size = _picker.Frame.Size;
                size.Height += 50;
                return size;
            }
            set { base.ContentSizeForViewInPopover = value; }
        }
    }
}