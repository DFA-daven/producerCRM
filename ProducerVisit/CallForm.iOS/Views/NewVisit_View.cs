namespace CallForm.iOS.Views
{
    using CallForm.Core.ViewModels;
    using CallForm.iOS.ViewElements;
    using Cirrious.MvvmCross.Binding.BindingContext;
    using Cirrious.MvvmCross.Touch.Views;
    using MonoTouch.Foundation;
    using MonoTouch.MessageUI;
    using MonoTouch.UIKit;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Reflection;

    // notes: see _Touch UI.txt for design details.
    /// <summary>An object representing a <c>NewVisit_View</c> view controller.
    /// </summary>
    [Register("NewVisit_View")]
    public class NewVisit_View : MvxViewController
    {
        #region Properties
        private UITableView _table;
        private float _frameWidth;
        string _nameSpace = "CallForm.iOS.";
        public float _buttonHeight = 50f;

        #endregion

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

        public override void ViewDidLoad()
        {
            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            CommonCore_iOS.DebugMessage(" > starting method...");

            View = new UIView { BackgroundColor = CommonCore_iOS.viewBackgroundColor };
            base.ViewDidLoad();

            // Perform any additional setup after loading the view

            //_table = new UITableView(UIScreen.MainScreen.Bounds)
            _table = new UITableView(View.Bounds)
            {
                BackgroundView = null
            };

            var source = new NewVisit_TableViewSource(ViewModel as NewVisit_ViewModel, _table);

            /* 
             * ToDo: iOS 7 design guidelines state that picker views should be presented in-line 
             * rather than as input views animated from the bottom of the screen of via a new 
             * controller pushed onto a navigation controller's stack, as in previous iOS versions. 
             * 
             * The system calendar app shows how this should now be implemented.
             */

            source.DatePickerPopover = new DateTimePickerDialog_ViewController(
                val => (ViewModel as NewVisit_ViewModel).Date = val, 
                (ViewModel as NewVisit_ViewModel).Date, 
                UIDatePickerMode.Date, 
                source);

            source.CallTypePickerPopover = new StringPickerDialog_ViewController(
                code => (ViewModel as NewVisit_ViewModel).SelectedCallType = code, 
                (ViewModel as NewVisit_ViewModel).SelectedCallType, 
                source,
                (ViewModel as NewVisit_ViewModel).ListOfCallTypes.ToArray());

            source.ReasonPickerPopover = new ReasonCodePickerDialog_ViewController(
                ViewModel as NewVisit_ViewModel, 
                source);

            source.EmailPickerPopover = new EmailRecipientSelectDialog_ViewController(
                ViewModel as NewVisit_ViewModel, 
                source);

            _table.Source = source;

            // define a sub-view for the saveButton and reSendButton
            UIView wrapper = new UIView(new RectangleF(0, 0, FrameWidth(), _buttonHeight));

            #region saveButton
            UIButton saveButton = new UIButton(UIButtonType.System);
            saveButton.Frame = new RectangleF(PercentOfFrameWidth(25), 0, PercentOfFrameWidth(50), ButtonHeight());

            if (!(ViewModel as NewVisit_ViewModel).Editing)
            {
                UIButton reSendButton = new UIButton(UIButtonType.System);
                reSendButton.SetTitle("Forward via Email", UIControlState.Normal);
                reSendButton.TouchUpInside += ReSendEmail;
                reSendButton.Frame = new RectangleF(PercentOfFrameWidth(20), 0, PercentOfFrameWidth(25), ButtonHeight());
               
                wrapper.Add(reSendButton);
                saveButton.Frame = new RectangleF(PercentOfFrameWidth(60), 0, PercentOfFrameWidth(25), ButtonHeight());
            }

            wrapper.Add(saveButton);
            #endregion saveButton

            var set = this.CreateBindingSet<NewVisit_View, NewVisit_ViewModel>();
            set.Bind(saveButton).For("Title").To(vm => vm.SaveButtonText);
            set.Bind(saveButton).To(vm => vm.SaveCommand);
            set.Bind(this).For(o => o.Title).To(vm => vm.Title);
            set.Apply();

            #region UI action

            #endregion UI action


            _table.TableFooterView = wrapper;
            Add(_table);
            _table.ReloadData();

            (ViewModel as NewVisit_ViewModel).Error += OnError;

            (ViewModel as NewVisit_ViewModel).SendEmail += OnSendEmail;

            // Note: the "AsString" method returns a string representation of the UUID. The "ToString" method returns the description (selector).
            // ToDo:  replace with the advertisingIdentifier property of the ASIdentifierManager class.
            (ViewModel as NewVisit_ViewModel).UserID = UIDevice.CurrentDevice.IdentifierForVendor.AsString();

            // Review: is it ok for this to be before the Height and Width are set?
            SetTableFrameForOrientation(InterfaceOrientation);

            (ViewModel as NewVisit_ViewModel).Height = FrameHeight();

            (ViewModel as NewVisit_ViewModel).Width = FrameWidth();

            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            CommonCore_iOS.DebugMessage(" > ...finished method.");
        }

        private void ReSendEmail(object sender, EventArgs eventArgs)
        {
            NewVisit_ViewModel viewModel = ViewModel as NewVisit_ViewModel;
            if (MFMailComposeViewController.CanSendMail)
            {
                MFMailComposeViewController mailView = new MFMailComposeViewController();
                mailView.SetSubject("Notes regarding contact with member " + viewModel.MemberNumber);
                if (viewModel.PictureBytes != null && viewModel.PictureBytes.Length > 0)
                {
                    mailView.AddAttachmentData(NSData.FromArray(viewModel.PictureBytes), "image/jpeg", "Picture.jpg");
                }

                // ToDo: add a message to the body to indicate if a picture was attached
                mailView.SetMessageBody(
                    "Member Number: " + viewModel.MemberNumber + "\n" +
                    "Contact Type: " + viewModel.SelectedCallType + "\n" +
                    "Date: " + viewModel.Date.ToShortDateString() + "\n" +
                    "Length of Call (hours): " + viewModel.Duration + "\n" +
                    "Reason(s) for Call: " + string.Join(", ", viewModel.SelectedReasonCodes) + "\n" +
                    "Notes: " + viewModel.Notes + "\n"
                    , false);
                mailView.Finished += ReSendFinished;
                InvokeOnMainThread(() => { PresentViewController(mailView, true, null); } ); 
            }
            else
            {
                InvokeOnMainThread(() => { new UIAlertView("Error", "There are no mail accounts configured to send email.", null, "OK").Show(); } );
            }
        }

        private void ReSendFinished(object sender, MFComposeResultEventArgs mfComposeResultEventArgs)
        {
            InvokeOnMainThread(() => { DismissViewController(true, null); } );
        }

        public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);

            base.WillAnimateRotation(toInterfaceOrientation, duration);

            SetTableFrameForOrientation(toInterfaceOrientation);
        }

        public override void ViewWillAppear(bool animated)
        {
            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);

            base.ViewWillAppear(animated);
            SetTableFrameForOrientation(InterfaceOrientation);
        }

        private void SetTableFrameForOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);

            switch (toInterfaceOrientation)
            {
                case UIInterfaceOrientation.Portrait:
                case UIInterfaceOrientation.PortraitUpsideDown:
                    // _reportTableView.Frame = UIScreen.MainScreen.Bounds;
                    _table.Frame = new RectangleF(0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height);
                    break;
                case UIInterfaceOrientation.LandscapeLeft:
                case UIInterfaceOrientation.LandscapeRight:
                    _table.Frame = new RectangleF(0, 0, UIScreen.MainScreen.Bounds.Height, UIScreen.MainScreen.Bounds.Width);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("toInterfaceOrientation");
            }
        }

        private void OnSendEmail(object sender, EventArgs eventArgs)
        {
            NewVisit_ViewModel viewModel = ViewModel as NewVisit_ViewModel;
            if (MFMailComposeViewController.CanSendMail)
            {
                MFMailComposeViewController mailView = new MFMailComposeViewController();
                List<string> recipientList = viewModel.SelectedEmailAddresses.Where(x =>  x != "Recipients Not Listed" ).ToList();
                if (recipientList.Count > 0)
                {
                    mailView.SetToRecipients(recipientList.ToArray());
                }
                mailView.SetSubject("Notes regarding contact with member " + viewModel.MemberNumber);
                if (viewModel.PictureBytes != null && viewModel.PictureBytes.Length > 0)
                {
                    mailView.AddAttachmentData(NSData.FromArray(viewModel.PictureBytes), "image/jpeg", "Picture.jpg");
                }
                // ToDo: add a message to the body to indicate if a picture was attached

                mailView.SetMessageBody(
                    "Member Number: " + viewModel.MemberNumber + "\n" +
                    "Contact Type: " + viewModel.SelectedCallType + "\n" +
                    "Date: " + viewModel.Date.ToShortDateString() + "\n" +
                    "Length of Call (hours): " + viewModel.Duration + "\n" +
                    "Reason(s) for Call: " + string.Join(", ", viewModel.SelectedReasonCodes) + "\n" +
                    "Notes: " + viewModel.Notes + "\n"
                    ,false);
                mailView.Finished += MailViewOnFinished;
                InvokeOnMainThread(() => { PresentViewController(mailView, true, null); } );
            }
            else
            {
                InvokeOnMainThread(() => { new UIAlertView("Error", "There are no mail accounts configured to send email.", null, "OK").Show(); } );
            }
        }

        private void MailViewOnFinished(object sender, MFComposeResultEventArgs e)
        {
            InvokeOnMainThread(() =>
            {
                // Note: The Navigation Controller is a UI-less View Controller responsible for
                // managing a stack of View Controllers and provides tools for navigation, such 
                // as a navigation bar with a back button.
                DismissViewController(true, null);
                if (sender == null)
                {
                    NavigationController.PopViewControllerAnimated(true);
                    return;
                }
                if (e.Result == MFMailComposeResult.Sent)
                {
                    NavigationController.PopViewControllerAnimated(true);
                    return;
                }
                if (e.Result == MFMailComposeResult.Cancelled)
                {
                    NavigationController.PopViewControllerAnimated(true);
                    return;
                }
                if (e.Error != null)
                {
                    new UIAlertView("Error", e.Error.LocalizedDescription, null, "OK").Show();
                }
            });
        }

        /// <summary>Displays the error issued by the <c>ViewModel</c> .
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="errorEventArgs"></param>
        private void OnError(object sender, ErrorEventArgs errorEventArgs)
        {
            InvokeOnMainThread(() => { new UIAlertView("Error", errorEventArgs.Message, null, "OK").Show(); } );
        }

        internal float ButtonHeight()
        {
            float height = UIFont.SystemFontSize * 3f;
            //return 50;
            return height;
        }

        /// <summary>The height of the current <see cref="UIView.Frame"/>.
        /// </summary>
        /// <returns>The Frame height.</returns>
        public float FrameHeight()
        {
            return _table.Frame.Height;
        }

        /// <summary>The width of the current <see cref="UIView.Frame"/>.
        /// </summary>
        /// <returns>The Frame width.</returns>
        public float FrameWidth()
        {
            return _table.Frame.Width;
        }

        /// <summary>Calculates a value representing a percent of the <see cref="UIView.Frame"/> width.
        /// </summary>
        /// <param name="percent">A percent value. Ex: 25.0</param>
        /// <returns>The product of (<see cref="UIView.Frame"/> * <paramref name="percent"/>)</returns>
        internal float PercentOfFrameWidth(double percent)
        {
            float value = (float)Math.Round( PercentOfRectangleWidth(_table.Frame, percent), 0);
            return value;
        }

        /// <summary>Calculates a value representing a <paramref name="percent"/> of the <paramref name="rectangle"/> width.
        /// </summary>
        /// <param name="rectangle">The <see cref="RectangleF"/> object.</param>
        /// <param name="percent">A percent value. Ex: 25.0</param>
        /// <returns>The product of (<paramref name="rectangle">rectangle.Width</see> * <paramref name="percent"/>)</returns>
        internal float PercentOfRectangleWidth(RectangleF rectangle, double percent)
        {
            percent = percent / 100;
            float value = (float)Math.Round((rectangle.Width * percent), 0);
            return value;
        }
    }
}