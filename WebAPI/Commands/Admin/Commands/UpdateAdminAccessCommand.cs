using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using WebAPI.ApplicationDBContextService;
using WebAPI.Constants;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Admin.Commands
{
    public record UpdateAdminAccessCommand(string Id, string[] AccessControl) : IRequest<Result>;

    public class UpdateAdminAccessCommandHandler : AppDatabaseBase, IRequestHandler<UpdateAdminAccessCommand, Result>
    {
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly ILogger<UpdateAdminAccessCommandHandler> _logger;

        public UpdateAdminAccessCommandHandler(
            AppDbContext context,
            UserManager<ApplicationUserModel> userManager,
            ILogger<UpdateAdminAccessCommandHandler> logger) : base(context)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<Result> Handle(UpdateAdminAccessCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(command.Id);
                if (user == null)
                    return StatusCodeErrors.StatusCode(StatusCodes.Status404NotFound, "Admin user not found");

                // Verify user is actually an admin
                if (!await _userManager.IsInRoleAsync(user, Roles.Admin))
                    return StatusCodeErrors.StatusCode(StatusCodes.Status400BadRequest, "User is not an admin");

                user.AccessControl = command.AccessControl.Length > 0
                    ? string.Join(",", command.AccessControl)
                    : null;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to update admin access. Errors: {Errors}", errors);
                    return StatusCodeErrors.StatusCode(StatusCodes.Status500InternalServerError, errors);
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error updating admin access control");
                return StatusCodeErrors.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}

