using CamCon.Shared;
using Domain.Models;

namespace Service.Interfaces
{
    public interface IUserService
    {
        Task<TokenModel> Authenticate(LoginModel loginModel);
        Task<List<ApplicationUserModel>> GetAllUsers();
        Task<Result> CreateUser(ApplicationUserModel userModel);
        Task<ApplicationUserModel> GetUserById(string id);
        Task<Result> ChangePassword(ChangePasswordModel model);
    }
}
