using Blazored.LocalStorage;
using Domain.Constants;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Extensions;
using MudBlazor.Services;
using Presentation;
using Presentation.Authentication;
using Service;
using Service.Interfaces;
using Service.Notifiers;
using Service.Services.AdminServices;
using Service.Services.BaseService;
using Service.Services.CommentServices;
using Service.Services.DepartmentServices;
using Service.Services.NewsFeedServices;
using Service.Services.NotificationServices;
using Service.Services.OrganizationServices;
using Service.Services.PageRequestServices;
using Service.Services.SentimentServices;
using Service.Services.TokenProviderServices;
using Service.Services.UserServices;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.Configuration.GetValue<string>("BaseAPI:Url")) });

builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddMudServicesWithExtensions();
builder.Services.AddMemoryCache();

builder.Services.AddHttpClient();

builder.Services.AddSingleton<HubNotificationService>();
builder.Services.AddSingleton<LayoutNotifierService>();
builder.Services.AddSingleton<AppStateService>();
builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenProvider, TokenProvider>();
builder.Services.AddScoped<IOrganizationService, OrganizationService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<INotificationService, NotificationService>();
builder.Services.AddScoped<IPageRequestService, PageRequestService>();
builder.Services.AddScoped<IGeminiService, GeminiService>();
builder.Services.AddScoped<ISentimentService, SentimentService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<INewsFeedService, NewsFeedService>();
builder.Services.AddScoped<IAdminService, AdminService>();

builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationState>();

builder.Services.AddAuthorizationCore(options =>
{
    // Register a policy for each page-level permission.
    // Each policy checks that the user's "access" claim contains the permission name.
    foreach (var permission in AdminPermissions.All)
    {
        options.AddPolicy(permission, policy =>
            policy.RequireAssertion(context =>
            {
                var accessClaim = context.User.FindFirst("access");
                if (accessClaim is null) return false;
                var permissions = accessClaim.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                return permissions.Contains(permission, StringComparer.OrdinalIgnoreCase);
            }));
    }
});

await builder.Build().RunAsync();
