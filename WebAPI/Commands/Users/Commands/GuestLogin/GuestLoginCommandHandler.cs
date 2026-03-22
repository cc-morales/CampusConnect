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
    public class GuestLoginCommandHandler : AppDatabaseBase, IRequestHandler<GuestLoginCommand, Result<TokenModel>>
    {
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<GuestLoginCommandHandler> _logger;
        private readonly ITokenService _tokenService;

        public GuestLoginCommandHandler(
            AppDbContext context,
            UserManager<ApplicationUserModel> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<GuestLoginCommandHandler> logger,
            ITokenService tokenService) : base(context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _tokenService = tokenService;
        }

        public async Task<Result<TokenModel>> Handle(GuestLoginCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var guestId = Guid.NewGuid().ToString("N")[..8];
                var guestEmail = $"guest_{guestId}@cc.guest";
                var guestPassword = $"Guest@{Guid.NewGuid():N}";

                // Ensure Guest role exists
                if (!await _roleManager.RoleExistsAsync(Roles.Guest))
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole(Roles.Guest));
                    if (!roleResult.Succeeded)
                    {
                        var errors = roleResult.Errors.Select(e => e.Description);
                        _logger.LogError($"Failed to create guest role. Errors: {string.Join(",", errors)}");
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

                var createResult = await _userManager.CreateAsync(user, guestPassword);
                if (!createResult.Succeeded)
                {
                    var errors = createResult.Errors.Select(e => e.Description);
                    _logger.LogError($"Failed to create guest user. Errors: {string.Join(", ", errors)}");
                    return Result.Failure<TokenModel>(StatusCodeErrors.StatusCode(StatusCodes.Status500InternalServerError, "Failed to create guest account."));
                }

                await _userManager.AddToRoleAsync(user, Roles.Guest);

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
                var accessToken = _tokenService.GenerateAccessToken(authClaims);
                var refreshToken = _tokenService.GenerateRefreshToken();

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
                _logger.LogError(ex, "Failed to create guest account");
                return Result.Failure<TokenModel>(StatusCodeErrors.StatusCode(StatusCodes.Status500InternalServerError, ex.Message));
            }
        }
    }
}

