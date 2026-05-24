using Blazored.LocalStorage;
using Domain.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Service;
using Service.Interfaces;
using Service.Notifiers;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Plugin.Firebase.CloudMessaging;

namespace Mobile.Authentication
{
    public class CustomAuthenticationState(
        ILocalStorageService localStorage,
        IJSRuntime jsRuntime,
        IUserService userService,
        AppStateService appStateService,
        INotificationService notificationService)
        : AuthenticationStateProvider
    {
 

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (jsRuntime is IJSInProcessRuntime)
            {
                //do nothing
            }
            else if (jsRuntime is null)
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
            try
            {
                var currentToken = await localStorage.GetItemAsync<TokenModel>("token");

                if (currentToken is not null)
                {
                    var claims = ParseClaimsFromJwt(currentToken.AccessToken);
                    var expClaim = claims?.FirstOrDefault(c => c.Type == "exp")?.Value;
                    if (expClaim != null && long.TryParse(expClaim, out long exp))
                    {
                        var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp);
                        if (expirationTime < DateTimeOffset.UtcNow)
                        {
                            await localStorage.RemoveItemAsync("token");
                            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                        }
                    }
                    await GetAccount(currentToken.AccessToken);

                    var identity = new ClaimsIdentity(claims, "jwt");

                    return new AuthenticationState(new ClaimsPrincipal(identity));
                }
            }
            catch
            {
               
            }

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        private IEnumerable<Claim>? ParseClaimsFromJwt(string jwt)
        {
            var payload = jwt.Split('.')[1];
            var jsonBytes = ParseBase64WithoutPadding(payload);
            var keyValuePairs = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBytes);

            return keyValuePairs?.Select(kvp => new Claim(kvp.Key, kvp.Value.ToString()!));
        }

        private byte[] ParseBase64WithoutPadding(string base64)
        {
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            return Convert.FromBase64String(base64);
        }
        public async Task NotifyUserAuthentication(string token)
        {
            var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));

            await GetAccount(token);

            var authState = Task.FromResult(new AuthenticationState(authenticatedUser));

            NotifyAuthenticationStateChanged(authState);
        }
        public void NotifyUserLogout()
        {
            var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(anonymousUser));
            NotifyAuthenticationStateChanged(authState);
        }

        private async Task GetAccount(string token)
        {
            try
            {
                var authenticatedUser = new ClaimsPrincipal(new ClaimsIdentity(ParseClaimsFromJwt(token), "jwt"));

                var userId = authenticatedUser.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

                var user = await userService.GetUserById(userId!);
                
                await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
                var currentFcmToken = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();

                await notificationService.RegisterFcm(currentFcmToken, userId);

                appStateService.CurrentUser = user;

            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
