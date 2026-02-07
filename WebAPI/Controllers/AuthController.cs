using CamCon.Domain.Enitity;
using CamCon.Shared;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebAPI.ApplicationDBContextService;
using WebAPI.Commands.Users.Commands.ChangePassword;
using WebAPI.Commands.Users.Commands.CreateCommand;
using WebAPI.Commands.Users.Commands.LoginCommand;
using WebAPI.Commands.Users.Queries;
using WebAPI.Services.TokenServices;

namespace MVC.WebAPI.Controllers
{
    [Route("api/auth/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUserModel> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<AuthController> _logger;
        private readonly ITokenService _tokenService;
        private readonly AppDbContext _context;
        private readonly IMediator _mediator;
        public AuthController(UserManager<ApplicationUserModel> userManager,
                              RoleManager<IdentityRole> roleManager,
                              ILogger<AuthController> logger,
                              ITokenService tokenService,
                              AppDbContext appDbContext,
                              IMediator sender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _tokenService = tokenService;
            _context = appDbContext;
            _mediator = sender;
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] CreateAccountCommand createAccountCommand)
        {
            var result = await _mediator.Send(createAccountCommand);
            return Ok(result);
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginCommand loginCommand)
        {
            Result<TokenModel> result = await _mediator.Send(loginCommand);
            if (result.IsSuccess)
                return Ok(result.Value);
            if (result.Error.Code == StatusCodes.Status400BadRequest)
                return BadRequest(result.Error.Description);
            if (result.Error.Code == StatusCodes.Status401Unauthorized)
                return Unauthorized(result.Error.Description);
            return StatusCode(500, result.Error.Description);

        }
        [Authorize]
        [HttpPost("token/refresh")]
        public async Task<IActionResult> Refresh(TokenModel tokenModel)
        {
            try
            {
                var principal = _tokenService.GetPrincipalFromExpiredToken(tokenModel.AccessToken);
                var username = principal.Identity!.Name;

                var tokenInfo = _context.TokenInfos.SingleOrDefault(u => u.Username == username);
                if (tokenInfo == null
                    || tokenInfo.RefreshToken != tokenModel.RefreshToken
                    || tokenInfo.ExpiredAt <= DateTime.UtcNow)
                {
                    return BadRequest("Invalid refresh token. Please login again.");
                }

                var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
                var newRefreshToken = _tokenService.GenerateRefreshToken();

                tokenInfo.RefreshToken = newRefreshToken; // rotating the refresh token
                await _context.SaveChangesAsync();

                return Ok(new TokenModel
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [HttpPost("token/revoke")]
        [Authorize]
        public async Task<IActionResult> Revoke()
        {
            try
            {
                var username = User.Identity!.Name;

                var user = _context.TokenInfos.SingleOrDefault(u => u.Username == username);
                if (user == null)
                {
                    return BadRequest();
                }

                user.RefreshToken = string.Empty;
                await _context.SaveChangesAsync();

                return Ok(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _mediator.Send(new GetAllUsersCommand());

            return Ok(result.Value);
        }

        [HttpPost("create")]
        public async Task<IActionResult> Create([FromBody]CreateAccountCommand command)
        {
            var result = await _mediator.Send(command);

            return Ok(result);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var result = await _mediator.Send(new GetUserByIdQuery(id));

            if(result.IsFailure)
                return StatusCode(result.Error.Code, result.Error.Description);

            return Ok(result.Value);
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsSuccess)
                return Ok(result);
            if (result.Error is not null)
                return StatusCode(result.Error.Code, result.Error.Description);
            return BadRequest();
        }
    }
}
