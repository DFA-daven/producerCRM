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
        /// <summary>The View that contains the data-entry controls (which are defined in <see cref="NewVisit_TableViewSource"/>).
        /// </summary>
        private UITableView _table; 
        private float _frameWidth;
        string _nameSpace = "CallForm.iOS.";
        public float _buttonHeight = 50f;
        bool? _isOS7;


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
            CommonCore_iOS.DebugMessage("  [nv_v][vdl] > starting method...");

            View = new UIView { BackgroundColor = CommonCore_iOS.viewBackgroundColor };

            base.ViewDidLoad();

            // Perform any additional setup after loading the view

            //_table = new UITableView(UIScreen.MainScreen.Bounds)
            _table = new UITableView(View.Bounds) // *****
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
            float navbarHeight = 11f;
            //topMargin = NavigationController.NavigationBar.Frame.Height; // the nearest ANCESTOR NavigationController
            // topMargin will probably be 44

            float screenHeight = UIScreen.MainScreen.Bounds.Height;
            //float viewFrameHeight = View.Frame.Height; // *****
            float viewFrameHeight = LayoutHeight() ; // *****

            float heightOfVisibleView = 22f;
            //heightOfVisibleView = NavigationController.VisibleViewController.View.Frame.Height;

            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            CommonCore_iOS.DebugMessage("  [nv_v][vdl] > screenHeight: " + screenHeight.ToString() + ", viewFrameHeight: " + viewFrameHeight.ToString() + ", heightOfVisibleView: " + heightOfVisibleView.ToString() + " <======= ");

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
            saveButton.BackgroundColor = UIColor.Red;

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

            float tableFrameHeight = _table.Frame.Height;

            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            CommonCore_iOS.DebugMessage("  [nv_v][vdl] > tableFrameHeight: " + tableFrameHeight.ToString() + " <======= ");

            (ViewModel as NewVisit_ViewModel).Error += OnError;

            (ViewModel as NewVisit_ViewModel).SendEmail += OnSendEmail;

            // Note: the "AsString" method returns a string representation of the UUID. The "ToString" method returns the description (selector).
            // ToDo:  replace with the advertisingIdentifier property of the ASIdentifierManager class.
            (ViewModel as NewVisit_ViewModel).UserID = UIDevice.CurrentDevice.IdentifierForVendor.AsString();

            // Review: double-check -- this should be setting the height/width for the **table** that holds the model
            SetTableFrameForOrientation(InterfaceOrientation); // use current orientation

            //(ViewModel as NewVisit_ViewModel).Height = FrameHeight();

            //(ViewModel as NewVisit_ViewModel).Width = FrameWidth();

            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            CommonCore_iOS.DebugMessage("  [nv_v][vdl] > (ViewModel as NewVisit_ViewModel).Height: " + (ViewModel as NewVisit_ViewModel).Height.ToString() + " <= = = = = ");
            CommonCore_iOS.DebugMessage("  [nv_v][vdl] > ...finished method.");
        }

        public override void ViewDidLayoutSubviews()
        {
            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);

            #region colorize subviews
            UIView[] subviews = new UIView[] { };
            int subviewsArrayLength = 0;
            UIView[] subSubviews = new UIView[] { };
            int subSubviewsArrayLength = 0;
            
            string subviewType = string.Empty;

            UIColor[] colors = new UIColor[] { UIColor.Cyan, UIColor.Blue, UIColor.Brown, UIColor.Green, UIColor.Magenta, UIColor.Orange, UIColor.Purple, UIColor.Red };
            UIColor[] rgbGrays = new UIColor[] { UIColor.FromRGB(223, 223, 223), UIColor.FromRGB(191, 191, 191), UIColor.FromRGB(159, 159, 159), UIColor.FromRGB(127, 127, 127), UIColor.FromRGB(95, 95, 95), UIColor.FromRGB(63, 63, 63), UIColor.FromRGB(31, 31, 31)};

            int colorArrayLength = colors.Length;
            int grayArrayLength = rgbGrays.Length;
            

            // Note: iOS 7 numbers Subviews differently from iOS 6.
            // _table.Subviews[0]: iOS 6: the footer (which contains the saveButton).
            //                     iOS 7: the foreground **below** the _table (on the same depth as the controls)
            // _table.Subviews[1]: iOS 6: UNDEFINED
            //                     iOS 7: the footer (which contains the saveButton).

#if (DEBUG)
            int colorIndex = 0;
            int grayIndex = 0;

            // The _table is in the foreground -- color the (undefined) space **behind** the controls.
            // Drag the entire page up or down to see the BackgroundColor.
            _table.BackgroundColor = UIColor.Orange;
            View.BackgroundColor = UIColor.Brown;

            subviews = View.Subviews;

            if (subviews == null)
            {
                CommonCore_iOS.DebugMessage("  [nv_v][vdls] > View.Subviews[] is NULL. How can the subview be null?!?");
            }
            else 
            {
                subviewsArrayLength = subviews.Length;
                CommonCore_iOS.DebugMessage("  [nv_v][vdls] > View.Subviews[] is NOT null. subviewsArrayLength = " + subviewsArrayLength.ToString() + ", colorArrayLength = " + colorArrayLength.ToString());
                // here

                for (int i = 0; i < subviewsArrayLength; i++)
                {
                    grayIndex = i % grayArrayLength; // use modulus to keep colorIndex from going out of range

                    if (subviews[i].GetType() == typeof(UIView))
                    {
                        CommonCore_iOS.DebugMessage("  [nv_v][vdls] > View.Subviews[" + i.ToString() + "] == typeof(UIView), Height = " + subviews[i].Frame.Height.ToString() + ", rgbGrays[" + grayIndex.ToString() + "] = " + rgbGrays[grayIndex].ToString());
                        subviews[i].BackgroundColor = rgbGrays[grayIndex]; // applying color should works here
                    }
                    else
                    {
                        subviewType = subviews[i].GetType().ToString();
                        CommonCore_iOS.DebugMessage("  [nv_v][vdls] > View.Subviews[" + i.ToString() + "] is wrapping something: Height = " + subviews[i].Frame.Height.ToString() + ", rgbGrays[" + grayIndex.ToString() + "] = " + rgbGrays[grayIndex].ToString());
                        // here
                        subviews[i].BackgroundColor = rgbGrays[grayIndex]; // applying color should not work here

                        subSubviews = subviews[i].Subviews;

                        if (subSubviews == null)
                        {
                            CommonCore_iOS.DebugMessage("  [nv_v][vdls] > View.Subviews[" + i.ToString() + "].Subviews[] is NULL. How can the subview be null?!?");
                        }
                        else
                        {
                            subSubviewsArrayLength = subSubviews.Length;
                            CommonCore_iOS.DebugMessage("  [nv_v][vdls] > View.Subviews[" + i.ToString() + "].Subviews[] is NOT null. subviewsArrayLength = " + subSubviewsArrayLength.ToString() + ", colorArrayLength = " + colorArrayLength.ToString());
                            // here
                            for (int j = 0; j < subSubviewsArrayLength; j++)
                            {
                                colorIndex = j % colorArrayLength; // use modulus to keep colorIndex from going out of range

                                if (subSubviews[j].GetType() == typeof(UIView))
                                {
                                    // this is probably a header or footer
                                    CommonCore_iOS.DebugMessage("  [nv_v][vdls] > View.Subviews[" + i.ToString() + "][" + j.ToString() + "] == typeof(UIView), Height = " + subSubviews[j].Frame.Height.ToString() + ", colors[" + colorIndex.ToString() + "] = " + colors[colorIndex].ToString());
                                    subSubviews[j].BackgroundColor = colors[colorIndex]; 
                                }
                                else
                                {
                                    // Note: not sure why, but the height values indicate that these are listed 
                                    // in reverse order. So, item [0] is the Email recipients, which is the 
                                    // last/bottom member of the view. 
                                    // this is a view that wraps something else
                                    subviewType = subSubviews[j].GetType().ToString();
                                    CommonCore_iOS.DebugMessage("  [nv_v][vdls] > View.Subviews[" + i.ToString() + "][" + j.ToString() + "] is wrapping something: Height = " + subSubviews[j].Frame.Height.ToString());
                                    //string wrappedType = subSubviews[j].Subviews[0].GetType().ToString();
                                    // this isn't getting down far enough to see the wrapped object
                                    //CommonCore_iOS.DebugMessage("  [nv_v][vdls] > type of the wrapped object: " + wrappedType);
                                }
                            }
                        }
                    }
                }
            }
#endif
            #endregion colorize subviews

            base.ViewDidLayoutSubviews();
            
            float screenHeight = UIScreen.MainScreen.Bounds.Height;
            CommonCore_iOS.DebugMessage("  [nv_v][vdls] > screenHeight: " + screenHeight.ToString() + ", viewFrameHeight: " + View.Frame.Height.ToString() + ", LayoutHeight(): " + LayoutHeight().ToString());

            /*
                * Note: the TopLayoutGuide and BottomLayoutGuide values are generated dynamically AFTER
                * the View has been added to the hierarchy, so attempting to read them in ViewDidLoad will return 0. 
                * So, calculate the value after the View has loaded, for example here in ViewDidLoadSubviews.
                */
            /*
                * Note: EdgesForExtendedLayout may allow this app to display, but using TopLayoutGuide
                * and BottomLayoutGuide are preferred since they allow the app to meet the iOS 7 design goals.
                */
            float displacement_y = 0f;
            float bottomGuide = 0f;

            bool isOS7orLater = IsOS7OrLater();
            if (isOS7orLater)
            {
                displacement_y = this.TopLayoutGuide.Length;
                bottomGuide = this.BottomLayoutGuide.Length;
            }

            CommonCore_iOS.DebugMessage("  [nv_v][vdls] > this.TopLayoutGuide.Length: " + displacement_y.ToString() + " this.BottomLayoutGuide.Length: " + bottomGuide.ToString() + " <======= ");
            CommonCore_iOS.DebugMessage("  [nv_v][vdls] > ...finished");
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

            SetTableFrameForOrientation(toInterfaceOrientation); // use next orientation
        }

        public override void ViewWillAppear(bool animated)
        {
            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);

            base.ViewWillAppear(animated);

            SetTableFrameForOrientation(InterfaceOrientation); // use current orientation
        }

#pragma warning disable 612,618
        [Obsolete("Deprecated in iOS6. Replace it with both GetSupportedInterfaceOrientations and PreferredInterfaceOrientationForPresentation", false)]
        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)  // iOS4/iOS5 only
        {
            bool rotate = false;

            if (toInterfaceOrientation == InterfaceOrientation)
            {
                rotate = true;
            }

            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            CommonCore_iOS.DebugMessage("  [nv_v][satio] > ShouldAutorotateToInterfaceOrientation = " + rotate.ToString() + " < [ersd_vc][satio]");

            return rotate;
        }
#pragma warning restore 612,618

        private void SetTableFrameForOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            float usableHeight = 0f;
            float usableWidth = 0f;
            float topMarginHeight = TopMargin();

            switch (toInterfaceOrientation)
            {
                case UIInterfaceOrientation.Portrait:
                case UIInterfaceOrientation.PortraitUpsideDown:
                    // _reportTableView.Frame = UIScreen.MainScreen.Bounds;
                    usableHeight = UIScreen.MainScreen.Bounds.Height - topMarginHeight;
                    _table.Frame = new RectangleF(0, 0, UIScreen.MainScreen.Bounds.Width, usableHeight);
                    break;
                case UIInterfaceOrientation.LandscapeLeft:
                case UIInterfaceOrientation.LandscapeRight:
                    usableWidth = UIScreen.MainScreen.Bounds.Width - topMarginHeight;
                    _table.Frame = new RectangleF(0, 0, UIScreen.MainScreen.Bounds.Height, usableWidth);
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
            CommonCore_iOS.DebugMessage("  [nv_v][bh] > ButtonHeight() = " + height.ToString() + " < ");
            //return 50;
            return height;
        }

        /// <summary>The height of the current <see cref="UIView.Frame"/>.
        /// </summary>
        /// <returns>The Frame height.</returns>
        public float FrameHeight()  // 1024 - (20 + 44) = 960
        {
            float usableHeight = 0f;
            float topMarginHeight = TopMargin();

            usableHeight = _table.Frame.Height - topMarginHeight;
            CommonCore_iOS.DebugMessage("  [nv_v][fh] > FrameHeight() = " + usableHeight.ToString() + " < ");

            return usableHeight;
        }

        /// <summary>The width of the current <see cref="UIView.Frame"/>.
        /// </summary>
        /// <returns>The Frame width.</returns>
        public float FrameWidth()
        {
            CommonCore_iOS.DebugMessage("  [nv_v][fw] > FrameWidth() = " + _table.Frame.Width.ToString() + " < ");

            return _table.Frame.Width;
        }

        /// <summary>Calculates a value representing a percent of the <see cref="UIView.Frame"/> width.
        /// </summary>
        /// <param name="percent">A percent value. Ex: 25.0</param>
        /// <returns>The product of (<see cref="UIView.Frame"/> * <paramref name="percent"/>)</returns>
        internal float PercentOfFrameWidth(double percent)
        {
            float value = (float)Math.Round( PercentOfRectangleWidth(_table.Frame, percent), 0);
            CommonCore_iOS.DebugMessage("  [nv_v][%fw] > PercentOfFrameWidth() = " + value.ToString() + " < ");

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
            CommonCore_iOS.DebugMessage("  [nv_v][%rw] > PercentOfRectangleWidth() = " + value.ToString() + " < ");

            return value;
        }

        /// <summary>The height of the device's screen.
        /// </summary>
        /// <returns>The screen height measured in points.</returns>
        internal float ViewFrameHeight()  // 960 
        {
            float viewFrameHeight = 0;
            //viewFrameHeight = UIScreen.MainScreen.Bounds.Height;
            viewFrameHeight = View.Frame.Height;
            
            if (View.Frame.Height < 1)
            {
                CommonCore_iOS.DebugMessage("  [nv_v][vfh] > View not ready... " );
                return viewFrameHeight;
            }

            switch (InterfaceOrientation)
            {
                case UIInterfaceOrientation.Portrait:
                case UIInterfaceOrientation.PortraitUpsideDown:
                    //viewFrameHeight = View.Frame.Height;
                    break;

                case UIInterfaceOrientation.LandscapeLeft:
                case UIInterfaceOrientation.LandscapeRight:
                    viewFrameHeight = View.Frame.Width;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("ViewFrameHeight");
            }
            CommonCore_iOS.DebugMessage("  [nv_v][vfh] > iOS 7 = " + IsOS7OrLater().ToString() + " > ViewFrameHeight(): " + viewFrameHeight.ToString());

            return viewFrameHeight;
        }

        private float TopMargin()
        {
            float topMargin = 0f;

            topMargin = StatusBarHeight() + NavBarHeight();

            if (View.Frame.Height < 1)
            {
                CommonCore_iOS.DebugMessage("  [nv_v][tm] > View not ready... ");
                if (IsOS7OrLater())
                {
                    topMargin = 20f;
                }
                else
                {
                    topMargin = 64f;
                }
            }
            
            CommonCore_iOS.DebugMessage("  [nv_v][tm] > topMargin: " + topMargin.ToString());

            return topMargin;
        }

        private float LayoutHeight()
        {
            float layoutHeight = 0f;
            UIView[] subviews = View.Subviews;

            CommonCore_iOS.DebugMessage("  [nv_v][lh] > Calculating layoutHeight....");
            if (subviews == null)
            {
                CommonCore_iOS.DebugMessage("  [nv_v][lh] > View.Subviews[] is NULL.");
            }
            else
            {
                int subviewsArrayLength = subviews.Length;
                CommonCore_iOS.DebugMessage("  [nv_v][lh] > View.Subviews[] is NOT null. subviewsArrayLength = " + subviewsArrayLength.ToString());
                for (int i = 0; i < subviewsArrayLength; i++)
                {
                    if (subviews[i].GetType() == typeof(UIView))
                    {
                        CommonCore_iOS.DebugMessage("  [nv_v][lh] > View.Subviews[" + i.ToString() + "] == typeof(UIView), Height = " + subviews[i].Frame.Height.ToString());
                    }
                    else
                    {
                        CommonCore_iOS.DebugMessage("  [nv_v][lh] > View.Subviews[" + i.ToString() + "] is wrapping something: Height = " + subviews[i].Frame.Height.ToString());
                        layoutHeight = subviews[i].Frame.Height;
                    }
                }
            }

            if (layoutHeight == 0)
            {
                CommonCore_iOS.DebugMessage("  [nv_v][lh] > layoutHeight was 0, substituting View.Frame.Height: " + View.Frame.Height.ToString());
                layoutHeight = View.Frame.Height;
            }

            CommonCore_iOS.DebugMessage("  [nv_v][lh] > layoutHeight = " + layoutHeight.ToString() + " (finished).");

            return layoutHeight;
        }

        private float NavBarHeight()  // 44
        {
            float screenHeight = UIScreen.MainScreen.Bounds.Height;
            float layoutHeight = 0f;
            float navbarHeight = 0f;
            UIView[] subviews = View.Subviews;

            if (IsOS7OrLater())
            {
                navbarHeight = NavigationController.NavigationBar.Frame.Height; // the nearest ANCESTOR NavigationController
            }
            else
            {
                navbarHeight = 44f;
            }

            CommonCore_iOS.DebugMessage("  [nv_v][nbh] > navbarHeight = " + navbarHeight.ToString() + " < = = = =");
            return navbarHeight;

            #region incomplete
            // ToDo: this works in portrait, but after the first rotation it returns 300, 556, 
            CommonCore_iOS.DebugMessage("  [nv_v][nbh] > Calculating navbarHeight....");
            
            // skip if we're not ready
            if (View.Frame.Height < 1)
            {
                CommonCore_iOS.DebugMessage("  [nv_v][nbh] > View not ready... ");
                if (IsOS7OrLater())
                {
                    return 44f;
                }
                else
                {
                    return 44f;
                }
            }

            layoutHeight = LayoutHeight();

            // Note: sometimes the layoutHeight will default to View.Frame.Height, which could equal the screenHeight.
            // navbarHeight could = 0
            navbarHeight = screenHeight - layoutHeight;

            // Note: comparisons of floating point values (double and float) are problematic because of the imprecision of floating point arithmetic on binary computers.
            if ((int)navbarHeight == (int)screenHeight)
            {
                CommonCore_iOS.DebugMessage("  [nv_v][nbh] > (int)navbarHeight == (int)screenHeight) == " + navbarHeight.ToString() + " < = =" );

                navbarHeight = 0f;
                return navbarHeight;
            }

            if (IsOS7OrLater())
            {
                CommonCore_iOS.DebugMessage("  [nv_v][nbh] > IsOS7OrLater() = true, navbarHeight = " + navbarHeight.ToString() + " < = = = " );
                navbarHeight = NavigationController.NavigationBar.Frame.Height; // the nearest ANCESTOR NavigationController
            }
            else
            {
                CommonCore_iOS.DebugMessage("  [nv_v][nbh] > IsOS7OrLater() = FALSE, navbarHeight = " + navbarHeight.ToString() + " - " + StatusBarHeight().ToString() + " = " + (navbarHeight - StatusBarHeight()).ToString() + " < = = = =" );
                navbarHeight = navbarHeight - StatusBarHeight();
            }

            return navbarHeight;
            #endregion
        }

        private float StatusBarHeight()  // 20 
        {
            SizeF statusBarFrameSize = UIApplication.SharedApplication.StatusBarFrame.Size;
            float statusBarHeight = Math.Min(statusBarFrameSize.Width, statusBarFrameSize.Height);
            CommonCore_iOS.DebugMessage("  [nv_v][sbh] > statusBarHeight: " + statusBarHeight.ToString() + " <= = = = = ");

            return statusBarHeight;
        }

        /// <summary>Is this device running iOS 7.0.
        /// </summary>
        /// <returns>True if this is iOS 7.0.</returns>
        public bool IsOS7OrLater()
        {
            bool thisIsOS7 = false;
            
            if (_isOS7 == null)
            {
                string version = UIDevice.CurrentDevice.SystemVersion;
                string[] parts = version.Split('.');
                string major = parts[0];
                int majorVersion = CommonCore_iOS.SafeConvert(major, 0);

                if (majorVersion > 6)
                {
                    //float displacement_y = this.TopLayoutGuide.Length;

                    thisIsOS7 = true;
                }

                CommonCore_iOS.DebugMessage("  [nv_v][i7ol] > major version (string): " + major);
                CommonCore_iOS.DebugMessage("  [nv_v][i7ol] > major version (int): " + majorVersion.ToString());
                CommonCore_iOS.DebugMessage("  [nv_v][i7ol] > version is higher than 6 = " + thisIsOS7.ToString());

                _isOS7 = thisIsOS7;
            }
            else
            {
                thisIsOS7 = (bool)_isOS7;
            }

            return thisIsOS7;
        }
    }
}