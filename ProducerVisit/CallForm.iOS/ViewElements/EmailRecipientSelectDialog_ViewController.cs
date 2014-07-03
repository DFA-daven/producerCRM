// CallForm.iOS\ViewElements\EmailRecipientSelectDialog_ViewController.cs

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
    /// selecting Email Recipients.
    /// </summary>
    public class EmailRecipientSelectDialog_ViewController : UIViewController
    {
        private readonly UITableView _table;
        private readonly NewVisit_ViewModel _viewModel;

        /// <summary>Creates an instance of the <see cref="EmailRecipientSelectDialog_ViewController"/> class.
        /// </summary>
        /// <param name="viewModel">The parent <see cref="MvxViewModel"/>.</param>
        /// <param name="source">The parent <see cref="UITableViewSource"/>.</param>
        public EmailRecipientSelectDialog_ViewController(NewVisit_ViewModel viewModel, NewVisit_TableViewSource source)
        {
            //View.BackgroundColor = UIColor.Green;
            _viewModel = viewModel;
            _table = new UITableView();
            _table.Source = new EmailRecipientsTableSource(_viewModel, source);
            //_table.BackgroundColor = UIColor.LightGray;
            //_table.Alpha = 0.5f;
            _table.AutoresizingMask = UIViewAutoresizing.FlexibleBottomMargin | UIViewAutoresizing.FlexibleRightMargin;

            float maxTableHeight = (float)Math.Round(UIScreen.MainScreen.Bounds.Height * 0.5, 0);  // the Y value
            float maxTableWidth = (float)Math.Round(UIScreen.MainScreen.Bounds.Width * 0.5, 0);    // the X value

            // Note: offset here is displayed as whitespace between the NW corner of the popover and the NW corner of the content.
            _table.Frame = new RectangleF(0, 0, maxTableWidth, maxTableHeight);

            _table.ScrollEnabled = true;

            View.Add(_table);
            View.SizeToFit();

        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            _viewModel.RaisePropertyChanged(GetPropertyName(() => _viewModel.SelectedEmailRecipients));
        }

        
        public override SizeF PreferredContentSize
        {
            get
            {
                SizeF size = _table.Frame.Size;
                //// leave space for "Done" button
                //size.Height += 50;
                //size.Height = (float)Math.Round(UIScreen.MainScreen.Bounds.Height * 0.5, 0);

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
    }

    public class EmailRecipientsTableSource : UITableViewSource
    {
        private readonly NewVisit_ViewModel _viewModel;
        private readonly NewVisit_TableViewSource _source;
        private const string CellIdentifier = "TableCell";


        public EmailRecipientsTableSource(NewVisit_ViewModel viewModel, NewVisit_TableViewSource source)
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
            return _viewModel.ListOfEmailDisplayNames.Count;
        }

        public override UIView GetViewForFooter(UITableView tableView, int section)
        {
            var doneButton = new UIButton(UIButtonType.System);
            // Hack: hide Done button

            //doneButton.SetTitle("Done", UIControlState.Normal);
            //// review: is InvokeOnMainThread() correct?
            //doneButton.TouchUpInside += (sender, args) => { InvokeOnMainThread(_source.DismissPopover); };
            //doneButton.Frame = new RectangleF(0, 0, tableView.Frame.Width, 50);

            //doneButton.Hidden = true;

            return doneButton;
        }

        /// <summary>Toggles selected email recipients.
        /// </summary>
        /// <param name="tableView">The <see cref="UITableView"/>/control that contains the selected row.</param>
        /// <param name="indexPath">The <see cref="NSIndexPath"/> of the selected row in the control.</param>
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            string currentlySelectedRow = _viewModel.ListOfEmailDisplayNames[indexPath.Row];
            if (_viewModel.SelectedEmailRecipients.Contains(currentlySelectedRow))
            {
                _viewModel.SelectedEmailRecipients.Remove(currentlySelectedRow);
            }
            else
            {
                _viewModel.SelectedEmailRecipients.Add(currentlySelectedRow);
            }

            _viewModel.RaisePropertyChanged(GetPropertyName(() => _viewModel.SelectedEmailRecipients));
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
            string email = _viewModel.ListOfEmailDisplayNames[indexPath.Row];
            cell.TextLabel.Text = email;
            cell.Accessory = _viewModel.SelectedEmailRecipients.Contains(email) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
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
