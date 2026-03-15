using CamCon.Domain.Enitity;
using CamCon.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebAPI.ApplicationDBContextService;
using WebAPI.Constants;
using WebAPI.Interfaces;
using WebAPI.Services.EmailService;
using WebAPI.Services.TokenServices;

namespace WebAPI.Commands.Users.Commands.LoginCommand
{
    public class UserLoginCommandHandler : AppDatabaseBase, IRequestHandler<UserLoginCommand, Result<TokenModel>>
    {
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly ILogger<UserLoginCommandHandler> _logger;
        private readonly ITokenService _tokenService;
        private readonly VerificationCodeService _verificationCodeService;
        private readonly IEmailService _emailService;

        public UserLoginCommandHandler(
            AppDbContext context, 
            UserManager<ApplicationUserModel> userManager,
            ILogger<UserLoginCommandHandler> logger,
            ITokenService tokenService,
            VerificationCodeService verificationCodeService,
            IEmailService emailService) : base(context)
        {
            _userManager = userManager;
            _logger = logger;
            _tokenService = tokenService;
            _verificationCodeService = verificationCodeService;
            _emailService = emailService;
        }

        public async Task<Result<TokenModel>> Handle(UserLoginCommand command, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(command.request.Username);
                if (user == null)
                {
                    return Result.Failure<TokenModel>(UserErrors.Unauthorized());
                }
                bool isValidPassword = await _userManager.CheckPasswordAsync(user, command.request.Password);
                if (isValidPassword == false)
                {
                    return Result.Failure<TokenModel>(UserErrors.Unauthorized());
                }

                // Check if email is verified (only for User role, not Admin)
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains(Roles.User))
                {
                    // Resend verification code if no pending code exists or email not confirmed
                    if (_verificationCodeService.HasPendingCode(user.Email!) || !user.EmailConfirmed)
                    {
                        var code = _verificationCodeService.GenerateAndStore(user.Email!);
                        await _emailService.SendVerificationCodeAsync(user.Email!, code);
                        
                        return Result.Failure<TokenModel>(UserErrors.EmailNotConfirmed());

                    }
                }

                // creating the necessary claims
                List<Claim> authClaims = [ 
                    new (JwtRegisteredClaimNames.Name, user.UserName),
                    new (JwtRegisteredClaimNames.Email, user.Email),
                    new (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new (JwtRegisteredClaimNames.Sub, user.Id)
                    ];

                var userRoles = await _userManager.GetRolesAsync(user);

                // adding roles to the claims. So that we can get the user role from the token.
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                // Add access control permissions claim for admin users
                if (!string.IsNullOrEmpty(user.AccessControl))
                {
                    authClaims.Add(new Claim("access", user.AccessControl));
                }

                //generate access token
                var token = _tokenService.GenerateAccessToken(authClaims);
                //generate refresh token
                string refreshToken = _tokenService.GenerateRefreshToken();

                //save refreshToken with exp date in the database
                var tokenInfo = GetDBContext().TokenInfos.FirstOrDefault(a => a.Username == user.UserName);

                // If tokenInfo is null for the user, create a new one
                if (tokenInfo == null)
                {
                    var ti = new TokenInfoModel
                    {
                        Username = user.UserName,
                        RefreshToken = refreshToken,
                        ExpiredAt = DateTime.UtcNow.AddDays(7)
                    };
                    GetDBContext().TokenInfos.Add(ti);
                }
                // Else, update the refresh token and expiration
                else
                {
                    tokenInfo.RefreshToken = refreshToken;
                    tokenInfo.ExpiredAt = DateTime.UtcNow.AddDays(7);
                }

                await GetDBContext().SaveChangesAsync();

                var tokens = new TokenModel();
                tokens.AccessToken = token;
                tokens.RefreshToken = refreshToken;

                return Result.Success(tokens);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Result.Failure<TokenModel>(UserErrors.Unauthorized());
            }
        }
    }
}
