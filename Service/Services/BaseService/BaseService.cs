using Domain;
using Service.Interfaces;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Service.Services.BaseService
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly ITokenProvider _tokenProvider;

        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
        {
            _httpClientFactory = httpClientFactory;
            _tokenProvider = tokenProvider;
        }
        public async Task<T> SendAsync<T>(RequestModel request)
        {
            HttpClient httpClient = _httpClientFactory.CreateClient("WebAPI");

            HttpRequestMessage message = new();
            message.Headers.Add("Accept", "application/json");

            var token = await _tokenProvider.GetToken();

            if(token is not null)
                if (!string.IsNullOrEmpty(token.AccessToken))
                    message.Headers.Add("Authorization", $"Bearer {token.AccessToken}");

            message.RequestUri = new Uri(request.RequestUrl!);

            if (request.Data != null)
                message.Content = new StringContent(JsonSerializer.Serialize(request.Data), Encoding.UTF8, "application/json");

            HttpResponseMessage response;

            message.Method = request.RequestType switch
            {
                Enums.RequestType.GET => HttpMethod.Get,
                Enums.RequestType.POST => HttpMethod.Post,
                Enums.RequestType.PUT => HttpMethod.Put,
                Enums.RequestType.DELETE => HttpMethod.Delete,
                Enums.RequestType.PATCH => HttpMethod.Patch,
                _ => throw new InvalidOperationException("Unsupported request type."),
            };

            response = await httpClient.SendAsync(message);

            if (!response.IsSuccessStatusCode)
                throw new Exception(await response.Content.ReadAsStringAsync());

            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}
