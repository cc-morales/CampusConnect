using CamCon.Shared;
using CamCon.Shared.Extensions;
using Domain;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using Service.Interfaces;

namespace Service.Services.AdminServices
{
    public class AdminService : IAdminService
    {
        private readonly IBaseService _baseService;
        private readonly string _baseUrl;
        private RequestModel _request = new();

        public AdminService(IConfiguration configuration, IBaseService baseService)
        {
            _baseService = baseService;
            _baseUrl = $"{configuration["BaseAPI:Url"]}/api/admin";
        }

        public async Task<List<AdminAccountModel>> GetAllAdminsAsync()
        {
            _request.RequestUrl = _baseUrl;
            _request.RequestType = Enums.RequestType.GET;
            _request.Data = null;

            return await _baseService.SendAsync<List<AdminAccountModel>>(_request);
        }

        public async Task<Result> CreateAdminAsync(AdminAccountModel model)
        {
            _request.RequestUrl = $"{_baseUrl}/create";
            _request.RequestType = Enums.RequestType.POST;
            _request.Data = new
            {
                model.Name,
                model.Email,
                model.Password,
                model.AccessControl
            };

            return await _baseService.SendAsync<Result>(_request);
        }

        public async Task<Result> UpdateAccessControlAsync(string id, string[] accessControl)
        {
            _request.RequestUrl = $"{_baseUrl}/access";
            _request.RequestType = Enums.RequestType.PUT;
            _request.Data = new
            {
                Id = id,
                AccessControl = accessControl
            };

            return await _baseService.SendAsync<Result>(_request);
        }
    }
}

