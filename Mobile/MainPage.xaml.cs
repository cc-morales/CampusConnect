namespace Mobile
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        
        // protected override async void OnAppearing()
        // {
        //     try
        //     {
        //         base.OnAppearing();
        //         await SendFcmTokenAsync();
        //     }
        //     catch (Exception e)
        //     {
        //         //Ignore
        //     }
        // }
        //
        // private async Task SendFcmTokenAsync()
        // {
        //     try
        //     {
        //         await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
        //         var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
        //
        //         await _notificationService.RegisterFCM(token);
        //
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"FCM error: {ex.Message}");
        //     }
        // }
    }
}
