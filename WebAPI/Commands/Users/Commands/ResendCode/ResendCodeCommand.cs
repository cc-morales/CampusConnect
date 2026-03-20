using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using WebAPI.ApplicationDBContextService;
using WebAPI.Constants;
using WebAPI.Interfaces;
using WebAPI.Services.EmailService;

namespace WebAPI.Commands.Users.Commands.ResendCode
{
    public record ResendCodeCommand(string Email) : IRequest<Result>;

    public class ResendCodeCommandHandler : AppDatabaseBase, IRequestHandler<ResendCodeCommand, Result>
    {
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly VerificationCodeService _verificationCodeService;
        private readonly IEmailService _emailService;
        private readonly ILogger<ResendCodeCommandHandler> _logger;

        public ResendCodeCommandHandler(
            AppDbContext context,
            UserManager<ApplicationUserModel> userManager,
            VerificationCodeService verificationCodeService,
            IEmailService emailService,
            ILogger<ResendCodeCommandHandler> logger) : base(context)
        {
            _userManager = userManager;
            _verificationCodeService = verificationCodeService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<Result> Handle(ResendCodeCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(command.Email);
                if (user is null)
                    return StatusCodeErrors.StatusCode(StatusCodes.Status404NotFound, "User not found");

                if (user.EmailConfirmed)
                    return StatusCodeErrors.StatusCode(StatusCodes.Status400BadRequest, "Email is already verified");

                var code = _verificationCodeService.GenerateAndStore(command.Email);
                var sent = await _emailService.SendVerificationCodeAsync(command.Email, code);

                if (!sent)
                    return StatusCodeErrors.StatusCode(StatusCodes.Status500InternalServerError, "Failed to send verification email");

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending verification code for {Email}", command.Email);
                return StatusCodeErrors.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}

