// CallForm.iOS\ViewElements\ReasonCodePickerDialog_ViewController.cs

namespace CallForm.iOS.ViewElements
{
    using CallForm.Core.Models;
    using CallForm.Core.ViewModels;
    using CallForm.iOS.Views;
    using CallForm.iOS.Views;
    using MonoTouch.Foundation;
    using MonoTouch.UIKit;
    using System;
    using System.Drawing;
    using System.Linq.Expressions;
    using System.Reflection;
    using XibFree;

    /// <summary>The class that defines View Element (control) for displaying and 
    /// selecting Reason Codes.
    /// </summary>
    public class ReasonCodePickerDialog_ViewController : UIViewController
    {
        string _nameSpace = "CallForm.iOS.";

        private readonly UITableView _table;
        private readonly NewVisit_ViewModel _viewModel;
        //private readonly float _heightFactor = 0.75f;
        //private readonly float _widthFactor = 0.75f;

        //private SizeF _size;
        //private float _reasonCodeHeight;
        //private float _reasonCodeWidth;

        /// <summary>Creates an instance of the <see cref="ReasonCodePickerDialog_ViewController"/> class.
        /// </summary>
        /// <param name="viewModel">The parent <see cref="MvxViewModel"/>.</param>
        /// <param name="source">The parent <see cref="UITableViewSource"/>.</param>
        public ReasonCodePickerDialog_ViewController(NewVisit_ViewModel viewModel, NewVisit_TableViewSource source)
        {
            _viewModel = viewModel;
            _table = new UITableView();
            _table.Source = new ReasonCodeTableSource(_viewModel, source);

            // ToDo: Gray with no direction-arrow looks pretty good!
            //_reportTableView.BackgroundColor = UIColor.Gray;
            //_table.AutoresizingMask = UIViewAutoresizing.FlexibleBottomMargin | UIViewAutoresizing.FlexibleRightMargin;

            // Note: using cell height won't work -- the cell's don't exist yet
            //int sectionNumber = 0;
            //int count = _reportTableView.Source.RowsInSection(_reportTableView, sectionNumber);
            //UITableViewCell aCell = _reportTableView.VisibleCells[0];
            //float aCellHeight = aCell.Frame.Height;
            //float preferredHeight = count * aCellHeight;

            float safestMaxWidth = 0f;

            // find the smaller dimension
            safestMaxWidth = Math.Min(UIScreen.MainScreen.Bounds.Height, UIScreen.MainScreen.Bounds.Width);
            // 75% of the dimension, rounded off to zero decimal places
            safestMaxWidth = (float)Math.Round(safestMaxWidth * 0.75, 0);

            float reasonCodeHeight = (float)Math.Round(UIScreen.MainScreen.Bounds.Height * 0.50, 0);  // the Y value
            //float reasonCodeWidth = (float)Math.Round(UIScreen.MainScreen.Bounds.Width * 0.75, 0);    // the X value

            //reasonCodeHeight = _table.ContentSize.Height;
            //reasonCodeHeight = View.Frame.Height;
            //reasonCodeHeight = UIScreen.MainScreen.Bounds.Height;

            CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            CommonCore_iOS.DebugMessage(" > reasonCodeHeight = " + reasonCodeHeight.ToString() + ", safestMaxWidth = " + safestMaxWidth.ToString() + " < = = = = = =########" );

            //if (reasonCodeHeight < reasonCodeWidth)
            //{
            //    reasonCodeHeight = reasonCodeWidth;
            //}

            // Note: offset here is displayed as whitespace between the NW corner of the popover and the NW corner of the content.
            _table.Frame = new RectangleF(0, 0, safestMaxWidth, reasonCodeHeight);
#if (DEBUG || BETA)
            _table.BackgroundColor = UIColor.Green;
#endif

            // FixMe: the frame sizes are still not quite right -- problems when the screen is rotated.

            _table.ScrollEnabled = true; // scrolling in the ReasonCode table -- not the container...

            //View.BackgroundColor = UIColor.Blue; // this is the inner view -- the cell rows
            View.Add(_table); 
            View.SizeToFit();
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

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            _viewModel.RaisePropertyChanged(GetPropertyName(() => _viewModel.SelectedReasonCodes));
        }

        //internal float ReasonCodeHeight
        //{
        //    get
        //    {
        //        return _reasonCodeHeight;
        //    }
        //    set
        //    {
        //        _reasonCodeHeight = value;
        //    }
        //}

        //internal float ReasonCodeWidth
        //{
        //    get
        //    {
        //        return _reasonCodeWidth;
        //    }
        //    set
        //    {
        //        _reasonCodeWidth = value;
        //    }
        //}

        public override SizeF PreferredContentSize
        {
            get
            {
                SizeF size;
                size = _table.Frame.Size;
                //size.Height = _table.Source.RowsInSection(_table, 1)  * 50f;

                float layoutHeight = 0f;
                UIView[] subviews = View.Subviews;

                CommonCore_iOS.DebugMessage(" [rcpd][pcs] > Calculating layoutHeight....");
                if (subviews == null)
                {
                    CommonCore_iOS.DebugMessage(" [rcpd][pcs] > View.Subviews[] is NULL.");
                }
                else
                {
                    int subviewsArrayLength = subviews.Length;
                    CommonCore_iOS.DebugMessage(" [nv_v][lh] > View.Subviews[] is NOT null. subviewsArrayLength = " + subviewsArrayLength.ToString());
                    for (int i = 0; i < subviewsArrayLength; i++)
                    {
                        if (subviews[i].GetType() == typeof(UIView))
                        {
                            CommonCore_iOS.DebugMessage(" [nv_v][lh] > View.Subviews[" + i.ToString() + "] == typeof(UIView), Height = " + subviews[i].Frame.Height.ToString());
                        }
                        else
                        {
                            CommonCore_iOS.DebugMessage(" [nv_v][lh] > View.Subviews[" + i.ToString() + "] is wrapping something: Height = " + subviews[i].Frame.Height.ToString());
                            layoutHeight = subviews[i].Frame.Height;
                        }
                    }
                }

                // leave space for "Done" button
                //size.Height += 50;
                //size.Height = (float)Math.Round(UIScreen.MainScreen.Bounds.Height * 0.75, 0);

                CommonCore_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
                CommonCore_iOS.DebugMessage(" > PreferredContentSize, _table.Frame.Size.Height = " + size.Height.ToString() + ", Width = " + size.Width.ToString() + " <= = = = = = = ");
                //CommonCore_iOS.DebugMessage(" > PreferredContentSize Height = " + size.Height.ToString() + ", Width = " + size.Width.ToString() + " <= = = = = = = ");

                return size;
            }
            set { base.PreferredContentSize = value; }
        }

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

        ///// <summary>Calculates a value representing a percent of the <see cref="UIView.Frame"/> height.
        ///// </summary>
        ///// <param name="percent">A percent value. Ex: 25.0</param>
        ///// <returns>The product of (<see cref="UIView.Frame"/> * <paramref name="percent"/>)</returns>
        //internal float PercentOfFrameHeight(double percent)
        //{
        //    float value = (float)Math.Round( PercentOfRectangleHeight(_reportTableView.Frame, percent), 0);
        //    return value;
        //}

        ///// <summary>Calculates a value representing a <paramref name="percent"/> of the <paramref name="rectangle"/> height.
        ///// </summary>
        ///// <param name="rectangle">The <see cref="RectangleF"/> object.</param>
        ///// <param name="percent">A percent value. Ex: 25.0</param>
        ///// <returns>The product of (<paramref name="rectangle">rectangle.Height</see> * <paramref name="percent"/>)</returns>
        //internal float PercentOfRectangleHeight(RectangleF rectangle, double percent)
        //{
        //    percent = percent / 100;
        //    float value = (float)Math.Round((rectangle.Height * percent), 0);
        //    return value;
        //}

        ///// <summary>Calculates a value representing a percent of the <see cref="UIView.Frame"/> width.
        ///// </summary>
        ///// <param name="percent">A percent value. Ex: 25.0</param>
        ///// <returns>The product of (<see cref="UIView.Frame"/> * <paramref name="percent"/>)</returns>
        //internal float PercentOfFrameWidth(double percent)
        //{
        //    float value = (float)Math.Round( PercentOfRectangleWidth(_reportTableView.Frame, percent), 0);
        //    return value;
        //}

        ///// <summary>Calculates a value representing a <paramref name="percent"/> of the <paramref name="rectangle"/> width.
        ///// </summary>
        ///// <param name="rectangle">The <see cref="RectangleF"/> object.</param>
        ///// <param name="percent">A percent value. Ex: 25.0</param>
        ///// <returns>The product of (<paramref name="rectangle">rectangle.Width</see> * <paramref name="percent"/>)</returns>
        //internal float PercentOfRectangleWidth(RectangleF rectangle, double percent)
        //{
        //    percent = percent / 100;
        //    float value = (float)Math.Round((rectangle.Width * percent), 0);
        //    return value;
        //}
    }

    public class ReasonCodeTableSource : UITableViewSource
    {
        string _nameSpace = "CallForm.iOS.";

        private readonly NewVisit_ViewModel _viewModel;
        private readonly NewVisit_TableViewSource _source;
        private const string CellIdentifier = "ReasonCodeTableCell";
        private float _doneButtonHeight = 50f;

        public ReasonCodeTableSource(NewVisit_ViewModel viewModel, NewVisit_TableViewSource source)
        {
            _viewModel = viewModel;
            _source = source;
        }

        /// <summary>The number of rows (cells) in this section of <see cref="ReasonCodeTableSource"/>.
        /// </summary>
        /// <param name="tableView">The <see cref="UITableView"/>/control that contains the section.</param>
        /// <param name="section">The index number of the section that contains the rows (cells).</param>
        /// <remarks><paramref name="section"/> is included as part of the override -- it is not used in this method.</remarks>
        /// <returns>A row count.</returns>
        public override int RowsInSection(UITableView tableview, int section)
        {
            return _viewModel.ListOfReasonCodes.Count;
        }

        public override UIView GetViewForFooter(UITableView tableView, int section)
        {
            var doneButton = new UIButton(UIButtonType.System);
            // Hack: hide Done button

            doneButton.SetTitle("Done", UIControlState.Normal);
            // review: is InvokeOnMainThread() correct?
            doneButton.TouchUpInside += (sender, args) => { InvokeOnMainThread(_source.SafeDismissPopover); };
            //doneButton.TouchUpInside += (sender, args) => { Invoke(_source.SafeDismissPopover, 0); };
            doneButton.Frame = new RectangleF(0, 0, tableView.Frame.Width, _doneButtonHeight);

            // Hack: hide Done button.
            _doneButtonHeight = 0f;
            doneButton.Hidden = true;

            return doneButton;
        }

        /// <summary>Toggles the selected row (cell) in <see cref="ReasonCodeTableSource"/>.
        /// </summary>
        /// <param name="tableView">The <see cref="UITableView"/>/control that contains the selected row.</param>
        /// <param name="indexPath">The <see cref="NSIndexPath"/> of the selected row in the control.</param>
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            ReasonCode currentlySelectedReasonCode = _viewModel.ListOfReasonCodes[indexPath.Row];
            if (_viewModel.SelectedReasonCodes.Contains(currentlySelectedReasonCode))
            {
                _viewModel.SelectedReasonCodes.Remove(currentlySelectedReasonCode);
            }
            else
            {
                _viewModel.SelectedReasonCodes.Add(currentlySelectedReasonCode);
            }
            _viewModel.RaisePropertyChanged(GetPropertyName(() => _viewModel.SelectedReasonCodes));
            tableView.DeselectRow(indexPath, true);
            // Review: will reloadData() work in other places?
            tableView.ReloadData();
        }

        public override float GetHeightForFooter(UITableView tableView, int section)
        {
            float heightToReport = _doneButtonHeight;

            return heightToReport;
        }

        /// <summary>Gets a cell based on the selected <see cref="NSIndexPath">Row</see>.
        /// </summary>
        /// <param name="tableView">The active <see cref="UITableView"/>.</param>
        /// <param name="indexPath">The <see cref="NSIndexPath"/> with the selected row (cell).</param>
        /// <returns>The requested <see cref="UITableViewCell" /> from the <see cref="ReasonCodeTableSource"/>.</returns>
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell(CellIdentifier) ??
                                   new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier);
            ReasonCode reasonCode = _viewModel.ListOfReasonCodes[indexPath.Row];
            cell.TextLabel.Text = reasonCode.Name;
            cell.Accessory = _viewModel.SelectedReasonCodes.Contains(reasonCode) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
            return cell;
        }

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
}
