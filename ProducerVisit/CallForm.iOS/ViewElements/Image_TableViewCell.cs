using System.Drawing;
using CallForm.Core.ViewModels;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace CallForm.iOS.ViewElements
{
    class Image_TableViewCell : UITableViewCell
    {
        private readonly UIButton _imageButton, _clearButton;
        private UIImage _image;
        private readonly NewVisit_ViewModel _viewModel;

        public Image_TableViewCell(string cellID, byte[] pictureBytes, bool editing, NewVisit_ViewModel viewModel)
            : base(UITableViewCellStyle.Value1, cellID)
        {
            _imageButton = new UIButton(UIButtonType.System);
            _imageButton.TouchUpInside += (sender, args) => { OnClick(); };
            if (pictureBytes != null && pictureBytes.Length > 0)
            {
                var imageData = NSData.FromArray(pictureBytes);
                _image = UIImage.LoadFromData(imageData);
                _imageButton.SetBackgroundImage(_image, UIControlState.Normal);
                ContentView.Add(_imageButton);
            }
            _clearButton = new UIButton(UIButtonType.System);
            _clearButton.SetTitle("Remove Image", UIControlState.Normal);
            _clearButton.TouchUpInside += (sender, args) => { ClearImage(); };
            if (editing)
            {
                ContentView.Add(_clearButton);
            }
            _viewModel = viewModel;
        }

        private void ClearImage()
        {
            _viewModel.PictureBytes = null;
        }

        private void OnClick()
        {
            if (_viewModel.PictureBytes == null)
            {
                _viewModel.TakePictureCommand.Execute(_viewModel);
            }
            else
            {
                var popover = new UIPopoverController(new UIViewController {View = new UIImageView(_image)});
                popover.PopoverContentSize = _image.Size;
                popover.PresentFromRect(_imageButton.Bounds, _imageButton.Superview,
                    UIPopoverArrowDirection.Any, true);
            }
        }

        public void SetPicture(byte[] pictureBytes)
        {
            if (_image == null)
            {
                ContentView.Add(_imageButton);
            }
            if (pictureBytes == null)
            {
                _image = UIImage.FromBundle("Camera_icon.gif");
            }
            else
            {
                var imageData = NSData.FromArray(pictureBytes);
                _image = UIImage.LoadFromData(imageData);
            }
            _imageButton.SetBackgroundImage(_image, UIControlState.Normal);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
            _imageButton.Frame = new RectangleF(ContentView.Bounds.Width - 165, 5, 150, 150);
            _clearButton.Frame = new RectangleF(ContentView.Bounds.Width / 2 - 75, 5, 150, 150);

        }
    }
}
