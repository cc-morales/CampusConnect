using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using WebAPI.ApplicationDBContextService;
using WebAPI.Services.EmailService;

namespace WebAPI.Commands.Users.Commands.RequestPasswordReset
{
    public class RequestPasswordResetCommandHandler : IRequestHandler<RequestPasswordResetCommand, Result>
    {
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<RequestPasswordResetCommandHandler> _logger;

        public RequestPasswordResetCommandHandler(
            AppDbContext context,
            UserManager<ApplicationUserModel> userManager,
            IEmailService emailService,
            IMemoryCache memoryCache,
            ILogger<RequestPasswordResetCommandHandler> logger)
        {
            _userManager = userManager;
            _emailService = emailService;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<Result> Handle(RequestPasswordResetCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                // For security, don't reveal if email exists
                return Result.Success();
            }

            var guid = Guid.NewGuid().ToString();
            var resetUrl = $"{request.BaseUrl.TrimEnd('/')}/passwordreset/{guid}";

            // Store email in cache with GUID as key for 30 minutes
            var cacheKey = $"password-reset-{guid}";
            _memoryCache.Set(cacheKey, request.Email, TimeSpan.FromMinutes(30));

            // Send email
            var emailSent = await _emailService.SendPasswordResetLinkAsync(request.Email, resetUrl);
            if (!emailSent)
            {
                _logger.LogError("Failed to send password reset email to {Email}", request.Email);
                return Result.Failure(new Error(StatusCodes.Status500InternalServerError, "Failed to send password reset email"));
            }

            _logger.LogInformation("Password reset email sent to {Email} with GUID {Guid}", request.Email, guid);
            return Result.Success();
        }
    }
}
