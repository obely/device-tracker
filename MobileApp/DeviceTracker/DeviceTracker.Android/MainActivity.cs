using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using DeviceTracker.Droid.Services;
using DeviceTracker.Messages;
using Xamarin.Forms;

namespace DeviceTracker.Droid
{
    [Activity(Label = "DeviceTracker", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize,
        LaunchMode = LaunchMode.SingleTask)] // LaunchMode = LaunchMode.SingleTask is required for Auth0!
    [IntentFilter(
        new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "com.companyname.devicetracker",
        DataHost = "dev-0b4eotxb.us.auth0.com",
        DataPathPrefix = "/android/com.companyname.devicetracker/callback")]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            Xamarin.FormsMaps.Init(this, savedInstanceState);

            SetServiceMethods();
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            var goTo = intent.GetStringExtra("GoTo");
            if (!string.IsNullOrEmpty(goTo))
            {
                Shell.Current.GoToAsync($"//{goTo}");
                return;
            }

            Auth0.OidcClient.ActivityMediator.Instance.Send(intent.DataString);
        }

        private void SetServiceMethods()
        {
            MessagingCenter.Subscribe<StartRouteCommand>(this, nameof(StartRouteCommand), message =>
            {
                if (!IsServiceRunning(typeof(AndroidLocationService)))
                {
                    var serviceIntent = new Intent(this, typeof(AndroidLocationService));
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
                    {
                        StartForegroundService(serviceIntent);
                    }
                    else
                    {
                        StartService(serviceIntent);
                    }
                }
            });

            MessagingCenter.Subscribe<StopRouteCommand>(this, nameof(StopRouteCommand), message =>
            {
                if (IsServiceRunning(typeof(AndroidLocationService)))
                {
                    var serviceIntent = new Intent(this, typeof(AndroidLocationService));
                    StopService(serviceIntent);
                }
            });
        }

        private bool IsServiceRunning(System.Type cls)
        {
            ActivityManager manager = (ActivityManager)GetSystemService(ActivityService);
            foreach (var service in manager.GetRunningServices(int.MaxValue))
            {
                if (service.Service.ClassName.Equals(Java.Lang.Class.FromType(cls).CanonicalName))
                {
                    return true;
                }
            }
            return false;
        }
    }
}