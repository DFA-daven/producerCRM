using Cirrious.MvvmCross.Plugins.Messenger;

namespace CallForm.Core.Services
{
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
