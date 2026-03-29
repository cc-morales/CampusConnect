using CamCon.Shared;
using CamCon.Shared.Extensions;
using Domain;
using Domain.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Service.Cache;
using Service.Interfaces;

namespace Service.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IBaseService _baseService;
        private readonly string defaultRequestUrl;
        private RequestModel request = new();
        private IMemoryCache _memoryCache;

        public UserService(IConfiguration configuration, IBaseService baseService, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _baseService = baseService;
            request.RequestUrl = defaultRequestUrl = $"{_configuration["BaseAPI:Url"]}/api/auth";
            _memoryCache = memoryCache;
        }

        public async Task<TokenModel> Authenticate(LoginModel model)
        {
            request.RequestUrl = $"{defaultRequestUrl}/login";
            request.RequestType = Enums.RequestType.POST;
            request.Data = model.Wrap("request");

            var response = await _baseService.SendAsync<TokenModel>(request);

            return response;
        }

        public async Task<TokenModel> LoginAsGuest()
        {
            request.RequestUrl = $"{defaultRequestUrl}/guest-login";
            request.RequestType = Enums.RequestType.POST;
            request.Data = null;

            var response = await _baseService.SendAsync<TokenModel>(request);

            return response;
        }

        public async Task<List<ApplicationUserModel>> GetAllUsers()
        {
            request.RequestType = Enums.RequestType.GET;
            request.Data = null;
            request.RequestUrl = defaultRequestUrl;

            var response = await _baseService.SendAsync<List<ApplicationUserModel>>(request);

            return response;
        }

        public async Task<Result> CreateUser(ApplicationUserModel model)
        {
            request.RequestUrl = $"{defaultRequestUrl}/create";
            request.RequestType = Enums.RequestType.POST;
            request.Data = model.Wrap("request");

            var response = await _baseService.SendAsync<Result>(request);

            return response;
        }

        public async Task<ApplicationUserModel> GetUserById(string id)
        {
            return await GetUserFromCacheOrAPI(id);
        }

        private async ValueTask<ApplicationUserModel> GetUserFromCacheOrAPI(string id)
        {
            var cacheKey = CacheKeys.UserKey.ByUserId(id);

            var result = _memoryCache.Get<ApplicationUserModel>(cacheKey.Key);

            if (result is not null)
                return result;

            try
            {
                request.RequestUrl = $"{defaultRequestUrl}/{id}";
                request.RequestType = Enums.RequestType.GET;
                request.Data = null;

                result = await _baseService.SendAsync<ApplicationUserModel>(request);

                _memoryCache.Set(cacheKey.Key, result, cacheKey.Duration);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception(ex.Message);
            }

            return result;
        }

        public async Task<Result> ChangePassword(ChangePasswordModel model)
        {
            request.RequestUrl = $"{defaultRequestUrl}/change-password";
            request.RequestType = Enums.RequestType.POST;
            request.Data = model.Wrap("request");

            var response = await _baseService.SendAsync<Result>(request);

            return response;
        }

        public async Task<Result> VerifyEmailAsync(string email, string code)
        {
            request.RequestUrl = $"{defaultRequestUrl}/verify-email";
            request.RequestType = Enums.RequestType.POST;
            request.Data = new { Email = email, Code = code };

            var response = await _baseService.SendAsync<Result>(request);

            return response;
        }

        public async Task<Result> ResendVerificationCodeAsync(string email)
        {
            request.RequestUrl = $"{defaultRequestUrl}/resend-code";
            request.RequestType = Enums.RequestType.POST;
            request.Data = new { Email = email };

            var response = await _baseService.SendAsync<Result>(request);

            return response;
        }

        public async Task<Result> DeleteUserAsync(string userId)
        {
            request.RequestUrl = $"{defaultRequestUrl}/{userId}";
            request.RequestType = Enums.RequestType.DELETE;
            request.Data = null;

            var response = await _baseService.SendAsync<Result>(request);

            return response;
        }

        public async Task<Result> RequestPasswordResetAsync(string email, string baseUrl)
        {
            request.RequestUrl = $"{defaultRequestUrl}/request-password-reset";
            request.RequestType = Enums.RequestType.POST;
            request.Data = new { Email = email, BaseUrl = baseUrl };

            var response = await _baseService.SendAsync<Result>(request);

            return response;
        }

        public async Task<Result> ResetPasswordAsync(string guid, string newPassword, string confirmPassword)
        {
            request.RequestUrl = $"{defaultRequestUrl}/reset-password";
            request.RequestType = Enums.RequestType.POST;
            request.Data = new { Guid = guid, NewPassword = newPassword, ConfirmPassword = confirmPassword };

            var response = await _baseService.SendAsync<Result>(request);

            return response;
        }
    }
}
