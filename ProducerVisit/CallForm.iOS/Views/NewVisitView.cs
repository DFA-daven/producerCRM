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

    // notes: see _Touch UI.txt for design details.
    [Register("NewVisitView")]
    public class NewVisitView : MvxViewController
    {
        private UITableView _table;

        public override void ViewDidLoad()
        {
            UIColor viewBackgroundColor = UIColor.FromRGB(200, 200, 255);
            View = new UIView { BackgroundColor = viewBackgroundColor };
            base.ViewDidLoad();

            // Perform any additional setup after loading the view

            _table = new UITableView(UIScreen.MainScreen.Bounds)
            {
                BackgroundView = null
            };
            var source = new NewVisitTableViewSource(ViewModel as NewVisitViewModel, _table);
            source.DatePickerPopover = new DateTimePickerDialogViewController(
                val => (ViewModel as NewVisitViewModel).Date = val,
                (ViewModel as NewVisitViewModel).Date, UIDatePickerMode.Date, source);
            source.CallTypePickerPopover = new StringPickerDialogViewController(
                code => (ViewModel as NewVisitViewModel).CallType = code,
                (ViewModel as NewVisitViewModel).CallType, source,
                (ViewModel as NewVisitViewModel).CallTypes.ToArray());
            source.ReasonPickerPopover = new ReasonCodePickerDialogViewController(ViewModel as NewVisitViewModel, source);
            source.EmailPickerPopover = new EmailRecipientSelectDialogViewController(ViewModel as NewVisitViewModel, source);

            _table.Source = source;

            UIView wrapper = new UIView(new RectangleF(0, 0, _table.Frame.Width, 60));

            UIButton saveButton = new UIButton(UIButtonType.System);
            saveButton.Frame = new RectangleF(_table.Frame.Width / 4, 0, _table.Frame.Width / 2, 50);

            if (!(ViewModel as NewVisitViewModel).Editing)
            {
                UIButton reSendButton = new UIButton(UIButtonType.System);
                reSendButton.SetTitle("Forward via Email", UIControlState.Normal);
                reSendButton.TouchUpInside += ReSendEmail;
                reSendButton.Frame = new RectangleF(_table.Frame.Width / 5, 0, _table.Frame.Width / 4, 50);
                wrapper.AddSubview(reSendButton);
                saveButton.Frame = new RectangleF( 3 * _table.Frame.Width / 5, 0, _table.Frame.Width / 4, 50);
            }

            wrapper.AddSubview(saveButton);

            var set = this.CreateBindingSet<NewVisitView, NewVisitViewModel>();
            set.Bind(saveButton).For("Title").To(vm => vm.SaveButtonText);
            set.Bind(saveButton).To(vm => vm.SaveCommand);
            set.Bind(this).For(o => o.Title).To(vm => vm.Title);
            set.Apply();

            _table.TableFooterView = wrapper;
            Add(_table);
            _table.ReloadData();

            (ViewModel as NewVisitViewModel).Error += OnError;

            (ViewModel as NewVisitViewModel).SendEmail += OnSendEmail;

            (ViewModel as NewVisitViewModel).UserID = UIDevice.CurrentDevice.IdentifierForVendor.AsString();

            SetTableFrameForOrientation(InterfaceOrientation);
        }

        private void ReSendEmail(object sender, EventArgs eventArgs)
        {
            NewVisitViewModel viewModel = ViewModel as NewVisitViewModel;
            if (MFMailComposeViewController.CanSendMail)
            {
                MFMailComposeViewController mailView = new MFMailComposeViewController();
                mailView.SetSubject("Notes regarding contact with member " + viewModel.FarmNumber);
                if (viewModel.PictureBytes != null && viewModel.PictureBytes.Length > 0)
                {
                    mailView.AddAttachmentData(NSData.FromArray(viewModel.PictureBytes), "image/jpeg", "Picture.jpg");
                }

                // todo: add a message to the body to indicate if a picture was attached
                mailView.SetMessageBody(
                    "Member Number: " + viewModel.FarmNumber + "\n" +
                    "Contact Type: " + viewModel.CallType + "\n" +
                    "Date: " + viewModel.Date.ToShortDateString() + "\n" +
                    "Length of Call (hours): " + viewModel.Duration + "\n" +
                    "Reason(s) for Call: " + string.Join(", ", viewModel.ReasonCodes) + "\n" +
                    "Notes: " + viewModel.Notes + "\n"
                    , false);
                mailView.Finished += ReSendFinished;
                InvokeOnMainThread(() => PresentViewController(mailView, true, null));
            }
            else
            {
                InvokeOnMainThread(() => new UIAlertView("Error", "There are no mail accounts configured to send email.", null, "OK").Show());
            }
        }

        private void ReSendFinished(object sender, MFComposeResultEventArgs mfComposeResultEventArgs)
        {
            InvokeOnMainThread(() => DismissViewController(true, null));
        }

        public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillAnimateRotation(toInterfaceOrientation, duration);

            SetTableFrameForOrientation(toInterfaceOrientation);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            SetTableFrameForOrientation(InterfaceOrientation);
        }

        private void SetTableFrameForOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            switch (toInterfaceOrientation)
            {
                case UIInterfaceOrientation.Portrait:
                case UIInterfaceOrientation.PortraitUpsideDown:
                    // _table.Frame = UIScreen.MainScreen.Bounds;
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
            NewVisitViewModel viewModel = ViewModel as NewVisitViewModel;
            if (MFMailComposeViewController.CanSendMail)
            {
                MFMailComposeViewController mailView = new MFMailComposeViewController();
                List<string> recipientList = viewModel.EmailRecipients.Where(x => x != "Recipients Not Listed").ToList();
                if (recipientList.Count > 0)
                {
                    mailView.SetToRecipients(recipientList.ToArray());
                }
                mailView.SetSubject("Notes regarding contact with member " + viewModel.FarmNumber);
                if (viewModel.PictureBytes != null && viewModel.PictureBytes.Length > 0)
                {
                    mailView.AddAttachmentData(NSData.FromArray(viewModel.PictureBytes), "image/jpeg", "Picture.jpg");
                }
                // todo: add a message to the body to indicate if a picture was attached

                mailView.SetMessageBody(
                    "Member Number: " + viewModel.FarmNumber + "\n" +
                    "Contact Type: " + viewModel.CallType + "\n" +
                    "Date: " + viewModel.Date.ToShortDateString() + "\n" +
                    "Length of Call (hours): " + viewModel.Duration + "\n" +
                    "Reason(s) for Call: " + string.Join(", ", viewModel.ReasonCodes) + "\n" +
                    "Notes: " + viewModel.Notes + "\n"
                    ,false);
                mailView.Finished += MailViewOnFinished;
                InvokeOnMainThread(() => PresentViewController(mailView, true, null));
            }
            else
            {
                InvokeOnMainThread(() => new UIAlertView("Error", "There are no mail accounts configured to send email.", null, "OK").Show());
            }
        }

        private void MailViewOnFinished(object sender, MFComposeResultEventArgs e)
        {
            InvokeOnMainThread(() =>
            {
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

        private void OnError(object sender, ErrorEventArgs errorEventArgs)
        {
            InvokeOnMainThread(() => new UIAlertView("Error", errorEventArgs.Message, null, "OK").Show());
        }
    }

}