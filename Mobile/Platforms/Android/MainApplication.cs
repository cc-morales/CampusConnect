using Android.App;
using Android.Runtime;
using System;

namespace Mobile
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();
            try
            {
                Firebase.FirebaseApp.InitializeApp(ApplicationContext);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Firebase initialization error: {ex.Message}");
            }
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
