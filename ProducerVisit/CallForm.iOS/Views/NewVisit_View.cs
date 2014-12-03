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
    //using System.Reflection;

    // notes: see _Touch UI.txt for design details.

    /// <summary>Creates a class representing the UI View for "NewVisit". 
    /// </summary>
    /// <remarks><para>This view inherits from <see cref="MvxViewController"/>. The main over-ridable methods 
    /// here handle the view's life-cycle.</para>
    /// <para>Design goal is to only deal with formatting and layout of data here -- no state information.</para></remarks>
    [Register("NewVisit_View")]
    public class NewVisit_View : MvxViewController
    {
        #region Properties
        string _namespace = "CallForm.iOS.Views.";
		string _class = "NewVisit_View.";
		string _method = "TBD";

        /// <summary>Class name abbreviation
        /// </summary>
        string _cAbb = "[nv_v]";

        /// <summary>The View that contains the data-entry controls (which are defined in <see cref="NewVisit_TableViewSource"/>).
        /// </summary>
        private UITableView _table; 
        private float _frameWidth = 0f;

        private bool _isOS7OrLater;
        public bool IsOS7OrLater
        {
            get { return _isOS7OrLater; }
            set { _isOS7OrLater = value; }
        }

        private bool _isOS8OrLater;
        public bool IsOS8OrLater
        {
            get { return _isOS8OrLater; }
            set { _isOS8OrLater = value; }
        }

        private float _statusBarHeight = 0f;
        public float StatusBarHeight
        {
            get { return _statusBarHeight; }
            set 
            { 
                _statusBarHeight = value; 
            }
        }

        private float _navBarHeight = 0f;
        public float NavBarHeight
        {
            get { return _navBarHeight; }
            set 
            { 
                _navBarHeight = value; 
            }
        }

        /// <summary>Store for the <c>ButtonHeight</c> property.</summary>
        private float _buttonHeight = 0f;
        public float ButtonHeight
        {
            get { return _buttonHeight; }
            set
            {
                _buttonHeight = value;
                //RaisePropertyChanged(() => ButtonHeight);
            }
        }

        /// <summary>Store for the <c>RowHeight</c> property.</summary>
        private float _rowHeight = 0f;
        public float RowHeight
        {
            get { return _rowHeight; }
            set
            {
                _rowHeight = value;
                //RaisePropertyChanged(() => RowHeight);
            }
        }
        /// <summary>Store for the <c>Portrait</c> property.</summary>
        private bool _portrait;

        /// <summary>Keeps track of portrait (or landscape) orientation.
        /// </summary>
        /// <remarks>This is a property that SHOULD be in the ViewController.</remarks>
        public bool Portrait
        {
            get { return _portrait; }
            set
            {
                // Undone: implement this in order to track orientation changes.
                _portrait = value;
                //RaisePropertyChanged(() => Height);
                //RaisePropertyChanged(() => Width);
                //RaisePropertyChanged(() => RowHeight);
            }
        }
        #endregion

        UIBarButtonItem trashBBI;
        UIBarButtonItem searchBBI;
        public NewVisit_View()
        {
            Common_iOS.DebugMessage("  [nv_v][nv_v] > Creating new instance...");

            Editing = (ViewModel as NewVisit_ViewModel).IsNewReport;

            //IsOS8OrLater = Common_iOS.iOSVersionOK;
            StatusBarHeight = FindStatusBarHeight();
            NavBarHeight = FindNavBarHeight();

            // FixMe: hard-coded values -- calculate these from the screen dimensions?
            ButtonHeight = (float)Math.Round((UIFont.SystemFontSize * 3f), 0);
            RowHeight = 50f;

            try
            {
                #region UIBarButtonItems
                // ToDo: check if we're using local or remote data
                bool producerCanBeSearched = true;

                // ToDo: check if this record can be deleted
                bool recordCanBeDeleted = true;

                if (Editing)
                {
                    trashBBI = new UIBarButtonItem(UIBarButtonSystemItem.Trash, (sender, e) =>
                    {
                        string message = "  [nv_v][nv_v] > 'Search' (New Visit report) clicked.";
                        Console.WriteLine(message);
                    });

                    trashBBI.Enabled = recordCanBeDeleted;
                    trashBBI.TintColor = UIColor.Red;

                    NavigationItem.SetRightBarButtonItem(trashBBI, false);
                }
                else
                {
                    searchBBI = new UIBarButtonItem(UIBarButtonSystemItem.Search, (sender, e) =>
                    {
                        string message = "  [nv_v][nv_v] > 'Trash' (New Visit report) clicked.";
                        Console.WriteLine(message);
                    });

                    searchBBI.Enabled = producerCanBeSearched;

                    // ToDo: add logic to check if this is a new or existing record
                    // if (is a new record)
                    // {
                    NavigationItem.SetRightBarButtonItem(searchBBI, false);
                    // }
                }
                #endregion


            }
            finally
            {
                Common_iOS.DebugMessage("  [nv_v][nv_v] > New instance created.");
            }
            
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
				_method = "EdgesForExtendedLayout";
				Common_iOS.DebugMessage(_namespace + _class, _method);

                return UIRectEdge.None;
            }
        }

        public override void ViewDidLoad()
        {
			_method = "ViewDidLoad";
			Common_iOS.DebugMessage(_namespace + _class, _method);
            Common_iOS.DebugMessage("  [nv_v][vdl] > starting method...");

            #region view 
            View = new UIView { BackgroundColor = Common_iOS.viewBackgroundColor };

            base.ViewDidLoad();

            // Perform any additional setup after loading the view

            //_table = new UITableView(UIScreen.MainScreen.Bounds)
            _table = new UITableView(View.Bounds) // *****
            {
                BackgroundView = null
            };

            var source = new NewVisit_TableViewSource(ViewModel as NewVisit_ViewModel, _table);

            float screenHeight = UIScreen.MainScreen.Bounds.Height;
            float viewFrameHeight = LayoutHeight(); // *****
            float heightOfVisibleView = NavigationController.VisibleViewController.View.Frame.Height;

            Common_iOS.DebugMessage("  [nv_v][vdl] > screenHeight: " + screenHeight.ToString() + ", viewFrameHeight: " + viewFrameHeight.ToString() + ", heightOfVisibleView: " + heightOfVisibleView.ToString() + " <======= ");
            #endregion view 

            #region popovers
            /* 
             * ToDo: iOS 7 design guidelines state that picker views should be presented in-line 
             * rather than as input views animated from the bottom of the screen of via a new 
             * controller pushed onto a navigation controller's stack, as in previous iOS versions. 
             * 
             * The system calendar app shows how this should now be implemented.
             */

            // at this point the "popovers" are still UIViewController
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
            #endregion popovers

            #region saveButton
            // define a sub-view for the saveButton and reSendButton
            float wrapperWidth = TableFrameWidth();
            float wrapperHeight = ButtonHeight; ;

            Common_iOS.DebugMessage("  [nv_v][vdl] > wrapperWidth: " + wrapperWidth.ToString()  + ", wrapperHeight: " + wrapperHeight.ToString() + " < [nv_v][vdl]");
            UIView wrapper = new UIView(new RectangleF(0, 0, wrapperWidth, wrapperHeight));

            UIButton saveButton = new UIButton(UIButtonType.System);

            float saveButtonWidth = (float)Math.Round((wrapperWidth * 0.5), 0);
            float saveButtonHeight = wrapperHeight;

            Common_iOS.DebugMessage("  [nv_v][vdl] > saveButtonWidth: " + saveButtonWidth.ToString() + ", saveButtonHeight: " + saveButtonHeight.ToString() + " <======= ");
            saveButton.Frame = new RectangleF(PercentOfTableFrameWidth(25), 0, saveButtonWidth, saveButtonHeight );
            //saveButton.BackgroundColor = UIColor.Red;

            if (!(ViewModel as NewVisit_ViewModel).IsNewReport)
            {
                UIButton reSendButton = new UIButton(UIButtonType.System);
                reSendButton.SetTitle("Forward via Email", UIControlState.Normal);
                reSendButton.TouchUpInside += ReSendEmail;
                reSendButton.Frame = new RectangleF(PercentOfTableFrameWidth(20), 0, PercentOfTableFrameWidth(25), ButtonHeight);
               
                wrapper.Add(reSendButton);
                saveButton.Frame = new RectangleF(PercentOfTableFrameWidth(60), 0, PercentOfTableFrameWidth(25), ButtonHeight);
            }

            wrapper.Add(saveButton);
            #endregion saveButton

            // Note: this BindingDescriptionSet represents the link between the NewVisit_View and the NewVisit_ViewModel.
            var set = this.CreateBindingSet<NewVisit_View, NewVisit_ViewModel>();

            set.Bind(saveButton).For("Title").To(vm => vm.SaveButtonText);
            set.Bind(saveButton).To(vm => vm.SaveCommand);

            // Undone: need new command for producer search
            if (Editing)
            {
                set.Bind(trashBBI).To(vm => vm.SaveCommand);
            }
            else
            {
                set.Bind(searchBBI).To(vm => vm.SaveCommand);
            }

            set.Bind(this).For(o => o.Title).To(vm => vm.Title);
            set.Apply();



            #region UI action
            //trashBBI.Clicked += (sender, args) => { new UIAlertView("Error", "There are no mail accounts configured to send email.", null, "OK").Show(); };
            //InvokeOnMainThread(() => { new UIAlertView("Error", "There are no mail accounts configured to send email.", null, "OK").Show(); });


            #endregion UI action

            _table.TableFooterView = wrapper;
            Add(_table);
            _table.ReloadData();

            float tableFrameHeight = _table.Frame.Height;
            string sourceType = _table.Frame.GetType().ToString();

            if (tableFrameHeight < 1)
            {
                Common_iOS.DebugMessage("  " + _cAbb + "[vdl] > " + sourceType + " not ready... < !!!!! ");
            }

            Common_iOS.DebugMessage("  [nv_v][vdl] > tableFrameHeight: " + tableFrameHeight.ToString() + " <======= ");

            (ViewModel as NewVisit_ViewModel).Error += OnError;

            (ViewModel as NewVisit_ViewModel).SendEmail += OnSendEmail;

            // Note: the "AsString" method returns a string representation of the UUID. The "ToString" method returns the description (selector).
            // ToDo:  replace with the advertisingIdentifier property of the ASIdentifierManager class.
            (ViewModel as NewVisit_ViewModel).UserID = UIDevice.CurrentDevice.IdentifierForVendor.AsString();

            // Review: double-check -- this should be setting the value/value for the **table** that holds the model
            SetTableFrameForOrientation(InterfaceOrientation); // use current orientation

            //(ViewModel as NewVisit_ViewModel).Height = FrameHeight();

            //(ViewModel as NewVisit_ViewModel).Width = TableFrameWidth();

            Common_iOS.DebugMessage("  [nv_v][vdl] > ...finished method.");
        }

        public override void ViewDidLayoutSubviews()
        {
			_method = "ViewDidLayoutSubviews";
			Common_iOS.DebugMessage(_namespace + _class, _method);
            View.BackgroundColor = Common_iOS.viewBackgroundColor;

            #region colorize subviews
            UIView[] subviews = new UIView[] { };
            int subviewsArrayLength = 0;
            UIView[] subISubviews = new UIView[] { };
            int subISubviewsArrayLength = 0;
            UIView[] subJSubviews = new UIView[] { };
            int subJSubviewsArrayLength = 0;
            UIView[] subKSubviews = new UIView[] { };
            int subKSubviewsArrayLength = 0;
            
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
            // No changes / variable assignment here -- this is diagnostic code!
            #region

            int colorIIndex = 0;
            int colorJIndex = 0;
            int colorKIndex = 0;

            // The _table is in the foreground -- this color is the (undefined) space **behind** the controls.
            // Drag the entire page up or down to see the BackgroundColor.
            //_table.BackgroundColor = UIColor.Orange;
            View.BackgroundColor = UIColor.Brown;

            // +++++
            subviews = View.Subviews;

            if (subviews == null)
            {
                Common_iOS.DebugMessage("  [nv_v][vdls] > View.Subviews[] is NULL.");
            }
            else 
            {
                subviewsArrayLength = subviews.Length;
                Common_iOS.DebugMessage("  [nv_v][vdls] > View.Subviews[] is NOT null. Length = " + subviewsArrayLength.ToString() + ".");

                for (int i = 0; i < subviewsArrayLength; i++)
                {
                    colorIIndex = i % grayArrayLength; // use modulus to keep colorIndex from going out of range
                    subviewType = subviews[i].GetType().ToString();

                    if (subviews[i].GetType() == typeof(UIView))
                    {
                        subviews[i].BackgroundColor = rgbGrays[colorIIndex]; // applying color should works here
                    }

                    Common_iOS.DebugMessage("  [nv_v][vdls] > View.Subviews[" + i.ToString() + "] is a " + subviewType + ": Height = " + subviews[i].Frame.Height.ToString() + ", Width = " + subviews[i].Frame.Width.ToString() + ", color set to " + colorIIndex + ".");

                    // +++++
                    subISubviews = subviews[i].Subviews;

                    if (subISubviews == null)
                    {
                        Common_iOS.DebugMessage("  [nv_v][vdls] > View.Subviews[" + i.ToString() + "].Subviews[] is NULL.");
                    }
                    else
                    {
                        subISubviewsArrayLength = subISubviews.Length;
                        Common_iOS.DebugMessage("  [nv_v][vdls] > View.Subviews[" + i.ToString() + "].Subviews[] is NOT null. Length = " + subISubviewsArrayLength.ToString() + ".");
                        
                        for (int j = 0; j < subISubviewsArrayLength; j++)
                        {
                            colorJIndex = j % colorArrayLength; // use modulus to keep colorIndex from going out of range
                            subviewType = subISubviews[j].GetType().ToString();

                            if (subISubviews[j].GetType() == typeof(UIView))
                            {
                                subISubviews[j].BackgroundColor = colors[colorJIndex];
                                subISubviews[j].BackgroundColor = UIColor.FromRGB(159, 0, 255);
                            }

                            Common_iOS.DebugMessage("  [nv_v][vdls] > View.Subviews[" + i.ToString() + "][" + j.ToString() + "] is a " + subviewType + ": Height = " + subISubviews[j].Frame.Height.ToString() + ", Width = " + subISubviews[j].Frame.Width.ToString() + ", color set to " + colorJIndex + ".");
                           
                            // +++++
                            subJSubviews = subISubviews[j].Subviews;

                            if (subJSubviews == null)
                            {
                                Common_iOS.DebugMessage("  [nv_v][vdls] > View.Subviews[" + i.ToString() + "][" + j.ToString() + "].Subviews[] is NULL.");
                            }
                            else
                            {
                                subJSubviewsArrayLength = subJSubviews.Length;
                                Common_iOS.DebugMessage("  [nv_v][vdls] > View.Subviews[" + i.ToString() + "][" + j.ToString() + "].Subviews[] is NOT null. Length = " + subJSubviewsArrayLength.ToString() + ".");

                                for (int k = 0; k < subJSubviewsArrayLength; k++)
                                {
                                    //colorKIndex = k % grayArrayLength; // use modulus to keep colorIndex from going out of range
                                    colorKIndex = k % colorArrayLength;
                                    subviewType = subJSubviews[k].GetType().ToString();

                                    if (subJSubviews[k].GetType() == typeof(UIView))
                                    {
                                        //subJSubviews[k].BackgroundColor = rgbGrays[colorKIndex];
                                        subJSubviews[k].BackgroundColor = colors[colorKIndex];
                                        //subJSubviews[k].BackgroundColor = UIColor.Red;
                                    }
                                        
                                    Common_iOS.DebugMessage("  [nv_v][vdls] > View.Subviews[" + i.ToString() + "][" + j.ToString() + "][" + k.ToString() + "] is a " + subviewType + ": Height = " + subJSubviews[k].Frame.Height.ToString() + ", Width = " + subJSubviews[k].Frame.Width.ToString() + ", color set to " + colorKIndex + ".");

                                    // +++++
                                    subKSubviews = subJSubviews[k].Subviews;

                                    if (subKSubviews == null)
                                    {
                                        Common_iOS.DebugMessage("  [nv_v][vdls] > View.Subviews[" + i.ToString() + "][" + j.ToString() + "][" + k.ToString() + "].Subviews[] is NULL.");
                                    }
                                    else
                                    {
                                        subKSubviewsArrayLength = subKSubviews.Length;
                                        Common_iOS.DebugMessage("  [nv_v][vdls] > View.Subviews[" + i.ToString() + "][" + j.ToString() + "][" + k.ToString() + "].Subviews[] is NOT null. Length = " + subKSubviewsArrayLength.ToString() + ".");

                                        // for loop
                                    }
                                }
                            }
                        }
                    }
                }
            }
        #endregion
#endif
            #endregion colorize subviews

    base.ViewDidLayoutSubviews();
            
            float screenHeight = UIScreen.MainScreen.Bounds.Height;
            Common_iOS.DebugMessage("  [nv_v][vdls] > screenHeight: " + screenHeight.ToString() + ", viewFrameHeight: " + View.Frame.Height.ToString() + ", LayoutHeight(): " + LayoutHeight().ToString());
            float screenWidth = UIScreen.MainScreen.Bounds.Width;
            Common_iOS.DebugMessage("  [nv_v][vdls] > screenWidth: " + screenWidth.ToString() + ", viewFrameWidth: " + View.Frame.Width.ToString());
            
            
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

            Common_iOS.DebugMessage("  [nv_v][vdls] > Using IOSVersionOK...");
            if (IsOS7OrLater)
            {
                displacement_y = this.TopLayoutGuide.Length;
                bottomGuide = this.BottomLayoutGuide.Length;
            }

            Common_iOS.DebugMessage("  [nv_v][vdls] > this.TopLayoutGuide.Length: " + displacement_y.ToString() + " this.BottomLayoutGuide.Length: " + bottomGuide.ToString() + " <======= ");
            Common_iOS.DebugMessage("  [nv_v][vdls] > ...finished");
        }

        public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
			_method = "WillAnimateRotation";
			Common_iOS.DebugMessage(_namespace + _class, _method);

            base.WillAnimateRotation(toInterfaceOrientation, duration);

            SetTableFrameForOrientation(toInterfaceOrientation); // use next orientation
        }

        public override void ViewWillAppear(bool animated)
        {
			_method = "ViewWillAppear";
			Common_iOS.DebugMessage(_namespace + _class, _method);

            base.ViewWillAppear(animated);

            SetTableFrameForOrientation(InterfaceOrientation); // use current orientation
        }

#pragma warning disable 612,618
        [Obsolete("Deprecated in iOS6. Replace it with both GetSupportedInterfaceOrientations and PreferredInterfaceOrientationForPresentation", false)]
        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)  // iOS4/iOS5 only
        {
			_method = "ShouldAutorotateToInterfaceOrientation";
			Common_iOS.DebugMessage(_namespace + _class, _method);

            bool rotate = false;

            if (toInterfaceOrientation == InterfaceOrientation)
            {
                rotate = true;
            }

			Common_iOS.DebugMessage(_namespace + _class, _method);
            Common_iOS.DebugMessage("  [nv_v][satio] > ShouldAutorotateToInterfaceOrientation = " + rotate.ToString() + " < [ersd_vc][satio]");

            return rotate;
        }
#pragma warning restore 612,618

        #pragma warning restore 1591
        #endregion overrides

        private void ReSendEmail(object sender, EventArgs eventArgs)
        {
			_method = "ReSendEmail";
			Common_iOS.DebugMessage(_namespace + _class, _method);

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
			_method = "ReSendFinished";
			Common_iOS.DebugMessage(_namespace + _class, _method);

            InvokeOnMainThread(() => { DismissViewController(true, null); } );
        }
        
        private void OnSendEmail(object sender, EventArgs eventArgs)
        {
			_method = "OnSendEmail";
			Common_iOS.DebugMessage(_namespace + _class, _method);

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
			_method = "MailViewOnFinished";
			Common_iOS.DebugMessage(_namespace + _class, _method);

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
			_method = "OnError";
			Common_iOS.DebugMessage(_namespace + _class, _method);

            InvokeOnMainThread(() => { new UIAlertView("Error", errorEventArgs.Message, null, "OK").Show(); } );
        }

        private void SetTableFrameForOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
			_method = "SetTableFrameForOrientation";
			Common_iOS.DebugMessage(_namespace + _class, _method);

            float usableHeight = 0f;
            float usableWidth = 0f;

            // ToDo: TopMargin() is called each time the orientation changes. This would be a good place to fix the saveButton origin.
            float topMarginHeight = TopMargin();

            switch (toInterfaceOrientation)
            {
                case UIInterfaceOrientation.Portrait:
                case UIInterfaceOrientation.PortraitUpsideDown:
                    // _reportTableView.Frame = UIScreen.MainScreen.Bounds;
                    usableHeight = UIScreen.MainScreen.Bounds.Height - topMarginHeight;
                    _table.Frame = new RectangleF(0, 0, UIScreen.MainScreen.Bounds.Width, usableHeight);
                    Portrait = true;
                    break;
                case UIInterfaceOrientation.LandscapeLeft:
                case UIInterfaceOrientation.LandscapeRight:
                    usableWidth = UIScreen.MainScreen.Bounds.Width - topMarginHeight;
                    _table.Frame = new RectangleF(0, 0, UIScreen.MainScreen.Bounds.Height, usableWidth);
                    Portrait = false;

                    break;
                default:
                    throw new ArgumentOutOfRangeException("toInterfaceOrientation");
            }
        }

        /// <summary>The value of the current <see cref="UIView.Frame"/>.
        /// </summary>
        /// <returns>The Frame value.</returns>
        public float FrameHeight()  // 1024 - (20 + 44) = 960
        {
			_method = "FrameHeight";
			Common_iOS.DebugMessage(_namespace + _class, _method);

            float value = _table.Frame.Height;
            string sourceType = _table.GetType().ToString();
            string mAbb = "[fh]"; // method name abbreviation

            if (value < 1)
            {
                Common_iOS.DebugMessage("  " + _cAbb + mAbb + " > " + sourceType + " not ready... < ?");

                value = View.Frame.Height;
                sourceType = View.ToString();

                if (value < 1)
                {
                    Common_iOS.DebugMessage("  " + _cAbb + mAbb + " > " + sourceType + " also not ready... < ?");

                    value = UIScreen.MainScreen.Bounds.Height;
                    sourceType = UIScreen.MainScreen.ToString();

                    if (value < 1)
                    {
                        Common_iOS.DebugMessage("  " + _cAbb + mAbb + " > " + sourceType + " also not ready... (how?) < !!!!!");
                        return 0f;
                    }
                }
            }
            
            float usableHeight = 0f;
            float topMarginHeight = TopMargin();

            usableHeight = value - topMarginHeight;
            Common_iOS.DebugMessage("  " + _cAbb + mAbb + " > used " + sourceType + " to find TableFrameWidth() = " + value.ToString() + " < ");

            return usableHeight;
        }

        /// <summary>The value of the current <see cref="UIView.Frame"/>.
        /// </summary>
        /// <returns>The width of the _table's frame.</returns>
        /// <remarks>If _table.Frame.Width is not ready, this method returns View.Frame.Width. 
        /// If View.Frame.Width is not ready, this method returns UIScreen.MainScreen.Bounds.Width.</remarks>
        public float TableFrameWidth()
        {
			_method = "TableFrameWidth";
			Common_iOS.DebugMessage(_namespace + _class, _method);

            string mAbb = "[fw]"; // method name abbreviation

            float value = _table.Frame.Width;
            string sourceType = _table.GetType().ToString();

            if (value < 1)
            {
                Common_iOS.DebugMessage("  " + _cAbb + mAbb + " > " + sourceType + " not ready... < ?");

                value = View.Frame.Width;
                sourceType = View.ToString();

                if (value < 1)
                {
                    Common_iOS.DebugMessage("  " + _cAbb + mAbb + " > " + sourceType + " also not ready... < ??");

                    value = UIScreen.MainScreen.Bounds.Width;
                    sourceType = UIScreen.MainScreen.ToString();

                    if (value < 1)
                    {
                        Common_iOS.DebugMessage("  " + _cAbb + mAbb + " > " + sourceType + " also not ready... (how?) < !!!!!");
                        return 0f;
                    }
                }
            }

            Common_iOS.DebugMessage("  " + _cAbb + mAbb + " > used " + sourceType + " to find " + mAbb + "() = " + value.ToString() + " < OK");

            return value;
        }

        /// <summary>Calculates a value representing a percent of the <see cref="UIView.Frame"/> value.
        /// </summary>
        /// <param name="percent">A percent value. Ex: 25.0</param>
        /// <returns>The product of (<see cref="TableFrameWidth"/> * <paramref name="percent"/>)</returns>
        internal float PercentOfTableFrameWidth(double percent)
        {
			_method = "PercentOfTableFrameWidth";
			Common_iOS.DebugMessage(_namespace + _class, _method);

            string mAbb = "[%fw]"; // method name abbreviation

            string sourceType = percent.GetType().ToString();

            if (percent < 1)
            {
                Common_iOS.DebugMessage("  " + _cAbb + mAbb + " > Warning: " + sourceType + " is < 1. < !!!!!");
            }

            percent = percent / 100;

            float value = (float)Math.Round((TableFrameWidth() * percent), 0);

            if (value < 1)
            {
                Common_iOS.DebugMessage("  " + _cAbb + mAbb + " > Warning: Result of calculation was < 1. < !!!!!");
            }

            return value;
        }

        /// <summary>Calculates a value representing a <paramref name="percent"/> of the <paramref name="rectangle"/> value.
        /// </summary>
        /// <param name="rectangle">The <see cref="RectangleF"/> object.</param>
        /// <param name="percent">A percent value. Ex: 25.0</param>
        /// <returns>The product of (<paramref name="rectangle">rectangle.Width</see> * <paramref name="percent"/>)</returns>
        internal float PercentOfRectangleWidth(RectangleF rectangle, double percent)
        {
			_method = "PercentOfRectangleWidth";
			Common_iOS.DebugMessage(_namespace + _class, _method);

            string mAbb = "[%rw]"; // method name abbreviation

            string sourceType = percent.GetType().ToString();

            if (percent < 1)
            {
                Common_iOS.DebugMessage("  " + _cAbb + mAbb + " > " + sourceType + " not ready... < !!!!!");
                return 0f;
            }

            float value = rectangle.Width;
            sourceType = rectangle.Width.GetType().ToString();

            if (value < 1)
            {
                Common_iOS.DebugMessage("  " + _cAbb + mAbb + " > " + sourceType + " not ready... < !!!!!");
                return 0f;
            }

            percent = percent / 100;
            value = (float)Math.Round((value * percent), 0);

            Common_iOS.DebugMessage("  " + _cAbb + mAbb + " > used " + sourceType + " to find " + mAbb + "() = " + value.ToString() + " < OK");

            return value;
        }

        /// <summary>The value of the device's screen.
        /// </summary>
        /// <returns>The screen value measured in points.</returns>
        internal float ViewFrameHeight()  // 960 
        {
			_method = "ViewFrameHeight";
			Common_iOS.DebugMessage(_namespace + _class, _method);

            float viewFrameHeight = 0;
            //viewFrameHeight = UIScreen.MainScreen.Bounds.Height;
            viewFrameHeight = View.Frame.Height;
            
            if (View.Frame.Height < 1)
            {
                Common_iOS.DebugMessage("  [nv_v][vfh] > View not ready... " );
                return 0f;
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

            Common_iOS.DebugMessage("  [nv_v][vdls] > Using IOSVersionOK...");
            Common_iOS.DebugMessage("  [nv_v][vfh] > iOS 7 = " + IsOS7OrLater.ToString() + " > ViewFrameHeight(): " + viewFrameHeight.ToString());

            return viewFrameHeight;
        }

        private float TopMargin()
        {
			_method = "TopMargin";
			Common_iOS.DebugMessage(_namespace + _class, _method);

            float topMargin = 0f;

            Common_iOS.DebugMessage("  [nv_v][vdls] > Using StatusBarHeight & NavBarHeight...");
            topMargin = StatusBarHeight + NavBarHeight;

            if (View.Frame.Height < 1)
            {
                Common_iOS.DebugMessage("  [nv_v][tm] > View not ready. Using hard-coded value. < #####");
                Common_iOS.DebugMessage("  [nv_v][vdls] > Using IOSVersionOK...");
                if (IsOS7OrLater)
                {
                    topMargin = 20f;
                }
                else
                {
                    topMargin = 64f;
                }
            }
            
            Common_iOS.DebugMessage("  [nv_v][tm] > topMargin: " + topMargin.ToString() + " < OK");

            return topMargin;
        }

        private float LayoutHeight()
        {
			_method = "LayoutHeight";
			Common_iOS.DebugMessage(_namespace + _class, _method);

            float layoutHeight = 0f;
            UIView[] subviews = View.Subviews;

            Common_iOS.DebugMessage("  [nv_v][lh] > Calculating layoutHeight....");
            if (subviews == null)
            {
                Common_iOS.DebugMessage("  [nv_v][lh] > View.Subviews[] is NULL.");
            }
            else
            {
                int subviewsArrayLength = subviews.Length;
                Common_iOS.DebugMessage("  [nv_v][lh] > View.Subviews[] is NOT null. subviewsArrayLength = " + subviewsArrayLength.ToString());
                for (int i = 0; i < subviewsArrayLength; i++)
                {
                    if (subviews[i].GetType() == typeof(UIView))
                    {
                        Common_iOS.DebugMessage("  [nv_v][lh] > View.Subviews[" + i.ToString() + "] == typeof(UIView), Height = " + subviews[i].Frame.Height.ToString());
                    }
                    else
                    {
                        Common_iOS.DebugMessage("  [nv_v][lh] > View.Subviews[" + i.ToString() + "] is wrapping something: Height = " + subviews[i].Frame.Height.ToString());
                        layoutHeight = subviews[i].Frame.Height;
                    }
                }
            }

            if (layoutHeight == 0)
            {
				Common_iOS.DebugMessage(_namespace + _class, _method);
                layoutHeight = View.Frame.Height;
            }

            Common_iOS.DebugMessage("  [nv_v][lh] > layoutHeight = " + layoutHeight.ToString() + " (finished).");

            return layoutHeight;
        }

        internal float FindNavBarHeight()  // 44
        {
			_method = "FindNavBarHeight";
			Common_iOS.DebugMessage(_namespace + _class, _method);
            float navbarHeight = 44f;

            try
            {
                if (IsOS7OrLater)
                {
                    navbarHeight = NavigationController.NavigationBar.Frame.Height; // the nearest ANCESTOR NavigationController
                }
                else
                {
                    Common_iOS.DebugMessage("  " + _cAbb + "[nbh] > View not ready. Using hard-coded value. < #####");
                }
            }
            catch
            {
                Common_iOS.DebugMessage("  " + _cAbb + "[nbh] > ERROR. < #####");
            }
            

            Common_iOS.DebugMessage("  " + _cAbb + "[nbh] > navbarHeight = " + navbarHeight.ToString() + " < OK");
            return navbarHeight;

            #region incomplete
            //float screenHeight = UIScreen.MainScreen.Bounds.Height;
            //float layoutHeight = 0f;
            //UIView[] subviews = View.Subviews;

            //// ToDo: this works in portrait, but after the first rotation it returns 300, 556, 
            // Common_iOS.DebugMessage("  [nv_v][nbh] > Calculating navbarHeight....");
            
            //// skip if we're not ready
            //if (View.Frame.Height < 1)
            //{
            //    Common_iOS.DebugMessage("  [nv_v][nbh] > View not ready... ");
            //    if (IOSVersionOK)
            //    {
            //        return 44f;
            //    }
            //    else
            //    {
            //        return 44f;
            //    }
            //}

            //layoutHeight = LayoutHeight();

            //// Note: sometimes the layoutHeight will default to View.Frame.Height, which could equal the screenHeight.
            //// navbarHeight could = 0
            //navbarHeight = screenHeight - layoutHeight;

            //// Note: comparisons of floating point values (double and float) are problematic because of the imprecision of floating point arithmetic on binary computers.
            //if ((int)navbarHeight == (int)screenHeight)
            //{
            //    Common_iOS.DebugMessage("  [nv_v][nbh] > (int)navbarHeight == (int)screenHeight) == " + navbarHeight.ToString() + " < = =" );

            //    navbarHeight = 0f;
            //    return navbarHeight;
            //}

            //if (IOSVersionOK)
            //{
            //    Common_iOS.DebugMessage("  [nv_v][nbh] > IsMinimumiOS6() = true, navbarHeight = " + navbarHeight.ToString() + " < = = = " );
            //    navbarHeight = NavigationController.NavigationBar.Frame.Height; // the nearest ANCESTOR NavigationController
            //}
            //else
            //{
            //    Common_iOS.DebugMessage("  [nv_v][nbh] > IsMinimumiOS6() = FALSE, navbarHeight = " + navbarHeight.ToString() + " - " + StatusBarHeight.ToString() + " = " + (navbarHeight - StatusBarHeight).ToString() + " < = = = =");
            //    navbarHeight = navbarHeight - StatusBarHeight;
            //}

            //return navbarHeight;
            #endregion
        }

        /// <summary>Determines the current height of the status bar.
        /// </summary>
        /// <returns>The status bar height in pixels.</returns>
        /// <remarks>For iPad (1st and 2nd generation) and iPad Mini; 20px. 
        /// For Retina iPad (iPad 3, 4, Air, Mini retina), iPone 4/4s, and iPhone 5 (iPhone 5, 5S, 5C); 40px. </remarks>
        internal float FindStatusBarHeight()
        {
            // Review: this always returns "20", never "40". May be a limitation of the simulator?
			_method = "FindStatusBarHeight";
			Common_iOS.DebugMessage(_namespace + _class, _method);
            float statusBarHeight = 0f;

            if (!UIApplication.SharedApplication.StatusBarHidden)
            {
                SizeF statusBarFrameSize = UIApplication.SharedApplication.StatusBarFrame.Size;
                statusBarHeight = Math.Min(statusBarFrameSize.Width, statusBarFrameSize.Height);
            }
            
            Common_iOS.DebugMessage("  [nv_v][sbh] > statusBarHeight: " + statusBarHeight.ToString() + " < OK");

            return statusBarHeight;
        }
    }
}