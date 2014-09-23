// CallForm.iOS\ViewElements\EmailRecipientSelectDialog_ViewController.cs

namespace CallForm.iOS.ViewElements
{
    using CallForm.Core.Models;
    using CallForm.Core.ViewModels;
    using CallForm.iOS.Views;
    using CallForm.iOS.Views;
    using MonoTouch.Foundation;
    using MonoTouch.MessageUI;
    using MonoTouch.UIKit;
    using System;
    using System.Drawing;
    using System.Linq.Expressions;
    using System.Reflection;
    using XibFree;

    /// <summary>The class that defines View Element (control) for displaying and 
    /// selecting Email Recipients.
    /// </summary>
    public class EmailRecipientSelectDialog_ViewController : UIViewController
    {
        string _nameSpace = "CallForm.iOS.";

        private readonly UITableView _table;
        private readonly NewVisit_ViewModel _viewModel;

        /// <summary>Creates an instance of the <see cref="EmailRecipientSelectDialog_ViewController"/> class.
        /// This holds the "content" inside the _popoverController.
        /// </summary>
        /// <param name="viewModel">The parent <see cref="MvxViewModel"/>.</param>
        /// <param name="source">The parent <see cref="UITableViewSource"/>.</param>
        /// <remarks>This ViewController is created when NewVisit_View is loaded.</remarks> 
        public EmailRecipientSelectDialog_ViewController(NewVisit_ViewModel viewModel, NewVisit_TableViewSource source)
        {
            //View.BackgroundColor = UIColor.Green;
            _viewModel = viewModel;
            _table = new UITableView();
            _table.Source = new EmailRecipientsTableSource(_viewModel, source);
            //_reportTableView.BackgroundColor = UIColor.LightGray;
            //_reportTableView.Alpha = 0.5f;
            _table.AutoresizingMask = UIViewAutoresizing.FlexibleBottomMargin | UIViewAutoresizing.FlexibleRightMargin;
            
            // Review: would _table.EstimatedRowHeight be a better value?
            float maxTableHeight = (float)Math.Round(UIScreen.MainScreen.Bounds.Height * 0.5, 0);  // the Y value
            float maxTableWidth = (float)Math.Round(UIScreen.MainScreen.Bounds.Width * 0.5, 0);    // the X value

            Common_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            Common_iOS.DebugMessage("  [ersd_vc][ersd_vc] > maxTableHeight = " + maxTableHeight.ToString() + ", maxTableWidth = " + maxTableWidth.ToString());

            // Note: offset here is displayed as whitespace between the NW corner of the popover and the NW corner of the content.
            _table.Frame = new RectangleF(0, 0, maxTableWidth, maxTableHeight);

            _table.ScrollEnabled = true; // disabling locks the rows in the _popoverController

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
            _viewModel.RaisePropertyChanged(GetPropertyName(() => _viewModel.SelectedEmailAddresses));
            _viewModel.RaisePropertyChanged(GetPropertyName(() => _viewModel.SelectedEmailDisplayNames));
        }

        public override SizeF PreferredContentSize
        {
            get
            {
                SizeF size = _table.Frame.Size;
                //// leave space for "Done" button
                //size.Height += 50;
                //size.Height = (float)Math.Round(UIScreen.MainScreen.Bounds.Height * 0.5, 0);

                Common_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
                Common_iOS.DebugMessage("  [ersd_vc][pcs] > PreferredContentSize Height = " + size.Height.ToString() + ", Width = " + size.Width.ToString());

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

        public void PopoverFinished(object sender, MFComposeResultEventArgs mfComposeResultEventArgs)
        {
            InvokeOnMainThread(() => { DismissViewController(true, null); } );
        }
    }

    public class EmailRecipientsTableSource : UITableViewSource
    {
        string _nameSpace = "CallForm.iOS.";

        private readonly NewVisit_ViewModel _viewModel;
        private readonly NewVisit_TableViewSource _source;
        private const string CellIdentifier = "EmailTableCell";
        private float _doneButtonHeight = 50f;

        public EmailRecipientsTableSource(NewVisit_ViewModel viewModel, NewVisit_TableViewSource source)
        {
            _viewModel = viewModel;
            _source = source;
        }

        /// <summary>The number of rows (cells) in this section of <see cref="EmailRecipientsTableSource"/>.
        /// </summary>
        /// <param name="tableView">The <see cref="UITableView"/>/control that contains the section.</param>
        /// <param name="section">The index number of the section that contains the rows (cells).</param>
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

            doneButton.SetTitle("Done", UIControlState.Normal);
            // review: is InvokeOnMainThread() correct?
            doneButton.TouchUpInside += (sender, args) => { InvokeOnMainThread(_source.SafeDismissPopover); };
            //doneButton.TouchUpInside += (sender, args) => { Invoke(_source.SafeDismissPopover, 0); };
            //doneButton.TouchUpInside += (sender, args) => { InvokeOnMainThread(() => {Dism})

            doneButton.Frame = new RectangleF(0, 0, tableView.Frame.Width, _doneButtonHeight);

            // Hack: hide Done button.
            _doneButtonHeight = 0f;
            doneButton.Hidden = true;

            return doneButton;
        }

        /// <summary>Toggles the selected row (cell) in <see cref="EmailRecipientsTableSource"/>.
        /// </summary>
        /// <param name="tableView">The <see cref="UITableView"/>/control that contains the selected row.</param>
        /// <param name="indexPath">The <see cref="NSIndexPath"/> of the selected row in the control.</param>
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            // FixMe: SelectedEmailAddresses
            string currentlySelectedAddress = _viewModel.ListOfEmailAddresses[indexPath.Row];
            string currentlySelectedDisplayName = _viewModel.ListOfEmailDisplayNames[indexPath.Row];

            if (_viewModel.SelectedEmailDisplayNames.Contains(currentlySelectedDisplayName))
            {
                _viewModel.SelectedEmailAddresses.Remove(currentlySelectedAddress);
                _viewModel.SelectedEmailDisplayNames.Remove(currentlySelectedDisplayName);
            }
            else
            {
                _viewModel.SelectedEmailAddresses.Add(currentlySelectedAddress);
                _viewModel.SelectedEmailDisplayNames.Add(currentlySelectedDisplayName);
            }

            _viewModel.RaisePropertyChanged(GetPropertyName(() => _viewModel.SelectedEmailAddresses));
            _viewModel.RaisePropertyChanged(GetPropertyName(() => _viewModel.SelectedEmailDisplayNames));
            tableView.DeselectRow(indexPath, true);
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
        /// <returns>The requested <see cref="UITableViewCell" /> from the <see cref="EmailRecipientsTableSource"/>.</returns>
        /// <remarks>This method is strictly viewable information -- it doesn't change any content.</remarks>
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            // request a recycled cell to save memory; if there are no cells to reuse, create a new one
            UITableViewCell cell = tableView.DequeueReusableCell(CellIdentifier) ??
                                   new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier);
            string selectedDisplayName = _viewModel.ListOfEmailDisplayNames[indexPath.Row];
            cell.TextLabel.Text = selectedDisplayName;
            cell.Accessory = _viewModel.SelectedEmailDisplayNames.Contains(selectedDisplayName) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
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
