namespace CallForm.iOS.Views
{
    using CallForm.Core.Models;
    using CallForm.Core.ViewModels;
    using Cirrious.MvvmCross.Binding.BindingContext;
    using Cirrious.MvvmCross.Touch.Views;
    using MonoTouch.Foundation;
    using MonoTouch.UIKit;
    using System;
    using System.Drawing;
    using XibFree;

    /// <summary>The project's initial view controller. 
    /// </summary>
    /// <remarks>
    /// <para>This view inherits from <see cref="MvxViewController"/>. The main over-ridable methods 
    /// here handle the view's life-cycle.</para></remarks>
    class ViewReports_View : MvxViewController
    {
        #region Properties
        /// <summary>Store for the logo property.</summary>
        private UIView _logoView;
        /// <summary>Store for the logo button property.</summary>
        private UIButton _logoButton;
        /// <summary>Store for the search filter property.</summary>
        private UITextField _filterField;
        /// <summary>Store for the button property.</summary>
        private UIButton _findButton, _newButton;
        /// <summary>Store for the report table property.</summary>
        private UITableView _reportTableView;
        /// <summary>Store for the logo orientation property.</summary>
        private LinearLayout _logoLinearLayout;
        #endregion

        #region Hard-coded values 
        /// <summary>The space reserved for the status-bar in iOS 7 and later.
        /// </summary>
        //private static float topMarginPixels = 65;

        // FixMe: until we get a new banner, just hiding the old one
        private static double bannerHeightPercent = 10;
        //private static double bannerHeightPercent = 0.5;

        /// <summary>The height of controls as a percentage of screen height.
        /// </summary>
        private static double controlHeightPercent = 8;

        /// <summary>The width of controls as a percentage of screen width.
        /// </summary>
        private static double controlWidthPercent = 33;

        /// <summary>The percentage of the horizontal width to indent this control's origin.
        /// </summary>
        private static double leftControlOriginPercent = 1;

        /// <summary>The percentage of the horizontal width to indent this control's origin.
        /// </summary>
        private static double middleControlOriginPercent = 33;

        /// <summary>The percentage of the horizontal width to indent this control's origin.
        /// </summary>
        private static double rightControlOriginPercent = 66;
        #endregion

        public override void ViewDidLoad()
        {
            // FixMe: this is incorrect: the Nav Bar isn't sahown on this view.
            float topMargin = 0;
            topMargin = NavigationController.NavigationBar.Frame.Height;

            //if (isOS7())
            //{
            //    topMargin += topMarginPixels;  
            //}

            var logoButton = _logoButton = new UIButton(UIButtonType.Custom);
            logoButton.Frame = new RectangleF(bannerHorizontalOrigin(), topMargin, bannerWidth(), bannerHeight());
            logoButton.SetTitle("DFA & DMS", UIControlState.Normal);
            logoButton.SetImage(UIImage.FromBundle("DFA-DMS-Banner.png"), UIControlState.Normal);
            logoButton.BackgroundColor = UIColor.White;

            var logoView = _logoView = new UIView();
            //var logoView = _logoView = new UIImageView
            //{
            //    //Image = UIImage.FromBundle("DFA-DMS-Banner.png"),
                
            //    ////Frame = new RectangleF(0, 70, 768, 128),
            //    Frame = new RectangleF(bannerHorizontalOrigin(), topMargin, bannerWidth(), bannerHeight())
                

            //};

            //logoView.Add(logoButton);
            //logoView.BackgroundColor = UIColor.White;

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
                            Gravity = Gravity.TopCenter,
                        }
                    }
                }
            };

            

            logoView = new UILayoutHost(logoLayout)
            {
                BackgroundColor = UIColor.White,
            };

            logoView.SizeToFit();

            // ToDo: place the 3 controls in a horizontal view with something like
            // var layout = new LinearLayout(Orientation.Horizontal)
            // SubViews = new View[]
            
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
                Frame = new RectangleF(percentWidth(leftControlOriginPercent), bannerBottom(), controlWidth(), controlHeight()),
                BackgroundColor = Common.controlBackgroundColor,
            };
            filterField.VerticalAlignment = UIControlContentVerticalAlignment.Center;

            var findButton = _findButton = new UIButton(UIButtonType.Custom);
            findButton.Frame = new RectangleF(percentWidth(middleControlOriginPercent), bannerBottom(), controlWidth(), controlHeight());
            findButton.SetTitle("Refresh", UIControlState.Normal);
            findButton.BackgroundColor = Common.viewBackgroundColor;

            var newButton = _newButton = new UIButton(UIButtonType.Custom);
            newButton.Frame = new RectangleF(percentWidth(rightControlOriginPercent), bannerBottom(), controlWidth(), controlHeight());
            newButton.SetTitle("New", UIControlState.Normal);
            // ToDo: scale the image so it fits in the control
            var plusSign = UIImage.FromBundle("Add.png");
            //plusSign.Scale();
            newButton.SetImage(UIImage.FromBundle("Add.png"), UIControlState.Normal);
            newButton.BackgroundColor = Common.viewBackgroundColor;

            //var tableView = _reportTableView = new UITableView(new RectangleF(percentWidth(leftControlOriginPercent), tableTop(), percentWidth(98), screenHeight() - tableTop()));
            var tableView = _reportTableView = new UITableView(new RectangleF(0, tableTop(), screenWidth(), screenHeight() - tableTop()));
            tableView.BackgroundView = null;
            tableView.BackgroundColor = Common.viewBackgroundColor;

            View.BackgroundColor = UIColor.White;
            View.Add(logoButton);
            //View.Add(logoView);
            //View.Add(logoLayout);
            View.Add(tableView);
            View.Add(findButton);
            View.Add(filterField);
            View.Add(newButton);

            var loading = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
            loading.Center = View.Center;
            loading.StartAnimating();
            View.Add(loading);

            base.ViewDidLoad();

            // Note: this is where the ViewReports_View view controller is created.
            // It's similar to having
            //   controller = new ViewReports_View();
            //   window.RootViewController = controller;
            // in AppDelegate.cs.

            var set = this.CreateBindingSet<ViewReports_View, ViewReports_ViewModel>();
            set.Bind(filterField).To(vm => vm.Filter);
            set.Bind(findButton).To(vm => vm.GetReportsCommand);
            set.Bind(loading).For("Visibility").To(vm => vm.Loading).WithConversion("Visibility");
            set.Bind(tableView).For("Visibility").To(vm => vm.Loading).WithConversion("InvertedVisibility");
            set.Bind(newButton).To(vm => vm.NewVisitCommand);
            set.Apply();

            findButton.TouchUpInside += (sender, args) => { filterField.ResignFirstResponder(); };

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
#if ALPHA
    appName += " (ALPHA)";
#elif BETA
    appName += " (BETA)";
#elif RELEASE
	appName = appName;
#elif DEBUG
	appName += " (DEBUG)";

#endif

            // FixMe: this only catches if the debugger is attached - so 'alpha' and 'beta' are never true.
            Title = appName + " " + appVersion;
            //Title = appName + " (VFRVBETA); " + appVersion;
            //this.AddChildViewController
            //this.availableHeight
        }

        // ToDo: manipulate base image? 
        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            // ToDo: use boolean method here
            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                float displacement_y = this.TopLayoutGuide.Length;

                // load sub-views with displacement
            }
        }

        private bool isOS7()
        {
            bool thisIsOS7 = false;

            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                float displacement_y = this.TopLayoutGuide.Length;

                thisIsOS7 = true;
            }

            return thisIsOS7;
        }

        /// <summary>The height of the device's screen.
        /// </summary>
        /// <returns>The screen height measured in points.</returns>
        internal float screenHeight()
        {
            //float screenHeight = UIScreen.MainScreen.Bounds.Height;
            float screenHeight = View.Frame.Height;
            return screenHeight;
        }

        /// <summary>The width of the device's screen.
        /// </summary>
        /// <returns>The screen width measured in points.</returns>
        internal float screenWidth()
        {
            float screenWidth = UIScreen.MainScreen.Bounds.Width;
            //float screenWidth = View.Frame.Width;
            return screenWidth;
        }

        /// <summary>The height of the portion of the screen available for Views.
        /// </summary>
        /// <returns>The available height measured in points.</returns>
        /// <remarks>For pre-iOS 7 devices, this value will be the same as <see cref="screenHeight"/>. For
        /// iOS 7 and later, the <see cref="topMarginPixels"/> are reserved.</remarks>
        internal float availableHeight()
        {
            float availableHeight = screenHeight();
            availableHeight = screenHeight() - NavigationController.NavigationBar.Frame.Height;

            //if (isOS7())
            //{
            //    availableHeight -= topMarginPixels;
            //}

            return availableHeight;
        }

        private float bannerHeight()
        {
            float maxAllowedBannerHeight = calculatePercent(availableHeight(), bannerHeightPercent);

            return maxAllowedBannerHeight;
        }

        private float bannerWidth()
        {
            float actualBannerWidth = UIImage.FromBundle("DFA-DMS-Banner.png").Size.Width;
            float actualBannerHeight = UIImage.FromBundle("DFA-DMS-Banner.png").Size.Height;

            float maxAllowedBannerWidth = screenWidth();
            float maxAllowedBannerHeight = bannerHeight();

            float heightRatio = maxAllowedBannerHeight / actualBannerHeight;
            
            float desiredBannerWidth = actualBannerWidth * heightRatio;

            if (desiredBannerWidth >= maxAllowedBannerWidth) 
            {
                desiredBannerWidth = maxAllowedBannerWidth;
            }

            return desiredBannerWidth;
        }

        private float bannerHorizontalOrigin()
        {
            float bannerHorizontalOrigin = 0;

            if (bannerWidth() < screenWidth())
            {
                bannerHorizontalOrigin = (screenWidth() - bannerWidth()) / 2;
            }

            return bannerHorizontalOrigin;
        }

        private float bannerBottom()
        {
            float bannerBottom = bannerHeight();
            bannerBottom = bannerHeight() - NavigationController.NavigationBar.Frame.Height;

            //if (isOS7())
            //{
            //    bannerBottom += topMarginPixels;
            //}
            
            return bannerBottom;
        }

        /// <summary>Calculates the pixel-height of controls based on the current screen height.
        /// </summary>
        /// <returns>The pixel-heighth of controls.</returns>
        internal float controlHeight()
        {
            //float controlHeight = calculatePercent(availableHeight(), controlHeightPercent);
            float controlHeight = UIFont.ButtonFontSize * 3f;

            return controlHeight;
        }

        /// <summary>Calculates the pixel-width of controls based on the current screen width.
        /// </summary>
        /// <returns>The pixel-width of controls.</returns>
        internal float controlWidth()
        {
            float controlWidth = percentWidth(controlWidthPercent);
            return controlWidth;
        }

        /// <summary>The top offset for the table.
        /// </summary>
        /// <returns></returns>
        internal float tableTop()
        {
            float tableTop = bannerBottom() + controlHeight();
            return tableTop;
        }

        /// <summary>Calculates the product of the current screen height and a percent.
        /// </summary>
        /// <param name="percent">A percent in the range 0 - 100.</param>
        /// <returns>A value representing a percent of the current screen height.</returns>
        private float percentHeight(double percent)
        {
            return calculatePercent(UIScreen.MainScreen.Bounds.Height, percent);
        }

        /// <summary>Calculates the product of the current screen width and a percent.
        /// </summary>
        /// <param name="percent">A percent in the range 0 - 100.</param>
        /// <returns>A value representing a percent of the current screen width.</returns>
        private float percentWidth(double percent)
        {
            float width = calculatePercent(UIScreen.MainScreen.Bounds.Width, percent);
            return width;
        }

        /// <summary>Calculates the product of a dimension and a percent.
        /// </summary>
        /// <param name="dimension">A dimension, such as screen width or height.</param>
        /// <param name="percent">A percent in the range 0 - 100.</param>
        /// <returns>The product of a given dimension and percent.</returns>
        private float calculatePercent(float dimension, double percent)
        {
            percent = percent / 100;
            double value = dimension * percent;
            value = Math.Abs(Math.Round(value));
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
            base.ViewWillAppear(animated);
            SetFramesForOrientation(InterfaceOrientation);
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            (ViewModel as ViewReports_ViewModel).UploadReports();
            (ViewModel as ViewReports_ViewModel).Loading = false;
        }

        public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
        {
            base.WillAnimateRotation(toInterfaceOrientation, duration);

            SetFramesForOrientation(toInterfaceOrientation);
        }

        private void SetFramesForOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            switch (toInterfaceOrientation)
            {
                case UIInterfaceOrientation.Portrait:
                case UIInterfaceOrientation.PortraitUpsideDown:
                    //SetFrameX(_logoView, 0);
                    SetFrameX(_logoButton, 0);
                    SetFrameX(_filterField, percentWidth(leftControlOriginPercent));
                    SetFrameX(_findButton, percentWidth(middleControlOriginPercent));
                    SetFrameX(_newButton, percentWidth(rightControlOriginPercent));
                    _reportTableView.Frame = new RectangleF(percentWidth(1), tableTop(), percentWidth(98), screenHeight() - tableTop());
                    break;
                case UIInterfaceOrientation.LandscapeLeft:
                case UIInterfaceOrientation.LandscapeRight:
                    float difference = Math.Abs(screenHeight() - screenWidth());
                    float offset = (difference / 2) + percentHeight(1);
                    //SetFrameX(_logoView, percentWidth(leftControlOriginPercent) + offset);
                    SetFrameX(_logoButton, percentWidth(leftControlOriginPercent) + offset);
                    SetFrameX(_filterField, percentWidth(leftControlOriginPercent) + offset);
                    SetFrameX(_findButton, percentWidth(middleControlOriginPercent) + offset);
                    SetFrameX(_newButton, percentWidth(rightControlOriginPercent) + offset);
                    _reportTableView.Frame = new RectangleF(percentHeight(1), tableTop(), percentHeight(98), screenWidth() - tableTop());


                    break;
                default:
                    throw new ArgumentOutOfRangeException("toInterfaceOrientation");
            }
        }

        /// <summary>Updates the horizontal origin of a view.
        /// </summary>
        /// <param name="view">The <see cref="UIView"/> to be updated.</param>
        /// <param name="x">The new "X" coordinate for the View.</param>
        internal void SetFrameX(UIView view, float x)
        {
            var frame = view.Frame;
            frame.X = x;
            view.Frame = frame;
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
            var layout = new LinearLayout(Orientation.Horizontal)
            {
                //Padding = new UIEdgeInsets(5, 5, 5, 5),
                Spacing = 20,
                Gravity = Gravity.CenterVertical,
                LayoutParameters = new LayoutParameters
                {
                    Width = AutoSize.FillParent,
                    Height = AutoSize.FillParent,
                },
                SubViews = new View[]
                {
                    new TextNativeView(Date = new UILabel
                    {
                        //Font = UIFont.SystemFontOfSize(18)
                        Font = UIFont.SystemFontOfSize(UIFont.LabelFontSize)
                    }),
                    new LinearLayout(Orientation.Vertical)
                    {
                        Gravity = Gravity.Left,
                        LayoutParameters = new LayoutParameters
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
            LayoutParameters = new LayoutParameters
            {
                Width = AutoSize.WrapContent,
                Height = AutoSize.WrapContent,
            };
        }
    }
}
