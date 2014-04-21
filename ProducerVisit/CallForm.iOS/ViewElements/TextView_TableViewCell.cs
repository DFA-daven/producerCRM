using System;
using System.Drawing;
using MonoTouch.UIKit;

namespace CallForm.iOS.ViewElements
{
    class TextView_TableViewCell : UITableViewCell
    {
        private readonly UITextView _textField;

        public TextView_TableViewCell(string cellID, bool editing, string text, EventHandler didChange)
            : base(UITableViewCellStyle.Value1, cellID)
        {
            _textField = new UITextView
            {
                Text = text,
                TextColor = UIColor.Black,
                Editable = editing,
            };
            _textField.Changed += didChange;
            _textField.Font = UIFont.SystemFontOfSize(UIFont.SmallSystemFontSize + 2);
            ContentView.AddSubview(_textField);
        }

        public void Edit()
        {
            _textField.BecomeFirstResponder();
        }

        public void SetText(string text)
        {
            _textField.Text = text;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            DetailTextLabel.TextColor = UIColor.Clear;
            _textField.Frame = new RectangleF(TextLabel.Bounds.Width + 20, 5, ContentView.Bounds.Width - TextLabel.Bounds.Width - 25, 100);
        }

        public void HideKeyboard()
        {
            _textField.ResignFirstResponder();
        }
    }
}
