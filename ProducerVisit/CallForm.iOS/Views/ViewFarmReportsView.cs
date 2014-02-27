using CallForm.Core.Models;
using CallForm.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.Drawing;
using XibFree;

namespace CallForm.iOS.Views
{
    class ViewFarmReportsView : MvxViewController
    {
        private UIImageView _logos;
        private UITextField _filter;
        private UIButton _find, _new;
        private UITableView _table;

        // hard-coded values
        private static float topMarginPixels = 65;
        private static double bannerHeightPercent = 12.5;

        /// <summary>The height of controls as a percentage of screen height.
        /// </summary>
        private static double controlHeightPercent = 6.25;

        /// <summary>The width of controls as a percentage of screen width.
        /// </summary>
        private static double controlWidthPercent = 31;

        /// <summary>The percentage of the horizontal width to indent this control's origin.
        /// </summary>
        private static double leftControlOriginPercent = 1;

        /// <summary>The percentage of the horizontal width to indent this control's origin.
        /// </summary>
        private static double middleControlOriginPercent = 34;

        /// <summary>The percentage of the horizontal width to indent this control's origin.
        /// </summary>
        private static double rightControlOriginPercent = 66;


        public override void ViewDidLoad()
        {
            UIColor controlBackgroundColor = UIColor.FromRGB(230, 230, 255);
            UIColor viewBackgroundColor = UIColor.FromRGB(200, 200, 255);

            float topMargin = 0;

            if (isOS7())
            {
                topMargin += topMarginPixels;  
            }

            View.Add(_logos = new UIImageView
            {
                Image = UIImage.FromBundle("Dairylea-Banner.png"),
                //Frame = new RectangleF(0, 70, 768, 128),


                Frame = new RectangleF(0, topMargin, screenWidth(), bannerHeight()),
            });

            // todo: place the 3 controls in a horizontal view with something like
            // var layout = new LinearLayout(Orientation.Horizontal)
            // SubViews = new View[]
            
            var filterField = _filter = new UITextField
            {
                TextAlignment = UITextAlignment.Center,
                KeyboardType = UIKeyboardType.NumberPad,
                Placeholder = "Farm #",
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
            findButton.BackgroundColor = viewBackgroundColor;

            var newButton = _new = new UIButton(UIButtonType.Custom);
            newButton.Frame = new RectangleF(percentWidth(rightControlOriginPercent), bannerBottom(), controlWidth(), controlHeight());
            newButton.SetTitle("New", UIControlState.Normal);
            // todo: scale the image so it fits in the control
            var plusSign = UIImage.FromBundle("Add.png");
            //plusSign.Scale();
            newButton.SetImage(UIImage.FromBundle("Add.png"), UIControlState.Normal);

            newButton.BackgroundColor = viewBackgroundColor;

            var tableView = _table = new UITableView(new RectangleF(percentWidth(leftControlOriginPercent), tableTop(), percentWidth(98), screenHeight() - tableTop()));
            tableView.BackgroundView = null;
            tableView.BackgroundColor = viewBackgroundColor;
            View.BackgroundColor = viewBackgroundColor;

            View.Add(tableView);
            View.Add(findButton);
            View.Add(filterField);
            View.Add(newButton);

            var loading = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
            loading.Center = View.Center;
            loading.StartAnimating();
            View.Add(loading);

            base.ViewDidLoad();

            var set = this.CreateBindingSet<ViewFarmReportsView, ViewFarmReportsViewModel>();
            set.Bind(filterField).To(vm => vm.Filter);
            set.Bind(findButton).To(vm => vm.GetReportsCommand);
            set.Bind(loading).For("Visibility").To(vm => vm.Loading).WithConversion("Visibility");
            set.Bind(tableView).For("Visibility").To(vm => vm.Loading).WithConversion("InvertedVisibility");
            set.Bind(newButton).To(vm => vm.NewVisitCommand);
            set.Apply();

            findButton.TouchUpInside += (sender, args) => filterField.ResignFirstResponder();

            (ViewModel as ViewFarmReportsViewModel).Error += OnError;

            var source = new ViewReportsTableSource(ViewModel as ViewFarmReportsViewModel, tableView);

            tableView.Source = source;

            // fixme: find way to get the app's title
            // get current values from the assembly
            var assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
            string appName = assemblyName.Name;                     // the name of this project -- CallForm.iOS
            appName = "Field Contact";
            string appVersion = assemblyName.Version.ToString();    // the version number
            ////var up = System.Reflection.Ass

            #if DEBUG
                appName += " (DEBUG)";
            #elif ALPHA
                appName += " (ALPHA)";
            #elif BETA
                appName += " (BETA)";
            #endif

            Title = appName + "; " + appVersion;
        }

        // todo: manipulate base image? 
        public override void ViewDidLayoutSubviews()
        {
            base.ViewDidLayoutSubviews();

            if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
            {
                float displacement_y = this.TopLayoutGuide.Length;

                //load subviews with displacement
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
            float bannerHeight = calculatePercent(availableHeight(), bannerHeightPercent);
            return bannerHeight;
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
            (ViewModel as ViewFarmReportsViewModel).UploadReports();
            (ViewModel as ViewFarmReportsViewModel).Loading = false;
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
                    SetFrameX(_logos, 0);
                    SetFrameX(_filter, percentWidth(leftControlOriginPercent));
                    SetFrameX(_find, percentWidth(middleControlOriginPercent));
                    SetFrameX(_new, percentWidth(rightControlOriginPercent));
                    _table.Frame = new RectangleF(percentWidth(1), tableTop(), percentWidth(98), screenHeight() - tableTop());
                    break;
                case UIInterfaceOrientation.LandscapeLeft:
                case UIInterfaceOrientation.LandscapeRight:
                    float difference = Math.Abs(screenHeight() - screenWidth());
                    float offset = difference / 2;
                    SetFrameX(_logos, offset);
                    SetFrameX(_filter, percentWidth(leftControlOriginPercent) + offset);
                    SetFrameX(_find, percentWidth(middleControlOriginPercent) + offset);
                    SetFrameX(_new, percentWidth(rightControlOriginPercent) + offset);
                    _table.Frame = new RectangleF(offset, tableTop(), percentWidth(98), screenHeight() - tableTop());

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
        private readonly ViewFarmReportsViewModel _viewModel;
        private const string CellIdentifier = "tableViewCell";


        public ViewReportsTableSource(ViewFarmReportsViewModel viewModel, UITableView tableView)
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
            return 50;
        }

        public override int RowsInSection(UITableView tableview, int section)
        {
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

            ReportListItem rli = _viewModel.Reports[indexPath.Row];

            cell.Date.Text = rli.VisitDate.ToShortDateString();
            cell.FarmNo.Text = rli.FarmNumber;
            cell.Source.Text = rli.UserEmail;
            cell.Reasons.Text = rli.PrimaryReasonCode.Name;

            cell.Host.SetNeedsLayout();

            return cell;
        }
    }

    public class TableViewCell : UITableViewCell
    {
        public UILabel Date, FarmNo, Source, Reasons;
        public UILayoutHost Host;

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
                            new TextNativeView(FarmNo = new UILabel
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
