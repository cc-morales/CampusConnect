using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebAPI.ApplicationDBContextService;
using WebAPI.Constants;
using WebAPI.Interfaces;
using WebAPI.Services.TokenServices;

namespace WebAPI.Commands.Users.Commands.GuestLogin
{
    public class GuestLoginCommandHandler(
        AppDbContext context,
        UserManager<ApplicationUserModel> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<GuestLoginCommandHandler> logger,
        ITokenService tokenService)
        : AppDatabaseBase(context), IRequestHandler<GuestLoginCommand, Result<TokenModel>>
    {
        public async Task<Result<TokenModel>> Handle(GuestLoginCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var guestId = Guid.NewGuid().ToString("N")[..8];
                var guestEmail = $"guest_{guestId}@cc.guest";
                var guestPassword = $"Guest@{Guid.NewGuid():N}";

                // Ensure Guest role exists
                if (!await roleManager.RoleExistsAsync(Roles.Guest))
                {
                    var roleResult = await roleManager.CreateAsync(new IdentityRole(Roles.Guest));
                    if (!roleResult.Succeeded)
                    {
                        var errors = roleResult.Errors.Select(e => e.Description);
                        logger.LogError($"Failed to create guest role. Errors: {string.Join(",", errors)}");
                        return Result.Failure<TokenModel>(StatusCodeErrors.StatusCode(StatusCodes.Status500InternalServerError, "Failed to create guest role."));
                    }
                }

                var user = new ApplicationUserModel
                {
                    Email = guestEmail,
                    UserName = guestEmail,
                    Name = $"Guest",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    EmailConfirmed = true,
                    IsGuest = true,
                    GuestExpiresAt = DateTime.UtcNow.AddHours(24)
                };

                var createResult = await userManager.CreateAsync(user, guestPassword);
                if (!createResult.Succeeded)
                {
                    var errors = createResult.Errors.Select(e => e.Description);
                    logger.LogError($"Failed to create guest user. Errors: {string.Join(", ", errors)}");
                    return Result.Failure<TokenModel>(StatusCodeErrors.StatusCode(StatusCodes.Status500InternalServerError, "Failed to create guest account."));
                }

                await userManager.AddToRoleAsync(user, Roles.Guest);

                // Create claims
                var authClaims = new List<Claim>
                {
                    new(JwtRegisteredClaimNames.Name, user.UserName),
                    new(JwtRegisteredClaimNames.Email, user.Email),
                    new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new(JwtRegisteredClaimNames.Sub, user.Id),
                    new(ClaimTypes.Role, Roles.Guest)
                };

                // Generate tokens
                var accessToken = tokenService.GenerateAccessToken(authClaims);
                var refreshToken = tokenService.GenerateRefreshToken();

                // Save token info
                var tokenInfo = new TokenInfoModel
                {
                    Username = user.UserName,
                    RefreshToken = refreshToken,
                    ExpiredAt = DateTime.UtcNow.AddHours(24)
                };
                GetDBContext().TokenInfos.Add(tokenInfo);
                await GetDBContext().SaveChangesAsync(cancellationToken);

                return Result.Success(new TokenModel
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to create guest account");
                return Result.Failure<TokenModel>(StatusCodeErrors.StatusCode(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
    }
}

