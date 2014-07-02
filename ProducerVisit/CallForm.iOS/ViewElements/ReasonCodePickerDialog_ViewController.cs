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
    using XibFree;

    /// <summary>The class that defines View Element (control) for displaying and 
    /// selecting Reason Codes.
    /// </summary>
    public class ReasonCodePickerDialog_ViewController : UIViewController
    {
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
            //View.BackgroundColor = UIColor.Blue;
            _viewModel = viewModel;
            _table = new UITableView();
            _table.Source = new ReasonCodeTableSource(_viewModel, source);

            // Note: using cell height won't work -- the cell's don't exist yet
            //int sectionNumber = 0;
            //int count = _table.Source.RowsInSection(_table, sectionNumber);
            //UITableViewCell aCell = _table.VisibleCells[0];
            //float aCellHeight = aCell.Frame.Height;
            //float preferredHeight = count * aCellHeight;

            // 75% of the Height, rounded off to zero decimal places
            float reasonCodeHeight = (float)Math.Round(UIScreen.MainScreen.Bounds.Height * 0.75, 0);  // the Y value
            float reasonCodeWidth = (float)Math.Round(UIScreen.MainScreen.Bounds.Width * 0.75, 0);    // the X value

            // Note: offset here is displayed as whitespace between the NW corner of the popover and the NW corner of the content.
            _table.Frame = new RectangleF(0, 0, reasonCodeWidth, reasonCodeHeight);

            _table.ScrollEnabled = true;

            View.Add(_table);
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
                SizeF size = _table.Frame.Size;
                // leave space for "Done" button
                //size.Height += 50;
                size.Height = (float)Math.Round(UIScreen.MainScreen.Bounds.Height * 0.75, 0);

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
        //    float value = (float)Math.Round( PercentOfRectangleHeight(_table.Frame, percent), 0);
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
        //    float value = (float)Math.Round( PercentOfRectangleWidth(_table.Frame, percent), 0);
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
        private readonly NewVisit_ViewModel _viewModel;
        private readonly NewVisit_TableViewSource _source;
        private const string CellIdentifier = "ReasonCodeTableCell";


        public ReasonCodeTableSource(NewVisit_ViewModel viewModel, NewVisit_TableViewSource source)
        {
            _viewModel = viewModel;
            _source = source;
        }

        /// <summary>The number of rows to be displayed.
        /// </summary>
        /// <param name="tableview"></param>
        /// <param name="section"></param>
        /// <remarks><paramref name="section"/> is included as part of the override -- it is not used in this method.</remarks>
        /// <returns>A row count.</returns>
        public override int RowsInSection(UITableView tableview, int section)
        {
            return _viewModel.ListOfReasonCodes.Count;
        }

        public override UIView GetViewForFooter(UITableView tableView, int section)
        {
            var doneButton = new UIButton(UIButtonType.System);
            doneButton.SetTitle("Done", UIControlState.Normal);
            // review: is InvokeOnMainThread() correct?
            doneButton.TouchUpInside += (sender, args) => { InvokeOnMainThread(_source.DismissPopover); };
            doneButton.Frame = new RectangleF(0, 0, tableView.Frame.Width, 50);
            //doneButton.Frame = new RectangleF(0, 0, tableView.Frame.Width, GetHeightForFooter(tableView, section));

            // Hack: hide Done button
            doneButton = new UIButton(UIButtonType.System);

            return doneButton;
        }

        /// <summary>Toggles selected visit reasons.
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
            tableView.ReloadData();
        }

        public override float GetHeightForFooter(UITableView tableView, int section)
        {
            // Hack: hide Done button.
            //return 50f;
            return 5f;
        }

        /// <summary>Find the currently selected cell (row) in the <see cref="UITableView"/>.
        /// </summary>
        /// <param name="tableView">The active <see cref="UITableView"/>.</param>
        /// <param name="indexPath">The <see cref="NSIndexPath"/> with the selected row (cell).</param>
        /// <returns></returns>
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
