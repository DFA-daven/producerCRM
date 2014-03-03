namespace CallForm.iOS.Views
{
    using CallForm.Core.ViewModels;
    using Cirrious.MvvmCross.Binding.BindingContext;
    using Cirrious.MvvmCross.Touch.Views;
    using MonoTouch.UIKit;
    using System;
    using XibFree;



    // notes: see _Touch UI.txt for design details.
    // fixme: the UserIdentity view does not implement modal correctly. It looks like the foundation is here, but it was never implemented.
    //public class UserIdentityView : MvxBindingTouchViewController<MyViewModel>, IMvxModalTouchView
    // visit http://stackoverflow.com/questions/14518876/integrating-google-mobile-analytics-with-mvvmcross for examples of getting this to work.
    public class UserIdentityView : MvxViewController, IMvxModalTouchView
    {
        // hard-coded values
        // todo assigned but not used?
        private static float topMarginPixels = 70;
        private static double bannerHeightPercent = 12.5;
        private static double controlHeightPercent = 5;
        private static double controlWidthPercent = 31;

        private static double leftControlOriginPercent = 1;

        public override void ViewDidLoad()
        {
            UIColor controlBackgroundColor = UIColor.FromRGB(230, 230, 255);
            UIColor viewBackgroundColor = UIColor.FromRGB(200, 200, 255);

            var instructions = new UILabel
            {
                //Font = UIFont.SystemFontOfSize(UIFont.LabelFontSize * 1.1f),
                Text = "Please Enter Your Email Address and Asset Tag (if any)",
                TextColor = UIColor.White,
                BackgroundColor = viewBackgroundColor,
            };

            var email = new UITextField
            {
                Placeholder = "type email address here...",
                BackgroundColor = controlBackgroundColor,
                KeyboardType = UIKeyboardType.EmailAddress,
                
            };

            var assetTag = new UITextField
            {
                Placeholder = "enter asset tag information here...",
                BackgroundColor = controlBackgroundColor,
            };

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
                    new NativeView
                    {
                        // this "view" pushes the others down to the bottom of the page
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
                            Width = percentWidth(90), 
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
                            Width = percentWidth(90), 
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
                            Width = percentWidth(90), 
                            //MaxWidth = View.Frame.Width * 0.9f, 
                            //MarginLeft = View.Frame.Width * 0.05f,
                            //MarginTop = View.Frame.Height * 0.05f,
                            Gravity = Gravity.TopCenter,
                        },
                        
                    },
                    new NativeView
                    {
                        // this "view" pushes the others up to the top of the page
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
                BackgroundColor = viewBackgroundColor,
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
