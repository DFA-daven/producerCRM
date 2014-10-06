namespace CallForm.iOS.Views
{
    using CallForm.Core.Models;
    using CallForm.Core.ViewModels;
    using CallForm.iOS.ViewElements;
    using Cirrious.MvvmCross.Binding.BindingContext;
    using Cirrious.MvvmCross.Touch.Views;
    using MonoTouch.Foundation;
    using MonoTouch.UIKit;
    using System;
    using System.Drawing;
    using System.Linq.Expressions;
    using System.Reflection;
    using XibFree;

    // notes: see _Touch UI.txt for design details.

    /// <summary>Creates a class representing the UI View for "ViewReports". 
    /// This is The project's initial view controller, and the primary page for the App.
    /// </summary>
    /// <remarks><para>This view inherits from <see cref="MvxViewController"/>. The main over-ridable methods 
    /// here handle the view's life-cycle.</para>
    /// <para>Design goal is to only deal with formatting and layout of data here -- no state information.</para></remarks>
    class ViewReports_View : MvxViewController
    {
        
        // class-level declarations

        string _nameSpace1 = "CallForm.iOS.";

        /// <summary>Class name abbreviation
        /// </summary>
        string _cAbb = "[vr_v]";

        #region Properties
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

        /// <summary>Store for the logo button property.</summary>
        private UIButton _logoButton;
        /// <summary>Store for the logo orientation property.</summary>
        private LinearLayout _logoLinearLayout;
        /// <summary>Store for the logo property.</summary>
        private UIView _logoView;

        /// <summary>Store for the footer button property.</summary>
        private UIButton _footerButton;
        /// <summary>Store for the footer orientation property.</summary>
        private LinearLayout _footerLinearLayout;
        /// <summary>Store for the footer property.</summary>
        private UIView _footerView;

        /// <summary>Store for the search filter property.</summary>
        private UITextField _filterField;
        /// <summary>Store for the button property.</summary>
        private UIButton _findButton, _newButton;
        /// <summary>Store for the logo orientation property.</summary>
        private LinearLayout _buttonLinearLayout;
        /// <summary>Store for the logo property.</summary>
        private UIView _buttonView;

        /// <summary>Store for the report table property.</summary>
        private UITableView _reportTableView;

        /// <summary>Store for the loading overlay property.</summary>
        LoadingOverlay _loadingOverlay;

        private bool ShowLogo = true;
        #endregion

        #region Hard-coded values 
        /// <summary>The space reserved for the status-bar in iOS 7 and later.
        /// </summary>
        //private static float topMarginPixels = 65;

        // FixMe: until we get a new banner, just hiding the old one
        private static decimal bannerHeightPercent = 10;
        //private static decimal bannerHeightPercent = 0.5;

        /// <summary>The value of controls as a percentage of screen value.
        /// </summary>
        private static decimal controlHeightPercent = 8;

        /// <summary>The value of controls as a percentage of screen value.
        /// </summary>
        private static decimal controlWidthPercent = 33.333M;

        /// <summary>The percentage of the horizontal value to indent this control's origin.
        /// </summary>
        private static decimal leftControlOriginPercent = 0;

        /// <summary>The percentage of the horizontal value to indent this control's origin.
        /// </summary>
        private static decimal middleControlOriginPercent = 33.333M;

        /// <summary>The percentage of the horizontal value to indent this control's origin.
        /// </summary>
        private static decimal rightControlOriginPercent = 66.666M;
        #endregion

        UIBarButtonItem preferencesBBI;
        UIBarButtonItem refreshBBI;
        UIBarButtonItem newBBI;
        public ViewReports_View()
        {
            IsOS7OrLater = Common_iOS.IsMinimumOS7;
            IsOS8OrLater = Common_iOS.IsMinimumiOS8();
            //NavBarHeight = FindNavBarHeight();
            //ViewFrameHeight = FindViewFrameHeight();

            // FixMe: hard-coded values -- calculate these from the screen dimensions?
            ButtonHeight = 44f;
            RowHeight = 44f;

            #region UIRefreshControl
            //// alternate older-style
            //if (Common_iOS.IsMinimumOS6)
            //{
            //    var RefreshControl = new UIRefreshControl();
            //    RefreshControl.ValueChanged += HandleValueChanged;

            //    InvokeOnMainThread(() =>
            //    {
            //        RefreshControl.EndRefreshing();
            //    });

            //}

            //NavigationItem.SetLeftBarButtonItem(new UIBarButtonItem(UIBarButtonSystemItem.Refresh), false);
            //NavigationItem.LeftBarButtonItem.Clicked += (sender, e) => { Refresh(); };
            #endregion

            #region UIBarButtonItem Preferences
            // this works b/c of the line: set.Bind(refreshBBI).To(vm => vm.GetReportsCommand);
            //refreshBBI = new UIBarButtonItem("[R]", UIBarButtonItemStyle.Plain, (sender, e) =>
            //refreshBBI = new UIBarButtonItem(UIImage.FromBundle("Images/slideout"), UIBarButtonItemStyle.Plain, (sender, e) =>

            // ToDo: switch this to a transparent PNG based on glyph 5060-8 from the Apple Symbols font (three stacked sheets).
            preferencesBBI = new UIBarButtonItem(UIBarButtonSystemItem.Bookmarks, (sender, e) =>
            {
                string message = "  [vr_v][vr_v] > Preferences BBI clicked.";
                Console.WriteLine(message);
#if (DEBUG)
                InvokeOnMainThread(() => { new UIAlertView("ViewReports_View", message, null, "OK").Show(); });
#endif
            });

#if (DEBUG)
            // Undone: missing Preferences view; missing correct icon
            NavigationItem.SetLeftBarButtonItem(preferencesBBI, false);
#endif 
            #endregion

            #region UIBarButtonItem Refresh
            // this works b/c of the line: set.Bind(refreshBBI).To(vm => vm.GetReportsCommand);
            refreshBBI = new UIBarButtonItem(UIBarButtonSystemItem.Refresh, (sender, e) =>
            {
                string message = "  [vr_v][vr_v] > Refresh data from server, (right clicked).";
                Console.WriteLine(message);
            });

            NavigationItem.SetRightBarButtonItem(refreshBBI, false);
            #endregion
        }

        #region refresh
        // UIRefreshControl iOS6
        void HandleValueChanged(object sender, EventArgs e)
        {
            Refresh();
        }

        void Refresh()
        {
            string message = "  [vr_v][refresh] > Refresh data from server, (left clicked).";
            Console.WriteLine(message);
#if (DEBUG)
            InvokeOnMainThread(() => { new UIAlertView("ViewReports_View", message, null, "OK").Show(); });
#endif
            // ToDo: if using this approach (HandleValueChanged/Refresh) need to call the GetReportsCommand().

            //AppDelegate.Conference.DownloadFromServer();
        }
        #endregion refresh

        #region overrides
        #pragma warning disable 1591
        public override void ViewDidLoad()
        {
            Common_iOS.DebugMessage(_nameSpace1 + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            Common_iOS.DebugMessage("  [vr_v][vdl] > starting method...");

            if (!IsOS8OrLater)
            {
                string message = "  [vr_v][vr_v] > This App requires iOS 8 or higher.";
                Console.WriteLine(message);
                Common_iOS.DebugMessage("##################### Halt app. ####################################################");

                //InvokeOnMainThread(() => { new UIAlertView("Wrong iOS version", message, null, "OK").Show(); });

                // Review: pick one: 1. P/Invoke exit(); 2. NSThread.Exit(); 3. throwing an exception; 4. terminateWithSuccess
                // NSThread.Exit();
            }

            #region pageLayout
            float topMargin = 0;

            // Note: The Navigation Controller is a UI-less View Controller responsible for
            // managing a stack of View Controllers and provides tools for navigation, such 
            // as a navigation bar with a back button.
            topMargin = StatusBarHeight() + NavBarHeight;
            Common_iOS.DebugMessage("  [vr_v][vdl] > topMargin = " + topMargin.ToString() + " < <======= ");

            #region logo
            #region logoButton
            var logoButton = _logoButton = new UIButton(UIButtonType.Custom);
            logoButton.Frame = new RectangleF(0, 0, MaxBannerWidth(), MaxBannerHeight());
            logoButton.SetTitle("DFA & DMS", UIControlState.Normal);
            logoButton.SetImage(UIImage.FromBundle("DFA-DMS-Banner.png"), UIControlState.Normal);
            logoButton.BackgroundColor = UIColor.Yellow;
            #endregion logoButton

            #region logoLayout
            var logoLayout = _logoLinearLayout = new LinearLayout(Orientation.Vertical)
            {
                Gravity = Gravity.TopCenter,
                SubViews = new View[]
                {
                    new NativeView
                    {
                        View = logoButton,
                        LayoutParameters = new LayoutParameters()
                        {
                            Gravity = Gravity.Top,
                        }
                    }
                }
            };
            #endregion logoLayout

            #region logoView
            var logoView = _logoView = new UIView();
            logoView = new UILayoutHost(logoLayout)
            {
                BackgroundColor = UIColor.White,
            };

            logoView.SizeToFit();
            #endregion logoView
            #endregion logo

            #region buttons
            // ToDo: place the 3 controls in a horizontal view with something like
            // var layout = new LinearLayout(Orientation.Horizontal)
            // SubViews = new View[]

            #region filterField
            var filterField = _filterField = new UITextField
            {
                TextAlignment = UITextAlignment.Center,
                KeyboardType = UIKeyboardType.NumberPad,
                Placeholder = "Member #",
                ShouldChangeCharacters = (field, range, replacementString) =>
                {
                    int i;
                    return replacementString.Length <= 0 || int.TryParse(replacementString, out i);
                },
                //Font = UIFont.SystemFontOfSize(20),
                Frame = new RectangleF(PercentWidth(leftControlOriginPercent), BannerBottom(), ControlWidth(), ControlHeight()),
                BackgroundColor = Common_iOS.controlBackgroundColor,
                //BackgroundColor = UIColor.Blue,
            };
            filterField.VerticalAlignment = UIControlContentVerticalAlignment.Center;       // text should appear vertically centered
            filterField.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;     // cursor should appear to right of placeholder; 
            filterField.HorizontalAlignment = UIControlContentHorizontalAlignment.Fill;     // cursor should appear to right of placeholder; 
            #endregion filterField

            #region findButton
            var findButton = _findButton = new UIButton(UIButtonType.Custom);
            findButton.Frame = new RectangleF(PercentWidth(middleControlOriginPercent), BannerBottom(), ControlWidth(), ControlHeight());
            findButton.SetTitle("Search", UIControlState.Normal);
            findButton.BackgroundColor = Common_iOS.viewBackgroundColor;
            //findButton.BackgroundColor = UIColor.Green;
            #endregion findbutton

            #region newButton
            var newButton = _newButton = new UIButton(UIButtonType.Custom);
            newButton.Frame = new RectangleF(PercentWidth(rightControlOriginPercent), BannerBottom(), ControlWidth(), ControlHeight());
            newButton.SetTitle("New", UIControlState.Normal);
            // ToDo: scale the image so it fits in the control
            var plusSign = UIImage.FromBundle("Add.png");
            //plusSign.Scale();
            newButton.SetImage(UIImage.FromBundle("Add.png"), UIControlState.Normal);
            newButton.BackgroundColor = Common_iOS.viewBackgroundColor;
            //newButton.BackgroundColor = UIColor.Red;
            #endregion newButton

            #endregion buttons

            #region table
            //var tableView = _reportTableView = new UITableView(new RectangleF(PercentWidth(leftControlOriginPercent), TableTop(), PercentWidth(98), ScreenHeight() - TableTop()));
            //var tableView = _reportTableView = new UITableView(new RectangleF(0, TableTop(), ScreenWidth(), ScreenHeight() - TableTop()));
            var tableView = _reportTableView = new UITableView(new RectangleF(0, TableTop(), View.Frame.Width, View.Frame.Height - TableTop()));
            tableView.BackgroundView = null;
            tableView.BackgroundColor = UIColor.White; // this makes the rows of the table white
            //tableView.BackgroundColor = UIColor.Clear; // this allows the View.BackgroundColor to be visible
            #endregion table

            #region footer
            #region footerButton
            var footerButton = _footerButton = new UIButton(UIButtonType.Custom);
            footerButton.Frame = new RectangleF(0, 0, MaxBannerWidth(), MaxBannerHeight());
            footerButton.SetTitle("DFA & DMS", UIControlState.Normal);
            footerButton.SetImage(UIImage.FromBundle("DFA-DMS-Banner.png"), UIControlState.Normal);
            footerButton.BackgroundColor = UIColor.Yellow;
            #endregion footerButton

            #region footerLayout
            var footerLayout = _footerLinearLayout = new LinearLayout(Orientation.Vertical)
            {
                Gravity = Gravity.BottomCenter,
                SubViews = new View[]
                {
                    new NativeView
                    {
                        View = footerButton,
                        LayoutParameters = new LayoutParameters()
                        {
                            Gravity = Gravity.Top,
                        }
                    }
                }
            };
            #endregion footerLayout

            #region footerView
            var footerView = _footerView = new UIView();
            footerView = new UILayoutHost(footerLayout)
            {
                BackgroundColor = UIColor.White,
            };

            footerView.SizeToFit();
            #endregion footerView
            #endregion footer

            #region loading
            var loading = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
            loading.Center = View.Center;
            loading.StartAnimating();

            var loadingOverlay = _loadingOverlay = new LoadingOverlay(UIScreen.MainScreen.Bounds);
            #endregion loading

            #endregion layout

            // Note: most of these view elements don't have a layout/gravity, 
            // so the order that Views are added determines their position in front of (or behind) other Views.
            // the buttons must have some off-set that is causing a gap.

            View.BackgroundColor = Common_iOS.viewBackgroundColor;
#if (DEBUG || BETA)
    View.BackgroundColor = UIColor.Orange;
#endif
            View.Add(logoButton);
            //View.Add(logoLayout);
            //View.Add(logoView);

            View.Add(findButton);
            View.Add(filterField);
            View.Add(newButton);

            View.Add(tableView);

            //View.Add(footerButton);
            //View.Add(footerLayout);
            //View.Add(footerView);

            View.Add(loading);
            View.Add(loadingOverlay);

            base.ViewDidLoad();
            #region experimental FlyoutNavigationController
            /*
            // base.ViewDidLoad();
            var navigation = new FlyoutNavigationController
            {
                // Create the navigation menu
                NavigationRoot = new RootElement("Navigation") 
                {
                    new Section ("Pages") 
                    {
                        new StringElement ("Animals"),
                        new StringElement ("Vegetables"),
                        new StringElement ("Minerals"),
                    }
                },

                // Supply view controllers corresponding to menu items:
                ViewControllers = new[] 
                {
                    new UIViewController { View = new UILabel { Text = "Animals (drag right)" } },
                    new UIViewController { View = new UILabel { Text = "Vegetables (drag right)" } },
                    new UIViewController { View = new UILabel { Text = "Minerals (drag right)" } },
                },
            };

            // Show the navigation view
            navigation.ToggleMenu();
            View.AddSubview(navigation.View);
             * */
            #endregion

            #region bind content to view
            // Note: this is where the ViewReports_View view controller is created.
            // It's similar to having
            //   controller = new ViewReports_View();
            //   window.RootViewController = controller;
            // in AppDelegate.cs.

            // Note: this BindingDescriptionSet represents the link between the UserIdentity_View and the UserIdentity_ViewModel.
            var set = this.CreateBindingSet<ViewReports_View, ViewReports_ViewModel>();

            set.Bind(logoButton).To(vm => vm.GetReportsCommand);

            set.Bind(refreshBBI).To(vm => vm.GetReportsCommand);

            set.Bind(filterField).To(vm => vm.Filter);
            set.Bind(findButton).To(vm => vm.GetReportsCommand);

            set.Bind(newButton).To(vm => vm.NewVisitCommand);

            set.Bind(loading).For("Visibility").To(vm => vm.Loading).WithConversion("Visibility");
            set.Bind(loadingOverlay).For("Visibility").To(vm => vm.Loading).WithConversion("Visibility");
            
            set.Bind(tableView).For("Visibility").To(vm => vm.Loading).WithConversion("InvertedVisibility");
            set.Apply();

            #region UI action
            // Note: resigning the first responder automatically dismisses the keyboard (if displayed)
            findButton.TouchUpInside += (sender, args) => { filterField.ResignFirstResponder(); };
            logoButton.TouchUpInside += (sender, args) => { filterField.ResignFirstResponder(); };
            refreshBBI.Clicked += (sender, args) => { filterField.ResignFirstResponder(); };

            filterField.ShouldReturn = delegate
            {
                filterField.ResignFirstResponder();
                return true;
            };
            
            // keyboard should disappear if user taps outside of a text-box
            //var goAway = new UITapGestureRecognizer(() => View.EndEditing(true));
            //View.AddGestureRecognizer(goAway);
            #endregion UI action

            (ViewModel as ViewReports_ViewModel).Error += OnError;

            var source = new ViewReports_TableViewSource(ViewModel as ViewReports_ViewModel, tableView);

            tableView.Source = source;

            // FixMe: find way to get the app's title
            // get current values from the assembly
            var assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
            string appName = assemblyName.Name;                     // the name of this project -- CallForm.iOS
            appName = "ProducerCRM";
            string appVersion = assemblyName.Version.ToString();    // the version number
            ////var up = System.Reflection.Ass

            // FixMe: this only catches if the debugger is attached - so 'alpha' and 'beta' are never true.
            // need something like if this.config != release then appName = appName + this.config
#if (ALPHA)
    // No changes / variable assignment here -- this is preprocessor code!
    appName += " (ALPHA)";
#elif (BETA)
    appName += " (BETA)";
#elif (RELEASE)
	appName = appName;
#elif (DEBUG)
            appName += " (DEBUG)";

#endif

            // FixMe: this only catches if the debugger is attached - so 'alpha' and 'beta' are never true.
            Title = appName + " " + appVersion;
            //Title = appName + " (VFRVBETA); " + appVersion;
            //this.AddChildViewController
            //this.availableViewHeight
            #endregion

            Common_iOS.DebugMessage(_nameSpace1 + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            Common_iOS.DebugMessage("  [vr_v][vdl] > ...finished method.");
        }

        public override void ViewDidLayoutSubviews()
        {
            Common_iOS.DebugMessage(_nameSpace1 + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            Common_iOS.DebugMessage("  [vr_v][vdls] > System version: " + UIDevice.CurrentDevice.SystemVersion);

            base.ViewDidLayoutSubviews();

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
            if (IsOS7OrLater)
            {
                //displacement_y = this.TopLayoutGuide.Length;
            }

            Common_iOS.DebugMessage(_nameSpace1 + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            Common_iOS.DebugMessage("  [vr_v][vdls] > ...finished");
        }

        public override void MotionEnded(UIEventSubtype motion, UIEvent evt)
        {
            if (motion == UIEventSubtype.MotionShake)
            {
                // ToDo: use this to refresh the view reports table
            }

            base.MotionEnded(motion, evt);
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

        public override void ViewWillAppear(bool animated)
        {
            Common_iOS.DebugMessage(_nameSpace1 + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);

            base.ViewWillAppear(animated);
            SetFramesForOrientation(InterfaceOrientation);
        }

        public override void ViewDidAppear(bool animated)
        {
            Common_iOS.DebugMessage(_nameSpace1 + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);

            // Note: each time ViewReports is displayed/appears UploadReports() is triggered.
            base.ViewDidAppear(animated);
            (ViewModel as ViewReports_ViewModel).UploadReports();
            (ViewModel as ViewReports_ViewModel).Loading = false;
            (ViewModel as ViewReports_ViewModel).IOSVersionOK = false;
        }

        public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            Common_iOS.DebugMessage(_nameSpace1 + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);

            base.WillAnimateRotation(toInterfaceOrientation, duration);

            SetFramesForOrientation(toInterfaceOrientation);
        }

        #pragma warning restore 1591
        #endregion overrides

        private static float _viewFrameHeight = 0f;
        public static float ViewFrameHeight
        {
            get { return _viewFrameHeight; }
            set { _viewFrameHeight = value; }
        }
        /// <summary>The value of the device's screen.
        /// </summary>
        /// <returns>The screen value measured in points.</returns>
        internal float FindViewFrameHeight()
        {
            float viewFrameHeight = 0;
            //viewFrameHeight = UIScreen.MainScreen.Bounds.Height;
            viewFrameHeight = View.Frame.Height;

            switch (InterfaceOrientation)
            {
                case UIInterfaceOrientation.Portrait:
                case UIInterfaceOrientation.PortraitUpsideDown:
                    viewFrameHeight = View.Frame.Height;
                    break;

                case UIInterfaceOrientation.LandscapeLeft:
                case UIInterfaceOrientation.LandscapeRight:
                    viewFrameHeight = View.Frame.Width;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("ViewFrameHeight");
            }

            return viewFrameHeight;
        }

        /// <summary>The value of the device's screen.
        /// </summary>
        /// <returns>The screen value measured in points.</returns>
        internal float ViewFrameWidth()
        {
            float viewFrameWidth = 0;
            //viewFrameWidth = UIScreen.MainScreen.Bounds.Width;
            viewFrameWidth = View.Frame.Width;

            switch (InterfaceOrientation)
            {
                case UIInterfaceOrientation.Portrait:
                case UIInterfaceOrientation.PortraitUpsideDown:
                    viewFrameWidth = View.Frame.Width;
                    break;

                case UIInterfaceOrientation.LandscapeLeft:
                case UIInterfaceOrientation.LandscapeRight:
                    viewFrameWidth = View.Frame.Height;
                    break;

                default:
                    throw new ArgumentOutOfRangeException("ViewFrameWidth");
            }

            return viewFrameWidth;
        }

        ///// <summary>The value of the device's screen.
        ///// </summary>
        ///// <returns>The screen value measured in points.</returns>
        //internal float ScreenHeight()
        //{
        //    float screenHeight = 0;
        //    screenHeight = UIScreen.MainScreen.Bounds.Height;
        //    return screenHeight;
        //}

        ///// <summary>The value of the device's screen.
        ///// </summary>
        ///// <returns>The screen value measured in points.</returns>
        //internal float ScreenWidth()
        //{
        //    float screenWidth = 0;
        //    screenWidth = UIScreen.MainScreen.Bounds.Width;
        //    return screenWidth;
        //}

        /// <summary>The value of the portion of the screen available for Views.
        /// </summary>
        /// <returns>The available value measured in points.</returns>
        /// <remarks>For pre-iOS 7 devices, this value will be the same as <see cref="ViewFrameHeight"/>. For
        /// iOS 7 and later, the <see cref="topMarginPixels"/> are reserved.</remarks>
        internal float AvailableHeight()
        {
            float availableViewHeight = 0;
            //availableViewHeight = ViewFrameHeight() - NavigationController.NavigationBar.Frame.Height;
            //availableViewHeight = ViewFrameHeight();
            availableViewHeight = View.Frame.Height;

            //if (isOS7())
            //{
            //    availableViewHeight -= topMarginPixels;
            //}

            return availableViewHeight;
        }

        private float DesiredBannerHeight()
        {
            float maxAllowedBannerHeight = 0;
            // ToDo: fix to handle changes in orientation
            maxAllowedBannerHeight = CalculatePercent(View.Frame.Height, bannerHeightPercent);

            maxAllowedBannerHeight = (float)Math.Round(maxAllowedBannerHeight, 0);

            return maxAllowedBannerHeight;
        }

        private float BannerHeightRatio()
        {
            float actualBannerHeight = UIImage.FromBundle("DFA-DMS-Banner.png").Size.Height;
            float maxBannerHeight = DesiredBannerHeight();

            float heightRatio = maxBannerHeight / actualBannerHeight;

            return heightRatio;
        }

        private float MaxBannerHeight()
        {
            float actualBannerHeight = UIImage.FromBundle("DFA-DMS-Banner.png").Size.Height;
            float maxBannerHeight = DesiredBannerHeight();

            float desiredBannerHeight = actualBannerHeight * BannerHeightRatio();

            if (BannerRatioLimit())
            {
                desiredBannerHeight = maxBannerHeight;
            }

            desiredBannerHeight = (float)Math.Round(desiredBannerHeight, 0);

            return desiredBannerHeight;
        }

        private bool BannerRatioLimit()
        {
            float actualBannerWidth = UIImage.FromBundle("DFA-DMS-Banner.png").Size.Width;
            //float maxAllowedBannerWidth = ViewFrameWidth();
            float maxAllowedBannerWidth = View.Frame.Width;

            float desiredBannerWidth = actualBannerWidth * BannerHeightRatio();

            return (desiredBannerWidth >= maxAllowedBannerWidth);
        }

        private float MaxBannerWidth()
        {
            float actualBannerWidth = UIImage.FromBundle("DFA-DMS-Banner.png").Size.Width;
            float maxAllowedBannerWidth = View.Frame.Width;

            float desiredBannerWidth = actualBannerWidth * BannerHeightRatio();

            if (BannerRatioLimit()) 
            {
                desiredBannerWidth = maxAllowedBannerWidth;
            }

            desiredBannerWidth = (float)Math.Round(desiredBannerWidth, 0);
            return desiredBannerWidth;
        }

        //private float bannerHorizontalOrigin()
        //{
        //    float bannerHorizontalOrigin = 0;

        //    if (MaxBannerWidth() < ViewFrameWidth())
        //    {
        //        bannerHorizontalOrigin = (float)Math.Round((ViewFrameWidth() - MaxBannerWidth()) / 2, 0);
        //    }

        //    return bannerHorizontalOrigin;
        //}

        private float BannerBottom()
        {
            float bannerBottom = 0;
            //bannerBottom = MaxBannerHeight() + NavigationController.NavigationBar.Frame.Height;
            bannerBottom = _logoButton.Frame.Height;

            if (!ShowLogo)
            {
                bannerBottom = 0;
            }

            //if (isOS7())
            //{
            //    BannerBottom += topMarginPixels;
            //}
            
            return bannerBottom;
        }

        /// <summary>Calculates the pixel-value of controls based on the current screen value.
        /// </summary>
        /// <returns>The pixel-value of controls.</returns>
        internal float ControlHeight()
        {
            //float ControlHeight = CalculatePercent(availableViewHeight(), controlHeightPercent);
            float controlHeight = UIFont.ButtonFontSize * 3f;
            controlHeight = (float)Math.Round(controlHeight, 0);

            return controlHeight;
        }

        /// <summary>Calculates the pixel-value of controls based on the current screen value.
        /// </summary>
        /// <returns>The pixel-value of controls.</returns>
        internal float ControlWidth()
        {
            float controlWidth = PercentWidth(controlWidthPercent);
            controlWidth = (float)Math.Round(controlWidth, 0);
            return controlWidth;
        }

        /// <summary>The top offset for the table.
        /// </summary>
        /// <returns></returns>
        internal float TableTop()
        {
            float tableTop = BannerBottom() + ControlHeight();
            return tableTop;
        }

        /// <summary>Calculates the product of the current screen value and a percent.
        /// </summary>
        /// <param name="percent">A percent in the range 0 - 100.</param>
        /// <returns>A value representing a percent of the current screen value.</returns>
        private float PercentHeight(decimal percent)
        {
            float height = CalculatePercent(UIScreen.MainScreen.Bounds.Height, percent);
            height = (float)Math.Round(height, 0);

            return height;
        }

        /// <summary>Calculates the product of the current screen value and a percent.
        /// </summary>
        /// <param name="percent">A percent in the range 0 - 100.</param>
        /// <returns>A value representing a percent of the current screen value.</returns>
        private float PercentWidth(decimal percent)
        {
            float width = CalculatePercent(UIScreen.MainScreen.Bounds.Width, percent);
            width = (float)Math.Round(width, 0);
            return width;
        }

        /// <summary>Calculates the product of a dimension and a percent.
        /// </summary>
        /// <param name="dimension">A dimension, such as screen value or value.</param>
        /// <param name="percent">A percent in the range 0 - 100.</param>
        /// <returns>The product of a given dimension and percent.</returns>
        private float CalculatePercent(float dimension, decimal percent)
        {
            percent = percent / 100;
            decimal value = (decimal)dimension * percent;
            // value = Math.Abs(Math.Round(value));
            return (float)value;
        }

        /// <summary>Displays the error issued by the <c>ViewModel</c> .
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="errorEventArgs"></param>
        private void OnError(object sender, ErrorEventArgs errorEventArgs)
        {
            InvokeOnMainThread(() => { new UIAlertView("Error", errorEventArgs.Message, null, "OK").Show(); } );
        }

        private void SetFramesForOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            Common_iOS.DebugMessage(_nameSpace1 + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            Common_iOS.DebugMessage("? [vr_v][sffo] > SetFramesForOrientation: make a note of when this runs");

            float difference = 0;
            float offset = 0;

            difference = Math.Abs(View.Frame.Width - View.Frame.Height);

            // Hack: this should be handled automatically with .gravity.
            switch (toInterfaceOrientation)
            {
                case UIInterfaceOrientation.Portrait:
                case UIInterfaceOrientation.PortraitUpsideDown:
                    Portrait = true;

                    break;

                case UIInterfaceOrientation.LandscapeLeft:
                case UIInterfaceOrientation.LandscapeRight:
                    offset = (difference / 2);
                    offset = (float)Math.Round(offset, 0);
                    Portrait = false;

                    break;

                default:
                    throw new ArgumentOutOfRangeException("toInterfaceOrientation");
            }

            SetFrameX(_logoView, PercentWidth(leftControlOriginPercent) + offset);
            SetFrameX(_logoButton, PercentWidth(leftControlOriginPercent) + offset);
            SetFrameX(_filterField, PercentWidth(leftControlOriginPercent) + offset);
            SetFrameX(_findButton, PercentWidth(middleControlOriginPercent) + offset);
            SetFrameX(_newButton, PercentWidth(rightControlOriginPercent) + offset);

            _reportTableView.Frame = new RectangleF(0, TableTop(), View.Frame.Width, View.Frame.Height - TableTop());

        }

        /// <summary>Updates the horizontal origin of a view.
        /// </summary>
        /// <param name="view">The <see cref="UIView"/> to be updated.</param>
        /// <param name="x">The new "X" coordinate for the View.</param>
        internal void SetFrameX(UIView view, float x)
        {
            // Hack: this should be handled automatically with .gravity.
            var frame = view.Frame;
            frame.X = x;
            view.Frame = frame;
        }

        private static float _navBarHeight = 0f;
        public static float NavBarHeight
        {
            get { return _navBarHeight; }
            set { _navBarHeight = value; }
        }
        private float FindNavBarHeight()
        {
            float screenHeight = UIScreen.MainScreen.Bounds.Height;
            float layoutHeight = 0f;
            float navbarHeight = 0f;

            layoutHeight = ViewFrameHeight;
            navbarHeight = screenHeight - layoutHeight;

            Common_iOS.DebugMessage(_nameSpace1 + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            Common_iOS.DebugMessage("  [common_iOS][fnbh] > screenHeight: " + screenHeight.ToString() + ", layoutHeight = " + layoutHeight.ToString() + ", calc navbar value: " + navbarHeight.ToString() + " <=======");

            if (Common_iOS.IsMinimumOS7)
            {
                navbarHeight = NavigationController.NavigationBar.Frame.Height; // the nearest ANCESTOR NavigationController
                layoutHeight = this.BottomLayoutGuide.Length - this.TopLayoutGuide.Length;
                Common_iOS.DebugMessage("  [common_iOS][fnbh] > iOS 7 topMarginHeight: " + navbarHeight.ToString() + ", iOS7 layoutHeight = " + layoutHeight.ToString() + " <======= ");
            }

            return navbarHeight;
        }

        private float StatusBarHeight()
        {
            SizeF statusBarFrameSize = UIApplication.SharedApplication.StatusBarFrame.Size;
            return Math.Min(statusBarFrameSize.Width, statusBarFrameSize.Height);
        }
    }

    /// <summary>Abstract class (replaces UITableViewDelegate and UITableViewDataSource).
    /// </summary>
    /// <remarks>GetCell() and other methods use NSIndexPath.</remarks>
    public class ViewReports_TableViewSource : UITableViewSource
    {
        #region declarations
        // class-level declarations
        string _nameSpace = "CallForm.iOS.";

        /// <summary>Class name abbreviation
        /// </summary>
        string _cAbb = "[vr_v]";

        #region Properties
        private readonly ViewReports_ViewModel _viewModel;
        private readonly UITableView _tableView;
        private const string CellIdentifier = "tableViewCell";
        private const string FooterIdentifier = "tableViewFooter";

        private bool _isOS7OrLater;
        public bool IsOS7OrLater
        {
            get { return _isOS7OrLater; }
            set { _isOS7OrLater = value; }
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
        #endregion
        #endregion

        UIButton newReportButtonTableView;
        public ViewReports_TableViewSource(ViewReports_ViewModel viewModel, UITableView tableView)
        {
            _viewModel = viewModel;
            _tableView = tableView;

            // FixMe: hard-coded values -- calculate these from the screen dimensions?
            ButtonHeight = (float)Math.Round((UIFont.SystemFontSize * 3f), 0);
            RowHeight = 50f;

            try
            {
                _viewModel.PropertyChanged += (sender, args) =>
                    {
                        if (args.PropertyName == "Reports")
                        {
                            tableView.ReloadData();
                        }
                    };
            }
            
            finally
            {
                
            }
        }

        #region Overrides
        #pragma warning disable 1591
        public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            // FixMe: remove hard-coded values (or add XML entry)
            return RowHeight;
        }

        /// <summary>The number of rows (cells) in this section of <see cref="ViewReports_TableViewSource"/>.
        /// </summary>
        /// <param name="tableView">The <see cref="UITableView"/>/control that contains the section.</param>
        /// <param name="section">The index number of the section that contains the rows (cells).</param>
        /// <remarks><paramref name="section"/> is included as part of the override -- it is not used in this method.</remarks>
        /// <returns>A row count.</returns>
        public override int RowsInSection(UITableView tableview, int section)
        {
            return _viewModel.Reports == null ? 0 : _viewModel.Reports.Count;
        }

        /// <summary>Handles the selected row (cell) in <see cref="ViewReports_TableViewSource"/>.
        /// </summary>
        /// <param name="tableView">The <see cref="UITableView"/>/control that contains the selected row.</param>
        /// <param name="indexPath">The <see cref="NSIndexPath"/> of the selected row in the control.</param>
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            _viewModel.SelectedReport = _viewModel.Reports[indexPath.Row];
            _viewModel.ViewReportCommand.Execute(_viewModel);
            tableView.DeselectRow(indexPath, true);
            tableView.ReloadData();
        }

        public override UIView GetViewForFooter(UITableView tableView, int section)
        {
            // FixMe: until the TableView is loaded there is no content, so this footer appears near the top of the page.
            // Solution: move this back into the main View, and set the gravity so that it acts like a footer.
            newReportButtonTableView = new UIButton(UIButtonType.Custom);
            //newReportButtonTableView.Frame = new RectangleF(0, 0, ControlWidth(), ControlHeight());
            newReportButtonTableView.SetTitle("New Report (tableView footer)", UIControlState.Normal);
            newReportButtonTableView.BackgroundColor = Common_iOS.controlBackgroundColor;

            return newReportButtonTableView;
        }

        public override float GetHeightForFooter(UITableView tableView, int section)
        {
            float heightToReport = RowHeight;

            Common_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            Common_iOS.DebugMessage("  [vr_tvs][ghff] > Footer Height = " + RowHeight.ToString() + " < = = = = = =");

            return heightToReport;
        }

        /// <summary>Gets a cell based on the selected <see cref="NSIndexPath">Row</see>.
        /// </summary>
        /// <param name="tableView">The <see cref="UITableView">(view) table</see> that contains the cell.</param>
        /// <param name="indexPath">The <see cref="NSIndexPath"/> to the selected row (cell).</param>
        /// <returns>The requested <see cref="TableViewCell" /> from the <see cref="ViewReports_TableViewSource"/>.</returns>
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            TableViewCell cell = tableView.DequeueReusableCell(CellIdentifier) as TableViewCell ?? new TableViewCell();

            Common_iOS.DebugMessage("  [vr_tvs][gc] > indexPath.Row = " + indexPath.Row.ToString() + ". < ++++++++ ++++++++");
            ReportListItem reportListItem = _viewModel.Reports[indexPath.Row];

            /*
             * ToDo: indent the cells (rows)
             * cell.SeparatorInset = new UIEdgeInsets(0, 50, 0, 0);
             */
            cell.Date.Text = reportListItem.VisitDate.ToShortDateString();
            cell.MemberNumber.Text = reportListItem.MemberNumber;
            cell.Source.Text = reportListItem.UserEmail;
            cell.Reasons.Text = reportListItem.PrimaryReasonCode.Name;

            // this keeps the color behind the text the same as the rest of the table
            cell.Date.BackgroundColor = cell.MemberNumber.BackgroundColor = tableView.BackgroundColor;
            cell.Source.BackgroundColor = cell.Reasons.BackgroundColor = tableView.BackgroundColor;

            cell.Host.SetNeedsLayout();

            return cell;
        }
        #pragma warning restore 1591
        #endregion

        // <summary>Get the name of a static or instance property from a property access lambda.
        // </summary>
        // <typeparam name="T">Type of the property.</typeparam>
        // <param name="propertyLambda">lambda expression of the form: '() => Class.Property' or '() => object.Property'.</param>
        // <returns>The name of the property.</returns>
        public string GetPropertyName<T>(Expression<Func<T>> propertyLambda)
        {
            var me = propertyLambda.Body as MemberExpression;

            if (me == null)
            {
                throw new ArgumentException("You must pass a lambda of the form: '() => Class.Property' or '() => object.Property'");
            }

            return me.Member.Name;
        }
    }

    /// <summary>These are the rows in the View Reports view. 
    /// </summary>
    public class TableViewCell : UITableViewCell
    {
        public UILabel Date, MemberNumber, Source, Reasons;
        public UILayoutHost Host;

        // FixMe: plenty of hard-coded values in this one
        /// <summary>The class constructor.
        /// </summary>
        public TableViewCell() : base(UITableViewCellStyle.Default, "tableViewCell")
        {
            // this layout is composed of "columns"
            var layout = new LinearLayout(Orientation.Horizontal)
            {
                Padding = new UIEdgeInsets(4, 6, 4, 6),
                Spacing = 20,
                Gravity = Gravity.CenterVertical,
                LayoutParameters = new LayoutParameters()
                {
                    Width = AutoSize.FillParent,
                    Height = AutoSize.FillParent,
                },
                SubViews = new View[]
                {
                    // date "column"
                    new TextNativeView(Date = new UILabel
                    {
                        //Font = UIFont.SystemFontOfSize(18)
                        Font = UIFont.SystemFontOfSize(UIFont.LabelFontSize)
                    }),
                    // the member number/source "column"
                    // this layout (column) is composed of "rows"
                    new LinearLayout(Orientation.Vertical)
                    {
                        Gravity = Gravity.Left,
                        LayoutParameters = new LayoutParameters()
                        {
                            Width = AutoSize.WrapContent,
                            Height = AutoSize.FillParent,
                        },
                        Spacing = 10,
                        SubViews = new View[]
                        {
                            new TextNativeView(MemberNumber = new UILabel
                            {
                                //Font = UIFont.SystemFontOfSize(14)
                                Font = UIFont.SystemFontOfSize(UIFont.SystemFontSize)
                            }),
                            new TextNativeView(Source = new UILabel
                            {
                                //Font = UIFont.SystemFontOfSize(14)
                                Font = UIFont.SystemFontOfSize(UIFont.SystemFontSize)
                            }),
                        }
                    },
                    // the reason "column"
                    new TextNativeView(Reasons = new UILabel
                    {
                        Font = UIFont.SystemFontOfSize(UIFont.SystemFontSize)
                    }),
                }
            };
            
            ContentView.Add(Host = new UILayoutHost(layout, ContentView.Bounds));
        }
    }

    /// <summary>Used to create the content of <see cref="TableViewCell"/>.
    /// </summary>
    public class TextNativeView : NativeView
    {
        public TextNativeView(UILabel view)
        {
            View = view;
            view.UserInteractionEnabled = false;
            LayoutParameters = new LayoutParameters()
            {
                Width = AutoSize.WrapContent,
                Height = AutoSize.WrapContent,
            };
        }
    }
}
