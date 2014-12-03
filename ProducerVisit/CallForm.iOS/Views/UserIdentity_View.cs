namespace CallForm.iOS.Views
{
    using CallForm.Core.ViewModels;
    using Cirrious.MvvmCross.Binding.BindingContext;
    using Cirrious.MvvmCross.Touch.Views;
    using MonoTouch.UIKit;
    using System;
    using System.Drawing;
    //using System.Reflection;
    using XibFree;

    // notes: see _Touch UI.txt for design details.

    // FixMe: the UserIdentity view does not implement modal correctly. It looks like the foundation is here, but it was never implemented.
    //public class UserIdentityView : MvxBindingTouchViewController<MyViewModel>, IMvxModalTouchView
    // visit http://stackoverflow.com/questions/14518876/integrating-google-mobile-analytics-with-mvvmcross for examples of getting this to work.

    /// <summary>Creates a class representing the UI View for "UserIdentity". 
    /// </summary>
    /// <remarks><para>This view inherits from <see cref="MvxViewController"/>. The main over-ridable methods 
    /// here handle the view's life-cycle.</para>
    /// <para>Design goal is to only deal with formatting and layout of data here -- no state information.</para></remarks>
    public class UserIdentity_View : MvxViewController, IMvxModalTouchView
    {
        #region Properties
        string _namespace = "CallForm.iOS.Views.";
		string _class = "UserIdentity_View.";
		string _method = "TBD";

        /// <summary>Store for the <c>iPhoneIdiom</c> property.</summary>
        private bool _iPhoneIdiom;
        /// <summary>Keeps track of if this is an iPhone (vs iPad).
        /// </summary>
        /// <remarks>This is a property that SHOULD be in the ViewController.</remarks>
        public bool iPhoneIdiom
        {
            get { return _iPhoneIdiom; }
            set
            {
                _iPhoneIdiom = value;
            }
        }

        /// <summary>Store for the <c>Portrait</c> property.</summary>
        private bool _portrait;
        /// <summary>Keeps track of portrait (or landscape) orientation.
        /// </summary>
        /// <remarks>This is a property that SHOULD be in the ViewController.</remarks>
        public bool Portrait
        {
            get { return _portrait; }
            set
            {
                // Undone: implement this in order to track orientation changes.
                _portrait = value;
                //RaisePropertyChanged(() => Height);
                //RaisePropertyChanged(() => Width);
                //RaisePropertyChanged(() => RowHeight);
            }
        }
        #endregion

        public UserIdentity_View()
        {
            //NavBarHeight = FindNavBarHeight();

            bool iPhoneIdiom = Common_iOS.UserInterfaceIdiomIsPhone;
        }

        #region overrides
        #pragma warning disable 1591
        public override void ViewDidLoad()
        {
			_method = "ViewDidLoad";
	  // Common_iOS.DebugMessage(_namespace + _class, _method); 

            // Note: this took a while to find...
            NavigationItem.SetHidesBackButton(true, false);

            // if there's already a Title, use it. Otherwise set it to "User Settings".
            // ToDo: get the title from the ViewReports_View.
            //NavigationItem.Title = NavigationItem.Title ?? appName;

            //var statusBarHidden = UIApplication.SharedApplication.StatusBarHidden;

            //InvokeOnMainThread(() => { new UIAlertView("starting method...", MethodBase.GetCurrentMethod().DeclaringType.Name + "." + MethodBase.GetCurrentMethod().Name, null, "OK").Show(); });

            #region controls

            // instructions for the inputs
            var instructions = new UILabel
            {
                Text = "Please enter your Email Address and Asset Tag (if any).",
                TextColor = UIColor.White,

                BackgroundColor = Common_iOS.viewBackgroundColor,
                //BackgroundColor = Common_iOS.controlBackgroundColor,
                //BackgroundColor = UIColor.Green,
                
                // Note: this took a while to find...
                // allow text to wrap in the control
                LineBreakMode = UILineBreakMode.WordWrap,
                Lines = 0
            };

            // the email address field
            var email = new UITextField
            {
                // leading and trailing space
                Placeholder = " example@dfamilk.com ",
                BackgroundColor = Common_iOS.controlBackgroundColor,
                KeyboardType = UIKeyboardType.EmailAddress,
            };

            // asset tag field
            var assetTag = new UITextField
            {
                // leading and trailing space
                Placeholder = " check back of device ",
                BackgroundColor = Common_iOS.controlBackgroundColor,
                KeyboardType = UIKeyboardType.Default,
            };

            // ok button
            var okButton = new UIButton(UIButtonType.System);
            okButton.SetTitle("OK", UIControlState.Normal);
            okButton.SetTitle("OK", UIControlState.Disabled);
            okButton.SetTitleColor(UIColor.Gray, UIControlState.Disabled);
            okButton.BackgroundColor = Common_iOS.controlBackgroundColor;
            //okButton.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleBottomMargin;
            okButton.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            
            #region files
            bool file1OK, file2OK, file3OK;
            file1OK = file2OK = file3OK = false;
            UIColor badColor = UIColor.Red;
            UIColor goodColor = UIColor.Green;

            // ToDo
            badColor = UIColor.LightGray;

            //if ((ViewModel as NewVisit_ViewModel).ListOfCallTypes.Count > 0)
            //{
            //    file1OK = true;
            //}

            //if ((ViewModel as NewVisit_ViewModel).ListOfEmailAddresses.Count > 0)
            //{
            //    file2OK = true;
            //}

            //if ((ViewModel as NewVisit_ViewModel).ListOfReasonCodes.Count > 0)
            //{
            //    file3OK = true;
            //}

            var file1 = new UIButton(UIButtonType.System);
#if (DEBUG)
            file1.SetTitle("Call", UIControlState.Normal);
            file1.SetTitle("Call", UIControlState.Disabled);
#endif
            file1.SetTitleColor(UIColor.Gray, UIControlState.Disabled);
            file1.BackgroundColor = file1OK ? goodColor : badColor;

            var file2 = new UIButton(UIButtonType.System);
#if (DEBUG)
            file2.SetTitle("Email", UIControlState.Normal);
            file2.SetTitle("Email", UIControlState.Disabled);
#endif
            file2.SetTitleColor(UIColor.Gray, UIControlState.Disabled);
            file2.BackgroundColor = file2OK ? goodColor : badColor;

            var file3 = new UIButton(UIButtonType.System);
#if (DEBUG)
            file3.SetTitle("Reason", UIControlState.Normal);
            file3.SetTitle("Reason", UIControlState.Disabled);
#endif
            file3.SetTitleColor(UIColor.Gray, UIControlState.Disabled);
            file3.BackgroundColor = file3OK ? goodColor : badColor;
            #endregion files

            //// ToDo: can the _logoButton be shared?
            //var testVRV = new ViewReports_View();
            //bool valueFromVRV = testVRV.IsOS8OrLater;

            #endregion controls

            #region layout
            float shortButtonHeight = (float)Math.Round((UIFont.SystemFontSize * 2f), 0);
            float minimumHeightFromPercent = percentHeight(0.5);
            float fileButtonHeight = minimumHeightFromPercent;

            //float minimumWidthUnit = percentWidth(0.5);
            double controlWidthPercent = 50;
            double fileControlHorzSpacingPercent = 5;
            double fileControlVertSpacingPercent = 5;

            double pushDownPercent = 5;

            if (Common_iOS.UserInterfaceIdiomIsPhone)
            {
                fileButtonHeight = fileButtonHeight * 2;
                controlWidthPercent = 75;
                pushDownPercent = 1;
                fileControlVertSpacingPercent = 2;
            }

            float minHeightSpace = Math.Min(percentHeight(fileControlVertSpacingPercent), shortButtonHeight);
            float maxHeightSpace = Math.Max(percentHeight(fileControlVertSpacingPercent), shortButtonHeight);

            double fileControlWidthPercent = (controlWidthPercent - (fileControlHorzSpacingPercent * 2)) / 3;

            #region file status layout
            var fileStatusLayout = new LinearLayout(Orientation.Horizontal)
            {
                Gravity = Gravity.TopCenter,
                Spacing = percentWidth(fileControlHorzSpacingPercent),
                SubViews = new View[]
                {
                    new NativeView
                    {
                        View = file1,
                        LayoutParameters = new LayoutParameters()
                        {
                            Width = percentWidth(fileControlWidthPercent),
                            Height = fileButtonHeight,
                            Weight = 3,

                            Gravity = Gravity.TopLeft ,
                        }
                    },
                    new NativeView
                    {
                        View = file2,
                        LayoutParameters = new LayoutParameters()
                        {
                            Width = percentWidth(fileControlWidthPercent),
                            Height = fileButtonHeight,
                            Weight = 2,

                            Gravity = Gravity.TopCenter ,
                        }
                    },
                    new NativeView
                    {
                        View = file3,
                        LayoutParameters = new LayoutParameters()
                        {
                            Width = percentWidth(fileControlWidthPercent),
                            Height = fileButtonHeight,
                            Weight = 1,

                            Gravity = Gravity.TopRight ,
                        }
                    },
                },
                LayoutParameters = new LayoutParameters()
                {
                    Width = percentWidth(controlWidthPercent),
                    //Height = minHeightSpace + minHeightSpace,
                    Gravity = Gravity.TopCenter,
                    //MaxHeight = minHeightSpace * 3,
                }
            };

            // wrap the LinearLayout in a UIView
            var fileStatusView = new UIView();
            fileStatusView = new UILayoutHost(fileStatusLayout)
            {
                BackgroundColor = UIColor.DarkGray,
                //BackgroundColor = Common_iOS.viewBackgroundColor,
            };

            //fileStatusView.SizeToFit();   // tightly enclose the sub-views
            #endregion

            //var textMargins = new UIEdgeInsets(0f, 15f, 0f, 4f);

            // this view has an array of sub-views
            var pageLayout = new LinearLayout(Orientation.Vertical)
            {
                Gravity = Gravity.TopCenter,
                Padding = new UIEdgeInsets(4f, 6f, 4f, 0f),
                Spacing = minHeightSpace,
                SubViews = new View[]
                {
                    // this "view" pushes the others down the page
                    new NativeView
                    {
                        View = new UIView()
                        {
                            BackgroundColor = UIColor.Yellow,
                        },
                        LayoutParameters = new LayoutParameters()
                        {
                            Width = percentWidth(controlWidthPercent),
                            Height = percentHeight(pushDownPercent),
                            Weight = 7f,
                            Gravity = Gravity.TopCenter,
                        }
                    },

// ToDo: remove for production
                    new NativeView
                    {
                        View = fileStatusView,
                        LayoutParameters = new LayoutParameters()
                        {
                            Width = percentWidth(controlWidthPercent),
                            Height = minHeightSpace * 2,
                            Weight = 6,
                            Gravity = Gravity.TopCenter ,
                        },
                    },

                    new NativeView
                    {
                        View = instructions,
                        LayoutParameters = new LayoutParameters()
                        {
                            //Width = AutoSize.FillParent,  
                            Width = percentWidth(controlWidthPercent),
                            MaxHeight = maxHeightSpace * 3,
                            //MinHeight = maxHeightSpace + maxHeightSpace,
                            Weight = 7f,
                            //Margins = textMargins,
                            Gravity = Gravity.TopCenter,
                        },
                    },

                    new NativeView
                    {
                        View = email,
                        LayoutParameters = new LayoutParameters()
                        {
                            Width = percentWidth(controlWidthPercent),
                            Height = shortButtonHeight,
                            Weight = 4,
                            //Margins = textMargins,
                            Gravity = Gravity.TopCenter,
                        }, 
                    },

                    new NativeView
                    {
                        View = assetTag,
                        LayoutParameters = new LayoutParameters()
                        {
                            Width = percentWidth(controlWidthPercent),
                            Height = shortButtonHeight,
                            Weight = 3,
                            Gravity = Gravity.TopCenter,
                        },
                    },
                    new NativeView
                    {
                        View = okButton,
                        LayoutParameters = new LayoutParameters()
                        {
                            Width = percentWidth(fileControlHorzSpacingPercent * 3),
                            Height = shortButtonHeight,
                            Weight = 2,
                            Gravity = Gravity.TopCenter,
                        },
                        
                    },

                    // this "view" pushes the others up to the top of the page
                    new NativeView
                    {
                        View = new UIView()
                        {
                            BackgroundColor = UIColor.Yellow,
                        },
                        LayoutParameters = new LayoutParameters()
                        {
                            Width = percentWidth(controlWidthPercent),
                            Height = percentHeight(pushDownPercent),
                            Weight = 1,
                            Gravity = Gravity.TopCenter,
                        }
                    },
                },
                LayoutParameters = new LayoutParameters()
                {
                    // these are the parameters for pageLayout
                    Gravity = Gravity.TopCenter,
                }
            };

            View = new UILayoutHost(pageLayout)
            {
                BackgroundColor = Common_iOS.viewBackgroundColor,
            };
            #endregion layout


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

            // Note: this BindingDescriptionSet represents the link between the UserIdentity_View and the UserIdentity_ViewModel.
            var set = this.CreateBindingSet<UserIdentity_View, UserIdentity_ViewModel>();
            set.Bind(email).To(vm => vm.UserEmail);
            set.Bind(assetTag).To(vm => vm.AssetTag);
            set.Bind(okButton).To(vm => vm.SaveCommand);
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
                okButton.BecomeFirstResponder();
                return true;
            };

            // keyboard should disappear if user taps outside of a text-box
            // Note: this works on this view b/c 1) there is open space, and 2) the only controls are text-box or button.
            var goAway = new UITapGestureRecognizer(() => View.EndEditing(true));
            View.AddGestureRecognizer(goAway);
            #endregion UI action

	  // Common_iOS.DebugMessage(_namespace + _class, _method);
            // Common_iOS.DebugMessage("  [ui_v][vdl] > ...finished method.");
        }

        public override void ViewDidLayoutSubviews()
        {
			_method = "ViewDidLayoutSubviews";
	  // Common_iOS.DebugMessage(_namespace + _class, _method);            

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
        #pragma warning restore 1591
        #endregion overrides

        #region layout properties
        private static float _navBarHeight = 0f;
        public static float NavBarHeight
        {
            get { return _navBarHeight; }
            set { _navBarHeight = value; }
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
        #endregion

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
