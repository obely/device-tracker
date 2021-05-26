using Android.App;
using Android.Content;
using Android.OS;
using AndroidX.Core.App;
using DeviceTracker.Messages;
using DeviceTracker.Services;
using DeviceTracker.Views;
using System.Threading;
using Xamarin.Forms;
using static Android.OS.PowerManager;

namespace DeviceTracker.Droid.Services
{
    [Service]
    public class AndroidLocationService : Service
    {
        private const int SERVICE_RUNNING_NOTIFICATION_ID = 1;
        private const string ActionStop = "com.companyname.devicetracker.LOCATIONSERVICE_STOP";

        private readonly ILocationService _locationService;
        private WakeLock _wakeLock;
        private CancellationTokenSource _cts;

        public AndroidLocationService()
        {
            _locationService = DependencyService.Get<ILocationService>();
        }

        public override IBinder OnBind(Intent intent) => null;

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            switch (intent.Action)
            {
                case ActionStop:
                    {
                        MessagingCenter.Send(new StopRouteCommand(), nameof(StopRouteCommand));
                        break;
                    }
                default:
                    {
                        var channelId = Build.VERSION.SdkInt >= BuildVersionCodes.O ? CreateNotificationChannel() : string.Empty;

                        var notification = new NotificationCompat.Builder(this, channelId)
                            .SetContentTitle(Resources.GetString(Resource.String.app_name))
                            .SetContentText(Resources.GetString(Resource.String.location_service_notification_text))
                            .SetSmallIcon(Resource.Drawable.abc_ic_star_black_48dp)
                            .SetContentIntent(BuildIntentToShowMainActivity())
                            .AddAction(Resource.Drawable.ic_stop_black_18dp, Resources.GetString(Resource.String.action_stop), BuildStopAction())
                            .SetOngoing(true)
                            .Build();

                        // Enlist this instance of the service as a foreground service
                        StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification);

                        Start();

                        break;
                    }
            }

            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            Stop();
            base.OnDestroy();
        }

        private string CreateNotificationChannel()
        {
            var channelId = Resources.GetString(Resource.String.app_name);
            var channelName = Resources.GetString(Resource.String.app_name);

            var channel = new NotificationChannel(channelId, channelName, NotificationImportance.Low);

            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
            return channelId;
        }

        private PendingIntent BuildIntentToShowMainActivity()
        {
            var intent = new Intent(Intent.ActionView, null, this, typeof(MainActivity));
            intent.PutExtra("GoTo", nameof(MapPage));

            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.UpdateCurrent);
            return pendingIntent;
        }

        private PendingIntent BuildStopAction()
        {
            var pendingIntent = PendingIntent.GetService(this, 0, new Intent(ActionStop, null, this, typeof(AndroidLocationService)), PendingIntentFlags.UpdateCurrent);
            return pendingIntent;
        }

        private void Start()
        {
            var powerManager = (PowerManager)GetSystemService(PowerService);
            _wakeLock = powerManager.NewWakeLock(WakeLockFlags.Full, nameof(LocationService));
            _wakeLock.SetReferenceCounted(false);
            _wakeLock.Acquire();

            _cts = new CancellationTokenSource();
            _locationService.Start(_cts.Token);
        }

        private void Stop()
        {
            _wakeLock?.Release();
            _cts?.Cancel();
        }
    }
}