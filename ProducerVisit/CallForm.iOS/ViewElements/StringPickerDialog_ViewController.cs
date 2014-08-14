// CallForm.iOS\ViewElements\StringPickerDialog_ViewController.cs

namespace CallForm.iOS.ViewElements
{
    using CallForm.iOS.Views;
    using MonoTouch.Foundation;
    using MonoTouch.UIKit;
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;

    public partial class StringPickerDialog_ViewController : UIViewController
    {
        private Action<string> _setValue = obj => { };
        private readonly UIPickerView _picker;
        private readonly StringListPickerViewModel _model;
        private float _doneButtonHeight = 50f;

        public StringPickerDialog_ViewController(Action<string> setValue, string initialValue, NewVisit_TableViewSource source, params string[] values)
        {
            #region _picker
            _model = new StringListPickerViewModel(values.ToList());
            _picker = new UIPickerView
            {
                Model = _model,
                ShowSelectionIndicator = true
            };
            
            _picker.AutoresizingMask = UIViewAutoresizing.FlexibleBottomMargin | UIViewAutoresizing.FlexibleRightMargin;
            _picker.Select(values.ToList().IndexOf(initialValue), 0, true);

            View.Add(_picker);
            #endregion _picker

            #region doneButton
            var doneButton = new UIButton(UIButtonType.System);
            doneButton.SetTitle("Done", UIControlState.Normal);
            doneButton.TouchUpInside += (sender, args) => { source.SafeDismissPopover(); };

            // Note: the _picker has already been added, so it has a Height. That Height is used as the vertical offset for the doneButton.
            doneButton.Frame = new RectangleF(0, _picker.Frame.Height, _picker.Frame.Width, _doneButtonHeight);

            // Hack: hide doneButton
            View.Add(doneButton);
            #endregion doneButton

            View.SizeToFit();
            View.BackgroundColor = UIColor.White;

            _setValue += setValue;
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

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            _setValue(_model.SelectedItem);
        }

        public override SizeF PreferredContentSize
        {
            get
            {
                SizeF size = _picker.Frame.Size;

                // Hack: comment out to hide doneButton
                //size.Height = _picker.Frame.Size.Height + _doneButtonHeight;

                return size;
            }
            set { base.PreferredContentSize = value; }
        }
    }

    public class StringListPickerViewModel : UIPickerViewModel
    {
        public string SelectedItem { get; private set; }

        private IList<string> _items;

        public IList<string> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                Selected(null, 0, 0);
            }
        }

        public StringListPickerViewModel()
        {
        }

        public StringListPickerViewModel(IList<string> items)
        {
            Items = items;
        }

        public override int GetRowsInComponent(UIPickerView picker, int component)
        {
            if (NoItem())
                return 1;
            return Items.Count;
        }

        public override string GetTitle(UIPickerView picker, int row, int component)
        {
            if (NoItem(row))
                return string.Empty;
            var item = Items[row];
            return GetTitleForItem(item);
        }

        public override void Selected(UIPickerView picker, int row, int component)
        {
            if (NoItem(row))
                SelectedItem = default(string);
            else
                SelectedItem = Items[row];
        }

        public override int GetComponentCount(UIPickerView picker)
        {
            return 1;
        }

        public virtual string GetTitleForItem(string item)
        {
            return item.ToString();
        }

        private bool NoItem(int row = 0)
        {
            return Items == null || row >= Items.Count;
        }
    }
}