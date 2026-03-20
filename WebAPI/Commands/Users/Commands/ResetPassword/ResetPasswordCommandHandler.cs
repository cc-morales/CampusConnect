using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using WebAPI.ApplicationDBContextService;

namespace WebAPI.Commands.Users.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
    {
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<ResetPasswordCommandHandler> _logger;

        public ResetPasswordCommandHandler(
            AppDbContext context,
            UserManager<ApplicationUserModel> userManager,
            IMemoryCache memoryCache,
            ILogger<ResetPasswordCommandHandler> logger)
        {
            _userManager = userManager;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            if (request.NewPassword != request.ConfirmPassword)
            {
                return Result.Failure(new Error(StatusCodes.Status400BadRequest, "Passwords do not match"));
            }

            var cacheKey = $"password-reset-{request.Guid}";
            if (!_memoryCache.TryGetValue(cacheKey, out string? email) || string.IsNullOrEmpty(email))
            {
                return Result.Failure(new Error(StatusCodes.Status400BadRequest, "Invalid or expired reset link"));
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return Result.Failure(new Error(StatusCodes.Status404NotFound, "User not found"));
            }

            // Remove old password and set new one
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Failed to reset password for {Email}: {Errors}", email, errors);
                return Result.Failure(new Error(StatusCodes.Status400BadRequest, errors));
            }

            // Remove from cache after successful reset
            _memoryCache.Remove(cacheKey);

            _logger.LogInformation("Password successfully reset for {Email}", email);
            return Result.Success();
        }
    }
}
