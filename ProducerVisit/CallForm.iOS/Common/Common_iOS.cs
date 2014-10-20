namespace CallForm.iOS
{
    using MonoTouch.UIKit;
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Resources;

    /// <summary>Commonly used methods.
    /// </summary>
    /// <remarks>This class may be (partially) duplicated in other Projects.</remarks>
    public class Common_iOS
    {
        // class-level declarations

        string _nameSpace1 = "CallForm.";

        /// <summary>Class name abbreviation
        /// </summary>
        string _cAbb = "[Common_iOS]";

        #region properties
        //private static bool _isOS7 = false;
        //public static bool IsMinimumOS7
        //{
        //    get { return _isOS7; }
        //    set { _isOS7 = value; }
        //}

        //private static bool _isOS8 = false;
        //public static bool IsMinimumOS8
        //{
        //    get { return _isOS8; }
        //    set { _isOS8 = value; }
        //}

        private static bool _iOSVersionOK = false;
        public static bool iOSVersionOK
        {
            get { return _iOSVersionOK; }
            set { _iOSVersionOK = value; }
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

        /// <summary>Use for the background of controls: 230, 230, 255
        /// </summary>
        public static UIColor controlBackgroundColor = UIColor.FromRGB(230, 230, 255);

        /// <summary>Use for the background of views: 200, 200, 255
        /// </summary>
        public static UIColor viewBackgroundColor = UIColor.FromRGB(200, 200, 255);

        // ToDo: implement text color(s) here
        #endregion properties

        public Common_iOS()
        {
            //iOSVersionOK = IsMinimumiOS(7);

            // Review: should screen control size information be implemented here? The idiom doesn't change once the app is running.
        }

        #region methods
        /// <summary>Is this device running at least iOS 8.bannerImage?
        /// </summary>
        /// <returns>True if this is OS majorVersion is greater than 8.</returns>
        private static bool IsMinimumiOS(int minimumVersion)
        {
            bool minimumOS = false;
            string version = UIDevice.CurrentDevice.SystemVersion;
            string[] parts = version.Split('.');
            string major = parts[0];
            int majorVersion = SafeConvert(major, 0);

            if (majorVersion >= minimumVersion)
            {
                minimumOS = true;
            }

            return minimumOS;
        }
        #endregion

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
            // ToDo: this method should *only* be toggling the network indicator in response to an instruction from one of the ViewModels.
            UIApplication.SharedApplication.NetworkActivityIndicatorVisible = setVisible;
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