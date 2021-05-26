## Building
Go to _\DeviceTracker\DeviceTracker_ folder, copy _appsettings.json.sample_ to _appsettings.json_ and provide the following parameters:
- Authentication:
    -   Domain - your Auth0 application domain
    -   ClientId - your Auth0 application client ID
    -   Audience - your Auth0 API identifier
- Service:
    -   Url - your HTTP API on AWS

Then open _DeviceTracker\DeviceTracker.Android\Properties\AndroidManifest.xml_ file and insert your Google Maps API Key:
```sh
<meta-data android:name="com.google.android.geo.API_KEY" android:value="Your Google Maps API Key" />
```