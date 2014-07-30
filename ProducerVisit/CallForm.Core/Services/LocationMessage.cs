using Cirrious.MvvmCross.Plugins.Messenger;

namespace CallForm.Core.Services
{
    // Review: see https://github.com/rafaelsteil/MvvmCross-Wiki/blob/master/MvvmCross-plugins.md#location 
    public class LocationMessage : MvxMessage
    {
        public LocationMessage(object sender, double lat, double lng)
            : base(sender)
        {
            Lng = lng;
            Lat = lat;
        }

        public double Lat { get; private set; }
        public double Lng { get; private set; }

    }
}
