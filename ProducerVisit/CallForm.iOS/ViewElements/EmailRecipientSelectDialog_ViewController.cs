using System.Drawing;
using CallForm.Core.ViewModels;
using CallForm.iOS.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using XibFree;

namespace CallForm.iOS.ViewElements
{
    public class EmailRecipientSelectDialog_ViewController : UIViewController
    {
        private readonly UITableView _table;
        private readonly NewVisit_ViewModel _viewModel;

        public EmailRecipientSelectDialog_ViewController(NewVisit_ViewModel viewModel, NewVisit_TableViewSource source)
        {
            View.BackgroundColor = UIColor.White;
            _viewModel = viewModel;
            _table = new UITableView(new RectangleF(0,0,500, 700));
            _table.Source = new EmailRecipientsTableSource(_viewModel, source);
            View.Add(_table);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            _viewModel.RaisePropertyChanged("pvrEmailRecipients");
        }

        // replace ContentSizeForViewInPopover with PreferredContentSize
        public override SizeF PreferredContentSize
        {
            get { return _table.Frame.Size; }
            // replace ContentSizeForViewInPopover with PreferredContentSize
            set { base.PreferredContentSize = value; }
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

        public override int RowsInSection(UITableView tableview, int section)
        {
            return _viewModel.ListOfEmailRecipients.Count;
        }

        public override UIView GetViewForFooter(UITableView tableView, int section)
        {
            var doneButton = new UIButton(UIButtonType.System);
            doneButton.SetTitle("Done", UIControlState.Normal);
            // review: is InvokeOnMainThread() a bug fix by Ben?
            doneButton.TouchUpInside += (sender, args) => { InvokeOnMainThread(_source.DismissPopover); };
            doneButton.Frame = new RectangleF(0, 0, tableView.Frame.Width, 50);
            return doneButton;
        }

        /// <summary>Toggles selected email recipients.
        /// </summary>
        /// <param name="tableView">The <see cref="UITableView"/>/control that contains the selected row.</param>
        /// <param name="indexPath">The <see cref="NSIndexPath"/> of the selected row in the control.</param>
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            string currentlySelectedRow = _viewModel.ListOfEmailRecipients[indexPath.Row];
            if (_viewModel.SelectedEmailRecipients.Contains(currentlySelectedRow))
            {
                _viewModel.SelectedEmailRecipients.Remove(currentlySelectedRow);
            }
            else
            {
                _viewModel.SelectedEmailRecipients.Add(currentlySelectedRow);
            }
            _viewModel.RaisePropertyChanged("SelectedEmailRecipients");
            tableView.DeselectRow(indexPath, true);
            tableView.ReloadData();
        }

        // ToDo: fixed value
        public override float GetHeightForFooter(UITableView tableView, int section)
        {
            return 50;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell(CellIdentifier) ??
                                   new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier);
            string email = _viewModel.ListOfEmailRecipients[indexPath.Row];
            cell.TextLabel.Text = email;
            cell.Accessory = _viewModel.SelectedEmailRecipients.Contains(email) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
            return cell;
        }
    }
}
