using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using WebAPI.ApplicationDBContextService;
using WebAPI.Constants;
using WebAPI.Interfaces;

namespace WebAPI.Commands.Admin.Commands
{
    public record CreateAdminCommand(string Name, string Email, string Password, string[] AccessControl) : IRequest<Result>;

    public class CreateAdminCommandHandler : AppDatabaseBase, IRequestHandler<CreateAdminCommand, Result>
    {
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<CreateAdminCommandHandler> _logger;

        public CreateAdminCommandHandler(
            AppDbContext context,
            UserManager<ApplicationUserModel> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<CreateAdminCommandHandler> logger) : base(context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task<Result> Handle(CreateAdminCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(command.Email);
                if (existingUser != null)
                    return UserErrors.UserExist(command.Email);

                // Ensure Admin role exists
                if (!await _roleManager.RoleExistsAsync(Roles.Admin))
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole(Roles.Admin));
                    if (!roleResult.Succeeded)
                    {
                        var roleErrors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                        _logger.LogError("Failed to create Admin role. Errors: {Errors}", roleErrors);
                        return UserErrors.FailedToCreateUserRoles(roleErrors);
                    }
                }

                var user = new ApplicationUserModel
                {
                    Name = command.Name,
                    Email = command.Email,
                    UserName = command.Email,
                    EmailConfirmed = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    AccessControl = command.AccessControl.Length > 0
                        ? string.Join(",", command.AccessControl)
                        : null
                };

                var createResult = await _userManager.CreateAsync(user, command.Password);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to create admin user. Errors: {Errors}", errors);
                    return UserErrors.FailedToCreateUser(errors);
                }

                var roleAssignResult = await _userManager.AddToRoleAsync(user, Roles.Admin);
                if (!roleAssignResult.Succeeded)
                {
                    var errors = string.Join(", ", roleAssignResult.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to assign Admin role. Errors: {Errors}", errors);
                }

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error creating admin account");
                return StatusCodeErrors.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}

