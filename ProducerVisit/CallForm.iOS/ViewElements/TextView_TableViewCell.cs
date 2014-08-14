using System;
using System.Drawing;
using MonoTouch.UIKit;

namespace CallForm.iOS.ViewElements
{
    class TextView_TableViewCell : UITableViewCell
    {
        private readonly UITextView _textView;

        public TextView_TableViewCell(string cellID, bool editing, string text, EventHandler didChange)
            : base(UITableViewCellStyle.Value1, cellID)
        {
            _textView = new UITextView
            {
                Text = text,
                TextColor = UIColor.Black,
                Editable = editing,
            };
            _textView.Changed += didChange;
            //_textView.Font = UIFont.SystemFontOfSize(UIFont.SmallSystemFontSize + 2);
            _textView.Font = UIFont.SystemFontOfSize(UIFont.SmallSystemFontSize);
            
            ContentView.Add(_textView);
        }

        public void Edit()
        {
            _textView.BecomeFirstResponder();
        }

        public void SetText(string text)
        {
            _textView.Text = text;
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            DetailTextLabel.TextColor = UIColor.Clear;
            DetailTextLabel.TextColor = UIColor.Red;
            _textView.Frame = new RectangleF(TextLabel.Bounds.Width + 20, 5, ContentView.Bounds.Width - TextLabel.Bounds.Width - 25, 100);
        }

        public void HideKeyboard()
        {
            _textView.ResignFirstResponder();
        }
    }
}
