using CamCon.Domain.Entity;
using CamCon.Shared;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using WebAPI.ApplicationDBContextService;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace WebAPI.Commands.Users.Commands.DisableUser
{
	public record DisableUserCommand(string UserId, bool IsDisabled) : IRequest<Result>;

	public class DisableUserCommandHandler : IRequestHandler<DisableUserCommand, Result>
	{
		private readonly UserManager<ApplicationUserModel> _userManager;
		private readonly ILogger<DisableUserCommandHandler> _logger;

		public DisableUserCommandHandler(UserManager<ApplicationUserModel> userManager, ILogger<DisableUserCommandHandler> logger)
		{
			_userManager = userManager;
			_logger = logger;
		}

		public async Task<Result> Handle(DisableUserCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var user = await _userManager.FindByIdAsync(command.UserId);
				if (user is null)
					return Result.Failure(new Error(StatusCodes.Status404NotFound, "User not found"));

				user.IsDisabled = command.IsDisabled;

				var updateResult = await _userManager.UpdateAsync(user);
				if (!updateResult.Succeeded)
				{
					var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
					_logger.LogError("Failed to update user {UserId}. Errors: {Errors}", command.UserId, errors);
					return Result.Failure(new Error(StatusCodes.Status500InternalServerError, errors));
				}

				return Result.Success();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error disabling/enabling user {UserId}", command.UserId);
				return Result.Failure(new Error(StatusCodes.Status500InternalServerError, ex.Message));
			}
		}
	}
}


