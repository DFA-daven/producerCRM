using System;
using System.Drawing;
using System.Xml;
using CallForm.Core.Models;
using CallForm.Core.ViewModels;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Touch.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using XibFree;

namespace CallForm.iOS.Views
{
    class ViewFarmReportsView : MvxViewController
    {
        private UIImageView _logos;
        private UITextField _filter;
        private UIButton _find, _new;
        private UITableView _table;

        public override void ViewDidLoad()
        {
            View.Add(_logos = new UIImageView
            {
                Image = UIImage.FromBundle("Dairylea-Banner.png"),
                Frame = new RectangleF(0, 70, 768, 128),
            });

            var filterField = _filter = new UITextField
            {
                TextAlignment = UITextAlignment.Center,
                KeyboardType = UIKeyboardType.NumberPad,
                Placeholder = "Farm Number",
                ShouldChangeCharacters = (field, range, replacementString) =>
                {
                    int i;
                    return replacementString.Length <= 0 || int.TryParse(replacementString, out i);
                },
                Font = UIFont.SystemFontOfSize(20),
                Frame = new RectangleF(0, 203, UIScreen.MainScreen.Bounds.Width - 430, 50),
                BackgroundColor = UIColor.FromRGB(230, 230, 255),
            };
            filterField.VerticalAlignment = UIControlContentVerticalAlignment.Center;

            var findButton = _find = new UIButton(UIButtonType.Custom);
            findButton.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 420, 203, 200, 50);
            findButton.SetTitle("Find Reports", UIControlState.Normal);

            var newButton = _new = new UIButton(UIButtonType.Custom);
            newButton.Frame = new RectangleF(UIScreen.MainScreen.Bounds.Width - 210, 203, 200, 50);
            newButton.SetTitle("New Report", UIControlState.Normal);
            newButton.SetImage(UIImage.FromBundle("Add.png"), UIControlState.Normal);

            var tableView = _table = new UITableView(new RectangleF(0, 266, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height - 266));
            tableView.BackgroundView = null;
            tableView.BackgroundColor = UIColor.FromRGB(200, 200, 255);
            View.BackgroundColor = UIColor.FromRGB(200, 200, 255);

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

            Title = "Producer Contact 1.4.038.2";
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
                    SetFrameX(_filter, 0);
                    SetFrameX(_find, UIScreen.MainScreen.Bounds.Width - 420);
                    SetFrameX(_new, UIScreen.MainScreen.Bounds.Width - 210);
                    _table.Frame = new RectangleF(0, 266, UIScreen.MainScreen.Bounds.Width,
                        UIScreen.MainScreen.Bounds.Height - 266);
                    break;
                case UIInterfaceOrientation.LandscapeLeft:
                case UIInterfaceOrientation.LandscapeRight:
                    float offset = UIScreen.MainScreen.Bounds.Height - UIScreen.MainScreen.Bounds.Width;
                    SetFrameX(_logos, offset / 2);
                    SetFrameX(_filter, offset / 2);
                    SetFrameX(_find, UIScreen.MainScreen.Bounds.Width - 420 + offset / 2);
                    SetFrameX(_new, UIScreen.MainScreen.Bounds.Width - 210 + offset / 2);
                    _table.Frame = new RectangleF(offset / 2, 266, UIScreen.MainScreen.Bounds.Width,
                        UIScreen.MainScreen.Bounds.Width - 266);

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
