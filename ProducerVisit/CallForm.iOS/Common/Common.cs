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
    public class CommonCore_iOS
    {
        static int _numberOfCallsToSetVisible = 0;

        /// <summary>Use for the background of controls: 230, 230, 255
        /// </summary>
        public static UIColor controlBackgroundColor = UIColor.FromRGB(230, 230, 255);

        /// <summary>Use for the background of views: 200, 200, 255
        /// </summary>
        public static UIColor viewBackgroundColor = UIColor.FromRGB(200, 200, 255);

        

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

        public void SetNetworkActivityIndicatorVisible(bool setVisible)
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
                CommonCore_iOS.DebugMessage("  [Common][snaiv] > SetNetworkActivityIndicatorVisible() was asked to hide more often than shown.");
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