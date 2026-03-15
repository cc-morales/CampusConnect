using ElasticEmail.Api;
using ElasticEmail.Client;
using ElasticEmail.Model;

namespace WebAPI.Services.EmailService
{
    public class ElasticEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<ElasticEmailService> _logger;

        public ElasticEmailService(
            IConfiguration configuration,
            ILogger<ElasticEmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> SendVerificationCodeAsync(string toEmail, string code)
        {
            var apiKey = _configuration["ElasticEmail:APIKey"]!;
            var fromEmail = _configuration["ElasticEmail:From"]!;
            var fromName = _configuration["ElasticEmail:FromName"]!;
            var templateName = _configuration["ElasticEmail:Template"]!;
            
            var digits = code.PadLeft(6, '0');
            
            var config = new Configuration();
            config.ApiKey.Add("X-ElasticEmail-ApiKey", apiKey);

            var emailsApi = new EmailsApi(config);

            var emailData = new EmailMessageData(
                recipients: [new EmailRecipient(toEmail)],
                content: new EmailContent(
                    from: fromEmail,
                    templateName: templateName, 
                    merge: new Dictionary<string, string>
                    {
                        { "digit1", digits[0].ToString() },
                        { "digit2", digits[1].ToString() },
                        { "digit3", digits[2].ToString() },
                        { "digit4", digits[3].ToString() },
                        { "digit5", digits[4].ToString() },
                        { "digit6", digits[5].ToString() }
                    }
                )
            );
            
            try {
                var response = await emailsApi.EmailsPostAsync(emailData);
                
                _logger.LogInformation("Sent verification code email to {Email}. Response: {Response}", toEmail, response);
                return true;
            } catch (Exception e) {
                Console.WriteLine("Exception when calling EmailsApi.EmailsPost: " + e.Message);
                return false;
            }
        }
    }
}

