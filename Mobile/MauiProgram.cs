using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Mobile.Authentication;
using MudBlazor.Services;
using MudExtensions.Services;
using Service;
using Service.Interfaces;
using Service.Notifiers;
using Service.Services.BaseService;
using Service.Services.CommentServices;
using Service.Services.DepartmentServices;
using Service.Services.NewsFeedServices;
using Service.Services.NotificationServices;
using Service.Services.OrganizationServices;
using Service.Services.PageRequestServices;
using Service.Services.ProfileServices;
using Service.Services.TokenProviderServices;
using Service.Services.UserServices;
using System.Reflection;
using Microsoft.Maui.LifecycleEvents;
#if ANDROID
using Plugin.Firebase.Core.Platforms.Android;
#endif

namespace Mobile
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.RegisterFirebaseServices();

            builder.Services.AddMauiBlazorWebView();

            AddAppSetting(builder);

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif
            
            builder.Services.AddHttpClient<IBaseService, BaseService>()
                .ConfigureHttpClient(client =>
                {
                    client.Timeout = TimeSpan.FromMinutes(5);
                })
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var handler = new HttpClientHandler();
                    // WARNING: Only for development/testing!
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                    return handler;
                });

            builder.Services.AddMudServices();
            builder.Services.AddBlazoredLocalStorage();
            builder.Services.AddMudExtensions();
            builder.Services.AddMemoryCache();

            builder.Services.AddScoped<HubNotificationService>();
            builder.Services.AddScoped<AppStateService>();
            builder.Services.AddScoped<LayoutNotifierService>();
            builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationState>();

            builder.Services.AddScoped<IBaseService, BaseService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<ITokenProvider, TokenProvider>();
            builder.Services.AddScoped<IOrganizationService, OrganizationService>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<IProfileService, ProfileService>();
            builder.Services.AddScoped<IPageRequestService, PageRequestService>();
            builder.Services.AddScoped<INotificationService, NotificationService>();
            builder.Services.AddScoped<IFeedNotifier, FeedNotifier>();
            builder.Services.AddScoped<INewsFeedService, NewsFeedService>();
            builder.Services.AddScoped<ICommentService, CommentService>();

            builder.Services.AddAuthorizationCore();
            builder.Services.AddCascadingAuthenticationState();

            AddHttpCertificate();

            return builder.Build();
        }
        private static void AddHttpCertificate()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };

            var httpClient = new HttpClient(handler);
        }
        private static void AddAppSetting(MauiAppBuilder builder)
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Mobile.appsettings.mobile.json");

            if (stream != null)
            {
                builder.Configuration.AddJsonStream(stream);
            }

            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration["BaseAPI:Url"]!) });
        }
        
        private static void RegisterFirebaseServices(this MauiAppBuilder builder)
        {
            builder.ConfigureLifecycleEvents(events => {
#if ANDROID
                events.AddAndroid(android => android.OnCreate((activity, _) =>
                {
                    CrossFirebase.Initialize(activity, () => activity);
                }));
#endif
            });
        }
    }
}
