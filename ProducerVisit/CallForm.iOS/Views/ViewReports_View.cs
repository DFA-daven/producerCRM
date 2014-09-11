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
    using System.Reflection;
    using XibFree;

    /// <summary>The project's initial view controller. 
    /// </summary>
    /// <remarks>
    /// <para>This view inherits from <see cref="MvxViewController"/>. The main over-ridable methods 
    /// here handle the view's life-cycle.</para></remarks>
    class ViewReports_View : MvxViewController
    {
        static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        private bool _isOS7OrLater;


        #region declarations        
        // class-level declarations
        #region Properties
        /// <summary>Store for the logo button property.</summary>
        private UIButton _logoButton;
        /// <summary>Store for the logo orientation property.</summary>
        private LinearLayout _logoLinearLayout;
        /// <summary>Store for the logo property.</summary>
        private UIView _logoView;

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

        string _nameSpace = "CallForm.iOS.";
        #endregion
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
            CommonCore_iOS.DebugMessage("  [vr_v][vdl] > starting method...");

            #region pageLayout
            float topMargin = 0;

            // Note: The Navigation Controller is a UI-less View Controller responsible for
            // managing a stack of View Controllers and provides tools for navigation, such 
            // as a navigation bar with a back button.
            topMargin = StatusBarHeight() + NavBarHeight();
            CommonCore_iOS.DebugMessage("  [vr_v][vdl] > topMargin = " + topMargin.ToString() + " < <======= ");


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
                BackgroundColor = CommonCore_iOS.controlBackgroundColor,
                //BackgroundColor = UIColor.Blue,
            };
            filterField.VerticalAlignment = UIControlContentVerticalAlignment.Center;       // text should appear vertically centered
            filterField.HorizontalAlignment = UIControlContentHorizontalAlignment.Left;     // cursor should appear to right of placeholder; 
            filterField.HorizontalAlignment = UIControlContentHorizontalAlignment.Fill;     // cursor should appear to right of placeholder; 
            #endregion filterField

            #region findButton
            var findButton = _findButton = new UIButton(UIButtonType.Custom);
            findButton.Frame = new RectangleF(PercentWidth(middleControlOriginPercent), BannerBottom(), ControlWidth(), ControlHeight());
            findButton.SetTitle("Refresh", UIControlState.Normal);
            findButton.BackgroundColor = CommonCore_iOS.viewBackgroundColor;
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
            newButton.BackgroundColor = CommonCore_iOS.viewBackgroundColor;
            //newButton.BackgroundColor = UIColor.Red;
            #endregion newButton
            #endregion buttons

            #region table
            //var tableView = _reportTableView = new UITableView(new RectangleF(PercentWidth(leftControlOriginPercent), TableTop(), PercentWidth(98), ScreenHeight() - TableTop()));
            //var tableView = _reportTableView = new UITableView(new RectangleF(0, TableTop(), ScreenWidth(), ScreenHeight() - TableTop()));
            var tableView = _reportTableView = new UITableView(new RectangleF(0, TableTop(), View.Frame.Width, View.Frame.Height - TableTop()));
            tableView.BackgroundView = null;
            tableView.BackgroundColor = CommonCore_iOS.viewBackgroundColor;
            #endregion table

            #region loading
            var loading = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
            loading.Center = View.Center;
            loading.StartAnimating();

            var loadingOverlay = _loadingOverlay = new LoadingOverlay(UIScreen.MainScreen.Bounds);
            #endregion loading

            #endregion layout

            // Note: the order that Views are added determines their position in front of (or behind) other Views.
            // the buttons must have some off-set that is causing the gap.
#if (DEBUG || BETA)
    // No changes / variable assignment here -- this is diagnostic code!
    View.BackgroundColor = UIColor.Brown;
#endif
            View.Add(logoButton);
            //View.Add(logoView);
            //View.Add(logoLayout);

            View.Add(findButton);
            View.Add(filterField);
            View.Add(newButton);

            View.Add(tableView);
            
            View.Add(loading);
            View.Add(loadingOverlay);

            base.ViewDidLoad();

            #region bind content to view
            // Note: this is where the ViewReports_View view controller is created.
            // It's similar to having
            //   controller = new ViewReports_View();
            //   window.RootViewController = controller;
            // in AppDelegate.cs.
            var set = this.CreateBindingSet<ViewReports_View, ViewReports_ViewModel>();

            set.Bind(logoButton).To(vm => vm.GetReportsCommand);

            set.Bind(filterField).To(vm => vm.Filter);
            set.Bind(findButton).To(vm => vm.GetReportsCommand);
            set.Bind(newButton).To(vm => vm.NewVisitCommand);

            set.Bind(loading).For("Visibility").To(vm => vm.Loading).WithConversion("Visibility");
            set.Bind(loadingOverlay).For("Visibility").To(vm => vm.Loading).WithConversion("Visibility");
            
            set.Bind(tableView).For("Visibility").To(vm => vm.Loading).WithConversion("InvertedVisibility");
            set.Apply();

            #region UI action
            findButton.TouchUpInside += (sender, args) => { filterField.ResignFirstResponder(); };

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

            var source = new ViewReportsTableSource(ViewModel as ViewReports_ViewModel, tableView);

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

            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            CommonCore_iOS.DebugMessage("  [vr_v][vdl] > ...finished method.");
        }


        public override void ViewDidLayoutSubviews()
        {
            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            CommonCore_iOS.DebugMessage("  [vr_v][vdls] > System version: " + UIDevice.CurrentDevice.SystemVersion);

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
            if (_isOS7OrLater)
            {
                //displacement_y = this.TopLayoutGuide.Length;
            }

            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            CommonCore_iOS.DebugMessage("  [vr_v][vdls] > ...finished");
        }

        public override void MotionEnded(UIEventSubtype motion, UIEvent evt)
        {
            if (motion == UIEventSubtype.MotionShake)
            {
                // ToDo: use this to refresh the view reports table
            }

            base.MotionEnded(motion, evt);
        }

        /// <summary>The value of the device's screen.
        /// </summary>
        /// <returns>The screen value measured in points.</returns>
        internal float ViewFrameHeight()
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

        public override void ViewWillAppear(bool animated)
        {
            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);

            base.ViewWillAppear(animated);
            SetFramesForOrientation(InterfaceOrientation);
        }

        public override void ViewDidAppear(bool animated)
        {
            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            
            // Note: each time ViewReports is displayed/appears UploadReports() is triggered.
            base.ViewDidAppear(animated);
            (ViewModel as ViewReports_ViewModel).UploadReports();
            (ViewModel as ViewReports_ViewModel).Loading = false;
        }

        public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);

            base.WillAnimateRotation(toInterfaceOrientation, duration);

            SetFramesForOrientation(toInterfaceOrientation);
        }

        private void SetFramesForOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            CommonCore_iOS.DebugMessage("? [vr_v][sffo] > SetFramesForOrientation: make a note of when this runs");

            float difference = 0;
            float offset = 0;

            difference = Math.Abs(View.Frame.Width - View.Frame.Height);

            // Hack: this should be handled automatically with .gravity.
            switch (toInterfaceOrientation)
            {
                case UIInterfaceOrientation.Portrait:
                case UIInterfaceOrientation.PortraitUpsideDown:
                    break;

                case UIInterfaceOrientation.LandscapeLeft:
                case UIInterfaceOrientation.LandscapeRight:
                    offset = (difference / 2);
                    offset = (float)Math.Round(offset, 0);
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

        private float NavBarHeight()
        {
            float screenHeight = UIScreen.MainScreen.Bounds.Height;
            float layoutHeight = 0f;
            float navbarHeight = 0f;

            layoutHeight = this.ViewFrameHeight(); 
            navbarHeight = screenHeight - layoutHeight;

            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            CommonCore_iOS.DebugMessage("  [vr_v][nbh] > screenHeight: " + screenHeight.ToString() + ", layoutHeight = " + layoutHeight.ToString() + ", calc navbar value: " + navbarHeight.ToString() + " <=======");

            if (IsOS7OrLater())
            {
                navbarHeight = NavigationController.NavigationBar.Frame.Height; // the nearest ANCESTOR NavigationController
                layoutHeight = this.BottomLayoutGuide.Length - this.TopLayoutGuide.Length;
                CommonCore_iOS.DebugMessage("  [vr_v][nbh] > iOS 7 topMarginHeight: " + navbarHeight.ToString() + ", iOS7 layoutHeight = " + layoutHeight.ToString() + " <======= ");

            }

            return navbarHeight;
        }

        private float StatusBarHeight()
        {
            SizeF statusBarFrameSize = UIApplication.SharedApplication.StatusBarFrame.Size;
            return Math.Min(statusBarFrameSize.Width, statusBarFrameSize.Height);
        }

        /// <summary>Is this device running iOS 7.0.
        /// </summary>
        /// <returns>True if this is iOS 7.0.</returns>
        public bool IsOS7OrLater()
        {
            bool thisIsOS7 = false;
            string version = UIDevice.CurrentDevice.SystemVersion;
            string[] parts = version.Split('.');
            string major = parts[0];
            CommonCore_iOS.DebugMessage("  [vr_v][i7ol] > major version (string): " + major);
            int majorVersion = CommonCore_iOS.SafeConvert(major, 0);
            CommonCore_iOS.DebugMessage("  [vr_v][i7ol] > major version (int): " + majorVersion.ToString());

            if (majorVersion > 6)
            {
                thisIsOS7 = true;
            }

            CommonCore_iOS.DebugMessage("  [vr_v][i7ol] > version is higher than 6 = " + thisIsOS7.ToString());

            return thisIsOS7;
        }
    }

    public class ViewReportsTableSource : UITableViewSource
    {
        #region Properties
        private readonly ViewReports_ViewModel _viewModel;
        private const string CellIdentifier = "tableViewCell";
        #endregion

        public ViewReportsTableSource(ViewReports_ViewModel viewModel, UITableView tableView)
        {
            _viewModel = viewModel;

            _viewModel.PropertyChanged += (sender, args) =>
            {
                if (args.PropertyName == "Reports")
                {
                    tableView.ReloadData();
                }
            };
        }

        #region Overrides
        public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            // FixMe: remove hard-coded values (or add XML entry)
            return 50;
        }

        /// <summary>The number of rows (cells) in this section of <see cref="ViewReportsTableSource"/>.
        /// </summary>
        /// <param name="tableView">The <see cref="UITableView"/>/control that contains the section.</param>
        /// <param name="section">The index number of the section that contains the rows (cells).</param>
        /// <remarks><paramref name="section"/> is included as part of the override -- it is not used in this method.</remarks>
        /// <returns>A row count.</returns>
        public override int RowsInSection(UITableView tableview, int section)
        {
            return _viewModel.Reports == null ? 0 : _viewModel.Reports.Count;
        }

        /// <summary>Handles the selected row (cell) in <see cref="ViewReportsTableSource"/>.
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

        /// <summary>Gets a cell based on the selected <see cref="NSIndexPath">Row</see>.
        /// </summary>
        /// <param name="tableView">The <see cref="UITableView">(view) table</see> that contains the cell.</param>
        /// <param name="indexPath">The <see cref="NSIndexPath"/> to the selected row (cell).</param>
        /// <returns>The requested <see cref="TableViewCell" /> from the <see cref="ViewReportsTableSource"/>.</returns>
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            TableViewCell cell = tableView.DequeueReusableCell(CellIdentifier) as TableViewCell ?? new TableViewCell();

            ReportListItem reportListItem = _viewModel.Reports[indexPath.Row];

            /*
             * ToDo: indent the cells (rows)
             * cell.SeparatorInset = new UIEdgeInsets(0, 50, 0, 0);
             */
            cell.Date.Text = reportListItem.VisitDate.ToShortDateString();
            cell.MemberNumber.Text = reportListItem.MemberNumber;
            cell.Source.Text = reportListItem.UserEmail;
            cell.Reasons.Text = reportListItem.PrimaryReasonCode.Name;

            cell.Host.SetNeedsLayout();

            return cell;
        }
        #endregion
    }

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
                //Padding = new UIEdgeInsets(5, 5, 5, 5),
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
                    // the reason "column
                    new TextNativeView(Reasons = new UILabel
                    {
                        Font = UIFont.SystemFontOfSize(UIFont.SystemFontSize)
                    }),
                }
            };
            
            ContentView.Add(Host = new UILayoutHost(layout, ContentView.Bounds));
        }
    }

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
