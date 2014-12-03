// CallForm.iOS\ViewElements\ReasonCodePickerDialog_ViewController.cs

namespace CallForm.iOS.ViewElements
{
    using CallForm.Core.Models;
    using CallForm.Core.ViewModels;
    using CallForm.iOS.Views;
    using MonoTouch.Foundation;
    using MonoTouch.UIKit;
    using System;
    using System.Drawing;
    using System.Linq.Expressions;
    //using System.Reflection;
    using XibFree;

    /// <summary>The class that defines View Element (control) for displaying and 
    /// selecting Reason Codes.
    /// </summary>
    public class ReasonCodePickerDialog_ViewController : UIViewController
    {
		string _namespace = "CallForm.iOS.ViewElements.";
		string _class = "ReasonCodePickerDialog_ViewController.";
		string _method = "TBD";

        private readonly NewVisit_ViewModel _viewModel;
        private readonly NewVisit_TableViewSource _source;
        private readonly UITableView _table;

        /// <summary>Creates an instance of the <see cref="ReasonCodePickerDialog_ViewController"/> class. This holds the "content" inside the _popoverController.
        /// </summary>
        /// <param name="viewModel">The parent <see cref="MvxViewModel"/>.</param>
        /// <param name="source">The parent <see cref="UITableViewSource"/>.</param>
        /// <remarks>This ViewController is created when NewVisit_View is loaded.</remarks>
        public ReasonCodePickerDialog_ViewController(NewVisit_ViewModel viewModel, NewVisit_TableViewSource source)
        {
			_method = "ReasonCodePickerDialog_ViewController";
			Common_iOS.DebugMessage(_namespace + _class, _method);            

            _viewModel = viewModel;
            _source = source;

            _table = new UITableView();
            _table.Source = new ReasonCodeTableSource(_viewModel, source);

            // Note: using cell value won't work -- the cell's don't exist yet                       

            // x% of the dimension, rounded off to zero decimal places
            float halfScreenHeight = (float)Math.Round(UIScreen.MainScreen.Bounds.Height * 0.50, 0);  // the Y value
            float halfScreenWidth = (float)Math.Round(UIScreen.MainScreen.Bounds.Width * 0.50, 0);    // the X value
            // find the smaller dimension
            float safeContentHeight = Math.Min(halfScreenHeight, halfScreenWidth);

            float threeQuarterScreenHeight = (float)Math.Round(UIScreen.MainScreen.Bounds.Height * 0.75, 0);  // the Y value
            float threeQuarterScreenWidth = (float)Math.Round(UIScreen.MainScreen.Bounds.Width * 0.75, 0);    // the X value
            float safeContentWidth = Math.Min(threeQuarterScreenHeight, threeQuarterScreenWidth);

            // Note: don't attempt to call base in this method
            //float rowHeight = _table.EstimatedRowHeight; 
            float rowHeight = _table.RowHeight;
            int rowCount = _viewModel.ListOfReasonCodes.Count;
            rowCount = rowCount + 2; // add two to take into account the footer and header
            
            float estimatedContentHeight = (float)Math.Round(rowHeight * rowCount, 0);
            Common_iOS.DebugMessage("  [rcpd_vc][rcpd_vc] > estimatedContentHeight = " + estimatedContentHeight.ToString() + ", safeContentHeight = " + safeContentHeight.ToString() + " < [rcpd_vc][rcpd_vc] @ @ @ @");

            // Note: safeContentHeight defines the value of the "content". If it's larger than NewVisit_TableViewSource.availableDisplayHeight rows will be un-clickable.
            safeContentHeight = Math.Min(safeContentHeight, estimatedContentHeight);
            Common_iOS.DebugMessage("* [rcpd_vc][rcpd_vc] > safeContentHeight = " + safeContentHeight.ToString() + " < [rcpd_vc][rcpd_vc] @ @ @ @ @ @ @");
            
            // Note: offset here is displayed as whitespace between the NW corner of the popover and the NW corner of the content.
            _table.Frame = new RectangleF(0, 0, safeContentWidth, safeContentHeight);
#if (DEBUG || BETA)
            // No changes / variable assignment here -- this is diagnostic code!
            _table.BackgroundColor = UIColor.Green; // this is the background of the _table -- the color behind the rows.
            View.BackgroundColor = UIColor.Blue; // this is the background of the _popoverController -- the space beneath/behind the _table.
#endif
            // Review: does this line make any difference?
            _table.AutoresizingMask = UIViewAutoresizing.FlexibleBottomMargin | UIViewAutoresizing.FlexibleRightMargin;

            _table.ScrollEnabled = true; // disabling locks the rows in the _popoverController

            View.Add(_table); 
            View.SizeToFit();
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
                return UIRectEdge.None;
            }
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            _viewModel.RaisePropertyChanged(GetPropertyName(() => _viewModel.SelectedReasonCodes));
        }

        //public override void ViewDidLoad()
        //{
		//    _method = "ViewDidLoad";
		//    Common_iOS.DebugMessage(_nameSpace + _class, _method);
        //    Common_iOS.DebugMessage("? [rcpd_vc][vdl] > make a note of when this is being run");
        //}

        //public override void ViewDidLayoutSubviews()
        //{
		//    _method = "ViewDidLayoutSubviews";
		//    Common_iOS.DebugMessage(_nameSpace + _class, _method);
        //    Common_iOS.DebugMessage("? [rcpd_vc][vdls] > make a note of when this is being run");
        //}

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

                float rowHeight = _table.RowHeight;
                if (rowHeight < 1)
                {
                    rowHeight = _source.RowHeight;
                }

                int rowCount = _viewModel.ListOfReasonCodes.Count;
                float preferredHeight = (float)Math.Round(rowHeight * rowCount, 0);

                float layoutHeight = 0f;
                UIView[] subviews = View.Subviews;

                Common_iOS.DebugMessage("  [rcpd_vc][pcs][g] > rowHeight = " + rowHeight.ToString() + ", rowCount = " + rowCount.ToString() + ". Calculating layoutHeight....");
                if (subviews == null)
                {
                    Common_iOS.DebugMessage("  [rcpd_vc][pcs][g] > View.Subviews[] is NULL.");
                }
                else
                {
                    int subviewsArrayLength = subviews.Length;
                    Common_iOS.DebugMessage("  [rcpd_vc][pcs][g] > View.Subviews[] is NOT null. subviewsArrayLength = " + subviewsArrayLength.ToString());
                    for (int i = 0; i < subviewsArrayLength; i++)
                    {
                        if (subviews[i].GetType() == typeof(UIView))
                        {
                            Common_iOS.DebugMessage("  [rcpd_vc][pcs][g] > View.Subviews[" + i.ToString() + "] == typeof(UIView), Height = " + subviews[i].Frame.Height.ToString());
                        }
                        else
                        {
                            Common_iOS.DebugMessage("  [rcpd_vc][pcs][g] > View.Subviews[" + i.ToString() + "] is wrapping something: Height = " + subviews[i].Frame.Height.ToString());
                            layoutHeight = subviews[i].Frame.Height;
                        }
                    }
                }

                // leave space for "Done" button
                //size.Height += 50;
                //size.Height = (float)Math.Round(UIScreen.MainScreen.Bounds.Height * 0.75, 0);
                size.Height = Math.Max(preferredHeight, layoutHeight);

                // Common_iOS.DebugMessage("  [rcpd_vc][pcs][g] > _table.Frame.Size.Height = " + size.Height.ToString() + ", Width = " + size.Width.ToString() + " [rcpd_vc][pcs][g] <= = = = = = = ");
                Common_iOS.DebugMessage("  [rcpd_vc][pcs][g] > PreferredContentSize Height = " + size.Height.ToString() + ", Width = " + size.Width.ToString() + " [rcpd_vc][pcs][g] <= = = = = = = ");

                return size;
            }
            set 
            {
                Common_iOS.DebugMessage("  [rcpd_vc][pcs][s] > value.Height = " + value.Height.ToString() + ", Width = " + value.Width.ToString() + " [rcpd_vc][pcs][s] <= = = = = = = ");
                base.PreferredContentSize = value; 
            }
        }
        #pragma warning restore 1591
        #endregion overrides

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

        ///// <summary>Calculates a value representing a percent of the <see cref="UIView.Frame"/> value.
        ///// </summary>
        ///// <param name="percent">A percent value. Ex: 25.0</param>
        ///// <returns>The product of (<see cref="UIView.Frame"/> * <paramref name="percent"/>)</returns>
        //internal float PercentOfFrameHeight(double percent)
        //{
        //    float value = (float)Math.Round( PercentOfRectangleHeight(_reportTableView.Frame, percent), 0);
        //    return value;
        //}

        ///// <summary>Calculates a value representing a <paramref name="percent"/> of the <paramref name="rectangle"/> value.
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

        ///// <summary>Calculates a value representing a percent of the <see cref="UIView.Frame"/> value.
        ///// </summary>
        ///// <param name="percent">A percent value. Ex: 25.0</param>
        ///// <returns>The product of (<see cref="UIView.Frame"/> * <paramref name="percent"/>)</returns>
        //internal float PercentOfTableFrameWidth(double percent)
        //{
        //    float value = (float)Math.Round( PercentOfRectangleWidth(_reportTableView.Frame, percent), 0);
        //    return value;
        //}

        ///// <summary>Calculates a value representing a <paramref name="percent"/> of the <paramref name="rectangle"/> value.
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
		string _class = "ReasonCodeTableSource.";
		string _method = "TBD";

        private readonly NewVisit_ViewModel _viewModel;
        private readonly NewVisit_TableViewSource _source;
        private const string CellIdentifier = "ReasonCodeTableCell";

        /// <summary>Store for the <c>ButtonHeight</c> property.</summary>
        private float _buttonHeight = 0f;
        public float ButtonHeight
        {
            get { return _buttonHeight; }
            set
            {
                _buttonHeight = value;
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
            }
        }

        public ReasonCodeTableSource(NewVisit_ViewModel viewModel, NewVisit_TableViewSource source)
        {
            _viewModel = viewModel;
            _source = source;

            ButtonHeight = source.RowHeight;
            RowHeight = source.RowHeight;

#if (DEBUG)
            // Note: in order to make to past the New Visit view, select the first reasonCode.
            if (Common_iOS.UserInterfaceIdiomIsPhone)
            {
                ReasonCode fakeSelectedReasonCode = _viewModel.ListOfReasonCodes[0];
                //if (_viewModel.SelectedReasonCodes.Contains(fakeSelectedReasonCode))
                //{
                //    _viewModel.SelectedReasonCodes.Remove(fakeSelectedReasonCode);
                //}
                //else
                //{
                    _viewModel.SelectedReasonCodes.Add(fakeSelectedReasonCode);
                //}
                _viewModel.RaisePropertyChanged(GetPropertyName(() => _viewModel.SelectedReasonCodes));
                //tableView.ReloadData();
            }
#endif
        }

        #region overrides
        #pragma warning disable 1591
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
            doneButton.Frame = new RectangleF(0, 0, tableView.Frame.Width, ButtonHeight);

            // Hack: hide Done button.
            ButtonHeight = 0f;
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
            float heightToReport = _source.RowHeight;

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
        #pragma warning restore 1591
        #endregion overrides

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
