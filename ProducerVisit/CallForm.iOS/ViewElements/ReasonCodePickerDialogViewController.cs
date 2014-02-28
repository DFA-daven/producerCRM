namespace CallForm.iOS.ViewElements
{
    using CallForm.Core.Models;
    using CallForm.Core.ViewModels;
    using CallForm.iOS.Views;
    using MonoTouch.Foundation;
    using MonoTouch.UIKit;
    using System.Drawing;

    public class ReasonCodePickerDialogViewController : UIViewController
    {
        private readonly UITableView _table;
        private readonly NewVisitViewModel _viewModel;

        // todo: replace fixed values
        public ReasonCodePickerDialogViewController(NewVisitViewModel viewModel, NewVisitTableViewSource source)
        {
            View.BackgroundColor = UIColor.White;
            _viewModel = viewModel;
            _table = new UITableView(new RectangleF(0,0,500, 700));
            _table.Source = new ReasonCodeTableSource(_viewModel, source);
            View.Add(_table);
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            _viewModel.RaisePropertyChanged("ReasonCodes");
        }

        public override SizeF PreferredContentSize
        {
            get { return _table.Frame.Size; }
            set { base.PreferredContentSize = value; }
        }
    }

    public class ReasonCodeTableSource : UITableViewSource
    {
        private readonly NewVisitViewModel _viewModel;
        private readonly NewVisitTableViewSource _source;
        private const string CellIdentifier = "TableCell";


        public ReasonCodeTableSource(NewVisitViewModel viewModel, NewVisitTableViewSource source)
        {
            _viewModel = viewModel;
            _source = source;
        }

        public override int RowsInSection(UITableView tableview, int section)
        {
            return _viewModel.BuiltInReasonCodes.Count;
        }

        public override UIView GetViewForFooter(UITableView tableView, int section)
        {
            var doneButton = new UIButton(UIButtonType.System);
            doneButton.SetTitle("Done", UIControlState.Normal);
            // Review: is InvokeOnMainThread() a bug fix by Ben?
            doneButton.TouchUpInside += (sender, args) => InvokeOnMainThread(_source.DismissPopover);
            doneButton.Frame = new RectangleF(0, 0, tableView.Frame.Width, 50);
            return doneButton;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            ReasonCode reasonCode = _viewModel.BuiltInReasonCodes[indexPath.Row];
            if (_viewModel.ReasonCodes.Contains(reasonCode))
            {
                _viewModel.ReasonCodes.Remove(reasonCode);
            }
            else
            {
                _viewModel.ReasonCodes.Add(reasonCode);
            }
            _viewModel.RaisePropertyChanged("ReasonCodes");
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
            ReasonCode reasonCode = _viewModel.BuiltInReasonCodes[indexPath.Row];
            cell.TextLabel.Text = reasonCode.Name;
            cell.Accessory = _viewModel.ReasonCodes.Contains(reasonCode) ? UITableViewCellAccessory.Checkmark : UITableViewCellAccessory.None;
            return cell;
        }
    }
}
