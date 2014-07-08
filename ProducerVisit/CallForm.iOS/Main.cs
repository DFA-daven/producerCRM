using Cirrious.CrossCore;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.Collections.Generic;
using System.Linq;

//using CallForm.Core.ViewModels;

// note: check which of these "usings" are required. 
// MonoTouch.UIKit is the only one needed to build, but the application doesn't work without the others. 
// This could either be an issue that Clean/Rebuild could fix, or it may be that the "usings" loaded here
// are inherited by the App.
    


namespace CallForm.iOS
{
    /// <summary>Defines the application's entry point.
    /// </summary>
    public class Application
    {
        // This is the main entry point of the application. Control moves from here to AppDelegate.
        static void Main(string[] args)
        {
            bool hostWasReachable = false;

            if (!Reachability.IsHostReachable("http://ProducerCRM.DairyDataProcessing.com"))
            {
                hostWasReachable = false;

                //Mvx.Error("Host not reachable.");
                //Error(this, new ErrorEventArgs { Message = "Host not reachable." });
                
                // if you want to use a different Application Delegate class from "AppDelegate"
                // you can specify it here.
                UIApplication.Main(args, null, "AppDelegate"); 
            }
            else
            {
                hostWasReachable = true;
                
                UIApplication.Main(args, null, "AppDelegate");
            }
        }

        //public event EventHandler<ErrorEventArgs> Error;

        //public event EventHandler SendEmail;
    }

    ///// <summary>An instance of an error event.
    ///// </summary>
    //public class ErrorEventArgs : EventArgs
    //{
    //    /// <summary>The message to display on the error pop-up.
    //    /// </summary>
    //    public string Message { get; set; }
    //}
}
