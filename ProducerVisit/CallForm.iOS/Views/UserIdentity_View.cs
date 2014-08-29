namespace CallForm.iOS.Views
{
    using CallForm.Core.ViewModels;
    using Cirrious.MvvmCross.Binding.BindingContext;
    using Cirrious.MvvmCross.Touch.Views;
    using MonoTouch.UIKit;
    using System;
    using System.Reflection;
    using XibFree;

    // notes: see _Touch UI.txt for design details.
    // FixMe: the UserIdentity view does not implement modal correctly. It looks like the foundation is here, but it was never implemented.
    //public class UserIdentityView : MvxBindingTouchViewController<MyViewModel>, IMvxModalTouchView
    // visit http://stackoverflow.com/questions/14518876/integrating-google-mobile-analytics-with-mvvmcross for examples of getting this to work.
    public class UserIdentity_View : MvxViewController, IMvxModalTouchView
    {
        #region Properties
        string _namespace = "CallForm.iOS.";
        //string _namespace = "CallForm.iOS.Views.UserIdentity_View";
        #endregion

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

        public override void ViewDidLoad()
        {
            CommonCore_iOS.DebugMessage(_namespace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            CommonCore_iOS.DebugMessage("  [ui_v][vdl] > starting method...");

            //InvokeOnMainThread(() => { new UIAlertView("starting method...", MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name, null, "OK").Show(); });

            float space = 20;

            #region controls
            // instructions for the inputs
            var instructions = new UILabel
            {
                //Font = UIFont.SystemFontOfSize(UIFont.LabelFontSize * 1.1f),
                Text = "Please Enter Your Email Address and Asset Tag (if any)",
                TextColor = UIColor.White,
                BackgroundColor = CommonCore_iOS.viewBackgroundColor,
            };

            // the email address field
            var email = new UITextField
            {
                Placeholder = "type email address here...",
                BackgroundColor = CommonCore_iOS.controlBackgroundColor,
                KeyboardType = UIKeyboardType.EmailAddress,
            };

            // asset tag field
            var assetTag = new UITextField
            {
                Placeholder = "enter asset tag information here...",
                BackgroundColor = CommonCore_iOS.controlBackgroundColor,
                KeyboardType = UIKeyboardType.Default,
            };

            // ok button
            var button = new UIButton(UIButtonType.System);
            button.SetTitle("OK", UIControlState.Normal);
            button.SetTitle("OK", UIControlState.Disabled);
            button.SetTitleColor(UIColor.Gray, UIControlState.Disabled);
            //button.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleBottomMargin;
            button.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            
            #region files
            var file1 = new UIButton(UIButtonType.System);
            file1.SetTitle("CallType", UIControlState.Normal);
            file1.SetTitle("CallType", UIControlState.Disabled);
            file1.SetTitleColor(UIColor.Gray, UIControlState.Disabled);

            var file2 = new UIButton(UIButtonType.System);
            file2.SetTitle("EmailRecipient", UIControlState.Normal);
            file2.SetTitle("EmailRecipient", UIControlState.Disabled);
            file2.SetTitleColor(UIColor.Gray, UIControlState.Disabled);

            var file3 = new UIButton(UIButtonType.System);
            file3.SetTitle("ReasonCode", UIControlState.Normal);
            file3.SetTitle("ReasonCode", UIControlState.Disabled);
            file3.SetTitleColor(UIColor.Gray, UIControlState.Disabled);
            #endregion files
            #endregion controls



            #region file status
            var fileStatusLayout = new LinearLayout(Orientation.Horizontal)
            {
                Gravity = Gravity.TopCenter,
                Spacing = space, 
                SubViews = new View[]
                {
                    new NativeView
                    {
                        View = file1,
                        LayoutParameters = new LayoutParameters()
                        {
                            Width = percentWidth(10),
                            Height = space,
                            Weight = 3,

                            Gravity = Gravity.TopCenter ,
                        }
                    },
                    new NativeView
                    {
                        View = file2,
                        LayoutParameters = new LayoutParameters()
                        {
                            Width = percentWidth(10),
                            Height = space,
                            Weight = 2,

                            Gravity = Gravity.TopCenter ,
                        }
                    },
                    new NativeView
                    {
                        View = file3,
                        LayoutParameters = new LayoutParameters()
                        {
                            Width = percentWidth(10),
                            Height = space,
                            Weight = 1,

                            Gravity = Gravity.TopCenter ,
                        }
                    },
                },
                LayoutParameters = new LayoutParameters()
                {
                    Height = space + space,
                    Gravity = Gravity.TopCenter,
                    MaxHeight = space + space + space,
                }
            };

            // wrap the LinearLayout in a UIView
            var fileStatusView = new UIView();
            fileStatusView = new UILayoutHost(fileStatusLayout)
            {
                //BackgroundColor = UIColor.LightGray, 
            };

            //fileStatusView.SizeToFit();   // tightly enclose the sub-views
            #endregion

            // this view has an array of sub-views
            var pageLayout = new LinearLayout(Orientation.Vertical)
            {
                Gravity = Gravity.TopCenter,
                Spacing = space,
                SubViews = new View[]
                {
                    // this "view" pushes the others down the page
                    new NativeView
                    {
                        View = new UIView()
                        {
                            //BackgroundColor = UIColor.Yellow,
                        },
                        LayoutParameters = new LayoutParameters()
                        {
                            Width = space,
                            Height = percentHeight(5),
                            Weight = 7,

                            Gravity = Gravity.TopCenter,
                        }
                    },

                    new NativeView
                    {
                        View = fileStatusView,
                        LayoutParameters = new LayoutParameters()
                        {
                            //Width = fileStatusView.Frame.Width,
                            Height = space + space,
                            Weight = 6,

                            Gravity = Gravity.TopCenter ,
                        },
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
                            Weight = 7,

                            Gravity = Gravity.TopCenter,
                        },
                    },

                    new NativeView
                    {
                        View = email,
                        LayoutParameters = new LayoutParameters()
                        {
                            //Width = PercentWidth(90), 
                            //MaxWidth = View.Frame.Width * 0.9f, 
                            //MarginLeft = ViewFrameWidth() * 0.05f,
                            //MarginTop = View.Frame.Height * 0.05f,
                            Weight = 4,

                            Gravity = Gravity.TopCenter,
                        },
                    },

                    new NativeView
                    {
                        View = assetTag,
                        LayoutParameters = new LayoutParameters()
                        {
                            //Width = PercentWidth(90), 
                            //MaxWidth = View.Frame.Width * 0.9f, 
                            //MarginLeft = PercentWidth(5),
                            //MarginTop = View.Frame.Height * 0.05f,
                            Weight = 3,

                            Gravity = Gravity.TopCenter,
                        },
                    },
                    new NativeView
                    {
                        View = button,
                        LayoutParameters = new LayoutParameters()
                        {
                            //Width = PercentWidth(90), 
                            //MaxWidth = View.Frame.Width * 0.9f, 
                            //MarginLeft = View.Frame.Width * 0.05f,
                            //MarginTop = View.Frame.Height * 0.05f,
                            Weight = 2,

                            Gravity = Gravity.TopCenter,
                        },
                        
                    },

                    // this "view" pushes the others up to the top of the page
                    new NativeView
                    {
                        View = new UIView(),
                        LayoutParameters = new LayoutParameters()
                        {
                            Height = percentHeight(5),
                            Weight = 1,

                            Gravity = Gravity.TopCenter,
                        }
                    },
                }
            };

            View = new UILayoutHost(pageLayout)
            {
                BackgroundColor = CommonCore_iOS.viewBackgroundColor,
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

            // Note: the "AsString" method returns a string representation of the UUID. The "ToString" method returns the description (selector).
            // ToDo:  replace with the advertisingIdentifier property of the ASIdentifierManager class.
            (ViewModel as UserIdentity_ViewModel).DeviceID = UIDevice.CurrentDevice.IdentifierForVendor.AsString();

            var set = this.CreateBindingSet<UserIdentity_View, UserIdentity_ViewModel>();
            set.Bind(email).To(vm => vm.UserEmail);
            set.Bind(assetTag).To(vm => vm.AssetTag);
            set.Bind(button).To(vm => vm.SaveCommand);
            set.Apply();

            #region UI action
            email.ShouldReturn = delegate
            {
                // keep keyboard open; shift input to the next field
                assetTag.BecomeFirstResponder();
                return true;
            };

            assetTag.ShouldReturn = delegate
            {
                assetTag.ResignFirstResponder();    // close the keyboard
                button.BecomeFirstResponder();
                return true;
            };

            // keyboard should disappear if user taps outside of a text-box
            // Note: this works on this view b/c 1) there is open space, and 2) the only controls are text-box or button.
            var goAway = new UITapGestureRecognizer(() => View.EndEditing(true));
            View.AddGestureRecognizer(goAway);
            #endregion UI action

            CommonCore_iOS.DebugMessage(_namespace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);
            CommonCore_iOS.DebugMessage("  [ui_v][vdl] > ...finished method.");
        }

        public override void ViewDidLayoutSubviews()
        {
            CommonCore_iOS.DebugMessage(_namespace + MethodBase.GetCurrentMethod().DeclaringType.Name, MethodBase.GetCurrentMethod().Name);

            base.ViewDidLayoutSubviews();

            /*
             * Note: the TopLayoutGuide and BottomLayoutGuide values are generated dynamically AFTER
             * the View has been added to the hierarchy, so attempting to read them in ViewDidLoad will return 0. 
             * So, calculate the value after the View has loaded, for example here in ViewDidLoadSubviews.
             */
            /*
             * Note: EdgesForExtendedLayout may allow this app to display, but using TopLayoutGuide
             * and BottomLayoutGuide are preferred since they allow the app to meet the iOS 7 design goals.
             */
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

        /// <summary>Displays the error issued by the <c>ViewModel</c> .
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="errorEventArgs"></param>
        private void OnError(object sender, ErrorEventArgs errorEventArgs)
        {
            InvokeOnMainThread(() => { new UIAlertView("Error", errorEventArgs.Message, null, "OK").Show(); } );
        }
    }
}
