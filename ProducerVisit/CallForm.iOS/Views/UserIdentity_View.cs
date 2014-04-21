namespace CallForm.iOS.Views
{
    using CallForm.Core.ViewModels;
    using Cirrious.MvvmCross.Binding.BindingContext;
    using Cirrious.MvvmCross.Touch.Views;
    using MonoTouch.UIKit;
    using System;
    using XibFree;

    // notes: see _Touch UI.txt for design details.
    // FixMe: the UserIdentity view does not implement modal correctly. It looks like the foundation is here, but it was never implemented.
    //public class UserIdentityView : MvxBindingTouchViewController<MyViewModel>, IMvxModalTouchView
    // visit http://stackoverflow.com/questions/14518876/integrating-google-mobile-analytics-with-mvvmcross for examples of getting this to work.
    public class UserIdentity_View : MvxViewController, IMvxModalTouchView
    {
        public override void ViewDidLoad()
        {
            // instructions for the inputs
            var instructions = new UILabel
            {
                //Font = UIFont.SystemFontOfSize(UIFont.LabelFontSize * 1.1f),
                Text = "Please Enter Your Email Address and Asset Tag (if any)",
                TextColor = UIColor.White,
                BackgroundColor = Common.viewBackgroundColor,
            };

            // the email address field
            var email = new UITextField
            {
                Placeholder = "type email address here...",
                BackgroundColor = Common.controlBackgroundColor,
                KeyboardType = UIKeyboardType.EmailAddress,
                
            };

            // asset tag field
            var assetTag = new UITextField
            {
                Placeholder = "enter asset tag information here...",
                BackgroundColor = Common.controlBackgroundColor,
            };

            // ok button
            var button = new UIButton(UIButtonType.System);
                button.SetTitle("OK", UIControlState.Normal);
                button.SetTitle("OK", UIControlState.Disabled);
                button.SetTitleColor(UIColor.Gray, UIControlState.Disabled);
                //button.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleBottomMargin;
                button.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;

            // this view has an array of subviews
            var layout = new LinearLayout(Orientation.Vertical)
            {
                Gravity = Gravity.TopCenter,
                Spacing = 20,
                SubViews = new View[]
                {
                    // this "view" pushes the others down the page
                    new NativeView
                    {
                        View = new UIView(),
                        LayoutParameters = new LayoutParameters
                        {
                            Weight = 1,
                            Height = percentHeight(10),
                            Gravity = Gravity.TopCenter,
                        }
                    },
                    new NativeView
                    {
                        View = instructions,
                        LayoutParameters = new LayoutParameters()
                        {
                            //Width = AutoSize.WrapContent,  
                            //Height = AutoSize.WrapContent,
                            //MaxWidth = View.Frame.Width * 0.9f, 
                            //MarginLeft = View.Frame.Width * 0.05f,
                            //MarginTop = View.Frame.Height * 0.05f,
                            Gravity = Gravity.TopCenter,
                        },
                    },

                    new NativeView
                    {
                        View = email,
                        LayoutParameters = new LayoutParameters()
                        {
                            //Width = percentWidth(90), 
                            //MaxWidth = View.Frame.Width * 0.9f, 
                            //MarginLeft = screenWidth() * 0.05f,
                            //MarginTop = View.Frame.Height * 0.05f,
                            Gravity = Gravity.TopCenter,
                        },
                    },

                    new NativeView
                    {
                        View = assetTag,
                        LayoutParameters = new LayoutParameters()
                        {
                            //Width = percentWidth(90), 
                            //MaxWidth = View.Frame.Width * 0.9f, 
                            //MarginLeft = percentWidth(5),
                            //MarginTop = View.Frame.Height * 0.05f,
                            Gravity = Gravity.TopCenter,
                        },
                    },
                    new NativeView
                    {
                        View = button,
                        LayoutParameters = new LayoutParameters()
                        {
                            //Width = percentWidth(90), 
                            //MaxWidth = View.Frame.Width * 0.9f, 
                            //MarginLeft = View.Frame.Width * 0.05f,
                            //MarginTop = View.Frame.Height * 0.05f,
                            Gravity = Gravity.TopCenter,
                        },
                        
                    },

                    // this "view" pushes the others up to the top of the page
                    new NativeView
                    {
                        View = new UIView(),
                        LayoutParameters = new LayoutParameters
                        {
                            Weight = 1,
                            Height = percentHeight(5),
                            Gravity = Gravity.TopCenter,
                        }
                    },
                }
            };

            View = new UILayoutHost(layout)
            {
                BackgroundColor = Common.viewBackgroundColor,
            };

            base.ViewDidLoad();

            (ViewModel as UserIdentity_ViewModel).Error += OnError;

            // note: regarding UIDevice.CurrentDevice.IdentifierForVendor
            // The value of this property is the same for apps that come from the same vendor running on the same
            // device. A different value is returned for apps on the same device that come from different vendors, and 
            // for apps on different devices regardless of vendor.

            // The value of this property may be nil if the app is running in the background, before the user has 
            // unlocked the device the first time after the device has been restarted. If the value is nil, wait and get
            // the value again later.

            // The value in this property remains the same while the app (or another app from the same vendor) is 
            // installed on the iOS device. The value changes when the user deletes all of that vendor’s apps from the 
            // device and subsequently reinstalls one or more of them. Therefore, if your app stores the value of this 
            // property anywhere, you should gracefully handle situations where the identifier changes.
            // http://developer.apple.com/library/ios/documentation/uikit/reference/UIDevice_Class/Reference/UIDevice.html#//apple_ref/occ/instp/UIDevice/identifierForVendor
            (ViewModel as UserIdentity_ViewModel).DeviceID = UIDevice.CurrentDevice.IdentifierForVendor.AsString();

            var set = this.CreateBindingSet<UserIdentity_View, UserIdentity_ViewModel>();
            set.Bind(email).To(vm => vm.UserEmail);
            set.Bind(assetTag).To(vm => vm.AssetTag);
            set.Bind(button).To(vm => vm.SaveCommand);
            set.Apply();
        }

        private float screenWidth()
        {
            float screenWidth = UIScreen.MainScreen.Bounds.Width;
            return screenWidth;
        }

        private float percentHeight(double percent)
        {
            return calculatePercent(UIScreen.MainScreen.Bounds.Height, percent);
        }


        private float percentWidth(double percent)
        {
            float width = calculatePercent(screenWidth(), percent);
            return width;
        }

        private float calculatePercent(float dimension, double percent)
        {
            percent = percent / 100;
            double value = dimension * percent;
            value = Math.Abs(Math.Round(value));
            return (float)value;
        }

        private void OnError(object sender, ErrorEventArgs errorEventArgs)
        {
            InvokeOnMainThread(() => new UIAlertView("Error", errorEventArgs.Message, null, "OK").Show());
        }
    }
}
