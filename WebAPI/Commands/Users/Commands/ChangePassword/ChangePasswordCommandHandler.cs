using CamCon.Domain.Enitity;
using CamCon.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using WebAPI.ApplicationDBContextService;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Users.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : AppDatabaseBase, IRequestHandler<ChangePasswordCommand, Result>
    {
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly ILogger<ChangePasswordCommandHandler> _logger;

        public ChangePasswordCommandHandler(UserManager<ApplicationUserModel> userManager, ILogger<ChangePasswordCommandHandler> logger, AppDbContext context) : base(context)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<Result> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var req = command.Request;
                if (req is null)
                    return Result.Failure(new Error(StatusCodes.Status400BadRequest, "Invalid request"));

                if (string.IsNullOrWhiteSpace(req.UserId))
                    return Result.Failure(new Error(StatusCodes.Status400BadRequest, "UserId is required"));

                var user = await _userManager.FindByIdAsync(req.UserId);
                if (user is null)
                    return Result.Failure(new Error(StatusCodes.Status404NotFound, "User not found"));

                var identityResult = await _userManager.ChangePasswordAsync(user, req.CurrentPassword, req.NewPassword);
                if (identityResult.Succeeded)
                    return Result.Success();

                var errors = identityResult.Errors.Select(e => e.Description);
                return Result.Failure(new Error(StatusCodes.Status400BadRequest, string.Join("; ", errors)));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Change password failed");
                return Result.Failure(new Error(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
    }
}