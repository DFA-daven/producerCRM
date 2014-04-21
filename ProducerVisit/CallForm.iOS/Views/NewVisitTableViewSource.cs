namespace CallForm.iOS.Views
{
    using CallForm.Core.ViewModels;
    using CallForm.iOS.ViewElements;
    using MonoTouch.Foundation;
    using MonoTouch.UIKit;
    using System.Drawing;

    public class NewVisitTableViewSource : UITableViewSource
    {
        private readonly NewVisitViewModel _viewModel;
        private readonly TextFieldTableViewCell _farmNoCell, _durationCell;
        private readonly UITableViewCell _callTypeCell, _dateCell, _reasonCell, _emailRecipientsCell;
        private readonly ImageTableViewCell _takePictureCell;
        private readonly TextViewTableViewCell _notesCell;

        UIPopoverController _popover;

        public UIViewController DatePickerPopover;
        public UIViewController CallTypePickerPopover;
        public UIViewController ReasonPickerPopover;
        public UIViewController EmailPickerPopover;

        public NewVisitTableViewSource(NewVisitViewModel viewModel, UITableView tableView)
        {
            _viewModel = viewModel;
            _viewModel.PropertyChanged += (sender, args) =>
            {
                // Review: Is switch/case the best way to handle this? What if a property name changes?
                switch (args.PropertyName)
                {
                    case "MemberNumber":
                        _farmNoCell.SetText(_viewModel.MemberNumber);
                        break;
                    case "CallType":
                        _callTypeCell.DetailTextLabel.Text = _viewModel.CallType;
                        break;
                    case "Date":
                        _dateCell.DetailTextLabel.Text = _viewModel.Date.ToShortDateString();
                        break;
                    case "ReasonCodes":
                        _reasonCell.DetailTextLabel.Text = _viewModel.ReasonCodes != null &&
                                                           _viewModel.ReasonCodes.Count > 0
                            ? string.Join(", ", _viewModel.ReasonCodes)
                            : "Tap to Select";
                        break;
                    case "DurationString":
                        _durationCell.SetText(_viewModel.DurationString);
                        break;
                    case "Notes":
                        _notesCell.SetText(_viewModel.Notes);
                        break;
                    case "PictureBytes":
                        _takePictureCell.SetPicture(_viewModel.PictureBytes);
                        tableView.ReloadData();
                        break;
                    case "EmailRecipients":
                        _emailRecipientsCell.DetailTextLabel.Text = _viewModel.EmailRecipients == null
                            ? string.Empty
                            : string.Join(", ", _viewModel.EmailRecipients);
                        tableView.ReloadData();
                        break;
                }
            }; 

            _farmNoCell = new TextFieldTableViewCell("farmNo", _viewModel.Editing, _viewModel.MemberNumber,
                UIKeyboardType.NumberPad, (field, range, replacementString) =>
                {
                    int i;
                    return replacementString.Length <= 0 || int.TryParse(replacementString, out i);
                },
                (sender, args) =>
                {
                    _viewModel.MemberNumber = (sender as UITextField).Text;
                });
            _farmNoCell.TextLabel.Text = "Member Number";
            if (!_viewModel.Editing)
            {
                _farmNoCell.DetailTextLabel.Text = _viewModel.MemberNumber ?? string.Empty;

                // TODO: validate data (business logic - check for 8 digits now?, or wait until save is pressed?)
                if (_farmNoCell.DetailTextLabel.Text.Length != 8)
                {
                    _farmNoCell.DetailTextLabel.TextColor = UIColor.Red;
                }
                else
                {
                    _farmNoCell.DetailTextLabel.TextColor = UIColor.Black;
                }
            }

            _durationCell = new TextFieldTableViewCell("duration", _viewModel.Editing, _viewModel.DurationString,
                UIKeyboardType.DecimalPad, (field, range, replacementString) =>
                {
                    string newString = new NSString(field.Text).Replace(range, new NSString(replacementString));
                    decimal d;
                    if (decimal.TryParse(newString, out d))
                    {
                        if (d > 100)
                        {
                            return false;
                        }
                        if (newString.Contains(".") && newString.Length - newString.IndexOf('.') > 3)
                        {
                            return false;
                        }
                        return true;
                    }
                    return newString.Length <= 0;
                }, 
                (sender, args) =>
                {
                    _viewModel.DurationString = (sender as UITextField).Text;
                }
                );
            _durationCell.TextLabel.Text = "Length of Call (hours)";
            if (!_viewModel.Editing)
            {
                // TODO: validate data (make sure it's a number before attempting ToString) orig: ("#0.##")
                _durationCell.DetailTextLabel.Text = _viewModel.Duration.ToString("00.00");
                _durationCell.DetailTextLabel.TextColor = UIColor.Black;
            }

            _callTypeCell = new UITableViewCell(UITableViewCellStyle.Value1, "callType");
            _callTypeCell.TextLabel.Text = "Call Type";
            _callTypeCell.DetailTextLabel.Text = _viewModel.CallType;
            _callTypeCell.DetailTextLabel.TextColor = UIColor.Black;

            _dateCell = new UITableViewCell(UITableViewCellStyle.Value1, "date");
            _dateCell.TextLabel.Text = "Date";
            _dateCell.DetailTextLabel.Text = _viewModel.Date.ToShortDateString();
            _dateCell.DetailTextLabel.TextColor = UIColor.Black;

            _reasonCell = new UITableViewCell(UITableViewCellStyle.Value1, "reason");
            _reasonCell.TextLabel.Text = "Reason For Call";
            _reasonCell.DetailTextLabel.Text = _viewModel.ReasonCodes != null &&
                                               _viewModel.ReasonCodes.Count > 0
                ? string.Join(", ", _viewModel.ReasonCodes)
                : "Tap to Select";
            _reasonCell.DetailTextLabel.TextColor = UIColor.Black;

            _notesCell = new TextViewTableViewCell("notes", _viewModel.Editing, _viewModel.Notes,
                (sender, args) => _viewModel.Notes = (sender as UITextView).Text);
            _notesCell.TextLabel.Text = "Notes";

            _takePictureCell = new ImageTableViewCell("takePicture", _viewModel.PictureBytes, _viewModel.Editing, _viewModel);
            _takePictureCell.TextLabel.Text = "Take Picture";

            _emailRecipientsCell = new UITableViewCell(UITableViewCellStyle.Value1, "emailRecipients");
            _emailRecipientsCell.TextLabel.Text = "Email Recipients";
            _emailRecipientsCell.DetailTextLabel.Text = _viewModel.EmailRecipients == null
                ? string.Empty
                : string.Join(", ", _viewModel.EmailRecipients);
            _emailRecipientsCell.DetailTextLabel.TextColor = UIColor.Black;

        }

        public override int RowsInSection(UITableView tableview, int section)
        {
            return 8;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (_viewModel.Editing)
            {
                _farmNoCell.HideKeyboard();
                _durationCell.HideKeyboard();
                _notesCell.HideKeyboard();
                switch (indexPath.Row)
                {
                    case 0:
                        _farmNoCell.Edit();
                        break;
                    case 1:
                        // ToDo: seeing an un-handled exception here if debug on iPhoneSimulator
                        _popover = new UIPopoverController(CallTypePickerPopover);
                        // _popover.PopoverContentSize = CallTypePickerPopover.ContentSizeForViewInPopover;
                        _popover.PopoverContentSize = CallTypePickerPopover.PreferredContentSize;
                        _popover.PresentFromRect(tableView.RectForRowAtIndexPath(indexPath), tableView.Superview,
                            UIPopoverArrowDirection.Any, true);
                        break;
                    case 2:
                        _popover = new UIPopoverController(DatePickerPopover);
                        // _popover.PopoverContentSize = DatePickerPopover.ContentSizeForViewInPopover;
                        _popover.PopoverContentSize = DatePickerPopover.PreferredContentSize;
                        _popover.PresentFromRect(tableView.RectForRowAtIndexPath(indexPath), tableView.Superview,
                            UIPopoverArrowDirection.Any, true);
                        break;
                    case 3:
                        _durationCell.Edit();
                        break;
                    case 4:
                        _popover = new UIPopoverController(ReasonPickerPopover);
                        // _popover.PopoverContentSize = ReasonPickerPopover.ContentSizeForViewInPopover;
                        _popover.PopoverContentSize = ReasonPickerPopover.PreferredContentSize;
                        _popover.PresentFromRect(RectangleF.Empty, tableView.Superview,
                            UIPopoverArrowDirection.Any, true);
                        break;
                    case 5:
                        _notesCell.Edit();
                        break;
                    case 6:
                        _viewModel.TakePictureCommand.Execute(_viewModel);
                        break;
                    case 7:
                        _popover = new UIPopoverController(EmailPickerPopover);
                        // _popover.PopoverContentSize = EmailPickerPopover.ContentSizeForViewInPopover;
                        _popover.PopoverContentSize = EmailPickerPopover.PreferredContentSize;
                        var rect = tableView.RectForRowAtIndexPath(indexPath);
                        rect.Width = 0;
                        _popover.PresentFromRect(rect, tableView.Superview,
                            UIPopoverArrowDirection.Any, true);
                        break;
                }
            }
            tableView.DeselectRow(indexPath, true);
        }

        // ToDo: is this ever used?
        public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            if (indexPath.Row == 5)
            {
                return 110;
            }
            if (indexPath.Row == 6)
            {
                return 160;
            }
            return 50;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            switch (indexPath.Row)
            {
                case 0:
                    return _farmNoCell;
                case 1:
                    return _callTypeCell;
                case 2:
                    return _dateCell;
                case 3:
                    return _durationCell;
                case 4:
                    return _reasonCell;
                case 5:
                    return _notesCell;
                case 6:
                    return _takePictureCell;
                case 7:
                    return _emailRecipientsCell;
            }
            var rVal = new UITableViewCell(UITableViewCellStyle.Value1, string.Empty);
            rVal.TextLabel.Text = indexPath.Row.ToString();
            return rVal;
        }

        public void DismissPopover()
        {
            if (_popover != null && _popover.PopoverVisible)
            {
                _popover.Dismiss(true);
            }
        }
    }
}
