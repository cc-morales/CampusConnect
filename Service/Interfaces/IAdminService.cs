using CamCon.Shared;
using Domain.Models;

namespace Service.Interfaces
{
    public interface IAdminService
    {
        Task<List<AdminAccountModel>> GetAllAdminsAsync();
        Task<Result> CreateAdminAsync(AdminAccountModel model);
        Task<Result> UpdateAccessControlAsync(string id, string[] accessControl);
    }
}

