using CamCon.Domain.Enitity;
using CamCon.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Camcon.WebAPI.Controllers;
using WebAPI.ApplicationDBContextService;
using WebAPI.Constants;
using WebAPI.Interfaces;
using WebAPI.Services.EmailService;

namespace WebAPI.Commands.Users.Commands.CreateCommand
{
    public class CreateAccountCommandHandler : AppDatabaseBase, IRequestHandler<CreateAccountCommand, Result>
    {
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthController> _logger;
        private readonly VerificationCodeService _verificationCodeService;
        private readonly IEmailService _emailService;

        public CreateAccountCommandHandler(
            UserManager<ApplicationUserModel> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<AuthController> logger,
            AppDbContext context,
            VerificationCodeService verificationCodeService,
            IEmailService emailService) : base(context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _verificationCodeService = verificationCodeService;
            _emailService = emailService;
        }

        public async Task<Result> Handle(CreateAccountCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(command.Request!.Email);
                if (existingUser != null)
                {
                    return UserErrors.UserExist(existingUser.Email!);
                }
                if (await _roleManager.RoleExistsAsync(Roles.User) == false)
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole(Roles.User));

                    if (roleResult.Succeeded == false)
                    {
                        var roleErros = roleResult.Errors.Select(e => e.Description);
                        _logger.LogError($"Failed to create user role. Errors : {string.Join(",", roleErros)}");
                        return UserErrors.FailedToCreateUser(string.Join(",", roleErros));
                    }
                }

                var guid = Guid.NewGuid();
                ApplicationUserModel user = new();
                user.Email = command.Request.Email;
                user.SecurityStamp = Guid.NewGuid().ToString();
                user.UserName = command.Request.Email;
                user.Name = command.Request.Name;
                user.EmailConfirmed = false; // Requires email verification
                user.ProfileInformationId = guid;
                user.ProfileInformation = command.Request.ProfileInformation;
                user.ProfileInformation!.ProfileInformationId = guid;

                if(user.ProfileInformation.MyOrganization is null) return StatusCodeErrors.StatusCode(StatusCodes.Status500InternalServerError, "Organization is null");

                GetDBContext().Entry(user.ProfileInformation.MyOrganization).State = EntityState.Unchanged;

                var createUserResult = await _userManager.CreateAsync(user, command.Request.Password);
                if (createUserResult.Succeeded == false)
                {
                    var errors = createUserResult.Errors.Select(e => e.Description);
                    _logger.LogError($"Failed to create user. Errors: {string.Join(", ", errors)}");
                    return UserErrors.FailedToCreateUser(string.Join(", ", errors));
                }

                var addUserToRoleResult = await _userManager.AddToRoleAsync(user: user, role: Roles.User);

                if (addUserToRoleResult.Succeeded == false)
                {
                    var errors = addUserToRoleResult.Errors.Select(e => e.Description);
                    _logger.LogError($"Failed to add role to the user. Errors : {string.Join(",", errors)}");
                }

                // Generate verification code and send email
                var code = _verificationCodeService.GenerateAndStore(command.Request.Email);
                await _emailService.SendVerificationCodeAsync(command.Request.Email, code);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return StatusCodeErrors.StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
