// CallForm.iOS\ViewElements\DateTimePickerDialog_ViewController.cs

namespace CallForm.iOS.ViewElements
{
    using CallForm.iOS.Views;
    using MonoTouch.Foundation;
    using MonoTouch.UIKit;
    using System;
    using System.Drawing;

    public partial class DateTimePickerDialog_ViewController : UIViewController 
    {
        private Action<DateTime> _setValue = obj => { };
        private UIDatePicker _picker;
        private float _doneButtonHeight = 50f;

        /// <summary>Creates an instance of the <see cref="DateTimePickerDialog_ViewController"/> class.
        /// This holds the "content" inside the _popoverController.
        /// </summary>
        /// <param name="viewModel">The parent <see cref="MvxViewModel"/>.</param>
        /// <param name="source">The parent <see cref="UITableViewSource"/>.</param>
        /// <remarks>This ViewController is created when NewVisit_View is loaded.</remarks> 
        public DateTimePickerDialog_ViewController(Action<DateTime> setValue, DateTime initialValue, UIDatePickerMode mode, NewVisit_TableViewSource source)
        {
            #region _picker
            // ToDo: select time-zone from user preferences
            _picker = new UIDatePicker
            {
                Date = initialValue,
                Mode = mode,
                TimeZone = NSTimeZone.FromAbbreviation("GMT"),
            };

            //_picker.BackgroundColor = UIColor.Gray;
            //_picker.Alpha = 0.75f;
            //_picker.AutoresizingMask = UIViewAutoresizing.FlexibleBottomMargin | UIViewAutoresizing.FlexibleRightMargin;

            View.Add(_picker);
            #endregion _picker

            #region doneButton
            var doneButton = new UIButton(UIButtonType.System);
            doneButton.SetTitle("Done", UIControlState.Normal);
            doneButton.TouchUpInside += (sender, args) => { source.SafeDismissPopover(); };
            doneButton.Frame = new RectangleF(0, _picker.Frame.Height, _picker.Frame.Width, _doneButtonHeight);

            //View.Add(doneButton);
            #endregion doneButton

            //View.SizeToFit();
            View.BackgroundColor = UIColor.White; // this doesn't seem to matter

            _setValue += setValue;
        }

        #region overrides
        #pragma warning disable 1591
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

                // Hack: comment out to hide doneButton
                //size.Height = _picker.Frame.Size.Height + _buttonHeight;

                return size;
            }
            set { base.PreferredContentSize = value; }
        }

        /// <summary>Deprecated in iOS7 in favor of PreferredContentSize.
        /// </summary>
        [Obsolete("Deprecated in iOS7 in favor of PreferredContentSize.", true)]
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
        #pragma warning restore 1591
        #endregion overrides
    }
}
