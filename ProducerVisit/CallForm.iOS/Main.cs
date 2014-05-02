using System;
using System.Collections.Generic;
using System.Linq;
//using CallForm.Core.ViewModels;
using Cirrious.CrossCore;


    using MonoTouch.Foundation;
    using MonoTouch.UIKit;

namespace CallForm.iOS
{
    public class Application
    {
        // This is the main entry point of the application.
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
