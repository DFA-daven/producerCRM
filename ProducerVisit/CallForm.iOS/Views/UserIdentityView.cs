using CallForm.Core.ViewModels;
using Cirrious.MvvmCross.Binding.BindingContext;
using Cirrious.MvvmCross.Touch.Views;
using MonoTouch.UIKit;
using XibFree;

namespace CallForm.iOS.Views
{
    public class UserIdentityView : MvxViewController, IMvxModalTouchView
    {
        public override void ViewDidLoad()
        {
            var email = new UITextField
            {
                Placeholder = "Email Address",
                BackgroundColor = UIColor.FromRGB(230, 230, 255),
                KeyboardType = UIKeyboardType.EmailAddress,
            };
            var assetTag = new UITextField
            {
                Placeholder = "Asset Tag",
                BackgroundColor = UIColor.FromRGB(230, 230, 255),
            };
            var button = new UIButton(UIButtonType.System);
            button.SetTitle("OK", UIControlState.Normal);

            var layout = new LinearLayout(Orientation.Vertical)
            {
                Gravity = Gravity.CenterHorizontal,
                Spacing = 30,
                SubViews = new View[]
                {
                    new NativeView
                    {
                        View = new UIView(),
                        LayoutParameters = new LayoutParameters
                        {
                            Weight = 1,
                            Height = AutoSize.FillParent
                        }
                    },
                    new NativeView
                    {
                        View = new UILabel
                        {
                            Font = UIFont.SystemFontOfSize(24),
                            Text = "Please Enter Your Email Address and Asset Tag (if any)",
                            TextColor = UIColor.White,
                            BackgroundColor = UIColor.FromRGB(200, 200, 255),
                        }
                    },
                    new NativeView
                    {
                        View = email,
                        LayoutParameters = new LayoutParameters
                        {
                            Width = 400
                        }
                    },
                    new NativeView
                    {
                        View = assetTag,
                        LayoutParameters = new LayoutParameters
                        {
                            Width = 400
                        }
                    },
                    new NativeView
                    {
                        View = button,
                        LayoutParameters = new LayoutParameters
                        {
                            Width = 400
                        }
                    },
                    new NativeView
                    {
                        View = new UIView(),
                        LayoutParameters = new LayoutParameters
                        {
                            Weight = 1,
                            Height = AutoSize.FillParent
                        }
                    },
                }
            };

            View = new UILayoutHost(layout)
            {
                BackgroundColor = UIColor.FromRGB(200, 200, 255),
            };

            base.ViewDidLoad();

            (ViewModel as UserIdentityViewModel).Error += OnError;
            (ViewModel as UserIdentityViewModel).DeviceID = UIDevice.CurrentDevice.IdentifierForVendor.AsString();

            var set = this.CreateBindingSet<UserIdentityView, UserIdentityViewModel>();
            set.Bind(email).To(vm => vm.UserEmail);
            set.Bind(assetTag).To(vm => vm.AssetTag);
            set.Bind(button).To(vm => vm.SaveCommand);
            set.Apply();
        }

        private void OnError(object sender, ErrorEventArgs errorEventArgs)
        {
            InvokeOnMainThread(() => new UIAlertView("Error", errorEventArgs.Message, null, "OK").Show());
        }
    }
}
