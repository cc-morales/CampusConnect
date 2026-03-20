using CamCon.Shared;
using CamCon.Shared.Extensions;
using Domain;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using Service.Interfaces;

namespace Service.Services.OrganizationServices
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IConfiguration _configuration;

        private readonly IBaseService _baseService;

        private readonly string defaultRequestUrl;

        private RequestModel request = new();

        public OrganizationService(IConfiguration configuration, IBaseService baseService)
        {
            _configuration = configuration;
            _baseService = baseService;
            request.RequestUrl = defaultRequestUrl = $"{_configuration["BaseAPI:Url"]}/api/org";
        }
        public async Task<Result> CreateOrganizationAsync(MyOrganizationModel model)
        {
            request.RequestUrl = $"{defaultRequestUrl}/create";
            request.RequestType = Enums.RequestType.POST;
            request.Data = model.Wrap("organization");

            var response = await _baseService.SendAsync<Result>(request);

            return response;
        }

        public Task<Result> DeleteOrganizationAsync(Guid organizationId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<MyOrganizationModel>> GetAllOrganizationsAsync()
        {
            request.RequestType = Enums.RequestType.GET;
            request.Data = null;
            request.RequestUrl = defaultRequestUrl;

            var response = await _baseService.SendAsync<List<MyOrganizationModel>>(request);

            return response;
        }

        public Task<MyOrganizationModel?> GetOrganizationByIdAsync(Guid organizationId)
        {
            throw new NotImplementedException();
        }

        public async Task<Result> UpdateOrganizationAsync(MyOrganizationModel model)
        {
            request.RequestType = Enums.RequestType.PUT;
            request.Data = model.Wrap("request");
            request.RequestUrl = defaultRequestUrl;

            var response = await _baseService.SendAsync<Result>(request);

            return response;
        }

        public async Task<Result> AddContributorAsync(Contributors contributor)
        {
            request.RequestUrl = $"{defaultRequestUrl}/contributor";
            request.RequestType = Enums.RequestType.POST;
            request.Data = contributor.Wrap("contributor");

            var response = await _baseService.SendAsync<Result>(request);

            return response;
        }
    }
}
