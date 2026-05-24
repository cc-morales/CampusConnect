using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Mobile.Platforms.Android;
using Plugin.Firebase.CloudMessaging;

namespace Mobile
{
    //[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, WindowSoftInputMode = Android.Views.SoftInput.AdjustPan | Android.Views.SoftInput.StateHidden, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle? savedInstanceState)
        {
           // Theme?.ApplyStyle(Resource.Style.OptOutEdgeToEdgeEnforcement, force: false);

            base.OnCreate(savedInstanceState);
            AndroidX.Activity.EdgeToEdge.Enable(this);

            WebViewSoftInputPatch.Initialize();
            CreateNotificationChannelIfNeeded();

            if (OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                if (CheckSelfPermission(Android.Manifest.Permission.PostNotifications) != Permission.Granted)
                {
                    RequestPermissions([Android.Manifest.Permission.PostNotifications], 1001);
                }
            }
        }
        
        protected override void OnNewIntent(Intent? intent)
        {
            base.OnNewIntent(intent);
            
            if (intent != null) HandleIntent(intent);
        }
        private static void HandleIntent(Intent intent)
        {
            FirebaseCloudMessagingImplementation.OnNewIntent(intent);
        }
        private void CreateNotificationChannelIfNeeded()
        {
            // Platform compatibility: guarded by version check
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                CreateNotificationChannel();
            }
        }
        private void CreateNotificationChannel()
        {
            // Platform compatibility: NotificationChannel requires Android O (API 26)+
            if (OperatingSystem.IsAndroidVersionAtLeast(26))
            {
                var channelId = $"{PackageName}.general";
                var notificationManager = (NotificationManager)GetSystemService(NotificationService)!;
                var channel = new NotificationChannel(channelId, "General", NotificationImportance.Default);
                notificationManager.CreateNotificationChannel(channel);
                FirebaseCloudMessagingImplementation.ChannelId = channelId;
            }
        }   
    }
}
