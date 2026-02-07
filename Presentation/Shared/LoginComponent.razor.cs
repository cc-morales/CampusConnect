using Blazored.LocalStorage;
using Domain.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using Presentation.Authentication;
using Service.Interfaces;
using Service.Notifiers;
using Shared.Component.Helpers;

namespace Presentation.Shared
{
    public partial class LoginComponentBase : ComponentBase
    {   
        [Inject] protected ISnackbar _snackBar { get; set; } = default!;
        [Inject] protected AuthenticationStateProvider _authenticationStateProvider { get; set; } = default!;
        [Inject] protected IUserService _userService { get; set; } = default!;
        [Inject] protected ILocalStorageService _localStorage { get; set; } = default!;
        [Inject] protected HubNotificationService _hubNotificationService { get; set; } = default!;

        [Parameter] public EventCallback OnLoginSuccess { get; set; }

        protected string username = string.Empty, password = string.Empty;
        protected bool _open = false, isLoading = false, isShow = false;
        protected InputType PasswordInput = InputType.Password;
        protected string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;

        protected void ToggleDrawer()
        {
            _open = !_open;
        }
        protected async Task HandleKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await Login();
            }
        }
        protected async Task Login()
        {
            isLoading = true;
            await Task.Delay(500);
            StateHasChanged();
           
            try
            {
                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    SnackBarHelper.ShowSnackbar("All fields are required", Variant.Filled, _snackBar, Severity.Error);
                    return;
                }
                var uid = await _userService.Authenticate(new LoginModel()
                {
                    Username = username,
                    Password = password
                });

                await _localStorage.SetItemAsync("token", uid);

                await ((CustomAuthenticationState)_authenticationStateProvider).NotifyUserAuthentication(uid.AccessToken);

                await _hubNotificationService.StartAdminNotificationConnection();

                username = string.Empty;
                password = string.Empty;

                await OnLoginSuccess.InvokeAsync();
            }
            catch (Exception ex)
            {
                if (ex.Message.ToLower().Contains("invalid"))
                    SnackBarHelper.ShowSnackbar("Invalid email or password", Variant.Filled, _snackBar, Severity.Error);
                else
                    SnackBarHelper.ShowSnackbar(ex.Message, Variant.Filled, _snackBar, Severity.Error);
            }
            finally
            {
                isLoading = false;
            }
        }
        protected void ShowPassword()
        {
            if (isShow)
            {
                isShow = false;
                PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                PasswordInput = InputType.Password;
            }
            else
            {
                isShow = true;
                PasswordInputIcon = Icons.Material.Filled.Visibility;
                PasswordInput = InputType.Text;
            }
        }
    }
}