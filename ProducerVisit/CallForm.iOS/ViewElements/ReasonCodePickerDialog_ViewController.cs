namespace CallForm.iOS.ViewElements
{
    using CallForm.Core.Models;
    using CallForm.Core.ViewModels;
    using CallForm.iOS.Views;
    using MonoTouch.Foundation;
    using MonoTouch.UIKit;
    using System;
    using System.Drawing;
    using System.Linq.Expressions;

    public class ReasonCodePickerDialog_ViewController : UIViewController
    {
        private readonly UITableView _table;
        private readonly NewVisit_ViewModel _viewModel;

        // ToDo: replace fixed values
        public ReasonCodePickerDialog_ViewController(NewVisit_ViewModel viewModel, NewVisit_TableViewSource source)
        {
            View.BackgroundColor = UIColor.White;
            _viewModel = viewModel;
            int sectionNumber = 0;
            int count = source.RowsInSection(_table, sectionNumber) + 1;
            int tableHeight = count * (int)source.GetHeightForFooter(_table, sectionNumber);

            _table = new UITableView(new RectangleF(0,0,500, tableHeight));
            _table.Source = new ReasonCodeTableSource(_viewModel, source);
            View.Add(_table);
            
        }

        public override void ViewDidDisappear(bool animated)
        {
            base.ViewDidDisappear(animated);
            _viewModel.RaisePropertyChanged(GetPropertyName(() => _viewModel.SelectedReasonCodes));
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

    public class ReasonCodeTableSource : UITableViewSource
    {
        private readonly NewVisit_ViewModel _viewModel;
        private readonly NewVisit_TableViewSource _source;
        private const string CellIdentifier = "TableCell";


        public ReasonCodeTableSource(NewVisit_ViewModel viewModel, NewVisit_TableViewSource source)
        {
            _viewModel = viewModel;
            _source = source;
        }

        /// <summary>The number of rows to be displayed.
        /// </summary>
        /// <param name="tableview"></param>
        /// <param name="section"></param>
        /// <returns>A row count.</returns>
        public override int RowsInSection(UITableView tableview, int section)
        {
            return _viewModel.ListOfReasonCodes.Count;
        }

        public override UIView GetViewForFooter(UITableView tableView, int section)
        {
            var doneButton = new UIButton(UIButtonType.System);
            doneButton.SetTitle("Test 2", UIControlState.Normal);
            // review: is InvokeOnMainThread() correct?
            doneButton.TouchUpInside += (sender, args) => { InvokeOnMainThread(_source.DismissPopover); };
            doneButton.Frame = new RectangleF(0, 0, tableView.Frame.Width, 50);
            return doneButton;
            // hack: as a last resort, hide the "Done" button?
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            ReasonCode reasonCode = _viewModel.ListOfReasonCodes[indexPath.Row];
            if (_viewModel.SelectedReasonCodes.Contains(reasonCode))
            {
                _viewModel.SelectedReasonCodes.Remove(reasonCode);
            }
            else
            {
                _viewModel.SelectedReasonCodes.Add(reasonCode);
            }
            _viewModel.RaisePropertyChanged(GetPropertyName(() => _viewModel.SelectedReasonCodes));
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
