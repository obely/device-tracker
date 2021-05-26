This is a demo app that allows to record routes from a mobile device and store them in AWS.

## Key Components:
- REST API for storing data in AWS DynamoDB, protected with JWT authorizer and Auth0
- Xamarin.Forms application

## Installation
- Register an API and a Native Mobile Application in Auth0. Make sure the Mobile App has 'com.companyname.devicetracker://YOUR_DOMAIN/android/com.companyname.devicetracker/callback' value in 'Allowed Callback URLs' and 'Allowed Logout URLs' fields. Also the API should be configured to 'Allow Offline Access'.
- Obtain Google Maps API Key as per this [article](https://docs.microsoft.com/en-US/xamarin/android/platform/maps-and-location/maps/obtaining-a-google-maps-api-key?tabs=windows)
- Deploy the backend to AWS following instructions in _RestApi_ folder.
- Build and run the Mobile App from _MobileApp_ folder.