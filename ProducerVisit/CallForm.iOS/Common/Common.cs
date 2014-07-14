namespace CallForm.iOS
{
    using MonoTouch.UIKit;
    using System;
    using System.Drawing;
    using System.Linq.Expressions;
    using System.Resources;

    public class Common
    {
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