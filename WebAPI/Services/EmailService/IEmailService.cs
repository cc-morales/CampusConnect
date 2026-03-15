namespace WebAPI.Services.EmailService
{
    public interface IEmailService
    {
        Task<bool> SendVerificationCodeAsync(string toEmail, string code);
        Task<bool> SendPasswordResetLinkAsync(string toEmail, string resetLink);
    }
}

