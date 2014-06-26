using System.Drawing;
using CallForm.Core.ViewModels;
using CallForm.iOS.Views;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using XibFree;
using System.Linq.Expressions;
using System;

namespace CallForm.iOS.ViewElements
{
    public class EmailRecipientSelectDialog_ViewController : UIViewController
    {
        private readonly UITableView _table;
        private readonly NewVisit_ViewModel _viewModel;

        // ToDo: replace fixed values
        public EmailRecipientSelectDialog_ViewController(NewVisit_ViewModel viewModel, NewVisit_TableViewSource source)
        {
            View.BackgroundColor = UIColor.White;
            _viewModel = viewModel;
            _table = new UITableView(new RectangleF(0,0,500, 600));
            _table.Source = new EmailRecipientsTableSource(_viewModel, source);
            View.Add(_table);
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
                //SizeF size = _table.Frame.Size;
                //// leave space for "Done" button
                //size.Height += 50;
                //return size;
                return _table.Frame.Size;
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

        public override int RowsInSection(UITableView tableview, int section)
        {
            return _viewModel.ListOfEmailRecipients.Count;
        }

        public override UIView GetViewForFooter(UITableView tableView, int section)
        {
            var doneButton = new UIButton(UIButtonType.System);
            doneButton.SetTitle("Test 1", UIControlState.Normal);
            // review: is InvokeOnMainThread() correct?
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

            _viewModel.RaisePropertyChanged(GetPropertyName(() => _viewModel.SelectedEmailRecipients));
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
