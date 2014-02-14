using System.Drawing;
using CallForm.Core.ViewModels;
using CallForm.iOS.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using XibFree;

namespace CallForm.iOS.ViewElements
{
    public class EmailRecipientSelectDialogViewController : UIViewController
    {
        private readonly UITableView _table;
        private readonly NewVisitViewModel _viewModel;

        public EmailRecipientSelectDialogViewController(NewVisitViewModel viewModel, NewVisitTableViewSource source)
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
            _viewModel.RaisePropertyChanged("EmailRecipients");
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
        private readonly NewVisitViewModel _viewModel;
        private readonly NewVisitTableViewSource _source;
        private const string CellIdentifier = "TableCell";


        public EmailRecipientsTableSource(NewVisitViewModel viewModel, NewVisitTableViewSource source)
        {
            _viewModel = viewModel;
            _source = source;
        }

        public override int RowsInSection(UITableView tableview, int section)
        {
            return _viewModel.BuiltinEmailRecipients.Count;
        }

        public override UIView GetViewForFooter(UITableView tableView, int section)
        {
            var doneButton = new UIButton(UIButtonType.System);
            doneButton.SetTitle("Done", UIControlState.Normal);
            // review: is InvokeOnMainThread() a bug fix by Ben?
            doneButton.TouchUpInside += (sender, args) => InvokeOnMainThread(_source.DismissPopover);
            doneButton.Frame = new RectangleF(0, 0, tableView.Frame.Width, 50);
            return doneButton;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            string email = _viewModel.BuiltinEmailRecipients[indexPath.Row];
            if (_viewModel.EmailRecipients.Contains(email))
            {
                _viewModel.EmailRecipients.Remove(email);
            }
            else
            {
                _viewModel.EmailRecipients.Add(email);
            }
            _viewModel.RaisePropertyChanged("EmailRecipients");
            tableView.DeselectRow(indexPath, true);
            tableView.ReloadData();
        }

        public override float GetHeightForFooter(UITableView tableView, int section)
        {
            return 50;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            UITableViewCell cell = tableView.DequeueReusableCell(CellIdentifier) ??
                                   new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier);
            string email = _viewModel.BuiltinEmailRecipients[indexPath.Row];
            cell.TextLabel.Text = email;
            cell.Accessory = _viewModel.EmailRecipients.Contains(email) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
            return cell;
        }
    }
}
