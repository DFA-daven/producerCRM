﻿using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Location;
using Cirrious.MvvmCross.Plugins.Messenger;

namespace CallForm.Core.Services
{
    public class LocationService : ILocationService
    {
        // note: replaced deprecated IMvxGeoLocationWatcher with IMvxLocationWatcher

        private readonly IMvxLocationWatcher _watcher;
        private readonly IMvxMessenger _messenger;

        public LocationService(IMvxLocationWatcher watcher, IMvxMessenger messenger)
        {
            _messenger = messenger;

            _watcher = watcher;
            _watcher.Start(new MvxLocationOptions(), OnSuccess, OnError);
        }

        private readonly object _lockObject = new object();
        private MvxGeoLocation _latestLocation;

        private void OnSuccess(MvxGeoLocation location)
        {
            lock (_lockObject)
            {
                _latestLocation = location;
            }

            var message = new LocationMessage(this,
                                location.Coordinates.Latitude,
                                location.Coordinates.Longitude);

            _messenger.Publish(message);
        }

        private void OnError(MvxLocationError error)
        {
            Mvx.Warning("Error seen during location {0}", error.Code);
        }

        public bool TryGetLatestLocation(out double lat, out double lng)
        {
            lock (_lockObject)
            {
                if (_latestLocation == null)
                {
                    lat = lng = 0;
                    return false;
                }

                lat = _latestLocation.Coordinates.Latitude;
                lng = _latestLocation.Coordinates.Longitude;
                return true;
            }
        }
    }
}
