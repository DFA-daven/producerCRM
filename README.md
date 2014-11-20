# producerCRM
producerCRM is a mobile app for logging staff contact with customers.

## Platforms
Currently, producerCRM is only targeting iOS, and specifically only iPad 2+ devices. 

## Technology
The producerCRM app is written in C#. Native iOS code is provided by the Xamarin Studio IDE. 

_See also: **MonoTouch**; **Xcode**._

## Enhancements
### Functionality
* differentiate manager and regular users
* office
    * reports
    * ability to edit entries

### Platforms
#### Browser
The app accesses with the backend database via a web service. The functionality provided by the app (create an entry; search existing entries) needs to also be available to staff working from a desk. This will require an additional View (M **V** VM).

#### iPhone
The app Views are currently designed/intended to be used only with iPads. It will run on iPhones (4S or higher), however the layout of the Views frequently cuts off controls rendering the UI useless. _Addressing this issue may invlove substantial changes to the View architecture._ One possible solution is to refactor the Views to use **Xamarin.Forms**.

#### Android / Windows devices
Assuming the changes necessary to run the app on an iPhone are completed, it should be straight-forward to port the app to Android. Likewise, porting to Windows phones and tablets should also be possible via the Xamarin architecture.
