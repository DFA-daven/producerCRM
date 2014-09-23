namespace CallForm.iOS.Views
{
    using CallForm.Core.ViewModels;
    using CallForm.iOS.ViewElements;
    using MonoTouch.Foundation;
    using MonoTouch.UIKit;
    using System;
    using System.Drawing;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading.Tasks;

    public class NewVisit_TableViewSource : UITableViewSource
    {
        string _nameSpace = "CallForm.iOS.";

        #region Properties
        private readonly NewVisit_ViewModel _viewModel;
        private readonly TextField_TableViewCell _memberNumberCell, _durationCell;
        private readonly UITableViewCell _callTypeCell, _dateCell, _reasonCell, _emailRecipientsCell;
        private readonly Image_TableViewCell _takePictureCell;
        private readonly TextView_TableViewCell _notesCell;

        /// <summary>The popover controller.
        /// </summary>
        UIPopoverController _popoverController;
        UIViewController _currentViewController;
        SizeF _contollerPreferredSize;

        /// <summary>The maximum value to display the controller. Anything larger will not be honored.
        /// </summary>
        float _distanceToBottom = 0f;
        #endregion

        /// <summary>A container for the popover controller.
        /// </summary>
        public UIViewController DatePickerPopover, CallTypePickerPopover, ReasonPickerPopover, EmailPickerPopover;

        public Task DismissPopover(UIViewController popover)
        {
            return popover.DismissViewControllerAsync(true);
        } 

        public NewVisit_TableViewSource(NewVisit_ViewModel viewModel, UITableView tableView)
        {
            _viewModel = viewModel;


            #region viewModel property changed
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
            #endregion viewModel property changed

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
        /// <summary>Gets the value (in points) for a given row (cell).
        /// </summary>
        /// <param name="tableView">The <see cref="UITableView">(view) table</see> that contains the cell.</param>
        /// <param name="indexPath">The <see cref="NSIndexPath"/> to the selected row (cell).</param>
        /// <returns></returns>
        public override float GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            Common_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);

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
            Common_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);

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
            Common_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);

            SizeF availableSize = new SizeF();

            
            float availableDisplayHeight = 0f;



            if (_viewModel.Editing)
            {
                _memberNumberCell.HideKeyboard();
                _durationCell.HideKeyboard();
                _notesCell.HideKeyboard();

                var baseCellRect = tableView.RectForRowAtIndexPath(indexPath);
                

                // Note: the Row # here matches the position of the row in the UITableView.
                switch (indexPath.Row)
                {
                    case 0:
                        _memberNumberCell.Edit();
                        break;
                    case 1:
                        // ToDo: seeing an un-handled exception here if debug on iPhoneSimulator
                        _currentViewController = CallTypePickerPopover;
                        if (_viewModel.Editing)
                        {
                            _popoverController = new UIPopoverController(_currentViewController);
                            _popoverController.PopoverContentSize = _currentViewController.PreferredContentSize;
                            //_popoverController.PresentFromRect(tableView.RectForRowAtIndexPath(indexPath ), tableView.Superview, UIPopoverArrowDirection.Any, true);
                            //_popoverController.PresentFromRect(GetPresentationRectangleForCell(tableView, _callTypeCell), tableView.Superview, UIPopoverArrowDirection.Any, true);
                            _popoverController.PresentFromRect(baseCellRect, tableView.Superview, UIPopoverArrowDirection.Any, true);
                        }
                        break;
                    case 2:
                        _currentViewController = DatePickerPopover;
                        _popoverController = new UIPopoverController(_currentViewController);
                        _popoverController.PopoverContentSize = _currentViewController.PreferredContentSize;
                        //_popoverController.PresentFromRect(GetPresentationRectangleForCell(tableView, _dateCell), tableView.Superview, UIPopoverArrowDirection.Any, true);
                        _popoverController.PresentFromRect(baseCellRect, tableView.Superview, UIPopoverArrowDirection.Any, true);
                        break;
                    case 3:
                        _durationCell.Edit();
                        break;
                    case 4:
                        _currentViewController = ReasonPickerPopover;
                        _popoverController = new UIPopoverController(_currentViewController);
                        //_popoverController.PopoverContentSize = _currentViewController.PreferredContentSize;

                        Common_iOS.DebugMessage("  [nv_tvs][rs][4] > baseCellRect.bottom = " + baseCellRect.Bottom.ToString() + ", tableView.Superview.Frame.Bottom = " + tableView.Superview.Frame.Bottom.ToString() + " < @ @ @ @ @ @ @");
                        _distanceToBottom = tableView.Superview.Frame.Bottom - baseCellRect.Bottom;

                        Common_iOS.DebugMessage("  [nv_tvs][rs][4] > _popoverController.ContentViewController.PreferredContentSize.Height = " + _popoverController.ContentViewController.PreferredContentSize.Height.ToString() + ", _distanceToBottom = " + _distanceToBottom.ToString() + " < @ @ @ @ @ @ @");
                        //availableDisplayHeight = Math.Min(_distanceToBottom, _currentViewController.PreferredContentSize.Height);
                        availableDisplayHeight = Math.Min(_distanceToBottom, _popoverController.ContentViewController.PreferredContentSize.Height);
                        //availableDisplayHeight = Math.Max(_distanceToBottom, _popoverController.ContentViewController.PreferredContentSize.Height);

                        //availableSize = _popoverController.ContentViewController.ContentSizeForViewInPopover; // deprecated
                        //availableSize = _popoverController.ContentViewController.PreferredContentSize; // null?
                        availableSize = _currentViewController.PreferredContentSize;
                        
                        availableSize.Height = availableDisplayHeight;
                        // Hack: just testing...
                        availableSize.Height = Math.Min((float)Math.Round(UIScreen.MainScreen.Bounds.Height * 0.50, 0), (float)Math.Round(UIScreen.MainScreen.Bounds.Width * 0.50, 0));
                        
                        Common_iOS.DebugMessage("* [nv_tvs][rs][4] > availableDisplayHeight = " + availableDisplayHeight.ToString() + " < [nv_tvs][rs] @ @ @ @ @ @ @");

                        //_popoverController.ContentViewController.PreferredContentSize = availableSize;
                        _popoverController.PopoverContentSize = availableSize;

                        _popoverController.PresentFromRect(baseCellRect, tableView.Superview, UIPopoverArrowDirection.Any, true);
#if (DEBUG || BETA)
                        // No changes / variable assignment here -- this is diagnostic code!
                        if (IsOS7OrLater())
                        {
                            _popoverController.BackgroundColor = UIColor.Red; // this is the outer container
                        }
#endif

                
                Common_iOS.DebugMessage("  [nv_tvs][rs][4] > _contollerPreferredSize Height = " + _contollerPreferredSize.Height.ToString() + ", Width = " + _contollerPreferredSize.Width.ToString());

                        break;
                    case 5:
                        _notesCell.Edit();
                        break;
                    case 6:
                        _viewModel.TakePictureCommand.Execute(_viewModel);
                        break;
                    case 7:
                        _currentViewController = EmailPickerPopover;
                        _popoverController = new UIPopoverController(_currentViewController);
                        //_popoverController.PopoverContentSize = _currentViewController.PreferredContentSize;

                        _popoverController.PresentFromRect(baseCellRect, tableView.Superview, UIPopoverArrowDirection.Any, true);
                        
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
            Common_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);

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

        /// <summary>Close the <c>_popoverController</c>.
        /// </summary>
        public void SafeDismissPopover()
        {
            Common_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);

            if (_popoverController != null)
            {
                if (_popoverController.PopoverVisible) 
                {
                    //_popoverController.Dismiss(true);
                    DismissPopover(_currentViewController);
                }
            }
        }

        /// <summary>Get the rectangle bounding a specific cell.
        /// </summary>
        /// <param name="tableView">The <see cref="UITableView">(view) table</see> that contains the cell.</param>
        /// <param name="targetCell">A <see cref="UITableViewCell">cell</see> (ie: row) in the <paramref name="tableView"/></param>
        /// <returns>A rectangle bounding the given cell.</returns>
        internal RectangleF GetRectangleForCell(UITableView tableView, UITableViewCell targetCell)
        {
            Common_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);

            return tableView.RectForRowAtIndexPath(tableView.IndexPathForCell(targetCell));
        }

        /// <summary>Creates a new rectangle which matches the on-screen location of the given cell.
        /// </summary>
        /// <param name="tableView">The <see cref="UITableView">(view) table</see> that contains the cell.</param>
        /// <param name="targetCell">A <see cref="UITableViewCell">cell</see> (ie: row) in the <paramref name="tableView"/></param>
        /// <returns>A rectangle offset vertically.</returns>
        /// <remarks>Because of the indexing used to identify the cells, the boundary of any given cell appears to match the 
        /// previous cell in the <see cref="UITableView">table view</see>. There is no direct method for 'adding one' to the 
        /// cell. Instead, this method shifts the cell boundary down by the value of the boundary. The resulting location overlays
        /// the on-screen location of the cell contents.</remarks>
        internal RectangleF GetPresentationRectangleForCell(UITableView tableView, UITableViewCell targetCell)
        {
            Common_iOS.DebugMessage(_nameSpace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);

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

        /// <summary>Is this device running iOS 7.0.
        /// </summary>
        /// <returns>True if this is iOS 7.0.</returns>
        public bool IsOS7OrLater()
        {
            bool thisIsOS7 = false;
            string version = UIDevice.CurrentDevice.SystemVersion;
            string[] parts = version.Split('.');
            string major = parts[0];
            Common_iOS.DebugMessage("  [nv_tvs][i7ol] > major version (string): " + major);
            int majorVersion = Common_iOS.SafeConvert(major, 0);
            Common_iOS.DebugMessage("  [nv_tvs][i7ol] > major version (int): " + majorVersion.ToString());

            if (majorVersion > 6)
            {
                //float displacement_y = this.TopLayoutGuide.Length;

                thisIsOS7 = true;
            }

            Common_iOS.DebugMessage("  [nv_tvs][i7ol] > version is higher than 6 = " + thisIsOS7.ToString());

            return thisIsOS7;
        }
    }
}
