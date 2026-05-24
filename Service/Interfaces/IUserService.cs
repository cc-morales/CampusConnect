using CamCon.Shared;
using Domain.Models;

namespace Service.Interfaces
{
    public interface IUserService
    {
        Task<TokenModel> Authenticate(LoginModel loginModel);
        Task<TokenModel> LoginAsGuest();
        Task<List<ApplicationUserModel>> GetAllUsers();
        Task<Result> CreateUser(ApplicationUserModel userModel);
        Task<ApplicationUserModel> GetUserById(string id);
        Task<Result> ChangePassword(ChangePasswordModel model);
        Task<Result> VerifyEmailAsync(string email, string code);
        Task<Result> ResendVerificationCodeAsync(string email);
        Task<Result> DeleteUserAsync(string userId);
        Task<Result> DisableUserAsync(string userId, bool isDisabled);
        Task<Result> RequestPasswordResetAsync(string email, string baseUrl);
        Task<Result> ResetPasswordAsync(string guid, string newPassword, string confirmPassword);
    }
}
