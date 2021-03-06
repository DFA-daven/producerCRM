﻿namespace CallForm.iOS.Views
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

    // notes: see _Touch UI.txt for design details.
    class ViewReports_View : MvxViewController
    {
        private UIView _logos;
        private UIButton _logoButton;
        private UITextField _filter;
        private UIButton _find, _new;
        private UITableView _table;
        private LinearLayout _logoLayout;

        // hard-coded values
        private static float topMarginPixels = 65;
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


        public override void ViewDidLoad()
        {
            UIColor controlBackgroundColor = UIColor.FromRGB(230, 230, 255);
            UIColor mainBackgroundColor = UIColor.FromRGB(200, 200, 255);

            float topMargin = 0;

            if (isOS7())
            {
                topMargin += topMarginPixels;  
            }

            var logoButton = _logoButton = new UIButton(UIButtonType.Custom);
            logoButton.Frame = new RectangleF(bannerHorizontalOrigin(), topMargin, bannerWidth(), bannerHeight());
            logoButton.SetTitle("DFA & DMS", UIControlState.Normal);
            logoButton.SetImage(UIImage.FromBundle("DFA-DMS-Banner.png"), UIControlState.Normal);
            logoButton.BackgroundColor = UIColor.White;

            var logoView = _logos = new UIView();
            //var logoView = _logos = new UIImageView
            //{
            //    //Image = UIImage.FromBundle("DFA-DMS-Banner.png"),
                
            //    ////Frame = new RectangleF(0, 70, 768, 128),
            //    Frame = new RectangleF(bannerHorizontalOrigin(), topMargin, bannerWidth(), bannerHeight())
                

            //};

            //logoView.Add(logoButton);
            //logoView.BackgroundColor = UIColor.White;

            var logoLayout = _logoLayout = new LinearLayout(Orientation.Vertical)
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
            
            var filterField = _filter = new UITextField
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
                BackgroundColor = controlBackgroundColor,
            };
            filterField.VerticalAlignment = UIControlContentVerticalAlignment.Center;

            var findButton = _find = new UIButton(UIButtonType.Custom);
            findButton.Frame = new RectangleF(percentWidth(middleControlOriginPercent), bannerBottom(), controlWidth(), controlHeight());
            findButton.SetTitle("Refresh", UIControlState.Normal);
            findButton.BackgroundColor = mainBackgroundColor;

            var newButton = _new = new UIButton(UIButtonType.Custom);
            newButton.Frame = new RectangleF(percentWidth(rightControlOriginPercent), bannerBottom(), controlWidth(), controlHeight());
            newButton.SetTitle("New", UIControlState.Normal);
            // ToDo: scale the image so it fits in the control
            var plusSign = UIImage.FromBundle("Add.png");
            //plusSign.Scale();
            newButton.SetImage(UIImage.FromBundle("Add.png"), UIControlState.Normal);
            newButton.BackgroundColor = mainBackgroundColor;

            //var tableView = _table = new UITableView(new RectangleF(percentWidth(leftControlOriginPercent), tableTop(), percentWidth(98), screenHeight() - tableTop()));
            var tableView = _table = new UITableView(new RectangleF(0, tableTop(), screenWidth(), screenHeight() - tableTop()));
            tableView.BackgroundView = null;
            tableView.BackgroundColor = mainBackgroundColor;

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

            var set = this.CreateBindingSet<ViewReports_View, ViewReports_ViewModel>();
            set.Bind(filterField).To(vm => vm.Filter);
            set.Bind(findButton).To(vm => vm.GetReportsCommand);
            set.Bind(loading).For("Visibility").To(vm => vm.Loading).WithConversion("Visibility");
            set.Bind(tableView).For("Visibility").To(vm => vm.Loading).WithConversion("InvertedVisibility");
            set.Bind(newButton).To(vm => vm.NewVisitCommand);
            set.Apply();

            findButton.TouchUpInside += (sender, args) => filterField.ResignFirstResponder();

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

        private float screenHeight()
        {
            float screenHeight = UIScreen.MainScreen.Bounds.Height;
            return screenHeight;
        }

        private float screenWidth()
        {
            float screenWidth = UIScreen.MainScreen.Bounds.Width;
            return screenWidth;
        }

        private float availableHeight()
        {
            float availableHeight = screenHeight();

            if (isOS7())
            {
                availableHeight -= topMarginPixels;
            }
            
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

            if (isOS7())
            {
                bannerBottom += topMarginPixels;
            }
            
            return bannerBottom;
        }

        /// <summary>Calculates the pixel-height of controls based on the current screen height.
        /// </summary>
        /// <returns>The pixel-heighth of controls.</returns>
        private float controlHeight()
        {
            float controlHeight = calculatePercent(availableHeight(), controlHeightPercent);
            return controlHeight;
        }

        /// <summary>Calculates the pixel-width of controls based on the current screen width.
        /// </summary>
        /// <returns>The pixel-width of controls.</returns>
        private float controlWidth()
        {
            float controlWidth = percentWidth(controlWidthPercent);
            return controlWidth;
        }

        private float tableTop()
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

        private void OnError(object sender, ErrorEventArgs errorEventArgs)
        {
            InvokeOnMainThread(() => new UIAlertView("Error", errorEventArgs.Message, null, "OK").Show());
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
                    //SetFrameX(_logos, 0);
                    SetFrameX(_logoButton, 0);
                    SetFrameX(_filter, percentWidth(leftControlOriginPercent));
                    SetFrameX(_find, percentWidth(middleControlOriginPercent));
                    SetFrameX(_new, percentWidth(rightControlOriginPercent));
                    _table.Frame = new RectangleF(percentWidth(1), tableTop(), percentWidth(98), screenHeight() - tableTop());
                    break;
                case UIInterfaceOrientation.LandscapeLeft:
                case UIInterfaceOrientation.LandscapeRight:
                    float difference = Math.Abs(screenHeight() - screenWidth());
                    float offset = (difference / 2) + percentHeight(1);
                    //SetFrameX(_logos, percentWidth(leftControlOriginPercent) + offset);
                    SetFrameX(_logoButton, percentWidth(leftControlOriginPercent) + offset);
                    SetFrameX(_filter, percentWidth(leftControlOriginPercent) + offset);
                    SetFrameX(_find, percentWidth(middleControlOriginPercent) + offset);
                    SetFrameX(_new, percentWidth(rightControlOriginPercent) + offset);
                    _table.Frame = new RectangleF(percentHeight(1), tableTop(), percentHeight(98), screenWidth() - tableTop());


                    break;
                default:
                    throw new ArgumentOutOfRangeException("toInterfaceOrientation");
            }
        }

        private void SetFrameX(UIView view, float x)
        {
            var frame = view.Frame;
            frame.X = x;
            view.Frame = frame;
        }
    }

    public class ViewReportsTableSource : UITableViewSource
    {
        private readonly ViewReports_ViewModel _viewModel;
        private const string CellIdentifier = "tableViewCell";


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

        public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            // FixMe: remove hard-coded values (or add XML entry)
            return 50;
        }

        public override int RowsInSection(UITableView tableview, int section)
        {
            // FixMe: why aren't the parameters being used here?
            return _viewModel.Reports == null ? 0 : _viewModel.Reports.Count;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            _viewModel.SelectedReport = _viewModel.Reports[indexPath.Row];
            _viewModel.ViewReportCommand.Execute(_viewModel);
            tableView.DeselectRow(indexPath, true);
            tableView.ReloadData();
        }

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
    }

    public class TableViewCell : UITableViewCell
    {
        public UILabel Date, MemberNumber, Source, Reasons;
        public UILayoutHost Host;

        // FixMe: plenty of hard-coded values in this one
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
                        Font = UIFont.SystemFontOfSize(18)
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
                                Font = UIFont.SystemFontOfSize(14)
                            }),
                            new TextNativeView(Source = new UILabel
                            {
                                Font = UIFont.SystemFontOfSize(14)
                            }),
                        }
                    },
                    new TextNativeView(Reasons = new UILabel
                    {
                        Font = UIFont.SystemFontOfSize(18)
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
