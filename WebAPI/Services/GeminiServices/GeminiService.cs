using CamCon.Domain.Enitity;
using System.Text;
using System.Text.Json;

namespace WebAPI.Services.GeminiServices
{
    public class GeminiService : IGeminiService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private string GeminiApiUrl = string.Empty;
        private readonly IConfiguration _configuration;
        public GeminiService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            
            _configuration = configuration;

            _apiKey = _configuration["Gemini:APIKey"] ?? throw new InvalidOperationException("API_KEY not found.");

            GeminiApiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{_configuration["Gemini:Agent"]}:generateContent";
        }

        public async Task<List<Guid>> ModerateCommentsAsync(List<NewsFeedCommentModel> comments, List<string> badWords)
        {
            var dictionary = comments.ToDictionary(c => c.NewsFeedCommentId, c => c.Message);

            if (dictionary.Count <= 0) return new();

            var prompt = $@"
                You are an intelligent content moderation assistant.
                Your task is to identify which comments from the provided JSON list 
                contain any of the words from the ""wordsToFlag"" list.
                The matching must be:
                - case-insensitive
                - whole word only

                List of comments:
                {JsonSerializer.Serialize(dictionary, new JsonSerializerOptions { WriteIndented = true })}

                Words to flag:
                {string.Join(", ", badWords)}

                Additional flag:
                - profanity
                - bad words

                Return ONLY this JSON structure:
                {{
                    ""flaggedCommentIds"": [ <list of NewsFeedCommentId guids> ]
                }}

                Do NOT return any explanations.
                ";

            var payload = new
            {
                contents = new[]
                {
                    new { 
                        parts = new[]
                        { 
                            new 
                            { 
                                text = prompt
                            } 
                        }
                    }
                },
                generationConfig = new
                {
                    response_mime_type = "application/json",
                    response_schema = new
                    {
                        type = "OBJECT",
                        properties = new
                        {
                            flaggedCommentIds = new
                            {
                                type = "ARRAY",
                                items = new { type = "STRING" }
                            }
                        },
                        required = new[] { "flaggedCommentIds" }
                    }
                }
            };

            var requestUri = $"{GeminiApiUrl}?key={_apiKey}";

            var content = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync($"{GeminiApiUrl}?key={_apiKey}", 
                new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Gemini API Fail: {error}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            using var jsonDoc = JsonDocument.Parse(responseBody);

            var textContent =
                jsonDoc.RootElement
                       .GetProperty("candidates")[0]
                       .GetProperty("content")
                       .GetProperty("parts")[0]
                       .GetProperty("text")
                       .GetString();

            var result = JsonSerializer.Deserialize<Dictionary<string, List<Guid>>>(textContent);

            return result["flaggedCommentIds"];
        }
    }
}
