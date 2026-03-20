using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using WebAPI.ApplicationDBContextService;
using WebAPI.Constants;
using WebAPI.Interfaces;
using WebAPI.Services.EmailService;

namespace WebAPI.Commands.Users.Commands.VerifyEmail
{
    public record VerifyEmailCommand(string Email, string Code) : IRequest<Result>;

    public class VerifyEmailCommandHandler : AppDatabaseBase, IRequestHandler<VerifyEmailCommand, Result>
    {
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly VerificationCodeService _verificationCodeService;
        private readonly ILogger<VerifyEmailCommandHandler> _logger;

        public VerifyEmailCommandHandler(
            AppDbContext context,
            UserManager<ApplicationUserModel> userManager,
            VerificationCodeService verificationCodeService,
            ILogger<VerifyEmailCommandHandler> logger) : base(context)
        {
            _userManager = userManager;
            _verificationCodeService = verificationCodeService;
            _logger = logger;
        }

        public async Task<Result> Handle(VerifyEmailCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(command.Email);
                if (user is null)
                    return StatusCodeErrors.StatusCode(StatusCodes.Status404NotFound, "User not found");

                if (user.EmailConfirmed)
                    return Result.Success(); // Already verified

                if (!_verificationCodeService.Validate(command.Email, command.Code))
                    return StatusCodeErrors.StatusCode(StatusCodes.Status400BadRequest, "Invalid or expired verification code");

                user.EmailConfirmed = true;
                var updateResult = await _userManager.UpdateAsync(user);

                if (!updateResult.Succeeded)
                {
                    var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to confirm email for {Email}. Errors: {Errors}", command.Email, errors);
                    return StatusCodeErrors.StatusCode(StatusCodes.Status500InternalServerError, errors);
                }

                _logger.LogInformation("Email confirmed for {Email}", command.Email);
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying email for {Email}", command.Email);
                return StatusCodeErrors.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}

