using Android.App;
using Android.Content.PM;
using Android.OS;
using Mobile.Platforms.Android;

namespace Mobile
{
    //[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, WindowSoftInputMode = Android.Views.SoftInput.AdjustPan | Android.Views.SoftInput.StateHidden, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
           // Theme?.ApplyStyle(Resource.Style.OptOutEdgeToEdgeEnforcement, force: false);

            base.OnCreate(savedInstanceState);
            AndroidX.Activity.EdgeToEdge.Enable(this);
            WebViewSoftInputPatch.Initialize();
        }
    }
}
