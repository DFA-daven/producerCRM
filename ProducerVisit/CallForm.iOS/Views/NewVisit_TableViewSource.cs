namespace CallForm.iOS.Views
{
    using CallForm.Core.ViewModels;
    using CallForm.iOS.ViewElements;
    using MonoTouch.Foundation;
    using MonoTouch.UIKit;
    using System;
    using System.Drawing;
    using System.Linq.Expressions;

    public class NewVisit_TableViewSource : UITableViewSource
    {
        #region Properties
        private readonly NewVisit_ViewModel _viewModel;
        private readonly TextField_TableViewCell _memberNumberCell, _durationCell;
        private readonly UITableViewCell _callTypeCell, _dateCell, _reasonCell, _emailRecipientsCell;
        private readonly Image_TableViewCell _takePictureCell;
        private readonly TextView_TableViewCell _notesCell;

        /// <summary>The popover controller.
        /// </summary>
        UIPopoverController _popover;
        #endregion

        /// <summary>A container for the popover controller.
        /// </summary>
        public UIViewController DatePickerPopover, CallTypePickerPopover, ReasonPickerPopover, EmailPickerPopover;

        public NewVisit_TableViewSource(NewVisit_ViewModel viewModel, UITableView tableView)
        {
            _viewModel = viewModel; 
            _viewModel.PropertyChanged += (sender, args) =>
            {
                // Review: Is switch/case the best way to handle this? What if a property name changes?
                // Note: these are just arbitrary number in order to organize the switch/case -- they do not relate to the view/subview element.
                int switchNumber = -1;

                if (args.PropertyName == GetPropertyName(() => _viewModel.MemberNumber))
                    { switchNumber = 0; }
                else if (args.PropertyName == GetPropertyName(() => _viewModel.SelectedCallType))
                    { switchNumber = 1; }
                else if (args.PropertyName == GetPropertyName(() => _viewModel.Date))
                    { switchNumber = 2; }
                else if (args.PropertyName == GetPropertyName(() => _viewModel.SelectedReasonCodes))
                    { switchNumber = 3; }
                else if (args.PropertyName == GetPropertyName(() => _viewModel.DurationString))
                    { switchNumber = 4; }
                else if (args.PropertyName == GetPropertyName(() => _viewModel.Notes))
                    { switchNumber = 5; }
                else if (args.PropertyName == GetPropertyName(() => _viewModel.PictureBytes))
                    { switchNumber = 6; }
                else if (args.PropertyName == GetPropertyName(() => _viewModel.SelectedEmailAddresses))
                    { switchNumber = 7; }
                else if (args.PropertyName == GetPropertyName(() => _viewModel.SelectedEmailDisplayNames))
                    { switchNumber = 7; }


                switch (switchNumber)
                {
                    case 0:
                        _memberNumberCell.SetText(_viewModel.MemberNumber);
                        break;
                    case 1:
                        _callTypeCell.DetailTextLabel.Text = _viewModel.SelectedCallType;
                        break;
                    case 2:
                        _dateCell.DetailTextLabel.Text = _viewModel.Date.ToShortDateString();
                        break;
                    case 3:
                        _reasonCell.DetailTextLabel.Text = _viewModel.SelectedReasonCodes != null &&
                                                           _viewModel.SelectedReasonCodes.Count > 0
                            ? string.Join(", ", _viewModel.SelectedReasonCodes)
                            : "Tap to Select";
                        break;
                    case 4:
                        _durationCell.SetText(_viewModel.DurationString);
                        break;
                    case 5:
                        _notesCell.SetText(_viewModel.Notes);
                        break;
                    case 6:
                        _takePictureCell.SetPicture(_viewModel.PictureBytes);
                        tableView.ReloadData();
                        break;
                    case 7:
                        _emailRecipientsCell.DetailTextLabel.Text = _viewModel.SelectedEmailAddresses == null
                            ? string.Empty
                            : string.Join(", ", _viewModel.SelectedEmailAddresses);
                        tableView.ReloadData();
                        break;
                }
            };

            #region memberNumber
            _memberNumberCell = new TextField_TableViewCell("memberNumber", _viewModel.Editing, _viewModel.MemberNumber,
                UIKeyboardType.NumberPad, (field, range, replacementString) =>
                {
                    int i;
                    return replacementString.Length <= 0 || int.TryParse(replacementString, out i);
                },
                (sender, args) =>
                {
                    _viewModel.MemberNumber = (sender as UITextField).Text;
                });

            /*
             * ToDo: indent the cells (rows)
             * cell.SeparatorInset = new UIEdgeInsets(0, 50, 0, 0);
             */
            _memberNumberCell.TextLabel.Text = "Member Number";

            if (!_viewModel.Editing)
            {
                _memberNumberCell.DetailTextLabel.Text = _viewModel.MemberNumber ?? string.Empty;

                // TODO: validate data (business logic - check for 8 digits now?, or wait until save is pressed?)
                if (_memberNumberCell.DetailTextLabel.Text.Length != 8)
                {
                    _memberNumberCell.DetailTextLabel.TextColor = UIColor.Red;
                }
                else
                {
                    _memberNumberCell.DetailTextLabel.TextColor = UIColor.Black;
                }
            }
            #endregion

            #region duration
            _durationCell = new TextField_TableViewCell("duration", _viewModel.Editing, _viewModel.DurationString,
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
            #endregion

            #region callType
            _callTypeCell = new UITableViewCell(UITableViewCellStyle.Value1, "callType");
            _callTypeCell.TextLabel.Text = "Call Type";
            _callTypeCell.DetailTextLabel.Text = _viewModel.SelectedCallType;
            _callTypeCell.DetailTextLabel.TextColor = UIColor.Black;
            #endregion

            #region date
            _dateCell = new UITableViewCell(UITableViewCellStyle.Value1, "date");
            _dateCell.TextLabel.Text = "Date";
            _dateCell.DetailTextLabel.Text = _viewModel.Date.ToShortDateString();
            _dateCell.DetailTextLabel.TextColor = UIColor.Black;
            #endregion

            #region reason
            _reasonCell = new UITableViewCell(UITableViewCellStyle.Value1, "reason");
            _reasonCell.TextLabel.Text = "Reason For Call";
            _reasonCell.DetailTextLabel.Text = _viewModel.SelectedReasonCodes != null &&
                                               _viewModel.SelectedReasonCodes.Count > 0
                ? string.Join(", ", _viewModel.SelectedReasonCodes)
                : "Tap to Select";
            _reasonCell.DetailTextLabel.TextColor = UIColor.Black;
            #endregion

            #region notes
            _notesCell = new TextView_TableViewCell("notes", _viewModel.Editing, _viewModel.Notes,
                (sender, args) => { _viewModel.Notes = (sender as UITextView).Text; } );
            _notesCell.TextLabel.Text = "Notes";
            #endregion

            #region takePicture
            _takePictureCell = new Image_TableViewCell("takePicture", _viewModel.PictureBytes, _viewModel.Editing, _viewModel);
            _takePictureCell.TextLabel.Text = "Take Picture";
            #endregion

            #region emailRecipients
            _emailRecipientsCell = new UITableViewCell(UITableViewCellStyle.Value1, "emailRecipients");
            _emailRecipientsCell.TextLabel.Text = "Email Recipients";
            _emailRecipientsCell.DetailTextLabel.Text = _viewModel.SelectedEmailAddresses == null
                ? string.Empty
                : string.Join(", ", _viewModel.SelectedEmailAddresses);
            _emailRecipientsCell.DetailTextLabel.TextColor = UIColor.Black;
            #endregion
        }

        #region Overrides
        /// <summary>Gets the height (in points) for a given row (cell).
        /// </summary>
        /// <param name="tableView">The <see cref="UITableView">(view) table</see> that contains the cell.</param>
        /// <param name="indexPath">The <see cref="NSIndexPath"/> to the selected row (cell).</param>
        /// <returns></returns>
        public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            // FixMe: hard-coded values
            // the _noteCell
            if (indexPath.Row == 5)
            {
                return 110;
            }

            // the _takePictureCell
            if (indexPath.Row == 6)
            {
                return 160;
            }

            // otherwise...
            return 50;
        }

        /// <summary>The number of rows (cells) in this section of <see cref="NewVisit_TableViewSource"/>.
        /// </summary>
        /// <param name="tableView">The <see cref="UITableView"/>/control that contains the section.</param>
        /// <param name="section">The index number of the section that contains the rows (cells).</param>
        /// <remarks><paramref name="section"/> is included as part of the override -- it is not used in this method.</remarks>
        /// <returns>A row count.</returns>
        public override int RowsInSection(UITableView tableview, int section)
        {
            // Review: find a way to get this automatically
            // return _viewModel.Reports == null ? 0 : _viewModel.Reports.Count;
            return 8;
        }

        /// <summary>Handles the selected row (cell) in <see cref="NewVisit_TableViewSource"/>.
        /// </summary>
        /// <param name="tableView">The <see cref="UITableView">(view) table</see> that contains the cell.</param>
        /// <param name="indexPath">The <see cref="NSIndexPath"/> to the selected row (cell).</param>
        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            if (_viewModel.Editing)
            {
                _memberNumberCell.HideKeyboard();
                _durationCell.HideKeyboard();
                _notesCell.HideKeyboard();

                // *********************************************************************************************
                // *********************************************************************************************

                // Review: the section numbers are off, maybe by one? Need to find where they are called and add 1

                // *********************************************************************************************
                // *********************************************************************************************

                // Note: the Row # here matches the position of the row in the UITableView.
                switch (indexPath.Row)
                {
                    case 0:
                        _memberNumberCell.Edit();
                        break;
                    case 1:
                        // ToDo: seeing an un-handled exception here if debug on iPhoneSimulator
                        _popover = new UIPopoverController(CallTypePickerPopover);
                        _popover.PopoverContentSize = CallTypePickerPopover.PreferredContentSize;
                        //_popover.PresentFromRect(tableView.RectForRowAtIndexPath(indexPath ), tableView.Superview, UIPopoverArrowDirection.Any, true);
                        _popover.PresentFromRect(GetPresentationRectangleForCell(tableView, _callTypeCell), tableView.Superview, UIPopoverArrowDirection.Any, true);
                        break;
                    case 2:
                        _popover = new UIPopoverController(DatePickerPopover);
                        _popover.PopoverContentSize = DatePickerPopover.PreferredContentSize;
                        _popover.PresentFromRect(GetPresentationRectangleForCell(tableView, _dateCell), tableView.Superview, UIPopoverArrowDirection.Any, true);
                        break;
                    case 3:
                        _durationCell.Edit();
                        break;
                    case 4:
                        _popover = new UIPopoverController(ReasonPickerPopover);
                        _popover.PopoverContentSize = ReasonPickerPopover.PreferredContentSize;

                        //_popover.use empty? ToString get iPhone Working?
                        // using RectangleF.Empty sets the popover origin to the NW corner
                        //_popover.PresentFromRect(RectangleF.Empty, tableView.Superview, UIPopoverArrowDirection.Any, true);

                        // using GetRectangle() allows the popover to point to a specific cell
                        //_popover.PresentFromRect(GetRectangle(tableView, _reasonCell), tableView.Superview, UIPopoverArrowDirection.Any, true);

                        var reasonRect = tableView.RectForRowAtIndexPath(indexPath);
                        _popover.PresentFromRect(GetPresentationRectangleForCell(tableView, _reasonCell), tableView.Superview, UIPopoverArrowDirection.Any, true);
                        break;
                    case 5:
                        _notesCell.Edit();
                        break;
                    case 6:
                        _viewModel.TakePictureCommand.Execute(_viewModel);
                        break;
                    case 7:
                        _popover = new UIPopoverController(EmailPickerPopover);
                        _popover.PopoverContentSize = EmailPickerPopover.PreferredContentSize;
                        //var emailRect = tableView.RectForRowAtIndexPath(indexPath);
                        //_popover.PresentFromRect(emailRect, tableView.Superview, UIPopoverArrowDirection.Any, true);
                        _popover.PresentFromRect(GetPresentationRectangleForCell(tableView, _emailRecipientsCell), tableView.Superview, UIPopoverArrowDirection.Any, true);

                        break;
                }
            }

            tableView.DeselectRow(indexPath, true);
        }

        /// <summary>Gets a cell based on the selected <see cref="NSIndexPath">Row</see>.
        /// </summary>
        /// <param name="tableView">The <see cref="UITableView">(view) table</see> that contains the cell.</param>
        /// <param name="indexPath">The <see cref="NSIndexPath"/> to the selected row (cell).</param>
        /// <returns>The requested cell (of type <see cref="Image_TableViewCell"/>, <see cref="TextField_TableViewCell"/>, 
        /// <see cref="TextView_TableViewCell"/>, or <see cref="UITableViewCell"/>) 
        /// from the <see cref="NewVisit_TableViewSource"/>.</returns>
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            switch (indexPath.Row)
            {
                case 0:
                    return _memberNumberCell;
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
        #endregion

        /// <summary>Close the <c>_popover</c>.
        /// </summary>
        public void DismissPopover()
        {
            if (_popover != null && _popover.PopoverVisible)
            {
                _popover.Dismiss(true);
            }
        }

        /// <summary>Get the rectangle bounding a specific cell.
        /// </summary>
        /// <param name="tableView">The <see cref="UITableView">(view) table</see> that contains the cell.</param>
        /// <param name="targetCell">A <see cref="UITableViewCell">cell</see> (ie: row) in the <paramref name="tableView"/></param>
        /// <returns>A rectangle bounding the given cell.</returns>
        internal RectangleF GetRectangleForCell(UITableView tableView, UITableViewCell targetCell)
        {
            return tableView.RectForRowAtIndexPath(tableView.IndexPathForCell(targetCell));
        }

        /// <summary>Creates a new rectangle which matches the on-screen location of the given cell.
        /// </summary>
        /// <param name="tableView">The <see cref="UITableView">(view) table</see> that contains the cell.</param>
        /// <param name="targetCell">A <see cref="UITableViewCell">cell</see> (ie: row) in the <paramref name="tableView"/></param>
        /// <returns>A rectangle offset vertically.</returns>
        /// <remarks>Because of the indexing used to identify the cells, the boundary of any given cell appears to match the 
        /// previous cell in the <see cref="UITableView">table view</see>. There is no direct method for 'adding one' to the 
        /// cell. Instead, this method shifts the cell boundary down by the height of the boundary. The resulting location overlays
        /// the on-screen location of the cell contents.</remarks>
        internal RectangleF GetPresentationRectangleForCell(UITableView tableView, UITableViewCell targetCell)
        {
            RectangleF cellBoundary = tableView.RectForRowAtIndexPath(tableView.IndexPathForCell(targetCell));

            // shift origin to left margin, and to the bottom of the cell
            PointF presentationLocation = new PointF(0f, cellBoundary.Location.Y + cellBoundary.Height);
            presentationLocation.Y = (float)Math.Round( presentationLocation.Y, 0);
            presentationLocation.X = (float)Math.Round(presentationLocation.X, 0);

            RectangleF presentationBoundary = new RectangleF(presentationLocation, cellBoundary.Size);

            return presentationBoundary;
        }






        /// <summary>Get the name of a static or instance property from a property access lambda.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="propertyLambda">lambda expression of the form: '() => Class.Property' or '() => object.Property'.</param>
        /// <returns>The name of the property.</returns>
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
