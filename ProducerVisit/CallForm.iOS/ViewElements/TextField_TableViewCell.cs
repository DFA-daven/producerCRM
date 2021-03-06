﻿using System;
using System.Drawing;
using MonoTouch.UIKit;

namespace CallForm.iOS.ViewElements
{
    class TextField_TableViewCell : UITableViewCell
    {
        private readonly UITextField _textField;

        public TextField_TableViewCell(string cellID, bool editing, string text, UIKeyboardType type, UITextFieldChange shouldChange, EventHandler onTextChanged)
            : base(UITableViewCellStyle.Value1, cellID)
        {
            _textField = new UITextField
            {
                ShouldChangeCharacters = shouldChange,
                Text = text ?? string.Empty,
                TextAlignment = UITextAlignment.Right,
                BackgroundColor = UIColor.Clear, // this doesn't seem to matter
                TextColor = UIColor.Black,
                KeyboardType = type,
            };
            _textField.EditingChanged += onTextChanged;
            if (editing)
            {
                DetailTextLabel.TextColor = UIColor.Clear;
                
                ContentView.Add(_textField);
            }
        }

        public void Edit()
        {
            _textField.BecomeFirstResponder();
        }

        public void SetText(string text)
        {
            _textField.Text = text ?? string.Empty;
        }

        #region overrides
        #pragma warning disable 1591
        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            _textField.Frame = new RectangleF(ContentView.Bounds.Width - 310, ContentView.Bounds.Height / 2 - 10, 300, 20);
        }
        #pragma warning restore 1591
        #endregion overrides

        public void HideKeyboard()
        {
            _textField.ResignFirstResponder();
        }
    }
}
