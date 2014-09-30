namespace CallForm.iOS
{
    using MonoTouch.UIKit;
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq.Expressions;
    using System.Resources;

    /// <summary>Commonly used methods.
    /// </summary>
    /// <remarks>This class may be (partially) duplicated in other Projects.</remarks>
    public class Common_iOS
    {
        static int _numberOfCallsToSetVisible = 0;

        private static bool _isOS6 = false;

        public static bool IsMinimumOS6
        {
            get { return _isOS6; }
            set { _isOS6 = value; }
        }

        private static bool _isOS7 = false;

        public static bool IsMinimumOS7
        {
            get { return _isOS7; }
            set { _isOS7 = value; }
        }

        /// <summary>True if the App is running on an iPhone.
        /// </summary>
        /// <remarks>Note: for multiple UserInterfaceIdioms, Targeted Devices should be set to Universal.</remarks>
        public static bool UserInterfaceIdiomIsPhone
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Phone; }
        }

        /// <summary>True if the App is running on an iPad.
        /// </summary>
        /// <remarks>Note: for multiple UserInterfaceIdioms, Targeted Devices should be set to Universal.</remarks>
        public static bool UserInterfaceIdiomIsPad
        {
            get { return UIDevice.CurrentDevice.UserInterfaceIdiom == UIUserInterfaceIdiom.Pad; }
        }

        public Common_iOS()
        {
            IsMinimumOS6 = IsMinimumiOS6();
            IsMinimumOS7 = IsMinimumiOS7();
        }

        /// <summary>Use for the background of controls: 230, 230, 255
        /// </summary>
        public static UIColor controlBackgroundColor = UIColor.FromRGB(230, 230, 255);

        /// <summary>Use for the background of views: 200, 200, 255
        /// </summary>
        public static UIColor viewBackgroundColor = UIColor.FromRGB(200, 200, 255);


        /// <summary>Is this device running at least iOS 6.x?
        /// </summary>
        /// <returns>True if this is OS majorVersion is greater than 6.</returns>
        public static bool IsMinimumiOS6()
        {
            int minimumVersion = 5;
            bool minimumOS = false;
            string version = UIDevice.CurrentDevice.SystemVersion;
            string[] parts = version.Split('.');
            string major = parts[0];
            int majorVersion = SafeConvert(major, 0);

            if (majorVersion > minimumVersion)
            {
                minimumOS = true;
            }

            return minimumOS;
        }

        /// <summary>Is this device running at least iOS 7.x?
        /// </summary>
        /// <returns>True if this is OS majorVersion is greater than 7.</returns>
        public static bool IsMinimumiOS7()
        {
            int minimumVersion = 6;
            bool minimumOS = false;
            string version = UIDevice.CurrentDevice.SystemVersion;
            string[] parts = version.Split('.');
            string major = parts[0];
            int majorVersion = SafeConvert(major, 0);

            if (majorVersion > minimumVersion)
            {
                minimumOS = true;
            }

            return minimumOS;
        }

        // review: assigned but not used?
        //public static float topMarginPixels = 70;
        //public static double bannerHeightPercent = 12.5;
        //public static double controlHeightPercent = 5;
        //public static double controlWidthPercent = 31;
        //public static double leftControlOriginPercent = 1;

        /// <summary>Take the given information and write it to the iOS app's err.log.
        /// </summary>
        /// <param name="message">The message to write to the log.</param>
        [System.Diagnostics.Conditional("DEBUG")]
        private static void DebugMessage(string message)
        {
            Debug.WriteLine(message);
        }

        /// <summary>Take the given information and write it to the iOS app's err.log.
        /// </summary>
        /// <param name="message">The message to write to the log.</param>
        /// <param name="writeToConsole">If <c>True</c>, also write <paramref name="message"/> to the <c>System.Console</c>.</param>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void DebugMessage(string message, bool writeToConsole = false)
        {
            if (writeToConsole)
            { 
                System.Console.WriteLine(message); 
            }
            
            DebugMessage(message);
        }

        /// <summary>Take the given information and write it to the iOS app's err.log.
        /// </summary>
        /// <param name="declaringName">The name of the file that threw the error</param>
        /// <param name="methodName">The name of the method that threw the error.
        /// This is useful for things like the Master page.</param>
        /// <param name="writeToConsole">If <c>True</c>, also write <paramref name="message"/> to the <c>System.Console</c>.</param>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void DebugMessage(string declaringName, string methodName, bool writeToConsole = false)
        {
            string message = "Class: " + declaringName + ", Method: " + methodName + "()";
            DebugMessage(message, writeToConsole);
        }

        /// <summary>Take the given information and write it to the iOS app's err.log.
        /// </summary>
        /// <param name="declaringName">The name of the file that threw the error</param>
        /// <param name="methodName">The name of the method that threw the error</param>
        /// <param name="parentName">The URL of the page that threw the exception. 
        /// This is useful for things like the Master page.</param>
        /// <param name="writeToConsole">If <c>True</c>, also write <paramref name="message"/> to the <c>System.Console</c>.</param>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void DebugMessage(string declaringName, string methodName, string parentName, bool writeToConsole = false)
        {
            string message = "Class: " + declaringName + ", Method: " + methodName + "(), Parent: " + parentName;
            DebugMessage(message, writeToConsole);
        }

        public static void SetNetworkActivityIndicatorVisible(bool setVisible)
        {
            if (setVisible)
            {
                _numberOfCallsToSetVisible++;
            }
            else
            {
                _numberOfCallsToSetVisible--;
            }

            if (_numberOfCallsToSetVisible < 0)
            {
                _numberOfCallsToSetVisible = 0;
                Common_iOS.DebugMessage("  [Common][snaiv] > SetNetworkActivityIndicatorVisible() was asked to hide more often than shown.");
            }

            UIApplication.SharedApplication.NetworkActivityIndicatorVisible = (_numberOfCallsToSetVisible > 0);
        }

        public static T SafeConvert<T>(string s, T defaultValue)
        {
            if (string.IsNullOrEmpty(s))
                return defaultValue;
            return (T)Convert.ChangeType(s, typeof(T));
        }
    }



    //[Serializable()]
    //public class AppColor
    //{
    //    private int red;
    //    private int green;
    //    private int blue;

    //    public AppColor(int red, int green, int blue)
    //    {
    //        this.red = red;
    //        this.green = green;
    //        this.blue = blue;
    //    }

    //    public int Red
    //    {
    //        get { return this.red; }
    //    }

    //    public int Green
    //    {
    //        get { return this.green; }
    //    }

    //    public int Blue
    //    {
    //        get { return this.blue; }
    //    }
    //}

    //public class Example
    //{
    //    public static void NotMain()
    //    {
    //        // Instantiate an object.
    //        AppColor controlBackgroundColor = new AppColor(230, 230, 255);
    //        AppColor viewBackgroundColor = new AppColor(200, 200, 255);
    //        // Define a resource file named CarResources.resx. 
    //        using (ResXResourceWriter resx = new ResXResourceWriter(@".\ColorResources.resx"))
    //        {
    //            resx.AddResource("controlBackgroundColor", controlBackgroundColor);
    //            resx.AddResource("viewBackgroundColor", viewBackgroundColor);
    //        }
    //    }
    //}
}