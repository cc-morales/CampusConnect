using CamCon.Shared;
using Domain;
using Domain.Models;
using Microsoft.Extensions.Configuration;
using Service.Interfaces;

namespace Service.Services.NotificationServices
{
    public class NotificationService : INotificationService
    {
        private readonly IConfiguration _configuration;

        private readonly IBaseService _baseService;

        private readonly string defaultRequestUrl;

        private RequestModel request = new();

        public NotificationService(IConfiguration configuration, IBaseService baseService)
        {
            _configuration = configuration;
            _baseService = baseService;
            request.RequestUrl = defaultRequestUrl = $"{_configuration["BaseAPI:Url"]}/api/notify";
        }

        public async Task<NotifyModel> GetByIdAsync(Guid notifyId)
        {
            request.RequestUrl = $"{defaultRequestUrl}/{notifyId}";
            request.RequestType = Enums.RequestType.GET;
            request.Data = null;

            var response = await _baseService.SendAsync<NotifyModel>(request);

            return response;
        }

        public async Task<List<NotifyModel>> GetAll()
        {
            request.RequestType = Enums.RequestType.GET;
            request.Data = null;

            var response = await _baseService.SendAsync<List<NotifyModel>>(request);

            return response;
        }

        public async Task<List<NotifyModel>> GetByRecipientAsync(string recipientId)
        {
            request.RequestUrl = $"{defaultRequestUrl}/recipient/{recipientId}";
            request.RequestType = Enums.RequestType.GET;
            request.Data = null;

            var response = await _baseService.SendAsync<List<NotifyModel>>(request);

            return response;
        }

        public async Task<Result> DeleteNotificationAsync(Guid notifyId)
        {
            request.RequestUrl = $"{defaultRequestUrl}/{notifyId}";
            request.RequestType = Enums.RequestType.DELETE;
            request.Data = null;

            return await _baseService.SendAsync<Result>(request);
        }

        public async Task<Result> ClearAllNotificationsAsync(string recipientUserId)
        {
            request.RequestUrl = $"{defaultRequestUrl}/clear/{recipientUserId}";
            request.RequestType = Enums.RequestType.DELETE;
            request.Data = null;

            return await _baseService.SendAsync<Result>(request);
        }

        public async Task<Result> ClearAllAsync()
        {
            request.RequestUrl = $"{defaultRequestUrl}/clear-all";
            request.RequestType = Enums.RequestType.DELETE;
            request.Data = null;

            return await _baseService.SendAsync<Result>(request);
        }
    }
}
